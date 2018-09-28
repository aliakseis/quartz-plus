using System;
using System.Configuration;
using Utils;
using System.Collections;

namespace LineCheckerSrv
{
    /// <summary>
    /// Class for getting service settings from configuration file
    /// </summary>
    internal static class AppSettings
    {
        public static readonly string ApplicationDataPathKey = "ApplicationDataPath";
        public static readonly string OutgoingChannelsKey = "OutgoingChannels";
        public static readonly string TerminatorKeyKey = "TerminatorKey";
        public static readonly string TimeSpanKey = "TimeSpan";
        public static readonly string TimeSpanBetweenVerificationsKey = "TimeSpanBetweenVerifications";
        public static readonly string AttemptsMaxNumberKey = "AttemptsMaxNumber";
        public static readonly string EmailFromAddressKey = "EmailFromAddress";
        public static readonly string CommonRecipientEmailAddressesKey = "CommonRecipientEmailAddresses";
        public static readonly string SilenceTimeoutKey = "SilenceTimeout";
        public static readonly string RecordDurationKey = "RecordDuration";
        public static readonly string LoginSilenceTimeoutKey = "LoginSilenceTimeout";
        public static readonly string SummaryReportCronKey = "SummaryReportCron";
        public static readonly string SummaryRecipientEmailKey = "SummaryRecipientEmail";
        public static readonly string JobMisfireThresholdKey = "JobMisfireThreshold";
        public static readonly string SettingsVersion = "SettingsVersion";
        public static readonly string ScriptsVersion = "ScriptsVersion";

        private static IDictionary appSettingsMap = new Hashtable();

        /// <summary>
        /// Map of application settings. Allows overwrite settings in runtime.
        /// </summary>
        public static IDictionary AppSettingsMap
        {
            get { return appSettingsMap; }
            set { appSettingsMap = value; }
        }

        /// <summary>
        /// Gets outgoing channels count
        /// </summary>
        /// <returns>outgoing channels count, -1 to use all available channels</returns>
        public static int GetOutgoingChannels()
        {
            object outgoingChannels = appSettingsMap[OutgoingChannelsKey];
            return (outgoingChannels == null)
                       ? -1
                       : Convert.ToInt32(outgoingChannels);
        }

        /// <summary>
        /// Gets line device
        /// </summary>
        /// <returns>Diva call property specified the device on which a call is handled</returns>
        public static int GetLineDevice()
        {
            string lineDevice = ConfigurationManager.AppSettings["LineDevice"];
            return string.IsNullOrEmpty(lineDevice)
                       ? -1
                       : Convert.ToInt32(lineDevice);
        }

        /// <summary>
        /// Gets terminator key
        /// </summary>
        /// <returns>terminator key</returns>
        public static string GetTerminatorKey()
        {
            string terminatorKey = appSettingsMap[TerminatorKeyKey] as string;
            return terminatorKey ?? "#";
        }

        /// <summary>
        /// Gets connection string
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetConnectionString()
        {
            return Connect.DecryptMaster(
                ConfigurationManager.ConnectionStrings["Cron"].ConnectionString);
        }

        /// <summary>
        /// Gets time between consequent item verifications
        /// </summary>
        /// <returns>time between consequent item verifications (e. g. if line was busy) in days</returns>
        public static double GetTimeSpanInDays()
        {
            object timeSpan = appSettingsMap[TimeSpanKey];
            return (timeSpan == null)
                       ? 100.0 / 86400
                       : Convert.ToDouble(timeSpan) / 86400;
        }

        /// <summary>
        /// Gets timespan between verification sessions
        /// </summary>
        /// <returns>time window between verification sessions (to define if we should start a new session) in days</returns>
        public static double GetTimeSpanBetweenVerificationsInDays()
        {
            object timeSpanBetweenVerifications = appSettingsMap[TimeSpanBetweenVerificationsKey];
            return (timeSpanBetweenVerifications == null)
                       ? 20.0 / 86400
                       : Convert.ToDouble(timeSpanBetweenVerifications) / 86400;
        }

        /// <summary>
        /// Gets maximum number of attempts
        /// </summary>
        /// <returns>maximum number of attempts</returns>
        public static int GetAttemptsMaxNumber()
        {
            object attemptsMaxNumber = appSettingsMap[AttemptsMaxNumberKey];
            return (attemptsMaxNumber == null)
                       ? 1
                       : Convert.ToInt32(attemptsMaxNumber);
        }

        /// <summary>
        /// Get application data path
        /// </summary>
        /// <returns>application data path</returns>
        public static string GetApplicationDataPath()
        {
            return ConfigurationManager.AppSettings[ApplicationDataPathKey];
        }

        /// <summary>
        /// Gets common recipient email addresses
        /// </summary>
        /// <returns>common recipient email addresses</returns>
        public static string GetCommonRecipientEmailAddresses()
        {
            return appSettingsMap[CommonRecipientEmailAddressesKey] as string;
        }

        /// <summary>
        /// Gets summary recipient email addresses
        /// </summary>
        /// <returns>summary recipient email addresses</returns>
        public static string GetSummaryRecipientEmailAddresses()
        {
            return appSettingsMap[SummaryRecipientEmailKey] as string;
        }

        /// <summary>
        /// Gets email server name
        /// </summary>
        /// <returns>email server name</returns>
        public static string GetEmailServerName()
        {
            return ConfigurationManager.AppSettings["EmailServerName"];
        }

        /// <summary>
        /// Gets email address from whom email is sent
        /// </summary>
        /// <returns>email address from whom email is sent</returns>
        public static string GetEmailFromAddress()
        {
            return appSettingsMap[EmailFromAddressKey] as string;
        }

        /// <summary>
        /// Gets email client login
        /// </summary>
        /// <returns>email client login</returns>
        public static string GetEmailClientLogin()
        {
            return ConfigurationManager.AppSettings["EmailClientLogin"];
        }

        /// <summary>
        /// Gets email client password
        /// </summary>
        /// <returns>email client password</returns>
        public static string GetEmailClientPassword()
        {
            return Connect.XCode(ConfigurationManager.AppSettings["EmailClientPassword"], false);
        }

        /// <summary>
        /// Gets silence timeout
        /// </summary>
        /// <returns>silence timeout</returns>
        public static uint GetSilenceTimeout()
        {
            object silenceTimeout = appSettingsMap[SilenceTimeoutKey];
            return (silenceTimeout == null)
                       ? 5
                       : Convert.ToUInt32(silenceTimeout);
        }

        /// <summary>
        /// Gets record duration
        /// </summary>
        /// <returns>record duration</returns>
        public static uint GetRecordDuration()
        {
            object recordDuration = appSettingsMap[RecordDurationKey];
            return (recordDuration == null)
                       ? 20
                       : Convert.ToUInt32(recordDuration);
        }

        /// <summary>
        /// Gets login silence timeout
        /// </summary>
        /// <returns>login silence timeout</returns>
        public static uint GetLoginSilenceTimeout()
        {
            object loginSilenceTimeout = appSettingsMap[LoginSilenceTimeoutKey];
            return (loginSilenceTimeout == null)
                       ? 0
                       : Convert.ToUInt32(loginSilenceTimeout);
        }

        /// <summary>
        /// Gets job misfire threshold(seconds)
        /// </summary>
        /// <returns>job misfire threshold</returns>
        public static uint GetJobMisfireThreshold()
        {
            object jobMisfireThreshold = appSettingsMap[JobMisfireThresholdKey];
            return (jobMisfireThreshold == null)
                       ? 300
                       : Convert.ToUInt32(jobMisfireThreshold);
        }

        /// <summary>
        /// Gets version of service settings
        /// </summary>
        /// <returns>version of service settings</returns>
        public static uint GetSettingsVersion()
        {
            return Convert.ToUInt32(appSettingsMap[SettingsVersion]);
        }

        /// <summary>
        /// Gets scripts version
        /// </summary>
        /// <returns>scripts version</returns>
        public static int GetScriptsVersion()
        {
            return Convert.ToInt32(appSettingsMap[ScriptsVersion]);
        }

        /// <summary>
        /// Gets summary report day
        /// </summary>
        /// <returns>Unix cron job style report scheduling string</returns>
        public static string GetSummaryReportDay()
        {
            string reportDay = appSettingsMap[SummaryReportCronKey] as string;
            return string.IsNullOrEmpty(reportDay)
                       ? "0 0 0 ? * FRI"
                       : reportDay;
        }

        /// <summary>
        /// Gets the number of days ILC would keep temp files
        /// </summary>
        /// <returns>contains the number of days ILC would keep temp files</returns>
        public static int GetKeepTempFilesDays()
        {
            string days = ConfigurationManager.AppSettings["KeepTempFilesDays"];
            return string.IsNullOrEmpty(days)
                       ? -1
                       : Convert.ToInt32(days);
        }

        /// <summary>
        /// Gets file name for diva logging
        /// </summary>
        /// <returns>file name for diva logging</returns>
        public static string GetDivaLogFileName()
        {
            return ConfigurationManager.AppSettings["DivaLogFileName"];
        }


        /// <summary>
        /// Gets common logger name
        /// </summary>
        /// <returns>common logger name</returns>
        public static string GetCommonLoggerName()
        {
            return "IvrsLineCheckerLogger";
        }

        /// <summary>
        /// Gets DB schema name
        /// </summary>
        /// <returns>DB schema name</returns>
        public static string GetSchemaName()
        {
            return "ILC";
        }
    }
}
