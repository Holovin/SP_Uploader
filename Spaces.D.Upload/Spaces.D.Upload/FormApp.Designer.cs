namespace SpacesDUpload {
  partial class FormApp {
    /// <summary>
    /// Требуется переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Обязательный метод для поддержки конструктора - не изменяйте
    /// содержимое данного метода при помощи редактора кода.
    /// </summary>
    private void InitializeComponent() {
      this.AppTabControl = new System.Windows.Forms.TabControl();
      this.AppTabPageAuth = new System.Windows.Forms.TabPage();
      this.GroupBoxAuth = new System.Windows.Forms.GroupBox();
      this.ButtonDebug = new System.Windows.Forms.Button();
      this.ButtonAuth = new System.Windows.Forms.Button();
      this.AppTabPageUploader = new System.Windows.Forms.TabPage();
      this.GroupBoxSpacDirs = new System.Windows.Forms.GroupBox();
      this.ButtonUpload = new System.Windows.Forms.Button();
      this.LabelSelectDirHelp = new System.Windows.Forms.Label();
      this.ListViewDirs = new System.Windows.Forms.ListView();
      this.СolumnHeaderID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColumnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.GroupBoxFiles = new System.Windows.Forms.GroupBox();
      this.ButtonFilesClear = new System.Windows.Forms.Button();
      this.LabelFilesInfo = new System.Windows.Forms.Label();
      this.ButtonAddDirectory = new System.Windows.Forms.Button();
      this.ButtonAddFiles = new System.Windows.Forms.Button();
      this.ListBoxFiles = new System.Windows.Forms.ListBox();
      this.AppTabPageProgress = new System.Windows.Forms.TabPage();
      this.GroupBoxProgress = new System.Windows.Forms.GroupBox();
      this.TextBoxUploaderLog = new System.Windows.Forms.TextBox();
      this.LabelCurrentFile = new System.Windows.Forms.Label();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.AppTabPageAbout = new System.Windows.Forms.TabPage();
      this.GroupBoxUpdate = new System.Windows.Forms.GroupBox();
      this.LabelVersion = new System.Windows.Forms.Label();
      this.ButtonUpdateCheck = new System.Windows.Forms.Button();
      this.GroupBoxAbout = new System.Windows.Forms.GroupBox();
      this.LabelAuthor = new System.Windows.Forms.LinkLabel();
      this.LabelAbout = new System.Windows.Forms.Label();
      this.AppTabControl.SuspendLayout();
      this.AppTabPageAuth.SuspendLayout();
      this.GroupBoxAuth.SuspendLayout();
      this.AppTabPageUploader.SuspendLayout();
      this.GroupBoxSpacDirs.SuspendLayout();
      this.GroupBoxFiles.SuspendLayout();
      this.AppTabPageProgress.SuspendLayout();
      this.GroupBoxProgress.SuspendLayout();
      this.AppTabPageAbout.SuspendLayout();
      this.GroupBoxUpdate.SuspendLayout();
      this.GroupBoxAbout.SuspendLayout();
      this.SuspendLayout();
      // 
      // AppTabControl
      // 
      this.AppTabControl.Controls.Add(this.AppTabPageAuth);
      this.AppTabControl.Controls.Add(this.AppTabPageUploader);
      this.AppTabControl.Controls.Add(this.AppTabPageProgress);
      this.AppTabControl.Controls.Add(this.AppTabPageAbout);
      this.AppTabControl.Location = new System.Drawing.Point(12, 12);
      this.AppTabControl.Name = "AppTabControl";
      this.AppTabControl.SelectedIndex = 0;
      this.AppTabControl.Size = new System.Drawing.Size(660, 338);
      this.AppTabControl.TabIndex = 0;
      // 
      // AppTabPageAuth
      // 
      this.AppTabPageAuth.Controls.Add(this.GroupBoxAuth);
      this.AppTabPageAuth.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageAuth.Name = "AppTabPageAuth";
      this.AppTabPageAuth.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPageAuth.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageAuth.TabIndex = 0;
      this.AppTabPageAuth.Text = "Авторизация";
      this.AppTabPageAuth.UseVisualStyleBackColor = true;
      // 
      // GroupBoxAuth
      // 
      this.GroupBoxAuth.Controls.Add(this.ButtonDebug);
      this.GroupBoxAuth.Controls.Add(this.ButtonAuth);
      this.GroupBoxAuth.Location = new System.Drawing.Point(6, 6);
      this.GroupBoxAuth.Name = "GroupBoxAuth";
      this.GroupBoxAuth.Size = new System.Drawing.Size(640, 300);
      this.GroupBoxAuth.TabIndex = 1;
      this.GroupBoxAuth.TabStop = false;
      this.GroupBoxAuth.Text = "Авторизация";
      // 
      // ButtonDebug
      // 
      this.ButtonDebug.Location = new System.Drawing.Point(535, 252);
      this.ButtonDebug.Name = "ButtonDebug";
      this.ButtonDebug.Size = new System.Drawing.Size(99, 42);
      this.ButtonDebug.TabIndex = 1;
      this.ButtonDebug.Text = "[Debug]";
      this.ButtonDebug.UseVisualStyleBackColor = true;
      this.ButtonDebug.Click += new System.EventHandler(this.ButtonDebug_Click);
      // 
      // ButtonAuth
      // 
      this.ButtonAuth.Location = new System.Drawing.Point(20, 33);
      this.ButtonAuth.Name = "ButtonAuth";
      this.ButtonAuth.Size = new System.Drawing.Size(99, 38);
      this.ButtonAuth.TabIndex = 0;
      this.ButtonAuth.Text = "Войти";
      this.ButtonAuth.UseVisualStyleBackColor = true;
      this.ButtonAuth.Click += new System.EventHandler(this.ButtonAuth_Click);
      // 
      // AppTabPageUploader
      // 
      this.AppTabPageUploader.Controls.Add(this.GroupBoxSpacDirs);
      this.AppTabPageUploader.Controls.Add(this.GroupBoxFiles);
      this.AppTabPageUploader.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageUploader.Name = "AppTabPageUploader";
      this.AppTabPageUploader.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageUploader.TabIndex = 2;
      this.AppTabPageUploader.Text = "Выбор файлов";
      this.AppTabPageUploader.UseVisualStyleBackColor = true;
      // 
      // GroupBoxSpacDirs
      // 
      this.GroupBoxSpacDirs.Controls.Add(this.ButtonUpload);
      this.GroupBoxSpacDirs.Controls.Add(this.LabelSelectDirHelp);
      this.GroupBoxSpacDirs.Controls.Add(this.ListViewDirs);
      this.GroupBoxSpacDirs.Location = new System.Drawing.Point(396, 3);
      this.GroupBoxSpacDirs.Name = "GroupBoxSpacDirs";
      this.GroupBoxSpacDirs.Size = new System.Drawing.Size(253, 306);
      this.GroupBoxSpacDirs.TabIndex = 2;
      this.GroupBoxSpacDirs.TabStop = false;
      this.GroupBoxSpacDirs.Text = "Выбор папки на Spaces";
      // 
      // ButtonUpload
      // 
      this.ButtonUpload.Location = new System.Drawing.Point(6, 248);
      this.ButtonUpload.Name = "ButtonUpload";
      this.ButtonUpload.Size = new System.Drawing.Size(241, 52);
      this.ButtonUpload.TabIndex = 4;
      this.ButtonUpload.Text = "Начать загрузку!";
      this.ButtonUpload.UseVisualStyleBackColor = true;
      this.ButtonUpload.Click += new System.EventHandler(this.ButtonUpload_Click);
      // 
      // LabelSelectDirHelp
      // 
      this.LabelSelectDirHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.LabelSelectDirHelp.Location = new System.Drawing.Point(6, 221);
      this.LabelSelectDirHelp.Name = "LabelSelectDirHelp";
      this.LabelSelectDirHelp.Size = new System.Drawing.Size(241, 24);
      this.LabelSelectDirHelp.TabIndex = 3;
      this.LabelSelectDirHelp.Text = "* Двойной клик для входа в папку. Для выбора папки просто оставьте её выделенной." +
    "";
      // 
      // ListViewDirs
      // 
      this.ListViewDirs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.СolumnHeaderID,
            this.ColumnHeaderName});
      this.ListViewDirs.FullRowSelect = true;
      this.ListViewDirs.GridLines = true;
      this.ListViewDirs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.ListViewDirs.LabelWrap = false;
      this.ListViewDirs.Location = new System.Drawing.Point(6, 19);
      this.ListViewDirs.MultiSelect = false;
      this.ListViewDirs.Name = "ListViewDirs";
      this.ListViewDirs.ShowGroups = false;
      this.ListViewDirs.Size = new System.Drawing.Size(241, 199);
      this.ListViewDirs.TabIndex = 2;
      this.ListViewDirs.UseCompatibleStateImageBehavior = false;
      this.ListViewDirs.View = System.Windows.Forms.View.Details;
      this.ListViewDirs.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListViewDirs_MouseDoubleClick);
      // 
      // СolumnHeaderID
      // 
      this.СolumnHeaderID.Text = "ID";
      // 
      // ColumnHeaderName
      // 
      this.ColumnHeaderName.Text = "Имя";
      this.ColumnHeaderName.Width = 175;
      // 
      // GroupBoxFiles
      // 
      this.GroupBoxFiles.Controls.Add(this.ButtonFilesClear);
      this.GroupBoxFiles.Controls.Add(this.LabelFilesInfo);
      this.GroupBoxFiles.Controls.Add(this.ButtonAddDirectory);
      this.GroupBoxFiles.Controls.Add(this.ButtonAddFiles);
      this.GroupBoxFiles.Controls.Add(this.ListBoxFiles);
      this.GroupBoxFiles.Location = new System.Drawing.Point(3, 3);
      this.GroupBoxFiles.Name = "GroupBoxFiles";
      this.GroupBoxFiles.Size = new System.Drawing.Size(387, 306);
      this.GroupBoxFiles.TabIndex = 0;
      this.GroupBoxFiles.TabStop = false;
      this.GroupBoxFiles.Text = "Файлы для загрузки";
      // 
      // ButtonFilesClear
      // 
      this.ButtonFilesClear.Location = new System.Drawing.Point(6, 277);
      this.ButtonFilesClear.Name = "ButtonFilesClear";
      this.ButtonFilesClear.Size = new System.Drawing.Size(114, 23);
      this.ButtonFilesClear.TabIndex = 4;
      this.ButtonFilesClear.Text = "Очистить список";
      this.ButtonFilesClear.UseVisualStyleBackColor = true;
      this.ButtonFilesClear.Click += new System.EventHandler(this.ButtonFilesClear_Click);
      // 
      // LabelFilesInfo
      // 
      this.LabelFilesInfo.Location = new System.Drawing.Point(7, 225);
      this.LabelFilesInfo.Name = "LabelFilesInfo";
      this.LabelFilesInfo.Size = new System.Drawing.Size(374, 20);
      this.LabelFilesInfo.TabIndex = 3;
      this.LabelFilesInfo.Text = "Файлов: 0 | Общий размер: 0 МБ";
      // 
      // ButtonAddDirectory
      // 
      this.ButtonAddDirectory.Location = new System.Drawing.Point(126, 248);
      this.ButtonAddDirectory.Name = "ButtonAddDirectory";
      this.ButtonAddDirectory.Size = new System.Drawing.Size(114, 23);
      this.ButtonAddDirectory.TabIndex = 2;
      this.ButtonAddDirectory.Text = "Добавить папку";
      this.ButtonAddDirectory.UseVisualStyleBackColor = true;
      this.ButtonAddDirectory.Click += new System.EventHandler(this.ButtonAddDirectory_Click);
      // 
      // ButtonAddFiles
      // 
      this.ButtonAddFiles.Location = new System.Drawing.Point(6, 248);
      this.ButtonAddFiles.Name = "ButtonAddFiles";
      this.ButtonAddFiles.Size = new System.Drawing.Size(114, 23);
      this.ButtonAddFiles.TabIndex = 1;
      this.ButtonAddFiles.Text = "Добавить файлы";
      this.ButtonAddFiles.UseVisualStyleBackColor = true;
      this.ButtonAddFiles.Click += new System.EventHandler(this.ButtonAddFiles_Click);
      // 
      // ListBoxFiles
      // 
      this.ListBoxFiles.FormattingEnabled = true;
      this.ListBoxFiles.HorizontalScrollbar = true;
      this.ListBoxFiles.Location = new System.Drawing.Point(6, 19);
      this.ListBoxFiles.Name = "ListBoxFiles";
      this.ListBoxFiles.Size = new System.Drawing.Size(375, 199);
      this.ListBoxFiles.TabIndex = 0;
      // 
      // AppTabPageProgress
      // 
      this.AppTabPageProgress.Controls.Add(this.GroupBoxProgress);
      this.AppTabPageProgress.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageProgress.Name = "AppTabPageProgress";
      this.AppTabPageProgress.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPageProgress.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageProgress.TabIndex = 3;
      this.AppTabPageProgress.Text = "Загрузка";
      this.AppTabPageProgress.UseVisualStyleBackColor = true;
      // 
      // GroupBoxProgress
      // 
      this.GroupBoxProgress.Controls.Add(this.TextBoxUploaderLog);
      this.GroupBoxProgress.Controls.Add(this.LabelCurrentFile);
      this.GroupBoxProgress.Controls.Add(this.progressBar1);
      this.GroupBoxProgress.Location = new System.Drawing.Point(6, 6);
      this.GroupBoxProgress.Name = "GroupBoxProgress";
      this.GroupBoxProgress.Size = new System.Drawing.Size(640, 300);
      this.GroupBoxProgress.TabIndex = 0;
      this.GroupBoxProgress.TabStop = false;
      // 
      // TextBoxUploaderLog
      // 
      this.TextBoxUploaderLog.Location = new System.Drawing.Point(385, 10);
      this.TextBoxUploaderLog.Multiline = true;
      this.TextBoxUploaderLog.Name = "TextBoxUploaderLog";
      this.TextBoxUploaderLog.Size = new System.Drawing.Size(249, 284);
      this.TextBoxUploaderLog.TabIndex = 2;
      this.TextBoxUploaderLog.Text = "Отчёт...";
      // 
      // LabelCurrentFile
      // 
      this.LabelCurrentFile.Location = new System.Drawing.Point(17, 63);
      this.LabelCurrentFile.Name = "LabelCurrentFile";
      this.LabelCurrentFile.Size = new System.Drawing.Size(607, 23);
      this.LabelCurrentFile.TabIndex = 1;
      this.LabelCurrentFile.Text = "Текущий файл: n/a";
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(33, 89);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(330, 23);
      this.progressBar1.TabIndex = 0;
      // 
      // AppTabPageAbout
      // 
      this.AppTabPageAbout.Controls.Add(this.GroupBoxUpdate);
      this.AppTabPageAbout.Controls.Add(this.GroupBoxAbout);
      this.AppTabPageAbout.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageAbout.Name = "AppTabPageAbout";
      this.AppTabPageAbout.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPageAbout.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageAbout.TabIndex = 1;
      this.AppTabPageAbout.Text = "О программе";
      this.AppTabPageAbout.UseVisualStyleBackColor = true;
      // 
      // GroupBoxUpdate
      // 
      this.GroupBoxUpdate.Controls.Add(this.LabelVersion);
      this.GroupBoxUpdate.Controls.Add(this.ButtonUpdateCheck);
      this.GroupBoxUpdate.Location = new System.Drawing.Point(411, 6);
      this.GroupBoxUpdate.Name = "GroupBoxUpdate";
      this.GroupBoxUpdate.Size = new System.Drawing.Size(235, 179);
      this.GroupBoxUpdate.TabIndex = 5;
      this.GroupBoxUpdate.TabStop = false;
      this.GroupBoxUpdate.Text = "Обновления";
      // 
      // LabelVersion
      // 
      this.LabelVersion.Location = new System.Drawing.Point(6, 16);
      this.LabelVersion.Name = "LabelVersion";
      this.LabelVersion.Size = new System.Drawing.Size(156, 30);
      this.LabelVersion.TabIndex = 2;
      this.LabelVersion.Text = "Текущая версия: 01234\r\nАктуальная версия: 12345\r\n\r\n";
      // 
      // ButtonUpdateCheck
      // 
      this.ButtonUpdateCheck.Location = new System.Drawing.Point(9, 51);
      this.ButtonUpdateCheck.Name = "ButtonUpdateCheck";
      this.ButtonUpdateCheck.Size = new System.Drawing.Size(153, 27);
      this.ButtonUpdateCheck.TabIndex = 1;
      this.ButtonUpdateCheck.Text = "Проверить обновления";
      this.ButtonUpdateCheck.UseVisualStyleBackColor = false;
      this.ButtonUpdateCheck.Click += new System.EventHandler(this.ButtonUpdateCheck_Click);
      // 
      // GroupBoxAbout
      // 
      this.GroupBoxAbout.Controls.Add(this.LabelAuthor);
      this.GroupBoxAbout.Controls.Add(this.LabelAbout);
      this.GroupBoxAbout.Location = new System.Drawing.Point(6, 6);
      this.GroupBoxAbout.Name = "GroupBoxAbout";
      this.GroupBoxAbout.Size = new System.Drawing.Size(399, 179);
      this.GroupBoxAbout.TabIndex = 4;
      this.GroupBoxAbout.TabStop = false;
      this.GroupBoxAbout.Text = "О...";
      // 
      // LabelAuthor
      // 
      this.LabelAuthor.Location = new System.Drawing.Point(6, 51);
      this.LabelAuthor.Name = "LabelAuthor";
      this.LabelAuthor.Size = new System.Drawing.Size(171, 23);
      this.LabelAuthor.TabIndex = 3;
      this.LabelAuthor.TabStop = true;
      this.LabelAuthor.Text = "by DJ_miXxXer";
      this.LabelAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LabelAuthor_LinkClicked);
      // 
      // LabelAbout
      // 
      this.LabelAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.LabelAbout.Location = new System.Drawing.Point(5, 16);
      this.LabelAbout.Name = "LabelAbout";
      this.LabelAbout.Size = new System.Drawing.Size(388, 29);
      this.LabelAbout.TabIndex = 0;
      this.LabelAbout.Text = "Spaces.D.Uploader";
      // 
      // FormApp
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(684, 362);
      this.Controls.Add(this.AppTabControl);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "FormApp";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Spaces.D.Uploader";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormApp_FormClosed);
      this.Shown += new System.EventHandler(this.FormApp_Shown);
      this.AppTabControl.ResumeLayout(false);
      this.AppTabPageAuth.ResumeLayout(false);
      this.GroupBoxAuth.ResumeLayout(false);
      this.AppTabPageUploader.ResumeLayout(false);
      this.GroupBoxSpacDirs.ResumeLayout(false);
      this.GroupBoxFiles.ResumeLayout(false);
      this.AppTabPageProgress.ResumeLayout(false);
      this.GroupBoxProgress.ResumeLayout(false);
      this.GroupBoxProgress.PerformLayout();
      this.AppTabPageAbout.ResumeLayout(false);
      this.GroupBoxUpdate.ResumeLayout(false);
      this.GroupBoxAbout.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl AppTabControl;
    private System.Windows.Forms.TabPage AppTabPageAuth;
    private System.Windows.Forms.TabPage AppTabPageAbout;
    private System.Windows.Forms.Label LabelAbout;
    private System.Windows.Forms.Button ButtonUpdateCheck;
    private System.Windows.Forms.Label LabelVersion;
    private System.Windows.Forms.LinkLabel LabelAuthor;
    private System.Windows.Forms.Button ButtonAuth;
    private System.Windows.Forms.TabPage AppTabPageUploader;
    private System.Windows.Forms.GroupBox GroupBoxFiles;
    private System.Windows.Forms.Button ButtonAddFiles;
    private System.Windows.Forms.ListBox ListBoxFiles;
    private System.Windows.Forms.Label LabelFilesInfo;
    private System.Windows.Forms.Button ButtonAddDirectory;
    private System.Windows.Forms.Button ButtonFilesClear;
    private System.Windows.Forms.GroupBox GroupBoxSpacDirs;
    private System.Windows.Forms.ListView ListViewDirs;
    private System.Windows.Forms.ColumnHeader СolumnHeaderID;
    private System.Windows.Forms.ColumnHeader ColumnHeaderName;
    private System.Windows.Forms.Button ButtonUpload;
    private System.Windows.Forms.Label LabelSelectDirHelp;
    private System.Windows.Forms.GroupBox GroupBoxAuth;
    private System.Windows.Forms.TabPage AppTabPageProgress;
    private System.Windows.Forms.GroupBox GroupBoxUpdate;
    private System.Windows.Forms.GroupBox GroupBoxAbout;
    private System.Windows.Forms.GroupBox GroupBoxProgress;
    private System.Windows.Forms.Label LabelCurrentFile;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.TextBox TextBoxUploaderLog;
    private System.Windows.Forms.Button ButtonDebug;
  }
}

