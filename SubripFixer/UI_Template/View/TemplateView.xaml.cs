using SubripFixer.UI_SearchTitle.ViewModel;
using SubripFixer.UI_Template.ViewModel;
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

namespace SubripFixer.UI_Template.View
{
    /// <summary>
    /// Interaction logic for TemplateView.xaml
    /// </summary>
    public partial class TemplateView : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static LogController oldLog = LogController.Instance;

        public TemplateView()
        {
            InitializeComponent();
            this.context = (TemplateVm)this.DataContext;
        }

        private TemplateVm context;

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is TemplateVm vm)
            {
                this.context = vm;
            }
        }
    }
}
