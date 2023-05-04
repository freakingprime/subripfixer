using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubripFixer.Utility
{
    public class MyComboboxItemVm
    {
        public MyComboboxItemVm(string text, string u)
        {
            this.DisplayText = text;
            this.Value = u;
        }
        public string DisplayText { get; set; }
        public string Value { get; set; }
    }
}
