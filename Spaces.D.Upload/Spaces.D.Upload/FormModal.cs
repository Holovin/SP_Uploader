using System;
using System.Windows.Forms;

namespace SpacesDUpload {
  public sealed partial class FormModal: Form {
    private FormModal() {
      InitializeComponent();
    }

    public FormModal(string caption, string message) {
      InitializeComponent();

      Text = caption;
      LabelText.Text = message;
      DialogResult = DialogResult.Cancel;
    }

    public string InputText {
      get {
        return TextBox.Text;
      }
    }

    private void Button_Click(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      Close();
    }

  }
}
