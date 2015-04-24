using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Web.Script.Serialization;

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

    private void ButtonAuth_Click(object sender, EventArgs e) {
      CAuth(sender, e);
    }

    private void ListViewDirs_MouseDoubleClick(object sender, MouseEventArgs e) {
      CChangeFileDir(sender, e);
    }

    private void ButtonUpload_Click(object sender, EventArgs e) {
      CUpload(sender, e);
    }

    // GUI
    private void CGUIInit() {
      App.net = new Networker(App.UA);
      App.api = new MixxerAPI(ref App.net);

      VGUIInit();
      VUpdateText();
    }

    private void CAddFiles(object sender, EventArgs e) {
      const int maxFileSize = 62914560;

      CLock(); 
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
        FileInfo f;
        int added = 0;

        foreach (string item in modal.FileNames) {
          f = new FileInfo(item);
          if (f.Length < maxFileSize) {
            added++;
            ListBoxFiles.Items.Add(item);
          }          
        }
        
        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (modal.FileNames.GetLength(0) - added));
      }
      CUnlock();
    }

    private void CAddFilesFromDir(object sender, EventArgs e) {
      CLock();
      FolderBrowserDialog modal = new FolderBrowserDialog();

      modal.Description = "Выберите папку с файлами";
      modal.ShowNewFolderButton = false;
      modal.RootFolder = Environment.SpecialFolder.MyComputer;

      if (modal.ShowDialog() == DialogResult.OK) {
        string[] files = Directory.GetFiles(modal.SelectedPath, "*.mp3", SearchOption.AllDirectories);
        ListBoxFiles.Items.AddRange(files);
        VUpdateFilesInfo();
      }
      CUnlock();
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
      CLock();
      VClearFilesList();
      VUpdateFilesInfo();
      CUnlock();
    }

    private void VLockControl(object sender) {
      Control control = sender as Control;
      if (control == null) return;
      control.Enabled = false;
    }

    private void VUnlockControl(object sender) {
      Control control = sender as Control;
      if (control == null) return;
      control.Enabled = true;
    }

    private async void CUpdateChecker(object sender, EventArgs e) {
      CLock();
      VLockControl(sender);

      Updater up = null;

      await Task.Factory.StartNew(() => {
        up = new Updater();
      }, TaskCreationOptions.LongRunning);
    
      if (Error.CheckIsError()) {
        VShowMessage(Error.ERROR_COMMON, Error.Message);
      } else {
        VShowMessage("Проверка обновлений", up.CompareVersions(App.VERSION));
      }

      VUpdateText(up.LastVersion);
      VUnlockControl(sender);
      CUnlock();
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

      //VTabAccessChange(AppTabPageUploader, false);
    }

    private bool CLock() {
      if (App.BeginWork()) {
        VLock();
        return true;
      }

      return false;      
    }

    private void VLock() {
      this.Text += " [...]";
      Application.DoEvents();
    }

    private void CUnlock() {
      App.EndWork();
      VUnlock();
    }

    private void VUnlock() {
      this.Text = App.NAME;
      Application.DoEvents();
    }

    public static class AppTh {
      public delegate void Run(string sid);
    }
    


    private void CAuth(object sender, EventArgs e) {
      CLock();
      VLockControl(sender);

      FormModal d = new FormModal("Авторизация", "Введите SID сессии:");

      bool okFlag = false;

      if (d.ShowDialog() == DialogResult.OK) {
        App.session = new Session(ref App.net, d.InputText);
        if (App.session.Vaid) {
          okFlag = true;

          AppTh.Run a = TAuth;
          IAsyncResult r = a.BeginInvoke(d.InputText, new AsyncCallback(VAuthOK), a);
        } else {
          VShowMessage("Ошибка", "Невалидный sid");
        }
      }

      if (okFlag == false) {
        VUnlockControl(sender);
        CUnlock();
      }
    }
    
    private void TAuth(string sid) {
      MixxerAPI m = new MixxerAPI(ref App.net);
      m.GetUserNameById(App.session.UserID);           
    }

    private void VAuthOK(IAsyncResult ar) {
      if (ar == null) throw new Exception("");

      AppTh.Run a = ar.AsyncState as AppTh.Run;
      if (a == null) throw new Exception("");
      a.EndInvoke(ar);
    
      this.Invoke((MethodInvoker)delegate {
        VShowMessage("Авторизация", "Привет, " + App.session.UserName + "!");

        List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
        list.Add(new KeyValuePair<string, string>("0", "(кликни для загрузки)"));
        VUploadDirsSet(list);
        VTabAccessChange(AppTabPageAuth, false);
        VTabAccessChange(AppTabPageUploader, true);
        CUnlock();
      });
    }

    private void VUploadDirsSet(List<KeyValuePair<string, string>> dict) {
      ListViewDirs.Items.Clear();

      foreach (KeyValuePair<string, string> item in dict) {
        ListViewDirs.Items.Add(new ListViewItem(new string[] { item.Key, item.Value }));
      }
    }
  


    private void CChangeFileDir(object sender, MouseEventArgs e) {
      List<KeyValuePair<string, string>> dict = App.api.GetMusicDirs(App.session.UserName, ListViewDirs.SelectedItems[0].Text);

      dict.Insert(0, new KeyValuePair<string, string>("0", "< корневая папка >"));

      VUploadDirsSet(dict);
    }

    private void VTabAccessChange(TabPage page, bool newValue) {
      foreach (Control control in page.Controls) {
        control.Enabled = newValue;
      }

      AppTabControl.SelectedTab = page;
    }

    private void CUpload(object sender, EventArgs e) {
      VTabAccessChange(AppTabPageUploader, false);
      VTabAccessChange(AppTabPageProgress, true);


      App.api.GetUploadUrl(App.session.SID);
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

    private static bool workFlag = false;
    public static bool BeginWork() {
      if (workFlag == true) {
        return false;
      } else {
        workFlag = true;
        return true;
      }
    }

    public static bool EndWork() {
      if (workFlag == true) {
        workFlag = false;
        return false;
      }

      throw new Exception("Try end work, but work doesnt started");
    }
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

    // 6 - music
    public string GetUploadUrl(string sid, string type = "6") {
      net.Create("http://spaces.ru/api/files/");

      net.PostAdd("method", "getUploadInfo");
      net.PostAdd("Type", type);

      net.ExecutePOST();

      String answer = "";

      try {
        var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(net.Answer.ToString());
        answer = (string)dict["url"];
      } catch (Exception e) {
        Error.SetError(Error.Codes.WRONG_PARSE_DATA, "Empty url parse");
      }
      
      return answer;
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
          //Error.SetError(Error.Codes.WRONG_PARSE_DATA, "Real size incorrect: " + temp.Groups.Count);
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

      //Error.Reset();

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
          //Error.SetError(Error.Codes.TRY_COMMON_FAIL, "E: " + e.Message);
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
    private NameValueCollection postParams;
    
    private string answer;
    public string Answer {
      get {
        return answer;
      }
    }

    private int lastCodeAnswer;
    public int LastCodeAnswer {
      get {
        return lastCodeAnswer;
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
      postParams = new NameValueCollection();
    }
     
    public void Create(string url) {
      request = (HttpWebRequest)WebRequest.Create(url);
      request.UserAgent = useragent;
      request.CookieContainer = cookies;
      request.AllowAutoRedirect = true;
      request.KeepAlive = false;

      postParams.Clear();
      postParams = HttpUtility.ParseQueryString(String.Empty);
    }

    public void ExecuteGET() {
      request.Method = "GET";

      try {
        using (HttpWebResponse answer = request.GetResponse() as HttpWebResponse) {
          using (StreamReader reader = new StreamReader(answer.GetResponseStream())) {
            this.answer = reader.ReadToEnd();
            this.lastCodeAnswer = (int)answer.StatusCode;
          }
        }
      } catch {
        // err
      }
    }

    public void PostAdd(string name, string value) {
      postParams.Add(name, value);
    }

    public void ExecutePOST() {
      request.Method = "POST";

      try {
        using (Stream rStream = request.GetRequestStream()) {
          rStream.Write(Encoding.UTF8.GetBytes(postParams.ToString()), 0,
                        Encoding.UTF8.GetByteCount(postParams.ToString()));
        }

        using (HttpWebResponse answer = request.GetResponse() as HttpWebResponse) {
          using (StreamReader reader = new StreamReader(answer.GetResponseStream())) {
            this.answer = reader.ReadToEnd();
            this.lastCodeAnswer = (int)answer.StatusCode;
          }
        }
      } catch {
        // err
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

