using SubripFixer.MVVM;
using SubripFixer.UI_SearchTitle.Model;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;

namespace SubripFixer.UI_SearchTitle.ViewModel
{
    public class SearchTitleVm : ViewModelBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static readonly LogController oldLog = LogController.Instance;

        public SearchTitleVm()
        {

        }

        #region Bind properties

        #endregion

        #region Normal Properties

        #endregion       

        public override async void Loaded()
        {
            var args = Environment.GetCommandLineArgs();
            var task = Task.Run(() =>
            {
                for (int i = 1; i < args.Length; ++i)
                {
                    string path = args[i];
                    oldLog.Debug("Process file: " + path);
                    FileInfo fi = new FileInfo(path);
                    if (fi.Exists && fi.Extension.IndexOf("srt", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var listSub = ProcessFile(fi.FullName);
                        if (listSub.Count > 0)
                        {
                            //backup current file
                            File.Copy(fi.FullName, fi.FullName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), true);
                            List<string> output = new List<string>();
                            oldLog.Debug("Subtitle count: " + listSub.Count);
                            for (int k = 0; k < listSub.Count; ++k)
                            {
                                output.Add(listSub[k].GetSubtitleAsString(k + 1));
                            }
                            File.WriteAllText(fi.FullName, string.Join(Environment.NewLine, output).Trim());
                            oldLog.Debug("Completed: " + fi.Name);
                        }
                    }
                }
            });
            await task;
#if !DEBUG
            if (args.Length > 1)
            {
                //run from Open With
                System.Windows.Application.Current.Shutdown();
            }
#endif
        }

        private List<SubtitleEntry> ProcessFile(string path)
        {
            List<SubtitleEntry> ret = new List<SubtitleEntry>();
            string[] lines = File.ReadAllLines(path);
            Regex regexTime = new Regex(@":\d\d,\d\d\d.*-->.*:\d\d,\d\d\d");
            SubtitleEntry sub = new SubtitleEntry();
            foreach (string line in lines)
            {
                if (regexTime.IsMatch(line))
                {
                    ret.Add(sub);
                    sub = new SubtitleEntry
                    {
                        TimeStr = line.Trim()
                    };
                }
                else
                {
                    sub.ListContent.Add(line.Trim());
                }
            }
            //add last sub
            ret.Add(sub);

            //clean up
            for (int i = 0; i < ret.Count; ++i)
            {
                ret[i].SelfCleaning();
                if (!ret[i].IsValid)
                {
                    ret.RemoveAt(i);
                    --i;
                }
            }

            //adjust time stamp
            if (Properties.Settings.Default.Sub_FixTimestamp)
            {
                for (int i = 1; i < ret.Count; ++i)
                {
                    const double seperationMilli = 50;
                    const double minimumDisplayMilli = 400;
                    if (ret[i - 1].EndDate > ret[i].StartDate)
                    {
                        ret[i].StartDate = ret[i - 1].EndDate.AddMilliseconds(seperationMilli);
                        log.Info("Fix start time for sub index " + (i + 1));
                    }
                    TimeSpan span = ret[i].EndDate - ret[i].StartDate;
                    if (span.TotalMilliseconds < minimumDisplayMilli)
                    {
                        ret[i].EndDate = ret[i].StartDate.AddMilliseconds(minimumDisplayMilli);
                        log.Info("Fix end time for sub index " + (i + 1));
                    }
                }
            }
            return ret;
        }
    }
}
