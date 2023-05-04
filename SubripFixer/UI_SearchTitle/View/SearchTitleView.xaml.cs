using NETCore.Encrypt;
using SubripFixer.UI_SearchTitle.ViewModel;
using System;
using System.Collections.Generic;
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
    }
}
