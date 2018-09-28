using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;


namespace LineCheckerSrv
{
    /// <summary>
    /// Implements logger management
    /// </summary>
    internal class SessionLogging
    {
        private IAppenderAttachable connectionAppender;
        private FileAppender fileAppender;
        private string sessionFileName;
        private readonly ILog log;

        /// <summary>
        /// Initializes SessionLogging instance
        /// </summary>
        /// <param name="loggerName">logger name</param>
        public SessionLogging(string loggerName)
        {
            log = LogManager.GetLogger(loggerName);            
        }

        /// <summary>
        /// Sets session file fame
        /// </summary>
        public string SessionFileName
        {
            set { sessionFileName = value; }
        }

        /// <summary>
        /// Gets attached ILog interface
        /// </summary>
        /// <returns>ILog interface</returns>
        public ILog Logger
        {
            get { return log; }
        }

        /// <summary>
        /// Opens session logger
        /// </summary>
        public void OpenSessionLogger()
        {
            connectionAppender = (IAppenderAttachable) log.Logger;

            fileAppender = new FileAppender();

            fileAppender.File = sessionFileName;
            fileAppender.AppendToFile = true;
            fileAppender.ImmediateFlush = false;

            PatternLayout patternLayout =
                new PatternLayout("%date{yyyy-MM-dd HH:mm:ss,fff} [%thread] %-5level %message%newline");

            fileAppender.Layout = patternLayout;

            fileAppender.ActivateOptions();
            connectionAppender.AddAppender(fileAppender);
        }

        /// <summary>
        /// Closes session logger
        /// </summary>
        public void CloseSessionLogger()
        {
            if (connectionAppender != null && fileAppender != null)
            {
                connectionAppender.RemoveAppender(fileAppender);
                fileAppender.Close();
            }
        }
    }
}
