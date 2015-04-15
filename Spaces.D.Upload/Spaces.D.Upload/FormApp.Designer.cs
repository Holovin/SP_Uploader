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
      this.AppTabPage1 = new System.Windows.Forms.TabPage();
      this.AppTabPageAbout = new System.Windows.Forms.TabPage();
      this.LabelVersion = new System.Windows.Forms.Label();
      this.ButtonUpdateCheck = new System.Windows.Forms.Button();
      this.LabelAbout = new System.Windows.Forms.Label();
      this.AppTabControl.SuspendLayout();
      this.AppTabPageAbout.SuspendLayout();
      this.SuspendLayout();
      // 
      // AppTabControl
      // 
      this.AppTabControl.Controls.Add(this.AppTabPage1);
      this.AppTabControl.Controls.Add(this.AppTabPageAbout);
      this.AppTabControl.Location = new System.Drawing.Point(12, 12);
      this.AppTabControl.Name = "AppTabControl";
      this.AppTabControl.SelectedIndex = 0;
      this.AppTabControl.Size = new System.Drawing.Size(660, 338);
      this.AppTabControl.TabIndex = 0;
      // 
      // AppTabPage1
      // 
      this.AppTabPage1.Location = new System.Drawing.Point(4, 22);
      this.AppTabPage1.Name = "AppTabPage1";
      this.AppTabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.AppTabPage1.Size = new System.Drawing.Size(652, 312);
      this.AppTabPage1.TabIndex = 0;
      this.AppTabPage1.Text = "tabPage1";
      this.AppTabPage1.UseVisualStyleBackColor = true;
      // 
      // AppTabPageAbout
      // 
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
      this.Load += new System.EventHandler(this.FormApp_Load);
      this.AppTabControl.ResumeLayout(false);
      this.AppTabPageAbout.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl AppTabControl;
    private System.Windows.Forms.TabPage AppTabPage1;
    private System.Windows.Forms.TabPage AppTabPageAbout;
    private System.Windows.Forms.Label LabelAbout;
    private System.Windows.Forms.Button ButtonUpdateCheck;
    private System.Windows.Forms.Label LabelVersion;
  }
}

