using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

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

    private void CGUIInit() {
      App.net = new Networker(App.UA);
      App.err = new Error();

      VGUIInit();
      VUpdateText();
    }

    private async void CAddFiles(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

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

        List<string> itemCache = new List<string>();
        await Task.Factory.StartNew(() => {
          foreach (string item in modal.FileNames) {
            f = new FileInfo(item);
              if (f.Length < App.Const.maxFileSize) {
                added++;
                itemCache.Add(item);
              }
          }
        }, TaskCreationOptions.LongRunning);
        ListBoxFiles.Items.AddRange(itemCache.ToArray());
     
        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (modal.FileNames.GetLength(0) - added));
      }

      VUnlockControl(sender);
      CUnlock();
    }

    private async void CAddFilesFromDir(object sender, EventArgs e) {
      if (!CLock()) return;
      FolderBrowserDialog modal = new FolderBrowserDialog();

      modal.Description = "Выберите папку с файлами";
      modal.ShowNewFolderButton = false;
      modal.RootFolder = Environment.SpecialFolder.MyComputer;

      if (modal.ShowDialog() == DialogResult.OK) {
        FileInfo f;
        int added = 0;

        List<string> itemCache = new List<string>();
        string[] files = null;
        await Task.Factory.StartNew(() => {
          files = Directory.GetFiles(modal.SelectedPath, "*.mp3", SearchOption.AllDirectories);

          foreach (string item in files) {
            f = new FileInfo(item);
            if (f.Length < App.Const.maxFileSize) {
              added++;
              itemCache.Add(item);
            }
          }
        }, TaskCreationOptions.LongRunning);

        ListBoxFiles.Items.AddRange(itemCache.ToArray());

        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (files.Length - added));
      }
      CUnlock();
    }

    private void VUpdateFilesInfo() {
      float size = 0;

      if (ListBoxFiles.Items.Count < 1) VLockControl(GroupBoxSpacDirs);
      else VUnlockControl(GroupBoxSpacDirs);

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
      if (!CLock()) return;

      VClearFilesList();
      VUpdateFilesInfo();

      CUnlock();
    }

    private void VLockControl(object sender) {
      this.Invoke((MethodInvoker)delegate {
      Control control = sender as Control;
      if (control == null) return;
      control.Enabled = false;
      });
    }

    private void VUnlockControl(object sender) {
      this.Invoke((MethodInvoker)delegate {
      Control control = sender as Control;
      if (control == null) return;
      control.Enabled = true;
      });
    }

    private async void CUpdateChecker(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

      Updater up = new Updater();

      await up.Create();
            
      if (App.err.CheckIsError()) {
        CShowErrorIfNeeded();
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

      VTabAccessChange(AppTabPageUploader, false);
      VTabAccessChange(AppTabPageProgress, false);
    }

    private bool CLock() {
      if (App.BeginWork()) {
        App.err.Reset();
        VLock();
        return true;
      }

      return false;      
    }

    private void VLock() {
      this.Invoke((MethodInvoker)delegate {
      this.Text += " [...]";
      Application.DoEvents();
      });
    }

    private void CUnlock() {
      App.EndWork();
      VUnlock();
    }

    private void VUnlock() {
      this.Invoke((MethodInvoker)delegate {
      this.Text = App.NAME;
      });
    }

    private void CShowErrorIfNeeded(string errMessage = "") {
      if (App.err.CheckIsError()) {
        string s = "Сообщение: " + Error.GetMessage(App.err.LastErrorCode);

        if (App.err.ExtMessage.Length > 1) s += "\nИнформация: " + App.err.ExtMessage;
        if (errMessage.Length > 1) s += "\n\nДоп. информация: " + errMessage;
        
        VShowMessage("Ошибка приложения! (" + App.err.ErrCount + ")", s);
      }
    }

    private async void CAuth(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

      FormModal d = new FormModal("Авторизация", "Введите SID сессии:");

      bool okFlag = false;

      if (d.ShowDialog() == DialogResult.OK) {      
        App.session = new Session();
        await App.session.Create(d.InputText);
        
        if (App.session.Valid) {
          okFlag = true;
        } else {
          App.err.SetError(Error.Codes.INCORRECT_SESSION, this.ToString(), "Невалидный sid");
        }
        
        if (okFlag) {
          VShowMessage("Авторизация", "Привет, " + App.session.UserName + "!");

          List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
          list.Add(new KeyValuePair<string, string>("0", "(кликни дважды для загрузки)"));
          VUploadDirsSet(list);

          VTabAccessChange(AppTabPageAuth, false);
          VTabAccessChange(AppTabPageUploader, true);
          VLockControl(GroupBoxSpacDirs);
        }

      } else {
        App.err.SetError(Error.Codes.INCORRECT_SESSION, this.ToString(), "Неправильный формат");
      }

      CShowErrorIfNeeded();
      VUnlockControl(sender);
      CUnlock();
    }
    
    private void VUploadDirsSet(List<KeyValuePair<string, string>> dict) {
      ListViewDirs.Items.Clear();

      foreach (KeyValuePair<string, string> item in dict) {
        ListViewDirs.Items.Add(new ListViewItem(new string[] { item.Key, item.Value }));
      }
    }
  
    private async void CChangeFileDir(object sender, MouseEventArgs e) {
      CLock();
      VLockControl(sender);

      List<KeyValuePair<string, string>> dict = new List<KeyValuePair<string, string>>();

      string dirID = ListViewDirs.SelectedItems[0].Text;

      dict = await MixxerAPI.GetMusicDirs(App.session.UserName, dirID);
        
      dict.Insert(0, new KeyValuePair<string, string>("0", "< корневая папка >"));

      CShowErrorIfNeeded();

      VUploadDirsSet(dict);
      VUnlockControl(sender);
      CUnlock();
    }

    private void VTabAccessChange(TabPage page, bool newValue) {
      this.Invoke((MethodInvoker)delegate {
      foreach (Control control in page.Controls) {
        control.Enabled = newValue;
      }

      if (newValue) AppTabControl.SelectedTab = page;
      });
    }

    private async void CUpload(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

      if (ListViewDirs.Items.Count < 1) {
        App.err.SetError(Error.Codes.WRONG_GUI_OP, this.ToString(), "Incorrect dir selector running!");
        CShowErrorIfNeeded();
        CUnlock();
        return;
      }

      if (ListViewDirs.SelectedIndices.Count < 1) {
        VShowMessage("Ошибка", "Ничего не выбрано!");
        CUnlock();
        VUnlockControl(sender);
        return;
      }
      
      string dirID = ListViewDirs.Items[ListViewDirs.SelectedIndices[0]].Text;

      VTabAccessChange(AppTabPageUploader, false);
      VTabAccessChange(AppTabPageProgress, true);

      List<string> files = new List<string>();

      foreach (string item in ListBoxFiles.Items) {
        files.Add(item);
      }
                
      var progressIndicatorCurrent = new Progress<int>(VProgressBarCurrentUpdate);
      VProgressBarCurrentUpdate(0);
      ProgressBarCurrent.Minimum = 0;
      ProgressBarCurrent.Maximum = 100;
           
      var progressIndicatorTotal = new Progress<int>(VProgressBarTotalUpdate);
      VProgressBarTotalUpdate(0);
      ProgressBarTotal.Minimum = 0;
      ProgressBarTotal.Maximum = files.Count;

      var progressIndicatorCurrentTask = new Progress<string>(VCurrentWorkUploadUpdate);
      var progressIndicatorCurrentLog = new Progress<string>(VCurrentWorkLogUploadUpdate);

      await UploadMusic(files, progressIndicatorTotal, progressIndicatorCurrent,
        progressIndicatorCurrentTask, progressIndicatorCurrentLog, dirID).ConfigureAwait(false);
     
      CShowErrorIfNeeded();

      VShowMessage("Загрузка завершена", "Загрузка завершена!\nЕсли хотите загрузить ещё - перезапустите программу.");

        VTabAccessChange(AppTabPageProgress, false);
        VTabAccessChange(AppTabPageAbout, true);
        VUnlockControl(sender);
        CUnlock();

    }

    private async Task UploadMusic(List<string> files, IProgress<int> progressTotal,
    IProgress<int> progressCurrent, IProgress<string> currentWork, IProgress<string> log, string dirID) {
      log.Report("Загрузились...");

      EventHandler<HttpProgressEventArgs> currnetProgressHandler = (_s, _e) => {
        progressCurrent.Report(_e.ProgressPercentage);
      };

      if (dirID == "0") dirID = "-" + App.session.UserID;

      progressTotal.Report(0);
      App.net.progressHandler.HttpSendProgress += currnetProgressHandler;

      log.Report("Начали...");
      try {
        string url = "";

        Debug.WriteLine("------ [0]");
        
        for (int i = 0; i < files.Count; i++) {
          Debug.WriteLine("------ [1] {" + i + "}");
          FileInfo f = new FileInfo(files[i]);
          url = "";

          currentWork.Report("Получаем URL загрузки...");
          
          Debug.WriteLine("------ [2] {" + i + "}");
          url = await MixxerAPI.GetUploadUrl(App.session.SID + "_" + i);
          

          progressCurrent.Report(0);
          Debug.WriteLine("------ [3] {" + i + "}");
          log.Report("Код ответа: " + App.net.LastCodeAnswer + ", новый URL: " + url);

          Debug.WriteLine("------ [4] {" + i + "}");
          List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
          keys.Add(new KeyValuePair<string, string>("add", "1"));
          keys.Add(new KeyValuePair<string, string>("dir", dirID));
          keys.Add(new KeyValuePair<string, string>("sid", App.session.SID));
          keys.Add(new KeyValuePair<string, string>("file", "1"));
          keys.Add(new KeyValuePair<string, string>("name", App.session.UserName));
          keys.Add(new KeyValuePair<string, string>("p", "1"));
          keys.Add(new KeyValuePair<string, string>("LT", ""));
          
          currentWork.Report("Ссылка получена...");
          await Task.Delay(2500);
          currentWork.Report("Загружаем файл...");
          log.Report("Начали загружать [" + i + "] " + " файл...");
          Debug.WriteLine("------ [5] {" + i + "}");
          int opCode = await App.net.PostMultipart(url, keys, new KeyValuePair<string, string>("myFile", f.ToString()));
          
          Debug.WriteLine("------ [6] {" + i + "}");
          log.Report("Закончили загружать файл[" + i + "] " + " файл... Код ответа: " + App.net.LastCodeAnswer);
          log.Report("Результат: " + Error.GetMessage(opCode));
            
          progressTotal.Report(i + 1);
          log.Report("Завершение итерации...");
          currentWork.Report("Ждём...");
          Debug.WriteLine("------ [7] {" + i + "}");
          await Task.Delay(2500);
        }       
      } catch (Exception e) {
        log.Report("[ИСКЛЮЧЕНИЕ!] " + e.Message + " от " + e.Source);        
      }

      App.net.progressHandler.HttpReceiveProgress -= currnetProgressHandler;
      currentWork.Report("Загрузка завершена");
      log.Report("Отключение потока...");

      Debug.WriteLine("OK");
      return;
    }

    private void VProgressBarCurrentUpdate(int value) {
      ProgressBarCurrent.Value = value;
    }

    private void VCurrentWorkLogUploadUpdate(string value) {      
      TextBoxUploadLog.AppendText("\r\n" + value);
    }

    private void VProgressBarTotalUpdate(int value) {
      LabelTotalWork.Text = "Общий прогресс: " + value + " из " + ProgressBarTotal.Maximum;
      ProgressBarTotal.Value = value;
    }

    private void VCurrentWorkUploadUpdate(string value) {
      LabelCurrentWork.Text = value;
    }

    private async void ButtonDebug_Click(object sender, EventArgs ev) {
      //CDebug(sender, ev);

      for (int i = 0; i < 10; i++) {

        await Task.Factory.StartNew(() => {
          Debug.WriteLine("[1] Thread 1 begin");
          Thread.Sleep(1500);
          Debug.WriteLine("[2] Thread 2 end");
        });

        Debug.WriteLine("[3] Main 0 between");

        await Task.Factory.StartNew(() => {
          Debug.WriteLine("[4] Thread 2 begin");
        });
        Debug.WriteLine("[5] Main 0 end");
      }

      return;
      /*try {
        string url = App.api.GetUploadUrl(App.session.SID);

        List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string,string>>();

        keys.Add(new KeyValuePair<string,string>("add", "1"));
        keys.Add(new KeyValuePair<string,string>("dir", "<<< ID >>>"));
        keys.Add(new KeyValuePair<string,string>("sid", App.session.SID));
        keys.Add(new KeyValuePair<string,string>("file", "1"));
        keys.Add(new KeyValuePair<string,string>("name", App.session.UserName));
        keys.Add(new KeyValuePair<string,string>("p", "1"));
        keys.Add(new KeyValuePair<string,string>("LT", ""));

        App.net.PostMultipart(url, keys, new KeyValuePair<string, string>("myFile", "c:/1.mp3"));
      } catch (Exception e) {
        //
      } finally {
        CShowErrorIfNeeded("");
      }*/
    }

    private void FormApp_FormClosed(object sender, FormClosedEventArgs e) {
      //CGUIClose(sender, e);
    }

    private void CGUIClose(object sender, FormClosedEventArgs e) {
      App.net.Free();
    }

    private void FormApp_Load(object sender, EventArgs e) {
      // go here 
    }

    private void ButtonCancel_Click(object sender, EventArgs e) {
    }
  }

  public static class App {
    // Const
    public static class Const {
      public const int maxFileSize = 62914560;
    }

    public static readonly string NAME = "Spaces.D.MusicUploader";
    public static readonly string AUTHOR = "DJ_miXxXer";
    public static readonly string AUTHOR_URL = "http://spaces.ru/mysite/?name=DJ_miXxXer";

    public const int VERSION = 2;
    public static readonly string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0 MixxerUploader/0." + VERSION;

    public static Session session;
    public static Networker net;
    public static Error err;

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

      Debug.WriteLine("[WARNING] Try end work, but work doesnt started");
      return false;
    }    
  }

  public static class MixxerAPI {
    public static async Task<string> GetUserNameById(string id) {
      await App.net.Get("http://" + id + ".spaces.ru/");

      return App.net.GetValueByParam("name");
    }

    // 6 - music
    public static async Task<string> GetUploadUrl(string sid, string type = "6") {
      List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string,string>>();

      postData.Add(new KeyValuePair<string, string>("Type", "6"));
      postData.Add(new KeyValuePair<string, string>("method", "getUploadInfo"));
      postData.Add(new KeyValuePair<string, string>("url", DateTime.UtcNow.ToString()));

      Debug.WriteLine("START GET [" + sid + "]");

      await App.net.Post("http://spaces.ru/api/files/", postData);
      //await net.Post("http://httpbin.org/post", postData).ConfigureAwait(false);

      Debug.WriteLine("END GET ");

      String answer = "";

      Debug.WriteLine("ANSWER " + App.net.Answer.ToString());

      try {
        var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(App.net.Answer.ToString());
        answer = (string)dict["url"];
      } catch (Exception e) {
        App.err.SetError(Error.Codes.WRONG_PARSE_DATA, "Empty url parse [" + e.Message + "]");
      }
           
      return answer;
    }

    public static async Task<List<KeyValuePair<string, string>>> GetMusicDirs(string userName, string dirId) {
      List<KeyValuePair<string, string>> dict = new List<KeyValuePair<string, string>>();

      string url = "http://spaces.ru/music/?r=main/index&name=" + userName;
      if (dirId != "0") url = "http://spaces.ru/music/?Dir=" + dirId;

      await App.net.Get(url);

      MatchCollection m = Regex.Matches(App.net.Answer, @"<a.*?</a>");

      if (m.Count == 0) App.err.SetError(Error.Codes.WRONG_PARSE_DATA, "MixxerAPI.GetMusicDirs", "Empty parse collection");

      foreach (Match item in m) {
        Match temp = Regex.Match(item.Value, @"Dir=(\d*).+?hover" + '"' + ">(.*?)</span>");

        if (temp.Groups.Count == 3 && temp.Groups[1].Value != "") {
          dict.Add(new KeyValuePair<string, string>(temp.Groups[1].Value, temp.Groups[2].Value));
        }
      }
     
      Match nav = Regex.Match(App.net.Answer, @"<div class=" + '"' +
                             "location-bar" + '"' + ".*?<a.+?</a>.+?</div>");

      if (nav.Length > 0) {
        MatchCollection navCounter = Regex.Matches(nav.Value, "<*.a>");

        if (navCounter.Count > 3) {
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
    public bool Valid {
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

    public Session() {
      
    }

    public async Task Create(string sid) {
      _ResetSessionData();

      if (_CheckInputSID(sid)) {
        this.sid = sid;

        try {
          await App.net.Get("http://spaces.ru/settings/?sid=" + sid);

          string temp = App.net.GetCookieValueByName("user_id");
          if (temp != "") {
            
            userID = temp;
            valid = true;

            userName = await MixxerAPI.GetUserNameById(UserID);
          }
        } catch (Exception e) {
          App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Session __constr err [e: " + e.Message + "]");
        }
      }
    }
  }

  public class Error {
    public static class Codes {
      public const int NO_ERROR = 0;
      public const int WRONG_PARSE_DATA = 1;
      public const int TRY_COMMON_FAIL = 2;
      public const int INCORRECT_SESSION = 3;
      public const int WRONG_GUI_OP = 4;
      public const int ERROR_TIMEOUT = 5;
    }

    public Error() {
      lastErrorCode = Codes.NO_ERROR;
      extMessage = "";
      place = "";
    }
   
    // Data
    private int lastErrorCode = 0;
    public int LastErrorCode {
      get {
        return lastErrorCode;
      }
    }

    private string extMessage = "";
    public string ExtMessage {
      get {
        return extMessage;
      }
    }

    private string place = "";
    public string Place {
      get {
        return place;
      }
    }

    private int errCount = 0;
    public int ErrCount {
      get {
        return errCount;
      }
    }

    // Func
    public bool CheckIsError() {
      if (lastErrorCode != Codes.NO_ERROR) return true;
      return false;
    }

    public void SetError(int code, string _place = "", string _extMessage = "") {
      errCount++;
      lastErrorCode = code;
      extMessage = _extMessage;
      place = _place;
      Debug.WriteLine("[ERROR] code: " + code + ", at: " + place + ", message: " + extMessage);

      if (code == Codes.NO_ERROR) {
        Debug.WriteLine("[WTF U DOING, DUDE???]");
      }
    }

    public void Reset() {
      lastErrorCode = Codes.NO_ERROR;
      errCount = 0;
      extMessage = "";
      place = "";
    }

    public static string GetMessage(int code) {
      switch (code) {
        case Codes.NO_ERROR: {
          return "Нет ошибки";
        }

        case Codes.WRONG_PARSE_DATA: {
          return "Ошибка чтения данных";
        }

        case Codes.TRY_COMMON_FAIL: {
          return "Общая ошибка приложения";
        }

        case Codes.INCORRECT_SESSION: {
          return "Недействительная ссессия";
        }

        case Codes.WRONG_GUI_OP: {
          return "Ошибка интерфейса";
        }

        default: {
          return "Неизвестная ошибка (код: " + code + ")";
        }
      }
    }    
  }


  public class Networker {
    // HTTP libs vars
    private HttpClientHandler handler;
    private HttpResponseMessage response;
    public HttpClient client;

    // public for binding external handlers
    public ProgressMessageHandler progressHandler;
        
    // HTTP opts
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
      string url = response.RequestMessage.RequestUri.Query.Substring(1);

      string []param = url.Split('&');

      foreach (string item in param) {
        if (item.Length < 1) continue;

        string []temp = item.Split('=');

        if (temp[0] == name) return temp[1];
	    }

      return "";
    }

    public Networker(string useragent) {
      cookies = new CookieContainer();
      postParams = new NameValueCollection();

      handler = new HttpClientHandler();

      handler.AllowAutoRedirect = true;
      handler.MaxAutomaticRedirections = 3;

      handler.MaxRequestContentBufferSize = 4;

      handler.UseCookies = true;
      handler.CookieContainer = cookies;

      handler.Credentials = CredentialCache.DefaultCredentials;
      handler.UseDefaultCredentials = true;

      progressHandler = new ProgressMessageHandler();

      progressHandler.HttpSendProgress += sendProgress;
      progressHandler.HttpReceiveProgress += recvProgress;

      client = HttpClientFactory.Create(handler, progressHandler);
      client.DefaultRequestHeaders.UserAgent.ParseAdd(useragent);
      client.Timeout = TimeSpan.FromMinutes(30);
      
      client.DefaultRequestHeaders.Accept.Clear();
      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
    }

    private void recvProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("NET.Recive: " + e.BytesTransferred + " / total: " + e.TotalBytes);
    }

    private void sendProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("Net.Send: " + e.BytesTransferred + " / total: " + e.TotalBytes);
    }
     
    private void _Clear() {
      answer = "";
      lastCodeAnswer = -1;
    }

    public async Task Get(string url) {
      try {
        _Clear();
        response = await client.GetAsync(url).ConfigureAwait(false);

        using (var stream = new StreamReader(response.Content.ReadAsStreamAsync().Result)) {
          answer = stream.ReadToEnd();
        }

        lastCodeAnswer = (int)response.StatusCode;
      } catch {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка get запроса");
        throw;
      }
    }

    public async Task Post(string url, List<KeyValuePair<string, string>> postParams) {     
      try {
        _Clear();
        using (var postHeaders = new FormUrlEncodedContent(postParams)) {
          response = await client.PostAsync(url, postHeaders).ConfigureAwait(false);

          using (var stream = new StreamReader(response.Content.ReadAsStreamAsync().Result)) {
            answer = stream.ReadToEnd();
          }

          lastCodeAnswer = (int)response.StatusCode;
        }        
      } catch {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка post запроса");
        throw;
      }
    }

    public async Task<int> PostMultipart(string url, List<KeyValuePair<string, string>> postParams, KeyValuePair<string, string> fileData) {
      try {
        _Clear();
        using (var contentData = new MultipartFormDataContent()) {
          foreach (var item in postParams) {
            contentData.Add(new StringContent(item.Value), item.Key);
          }

          FileInfo f = new FileInfo(fileData.Value);
          contentData.Add(new ByteArrayContent(File.ReadAllBytes(fileData.Value)), fileData.Key, f.Name);

          response = await client.PostAsync(url, contentData);

          using (var stream = new StreamReader(response.Content.ReadAsStreamAsync().Result)) {
            answer = stream.ReadToEnd();
          }

          lastCodeAnswer = (int)response.StatusCode;
        }
      } catch (TimeoutException) {
        return Error.Codes.ERROR_TIMEOUT;

      } catch (TaskCanceledException) {
        return Error.Codes.ERROR_TIMEOUT;

      } catch (Exception e) {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Неизвестная ошибка (" + e.Message + ")");
        throw;
      }
      return Error.Codes.NO_ERROR;
    }

    public void Free() {
      if (progressHandler != null) progressHandler.Dispose();
      if (handler != null) handler.Dispose();
      if (response != null) response.Dispose();
      if (client != null) client.Dispose();      
    }

    public string GetCookieValueByName(string name) {
      try {
        foreach (Cookie c in cookies.GetCookies(new Uri("http://" + response.RequestMessage.RequestUri.Host))) {
          if (c.Name == name) return c.Value;
        }
      } catch (Exception e) {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка чтения кук (" + e.Message + ")");
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

    public async Task Create() {  
      try {
        await App.net.Get(UPDATE_LINK);
               
        Match m = Regex.Match(App.net.Answer, @"###LASTVERSION:(\d)");

        if (m.Groups.Count == 2) {
          lastVersion = Convert.ToInt32(m.Groups[1].Value);
        } else {
          App.err.SetError(Error.Codes.WRONG_PARSE_DATA, this.ToString(), "Err count: " + m.Groups.Count);
        }      
      } catch (Exception e) {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка проверки обновления");
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

