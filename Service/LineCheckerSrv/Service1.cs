using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using LineCheckerSrv.Scripting;
using Utils;
using Utils.CompileScripts;

namespace LineCheckerSrv
{
    /// <summary>
    /// Compares CronExpressions in a canonical, expression independent way
    /// </summary>
    class CronComparer : IEqualityComparer<CronExpression>
    {
        public bool Equals(CronExpression x, CronExpression y)
        {
            return x.GetExpressionSummary() == y.GetExpressionSummary();
        }

        public int GetHashCode(CronExpression obj)
        {
            return obj.GetExpressionSummary().GetHashCode();
        }
    }
    /// <summary>
    /// Implements service functionality, runs line checking
    /// </summary>
    public partial class IVRSLineChecker : ServiceBase
    {
        public static readonly string divaInstanceNameTemplate = "DivaInstance";
        public static readonly string jobNameTemplate = "CheckingJob";
        public static readonly string misfireNameTemplate = "WasMisfired";
        public static readonly string triggerNameTemplate = "trigger";
        public static readonly string triggerProjectIds = "triggerProjectIds";
        public static readonly string serviceInstance = "serviceInstance";

        private ILog log = LogManager.GetLogger(AppSettings.GetCommonLoggerName());
        private IScheduler scheduler;
        private IDictionary<CronExpression, List<int>> projectIdsByCronExpressions;
        private FileSystemWatcher fileWatcher;

        // changes settings on the fly
        private Timer settingsReloadTimer = null;
        const int TIMER_DUE = 5000;
        const int TIMER_PERIOD = 5000;
        volatile bool configFileChanged;


        public IVRSLineChecker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += UnhandledExceptionEventHandler;

                string info = string.Format(Emailer.startedMessageTempl, Environment.MachineName, DateTime.Now);
                log.Info(info);

                InitializeDivaInstance();
                InitializeSettings(GetSettingsFromDatabase());
                ServiceDAO.ResetIlcState();
                ExecutorFactory.Instance.ConstructExecutor(ServiceDAO.GetScenarios());
                projectIdsByCronExpressions = GetProjectIdsByCronExpressions();
                InitializeSchedulerAsync();

                settingsReloadTimer = new Timer(CheckSettingsVersionAndReloadIfNeeded, null, TIMER_DUE, TIMER_PERIOD);

                fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileWatcher.Filter = Process.GetCurrentProcess().MainModule.ModuleName + ".config";
                fileWatcher.EnableRaisingEvents = true;
                fileWatcher.Changed += OnConfigFileChanged;

                if (Emailer.SendEmail("", info, AppSettings.GetCommonRecipientEmailAddresses()))
                {
                    scheduler.Start();
                    // Normal exit
                    return;
                }
                log.Error("Failed to send email. Service will be stopped.");
            }
            catch (CompileErrorException e)
            {
                log.Error("Script compiler error.");
                Emailer.SendEmail(e.Message, "Script compiler error.",
                    AppSettings.GetCommonRecipientEmailAddresses(), e.FileName);
            }
            catch (Exception e)
            {
                String failureMessage = e.Message + "\n" + e.StackTrace;
                log.Error("OnStart()", e);
                // Error handling
                Emailer.SendEmail(failureMessage, Emailer.generalFailureSubject,
                                  AppSettings.GetCommonRecipientEmailAddresses());
            }
            Stop();
        }

        protected override void OnStop()
        {
            // any tear-down necessary to stop your service.
            LineCheckerJob.serviceShutdown = true;
            if (scheduler != null)
                scheduler.Shutdown(true);
            // TODO

            string info = string.Format(Emailer.stoppedMessageTempl, Environment.MachineName, DateTime.Now);
            log.Info(info);
            Emailer.SendEmail("", info, AppSettings.GetCommonRecipientEmailAddresses());
        }

        private IDictionary GetSettingsFromDatabase()
        {
            log.Info("Getting settings from database.");
            return ServiceDAO.GetIlcConfigurationSettings();
        }

        private static void CheckApplicationDataPath()
        {
            string path = AppSettings.GetApplicationDataPath();
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException("Directory \"" + path + "\" not found. Change \"" +
                                                AppSettings.ApplicationDataPathKey +
                                                "\" key in the configuration file or create directory \"" + path + "\"");
            }

            char ch = path[path.Length - 1];
            if (ch != '\\' && ch != '/')
            {
                path += "\\";
                ConfigurationManager.AppSettings[AppSettings.ApplicationDataPathKey] = path;
            }
        }

        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                case (int)ServiceCustomCommand.StartCheck:
                    DoJobsNow();
                    break;
            }
        }

        private void DoJobsNow()
        {
            log.Info("Running jobs now...");
            if (scheduler == null)
            {
                log.Error("Can't run jobs now: no one job was created");
                return;
            }

            List<int> list = new List<int>();
            foreach (KeyValuePair<CronExpression, List<int>> csi in projectIdsByCronExpressions)
            {
                list.AddRange(csi.Value);
            }
            if (list.Count == 0)
            {
                log.Info("There are no projects to check.");
                return;
            }

            JobDataMap data = new JobDataMap();
            data[triggerProjectIds] = list;
            int numChannels = AppSettings.GetOutgoingChannels();
            for (int i = 0; i < numChannels; ++i)
            {
                scheduler.TriggerJob(new JobKey(jobNameTemplate + i), data);
            }
        }

        private int GetOutgoingChannels()
        {
            int numChannels = AppSettings.GetOutgoingChannels();
            if (numChannels == -1)
            {
                log.Warn(AppSettings.OutgoingChannelsKey + " is not set properly in the configuration file.");
                int device = AppSettings.GetLineDevice();

                // TODO

                log.Info(AppSettings.OutgoingChannelsKey + " temporarily is set to " + numChannels);
                ConfigurationManager.AppSettings[AppSettings.OutgoingChannelsKey] = numChannels.ToString();
            }
            return numChannels;
        }

        /// <summary>
        /// creates diva log if filename is defined in the app.config
        /// </summary>
        void SetupDivaLog()
        {
            string fileName = AppSettings.GetDivaLogFileName();

            // TODO

            string s = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\" + fileName;
            log.Info("Diva log will be written to " + s + ".");

            // TODO
        }

        static void WriteConfigToLog(ILog log)
        {
            log.Info("Configuration settings are following:");
            log.Info("-------------------------------------");
            foreach (string key in ConfigurationManager.AppSettings)
            {
                log.Info(key + " = " + ConfigurationManager.AppSettings[key]);
            }
            log.Info("-------------------------------------");
        }

        void InitializeDivaInstance()
        {
            SetupDivaLog();

            // TODO
        }

        void InitializeSettings(IDictionary settings)
        {
            AppSettings.AppSettingsMap = settings;
            WriteConfigToLog(log);

            CheckApplicationDataPath();

            // throws exception if the string is invalid
            new CronExpression(AppSettings.GetSummaryReportDay());
        }

        async void InitializeSchedulerAsync()
        {
            int numChannels = GetOutgoingChannels();

            NameValueCollection properties = new NameValueCollection();

            properties["quartz.threadPool.threadCount"] = numChannels.ToString();
            uint misfireThreshold = AppSettings.GetJobMisfireThreshold() * 1000; // milliseconds
            properties["quartz.jobStore.misfireThreshold"] = misfireThreshold.ToString();

            ISchedulerFactory sf = new StdSchedulerFactory(properties);

            scheduler = await sf.GetScheduler();

            scheduler.ListenerManager.AddTriggerListener(new MisfiredListener());

            for (int i = 0; i < numChannels; ++i)
            {
                IJobDetail job = JobBuilder.Create(typeof(LineCheckerJob)).WithIdentity(jobNameTemplate + i).Build();//  new JobDetail(jobNameTemplate + i, null, typeof(LineCheckerJob));
                //job.JobDataMap[divaInstanceNameTemplate] = divaInstance;
                job.JobDataMap[serviceInstance] = this;

                bool isFirst = true;
                foreach (KeyValuePair<CronExpression, List<int>> csi in projectIdsByCronExpressions)
                {
                    string cronExpressionString = csi.Key.CronExpressionString;
                    //Trigger trigger = new CronTrigger(triggerNameTemplate + cronExpressionString + i, null, jobNameTemplate + i, null,
                    //    cronExpressionString);
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithCronSchedule(cronExpressionString)
                        .ForJob(jobNameTemplate + i)
                        .WithIdentity(triggerNameTemplate + cronExpressionString + i).Build();
                    trigger.JobDataMap[triggerProjectIds] = csi.Value;

                    // TODO fix?
                    //trigger.AddTriggerListener("MisfiredListener");

                    if (isFirst)
                    {
                        await scheduler.ScheduleJob(job, trigger);
                        isFirst = false;
                    }
                    else
                    {
                        await scheduler.ScheduleJob(trigger);
                    }

                }
            }

        }

        IDictionary<CronExpression, List<int>> GetProjectIdsByCronExpressions()
        {
            log.Info("Getting project Ids grouped by cron expressions.");
            IList<KeyValuePair<string, int>> list = ServiceDAO.GetCronStringsByProjectIds();
            Dictionary<CronExpression, List<int>> expressions = new Dictionary<CronExpression, List<int>>(new CronComparer());
            foreach (KeyValuePair<string, int> kvp in list)
            {
                CronExpression cronExp = new CronExpression(kvp.Key);
                List<int> projectsList;
                if (!expressions.TryGetValue(cronExp, out projectsList))
                {
                    projectsList = new List<int>();
                    expressions[cronExp] = projectsList;
                }
                projectsList.Add(kvp.Value);
            }

            return expressions;
        }

        /// <summary>
        /// changes settings on the fly if needed
        /// </summary>
        /// <param name="stateInfo">parameter required by callback signature</param>
        public void CheckSettingsVersionAndReloadIfNeeded(Object stateInfo)
        {
            log.Info("CheckSettingsVersionAndReloadIfNeeded()...");
            try
            {
                // To avoid Timer Event Reentrance
                settingsReloadTimer.Change(Timeout.Infinite, Timeout.Infinite);

                if (!IsReloadNeeded())
                    return;

                // restart quartz
                scheduler.Shutdown(true);

                IDictionary settings = null;
                try
                {
                    settings = GetSettingsFromDatabase();
                    projectIdsByCronExpressions = GetProjectIdsByCronExpressions();

                    log.Info("Trying to apply new settings. Version: " + settings[AppSettings.SettingsVersion]);

                }
                catch (Exception ex)
                {
                    log.Info("Can't get service settings from database", ex);
                    log.Info("Trying to restore previous settings. Version: " + AppSettings.GetSettingsVersion());
                    settings = AppSettings.AppSettingsMap;
                }

                configFileChanged = false;

                foreach (string s in new string[] {"appSettings", "connectionStrings", "log4net", "common/logging"})
                {
                    ConfigurationManager.RefreshSection(s);
                }

                LogManager.Adapter = null;
                log = LogManager.GetLogger(AppSettings.GetCommonLoggerName());

                InitializeSettings(settings);

                InitializeSchedulerAsync();
                scheduler.Start();

            }
            catch (Exception e)
            {
                // we can't get settings
                String failureMessage = e.Message + "\n" + e.StackTrace;
                log.Error("CheckSettingsVersionAndReloadIfNeeded", e);

                Emailer.SendEmail(failureMessage, Emailer.generalFailureSubject,
                                  AppSettings.GetCommonRecipientEmailAddresses());

                Stop();
            }
            finally
            {
                settingsReloadTimer.Change(TIMER_DUE, TIMER_PERIOD);
            }
        }

        private bool IsReloadNeeded()
        {
             if (configFileChanged)
                 return true;

             try
             {
                 uint ver = ServiceDAO.GetSettingsVersion();
                 if (AppSettings.GetSettingsVersion() != ver)
                 {
                     log.Info("Service settings have been changed from " + AppSettings.GetSettingsVersion() +
                              " to " + ver);
                     return true;
                 }
             }
             catch (Exception ex)
             {
                 log.Info("Can't get service settings version from database", ex);
             }
             return false;
        }

        private void OnConfigFileChanged(object source, FileSystemEventArgs e)
        {
            log.Info("OnConfigFileChanged()...");
            configFileChanged = true;
        }

        private void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.ExceptionObject is Exception)
            {
                log.Error("UnhandledExceptionEventHandler caught exception", (Exception)args.ExceptionObject);
            }
            else
            {
                log.Error("UnhandledExceptionEventHandler caught exception object: " + args.ExceptionObject);                
            }
        }
    }
}
