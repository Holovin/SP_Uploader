using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

    private bool CNETVersionCheck() {
	    using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
		    int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
		    if (releaseKey >= App.Const.MIN_NET_VERSION) {
          return true;
		    }
	    }

      return false;
    }      

    private void LabelAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start(App.Const.AUTHOR_URL);
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
      if (!CNETVersionCheck()) {
        VShowMessage("Ошибка запуска", "Для запуска приложения требуется установленный\n.NET Framework 4.5 и/или выше. Приложение будет закрыто.");
        this.Close();
      }

      App.net = new Networker(App.Const.UA);
      App.err = new Error();
      App.session = new Session();

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
        this.Invoke((MethodInvoker)delegate {
          ListBoxFiles.Items.AddRange(itemCache.ToArray());
        });

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

        this.Invoke((MethodInvoker)delegate {
          ListBoxFiles.Items.AddRange(itemCache.ToArray());
        });

        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (files.Length - added));
      }
      CUnlock();
    }

    private void VUpdateFilesInfo() {
      this.Invoke((MethodInvoker)delegate {
        float size = 0;

        if (ListBoxFiles.Items.Count < 1) VLockControl(GroupBoxSpacDirs);
        else VUnlockControl(GroupBoxSpacDirs);

        foreach (string item in ListBoxFiles.Items) {
          FileInfo f = new FileInfo(item);
          size += f.Length / (1024 * 1024);
        }

        LabelFilesInfo.Text = "Файлов: " + ListBoxFiles.Items.Count +
          " | Общий размер: " + size + " МБ";
      });
    }

    private void VUpdateText(int newVersion = 0) {
      this.Invoke((MethodInvoker)delegate {
        String temp = "Текущая версия: " + App.Const.VERSION + '\n' + "Актуальная версия: ";

        if (newVersion != 0) temp += newVersion;
        else temp += "?";

        LabelVersion.Text = temp;
      });
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
        CShowErrorIfNeeded("");
      } else {
        VShowMessage("Проверка обновлений", up.CompareVersions(App.Const.VERSION));
      }

      VUpdateText(up.LastVersion);
      VUnlockControl(sender);
      CUnlock();
    }

    private void VShowMessage(string caption, string message) {
      this.Invoke((MethodInvoker)delegate {
        MessageBox.Show(message, caption);
      });
    }

    private void VClearFilesList() {
      this.Invoke((MethodInvoker)delegate {
        ListBoxFiles.Items.Clear();
      });
    }

    private void VGUIInit() {
      this.LabelAbout.Text = App.Const.NAME;

      if (App.DEV_MODE_ENABLED) ButtonDebug.Visible = true;
      else ButtonDebug.Visible = false;

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
        this.Text = VGetAppLabel();
      });
    }

    private void CShowErrorIfNeeded(string debugMessage = "") {
      if (App.err.CheckIsError()) {
        string s = "Сообщение: " + Error.GetMessage(App.err.LastErrorCode);

        if (App.err.ExtMessage.Length > 1) s += "\nИнформация: " + App.err.ExtMessage;

        if (App.DEV_MODE_ENABLED) {
          if (debugMessage.Length > 1) s += "\n\nDebug msg: " + debugMessage;
          if (App.err.Place.Length > 1) s += "\nAt: " + App.err.Place;
        }

        VShowMessage("Ошибка приложения! (c: " + App.err.ErrCount + ")", s);
      }

      App.err.Reset();
    }

    private async void CAuth(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

      FormModal d = new FormModal("Авторизация", "Введите SID сессии:");
       
      if (d.ShowDialog() == DialogResult.OK) {
        await App.session.Create(d.InputText);
        VAfterAuth();
      }

      CShowErrorIfNeeded();
      VUnlockControl(sender);
      CUnlock();
    }

    private void VAfterAuth() {
      if (!App.session.Valid) {
        App.err.SetError(Error.Codes.INCORRECT_SESSION, this.ToString(), "Невалидный sid");
        return;
      }

      VShowMessage("Авторизация", "Привет, " + App.session.UserName + "!");

      List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
      list.Add(new KeyValuePair<string, string>("0", "(кликни дважды для загрузки)"));
      VUploadDirsSet(list);

      VTabAccessChange(AppTabPageAuth, false);
      VTabAccessChange(AppTabPageUploader, true);
      VLockControl(GroupBoxSpacDirs);
    }

    private void VUploadDirsSet(List<KeyValuePair<string, string>> dict) {
      ListViewDirs.Items.Clear();

      foreach (KeyValuePair<string, string> item in dict) {
        ListViewDirs.Items.Add(new ListViewItem(new string[] { item.Key, item.Value }));
      }
    }

    private async void CChangeFileDir(object sender, EventArgs e) {
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
      if (App.DEV_MODE_ENABLED) return;

      this.Invoke((MethodInvoker)delegate {
        foreach (Control control in page.Controls) {
          control.Enabled = newValue;
          control.Visible = newValue;
        }

        if (newValue) AppTabControl.SelectedTab = page;
      });
    }

    private string VGetAppLabel() {
      return App.Const.NAME + " v0." + App.Const.VERSION + (App.DEV_MODE_ENABLED == true ? " [developer mode]" : "");
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

      var progressIndicatorCurrent = new Progress<HttpProgressEventArgs>(VProgressBarCurrentUpdate);
      VProgressBarCurrentUpdate(0, 0, 0);
      ProgressBarCurrent.Minimum = 0;
      ProgressBarCurrent.Maximum = 100;

      var progressIndicatorTotal = new Progress<int>(VProgressBarTotalUpdate);
      VProgressBarTotalUpdate(0);
      ProgressBarTotal.Minimum = 0;
      ProgressBarTotal.Maximum = files.Count;

      var progressIndicatorCurrentTask = new Progress<string>(VCurrentWorkUploadUpdate);
      var progressIndicatorCurrentLog = new Progress<string>(VCurrentWorkLogUploadUpdate);

      try {
        await UploadMusic(files, progressIndicatorTotal, progressIndicatorCurrent,
          progressIndicatorCurrentTask, progressIndicatorCurrentLog, dirID, App.cts.Token);

        VShowMessage("Загрузка завершена", "Загрузка завершена!\nЕсли хотите загрузить ещё - перезапустите программу.");

      } catch (OperationCanceledException) {
        VCurrentWorkLogUploadUpdate("Остановлено... (отмена пользователем)");
        VShowMessage("Загрузка завершена", "Операция отменена пользователем");

      } catch (Exception) {
        CShowErrorIfNeeded("Error at end load");
      }

      VUnlockControl(sender);
      CUnlock();
    }

    private void VProgressBarTaskBarSetValue(double current, double max) {
      TaskbarProgress.SetValue(App.winHandler, current, max);
    }

    private void VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates state) {
      TaskbarProgress.SetState(App.winHandler, state);
    }

    private async Task UploadMusic(List<string> files, IProgress<int> progressTotal,
      IProgress<HttpProgressEventArgs> progressCurrent, IProgress<string> currentWork, IProgress<string> log, string dirID,
      CancellationToken ct) {

      const int MAX_ERR_COUNT = 3;
      EventHandler<HttpProgressEventArgs> currnetProgressHandler = (_s, _e) => {
        progressCurrent.Report(_e);
      };

      // Fix spaces root dir
      if (dirID == "0") dirID = "-" + App.session.UserID;

      // Init GUI progressbars
      progressTotal.Report(0);
      App.net.progressHandler.HttpSendProgress += currnetProgressHandler;
      log.Report("Запуск...");

      // Init app
      string url = "";
      int i = 0, errorsCount = 0;

      while (i < files.Count && errorsCount < MAX_ERR_COUNT) {
        ct.ThrowIfCancellationRequested();

        VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates.Normal);
        log.Report("__________________________");

        try {
          FileInfo f = new FileInfo(files[i]);
          url = string.Empty;

          log.Report("Файл #" + (i + 1) + ": " + f.Name);
          currentWork.Report("Получаем URL загрузки...");
          progressCurrent.Report(new HttpProgressEventArgs(0, null, 0, 0));

          url = await MixxerAPI.GetUploadUrl(App.session.SID + "_" + i);

          if (url == string.Empty || App.err.LastErrorCode == Error.Codes.WRONG_PARSE_DATA) {
            log.Report("[Ошибка " + errorsCount + "] Ссылка не получена...");
            errorsCount++;
            continue;
          }

          log.Report("Ссылка для файла получена...");

          List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string, string>>();
          keys.Add(new KeyValuePair<string, string>("add", "1"));
          keys.Add(new KeyValuePair<string, string>("dir", dirID));
          keys.Add(new KeyValuePair<string, string>("sid", App.session.SID));
          keys.Add(new KeyValuePair<string, string>("file", "1"));
          keys.Add(new KeyValuePair<string, string>("name", App.session.UserName));
          keys.Add(new KeyValuePair<string, string>("p", "1"));
          keys.Add(new KeyValuePair<string, string>("LT", ""));

          // need for void error "too fast" from spaces
          await Task.Delay(2500);

          currentWork.Report("Загружаем " + f.Name + "...");
          log.Report("Начало загрузки...");

          Error.Codes opCode = await App.net.PostMultipart(url, keys, new KeyValuePair<string, string>("myFile", f.ToString()));

          log.Report("Загрузка заврешена (" + App.net.LastCodeAnswer + ")");
          log.Report("Результат: " + Error.GetMessage(opCode));

          if (opCode == Error.Codes.NO_ERROR) {
            progressTotal.Report(i + 1);
            i++;
            VProgressBarTaskBarSetValue(i, files.Count);
          } else {
            log.Report("Пробуем ещё раз... (всего ошибок: " + errorsCount + ")");
            errorsCount++;
          }
          await Task.Delay(2500);

        } catch (OperationCanceledException) {
          log.Report("Выполняется отмена операции...");
          break;

        } catch (Exception e) {
          log.Report("Ошибка при загрузке (" + e.Message + ") от (" + e.Source + ")");
          errorsCount++;
        }
      }

      if (errorsCount == MAX_ERR_COUNT) {
        log.Report("Загрузка остановлена из-за большого количества ошибок...\n" +
                   "(результат: " + i + "/" + files.Count + ")");
      } else {
        log.Report("Загрузка завершена без ошибок");
      }

      VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates.NoProgress);
      App.net.progressHandler.HttpReceiveProgress -= currnetProgressHandler;

      currentWork.Report("Завершено...");
      log.Report("Спасибо :)");
      return;
    }

    private void VProgressBarCurrentUpdate(HttpProgressEventArgs value) {
      // Dont care about long > int convert, because we have 60MB limit upload
      VProgressBarCurrentUpdate(value.ProgressPercentage, (int)value.BytesTransferred, (int)value.TotalBytes);
    }

    private void VProgressBarCurrentUpdate(int p, int current, int total) {
      ProgressBarCurrent.Value = p;
      LabelUploadedKB.Text = (current / 1024) + " / " + (total / 1024) + " kb";
    }

    private void VCurrentWorkLogUploadUpdate(string value) {
      TextBoxUploadLog.AppendText("\r\n[" + DateTime.Now.ToString("HH:mm:ss") + "] " + value);
    }

    private void VProgressBarTotalUpdate(int value) {
      LabelTotalWork.Text = "Общий прогресс: " + value + " из " + ProgressBarTotal.Maximum;
      ProgressBarTotal.Value = value;
    }

    private void VCurrentWorkUploadUpdate(string value) {
      LabelCurrentWork.Text = value;
    }

    private void ButtonDebug_Click(object sender, EventArgs ev) {
      // [debug your code here] //
    }

    private void CGUIClose(object sender, FormClosedEventArgs e) {
      App.net.Free();
    }

    private void ListViewDirs_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        CChangeFileDir(sender, e);
      }
    }

    private void ButtonCancel_Click(object sender, EventArgs e) {
      VLockControl(sender);

      DialogResult result = MessageBox.Show("Остановить загрузку?\nОстановка будет выполнена после загрузки текущего файла", "Отмена", MessageBoxButtons.YesNo);

      if (result == DialogResult.Yes) {
        CCancelAction(sender, e);
      } else {
        VUnlockControl(sender);
      }
    }

    private void CCancelAction(object sender, EventArgs e) {
      if (App.cts.IsCancellationRequested) {
        Debug.WriteLine("[WARNING] Cancel request already is true");
      }

      App.cts.Cancel();
    }

    private void ButtonRestart_Click(object sender, EventArgs e) {
      Application.Restart();
    }

    private void ButtonLoginFirefox_Click(object sender, EventArgs e) {
      DialogResult result = MessageBox.Show("Начать поиск активных сессий?", "Требуется подтверждение", MessageBoxButtons.YesNo);

      if (result == System.Windows.Forms.DialogResult.Yes) {
        CAuthFirefox(sender, e);
      }      
    }

    private async void CAuthFirefox(object sender, EventArgs e) {
      if (!CLock()) return;
      VLockControl(sender);

      string browserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Mozilla", "Firefox", "Profiles");

      DirectoryInfo d = new DirectoryInfo(browserPath);

      List<KeyValuePair<string, string>> accounts = new List<KeyValuePair<string, string>>();

      foreach (DirectoryInfo item in d.GetDirectories()) {
        string dbPath = Path.Combine(browserPath, item.Name, Path.GetFileName("cookies.sqlite"));
        string dbQuery = @"SELECT `value` FROM `moz_cookies` WHERE `baseDomain` = ""spaces.ru"" AND `name` = ""sid""";

        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath + "; Version=3;")) {
          await connection.OpenAsync();

          using (SQLiteCommand dbCommand = new SQLiteCommand(dbQuery, connection))
          using (var dbReader = await dbCommand.ExecuteReaderAsync()) {
            while (await dbReader.ReadAsync()) {
              string sid = dbReader.GetString(0);
              await App.session.Create(sid);
              if (App.session.Valid) {
                accounts.Add(new KeyValuePair<string, string>(App.session.UserName, sid));
              }
            }
          }
        }
      }

      App.session.ResetSessionData();

      if (accounts.Count > 0) {
        FormAccounts form = new FormAccounts(accounts);
        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
          await App.session.Create(form.GetSelectedSID());
          VAfterAuth();
        }
      } else {
        App.err.SetError(Error.Codes.SESSION_PARSER_NOT_FOUND, "Firefox parser", "Не найдено активных сессий");
      }

      CShowErrorIfNeeded();
      VUnlockControl(sender);
      CUnlock();
    }
  }
     
  public static class App {
    // Set true to unlock all GUI
    public static readonly bool DEV_MODE_ENABLED = false;

    // Const
    public static class Const {
      public const int maxFileSize = 62914560;
      public static readonly string NAME = "D.MusicUploader";
      public static readonly string AUTHOR = "DJ_miXxXer";
      public static readonly string AUTHOR_URL = "http://spaces.ru/mysite/?name=DJ_miXxXer&_ref=dmapp";
      public static readonly string UA = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0 MixxerUploader/0." + VERSION;
      public static readonly string SPACES = "http://spaces.ru";

      public const int VERSION = 3;
      public const int MIN_NET_VERSION = 378389;
    }

    public static IntPtr winHandler = Process.GetCurrentProcess().MainWindowHandle;   
    public static Session session;
    public static Networker net;
    public static Error err;
    public static CancellationTokenSource cts = new CancellationTokenSource();

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

      try {
        await App.net.Post("http://spaces.ru/api/files/", postData);
      }
      catch {
        App.err.SetError(Error.Codes.NETWORK_ERROR, "API.GetUploadURL.Post", "Нет ответа от spaces.ru");
        throw;        
      }

      String answer = string.Empty;

      try {
        JObject o = JObject.Parse(App.net.Answer.ToString());
        answer = o["url"].ToString();
      } catch {
        App.err.SetError(Error.Codes.WRONG_PARSE_DATA, "API.GetUploadURL.Parse", "Некорректные данные");
      }

      if (answer == string.Empty) {
        App.err.SetError(Error.Codes.WRONG_PARSE_DATA, "API.GetUploadURL.Parse", "Некорректные данные");
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

    public void ResetSessionData() {
      App.net.ClearCookies();
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
      ResetSessionData();

      if (_CheckInputSID(sid)) {
        this.sid = sid;

        try {
          await App.net.Get("http://spaces.ru/settings/?sid=" + sid);

          string temp = App.net.GetCookieValueByName("user_id");
          if (temp != string.Empty) {
            
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
    public enum Codes {
      NO_ERROR = 0,
      WRONG_PARSE_DATA = 1,
      TRY_COMMON_FAIL = 2,
      INCORRECT_SESSION = 3,
      WRONG_GUI_OP = 4,
      ERROR_TIMEOUT = 5,
      NETWORK_ERROR = 6,
      SESSION_PARSER_NOT_FOUND = 7
    }

    public Error() {
      lastErrorCode = Error.Codes.NO_ERROR;
      extMessage = "";
      place = "";
    }
   
    // Data
    private Codes lastErrorCode = 0;
    public Codes LastErrorCode {
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

    /// <summary>
    /// Setting app-error values
    /// </summary>
    /// <param name="code">Error code (Erorr.Code.*)</param>
    /// <param name="_place">[Debug] Class name</param>
    /// <param name="_extMessage">Message to user</param>
    public void SetError(Codes code, string _place = "", string _extMessage = "") {
      errCount++;
      lastErrorCode = code;
      extMessage = _extMessage;
      place = _place;
      Debug.WriteLine("[ERROR] code: " + code + ", at: " + place + ", message: " + extMessage);

      if (code == Codes.NO_ERROR) {
        Debug.WriteLine("[ERROR MAIN] cant set NO.ERROR as error!");
        throw new Exception("Curve hands coder exception :)");
      }
    }

    public void Reset() {
      lastErrorCode = Codes.NO_ERROR;
      errCount = 0;
      extMessage = "";
      place = "";
    }

    public static string GetMessage(Codes code) {
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

        case Codes.NETWORK_ERROR: {
          return "Ошибка сети";
        }

        case Codes.SESSION_PARSER_NOT_FOUND: {
          return "Поиск завершился неудачей";
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
    private HttpClient client;
    private Uri uri;

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

    public void ClearCookies() {
      if (cookies == null) return;

      // how get ALL cookies? :)
      foreach (Cookie co in cookies.GetCookies(new Uri(App.Const.SPACES))) {
        co.Expired = true;
      }
    }
   
    public string GetValueByParam(string name) {
      string url = uri.Query.Substring(1);

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

      handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
      handler.AllowAutoRedirect = true;
      handler.MaxAutomaticRedirections = 3;
     
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
      client.DefaultRequestHeaders.ExpectContinue = false;
    }

    private void recvProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("NET.Recive: " + e.BytesTransferred + " / total: " + e.TotalBytes + ".");
    }

    private void sendProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("Net.Send: " + e.BytesTransferred + " / total: " + e.TotalBytes + ".");
    }
     
    private void _Clear() {
      answer = "";
      lastCodeAnswer = -1;
    }

    public async Task Get(string url) {
      try {
        _Clear();
        using (var response = await client.GetAsync(url).ConfigureAwait(false)) {
          using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
            answer = stream.ReadToEnd();           
          }
          uri = response.RequestMessage.RequestUri;
          lastCodeAnswer = (int)response.StatusCode;
        }        
      } catch {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка get запроса");
        throw;
      }
    }

    public async Task Post(string url, List<KeyValuePair<string, string>> postParams) {     
      try {
        _Clear();
        using (var postHeaders = new FormUrlEncodedContent(postParams)) {
          using (var response = await client.PostAsync(url, postHeaders).ConfigureAwait(false)) {
            using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
              answer = stream.ReadToEnd();
            }
            uri = response.RequestMessage.RequestUri;
            lastCodeAnswer = (int)response.StatusCode;
          }          
        }        
      } catch {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка post запроса");
        throw;
        
      }
    }

    public async Task<Error.Codes> PostMultipart(string url, List<KeyValuePair<string, string>> postParams, KeyValuePair<string, string> fileData) {
      try {
        _Clear();
        using (var contentData = new MultipartFormDataContent()) {
          foreach (var item in postParams) {
            contentData.Add(new StringContent(item.Value), item.Key);
          }

          FileInfo f = new FileInfo(fileData.Value);
          contentData.Add(new StreamContent(File.Open(fileData.Value, FileMode.Open)), fileData.Key, f.Name);

          using (var response = await client.PostAsync(url, contentData)) {            
            using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
              answer = stream.ReadToEnd();
            }
            uri = response.RequestMessage.RequestUri;
            lastCodeAnswer = (int)response.StatusCode;
          }          
        }
      } catch (TimeoutException) {
        return Error.Codes.ERROR_TIMEOUT;

      } catch (TaskCanceledException) {
        return Error.Codes.ERROR_TIMEOUT;

      } catch (Exception e) {
        App.err.SetError(Error.Codes.TRY_COMMON_FAIL, this.ToString(), "Ошибка postM запроса (" + e.Message + ")");
        throw;
      }
      return Error.Codes.NO_ERROR;
    }

    public void Free() {
      if (progressHandler != null) progressHandler.Dispose();
      if (handler != null) handler.Dispose();
      if (client != null) client.Dispose();      
    }

    public string GetCookieValueByName(string name) {
      try {
        foreach (Cookie c in cookies.GetCookies(new Uri("http://" + uri.Host))) {
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
      } catch (Exception) {
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

