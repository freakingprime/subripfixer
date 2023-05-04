using NETCore.Encrypt;
using Newtonsoft.Json;
using SubripFixer.MVVM;
using SubripFixer.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SubripFixer
{
    public class MainWindowVm : ViewModelBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

        public MainWindowVm()
        {
            WindowTitle = Properties.Resources.TITLE + " " + Properties.Resources.VERSION + "." + Properties.Resources.BuildTime.Trim();
#if DEBUG
            WindowTitle += " DEBUG";
#endif
            //restore url
            log.Debug("Restore URL from setting");

            //reset text notice for browser
            TextNotice = string.Empty;

            //add predefined url
            ListPredefinedUrl = new ObservableCollection<MyComboboxItemVm>
            {
                new MyComboboxItemVm("Check User Agent", "https://www.whatsmyua.info"),
                new MyComboboxItemVm("Google", "https://google.com")
            };

            //set user agent
            UserAgent = Properties.Settings.Default.UserAgent;

            //init sub view model

            //init list history
            ListHistory = new ObservableCollection<MyComboboxItemVm>();
            LoadHistory();

            //Restore log wrapped setting
            IsLogWrapped = Properties.Settings.Default.IsLogWrapped;
        }

        #region Bind properties

        private string _txtPercentage;

        public string TxtPercentage
        {
            get { return _txtPercentage; }
            set { SetValue(ref _txtPercentage, value); }
        }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set { SetValue(ref _windowTitle, value); }
        }

        private string _currentUrl;

        public string CurrentUrl
        {
            get { return _currentUrl; }
            set
            {
                SetValue(ref _currentUrl, value);
                LogController.Instance.CurrentURL = _currentUrl;
            }
        }

        private string _textNotice;

        public string TextNotice
        {
            get { return _textNotice; }
            set { SetValue(ref _textNotice, value); }
        }

        public ObservableCollection<MyComboboxItemVm> ListPredefinedUrl { get; set; }
        public ObservableCollection<MyComboboxItemVm> ListHistory { get; set; }

        private MyComboboxItemVm _selectedPredefined;

        public MyComboboxItemVm SelectedPredefined
        {
            get { return _selectedPredefined; }
            set { SetValue(ref _selectedPredefined, value); }
        }

        private MyComboboxItemVm _selectedHistory;

        public MyComboboxItemVm SelectedHistory
        {
            get { return _selectedHistory; }
            set { SetValue(ref _selectedHistory, value); }
        }

        private string _userAgent;

        public string UserAgent
        {
            get { return _userAgent; }
            set { SetValue(ref _userAgent, value); }
        }

        private bool _isLogWrapped;

        public bool IsLogWrapped
        {
            get { return _isLogWrapped; }
            set { SetValue(ref _isLogWrapped, value); }
        }

        #endregion

        public override void Loaded()
        {

        }

        public override void Closing()
        {
            SaveHistory();
        }

        public void InsertHistory(string title, string u)
        {
            const int PREFIX_LENGTH = 30;
            const int SUFFIX_LENGTH = 10;
            const int COUNT_LIMIT = 15;
            const string MIDDLE = "...";
            string display = title;
            if (title.Length > PREFIX_LENGTH + SUFFIX_LENGTH + MIDDLE.Length)
            {
                display = title.Substring(0, PREFIX_LENGTH) + MIDDLE + title.Substring(title.Length - SUFFIX_LENGTH);
            }
            ListHistory.Insert(0, new MyComboboxItemVm(display, u));
            while (ListHistory.Count > COUNT_LIMIT)
            {
                ListHistory.RemoveAt(ListHistory.Count - 1);
            }
        }

        public void SaveHistory()
        {
            string text = JsonConvert.SerializeObject(ListHistory);
            log.Debug("Encrypt history setting");
            Properties.Settings.Default.LastHistory = text;
            Properties.Settings.Default.Save();
        }

        public void LoadHistory()
        {
            log.Debug("Decrypt history setting");
            string decrypted = Properties.Settings.Default.LastHistory;
            try
            {
                log.Debug("Convert to list of object");
                List<MyComboboxItemVm> list = JsonConvert.DeserializeObject<List<MyComboboxItemVm>>(decrypted);
                ListHistory.Clear();
                foreach (var item in list)
                {
                    ListHistory.Add(item);
                }
            }
            catch (Exception e1)
            {
                log.Error("Cannot load last history from file.", e1);
            }
        }
    }
}
