using System;
using System.Collections.Generic;

namespace LineCheckerSrv
{
    /// <summary>
    /// Contains validation info for summary reporting
    /// </summary>
    class SummaryReportItem
    {
        public string projectName;
        public string serverName;
        public string phone;
        public string userId;
        public int checksCount;
        public int failedChecksCount;
        public DateTime? lastSuccessDate;
        public DateTime? lastFailedDate;
    }

    /// <summary>
    /// Contains server info for summary reporting
    /// </summary>
    class SummaryServerItemInfo
    {
        public int checksCount;
        public int failedChecksCount;
        public DateTime? lastSuccessDate;
        public DateTime? lastFailedDate;
        /// <summary>
        /// Used instead of a map since .NET 2.0 does not have maps
        /// </summary>
        public Dictionary<string, int> projectDictionary = new Dictionary<string, int>();
    }
}
