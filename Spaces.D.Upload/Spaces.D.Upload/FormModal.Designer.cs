namespace SpacesDUpload {
  sealed partial class FormModal {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormModal));
      this.TextBox = new System.Windows.Forms.TextBox();
      this.LabelText = new System.Windows.Forms.Label();
      this.Button = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // TextBox
      // 
      this.TextBox.AcceptsReturn = true;
      this.TextBox.Location = new System.Drawing.Point(12, 51);
      this.TextBox.Multiline = true;
      this.TextBox.Name = "TextBox";
      this.TextBox.Size = new System.Drawing.Size(260, 68);
      this.TextBox.TabIndex = 0;
      this.TextBox.WordWrap = false;
      // 
      // LabelText
      // 
      this.LabelText.Location = new System.Drawing.Point(12, 9);
      this.LabelText.Name = "LabelText";
      this.LabelText.Size = new System.Drawing.Size(260, 39);
      this.LabelText.TabIndex = 1;
      this.LabelText.Text = "Text 000000000000000000000000\r\nText 000000000000000000000000\r\nText 00000000000000" +
    "0000000000";
      // 
      // Button
      // 
      this.Button.Location = new System.Drawing.Point(12, 125);
      this.Button.Name = "Button";
      this.Button.Size = new System.Drawing.Size(260, 31);
      this.Button.TabIndex = 2;
      this.Button.Text = "ОК";
      this.Button.UseVisualStyleBackColor = true;
      this.Button.Click += new System.EventHandler(this.Button_Click);
      // 
      // FormModal
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 162);
      this.Controls.Add(this.Button);
      this.Controls.Add(this.LabelText);
      this.Controls.Add(this.TextBox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(300, 200);
      this.MinimizeBox = false;
      this.Name = "FormModal";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "FormModal";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox TextBox;
    private System.Windows.Forms.Label LabelText;
    private System.Windows.Forms.Button Button;
  }
}