using Microsoft.Win32;
using SubripFixer.UI_SearchTitle.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace SubripFixer.UI_SearchTitle.View
{
    /// <summary>
    /// Interaction logic for SearchTitleView.xaml
    /// </summary>
    public partial class SearchTitleView : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static LogController oldLog = LogController.Instance;

        public SearchTitleView()
        {
            InitializeComponent();
            context = (SearchTitleVm)this.DataContext;
            ChkFixOverlap.IsChecked = Properties.Settings.Default.Sub_FixTimestamp;
            ChkIgnoreText.IsChecked = Properties.Settings.Default.Sub_IgnoreText;
            TxtIgnore.Text = Properties.Settings.Default.Sub_ListIgnoreText;
            TxtIconFile.Text = Properties.Settings.Default.Sub_IconFile;
        }

        private SearchTitleVm context;

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is SearchTitleVm vm)
            {
                context = vm;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            context.Loaded();
        }

        private void ChkFixOverlap_Checked(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            Properties.Settings.Default.Sub_FixTimestamp = (bool)chk.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ChkIgnoreText_Checked(object sender, RoutedEventArgs e)
        {
            var chk = (CheckBox)sender;
            Properties.Settings.Default.Sub_IgnoreText = (bool)chk.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void TxtIgnore_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Sub_ListIgnoreText = ((TextBox)sender).Text;
            Properties.Settings.Default.Save();
        }

        private void BtnBrowseIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Icon files (*.ico)|*.ico|All files (*.*)|*.*"
            };
            if ((dialog.ShowDialog() ?? false) == true)
            {
                TxtIconFile.Text = dialog.FileName;
            }
        }

        private void BtnAddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            oldLog.Debug("EXE path: " + exePath);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"Windows Registry Editor Version 5.00");
            sb.AppendLine();
            sb.AppendLine(@"[HKEY_CLASSES_ROOT\SystemFileAssociations\.srt]");
            sb.AppendLine();
            sb.AppendLine(@"[HKEY_CLASSES_ROOT\SystemFileAssociations\.srt\shell]");
            sb.AppendLine();
            sb.AppendLine(@"[HKEY_CLASSES_ROOT\SystemFileAssociations\.srt\shell\Subrip Fixer]");

            string path = "";
            FileInfo icon = new FileInfo("srt.ico");
            if (icon.Exists)
            {
                path = icon.FullName;
            }
            if (File.Exists(Properties.Settings.Default.Sub_IconFile))
            {
                path = Properties.Settings.Default.Sub_IconFile;
            }
            if (path.Length > 0)
            {
                oldLog.Debug("Use icon: " + path);
                path = path.Replace(@"\", @"\\");
                // "Icon"="\"E:\\SETUP\\OTHERS 3\\SubripFixer\\srt.ico\""
                sb.AppendLine(@"""Icon""=""\""" + path + @"\""""");
                sb.AppendLine();
            }

            sb.AppendLine(@"[HKEY_CLASSES_ROOT\SystemFileAssociations\.srt\shell\Subrip Fixer\command]");
            exePath = exePath.Replace(@"\", @"\\");
            // @="\"E:\\SETUP\\OTHERS 3\\SubripFixer\\SubripFixer.exe\" \"%1\""
            sb.AppendLine(@"@=""\""" + exePath + @"\"" \""%1\""""");

            FileInfo fi = new FileInfo("contextmenu.reg");
            File.WriteAllText(fi.FullName, sb.ToString());
            Process regeditProcess = Process.Start("regedit.exe", "/s \"" + fi.FullName + "\"");
            regeditProcess.WaitForExit();

            oldLog.Debug("Finish set registry key");
        }

        private void TxtIconFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Sub_IconFile = ((TextBox)sender).Text;
            Properties.Settings.Default.Save();
        }

        private void BtnRemoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"Windows Registry Editor Version 5.00");
            sb.AppendLine();
            sb.AppendLine(@"[-HKEY_CLASSES_ROOT\SystemFileAssociations\.srt\shell\Subrip Fixer]");

            FileInfo fi = new FileInfo("contextmenuremove.reg");
            File.WriteAllText(fi.FullName, sb.ToString());
            Process regeditProcess = Process.Start("regedit.exe", "/s \"" + fi.FullName + "\"");
            regeditProcess.WaitForExit();

            oldLog.Debug("Finish remove registry key");
        }

        private void BtnBackupIgnored_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi = new FileInfo("backup_ignored.txt");
            if (fi.Exists)
            {
                File.Copy(fi.FullName, fi.FullName + "_" + DateTime.Now.ToString("HHmmss"), true);
            }
            File.WriteAllText(fi.FullName, Properties.Settings.Default.Sub_ListIgnoreText);
            oldLog.Debug("Ignored text is backed up to: " + fi.FullName);
        }
    }
}
