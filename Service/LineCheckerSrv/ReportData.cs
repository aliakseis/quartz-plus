using System;

namespace LineCheckerSrv
{
    /// <summary>
    /// Contains data for reporting
    /// </summary>
    internal class ReportItem
    {
        /// <summary>
        /// Number of call attempts undertaken for the current item
        /// </summary>
        public int attemps;
        public string phone;
        public string projectName;
        public string serverName;
        /// <summary>
        /// Brief failure reason info
        /// </summary>
        public string reason;
        /// <summary>
        /// Item status as set in the database:
        ///     AWAITING - not connected;
        ///     WORKERXXXX - still in use (presumably interrupted);
        ///     SUCCEEDED, FAILED
        /// </summary>
        public string status;
        public string userId;
        /// <summary>
        /// Time when the current item verification has ended.
        /// </summary>
        public DateTime verificationTime;
    }
}