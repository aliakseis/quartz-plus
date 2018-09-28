using System;

namespace ILCWebApplication.ILCSettings
{
    /// <summary>
    /// Contains ILC settings data
    /// </summary>
    public class IlcSettingsData
    {
        public string scheduleCron;
        public string outgoingChannels;
        public string timeSpan;
        public string commonRecipientEmailAddresses;
        public string timeSpanBetweenVerifications;
        public string attemptsMaxNumber;
        public string emailFromAddress;
        public string summaryReportCron;
        public string summaryRecipientEmail;
        public string jobMisfireThreshold;
    }

    /// <summary>
    /// Contains item data
    /// </summary>
    public class DetailItemData
    {
        public string itemId;
        public string phoneNumber;
        public string projectId;
        public string login;
        public string password;
        public string enabled;
    }

    /// <summary>
    /// Contains project data
    /// </summary>
    public class DetailProjectData
    {
        public string projectId;
        public string serverId;
        public string name;
        public string emails;
        public string enabled;
        public string cron;
    }

    /// <summary>
    /// Contains server data
    /// </summary>
    public class DetailServerData
    {
        public string serverId;
        public string name;
        public string channels;
        public string connectionString;
        public string enabled;
        public string checker;
        public string cron;
    }

    /// <summary>
    /// Contains script data
    /// </summary>
    public class DetailScriptData
    {
        public string scriptId;
        public string name;
        public string scriptingExspression;
    }

    /// <summary>
    /// Contains ILC status data
    /// </summary>
    public class IlcStatusData
    {
        public DateTime lastCheckDate;
        public string currServiceState;
    }

    public class SummaryReportItem
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
}