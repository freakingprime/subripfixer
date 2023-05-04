using NETCore.Encrypt;
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

        }
    }
}
