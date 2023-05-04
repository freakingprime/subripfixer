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
        }

        private void TextboxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextboxLog.ScrollToEnd();
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            TextboxLog.Clear();
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

        private void BtnCopyLog_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextboxLog.Text);
        }
    }
}

