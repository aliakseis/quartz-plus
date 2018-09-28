using System.Threading;
using SessionLog = log4net.ILog;

namespace LineCheckerSrv
{
    /// <summary>
    /// Implements handling call-related events
    /// </summary>
    internal class DivaCheckCall
    {
        #region CallCurrentState enum

        /// <summary>
        /// states of a call
        /// </summary>
        public enum CallCurrentState
        {
            Connected = 0,
            Disconnected = 1,
            RecordFailure = 2,
            Busy = 3,
            DtmfToneSent = 4,
            // Not clear whether so called spurious wakeups can happen here.
            // Let's protect ourselves against them for any case.
            Waiting = 5,
        }

        #endregion

        private readonly SessionLog log;
        /// <summary>
        /// current state of the call
        /// </summary>
        public volatile CallCurrentState state;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionlog">session log</param>
        public DivaCheckCall(SessionLog sessionlog)
        {
            log = sessionlog;
        }


        public void Wait()
        {
            state = CallCurrentState.Waiting;
            do
            {
                log.Debug("Before Monitor.Wait()...");
                Monitor.Wait(this); // Pulse() in DivaCheckCall event handler
                log.Debug("After Monitor.Wait()...");
            } 
            while (state == CallCurrentState.Waiting);
        }
    }

    /// <summary>
    /// result codes of a call
    /// </summary>
    public enum CallResultCode
    {
        Success = 0,
        ErrorDestBusy = 1,
        ErrorConnectionFailed = 2,
        ScenarioFailed = 3,
        ErrorUnexpected = 4,
    }
}