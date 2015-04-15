using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SpacesDUpload {
  public partial class FormApp: Form {
    public FormApp() {
      InitializeComponent();
    }

    private void ButtonUpdateCheck_Click(object sender, EventArgs e) {    
      CUpdateChecker(sender, e);
    }

    private void FormApp_Load(object sender, EventArgs e) {
      CGUIInit();
    }

    // GUI
    public void CGUIInit() {
      VUpdateText();
    }

    public void VUpdateText(int newVersion = 0) {
      String temp = "Текущая версия: " + App.VERSION + '\n' + "Актуальная версия: ";

      if (newVersion != 0) temp += newVersion;
      else temp += "?";

      LabelVersion.Text = temp;
    }

    public void CUpdateChecker(object sender, EventArgs e) {
      Button control = sender as Button;
      if (control == null) return;

      control.Enabled = false;

      Updater up = new Updater();

      if (Error.CheckIsError()) {
        VShowMessage(Error.ERROR_COMMON, Error.Message);
      } else {
        VShowMessage("Проверка обновлений", up.CompareVersions(App.VERSION));
        Process.Start(SpacesDUpload.Updater.UPDATE_LINK);      
      }

      VUpdateText(up.LastVersion);

      control.Enabled = true;
    }

    public void VShowMessage(string caption, string message) {
      MessageBox.Show(message, caption);
    }


  }

  public static class App {
    public const int VERSION = 1;    
  }

  public static class Error {
    public static class Codes {
      public const int NO_ERROR = 0;
      public const int WRONG_PARSE_DATA = 1;
      public const int TRY_COMMON_FAIL = 2;
    }

    public static readonly string ERROR_COMMON = "Ошибка";

    // Data
    public static int lastErrorCode = 0;
    public static string extMessage = "";

    // Func
    public static bool CheckIsError() {
      if (lastErrorCode != Codes.NO_ERROR) return true;
      return false;
    }

    public static void SetError(int code, string _extMessage) {
      lastErrorCode = code;
      extMessage = _extMessage;
    }

    public static void Reset() {
      lastErrorCode = 0;
      extMessage = "";
    }

    public static string Message {
      get {
        string temp = "";
        switch (lastErrorCode) {
          case 0: {
            temp += "Нет ошибки";
            break;
          }

          case 1: {
            temp += "Ошибка чтения данных";
            break;
          }

          default: {
            temp += "Неизвестная ошибка (код: " + lastErrorCode + ")";
            break;
          }
        }

        return temp += "\n[e: " + extMessage + "]";
      }
    }
  }

  public class Updater {
    private int lastVersion = 0;
    public int LastVersion { 
      get {
        return lastVersion;
      }
    }

    public static readonly string UPDATE_LINK = "http://spaces.ru/forums/?r=17760125";

    public Updater() {
      Error.Reset();

      try {
        var request = (HttpWebRequest)WebRequest.Create(UPDATE_LINK);

        using (var answer = new StreamReader(request.GetResponse().GetResponseStream())) {
          Match m = Regex.Match(answer.ReadToEnd(), @"###LASTVERSION:(\d)");

          if (m.Groups.Count == 2) {
            lastVersion = Convert.ToInt32(m.Groups[1].Value);
          } else {
            Error.SetError(Error.Codes.WRONG_PARSE_DATA , "Err count: " + m.Groups.Count);
          }
        }
      } catch (Exception e) {
        Error.SetError(Error.Codes.TRY_COMMON_FAIL, "E: " + e.Message);
      }
    }

    public string CompareVersions(int currentVersion) {
      if (currentVersion < lastVersion) {
        return "Доступна новая версия!";
      }
      return "Обновление не треубется";
    }
  }
}
