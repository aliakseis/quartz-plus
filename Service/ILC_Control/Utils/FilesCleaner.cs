using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Text;

namespace ILC_ControlPanel.Utils
{
    /// <summary>
    /// Class for asynchronous files deleting
    /// </summary>
    static class FilesCleaner
    {
        private const string QUERY_TMPL =
            "SELECT * FROM CIM_DataFile WHERE Drive = '{0}' AND  Path = '{1}' {2} {3}";

        private static string GetDate(int days, DateTime finishDate)
        {
            string strDate = string.Format("AND CreationDate < '{0}'",ManagementDateTimeConverter.ToDmtfDateTime(finishDate));
            if (days != -1)
            {
                DateTime dt = DateTime.Now.AddDays(-days);
                strDate += string.Format(" AND CreationDate > '{0}'", ManagementDateTimeConverter.ToDmtfDateTime(dt));
            }
            
            return strDate;
        }

        private static string GetDrive(string sFullPath)
        {
            return Path.GetPathRoot(sFullPath).Substring(0, 2);
        }

        private static string GetPath(string sFullPath)
        {
            if (!sFullPath.EndsWith("\\"))
                sFullPath += "\\";

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

        /// <summary>
        /// Deletes out of date temp files
        /// </summary>
        /// <param name="sDataPath">application data path</param>
        /// <param name="extensions">temp files extensions string array</param>
        /// <param name="days">period for deleting files</param>
        /// <param name="finishDate">date after that files will not be deleted</param>
        /// <param name="completedHandler">handler to events for completion</param>
        /// <returns>true in case of success</returns>
        public static bool Clean(
            string sDataPath,
            IList<string> extensions,
            int days,
            DateTime finishDate,
            CompletedEventHandler completedHandler
            )
        {
            string query = string.Format(QUERY_TMPL, GetDrive(sDataPath), GetPath(sDataPath),
                                     GetExtensionCondition(extensions),
                                     GetDate(days, finishDate));

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                // Create a results watcher object,
                // and handler for results and completion
                ManagementOperationObserver results = new
                    ManagementOperationObserver();

                // Attach handler to events for results and completion
                results.ObjectReady += NewObject;
                results.Completed += completedHandler; // Done;

                // Call the asynchronous overload of Get()
                // to start the enumeration
                searcher.Get(results);

                return true;
            }
        }

        private static void NewObject(object sender,
                                      ObjectReadyEventArgs args)
        {
            ManagementObject obj = (ManagementObject)args.NewObject;
            obj.Delete();
        }

    }
}
