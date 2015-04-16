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
      this.ButtonAuth = new System.Windows.Forms.Button();
      this.AppTabPageUploader = new System.Windows.Forms.TabPage();
      this.GroupBoxFiles = new System.Windows.Forms.GroupBox();
      this.LabelFilesInfo = new System.Windows.Forms.Label();
      this.ButtonAddDirectory = new System.Windows.Forms.Button();
      this.ButtonAddFiles = new System.Windows.Forms.Button();
      this.ListBoxFiles = new System.Windows.Forms.ListBox();
      this.AppTabPageAbout = new System.Windows.Forms.TabPage();
      this.LabelAuthor = new System.Windows.Forms.LinkLabel();
      this.LabelVersion = new System.Windows.Forms.Label();
      this.ButtonUpdateCheck = new System.Windows.Forms.Button();
      this.LabelAbout = new System.Windows.Forms.Label();
      this.ButtonFilesClear = new System.Windows.Forms.Button();
      this.AppTabControl.SuspendLayout();
      this.AppTabPageAuth.SuspendLayout();
      this.AppTabPageUploader.SuspendLayout();
      this.GroupBoxFiles.SuspendLayout();
      this.AppTabPageAbout.SuspendLayout();
      this.SuspendLayout();
      // 
      // AppTabControl
      // 
      this.AppTabControl.Controls.Add(this.AppTabPageAuth);
      this.AppTabControl.Controls.Add(this.AppTabPageUploader);
      this.AppTabControl.Controls.Add(this.AppTabPageAbout);
      this.AppTabControl.Location = new System.Drawing.Point(12, 12);
      this.AppTabControl.Name = "AppTabControl";
      this.AppTabControl.SelectedIndex = 0;
      this.AppTabControl.Size = new System.Drawing.Size(660, 338);
      this.AppTabControl.TabIndex = 0;
      // 
      // AppTabPageAuth
      // 
      this.AppTabPageAuth.Controls.Add(this.ButtonAuth);
      this.AppTabPageAuth.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageAuth.Name = "AppTabPageAuth";
      this.AppTabPageAuth.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPageAuth.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageAuth.TabIndex = 0;
      this.AppTabPageAuth.Text = "Авторизация";
      this.AppTabPageAuth.UseVisualStyleBackColor = true;
      // 
      // ButtonAuth
      // 
      this.ButtonAuth.Location = new System.Drawing.Point(17, 16);
      this.ButtonAuth.Name = "ButtonAuth";
      this.ButtonAuth.Size = new System.Drawing.Size(99, 38);
      this.ButtonAuth.TabIndex = 0;
      this.ButtonAuth.Text = "Войти";
      this.ButtonAuth.UseVisualStyleBackColor = true;
      this.ButtonAuth.Click += new System.EventHandler(this.ButtonAuth_Click);
      // 
      // AppTabPageUploader
      // 
      this.AppTabPageUploader.Controls.Add(this.GroupBoxFiles);
      this.AppTabPageUploader.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageUploader.Name = "AppTabPageUploader";
      this.AppTabPageUploader.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageUploader.TabIndex = 2;
      this.AppTabPageUploader.Text = "Загрузка файлов";
      this.AppTabPageUploader.UseVisualStyleBackColor = true;
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
      this.GroupBoxFiles.Text = "Файлы";
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
      this.ListBoxFiles.Location = new System.Drawing.Point(6, 19);
      this.ListBoxFiles.Name = "ListBoxFiles";
      this.ListBoxFiles.Size = new System.Drawing.Size(375, 199);
      this.ListBoxFiles.TabIndex = 0;
      // 
      // AppTabPageAbout
      // 
      this.AppTabPageAbout.Controls.Add(this.LabelAuthor);
      this.AppTabPageAbout.Controls.Add(this.LabelVersion);
      this.AppTabPageAbout.Controls.Add(this.ButtonUpdateCheck);
      this.AppTabPageAbout.Controls.Add(this.LabelAbout);
      this.AppTabPageAbout.Location = new System.Drawing.Point(4, 22);
      this.AppTabPageAbout.Name = "AppTabPageAbout";
      this.AppTabPageAbout.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPageAbout.Size = new System.Drawing.Size(652, 312);
      this.AppTabPageAbout.TabIndex = 1;
      this.AppTabPageAbout.Text = "О программе";
      this.AppTabPageAbout.UseVisualStyleBackColor = true;
      // 
      // LabelAuthor
      // 
      this.LabelAuthor.Location = new System.Drawing.Point(10, 43);
      this.LabelAuthor.Name = "LabelAuthor";
      this.LabelAuthor.Size = new System.Drawing.Size(171, 23);
      this.LabelAuthor.TabIndex = 3;
      this.LabelAuthor.TabStop = true;
      this.LabelAuthor.Text = "by DJ_miXxXer";
      this.LabelAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LabelAuthor_LinkClicked);
      // 
      // LabelVersion
      // 
      this.LabelVersion.Location = new System.Drawing.Point(7, 235);
      this.LabelVersion.Name = "LabelVersion";
      this.LabelVersion.Size = new System.Drawing.Size(156, 30);
      this.LabelVersion.TabIndex = 2;
      this.LabelVersion.Text = "Текущая версия: 01234\r\nАктуальная версия: 12345\r\n\r\n";
      // 
      // ButtonUpdateCheck
      // 
      this.ButtonUpdateCheck.Location = new System.Drawing.Point(10, 268);
      this.ButtonUpdateCheck.Name = "ButtonUpdateCheck";
      this.ButtonUpdateCheck.Size = new System.Drawing.Size(153, 27);
      this.ButtonUpdateCheck.TabIndex = 1;
      this.ButtonUpdateCheck.Text = "Проверить обновления";
      this.ButtonUpdateCheck.UseVisualStyleBackColor = false;
      this.ButtonUpdateCheck.Click += new System.EventHandler(this.ButtonUpdateCheck_Click);
      // 
      // LabelAbout
      // 
      this.LabelAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.LabelAbout.Location = new System.Drawing.Point(6, 14);
      this.LabelAbout.Name = "LabelAbout";
      this.LabelAbout.Size = new System.Drawing.Size(175, 29);
      this.LabelAbout.TabIndex = 0;
      this.LabelAbout.Text = "Spaces.D.Uploader";
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
      this.Shown += new System.EventHandler(this.FormApp_Shown);
      this.AppTabControl.ResumeLayout(false);
      this.AppTabPageAuth.ResumeLayout(false);
      this.AppTabPageUploader.ResumeLayout(false);
      this.GroupBoxFiles.ResumeLayout(false);
      this.AppTabPageAbout.ResumeLayout(false);
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
  }
}

