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
            this.context = (SearchTitleVm)this.DataContext;
        }

        private SearchTitleVm context;

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is SearchTitleVm vm)
            {
                this.context = vm;
            }
        }
    }
}
