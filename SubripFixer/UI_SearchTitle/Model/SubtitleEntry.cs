﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SubripFixer.UI_SearchTitle.Model
{
    internal class SubtitleEntry
    {
        public SubtitleEntry()
        {

        }

        public string TimeStr = "";
        public List<string> ListContent = new List<string>();
        public DateTime StartDate = DateTime.MinValue;
        public DateTime EndDate = DateTime.MinValue;

        public bool IsValid
        {
            get
            {
                return StartDate > DateTime.MinValue && ListContent.Count > 0;
            }
        }

        public override string ToString()
        {
            return TimeStr + " " + string.Join(" | ", ListContent);
        }

        public string GetSubtitleAsString(int index)
        {
            string ret = index + Environment.NewLine + StartDate.ToString("HH:mm:ss,fff") + " --> " + EndDate.ToString("HH:mm:ss,fff");
            foreach (var item in ListContent)
            {
                ret += Environment.NewLine + item;
            }
            ret += Environment.NewLine;
            return ret;
        }

        public void SelfCleaning()
        {
            var split = TimeStr.Split(new string[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 2)
            {
                DateTime.TryParseExact(split[0].Trim(), "HH:mm:ss,fff", null, DateTimeStyles.None, out StartDate);
                DateTime.TryParseExact(split[1].Trim(), "HH:mm:ss,fff", null, DateTimeStyles.None, out EndDate);
            }

            //remove last number that can be id of next sub
            if (ListContent.Count > 0)
            {
                if (int.TryParse(ListContent.Last(), out _))
                {
                    ListContent.RemoveAt(ListContent.Count - 1);
                }
            }

            //remove ignored content
            if (Properties.Settings.Default.Sub_IgnoreText)
            {
                string[] listIgnore = Properties.Settings.Default.Sub_ListIgnoreText.Split('\n');
                for (int i = 0; i < listIgnore.Length; ++i)
                {
                    listIgnore[i] = listIgnore[i].Trim();
                }
                for (int i = 0; i < ListContent.Count; ++i)
                {
                    for (int k = 0; k < listIgnore.Length; ++k)
                    {
                        if (listIgnore[k].Length > 0)
                        {
                            //while (ListContent[i].IndexOf(listIgnore[k], StringComparison.OrdinalIgnoreCase) >= 0)
                            //{
                            //    ListContent[i] = ListContent[i].Remove(ListContent[i].IndexOf(listIgnore[k], StringComparison.OrdinalIgnoreCase), listIgnore[k].Length);
                            //}

                            //only remove if whole line match the text
                            if (ListContent[i].Equals(listIgnore[k], StringComparison.OrdinalIgnoreCase))
                            {
                                ListContent[i] = "";
                            }
                        }
                    }
                }
            }

            //remove empty content
            for (int i = 0; i < ListContent.Count; ++i)
            {
                if (ListContent[i].Length == 0)
                {
                    ListContent.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}
