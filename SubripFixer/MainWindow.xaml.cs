using CefSharp;
using CefSharp.Wpf;
using NETCore.Encrypt;
using SubripFixer.UI_SearchTitle.ViewModel;
using SubripFixer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SubripFixer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static LogController oldLog = LogController.Instance;
        public MainWindow()
        {
            InitializeComponent();
            log.Debug("Program start at: " + DateTime.Now.ToString());
            this.context = (MainWindowVm)this.DataContext;
            LogController.Instance.SetTextBox(TextboxLog);
            LogController.Instance.SetProgressBar(TheProgressBar);
            LogController.Instance.SetLabelPercentage(LabelPercentage);

            //restore activated tab
            int index = Properties.Settings.Default.LastActivatedTab;
            if (index < TabMain.Items.Count)
            {
                TabMain.SelectedIndex = index;
            }

            //Set custom cookies
            TxtCustomCookie.Text = Properties.Settings.Default.Search_CustomCookies;
            CheckboxUseCustomCookie.IsChecked = true;
            CheckboxUseCustomCookie.IsChecked = false;

#if !DEBUG
            TabForumFetch.Visibility = Visibility.Collapsed;
#endif
        }

        private MainWindowVm context = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //restore size
            if (Properties.Settings.Default.WindowHeight > 100 && Properties.Settings.Default.WindowWidth > 100)
            {
                this.Top = Properties.Settings.Default.WindowTop;
                this.Left = Properties.Settings.Default.WindowLeft;
                this.Height = Properties.Settings.Default.WindowHeight;
                this.Width = Properties.Settings.Default.WindowWidth;
                // Very quick and dirty - but it does the job
                if (Properties.Settings.Default.WindowMaximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }

            //restore tool height
            if (Properties.Settings.Default.BrowserHeight > 10)
            {
                RowWeb.Height = new GridLength(Properties.Settings.Default.BrowserHeight);
            }
            if (Properties.Settings.Default.LogWidth > 10)
            {
                ColLog.Width = new GridLength(Properties.Settings.Default.LogWidth);
            }

            //run initialize for sub ViewModel            
            context.Loaded();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Save window size
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.WindowTop = RestoreBounds.Top;
                Properties.Settings.Default.WindowLeft = RestoreBounds.Left;
                Properties.Settings.Default.WindowHeight = RestoreBounds.Height;
                Properties.Settings.Default.WindowWidth = RestoreBounds.Width;
                Properties.Settings.Default.WindowMaximized = true;
            }
            else
            {
                Properties.Settings.Default.WindowTop = this.Top;
                Properties.Settings.Default.WindowLeft = this.Left;
                Properties.Settings.Default.WindowHeight = this.Height;
                Properties.Settings.Default.WindowWidth = this.Width;
                Properties.Settings.Default.WindowMaximized = false;
            }

            //Save panel sizes
            Properties.Settings.Default.BrowserHeight = RowWeb.Height.Value;
            Properties.Settings.Default.LogWidth = ColLog.Width.Value;

            Properties.Settings.Default.LastActivatedTab = TabMain.SelectedIndex;

            Properties.Settings.Default.Save();

            //Call for sub ViewModel closing
            context.Closing();
            Cef.Shutdown();
        }

        private void BtnPasteLink_Click(object sender, RoutedEventArgs e)
        {
            TxtAddress.Text = Clipboard.GetText().Trim();
        }

        private void BtnGoWeb_Click(object sender, RoutedEventArgs e)
        {
            GoToAddress(TxtAddress.Text);
        }

        private void GoToAddress(string url)
        {
            context.TextNotice = string.Empty;
            try
            {
                Browser.Address = url;
            }
            catch (Exception e1)
            {
                string prefix = "Cannot parse text to URL: " + url + ". ";
                log.Error(prefix, e1);
                context.TextNotice = prefix + (e1.Message ?? "No more information.");
            }
        }

        private void ComboPredefinedUrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            TxtAddress.Text = combo.SelectedValue.ToString();
        }

        private void TextUserAgent_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.UserAgent = ((TextBox)sender).Text.Trim();
            Properties.Settings.Default.Save();
        }

        private void TextboxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextboxLog.ScrollToEnd();
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            TextboxLog.Clear();
        }

        private void ComboHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            TxtAddress.Text = combo.SelectedValue.ToString();
        }

        private void ComboBoxItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                MyComboboxItemVm vm = ((ComboBoxItem)sender).DataContext as MyComboboxItemVm;
                Clipboard.SetText(vm.Value);
                log.Debug("Copy URL to clipboard: " + vm.Value);
                e.Handled = true;
            }
        }

        private void BtnToggleWrap_Click(object sender, RoutedEventArgs e)
        {
            context.IsLogWrapped = !context.IsLogWrapped;
            Properties.Settings.Default.IsLogWrapped = context.IsLogWrapped;
            Properties.Settings.Default.Save();
        }

        private void TabMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.LastActivatedTab = ((TabControl)sender).SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void txtBoxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (context != null)
            {
                context.CurrentUrl = ((TextBox)sender).Text;
            }
        }

        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            string url = e.Browser.MainFrame.Url;
            if (e.IsLoading)
            {
                log.Debug("Loading: " + url);
                return;
            }
            log.Info("Page is loaded: " + url);
        }

        private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChromiumWebBrowser browser = (ChromiumWebBrowser)sender;
            string title = e.NewValue.ToString();
            string url = browser.GetMainFrame().Url;
            log.Info("Title changed to: " + title + " | URL: " + url);
            context.InsertHistory(title, url);

            //save cookie for search tab
            //if (url.ToLower().Contains(MyConstants.JL_CORE_URL))
            //{
            //    string tempCookie = Utils.GetCefSharpCookies(MyConstants.JL_HOME);
            //    if (!tempCookie.Equals(Properties.Settings.Default.Search_Cookie))
            //    {
            //        oldLog.Debug("New cookies are updated: " + tempCookie);
            //        Properties.Settings.Default.Search_Cookie = tempCookie;
            //    }
            //}
            Properties.Settings.Default.Save();
        }

        private void BtnCopyLog_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextboxLog.Text);
        }

        private void TxtCustomCookie_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Search_CustomCookies = TxtCustomCookie.Text.Trim();
            Properties.Settings.Default.Save();
        }

        private void CheckboxUseCustomCookie_Checked(object sender, RoutedEventArgs e)
        {
            log.Info("Use custom cookie status changed to: " + CheckboxUseCustomCookie.IsChecked);
            Utils.UseCustomCookies = (bool)CheckboxUseCustomCookie.IsChecked;
        }
    }
}

