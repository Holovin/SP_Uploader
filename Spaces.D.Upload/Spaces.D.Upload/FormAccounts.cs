using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpacesDUpload {
  public partial class FormAccounts: Form {
    private FormAccounts() {
      InitializeComponent();
    }

    public string GetSelectedSid() {
      if (ListView.Items.Count < 1 || ListView.SelectedIndices.Count < 1) return "";
      return ListView.Items[ListView.SelectedIndices[0]].SubItems[2].Text;
    }

    public FormAccounts(IEnumerable<KeyValuePair<string, string>> accounts) {
      InitializeComponent();
      VUpdateAccounts(accounts);

      DialogResult = DialogResult.Cancel;
    }

    private void FormAccounts_Load(object sender, EventArgs e) {

    }

    private void VUpdateAccounts(IEnumerable<KeyValuePair<string, string>> dict) {
      ListView.Items.Clear();

      var counter = 0;
      foreach (var item in dict) {
        counter++;
        ListView.Items.Add(new ListViewItem(new[] {counter.ToString(), item.Key, item.Value }));
      }

      if (ListView.Items.Count > 0) {
        ListView.Items[0].Selected = true;
      }
    }

    private void ListView_DoubleClick(object sender, EventArgs e) {
      CSelectAccount();
    }

    private void ListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        CSelectAccount();
      }
    }

    private void CSelectAccount() {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void ListView_SelectedIndexChanged(object sender, EventArgs e) {

    }
  }
}
