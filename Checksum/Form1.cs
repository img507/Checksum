using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checksum
{
    public partial class Form1 : Form
    {
        private string? folder1Path;
        private string? folder2Path;
        private int _sortColumnIndex = -1;
        private CancellationTokenSource? _cts;

        public Form1()
        {
            InitializeComponent();
            SetupListView();
        }

        private void SetupListView()
        {
            listViewResults.Columns.Add("Относительный путь", 400);
            listViewResults.Columns.Add("Базовая папка", 150);
            listViewResults.Columns.Add("MD5", 220);
            listViewResults.Columns.Add("CRC32", 80);
            listViewResults.Columns.Add("Размер (байт)", 100);
            listViewResults.Columns.Add("Статус", 150);

            listViewResults.ColumnClick += ListViewResults_ColumnClick;
            listViewResults.DoubleClick += ListViewResults_DoubleClick;
        }

        #region Кнопки выбора папок
        private void btnSelectFolder1_Click(object sender, EventArgs e)
        {
            var path = SelectFolder("Выберите первую папку для сравнения");
            if (path != null)
            {
                folder1Path = path;
                lblFolder1Path.Text = folder1Path;
                CheckPathsAndEnableButton();
            }
        }

        private void btnSelectFolder2_Click(object sender, EventArgs e)
        {
            var path = SelectFolder("Выберите вторую папку для сравнения");
            if (path != null)
            {
                folder2Path = path;
                lblFolder2Path.Text = folder2Path;
                CheckPathsAndEnableButton();
            }
        }

        private string? SelectFolder(string title)
        {
            var picker = new FolderPicker
            {
                Title = title,
                ForceFileSystem = true,
                Multiselect = false,
                InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            return picker.ShowDialog(this.Handle) == true ? picker.ResultPath : null;
        }

        private void CheckPathsAndEnableButton()
        {
            btnCompare.Enabled = !string.IsNullOrEmpty(folder1Path) && !string.IsNullOrEmpty(folder2Path);
        }
        #endregion

        #region Основная кнопка Сравнить и Отмена
        private async void btnCompare_Click(object sender, EventArgs e)
        {
            btnCompare.Enabled = false;
            btnCancel.Enabled = true;
            listViewResults.Items.Clear();
            listViewResults.ListViewItemSorter = null;
            _sortColumnIndex = -1;

            _cts = new CancellationTokenSource();

            var progress = new Progress<ProgressData>(data =>
            {
                lblStatus.Text = data.Message;
                if (data.Maximum > 0)
                {
                    progressBar.Maximum = data.Maximum;
                    progressBar.Value = Math.Min(data.CurrentValue, data.Maximum);
                }
            });

            try
            {
                var resultsData = await Task.Run(() => PerformComparison(txtFilter.Text, progress, _cts.Token));

                lblStatus.Text = "Отрисовка таблицы...";
                listViewResults.BeginUpdate();

                var itemsToAdd = new List<ListViewItem>();
                foreach (var data in resultsData)
                {
                    var item = new ListViewItem(new[] {
                        data.Key, data.FolderName, data.MD5, data.CRC32, data.Size.ToString(), data.Status
                    })
                    {
                        BackColor = data.RowColor,
                        Tag = data.FullPath
                    };
                    itemsToAdd.Add(item);
                }

                listViewResults.Items.AddRange(itemsToAdd.ToArray());
                listViewResults.EndUpdate();

                lblStatus.Text = $"Готово! Обраработано файлов: {resultsData.Count}";
                progressBar.Value = 0;
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Операция отменена пользователем.";
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка выполнения.";
            }
            finally
            {
                btnCompare.Enabled = true;
                btnCancel.Enabled = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                lblStatus.Text = "Отмена операции...";
                btnCancel.Enabled = false;
                _cts.Cancel();
            }
        }
        #endregion

        #region Логика обработки
        public class ProgressData
        {
            public string Message { get; set; } = string.Empty;
            public int CurrentValue { get; set; }
            public int Maximum { get; set; }
        }

        private class FileCompareResult
        {
            public string Key { get; set; } = string.Empty;
            public string FolderName { get; set; } = string.Empty;
            public string FullPath { get; set; } = string.Empty;
            public string MD5 { get; set; } = string.Empty;
            public string CRC32 { get; set; } = string.Empty;
            public long Size { get; set; }
            public string Status { get; set; } = string.Empty;
            public Color RowColor { get; set; }
        }

        private List<FileCompareResult> PerformComparison(string filter, IProgress<ProgressData> progress, CancellationToken token)
        {
            var results = new ConcurrentBag<FileCompareResult>();

            progress?.Report(new ProgressData { Message = "Сканирование папки 1..." });
            var files1 = GetAllFiles(folder1Path!, filter);

            progress?.Report(new ProgressData { Message = "Сканирование папки 2..." });
            var files2 = GetAllFiles(folder2Path!, filter);

            progress?.Report(new ProgressData { Message = "Анализ списков файлов..." });
            var comparer = StringComparer.OrdinalIgnoreCase;
            var commonKeys = files1.Keys.Intersect(files2.Keys).OrderBy(k => k, comparer).ToList();
            var uniqueTo1 = files1.Keys.Except(files2.Keys).OrderBy(k => k, comparer).ToList();
            var uniqueTo2 = files2.Keys.Except(files1.Keys).OrderBy(k => k, comparer).ToList();

            string folder1Name = new DirectoryInfo(folder1Path!).Name;
            string folder2Name = new DirectoryInfo(folder2Path!).Name;

            int totalFiles = commonKeys.Count + uniqueTo1.Count + uniqueTo2.Count;
            int processed = 0;

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = token };

            Parallel.ForEach(commonKeys, parallelOptions, key =>
            {
                var file1 = files1[key];
                var file2 = files2[key];

                var hash1 = CalculateHashes(file1.FullName, token);
                var hash2 = CalculateHashes(file2.FullName, token);

                bool sizesDiffer = file1.Length != file2.Length;
                bool hashesEqual = hash1.md5 == hash2.md5 && hash1.crc32 == hash2.crc32;

                string status = sizesDiffer ? "❌ Разные размеры" : (hashesEqual ? "✅ Совпадают" : "❌ Разные хеши");
                Color color = (!sizesDiffer && hashesEqual) ? Color.LightGreen : Color.LightCoral;

                results.Add(new FileCompareResult { Key = key, FolderName = folder1Name, FullPath = file1.FullName, MD5 = hash1.md5, CRC32 = hash1.crc32, Size = file1.Length, Status = status, RowColor = color });
                results.Add(new FileCompareResult { Key = key, FolderName = folder2Name, FullPath = file2.FullName, MD5 = hash2.md5, CRC32 = hash2.crc32, Size = file2.Length, Status = status, RowColor = color });

                ReportProgress(progress, ref processed, totalFiles);
            });

            Parallel.ForEach(uniqueTo1, parallelOptions, key =>
            {
                var file = files1[key];
                var hash = CalculateHashes(file.FullName, token);
                results.Add(new FileCompareResult { Key = key, FolderName = folder1Name, FullPath = file.FullName, MD5 = hash.md5, CRC32 = hash.crc32, Size = file.Length, Status = $"Только в '{folder1Name}'", RowColor = Color.LightGray });
                ReportProgress(progress, ref processed, totalFiles);
            });

            Parallel.ForEach(uniqueTo2, parallelOptions, key =>
            {
                var file = files2[key];
                var hash = CalculateHashes(file.FullName, token);
                results.Add(new FileCompareResult { Key = key, FolderName = folder2Name, FullPath = file.FullName, MD5 = hash.md5, CRC32 = hash.crc32, Size = file.Length, Status = $"Только в '{folder2Name}'", RowColor = Color.LightGray });
                ReportProgress(progress, ref processed, totalFiles);
            });

            return results.OrderBy(x => x.Key).ThenBy(x => x.FolderName).ToList();
        }

        private void ReportProgress(IProgress<ProgressData>? progress, ref int processed, int total)
        {
            int current = Interlocked.Increment(ref processed);
            if (current % 25 == 0 || current == total)
            {
                progress?.Report(new ProgressData { Message = $"Анализ файлов: {current} из {total}", CurrentValue = current, Maximum = total });
            }
        }

        private Dictionary<string, FileInfo> GetAllFiles(string basePath, string filterStr)
        {
            var dict = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
            var patterns = string.IsNullOrWhiteSpace(filterStr) ? new[] { "*.*" } : filterStr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());

            foreach (var pattern in patterns)
            {
                try
                {
                    var files = Directory.EnumerateFiles(basePath, pattern, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var rel = file.Substring(basePath.Length + 1);
                        if (!dict.ContainsKey(rel)) dict[rel] = new FileInfo(file);
                    }
                }
                catch (UnauthorizedAccessException) { }
            }
            return dict;
        }

        private (string md5, string crc32) CalculateHashes(string filePath, CancellationToken token)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);

            try
            {
                using SafeFileHandle handle = File.OpenHandle(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.SequentialScan);
                long fileLength = RandomAccess.GetLength(handle);
                long offset = 0;

                using var md5Hash = IncrementalHash.CreateHash(HashAlgorithmName.MD5);

                var crc32Hash = new System.IO.Hashing.Crc32();

                while (offset < fileLength)
                {
                    token.ThrowIfCancellationRequested();

                    int bytesToRead = (int)Math.Min(buffer.Length, fileLength - offset);
                    Span<byte> span = buffer.AsSpan(0, bytesToRead);

                    int bytesRead = RandomAccess.Read(handle, span, offset);
                    if (bytesRead == 0) break;

                    var activeSpan = span.Slice(0, bytesRead);

                    md5Hash.AppendData(activeSpan);
                    crc32Hash.Append(activeSpan);

                    offset += bytesRead;
                }

                Span<byte> md5Bytes = stackalloc byte[16];
                md5Hash.TryGetHashAndReset(md5Bytes, out _);
                string md5Hex = Convert.ToHexString(md5Bytes).ToLowerInvariant();

                Span<byte> crc32Bytes = stackalloc byte[4];
                crc32Hash.GetCurrentHash(crc32Bytes);
                crc32Bytes.Reverse();
                string crc32Hex = Convert.ToHexString(crc32Bytes).ToLowerInvariant();

                return (md5Hex, crc32Hex);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception)
            {
                return ("Ошибка чтения", "Ошибка чтения");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        #endregion

        #region События контекстного меню и UI элементов
        private void ListViewResults_DoubleClick(object? sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var path = listViewResults.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                }
            }
        }

        private void menuOpenFolder_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var path = listViewResults.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(path))
                {
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
                }
            }
        }

        private void menuCopyPath_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var path = listViewResults.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(path)) Clipboard.SetText(path);
            }
        }

        private void menuCopyHashes_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var item = listViewResults.SelectedItems[0];
                var text = $"MD5: {item.SubItems[2].Text} | CRC32: {item.SubItems[3].Text}";
                Clipboard.SetText(text);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (listViewResults.Items.Count == 0) return;

            using (var sfd = new SaveFileDialog() { Filter = "CSV Файл (*.csv)|*.csv", FileName = "Отчет.csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Относительный путь;Базовая папка;MD5;CRC32;Размер;Статус");
                    foreach (ListViewItem item in listViewResults.Items)
                    {
                        sb.AppendLine($"{item.SubItems[0].Text};{item.SubItems[1].Text};{item.SubItems[2].Text};{item.SubItems[3].Text};{item.SubItems[4].Text};{item.SubItems[5].Text}");
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Успешно сохранено!", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region Сортировка ListView
        private void ListViewResults_ColumnClick(object? sender, ColumnClickEventArgs e)
        {
            if (e.Column == _sortColumnIndex)
                this.listViewResults.Sorting = this.listViewResults.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                _sortColumnIndex = e.Column;
                this.listViewResults.Sorting = SortOrder.Ascending;
            }

            this.listViewResults.ListViewItemSorter = new ListViewItemComparer(e.Column, this.listViewResults.Sorting);
            this.listViewResults.Sort();
        }

        public class ListViewItemComparer : IComparer
        {
            private int _col;
            private SortOrder _order;
            public ListViewItemComparer(int column, SortOrder order) { _col = column; _order = order; }

            public int Compare(object? x, object? y)
            {
                if (x is not ListViewItem itemX || y is not ListViewItem itemY)
                    return 0;

                int returnVal = -1;
                var textX = itemX.SubItems[_col].Text;
                var textY = itemY.SubItems[_col].Text;

                if (_col == 4)
                {
                    long.TryParse(textX, out long numX);
                    long.TryParse(textY, out long numY);
                    returnVal = numX.CompareTo(numY);
                }
                else
                {
                    returnVal = String.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
                }

                return _order == SortOrder.Descending ? -returnVal : returnVal;
            }
        }
        #endregion
    }
}