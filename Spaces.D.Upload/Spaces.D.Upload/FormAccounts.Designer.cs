namespace SpacesDUpload {
  partial class FormAccounts {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAccounts));
      this.ListView = new System.Windows.Forms.ListView();
      this.ColumnHeaderNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColumnHeaderAccountName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ColumnHeaderSID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.SuspendLayout();
      // 
      // ListView
      // 
      this.ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeaderNumber,
            this.ColumnHeaderAccountName,
            this.ColumnHeaderSID});
      this.ListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ListView.FullRowSelect = true;
      this.ListView.GridLines = true;
      this.ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.ListView.LabelWrap = false;
      this.ListView.Location = new System.Drawing.Point(0, 0);
      this.ListView.MultiSelect = false;
      this.ListView.Name = "ListView";
      this.ListView.ShowGroups = false;
      this.ListView.Size = new System.Drawing.Size(429, 229);
      this.ListView.TabIndex = 1;
      this.ListView.UseCompatibleStateImageBehavior = false;
      this.ListView.View = System.Windows.Forms.View.Details;
      this.ListView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
      this.ListView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
      this.ListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView_KeyDown);
      // 
      // ColumnHeaderNumber
      // 
      this.ColumnHeaderNumber.Text = "#";
      this.ColumnHeaderNumber.Width = 30;
      // 
      // ColumnHeaderAccountName
      // 
      this.ColumnHeaderAccountName.Text = "Логин";
      this.ColumnHeaderAccountName.Width = 150;
      // 
      // ColumnHeaderSID
      // 
      this.ColumnHeaderSID.Text = "SID";
      this.ColumnHeaderSID.Width = 240;
      // 
      // FormAccounts
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(429, 229);
      this.Controls.Add(this.ListView);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormAccounts";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Выберите аккаунт (двойной клик или enter для выбора)";
      this.Load += new System.EventHandler(this.FormAccounts_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView ListView;
    private System.Windows.Forms.ColumnHeader ColumnHeaderNumber;
    private System.Windows.Forms.ColumnHeader ColumnHeaderAccountName;
    private System.Windows.Forms.ColumnHeader ColumnHeaderSID;

  }
}