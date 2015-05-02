using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpacesDUpload {
  public partial class FormAccounts: Form {
    private FormAccounts() {
      InitializeComponent();
    }

    public string GetSelectedSID() {
      if (ListView.Items.Count < 1) return "";
      if (ListView.SelectedIndices.Count < 1) return "";

      return ListView.Items[ListView.SelectedIndices[0]].SubItems[2].Text;
    }

    public FormAccounts(List<KeyValuePair<string, string>> accounts) {
      InitializeComponent();
      VUpdateAccounts(accounts);

      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    }

    private void FormAccounts_Load(object sender, EventArgs e) {

    }

    private void VUpdateAccounts(List<KeyValuePair<string, string>> dict) {
      ListView.Items.Clear();

      int counter = 0;
      foreach (KeyValuePair<string, string> item in dict) {
        counter++;
        ListView.Items.Add(new ListViewItem(new string[] {counter.ToString(), item.Key, item.Value }));
      }

      if (ListView.Items.Count > 0) {
        ListView.Items[0].Selected = true;
      }
    }

    private void ListView_DoubleClick(object sender, EventArgs e) {
      CSelectAccount(sender, e);
    }

    private void ListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        CSelectAccount(sender, e);
      }
    }

    private void CSelectAccount(object sender, EventArgs e) {
      this.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.Close();
    }
  }
}
