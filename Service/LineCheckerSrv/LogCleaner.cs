using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;
using Common.Logging;

namespace LineCheckerSrv
{
    /// <summary>
    /// Implements deleting out of date temp files
    /// </summary>
    internal class LogCleaner
    {
        private const string QUERY_TMPL =
            "SELECT * FROM CIM_DataFile WHERE Drive = '{0}' AND  Path = '{1}' {2} AND CreationDate < '{3}'";

        private static string GetDate(int days)
        {
            DateTime dt = DateTime.Today.AddDays(-days);
            return ManagementDateTimeConverter.ToDmtfDateTime(dt);
        }

        private static string GetDrive(string sFullPath)
        {
            return Path.GetPathRoot(sFullPath).Substring(0, 2);
        }

        private static string GetPath(string sFullPath)
        {
            return sFullPath.Length < 3
                       ? "\\\\"
                       : (Path.GetDirectoryName(sFullPath) + "\\").Substring(2).Replace("\\", "\\\\").Replace("'", "\\'");
        }

        private static string GetExtensionCondition(IEnumerable<string> extensions)
        {
            if (extensions == null)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (string ext in extensions)
            {
                if (ext.Length == 0)
                    continue;

                if (sb.Length == 0)
                    sb.AppendFormat(" AND (Extension = '{0}'", ext.Trim());
                else
                    sb.AppendFormat(" OR Extension = '{0}'", ext.Trim());
            }
            if (sb.Length != 0)
                sb.Append(") ");
            return sb.ToString();
        }

        private static string GetQuery(string sDataPath, IEnumerable<string> extensions, int days)
        {
            try
            {
                return string.Format(QUERY_TMPL, GetDrive(sDataPath), GetPath(sDataPath),
                                     GetExtensionCondition(extensions),
                                     GetDate(days));
            }
            catch (ArgumentException e)
            {
                GetLogger().Error("LogCleaner: Exception occurred while getting query.", e);
            }
            catch (PathTooLongException e)
            {
                GetLogger().Error("LogCleaner: Exception occurred while getting query.", e);
            }

            return "";
        }

        /// <summary>
        /// Deletes out of date temp files
        /// </summary>
        /// <param name="sDataPath">application data path</param>
        /// <param name="extensions">temp files extensions string array</param>
        /// <param name="days">number of days ILC would keep temp files</param>
        /// <returns>true in case of success</returns>
        public static bool Clean(string sDataPath, string[] extensions, int days)
        {
            ILog log = GetLogger();

            log.Info("LogCleaner: attempt to clean \"ApplicationDataPath\" directory.");

            if (days < 0)
            {
                log.Info(
                    "LogCleaner: Key \"KeepTempFilesDays\" is not set in the config file. \"ApplicationDataPath\" directory will not be cleaned.");
                return false;
            }

            string query = GetQuery(sDataPath, extensions, days);
            if (string.IsNullOrEmpty(query))
                return false;

            log.Info("LogCleaner: query string is " + query + ".");

            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    // Create a results watcher object,
                    // and handler for results and completion
                    ManagementOperationObserver results = new
                        ManagementOperationObserver();

                    // Attach handler to events for results and completion
                    results.ObjectReady += NewObject;
                    results.Completed += Done;

                    // Call the asynchronous overload of Get()
                    // to start the enumeration
                    searcher.Get(results);

                    return true;
                }
            }
            catch (ManagementException e)
            {
                log.Error("LogCleaner: Exception occurred while WMI working.", e);
                return false;
            }
        }

        private static ILog GetLogger()
        {
            return LogManager.GetLogger(AppSettings.GetCommonLoggerName());
        }


        private static void NewObject(object sender,
                                      ObjectReadyEventArgs args)
        {
            ManagementObject obj = (ManagementObject) args.NewObject;
            GetLogger().Info("LogCleaner: deleting file " + obj["Name"]
                             + " with creation date " +
                             ManagementDateTimeConverter.ToDateTime(obj["CreationDate"].ToString()) + ".");
            obj.Delete();
        }


        private static void Done(object sender,
                                 CompletedEventArgs obj)
        {
            GetLogger().Info("LogCleaner: done deleting files.");
        }
    }
}