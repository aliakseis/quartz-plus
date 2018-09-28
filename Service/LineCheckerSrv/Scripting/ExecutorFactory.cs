using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using Utils.CompileScripts;
using Common.Logging;

namespace LineCheckerSrv.Scripting
{
    /// <summary>
    /// Implements functionality of compiling and running scenario scripts
    /// </summary>
    class ExecutorFactory
    {
        private static ExecutorFactory executorFactory = new ExecutorFactory();

        private volatile Assembly compiledAssembly;
        private int scriptsVersion = -1;
        private object syncObject = new object();

        private ExecutorFactory() {}
 
        /// <summary>
        /// Return alone instance of instance(singleton)
        /// </summary>
        public static ExecutorFactory Instance
        {
            get { return executorFactory; }
        }

        /// <summary>
        /// Compile scenarios into assembly
        /// </summary>
        /// <param name="scenarioItems">scenario list</param>
        public void ConstructExecutor(IList<ScenarioItem> scenarioItems)
        {
            if (scriptsVersion == -1)
                scriptsVersion = AppSettings.GetScriptsVersion();

            GetLogger().Info("Scripts compiling.");
            compiledAssembly = ScriptsCompiling.CompileAssembly(scenarioItems,
                Path.GetFullPath(AppSettings.GetApplicationDataPath() + "ScriptSource.cs"));
            
        }

        /// <summary>
        /// Recompile scripts if scripts were changed
        /// </summary>
        /// <param name="actualScriptsVersion">actual script version</param>
        public void RecompileIfNeed(int actualScriptsVersion)
        {
            lock (syncObject)
            {
                if (actualScriptsVersion > scriptsVersion)
                {
                    GetLogger().Info("Scripts version have been changed from " +
                        scriptsVersion + " to " + actualScriptsVersion + ".");
                    ConstructExecutor(ServiceDAO.GetScenarios());
                    scriptsVersion = actualScriptsVersion;
                }
            }
        }
        
        /// <summary>
        /// Execute script scenario
        /// </summary>
        /// <param name="id">scenario id</param>
        /// <param name="server">server that implements scenario operations</param>
        /// <returns></returns>
        public bool Execute(int id, IScriptingServer server)
        {
            object executorObject = compiledAssembly.CreateInstance("Utils.CompileScripts._Executor");
            ((ScriptingBase)executorObject).Initialize(server);
            MethodInfo mi = executorObject.GetType().GetMethod("GetScenarioDelegate" + id);
            ScriptingBase.ScenarioDelegate executor = (ScriptingBase.ScenarioDelegate) mi.Invoke(executorObject, null);

            try
            {
                executor();
            }
            catch (ScriptingException e)
            {
                server.SetFailureReason(e.Message);
                return false;
            }
            
            return true;
        }

        private static ILog GetLogger()
        {
            return LogManager.GetLogger(AppSettings.GetCommonLoggerName());
        }
        
    }
}
