namespace Utils.CompileScripts
{
    /// <summary>
    /// Defines interface for scenario execution
    /// </summary>
    public interface IScriptingServer
    {
        /// <summary>
        /// Login
        /// </summary>
        string Login
        {
            get;
        }

        /// <summary>
        /// Password
        /// </summary>
        string Password
        {
            get;
        }

        /// <summary>
        /// Is brief call(without login or password)
        /// </summary>
        bool Brief
        {
            get;
        }

        /// <summary>
        /// Records audio into output file
        /// </summary>
        /// <param name="maxRecordTime">maximum record time</param>
        /// <param name="silenceTimeout">silence timeout</param>
        void RecordAudio(uint maxRecordTime, uint silenceTimeout);

        /// <summary>
        /// Checks audit table using login checker
        /// </summary>
        void CheckAuditTable();

        /// <summary>
        /// Sets silence to be skipped during next recording
        /// </summary>
        /// <param name="value">silence to be skipped, sec</param>
        void SkipSilence(uint value);

        /// <summary>
        /// Sets output record file
        /// </summary>
        /// <param name="value">output file name</param>
        void SetOutput(string value);

        /// <summary>
        /// Dials dtmf
        /// </summary>
        /// <param name="value">dtmf value</param>
        void Dial(string value);

        /// <summary>
        /// Fails script execution if silence was detected during previous recording
        /// </summary>
        void FailIfSilence();

        /// <summary>
        /// Sets failure reason
        /// </summary>
        /// <param name="failureReason">failure reason</param>
        void SetFailureReason(string failureReason);

    }
}