using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
      CUpdateChecker(sender);
    }

    private void FormApp_Shown(object sender, EventArgs e) {      
      CguiInit();
    }

    private static bool CnetVersionCheck() {
	    using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\")) {
	      if (ndpKey == null) return false;
	      var releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
	      if (releaseKey >= App.Const.MinNetVersion) {
	        return true;
	      }
	    }

      return false;
    }      

    private void LabelAuthor_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start(App.Const.AuthorUrl);
    }

    private void ButtonAddFiles_Click(object sender, EventArgs e) {
      CAddFiles(sender);
    }

    private void ButtonAddDirectory_Click(object sender, EventArgs e) {
      CAddFilesFromDir();
    }

    private void ButtonFilesClear_Click(object sender, EventArgs e) {
      CClearFilesList();
    }

    private void ButtonAuth_Click(object sender, EventArgs e) {
      CAuth(sender);
    }

    private void ListViewDirs_MouseDoubleClick(object sender, MouseEventArgs e) {
      CChangeFileDir(sender);
    }

    private void ButtonUpload_Click(object sender, EventArgs e) {
      CUpload(sender);
    }

    private void CguiInit() {
      if (!CnetVersionCheck()) {
        VShowMessage("Ошибка запуска", "Для запуска приложения требуется установленный\n.NET Framework 4.5 и/или выше. Приложение будет закрыто.");
        Close();
      }

      App.Net = new Networker(App.Const.Ua);
      App.Err = new Error();
      App.Session = new Session();

      VguiInit();
      VUpdateText();
    }

    private async void CAddFiles(object sender) {
      if (!CLock()) return;
      VLockControl(sender);

      var modal = new OpenFileDialog {
        AddExtension = true,
        CheckFileExists = true,
        CheckPathExists = true,
        DefaultExt = "mp3",
        Filter = @"Музыкальные файлы | *.mp3",
        InitialDirectory = Directory.GetCurrentDirectory(),
        Multiselect = true,
        Title = @"Выберите файлы"
      };

      if (modal.ShowDialog() == DialogResult.OK) {
        FileInfo f;
        var added = 0;

        var itemCache = new List<string>();
        await Task.Factory.StartNew(() => {
          foreach (var item in modal.FileNames) {
            f = new FileInfo(item);
            if (f.Length >= App.Const.MaxFileSize) continue;
            added++;
            itemCache.Add(item);
          }
        }, TaskCreationOptions.LongRunning);        
        // ReSharper disable once CoVariantArrayConversion
        ListBoxFiles.Items.AddRange(itemCache.ToArray());

        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (modal.FileNames.GetLength(0) - added));
      }

      VUnlockControl(sender);
      CUnlock();
    }

    private async void CAddFilesFromDir() {
      if (!CLock()) return;
      var modal = new FolderBrowserDialog
      {
        Description = @"Выберите папку с файлами",
        ShowNewFolderButton = false,
        RootFolder = Environment.SpecialFolder.MyComputer
      };


      if (modal.ShowDialog() == DialogResult.OK) {
        FileInfo f;
        var added = 0;

        var itemCache = new List<string>();
        string[] files = null;
        await Task.Factory.StartNew(() => {
          files = Directory.GetFiles(modal.SelectedPath, "*.mp3", SearchOption.AllDirectories);

          foreach (string item in files) {
            f = new FileInfo(item);
            if (f.Length >= App.Const.MaxFileSize) continue;
            added++;
            itemCache.Add(item);
          }
        }, TaskCreationOptions.LongRunning);

        // ReSharper disable once CoVariantArrayConversion
        ListBoxFiles.Items.AddRange(itemCache.ToArray());

        VUpdateFilesInfo();
        VShowMessage("Файлы добавлены", "Добавлено файлов: " + added + "\nПропущено (>60мб!): " + (files.Length - added));
      }
      CUnlock();
    }

    private void VUpdateFilesInfo() {
      Invoke((MethodInvoker)delegate {
        if (ListBoxFiles.Items.Count < 1) VLockControl(GroupBoxSpacDirs);
        else VUnlockControl(GroupBoxSpacDirs);

        // ReSharper disable once PossibleLossOfFraction
        var size = (from string item in ListBoxFiles.Items select new FileInfo(item)).Aggregate<FileInfo, double>(0, (current, f) => current + f.Length/(1024*1024));

        LabelFilesInfo.Text = @"Файлов: " + ListBoxFiles.Items.Count +
          @" | Общий размер: " + size + @" МБ";
      });
    }

    private void VUpdateText(int newVersion = 0) {
      Invoke((MethodInvoker)delegate {
        var temp = "Текущая версия: " + App.Const.Version + '\n' + "Актуальная версия: ";

        if (newVersion != 0) temp += newVersion;
        else temp += "?";

        LabelVersion.Text = temp;
      });
    }

    private void CClearFilesList() {
      if (!CLock()) return;

      VClearFilesList();
      VUpdateFilesInfo();

      CUnlock();
    }

    private void VLockControl(object sender) {
      Invoke((MethodInvoker)delegate {
        var control = sender as Control;
        if (control == null) return;
        control.Enabled = false;
      });
    }

    private void VUnlockControl(object sender) {
      Invoke((MethodInvoker)delegate {
        var control = sender as Control;
        if (control == null) return;
        control.Enabled = true;
      });
    }

    private async void CUpdateChecker(object sender) {
      if (!CLock()) return;
      VLockControl(sender);

      var up = new Updater();
      await up.Create();

      if (App.Err.CheckIsError()) {
        CShowErrorIfNeeded();
      } else {
        VShowMessage("Проверка обновлений", up.CompareVersions(App.Const.Version));
      }

      VUpdateText(up.LastVersion);
      VUnlockControl(sender);
      CUnlock();
    }

    private void VShowMessage(string caption, string message) {
      Invoke((MethodInvoker)delegate {
        MessageBox.Show(message, caption);
      });
    }

    private void VClearFilesList() {
      Invoke((MethodInvoker)delegate {
        ListBoxFiles.Items.Clear();
      });
    }

    private void VguiInit() {
      Text = VGetAppLabel();
      LabelAbout.Text = App.Const.Name;
      ButtonDebug.Visible = App.DevModeEnabled;

      VTabAccessChange(AppTabPageUploader, false);
      VTabAccessChange(AppTabPageProgress, false);
    }

    private bool CLock() {
      if (!App.BeginWork()) return false;
      App.Err.Reset();
      VLock();
      return true;
    }

    private void VLock() {
      Invoke((MethodInvoker)delegate {
        this.Text += @" [...]";
        Application.DoEvents();
      });
    }

    private void CUnlock() {
      App.EndWork();
      VUnlock();
    }

    private void VUnlock() {
      Invoke((MethodInvoker)delegate {
        this.Text = VGetAppLabel();
      });
    }

    private void CShowErrorIfNeeded(string debugMessage = "") {
      if (App.Err.CheckIsError()) {
        var s = "Сообщение: " + Error.GetMessage(App.Err.LastErrorCode);

        if (App.Err.ExtMessage.Length > 1) s += "\nИнформация: " + App.Err.ExtMessage;

        if (App.DevModeEnabled) {
          if (debugMessage.Length > 1) s += "\n\nDebug msg: " + debugMessage;
          if (App.Err.Place.Length > 1) s += "\nAt: " + App.Err.Place;
        }

        VShowMessage("Ошибка приложения! (c: " + App.Err.ErrCount + ")", s);
      }

      App.Err.Reset();
    }

    private async void CAuth(object sender) {
      if (!CLock()) return;
      VLockControl(sender);

      var d = new FormModal("Авторизация", "Введите SID сессии:");
       
      if (d.ShowDialog() == DialogResult.OK) {
        await App.Session.Create(d.InputText);
        VAfterAuth();
      }

      CShowErrorIfNeeded();
      VUnlockControl(sender);
      CUnlock();
    }

    private void VAfterAuth() {
      if (!App.Session.Valid) {
        App.Err.SetError(Error.Codes.IncorrectSession, ToString(), "Невалидный sid");
        return;
      }

      VShowMessage("Авторизация", "Привет, " + App.Session.UserName + "!");

      var list = new List<KeyValuePair<string, string>>
      {
        new KeyValuePair<string, string>("0", "(кликни дважды для загрузки)")
      };

      VUploadDirsSet(list);

      VTabAccessChange(AppTabPageAuth, false);
      VTabAccessChange(AppTabPageUploader, true);
      VLockControl(GroupBoxSpacDirs);
    }

    private void VUploadDirsSet(IEnumerable<KeyValuePair<string, string>> dict) {
      ListViewDirs.Items.Clear();

      foreach (var item in dict) {
        ListViewDirs.Items.Add(new ListViewItem(new[] { item.Key, item.Value }));
      }
    }

    private async void CChangeFileDir(object sender) {
      CLock();
      VLockControl(sender);

      var dirId = ListViewDirs.SelectedItems[0].Text;
      var dict = await MixxerApi.GetMusicDirs(App.Session.UserName, dirId);

      dict.Insert(0, new KeyValuePair<string, string>("0", "< корневая папка >"));

      CShowErrorIfNeeded();

      VUploadDirsSet(dict);
      VUnlockControl(sender);
      CUnlock();
    }

    private void VTabAccessChange(TabPage page, bool newValue) {
      if (App.DevModeEnabled) return;

      Invoke((MethodInvoker)delegate {
        foreach (Control control in page.Controls) {
          control.Enabled = newValue;
          control.Visible = newValue;
        }

        if (newValue) AppTabControl.SelectedTab = page;
      });
    }

    private static string VGetAppLabel() {
      return App.Const.Name + " v0." + App.Const.Version + (App.DevModeEnabled ? " [developer mode]" : "");
    }

    private async void CUpload(object sender) {
      if (!CLock()) return;
      VLockControl(sender);

      if (ListViewDirs.Items.Count < 1) {
        App.Err.SetError(Error.Codes.WrongGuiOp, ToString(), "Incorrect dir selector running!");
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

      string dirId = ListViewDirs.Items[ListViewDirs.SelectedIndices[0]].Text;

      VTabAccessChange(AppTabPageUploader, false);
      VTabAccessChange(AppTabPageProgress, true);

      var files = ListBoxFiles.Items.Cast<string>().ToList();

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
          progressIndicatorCurrentTask, progressIndicatorCurrentLog, dirId, App.Cts.Token);
        VLockControl(ButtonCancel);

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
      TaskbarProgress.SetValue(App.WinHandler, current, max);
    }

    private void VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates state) {
      TaskbarProgress.SetState(App.WinHandler, state);
    }

    private async Task UploadMusic(List<string> files, IProgress<int> progressTotal,
      IProgress<HttpProgressEventArgs> progressCurrent, IProgress<string> currentWork, IProgress<string> log, string dirId,
      CancellationToken ct) {

      const int maxErrCount = 3;
      EventHandler<HttpProgressEventArgs> currnetProgressHandler = (s, e) => {
        progressCurrent.Report(e);
      };

      // Fix spaces root dir
      if (dirId == "0") dirId = "-" + App.Session.UserId;

      // Init GUI progressbars
      progressTotal.Report(0);
      App.Net.ProgressHandler.HttpSendProgress += currnetProgressHandler;
      log.Report("Запуск...");

      // Init app
      int i = 0, errorsCount = 0;

      while (i < files.Count && errorsCount < maxErrCount) {
        ct.ThrowIfCancellationRequested();

        VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates.Normal);
        log.Report("__________________________");

        try {
          var f = new FileInfo(files[i]);

          log.Report("Файл #" + (i + 1) + ": " + f.Name);
          currentWork.Report("Получаем URL загрузки...");
          progressCurrent.Report(new HttpProgressEventArgs(0, null, 0, 0));

          var url = await MixxerApi.GetUploadUrl(App.Session.Sid + "_" + i);

          if (url == string.Empty || App.Err.LastErrorCode == Error.Codes.WrongParseData) {
            log.Report("[Ошибка " + errorsCount + "] Ссылка не получена...");
            errorsCount++;
            continue;
          }
          
          log.Report("Ссылка для файла получена...");

          var keys = new List<KeyValuePair<string, string>>
          {
            new KeyValuePair<string, string>("add", "1"),
            new KeyValuePair<string, string>("dir", dirId),
            new KeyValuePair<string, string>("sid", App.Session.Sid),
            new KeyValuePair<string, string>("file", "1"),
            new KeyValuePair<string, string>("name", App.Session.UserName),
            new KeyValuePair<string, string>("p", "1"),
            new KeyValuePair<string, string>("LT", "")
          };

          // need for void error "too fast" from spaces
          await Task.Delay(2500, ct);

          currentWork.Report("Загружаем " + f.Name + "...");
          log.Report("Начало загрузки...");

          Error.Codes opCode = await App.Net.PostMultipart(url, keys, new KeyValuePair<string, string>("myFile", f.ToString()));

          log.Report("Загрузка заврешена (" + App.Net.LastCodeAnswer + ")");
          log.Report("Результат: " + Error.GetMessage(opCode));

          if (opCode == Error.Codes.NoError) {
            progressTotal.Report(i + 1);
            i++;
            VProgressBarTaskBarSetValue(i, files.Count);
          } else {
            log.Report("Пробуем ещё раз... (всего ошибок: " + errorsCount + ")");
            errorsCount++;
          }
          await Task.Delay(2500, ct);

        } catch (OperationCanceledException) {
          log.Report("Выполняется отмена операции...");
          break;

        } catch (Exception e) {
          log.Report("Ошибка при загрузке (" + e.Message + ") от (" + e.Source + ")" + e.HResult);
          errorsCount++;
        }
      }

      if (errorsCount == maxErrCount) {
        log.Report("Загрузка остановлена из-за большого количества ошибок...\n" +
                   "(результат: " + i + "/" + files.Count + ")");
      } else {
        log.Report("Загрузка завершена без ошибок");
      }

      VProgressBarTaskBarSetState(TaskbarProgress.TaskbarStates.NoProgress);
      App.Net.ProgressHandler.HttpReceiveProgress -= currnetProgressHandler;

      currentWork.Report("Завершено...");
      log.Report("Спасибо :)");
    }

    private void VProgressBarCurrentUpdate(HttpProgressEventArgs value) {
      // Dont care about long > int convert, because we have 60MB limit upload
      if (value.TotalBytes != null)
        VProgressBarCurrentUpdate(value.ProgressPercentage, (int)value.BytesTransferred, (int)value.TotalBytes);
    }

    private void VProgressBarCurrentUpdate(int p, int current, int total) {
      ProgressBarCurrent.Value = p;
      LabelUploadedKB.Text = (current / 1024) + @" / " + (total / 1024) + @" kb";
    }

    private void VCurrentWorkLogUploadUpdate(string value) {
      TextBoxUploadLog.AppendText("\r\n[" + DateTime.Now.ToString("HH:mm:ss") + "] " + value);
    }

    private void VProgressBarTotalUpdate(int value) {
      LabelTotalWork.Text = @"Общий прогресс: " + value + @" из " + ProgressBarTotal.Maximum;
      ProgressBarTotal.Value = value;
    }

    private void VCurrentWorkUploadUpdate(string value) {
      LabelCurrentWork.Text = value;
    }

    private void ButtonDebug_Click(object sender, EventArgs ev) {
      // [debug your code here] //
    }

    private void ListViewDirs_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter) {
        CChangeFileDir(sender);
      }
    }

    private void ButtonCancel_Click(object sender, EventArgs e) {
      VLockControl(sender);

      var result = MessageBox.Show(@"Остановить загрузку? Остановка будет выполнена после загрузки текущего файла", @"Отмена", MessageBoxButtons.YesNo);

      if (result == DialogResult.Yes) {
        CCancelAction();
      } else {
        VUnlockControl(sender);
      }
    }

    private static void CCancelAction() {
      if (App.Cts.IsCancellationRequested) {
        Debug.WriteLine("[WARNING] Cancel request already is true");
      }

      App.Cts.Cancel();
    }

    private void ButtonRestart_Click(object sender, EventArgs e) {
      Application.Restart();
    }

    private void ButtonLoginFirefox_Click(object sender, EventArgs e) {
      var result = MessageBox.Show(@"Начать поиск активных сессий?", @"Требуется подтверждение", MessageBoxButtons.YesNo);

      if (result == DialogResult.Yes) {
        CAuthFirefox(sender);
      }      
    }

    private async void CAuthFirefox(object sender) {
      if (!CLock()) return;
      VLockControl(sender);

      var browserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Mozilla", "Firefox", "Profiles");

      var d = new DirectoryInfo(browserPath);

      var accounts = new List<KeyValuePair<string, string>>();

      foreach (var item in d.GetDirectories()) {
        var dbPath = Path.Combine(browserPath, item.Name, Path.GetFileName("cookies.sqlite"));
        const string dbQuery = @"SELECT `value` FROM `moz_cookies` WHERE `baseDomain` = ""spaces.ru"" AND `name` = ""sid""";

        using (var connection = new SQLiteConnection("Data Source=" + dbPath + "; Version=3;")) {
          await connection.OpenAsync();

          using (var dbCommand = new SQLiteCommand(dbQuery, connection))
          using (var dbReader = await dbCommand.ExecuteReaderAsync()) {
            while (await dbReader.ReadAsync()) {
              var sid = dbReader.GetString(0);
              await App.Session.Create(sid);
              if (App.Session.Valid) {
                accounts.Add(new KeyValuePair<string, string>(App.Session.UserName, sid));
              }
            }
          }
        }
      }

      App.Session.ResetSessionData();

      if (accounts.Count > 0) {
        var form = new FormAccounts(accounts);
        if (form.ShowDialog() == DialogResult.OK) {
          await App.Session.Create(form.GetSelectedSid());
          VAfterAuth();
        }
      } else {
        App.Err.SetError(Error.Codes.SessionParserNotFound, "Firefox parser", "Не найдено активных сессий");
      }

      CShowErrorIfNeeded();
      VUnlockControl(sender);
      CUnlock();
    }
  }
     
  public static class App {
    // Set true to unlock all GUI
    public static readonly bool DevModeEnabled = false;

    // Const
    public static class Const {
      public const int MaxFileSize = 62914560;
      public static readonly string Name = "D.MusicUploader";
      public static readonly string Author = "DJ_miXxXer";
      public static readonly string AuthorUrl = "http://spaces.ru/mysite/?name=DJ_miXxXer&_ref=dmapp";
      public static readonly string Ua = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0 MixxerUploader/0." + Version;
      public static readonly string Spaces = "http://spaces.ru";

      public const int Version = 5;
      public const int MinNetVersion = 378389;
    }

    public static IntPtr WinHandler = Process.GetCurrentProcess().MainWindowHandle;   
    public static Session Session;
    public static Networker Net;
    public static Error Err;
    public static CancellationTokenSource Cts = new CancellationTokenSource();

    private static bool _workFlag;
    public static bool BeginWork() {
      if (_workFlag) {
        return false;
      } else {
        _workFlag = true;
        return true;
      }
    }

    public static bool EndWork() {
      if (_workFlag) {
        _workFlag = false;
        return false;
      }

      Debug.WriteLine("[WARNING] Try end work, but work doesnt started");
      return false;
    }    
  }

  public static class MixxerApi {
    public static async Task<string> GetUserNameById(string id) {
      await App.Net.Get("http://" + id + ".spaces.ru/");

      return App.Net.GetValueByParam("name");
    }

    // 6 - music
    public static async Task<string> GetUploadUrl(string sid, string type = "6") {
      var postData = new List<KeyValuePair<string, string>> {
        new KeyValuePair<string, string>("Type", "6"),
        new KeyValuePair<string, string>("method", "getUploadInfo"),
        new KeyValuePair<string, string>("url", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
      };

      try {
        await App.Net.Post("http://spaces.ru/api/files/", postData);
      }
      catch {
        App.Err.SetError(Error.Codes.NetworkError, "API.GetUploadURL.Post", "Нет ответа от spaces.ru");
        throw;        
      }

      var answer = string.Empty;

      try {
        var o = JObject.Parse(App.Net.Answer);
        answer = o["url"].ToString();
      } catch {
        App.Err.SetError(Error.Codes.WrongParseData, "API.GetUploadURL.Parse", "Некорректные данные");
      }

      if (answer == string.Empty) {
        App.Err.SetError(Error.Codes.WrongParseData, "API.GetUploadURL.Parse", "Некорректные данные");
      }
                
      return answer;
    }

    public static async Task<List<KeyValuePair<string, string>>> GetMusicDirs(string userName, string dirId) {
      var url = "http://spaces.ru/music/?r=main/index&name=" + userName;
      if (dirId != "0") url = "http://spaces.ru/music/?Dir=" + dirId;

      await App.Net.Get(url);

      var m = Regex.Matches(App.Net.Answer, @"<a.*?</a>");

      if (m.Count == 0) App.Err.SetError(Error.Codes.WrongParseData, "MixxerAPI.GetMusicDirs", "Empty parse collection");
      
      /* Old realization:
       * foreach (Match item in m) {
        Match temp = Regex.Match(item.Value, @"Dir=(\d*).+?hover" + '"' + ">(.*?)</span>");

        if (temp.Groups.Count == 3 && temp.Groups[1].Value != "") {
          dict.Add(new KeyValuePair<string, string>(temp.Groups[1].Value, temp.Groups[2].Value));
        }
      }*/

      var dict = (from Match item in m select Regex.Match(item.Value, @"Dir=(\d*).+?hover" + '"' + ">(.*?)</span>") into temp where temp.Groups.Count == 3 && temp.Groups[1].Value != "" select new KeyValuePair<string, string>(temp.Groups[1].Value, temp.Groups[2].Value)).ToList();
      var nav = Regex.Match(App.Net.Answer, @"<div class=" + '"' + "location-bar" + '"' + ".*?<a.+?</a>.+?</div>");

      if (nav.Length <= 0) return dict;
      var navCounter = Regex.Matches(nav.Value, "<*.a>");

      if (navCounter.Count <= 3) return dict;
      var backLink = Regex.Match(nav.Value, @".+Dir=(\d+).+?>(.+?)<.a>");
      if (backLink.Length > 0) dict.Insert(0, new KeyValuePair<string, string>(backLink.Groups[1].Value, "[вверх на уровень] " + backLink.Groups[2].Value));

      return dict;
    }
  }

  public class Session {
    public string Sid { get; private set; }
    public string UserName { get; private set; }
    public string UserId { get; private set; }
    public bool Valid { get; private set; }

    public void ResetSessionData() {
      App.Net.ClearCookies();
      Sid = "";
      UserId = "";
      UserName = "";
      Valid = false;      
    }

    private static bool _CheckInputSID(string sid) {
      int trashVar;
      return sid.Length == 16 && !int.TryParse(sid, out trashVar);
    }

    public async Task Create(string sid) {
      ResetSessionData();

      if (_CheckInputSID(sid)) {
        Sid = sid;

        try {
          await App.Net.Get("http://spaces.ru/settings/?sid=" + sid);

          var temp = App.Net.GetCookieValueByName("user_id");
          if (temp != string.Empty) {
            
            UserId = temp;
            Valid = true;

            UserName = await MixxerApi.GetUserNameById(UserId);
          }
        } catch (Exception e) {
          App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Session __constr err [e: " + e.Message + "]");
        }
      }
    }
  }

  public class Error {
    public enum Codes {
      NoError = 0,
      WrongParseData = 1,
      TryCommonFail = 2,
      IncorrectSession = 3,
      WrongGuiOp = 4,
      ErrorTimeout = 5,
      NetworkError = 6,
      SessionParserNotFound = 7
    }

    public Error() {
      ErrCount = 0;
      _lastErrorCode = Codes.NoError;
      _extMessage = "";
      _place = "";
    }
   
    // Data
    private Codes _lastErrorCode;
    public Codes LastErrorCode {
      get {
        return _lastErrorCode;
      }
    }

    private string _extMessage;
    public string ExtMessage {
      get {
        return _extMessage;
      }
    }

    private string _place;
    public string Place {
      get {
        return _place;
      }
    }

    public int ErrCount { get; private set; }

    // Func
    public bool CheckIsError() {
      return _lastErrorCode != Codes.NoError;
    }

    /// <summary>
    /// Setting app-error values
    /// </summary>
    /// <param name="code">Error code (Erorr.Code.*)</param>
    /// <param name="place">[Debug] Class name</param>
    /// <param name="extMessage">Message to user</param>
    public void SetError(Codes code, string place = "", string extMessage = "") {
      ErrCount++;
      _lastErrorCode = code;
      _extMessage = extMessage;
      _place = place;
      Debug.WriteLine("[ERROR] code: " + code + ", at: " + _place + ", message: " + _extMessage);

      if (code != Codes.NoError) return;
      Debug.WriteLine("[ERROR MAIN] cant set NO.ERROR as error!");
      throw new Exception("Curve hands coder exception :)");
    }

    public void Reset() {
      _lastErrorCode = Codes.NoError;
      ErrCount = 0;
      _extMessage = "";
      _place = "";
    }

    public static string GetMessage(Codes code) {
      switch (code) {
        case Codes.NoError: {
          return "Нет ошибки";
        }

        case Codes.WrongParseData: {
          return "Ошибка чтения данных";
        }

        case Codes.TryCommonFail: {
          return "Общая ошибка приложения";
        }

        case Codes.IncorrectSession: {
          return "Недействительная ссессия";
        }

        case Codes.WrongGuiOp: {
          return "Ошибка интерфейса";
        }

        case Codes.NetworkError: {
          return "Ошибка сети";
        }

        case Codes.SessionParserNotFound: {
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
    private readonly HttpClientHandler _handler;
    private readonly HttpClient _client;
    private Uri _uri;

    // public for binding external handlers
    public ProgressMessageHandler ProgressHandler;
        
    // HTTP opts
    private readonly CookieContainer _cookies;

    public string Answer { get; private set; }
    public int LastCodeAnswer { get; private set; }

    public void ClearCookies() {
      if (_cookies == null) return;

      // how get ALL cookies? :)
      foreach (Cookie co in _cookies.GetCookies(new Uri(App.Const.Spaces))) {
        co.Expired = true;
      }
    }
   
    public string GetValueByParam(string name) {
      string url = _uri.Query.Substring(1);

      string []param = url.Split('&');

      foreach (string item in param) {
        if (item.Length < 1) continue;

        string []temp = item.Split('=');

        if (temp[0] == name) return temp[1];
	    }

      return "";
    }

    public Networker(string useragent) {
      _cookies = new CookieContainer();

      _handler = new HttpClientHandler {
        ClientCertificateOptions = ClientCertificateOption.Automatic,
        AllowAutoRedirect = true,
        MaxAutomaticRedirections = 3,
        UseCookies = true,
        CookieContainer = _cookies,
        Credentials = CredentialCache.DefaultCredentials,
        UseDefaultCredentials = true
      };

      ProgressHandler = new ProgressMessageHandler();
   
      ProgressHandler.HttpSendProgress += SendProgress;
      ProgressHandler.HttpReceiveProgress += RecvProgress;
     
      _client = HttpClientFactory.Create(_handler, ProgressHandler);
      _client.DefaultRequestHeaders.UserAgent.ParseAdd(useragent);
      _client.Timeout = TimeSpan.FromMinutes(30);
     
      _client.DefaultRequestHeaders.Accept.Clear();
      _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
      _client.DefaultRequestHeaders.ExpectContinue = false;
    }

    private static void RecvProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("NET.Recive: " + e.BytesTransferred + " / total: " + e.TotalBytes + ".");
    }

    private static void SendProgress(object sender, HttpProgressEventArgs e) {
      Debug.WriteLine("Net.Send: " + e.BytesTransferred + " / total: " + e.TotalBytes + ".");
    }
     
    private void _Clear() {
      Answer = "";
      LastCodeAnswer = -1;
    }

    public async Task Get(string url) {
      try {
        _Clear();
        using (var response = await _client.GetAsync(url).ConfigureAwait(false)) {
          using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
            Answer = stream.ReadToEnd();           
          }
          _uri = response.RequestMessage.RequestUri;
          LastCodeAnswer = (int)response.StatusCode;
        }        
      } catch {
        App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Ошибка get запроса");
        throw;
      }
    }

    public async Task Post(string url, List<KeyValuePair<string, string>> postParams) {     
      try {
        _Clear();
        using (var postHeaders = new FormUrlEncodedContent(postParams)) {
          using (var response = await _client.PostAsync(url, postHeaders).ConfigureAwait(false)) {
            using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
              Answer = stream.ReadToEnd();
            }
            _uri = response.RequestMessage.RequestUri;
            LastCodeAnswer = (int)response.StatusCode;
          }          
        }        
      } catch {
        App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Ошибка post запроса");
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

          var f = new FileInfo(fileData.Value);
          
          contentData.Add(new StreamContent(File.OpenRead(fileData.Value)), fileData.Key, f.Name);

          using (var response = await _client.PostAsync(url, contentData)) {            
            using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync())) {
              Answer = stream.ReadToEnd();
            }
            _uri = response.RequestMessage.RequestUri;
            LastCodeAnswer = (int)response.StatusCode;
          }          
        }
      } catch (TimeoutException) {
        return Error.Codes.ErrorTimeout;

      } catch (TaskCanceledException) {
        return Error.Codes.ErrorTimeout;

      } catch (Exception e) {
        App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Ошибка postM запроса (" + e.Message + ")");
        throw;
      }
      return Error.Codes.NoError;
    }

    public void Free() {
      if (ProgressHandler != null) ProgressHandler.Dispose();
      if (_handler != null) _handler.Dispose();
      if (_client != null) _client.Dispose();      
    }

    public string GetCookieValueByName(string name) {
      try {
        foreach (var c in from Cookie c in _cookies.GetCookies(new Uri("http://" + _uri.Host)) where c.Name == name select c) {
          return c.Value;
        }
      } catch (Exception e) {
        App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Ошибка чтения кук (" + e.Message + ")");
      }
      
      return "";
    }
  }

  public class Updater {
    public int LastVersion { get; private set; }

    public static readonly string UpdateLink = "http://spaces.ru/forums/?r=17760125";

    public Updater()
    {
      LastVersion = 0;
    }

    public async Task Create() {  
      try {
        await App.Net.Get(UpdateLink);
               
        var m = Regex.Match(App.Net.Answer, @"###LASTVERSION:(\d)");

        if (m.Groups.Count == 2) {
          LastVersion = Convert.ToInt32(m.Groups[1].Value);
        } else {
          App.Err.SetError(Error.Codes.WrongParseData, ToString(), "Err count: " + m.Groups.Count);
        }      
      } catch (Exception) {
        App.Err.SetError(Error.Codes.TryCommonFail, ToString(), "Ошибка проверки обновления");
      }
    }

    public string CompareVersions(int currentVersion) {
      return currentVersion < LastVersion ? "Доступна новая версия!" : "Обновление не треубется";
    }
  }  
}

