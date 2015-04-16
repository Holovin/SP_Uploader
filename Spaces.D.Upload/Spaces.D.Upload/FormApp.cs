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

    private void FormApp_Shown(object sender, EventArgs e) {
      CGUIInit();
    }

    private void LabelAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start(App.AUTHOR_URL);
    }

    // GUI
    public void CGUIInit() {
      VGUIInit();
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
      }

      VUpdateText(up.LastVersion);

      control.Enabled = true;
    }

    public void VShowMessage(string caption, string message) {
      MessageBox.Show(message, caption);
    }

    private void VGUIInit() {
      FormApp.ActiveForm.Text = App.NAME;

      AppTabControl.TabPages.Remove(AppTabPageUploader);
    }

    private void ButtonAuth_Click(object sender, EventArgs e) {
      CAuth(sender, e);
    }

    private void CAuth(object sender, EventArgs e) {
      FormModal d = new FormModal("Авторизация", "Введите SID сессии:");

      if (d.ShowDialog() == DialogResult.OK) {
        App.session = new Session(d.InputText.Trim());
        if (App.session.Vaid) {
          VAuthOK();
          VShowMessage("Результат", "Пользователь: " + App.session.UserID);          
        } else VShowMessage("Ошибка", "Невалидный sid");
      }
    }

    private void VAuthOK() {
      AppTabControl.TabPages.Remove(AppTabPageAuth);
      AppTabControl.TabPages.Insert(0, AppTabPageUploader);
      AppTabControl.SelectedIndex = 0;
    }
  }

  public static class App {
    // Const
    public static readonly string NAME = "Spaces.D.Uploader";
    public static readonly string AUTHOR = "DJ_miXxXer";
    public static readonly string AUTHOR_URL = "http://spaces.ru/mysite/?name=DJ_miXxXer";

    public const int VERSION = 2;
    public static readonly string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0 MixxerUploader/0." + VERSION;

    public static Session session;
  }


  public class Session {
    private string sid;
    public string SID {
      get {
        return sid;
      }
    }

    private string userID;
    public string UserID {
      get {
        return userID;
      }
    }

    private bool valid;
    public bool Vaid {
      get {
        return valid;
      }
    }

    private void _ResetSessionData() {
      sid = "";
      userID = "";
      valid = false;      
    }


    private bool _CheckInputSID(string sid) {
      int trashVar = 0;
      if (sid.Length != 16 || int.TryParse(sid, out trashVar)) return false;
      return true;
    }

    public Session(string sid) {
      _ResetSessionData();

      Error.Reset();

      if (_CheckInputSID(sid)) {
        this.sid = sid;

        try {
          Networker net = new Networker(App.UA);
          net.Create("http://spaces.ru/settings/?sid=" + sid);
          net.Execute();

          string temp = net.GetCookieValueByName("user_id");
          if (temp != "") {
            userID = temp;
            valid = true;
          }

        } catch (Exception e) {
          Error.SetError(Error.Codes.TRY_COMMON_FAIL, "E: " + e.Message);
        }
      }
    }
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


  public class Networker {
    private HttpWebRequest request;
    private string useragent;
    private CookieContainer cookies;
    
    private string answer;
    public string Answer {
      get {
        return answer;
      }
    }

    public Networker(string useragent) {
      this.useragent = useragent;

      cookies = new CookieContainer();
    }
     
    public void Create(string url) {
      request = (HttpWebRequest)WebRequest.Create(url);
      request.UserAgent = useragent;
      request.CookieContainer = cookies;
    }

    public void Execute() {
      using (var answer = new StreamReader(request.GetResponse().GetResponseStream())) {
        this.answer = answer.ReadToEnd();
      }
    }

    public string GetCookieValueByName(string name) {
      foreach (Cookie c in cookies.GetCookies(new Uri("http://" + request.Host))) {
        if (c.Name == name) return c.Value;
      }
      
      return "";
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

