using SubripFixer.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;

namespace SubripFixer.UI_Template.ViewModel
{
    public class TemplateVm : ViewModelBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static readonly LogController oldLog = LogController.Instance;

        public TemplateVm()
        {

        }

        #region Bind properties

        #endregion

        #region Normal Properties

        #endregion       
    }
}
