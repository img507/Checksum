namespace Directory_comparison
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnSelectFolder1 = new Button();
            btnSelectFolder2 = new Button();
            lblFolder1Path = new Label();
            lblFolder2Path = new Label();
            btnCompare = new Button();
            btnCancel = new Button();
            btnExport = new Button();
            lblFilter = new Label();
            txtFilter = new TextBox();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            progressBar = new ToolStripProgressBar();
            contextMenu = new ContextMenuStrip(components);
            menuOpenFolder = new ToolStripMenuItem();
            menuCopyPath = new ToolStripMenuItem();
            menuCopyHashes = new ToolStripMenuItem();
            listViewResults = new FlickerFreeListView();
            statusStrip1.SuspendLayout();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectFolder1
            // 
            btnSelectFolder1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSelectFolder1.Location = new Point(12, 547);
            btnSelectFolder1.Name = "btnSelectFolder1";
            btnSelectFolder1.Size = new Size(150, 40);
            btnSelectFolder1.TabIndex = 0;
            btnSelectFolder1.Text = "Выбрать Папку 1";
            btnSelectFolder1.UseVisualStyleBackColor = true;
            btnSelectFolder1.Click += btnSelectFolder1_Click;
            // 
            // btnSelectFolder2
            // 
            btnSelectFolder2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSelectFolder2.Location = new Point(168, 547);
            btnSelectFolder2.Name = "btnSelectFolder2";
            btnSelectFolder2.Size = new Size(150, 40);
            btnSelectFolder2.TabIndex = 1;
            btnSelectFolder2.Text = "Выбрать Папку 2";
            btnSelectFolder2.UseVisualStyleBackColor = true;
            btnSelectFolder2.Click += btnSelectFolder2_Click;
            // 
            // lblFolder1Path
            // 
            lblFolder1Path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFolder1Path.Location = new Point(12, 14);
            lblFolder1Path.Name = "lblFolder1Path";
            lblFolder1Path.Size = new Size(1178, 23);
            lblFolder1Path.TabIndex = 3;
            lblFolder1Path.Text = "Папка 1 не выбрана";
            // 
            // lblFolder2Path
            // 
            lblFolder2Path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFolder2Path.Location = new Point(12, 41);
            lblFolder2Path.Name = "lblFolder2Path";
            lblFolder2Path.Size = new Size(1178, 23);
            lblFolder2Path.TabIndex = 4;
            lblFolder2Path.Text = "Папка 2 не выбрана";
            // 
            // btnCompare
            // 
            btnCompare.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCompare.Enabled = false;
            btnCompare.Location = new Point(964, 547);
            btnCompare.Name = "btnCompare";
            btnCompare.Size = new Size(226, 40);
            btnCompare.TabIndex = 5;
            btnCompare.Text = "Сравнить";
            btnCompare.UseVisualStyleBackColor = true;
            btnCompare.Click += btnCompare_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(828, 547);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 40);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExport.Location = new Point(324, 547);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(115, 40);
            btnExport.TabIndex = 8;
            btnExport.Text = "Экспорт CSV";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // lblFilter
            // 
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(12, 69);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(141, 15);
            lblFilter.TabIndex = 6;
            lblFilter.Text = "Фильтр файлов (маски):";
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(178, 66);
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(155, 23);
            txtFilter.TabIndex = 7;
            txtFilter.Text = "*.*";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, progressBar });
            statusStrip1.Location = new Point(0, 595);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1202, 22);
            statusStrip1.TabIndex = 10;
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1035, 17);
            lblStatus.Spring = true;
            lblStatus.Text = "Готово к работе";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(150, 16);
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[] { menuOpenFolder, menuCopyPath, menuCopyHashes });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(223, 70);
            // 
            // menuOpenFolder
            // 
            menuOpenFolder.Name = "menuOpenFolder";
            menuOpenFolder.Size = new Size(222, 22);
            menuOpenFolder.Text = "Открыть папку с файлом";
            menuOpenFolder.Click += menuOpenFolder_Click;
            // 
            // menuCopyPath
            // 
            menuCopyPath.Name = "menuCopyPath";
            menuCopyPath.Size = new Size(222, 22);
            menuCopyPath.Text = "Скопировать путь";
            menuCopyPath.Click += menuCopyPath_Click;
            // 
            // menuCopyHashes
            // 
            menuCopyHashes.Name = "menuCopyHashes";
            menuCopyHashes.Size = new Size(222, 22);
            menuCopyHashes.Text = "Скопировать MD5 и CRC32";
            menuCopyHashes.Click += menuCopyHashes_Click;
            // 
            // listViewResults
            // 
            listViewResults.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listViewResults.ContextMenuStrip = contextMenu;
            listViewResults.FullRowSelect = true;
            listViewResults.GridLines = true;
            listViewResults.Location = new Point(12, 98);
            listViewResults.Name = "listViewResults";
            listViewResults.Size = new Size(1178, 439);
            listViewResults.TabIndex = 2;
            listViewResults.UseCompatibleStateImageBehavior = false;
            listViewResults.View = View.Details;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1202, 617);
            Controls.Add(statusStrip1);
            Controls.Add(txtFilter);
            Controls.Add(lblFilter);
            Controls.Add(btnExport);
            Controls.Add(btnCancel);
            Controls.Add(btnCompare);
            Controls.Add(lblFolder2Path);
            Controls.Add(lblFolder1Path);
            Controls.Add(listViewResults);
            Controls.Add(btnSelectFolder2);
            Controls.Add(btnSelectFolder1);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 400);
            Name = "Form1";
            Text = "Умное сравнение папок";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFolder1;
        private System.Windows.Forms.Button btnSelectFolder2;
        private Directory_comparison.FlickerFreeListView listViewResults;
        private System.Windows.Forms.Label lblFolder1Path;
        private System.Windows.Forms.Label lblFolder2Path;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuOpenFolder;
        private System.Windows.Forms.ToolStripMenuItem menuCopyPath;
        private System.Windows.Forms.ToolStripMenuItem menuCopyHashes;
    }
}