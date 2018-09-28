using System;
using Common.Logging;

namespace ILCWebApplication.HandleException
{
    /// <summary>
    /// Implements exceptions logging
    /// </summary>
    public class ExceptionLogger
    {
        private readonly ILog log = LogManager.GetLogger(WebSettings.GetCommonLoggerName());

        /// <summary>
        /// Handles server exception
        /// </summary>
        /// <param name="ex">exception</param>
        public void HandleException(Exception ex)
        {
            log.Error(ex);
        }
    }
}