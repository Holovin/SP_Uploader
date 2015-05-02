using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpacesDUpload {
  public partial class FormModal: Form {
    private FormModal() {
      InitializeComponent();
    }

    public FormModal(string caption, string message) {
      InitializeComponent();

      this.Text = caption;
      this.LabelText.Text = message;
      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    }

    public string InputText {
      get {
        return this.TextBox.Text;
      }
    }

    private void Button_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

  }
}
