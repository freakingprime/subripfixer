using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubripFixer
{
    public class Utils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        private static LogController oldLog = LogController.Instance;
        private Utils()
        {

        }

        private static Utils _utils = null;

        public static Utils Instance
        {
            get
            {
                if (_utils == null)
                {
                    _utils = new Utils();
                }
                return _utils;
            }
        }

        public const int DEFAULT_TIMEOUT = 5;
        public static int RequestTimeOutSecond = DEFAULT_TIMEOUT;

        #region Web browser
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
           string url,
           string cookieName,
           StringBuilder cookieData,
           ref int size,
           Int32 dwFlags,
           IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;
        public static string GetUriCookieString(Uri uri)
        {
            //Get cookies of a web page
            //Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                //Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString().Trim();
        }     

        #endregion    

        public static bool BackupFileToSubFolder(string path, int lowerbound = 0, int upperbound = 0)
        {
            bool ret = false;
            if (path.Trim().Length > 0)
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    DirectoryInfo di = fi.Directory.CreateSubdirectory(MyConstants.BACKUP_FOLDER);
                    if (di.Exists)
                    {
                        string backupName = fi.Name.Replace(fi.Extension, "");
                        backupName = backupName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + fi.Extension;
                        var targetFi = fi.CopyTo(Path.Combine(di.FullName, backupName), true);
                        if (targetFi.Exists)
                        {
                            ret = true;
                            if (upperbound > 0)
                            {
                                //remove old files
                                var list = di.GetFiles(fi.Name.Replace(fi.Extension, "") + "_*");
                                if (list.Length > upperbound)
                                {
                                    //begin remove files until reach lowerbound
                                    Array.Sort(list, (x, y) => x.Name.CompareTo(y.Name));
                                    for (int i = 0; i < list.Length - lowerbound; ++i)
                                    {
                                        try
                                        {
                                            list[i].Delete();
                                        }
                                        catch (Exception e)
                                        {
                                            oldLog.Error("Cannot delete backup file: " + list[i].FullName, e);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }

        public static string GetValidFolderPath(string folder)
        {
            //Get closet valid folder from a path
            while (folder.LastIndexOf(Path.DirectorySeparatorChar) > 0 && !Directory.Exists(folder))
            {
                folder = folder.Substring(0, folder.LastIndexOf(Path.DirectorySeparatorChar));
            }
            return folder;
        }

        #region Dabase operations

        public static object SQL_NULL = (object)DBNull.Value;

        private static readonly IProgress<(int Percent, string Text)> progressImport = new Progress<(int Percent, string Text)>(value =>
        {
            oldLog.SetValueProgress(value.Percent, value.Text);
        });


        #endregion

        public static string MinuteToHour(int t)
        {
            int hour = t / 60;
            int min = t % 60;
            string ret = "";
            if (hour > 0)
            {
                ret += hour + " hour";
            }
            if (min > 0)
            {
                ret += " " + min + " min";
            }
            return ret.Trim();
        }
    }
}
