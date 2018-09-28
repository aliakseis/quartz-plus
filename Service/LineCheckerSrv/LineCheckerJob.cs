using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using Common.Logging;
using LineCheckerSrv.LoginCheckersConfig;
using LineCheckerSrv.Scripting;
using Quartz;
using System.IO;
using Utils;
using SessionLog = log4net.ILog;
using System.ServiceProcess;
using Utils.CompileScripts;


namespace LineCheckerSrv
{
    /// <summary>
    /// Implements line checking using Diva SDK
    /// </summary>
    internal class LineCheckerJob : IScriptingServer
    {
        # region private members

        private const string notAvailable = "n/a";

        private CallResultCode callResult;
        private string failureReason;
        private bool isNewSession;
        private int itemId;
        private string jobName;
        private SessionLog log;
        private string login;
        private string password;
        private string phone;
        private string projectName;
        private string serverName;
        private string authLoginDbConn;
        private int sessionId;
        private SessionLogging sessionLogging;

		// Implement Authentication Checking by plug-ins
        private string authCheckerName;

        /// <summary>
        /// Time to wait for session state change / finalization
        /// before reporting, msecs
        /// </summary>
        private int sleepTime = 10000;

        private DateTime startTime;
        private DateTime? previousStartTime;

        private DateTime? beforeLoginTime;

        private string workerName;

        private int verificationAttempts;

        private DateTimeOffset? fireTimeUtc;

        private int scenarioId;

        // moved from CheckerDAO
        private object rowId;
        
        private string outputFile = "";
        List<string> files = new List<string>();
        DivaCheckCall call;
        uint skipSilence;
        bool isSilence = false;
        string callStage = "";

        #endregion

        #region public properties

        /// <summary>
        /// True if service was stopped
        /// </summary>
        public static volatile bool serviceShutdown;

        /// <summary>
        /// Worker name
        /// </summary>
        public string WorkerName
        {
            get { return workerName; }
            set { workerName = value; }
        }

        /// <summary>
        /// Session id
        /// </summary>
        public int SessionId
        {
            get { return sessionId; }
            set { sessionId = value; }
        }

        /// <summary>
        /// Phone number to call 
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        /// <summary>
        /// User name to authenticate
        /// </summary>
        public string Login
        {
            get { return login; }
            set { login = value ?? ""; }
        }

        /// <summary>
        /// User password to authenticate
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value ?? ""; }
        }

        public bool Brief
        {
            get { return (login.Length == 0 || password.Length == 0); }
        }

        /// <summary>
        /// Item id
        /// </summary>
        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        /// <summary>
        /// Server name
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        /// <summary>
        /// DB connection string to authentication check
        /// </summary>
        public string AuthLoginDbConn
        {
            get { return authLoginDbConn; }
            set { authLoginDbConn = value; }            
        }

        /// <summary>
        /// Session start time
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Previous session start time
        /// </summary>
        public DateTime? PreviousStartTime
        {
            get { return previousStartTime; }
            set { previousStartTime = value; }
        }

        /// <summary>
        /// Number of verification attempts
        /// </summary>
        public int VerificationAttempts
        {
            get { return verificationAttempts; }
            set { verificationAttempts = value; }
        }

		// Implement Authentication Checking by plug-ins
        /// <summary>
        /// authentication checker name
        /// </summary>
        public string AuthCheckerName
        {
            get { return authCheckerName; }
            set { authCheckerName = value; }
        }

        public int ScenarioId
        {
            get { return scenarioId; }
            set { scenarioId = value; }
        }

        /// <summary>
        /// Calculates time span since last verification in days
        /// </summary>
        /// <returns>Time span since last verification in days</returns>
        public double GetTimeSpanSinceLastVerificationInDays()
        {
            double value = AppSettings.GetTimeSpanBetweenVerificationsInDays();
            if (fireTimeUtc != null)
                value += (DateTimeOffset.UtcNow - (DateTimeOffset) fireTimeUtc).TotalDays;
            log.Debug("Time span since last verification in days = " + value);
            return value;
        }
        

        #endregion
       
        #region IStatefulJob Members

        /// <summary> 
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="Trigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute(IJobExecutionContext context)
        {
            jobName = context.JobDetail.Key.Name;
            fireTimeUtc = context.FireTimeUtc;

            sessionLogging = new SessionLogging(jobName);
            log = sessionLogging.Logger;

            log.Info("Execute trigger: " + context.Trigger.Key.Name);
            object objMisfired = context.Trigger.JobDataMap[IVRSLineChecker.misfireNameTemplate];
            bool wasMisfired = objMisfired != null ? (bool) objMisfired : false;
            context.Trigger.JobDataMap[IVRSLineChecker.misfireNameTemplate] = false;

            ICollection<int> prjIds = (ICollection<int>)context.Trigger.JobDataMap[IVRSLineChecker.triggerProjectIds];

            try
            {
                DoExecute(context.JobDetail.JobDataMap.Get(IVRSLineChecker.divaInstanceNameTemplate),
                          wasMisfired, prjIds);
            }
            catch (CompileErrorException e)
            {
                log.Error("Script compiler error.");
                Emailer.SendEmail(e.Message, "Script compiler error.",
                    AppSettings.GetCommonRecipientEmailAddresses(), e.FileName);
                context.Scheduler.Shutdown(false);
                ServiceBase service = (ServiceBase)context.JobDetail.JobDataMap[IVRSLineChecker.serviceInstance];
                service.Stop();
            }
        }

        #endregion

        #region private methods

        private void InitializeSession(bool wasMisfired, ICollection<int> prjIds)
        {
            // One point of decision making, see INITIALIZE_CHECKLIST stored proc. for details
            isNewSession = CheckerDAO.InitializeSession(this, wasMisfired, prjIds);
        }

        private bool GetCheckData()
        {
            int actualScriptsVersion;
            if (CheckerDAO.GetCheckData(this, out actualScriptsVersion, out rowId))
            {
                // Decrypt password if it is encrypted
                if (new List<char>(password.ToUpper())
                    .FindIndex(delegate(char c) { return "0123456789ABCD*#".IndexOf(c) == -1; }) != -1)
                {
                    password = Connect.XCode(password, false);
                }
                
                ExecutorFactory.Instance.RecompileIfNeed(actualScriptsVersion);

                return true;
            }

            return false;
        }

        private bool DoWeNeedToContinue()
        {
            return CheckerDAO.DoWeNeedToContinue(sessionId);
        }

        private void MakeReport()
        {
            DateTime endTime = startTime;
            int successCount = 0;
            int failedCount = 0;

            log.Info("Getting report data.");
            IList<ReportItem> reportData = CheckerDAO.GetReportData(sessionId);

            StringBuilder strBuilder = null;
            StringBuilder strSucceededBuilder = new StringBuilder();
            StringBuilder strFailedBuilder = new StringBuilder();

            strFailedBuilder.AppendLine("Project\tServer\tNumber\tUserId\tTime\tStatus\tReason\r\n");


            foreach (ReportItem item in reportData)
            {
                if (item.status == "SUCCEEDED")
                {
                    ++successCount;
                }
                else
                {
                    ++failedCount;
                }

                switch (item.status)
                {
                    case "SUCCEEDED":
                        strBuilder = strSucceededBuilder;
                        break;
                    default:
                        strBuilder = strFailedBuilder;
                        break;
                }

                strBuilder
                    .AppendFormat("{0}\t", item.projectName)
                    .AppendFormat("{0}\t", item.serverName)
                    .AppendFormat("{0}\t", item.phone)
                    .AppendFormat("{0}\t", String.IsNullOrEmpty(item.userId) ? notAvailable : item.userId)
                    .AppendFormat("{0}\t", item.verificationTime)
                    .AppendFormat("{0}\t", (item.status == "AWAITING") ? "FAILED" : item.status)
                    .Append((item.status == "AWAITING" && item.attemps < AppSettings.GetAttemptsMaxNumber())
                        ? "Not checked due to session termination"
                        : item.reason)
                    .Append("\t\r\n");
            }

            if (reportData.Count != 0)
                endTime = reportData[reportData.Count - 1].verificationTime;

            StringBuilder strHeaderBuilder = new StringBuilder();
            strHeaderBuilder
                .AppendLine("IVRS Lines Check Summary")
                .AppendLine("------------------------")
                .AppendFormat("Start Time: {0}\r\n", startTime)
                .AppendFormat("Stop Time: {0}\r\n", endTime)
                .AppendFormat("Total Successful: {0}\r\n", successCount)
                .AppendFormat("Total Failed: {0}\r\n\r\n", failedCount)
                .AppendLine("IVRS Lines Check Details")
                .Append("------------------------\r\n\r\n");

            string strSubject = String.Format(Emailer.alertSubjectTempl, Environment.MachineName, endTime, successCount,
                                              failedCount);
            Emailer.SendEmail(strHeaderBuilder + strFailedBuilder.ToString() + strSucceededBuilder,
                strSubject, GetEmailAddresses(true, false));
        }

        private void MakeSummaryReport(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            StringBuilder strBodyBuilder = new StringBuilder();

            log.Info("Getting summary report data.");
            ICollection<SummaryReportItem> reportData = CheckerDAO.GetSummaryReportData(startDate, endDate);

            strBodyBuilder.AppendLine("PROJECT\tSERVER\tPHONE\tUSERID\tCHECKS\tSUCCESS\t"+
                "FAILED\t%SUCCESS\tLAST SUCCESS\tLAST FAILED\r\n");

            SortedDictionary<string, SummaryServerItemInfo> serverDictionary =
                new SortedDictionary<string, SummaryServerItemInfo>();
            int totalChecks = 0;
            int totalSuccess = 0;

            foreach(SummaryReportItem item in reportData)
            {
                int checkSuccess = item.checksCount - item.failedChecksCount;
                strBodyBuilder
                    .AppendFormat("{0}\t", item.projectName)
                    .AppendFormat("{0}\t", item.serverName)
                    .AppendFormat("{0}\t", item.phone)
                    .AppendFormat("{0}\t", String.IsNullOrEmpty(item.userId) ? notAvailable : item.userId)
                    .AppendFormat("{0}\t", item.checksCount)
                    .AppendFormat("{0}\t", checkSuccess)
                    .AppendFormat("{0}\t", item.failedChecksCount)
                    .AppendFormat("{0:P1}\t", (float)checkSuccess / item.checksCount)
                    .AppendFormat("{0}\t", item.lastSuccessDate)
                    .AppendFormat("{0}\t\r\n", item.lastFailedDate);

                totalChecks += item.checksCount;
                totalSuccess += checkSuccess;

                SummaryServerItemInfo serverInfo;
                if (!serverDictionary.TryGetValue(item.serverName, out serverInfo))
                {
                    serverInfo = new SummaryServerItemInfo();
                    serverDictionary[item.serverName] = serverInfo;
                }

                serverInfo.projectDictionary[item.projectName] = 0;
                
                serverInfo.checksCount += item.checksCount;
                serverInfo.failedChecksCount += item.failedChecksCount;
                if (Nullable.Compare(serverInfo.lastFailedDate, item.lastFailedDate) < 0)
                    serverInfo.lastFailedDate = item.lastFailedDate;
                if (Nullable.Compare(serverInfo.lastSuccessDate, item.lastSuccessDate) < 0)
                    serverInfo.lastSuccessDate = item.lastSuccessDate;
            }
            strBodyBuilder.AppendLine("------------------------\r\n\r\n");
        
            int numProjects = 0;

            StringBuilder strServerTableBuilder = new StringBuilder();
            strServerTableBuilder.AppendLine("SERVER\tCHECKS\tSUCCESS\tFAILED\t%SUCCESS\tLAST SUCCESS\tLAST FAILED\r\n");
            foreach (KeyValuePair<string, SummaryServerItemInfo> server in serverDictionary)
            {
                strServerTableBuilder.AppendFormat("{0}\t", server.Key);
                int success = server.Value.checksCount - server.Value.failedChecksCount;
                strServerTableBuilder.AppendFormat("{0}\t", server.Value.checksCount);
                strServerTableBuilder.AppendFormat("{0}\t", success);
                strServerTableBuilder.AppendFormat("{0}\t", server.Value.failedChecksCount);
                strServerTableBuilder.AppendFormat("{0:P1}\t", (float)success / server.Value.checksCount);
                strServerTableBuilder.AppendFormat("{0}\t", server.Value.lastSuccessDate);
                strServerTableBuilder.AppendFormat("{0}\t\r\n", server.Value.lastFailedDate);

                numProjects += server.Value.projectDictionary.Count;
            }

            log.Info("reportData count = " + reportData.Count + " totalSuccess  = " + totalSuccess + " totalChecks = " + totalChecks);
            StringBuilder strHeaderBuilder = new StringBuilder();
            strHeaderBuilder
                .AppendFormat("Report Begin Date: {0}\r\n", startDate.ToString())
                .AppendFormat("Report End Date: {0}\r\n\r\n", endDate.ToString())
                .AppendFormat("Total Projects: {0}\r\n", numProjects)
                .AppendFormat("Total Servers: {0}\r\n", serverDictionary.Count)
                .AppendFormat("Total Checks: {0}\r\n", totalChecks)
                .AppendFormat("Total Success: {0}\r\n", totalSuccess)
                .AppendFormat("Total Failed: {0}\r\n", totalChecks - totalSuccess);
                if(totalChecks != 0)
                {
                    strHeaderBuilder.AppendFormat("Total % Success: {0:P1}\r\n", (float)totalSuccess / totalChecks);
                }
                strHeaderBuilder.AppendLine("------------------------\r\n\r\n");

            string strSubject = String.Format(Emailer.weeklyReportSubjectTempl,
                startDate.ToString(), endDate.ToString());

            string strContent = strHeaderBuilder + strBodyBuilder.ToString() + strServerTableBuilder;

            string summaryReportFileName = AppSettings.GetApplicationDataPath() + 
                    "SummaryReport_" + startDate.ToString("MM_dd_yyyy") + "-" + endDate.ToString("MM_dd_yyyy") + ".txt";
            log.Info("Writing summary report to file: " + summaryReportFileName);
            File.WriteAllText(summaryReportFileName, strContent);

            // This column will list all email addresses that Summary Report should be sent to.
            Emailer.SendEmail(strContent, strSubject, AppSettings.GetSummaryRecipientEmailAddresses());
        }

        private static bool RecordDurationExceeds(string fileName, uint seconds)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            return fileInfo.Exists
                // Produced wave file size semi-empiric formula
                   && fileInfo.Length > ((8000 * seconds + 0x200) /*& 0xFFFFFE00*/) + 46;
        }

        private void SaveCallResults()
        {
            CheckerDAO.SaveCallResults(GetResultString(callResult), failureReason, rowId);
        }

        private void HandleProjectFailure(string callStage, IEnumerable<string> files)
        {
            log.Info("Failure detected: " + failureReason + " for Phone #:" + phone + " User login:" +
                      ((login.Length == 0) ? notAvailable : login) + ".");

            // Sending failure email
            List<string> sendFiles = new List<string>(files);
            sendFiles.Add(GetSessionLogFilePath());
            string strSubject = String.Format(Emailer.projectFailureSubjectTempl,
                                            Environment.MachineName, projectName, callStage);
            string strBody = String.Format(Emailer.projectFailureBodyTempl, failureReason,
                phone, ((login.Length == 0) ? notAvailable : login), serverName);

            sessionLogging.CloseSessionLogger();
            Emailer.SendEmail(strBody, strSubject, GetEmailAddresses(true, true), sendFiles.ToArray());
            sessionLogging.OpenSessionLogger();
        }

        private bool GetReportFlag()
        {
            int reportFlagVal = CheckerDAO.GetReportFlag(sessionId);
            return reportFlagVal != 0;
        }

        private string GetEmailAddresses(bool commonRecipient, bool projectSpecificRecipient)
        {
            string emails = "";
            if (commonRecipient)
            {
                emails = AppSettings.GetCommonRecipientEmailAddresses();
            }

            if (projectSpecificRecipient)
            {
                string projectEmails = CheckerDAO.GetEmailAddresses(rowId);
                if (!string.IsNullOrEmpty(projectEmails))
                {
                    if (emails.Length != 0)
                        emails += ",";

                    emails += projectEmails;
                }
            }

            return emails;
        }


        private string GetSessionLogFilePath()
        {
            return AppSettings.GetApplicationDataPath() + "session_" + sessionId + "_" +
                  itemId + ".log";
        }


        private void ExecuteSummaryReport()
        {
            if (previousStartTime == null)
                return;

            // Defining summary report frames
            DateTimeOffset dtBegin, dtEnd;
            TimeZone timeZone = TimeZone.CurrentTimeZone;
            try
            {
                CronExpression expression = new CronExpression(AppSettings.GetSummaryReportDay());

                dtEnd = (DateTimeOffset)expression.GetTimeAfter(timeZone.ToUniversalTime((DateTime)previousStartTime));
                if (dtEnd == expression.GetTimeAfter(timeZone.ToUniversalTime(startTime)))
                    return;

                log.Info("Execute summary report");

                int dayCount = 0;
                DateTime dtBeginProbe = timeZone.ToUniversalTime((DateTime)previousStartTime);
                do
                {
                    dtBeginProbe = dtBeginProbe.AddDays(-1);
                    dtBegin = (DateTimeOffset)expression.GetTimeAfter(dtBeginProbe);
                    ++dayCount;
                }
                while (dtBegin == dtEnd && dayCount <= 366);
            }
            catch (Exception e)
            {
                log.Error("Invalid summary report expression never generates a valid fire date", e);
                return;
            }

            //dtBegin = timeZone.ToLocalTime(dtBegin);
            //dtEnd = timeZone.ToLocalTime(dtEnd);
            log.Info("Summary report frame is: " + dtBegin + " - " + dtEnd);

            MakeSummaryReport(dtBegin, dtEnd);
        }

        private void DoExecute(object instance, bool wasMisfired, ICollection<int> prjIds)
        {
            try
            {
                log.Info("Initialize session.");
                InitializeSession(wasMisfired, prjIds);

                if (isNewSession)
                {
                    // log ILC version periodically
                    string ver = Assembly.GetExecutingAssembly().Location;
                    if (!string.IsNullOrEmpty(ver))
                        log.Info("IVRS Line Checker version: " + FileVersionInfo.GetVersionInfo(ver).ProductVersion);

                    ExecuteSummaryReport();
                    if (previousStartTime != null
                        && ((DateTime)previousStartTime).Date != startTime.Date)
                    {
                        LogCleaner.Clean(AppSettings.GetApplicationDataPath(),
                                         new string[] {"wav", "log"}, AppSettings.GetKeepTempFilesDays());
                    }
                }

                // One point of decision making, see INITIALIZE_CHECKLIST stored proc. for details
                if (isNewSession && wasMisfired) 
                {
                    log.Info("Misfire handling.");
                    Emailer.SendEmail("",
                                      String.Format(Emailer.skippedMessageTempl, Environment.MachineName),
                                      GetEmailAddresses(true, false));
                    return;
                }

                log.Info("Start session.");

                log.Info("Session started at: " + startTime + "; previous session started at: " + previousStartTime);

                log.Info("Service " + (serviceShutdown ? "is shutting down." : "is working."));
                while (!serviceShutdown)
                {
                    log.Info("Getting item to proceed.");
                    if (!GetCheckData())
                    {
                        log.Info("There are no items to proceed at this moment. Checking continuation possibility.");
                        if (DoWeNeedToContinue() && !serviceShutdown)
                        {
                            log.Info("Continuation is possible. Waiting for some time.");
                            Thread.Sleep(sleepTime);
                            continue;
                        }
                        log.Info(serviceShutdown
                                     ? "Service is shutting down."
                                     : "There are no items to proceed in current session.");
                        break;
                    }
                    
                    sessionLogging.SessionFileName = GetSessionLogFilePath();
                    sessionLogging.OpenSessionLogger();

                    // TODO

                    log.Info("Saving call results.");
                    SaveCallResults();
                    sessionLogging.CloseSessionLogger();
                }

                log.Info("Checking if report can be generated.");
                if (GetReportFlag())
                {
                    // use IS_REPORTED in SET_WORKER_STATUS
                    // to avoid starting new checks afterwards if serviceShutdown == true
                    if (serviceShutdown)
                    {
                        while (CheckerDAO.DoWeHaveWorkers(sessionId))
                        {
                            log.Info("Waiting for all proceeding items to be processed.");
                            Thread.Sleep(sleepTime);
                        }
                    }
                    log.Info("Making report.");
                    MakeReport();
                }
                log.Info("Stop session.");
            }
            finally
            {
                if (sessionLogging != null)
                {
                    sessionLogging.CloseSessionLogger();
                }
            }
        }

        private static string GetResultString(CallResultCode callResult)
        {
            switch (callResult)
            {
                case CallResultCode.Success:
                    return "SUCCEEDED";
                case CallResultCode.ErrorDestBusy:
                case CallResultCode.ErrorConnectionFailed:
                    return "AWAITING";
                default:
                    return "FAILED";
            }
        }

        private void CheckRecordFailure()
        {
            if (call.state == DivaCheckCall.CallCurrentState.RecordFailure)
            {
                throw new ScriptingException("Failed to record file " + outputFile);
            }
        }

        #endregion

        #region IScriptingServer Functions

        public void SetOutput(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ScriptingException("Bad output file  name was set.");
            outputFile = AppSettings.GetApplicationDataPath() + value + sessionId + "_" + itemId + ".wav";
            log.Info("Set output file: " + outputFile + ".");
        }

        public void RecordAudio(uint maxRecordTime, uint silenceTimeout)
        {
            log.Info("RecordAudio()...");

            if (call.state == DivaCheckCall.CallCurrentState.Disconnected)
            {
                throw new ScriptingException("Disconnected");
            }

            callStage = "recording voice file: " + outputFile;

            if (string.IsNullOrEmpty(outputFile))
                throw new ScriptingException(callStage + ": output file need to be set.");

            files.Add(outputFile);

            int i = (int)skipSilence;
            if (i != 0)
            {
                log.Info("Skipping silence: " + i + " sec.");
                // TODO
                while (--i >= 0)
                {
                    log.Debug("Recording voice file: " + outputFile + ".");
                    // TODO
                    call.Wait(); // Pulse() in DivaCheckCall event handler

                    CheckRecordFailure();

                    if (call.state == DivaCheckCall.CallCurrentState.Disconnected)
                    {
                        throw new ScriptingException("Disconnected");
                    }
                    
                    // Leading silence timeout functionality added
                    if (!RecordDurationExceeds(outputFile, 1))
                    {
                        log.Debug("RecordVoiceFile: leading silence detected.");
                        continue;
                    }
                    break;
                }

                skipSilence = 0;
                if (i == -1)
                {
                    isSilence = true;
                    return;
                }

                if (maxRecordTime > 2)
                    maxRecordTime -= 2;
            }

            // we append voice stream data to output file, if it doesn't exist we create one
            // TODO
            log.Debug("Appending voice file: " + outputFile + ".");
            // TODO
            call.Wait(); // Pulse() in DivaCheckCall event handler
            CheckRecordFailure();

            // TODO: do we need to check file content for silence only?
            // Leading silence timeout functionality added
            isSilence = !RecordDurationExceeds(outputFile, silenceTimeout);
        }

        public void SkipSilence(uint value)
        {
            skipSilence = value;
        }
        
        public void FailIfSilence()
        {
            if (isSilence)
            {
                throw new ScriptingException("Silence detected");
            }
        }

        public void Dial(string value)
        {
            if (call.state == DivaCheckCall.CallCurrentState.Disconnected)
            {
                throw new ScriptingException("Disconnected");
            }

            callStage = "dialing " + value;
            if (beforeLoginTime == null)
                beforeLoginTime = DateTime.Now;

            log.Info("Sending DTMF: " + value + ".");
            // TODO
            call.Wait(); // Pulse() in DivaCheckCall event handler
            if (call.state != DivaCheckCall.CallCurrentState.DtmfToneSent)
                throw new ScriptingException("Failed to send dtmf: " + value);
        }

        public void CheckAuditTable()
        {
            log.Info("Checking audit table");
            if (string.IsNullOrEmpty(AuthCheckerName))
            {
                log.Info("AuthChecker is not set into DB.");
                return;
            }

            callStage = "checking audit table";
            string failure = "";

            try
            {
                ProjectConfigurationSection configurationSection =
                   (ProjectConfigurationSection)System.Configuration.ConfigurationManager.GetSection(
                   "loginCheckersConfiguration");

                CheckerElement checker = configurationSection.Checkers[AuthCheckerName];
                if (checker == null)
                {
                    failure = "AuthChecker \"" + AuthCheckerName +
                                    "\" configuration was not found in configuration file.";
                }
                else
                {
                    Assembly checkerAssembly = Assembly.Load(checker.Module);

                    MethodInfo method = checkerAssembly.GetType(checker.Class).GetMethod(checker.Method);

                    object[] pars = new object[] { AuthLoginDbConn, Login, beforeLoginTime, LogManager.GetLogger(jobName), null };
                    if ((bool)method.Invoke(null, pars))
                        return;

                    failure = (string)pars[4];
                }
            }
            // catch invocation exceptions:  inner one contains more detailed information
            catch (TargetInvocationException ex)
            {
                failure = "TargetInvocationException while AuthLogin checking: " + ex.Message + "\nInnerException: " + ex.InnerException.Message;
            }
            // Unfortunately we can't predict what other exceptions can be thrown
            catch (Exception ex)
            {
                failure = "Exception while AuthLogin checking: " + ex.Message;
            }

            throw new ScriptingException(failure);

        }

        public void SetFailureReason(string failureReason)
        {
            this.failureReason = failureReason;
        }

        #endregion
    }
}
