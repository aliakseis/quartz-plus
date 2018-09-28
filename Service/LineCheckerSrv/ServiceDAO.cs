using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Configuration;
using Utils.CompileScripts;

namespace LineCheckerSrv
{
    /// <summary>
    /// Implements service configutation related database operations
    /// </summary>
    static class ServiceDAO
    {
       
        /// <summary>
        /// Gets ILC configuration settings from the database.
        /// </summary>
        /// <returns>container for ILC configuration settings</returns>
        public static IDictionary GetIlcConfigurationSettings()
        {
            Hashtable map = new Hashtable();
            using (DbConnection connection = new OracleConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT NAME, VALUE FROM " +
                                      AppSettings.GetSchemaName() + ".RUNTIME_CONFIGURATION";
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["NAME"] as string;
                            string value = reader["VALUE"] as string;

                            ConfigurationManager.AppSettings[name] = value;
                        }
                    }
                }

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT OUT_CHANNEL_NUM, TERMINATOR_KEY, TIMESPAN,COMMON_RECIPIENT_EMAIL," +
                                      "TIME_BETWEEN_VERIFICATION, MAX_ATTEMPTS, FROM_EMAIL, SILENCE_TIMEOUT," +
                                      "RECORD_DURATION, LOGIN_SILENCE_TIMEOUT, SUMMARY_REPORT_CRON," +
                                      "SUMMARY_RECIPIENT_EMAIL, JOB_MISFIRE_THRESHOLD, VERSION," +
                                      "SCRIPTS_VERSION FROM " + AppSettings.GetSchemaName() + ".ILC_INSTANCE";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            map[AppSettings.OutgoingChannelsKey] = reader[0] as decimal?;
                            map[AppSettings.TerminatorKeyKey] = reader[1] as string;
                            map[AppSettings.TimeSpanKey] = reader[2] as decimal?;
                            map[AppSettings.CommonRecipientEmailAddressesKey] = reader[3] as string;
                            map[AppSettings.TimeSpanBetweenVerificationsKey] = reader[4] as decimal?;
                            map[AppSettings.AttemptsMaxNumberKey] = reader[5] as decimal?;
                            map[AppSettings.EmailFromAddressKey] = reader[6] as string;
                            map[AppSettings.SilenceTimeoutKey] = reader[7] as decimal?;
                            map[AppSettings.RecordDurationKey] = reader[8] as decimal?;
                            map[AppSettings.LoginSilenceTimeoutKey] = reader[9] as decimal?;
                            map[AppSettings.SummaryReportCronKey] = reader[10] as string;
                            map[AppSettings.SummaryRecipientEmailKey] = reader[11] as string;
                            map[AppSettings.JobMisfireThresholdKey] = reader[12] as decimal?;
                            map[AppSettings.SettingsVersion] = reader[13] as decimal?;
                            map[AppSettings.ScriptsVersion] = reader[14] as decimal?;
                        }
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// Gets list of project IDs and cron expression strings assigned to them.
        /// </summary>
        /// <returns>list of project IDs and cron expression strings assigned to them</returns>
        public static IList<KeyValuePair<string, int>> GetCronStringsByProjectIds()
        {
            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            using (DbConnection connection = new OracleConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT DISTINCT SERVICE_INFO.IVR_PROJECT_ID," +
                                        "NVL(NVL(IVR_PROJECT.SCHEDULE_CRON,IVR_SERVER.SCHEDULE_CRON)," +
                                        "(SELECT SCHEDULE_CRON FROM " + AppSettings.GetSchemaName() +
                                        ".ILC_INSTANCE)) FROM " + AppSettings.GetSchemaName() + ".SERVICE_INFO INNER JOIN " +
                                        AppSettings.GetSchemaName() + ".IVR_PROJECT ON " +
                                        "SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID AND " +
                                        "IVR_PROJECT.ENABLED != 0 INNER JOIN " + AppSettings.GetSchemaName() +
                                        ".IVR_SERVER ON IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID AND " +
                                        "IVR_SERVER.ENABLED != 0 WHERE SERVICE_INFO.ENABLED != 0";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new KeyValuePair<string, int>(reader[1] as string, Convert.ToInt32(reader[0])));
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets version of service settings
        /// </summary>
        /// <returns>version of service settings that increments on each settings update</returns>
        public static uint GetSettingsVersion()
        {
            string strQuery = "SELECT VERSION FROM " + AppSettings.GetSchemaName() + ".ILC_INSTANCE";

            using (DbConnection connection = new OracleConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;

                    return Convert.ToUInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Gets scenarios
        /// </summary>
        /// <returns>scenarios list</returns>
        public static IList<ScenarioItem> GetScenarios()
        {
            List<ScenarioItem> list = new List<ScenarioItem>();

            using (DbConnection connection = new OracleConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT ILC_SCENARIO_ID, NAME, ASSEMBLIES, SCRIPTING_EXPRESSION FROM " +
                        AppSettings.GetSchemaName() + ".ILC_SCENARIO";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ScenarioItem item = new ScenarioItem();
                            item.id = Convert.ToInt32(reader[0]);
                            item.name = reader[1] as string;
                            item.assemblies = reader[2] as string;
                            item.scriptingExpression = reader[3] as string;

                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Fixes ILC state inconsistency after improper service termination etc.
        /// </summary>
        public static void ResetIlcState()
        {
            using (DbConnection connection = new OracleConnection(AppSettings.GetConnectionString()))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE " + AppSettings.GetSchemaName() +
                                    ".SERVICE_VERIFICATION_SESSION SET IS_REPORTED=1";

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
