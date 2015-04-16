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

    private void ButtonAddFiles_Click(object sender, EventArgs e) {
      CAddFiles(sender, e);
    }

    private void ButtonAddDirectory_Click(object sender, EventArgs e) {
      CAddFilesFromDir(sender, e);
    }

    private void ButtonFilesClear_Click(object sender, EventArgs e) {
      CClearFilesList(sender, e);
    }

    // GUI
    private void CGUIInit() {
      App.net = new Networker(App.UA);
      App.api = new MixxerAPI(ref App.net);

      VGUIInit();
      VUpdateText();
    }

    private void CAddFiles(object sender, EventArgs e) {
      VGUILock(); 
      OpenFileDialog modal = new OpenFileDialog();

      modal.AddExtension = true;
      modal.CheckFileExists = true;
      modal.CheckPathExists = true;
      modal.DefaultExt = "mp3";
      modal.Filter = "Музыкальные файлы | *.mp3";
      modal.InitialDirectory = Directory.GetCurrentDirectory();
      modal.Multiselect = true;
      modal.Title = "Выберите файлы";

      if (modal.ShowDialog() == DialogResult.OK) {
        ListBoxFiles.Items.AddRange(modal.FileNames);
        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + modal.FileNames.GetLength(0));
      }
      VGUIUnlock();
    }

    private void CAddFilesFromDir(object sender, EventArgs e) {
      VGUILock();
      FolderBrowserDialog modal = new FolderBrowserDialog();

      modal.Description = "Выберите папку с файлами";
      modal.ShowNewFolderButton = false;
      modal.RootFolder = Environment.SpecialFolder.MyComputer;

      if (modal.ShowDialog() == DialogResult.OK) {
        string[] files = Directory.GetFiles(modal.SelectedPath, "*.mp3", SearchOption.AllDirectories);
        ListBoxFiles.Items.AddRange(files);
        VUpdateFilesInfo();
      }
      VGUIUnlock();
    }

    private void VUpdateFilesInfo() {
      float size = 0;

      foreach (string item in ListBoxFiles.Items) {
        FileInfo f = new FileInfo(item);
        size += f.Length / (1024 * 1024);
      }

      LabelFilesInfo.Text = "Файлов: "+ ListBoxFiles.Items.Count +
        " | Общий размер: " + size + " МБ";
    }

    private void VUpdateText(int newVersion = 0) {
      String temp = "Текущая версия: " + App.VERSION + '\n' + "Актуальная версия: ";

      if (newVersion != 0) temp += newVersion;
      else temp += "?";

      LabelVersion.Text = temp;
    }

    private void CClearFilesList(object sender, EventArgs e) {
      VGUILock();
      VClearFilesList();
      VUpdateFilesInfo();
      VGUIUnlock();
    }

    private void VLockButton(object sender) {
      Button control = sender as Button;
      if (control == null) return;
      control.Enabled = false;
    }

    private void VUnlockButton(object sender) {
      Button control = sender as Button;
      if (control == null) return;
      control.Enabled = true;
    }

    private void CUpdateChecker(object sender, EventArgs e) {
      VGUILock();
      VLockButton(sender);

      Updater up = new Updater();

      if (Error.CheckIsError()) {
        VShowMessage(Error.ERROR_COMMON, Error.Message);
      } else {
        VShowMessage("Проверка обновлений", up.CompareVersions(App.VERSION));
      }

      VUpdateText(up.LastVersion);
      VUnlockButton(sender);
      VGUIUnlock();
    }

    private void VShowMessage(string caption, string message) {
      MessageBox.Show(message, caption);
    }

    private void VClearFilesList() {
      ListBoxFiles.Items.Clear();
    }

    private void VGUIInit() {
      this.Text = App.NAME;
      this.LabelAbout.Text = App.NAME;

      //[DEBUG]
      //AppTabControl.TabPages.Remove(AppTabPageUploader);      
    }

    private void VGUILock() {
      this.Text += " [...]";
      Application.DoEvents();
    }

    private void VGUIUnlock() {
      this.Text = App.NAME;
    }

    private void ButtonAuth_Click(object sender, EventArgs e) {
      CAuth(sender, e);
    }

    private void CAuth(object sender, EventArgs e) {
      VGUILock();

      FormModal d = new FormModal("Авторизация", "Введите SID сессии:");

      if (d.ShowDialog() == DialogResult.OK) {
        App.session = new Session(ref App.net, d.InputText.Trim());
        if (App.session.Vaid) {
          MixxerAPI m = new MixxerAPI(ref App.net);
          m.GetUserNameById(App.session.UserID);
          //VAuthOK();

          List<KeyValuePair<string,string>> list = new List<KeyValuePair<string, string>>();
          list.Add(new KeyValuePair<string,string>("0", "нажми"));

          VUploadDirsSet(list);

          VShowMessage("Авторизация", "Привет, " + App.session.UserName + "!");
        } else VShowMessage("Ошибка", "Невалидный sid");
      }

      VGUIUnlock();
    }

    private void VAuthOK() {
      AppTabControl.TabPages.Remove(AppTabPageAuth);
      AppTabControl.TabPages.Insert(0, AppTabPageUploader);
      AppTabControl.SelectedIndex = 0;
    }

    private void VUploadDirsSet(List<KeyValuePair<string, string>> dict) {
      ListViewDirs.Items.Clear();

      foreach (KeyValuePair<string, string> item in dict) {
        ListViewDirs.Items.Add(new ListViewItem(new string[] { item.Key, item.Value }));
      }
    }

    private void button1_Click(object sender, EventArgs e) {
      FormModal x = new FormModal("1", "1");

      x.ShowDialog();

      Match backLink = Regex.Match(x.InputText, @"Dir=(\d+).+?>(.+?)<.a>");

      MessageBox.Show(backLink.Groups[1] + " - " + backLink.Groups[2]);

    }
  
    private void ListViewDirs_MouseDoubleClick(object sender, MouseEventArgs e) {
      CChangeFileDir(sender, e);      
    }

    private void CChangeFileDir(object sender, MouseEventArgs e) {
      List<KeyValuePair<string, string>> dict = App.api.GetMusicDirs(App.session.UserName, ListViewDirs.SelectedItems[0].Text);

      dict.Insert(0, new KeyValuePair<string, string>("0", "< корневая папка >"));

      VUploadDirsSet(dict);
    }

  }

  public static class App {
    // Const
    public static readonly string NAME = "Spaces.D.MusicUploader";
    public static readonly string AUTHOR = "DJ_miXxXer";
    public static readonly string AUTHOR_URL = "http://spaces.ru/mysite/?name=DJ_miXxXer";

    public const int VERSION = 2;
    public static readonly string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0 MixxerUploader/0." + VERSION;

    public static Session session;
    public static Networker net;
    public static MixxerAPI api;
  }

  public class MixxerAPI {
    private Networker net;

    public MixxerAPI(ref Networker net) {
      this.net = net;
    }

    public string GetUserNameById(string id) {
      net.Create("http://" + id + ".spaces.ru/");
      net.ExecuteGET();
      return net.GetValueByParam("name");
    }

    public List<KeyValuePair<string, string>> GetMusicDirs(string userName, string dirId) {
      List<KeyValuePair<string, string>> dict = new List<KeyValuePair<string, string>>();

      if (dirId == "0") net.Create("http://spaces.ru/music/?r=main/index&name=" + userName);
      else net.Create("http://spaces.ru/music/?Dir=" + dirId);
      net.ExecuteGET();

      MatchCollection m = Regex.Matches(net.Answer, @"<a.*?</a>");

      foreach (Match item in m) {
        Match temp = Regex.Match(item.Value, @"Dir=(\d*).+?hover" + '"' + ">(.*?)</span>");

        if (temp.Groups.Count == 3 && temp.Groups[1].Value != "") {
          dict.Add(new KeyValuePair<string, string>(temp.Groups[1].Value, temp.Groups[2].Value));
        } else {
          Error.SetError(Error.Codes.WRONG_PARSE_DATA, "Real size incorrect: " + temp.Groups.Count);
        }
      }
     
      Match nav = Regex.Match(net.Answer, @"<div class=" + '"' +
                             "location-bar" + '"' + ".*?<a.+?</a>.+?</div>");

      if (nav.Length > 0) {
        MatchCollection navCounter = Regex.Matches(nav.Value, "<*.a>");

        if (navCounter.Count > 3) {
          Debug.WriteLine(nav.Value);
          Match backLink = Regex.Match(nav.Value, @".+Dir=(\d+).+?>(.+?)<.a>");
          if (backLink.Length > 0) dict.Insert(0, new KeyValuePair<string, string>(backLink.Groups[1].Value, "[вверх на уровень] " + backLink.Groups[2].Value));
        }
      }
     
      return dict;     
    }
  }

  public class Session {
    private string sid;
    public string SID {
      get {
        return sid;
      }
    }

    private string userName;
    public string UserName {
      get {
        return userName;
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
      userName = "";
      valid = false;      
    }


    private bool _CheckInputSID(string sid) {
      int trashVar = 0;
      if (sid.Length != 16 || int.TryParse(sid, out trashVar)) return false;
      return true;
    }

    public Session(ref Networker net, string sid) {
      _ResetSessionData();

      Error.Reset();

      if (_CheckInputSID(sid)) {
        this.sid = sid;

        try {
          net.Create("http://spaces.ru/settings/?sid=" + sid);
          net.ExecuteGET();

          string temp = net.GetCookieValueByName("user_id");
          if (temp != "") {
            userID = temp;
            valid = true;

             MixxerAPI api = new MixxerAPI(ref net);
             userName = api.GetUserNameById(UserID);
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

    public string AnswerDecoded {
      get {
        return WebUtility.HtmlDecode(answer);
      }
    }

    public string GetValueByParam(string name) {
      string url = request.Address.Query.Substring(1);

      string []param = url.Split('&');

      foreach (string item in param) {
        if (item.Length < 1) continue;

        string []temp = item.Split('=');

        if (temp[0] == name) return temp[1];
	    }

      return "";
    }

    public Networker(string useragent) {
      this.useragent = useragent;

      cookies = new CookieContainer();
    }
     
    public void Create(string url) {
      request = (HttpWebRequest)WebRequest.Create(url);
      request.UserAgent = useragent;
      request.CookieContainer = cookies;
      request.AllowAutoRedirect = true;
    }

    public void ExecuteGET() {
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

