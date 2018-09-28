namespace Utils.CompileScripts
{
    /// <summary>
    /// Base class for scenario execution. Delegates most operations to server
    /// </summary>
    public class ScriptingBase
    {
        /// <summary>
        /// Scenario delegate
        /// </summary>
        public delegate void ScenarioDelegate();

        private IScriptingServer server = null;

        /// <summary>
        /// Initialize ScriptingBase instance
        /// </summary>
        /// <param name="server">IScriptingServer interface</param>
        public void Initialize(IScriptingServer server)
        {
            this.server = server;
        }

        /// <summary>
        /// -- operator(Is used  to end script execution)
        /// </summary>
        /// <param name="scripting">ScriptingBase object</param>
        /// <returns>ScriptingBase object(itself)</returns>
        public static ScriptingBase operator --(ScriptingBase scripting)
        {
            return scripting;
        }

        
        /// <summary>
        /// Is used  to end script execution
        /// </summary>
        /// <returns>ScriptingBase object(itself)</returns>
        public ScriptingBase end()
        {
            return this;
        }

        /// <summary>
        /// Fails script execution if silence was detected during previous recording
        /// </summary>
        public ScriptingBase FailIfSilence
        {
            get
            {
                server.FailIfSilence();
                return this;
            }
            set {}
        }

        /// <summary>
        /// Login
        /// </summary>
        public string UserId
        {
            get
            {
                return server.Login;
            }
            set {}
        }

        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get
            {
                return server.Password;
            }
            set {}
        }

        /// <summary>
        /// Is brief call(without login or password)
        /// </summary>
        public bool Brief
        {
            get
            {
                return server.Brief;
            }
            set {}
        }

        /// <summary>
        /// Dials phone number
        /// </summary>
        /// <param name="value">destination number</param>
        /// <returns>ScriptingBase object(itself)</returns>
        public ScriptingBase Dial(string value)
        {
            server.Dial(value);
            return this;
        }

        /// <summary>
        /// Checks audit table using login checker
        /// </summary>
        public ScriptingBase CheckAuditTable
        {
            get
            {
                server.CheckAuditTable();
                return this;
            }
            set { }
        }

        /// <summary>
        /// Sets silence to be skipped during next recording
        /// </summary>
        /// <param name="value">silence to be skipped, sec</param>
        public ScriptingBase SkipSilence(uint value)
        {
            server.SkipSilence(value);
            return this;
        }

        /// <summary>
        /// Sets output record file
        /// </summary>
        /// <param name="value">output file name</param>
        /// <returns>ScriptingBase object(itself)</returns>
        public ScriptingBase SetOutput(string value)
        {
            server.SetOutput(value);
            return this;
        }

        /// <summary>
        /// Records audio into output file
        /// </summary>
        /// <param name="maxRecordTime">maximum record time</param>
        /// <param name="silenceTimeout">silence timeout</param>
        /// <returns>ScriptingBase object(itself)</returns>
        public ScriptingBase RecordAudio(uint maxRecordTime, uint silenceTimeout)
        {
            server.RecordAudio(maxRecordTime, silenceTimeout);
            return this;
        }

        /// <summary>
        /// Records audio into output file
        /// </summary>
        /// <param name="outputFile">output file</param>
        /// <param name="maxRecordTime">maximum record time</param>
        /// <param name="silenceTimeout">silence timeout</param>
        /// <returns>ScriptingBase object(itself)</returns>
        public ScriptingBase RecordAudio(string outputFile, uint maxRecordTime, uint silenceTimeout)
        {
            SetOutput(outputFile);
            RecordAudio(maxRecordTime, silenceTimeout);
            return this;
        }
    }
}