using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Security.Principal;
using System.Text;
using ILCWebApplication.ILCSettings;
using Utils;

namespace ILCWebApplication
{
    /// <summary>
    /// Implements database operations
    /// </summary>
    public static class IlcWebDao
    {
        private static readonly string schemaName = WebSettings.GetSchemaName();

        private static void AddInputParameter(DbCommand cmd, string name, object value)
        {
            DaoHelper.AddInputParameter(cmd, name, value);
        }

        /// <summary>
        /// Gets list of projects
        /// </summary>
        /// <param name="serverId">server id</param>
        /// <returns>projects list</returns>
        public static IList<KeyValuePair<string, string>> GetProjectsList(string serverId)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT IVR_PROJECT_ID, NAME FROM " + schemaName +
                        ".IVR_PROJECT WHERE IVR_SERVER_ID =:PARAM_IVR_SERVER_ID";
                    AddInputParameter(cmd, "PARAM_IVR_SERVER_ID", serverId);

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new KeyValuePair<string, string>(Convert.ToString(reader[0]), reader[1] as string));
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets list of servers
        /// </summary>
        /// <returns>servers list</returns>
        public static IList<KeyValuePair<string, string>> GetServersList()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT IVR_SERVER_ID, NAME FROM " + schemaName + ".IVR_SERVER";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new KeyValuePair<string, string>(Convert.ToString(reader[0]), reader[1] as string));
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Check whether user exists
        /// </summary>
        /// <param name="strName">user name</param>
        /// <param name="strPassword">user password</param>
        /// <returns>true if user exists</returns>
        public static bool IsUserExist(string strName, string strPassword)
        {
            bool userExists = false;

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT * FROM ASPIRIN.AUTHUSERS WHERE USERID =:PARAM_USERID  AND PASSWORD =:PARAM_PASSWORD";
                    AddInputParameter(cmd, "PARAM_USERID", strName);
                    AddInputParameter(cmd, "PARAM_PASSWORD", Connect.XCode(strPassword, true));

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            userExists = true;
                    }
                }
            }
            return userExists;
        }

        
        /// <summary>
        /// Gets ILC settings data
        /// </summary>
        /// <returns>IlcSettingsData object</returns>
        public static IlcSettingsData GetIlcSettingsData()
        {
            IlcSettingsData data = new IlcSettingsData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT SCHEDULE_CRON,OUT_CHANNEL_NUM, TIMESPAN,COMMON_RECIPIENT_EMAIL," +
                        "TIME_BETWEEN_VERIFICATION, MAX_ATTEMPTS, FROM_EMAIL," +
                        "SUMMARY_REPORT_CRON,SUMMARY_RECIPIENT_EMAIL, JOB_MISFIRE_THRESHOLD FROM " +
                        schemaName + ".ILC_INSTANCE";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.scheduleCron = Convert.ToString(reader[0]);
                            data.outgoingChannels = Convert.ToString(reader[1]);
                            data.timeSpan = Convert.ToString(reader[2]);
                            data.commonRecipientEmailAddresses = Convert.ToString(reader[3]);
                            data.timeSpanBetweenVerifications = Convert.ToString(reader[4]);
                            data.attemptsMaxNumber = Convert.ToString(reader[5]);
                            data.emailFromAddress = Convert.ToString(reader[6]);
                            data.summaryReportCron = Convert.ToString(reader[7]);
                            data.summaryRecipientEmail = Convert.ToString(reader[8]);
                            data.jobMisfireThreshold = Convert.ToString(reader[9]);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Update ILC settings
        /// </summary>
        /// <param name="data">IlcSettingsData object</param>
        /// <param name="user">principal object interface</param>
        public static void UpdateIlcSettings(IlcSettingsData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "UPDATE " + schemaName + ".ILC_INSTANCE SET " +
                        "SCHEDULE_CRON=:PARAM_SCHEDULE_CRON, OUT_CHANNEL_NUM=:PARAM_OUT_CHANNEL_NUM," +
                        "TIMESPAN=:PARAM_TIMESPAN, COMMON_RECIPIENT_EMAIL=:PARAM_COMMON_RECIPIENT_EMAIL, TIME_BETWEEN_VERIFICATION=:PARAM_TIME_BETWEEN_VERIF," +
                        "MAX_ATTEMPTS=:PARAM_MAX_ATTEMPTS, FROM_EMAIL=:PARAM_FROM_EMAIL," +
                        "SUMMARY_REPORT_CRON=:PARAM_SUMMARY_REPORT_CRON,SUMMARY_RECIPIENT_EMAIL=:PARAM_SUMMARY_RECIPIENT_EMAIL," +
                        "JOB_MISFIRE_THRESHOLD=:PARAM_JOB_MISFIRE_THRESHOLD, VERSION=VERSION+1";

                    AddInputParameter(cmd, "PARAM_SCHEDULE_CRON", data.scheduleCron);
                    AddInputParameter(cmd, "PARAM_OUT_CHANNEL_NUM", data.outgoingChannels);
                    AddInputParameter(cmd, "PARAM_TIMESPAN", data.timeSpan);
                    AddInputParameter(cmd, "PARAM_COMMON_RECIPIENT_EMAIL", data.commonRecipientEmailAddresses);
                    AddInputParameter(cmd, "PARAM_TIME_BETWEEN_VERIF", data.timeSpanBetweenVerifications);
                    AddInputParameter(cmd, "PARAM_MAX_ATTEMPTS", data.attemptsMaxNumber);
                    AddInputParameter(cmd, "PARAM_FROM_EMAIL", data.emailFromAddress);
                    AddInputParameter(cmd, "PARAM_SUMMARY_REPORT_CRON", data.summaryReportCron);
                    AddInputParameter(cmd, "PARAM_SUMMARY_RECIPIENT_EMAIL", data.summaryRecipientEmail);
                    AddInputParameter(cmd, "PARAM_JOB_MISFIRE_THRESHOLD", data.jobMisfireThreshold);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static DbConnection GetConnection(IPrincipal user)
        {
            DbConnection connection = new OracleConnection(WebSettings.GetConnectionString());
            try
            {
                connection.Open();
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = schemaName + ".SET_ILC_CTX";
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddInputParameter(cmd, "p_ilc_user", user.Identity.Name);
                    cmd.ExecuteNonQuery();
                }
                return connection;
            }
            catch (Exception)
            {
                connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets item data
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>DetailItemData object</returns>
        public static DetailItemData GetDetailItemData(string id)
        {
            DetailItemData data = new DetailItemData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT ITEM_ID, TNUMBER, IVR_PROJECT_ID, USERID, PWD, ENABLED FROM " +
                        schemaName + ".SERVICE_INFO WHERE ITEM_ID=:PARAM_ITEM_ID";
                    AddInputParameter(cmd, "PARAM_ITEM_ID", id);

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.itemId = Convert.ToString(reader[0]);
                            data.phoneNumber = Convert.ToString(reader[1]);
                            data.projectId = Convert.ToString(reader[2]);
                            data.login = Convert.ToString(reader[3]);
                            data.password = Convert.ToString(reader[4]);
                            data.enabled = Convert.ToString(reader[5]);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets data for summary report
        /// </summary>
        /// <returns>collection of report items</returns>
        /// <param name="startDate">Reporting period start date and time</param>
        /// <param name="endDate">Reporting period end date and time</param>
        /// <param name="queryParams">report query parameters</param>
        public static IList<SummaryReportItem> GetSummaryReportData(
            DateTime startDate,
            DateTime endDate,
            List<KeyValuePair<string, object>> queryParams)
        {
            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;

                    AddInputParameter(cmd, "startDate", startDate);
                    AddInputParameter(cmd, "endDate", endDate);

                    StringBuilder condition = new StringBuilder();

                    int paramCounter = 0;
                    foreach(KeyValuePair<string, object> kvp in queryParams)
                    {
                        paramCounter++;
                        condition.Append(" AND ").Append(kvp.Key).Append(" = :").Append(paramCounter);
                        AddInputParameter(cmd, paramCounter.ToString(), kvp.Value);
                    }

                    cmd.CommandText = "SELECT IVR_PROJECT.NAME," +
                                  "IVR_SERVER.NAME, SERVICE_INFO.TNUMBER, SERVICE_INFO.USERID," +
                                  "COUNT(1), SUM (DECODE(STATUS, 'SUCCEEDED', 0, 1)) AS FAILED_CNT," +
                                  "MAX (DECODE (STATUS, 'SUCCEEDED', TIME, TO_DATE(NULL)))," +
                                  "MAX (DECODE (STATUS, 'SUCCEEDED', TO_DATE(NULL), TIME)) FROM " +
                                  schemaName + ".SERVICE_VERIFICATION INNER JOIN " + schemaName + ".SERVICE_INFO ON " +
                                  "SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID INNER JOIN " + schemaName +
                                  ".IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID " +
                                  "INNER JOIN " + schemaName + ".IVR_SERVER ON " +
                                  "IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID " +
                                  "WHERE TIME >= :startDate AND TIME < :endDate" +
                                  condition +
                                  " GROUP BY IVR_PROJECT.NAME, IVR_SERVER.NAME, TNUMBER, USERID " +
                                  "ORDER BY FAILED_CNT DESC";

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        IList<SummaryReportItem> reportData = new List<SummaryReportItem>();
                        while (reader.Read())
                        {
                            SummaryReportItem item = new SummaryReportItem();
                            item.projectName = reader[0] as string;
                            item.serverName = reader[1] as string;
                            item.phone = reader[2] as string;
                            item.userId = reader[3] as string;
                            item.checksCount = Convert.ToInt32(reader[4]);
                            item.failedChecksCount = Convert.ToInt32(reader[5]);
                            item.lastSuccessDate = reader[6] as DateTime?;
                            item.lastFailedDate = reader[7] as DateTime?;

                            reportData.Add(item);
                        }
                        return reportData;
                    }
                }
            }
        }

        /// <summary>
        /// Updates item data
        /// </summary>
        /// <param name="data">DetailItemData object</param>
        /// <param name="user">principal object interface</param>
        public static void UpdateDetailItemData(DetailItemData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "UPDATE " + schemaName + ".SERVICE_INFO SET TNUMBER=:PHONE, USERID=:LOGIN," +
                        "PWD=:PASSWORD, ENABLED=:ENABLED WHERE ITEM_ID=:ITEM_ID";

                    AddInputParameter(cmd, "PHONE", data.phoneNumber);
                    AddInputParameter(cmd, "LOGIN", data.login);
                    AddInputParameter(cmd, "PASSWORD", data.password);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "ITEM_ID", data.itemId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts item data
        /// </summary>
        /// <param name="data">DetailItemData object</param>
        /// <param name="user">principal object interface</param>
        /// <returns>inserted item id</returns>
        public static int InsertDetailItemData(DetailItemData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    AddInputParameter(cmd, "PHONE", data.phoneNumber);
                    AddInputParameter(cmd, "LOGIN", data.login);
                    AddInputParameter(cmd, "PASSWORD", data.password);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "PROJECT_ID", data.projectId);

                    return ExecuteAndHandleNewId(cmd,
                        "INSERT INTO " + schemaName + ".SERVICE_INFO (TNUMBER, USERID, PWD, ENABLED," +
                        "IVR_PROJECT_ID) VALUES(:PHONE, :LOGIN, :PASSWORD, :ENABLED, :PROJECT_ID)");
                }
            }
        }


        /// <summary>
        /// Gets project data
        /// </summary>
        /// <param name="id">project id</param>
        /// <returns>DetailProjectData object</returns>
        public static DetailProjectData GetDetailProjectData(string id)
        {
            DetailProjectData data = new DetailProjectData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT IVR_PROJECT_ID, IVR_SERVER_ID, NAME, EMAIL_ADDRESSES, ENABLED," +
                        "SCHEDULE_CRON FROM " + schemaName +
                        ".IVR_PROJECT WHERE IVR_PROJECT_ID =:PARAM_IVR_PROJECT_ID";
                    AddInputParameter(cmd, "PARAM_IVR_PROJECT_ID", id);

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.projectId = Convert.ToString(reader[0]);
                            data.serverId = Convert.ToString(reader[1]);
                            data.name = Convert.ToString(reader[2]);
                            data.emails = Convert.ToString(reader[3]);
                            data.enabled = Convert.ToString(reader[4]);
                            data.cron = Convert.ToString(reader[5]);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Updates project data
        /// </summary>
        /// <param name="data">DetailProjectData object</param>
        /// <param name="user">principal object interface</param>
        public static void UpdateDetailProjectData(DetailProjectData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "UPDATE " + schemaName + ".IVR_PROJECT " +
                        "SET IVR_SERVER_ID=:SERVER_ID, NAME=:NAME, EMAIL_ADDRESSES=:EMAILS," +
                        "ENABLED=:ENABLED, SCHEDULE_CRON=:CRON WHERE IVR_PROJECT_ID = :PROJECT_ID";

                    AddInputParameter(cmd, "SERVER_ID", data.serverId);
                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "EMAILS", data.emails);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "CRON", data.cron);
                    AddInputParameter(cmd, "PROJECT_ID", data.projectId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts project data
        /// </summary>
        /// <param name="data">DetailProjectData object</param>
        /// <param name="user">principal object interface</param>
        /// <returns>inserted project id</returns>
        public static int InsertDetailProjectData(DetailProjectData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "EMAILS", data.emails);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "CRON", data.cron);
                    AddInputParameter(cmd, "SERVER_ID", data.serverId);

                    return ExecuteAndHandleNewId(cmd,
                        "INSERT INTO " + schemaName + ".IVR_PROJECT (NAME, EMAIL_ADDRESSES, ENABLED," +
                        "SCHEDULE_CRON, IVR_SERVER_ID) VALUES (:NAME, :EMAILS, :ENABLED, :CRON, :SERVER_ID)");
                }
            }
        }

        /// <summary>
        /// Gets server data
        /// </summary>
        /// <param name="id">server id</param>
        /// <returns>DetailServerData object</returns>
        public static DetailServerData GetDetailServerData(string id)
        {
            DetailServerData data = new DetailServerData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT IVR_SERVER_ID, NAME, NUM_CHANNELS, DB_CONN, ENABLED, AUTH_CHECKER," +
                        "SCHEDULE_CRON FROM " + schemaName + ".IVR_SERVER WHERE IVR_SERVER_ID =:PARAM_SERVER_ID";
                    AddInputParameter(cmd, "PARAM_SERVER_ID", id);

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.serverId = Convert.ToString(reader[0]);
                            data.name = Convert.ToString(reader[1]);
                            data.channels = Convert.ToString(reader[2]);
                            data.connectionString = Convert.ToString(reader[3]);
                            data.enabled = Convert.ToString(reader[4]);
                            data.checker = Convert.ToString(reader[5]);
                            data.cron = Convert.ToString(reader[6]);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets script data
        /// </summary>
        /// <param name="id">script id</param>
        /// <returns>DetailScriptData object</returns>
        public static DetailScriptData GetDetailScriptData(string id)
        {
            DetailScriptData data = new DetailScriptData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT ILC_SCENARIO_ID, NAME, SCRIPTING_EXPRESSION FROM " +
                        schemaName + ".ILC_SCENARIO WHERE ILC_SCENARIO_ID =:PARAM_SCENARIO_ID";
                    AddInputParameter(cmd, "PARAM_SCENARIO_ID", id);

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.scriptId = Convert.ToString(reader[0]);
                            data.name = Convert.ToString(reader[1]);
                            data.scriptingExspression = Convert.ToString(reader[2]);
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Updates server data
        /// </summary>
        /// <param name="data">DetailServerData object</param>
        /// <param name="user">principal object interface</param>
        public static void UpdateDetailServerData(DetailServerData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "UPDATE " + schemaName + ".IVR_SERVER SET NAME=:NAME, NUM_CHANNELS=:CHANNELS, DB_CONN=:CONN," +
                        "ENABLED=:ENABLED, AUTH_CHECKER=:CHECKER, SCHEDULE_CRON=:CRON " +
                        "WHERE IVR_SERVER_ID = :SERVER_ID";

                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "CHANNELS", data.channels);
                    AddInputParameter(cmd, "CONN", data.connectionString);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "CHECKER", data.checker);
                    AddInputParameter(cmd, "CRON", data.cron);
                    AddInputParameter(cmd, "SERVER_ID", data.serverId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Updates script data
        /// </summary>
        /// <param name="data">DetailScriptData object</param>
        /// <param name="user">principal object interface</param>
        public static void UpdateDetailScriptData(DetailScriptData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "UPDATE " + schemaName + ".ILC_SCENARIO SET NAME=:NAME, SCRIPTING_EXPRESSION=:EXPRESSION " +
                        "WHERE ILC_SCENARIO_ID = :SCENARIO_ID";

                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "EXPRESSION", data.scriptingExspression);
                    AddInputParameter(cmd, "SCENARIO_ID", data.scriptId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Inserts server data
        /// </summary>
        /// <param name="data">DetailServerData object</param>
        /// <param name="user">principal object interface</param>
        /// <returns>inserted server id</returns>
        public static int InsertDetailServerData(DetailServerData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "CHANNELS", data.channels);
                    AddInputParameter(cmd, "CONN", data.connectionString);
                    AddInputParameter(cmd, "ENABLED", data.enabled);
                    AddInputParameter(cmd, "CHECKER", data.checker);
                    AddInputParameter(cmd, "CRON", data.cron);

                    return ExecuteAndHandleNewId(cmd,
                        "INSERT INTO " + schemaName + ".IVR_SERVER(NAME, NUM_CHANNELS, DB_CONN, ENABLED," +
                        "AUTH_CHECKER,SCHEDULE_CRON) VALUES(:NAME, :CHANNELS, :CONN, :ENABLED, :CHECKER, :CRON)");
                }
            }
        }

        /// <summary>
        /// Inserts script data
        /// </summary>
        /// <param name="data">DetailScriptData object</param>
        /// <param name="user">principal object interface</param>
        /// <returns>inserted script id</returns>
        public static int InsertDetailScriptData(DetailScriptData data, IPrincipal user)
        {
            using (DbConnection connection = GetConnection(user))
            {
                using (DbCommand cmd = connection.CreateCommand())
                {
                    AddInputParameter(cmd, "NAME", data.name);
                    AddInputParameter(cmd, "EXPRESSION", data.scriptingExspression);

                    return ExecuteAndHandleNewId(cmd,
                        "INSERT INTO " + schemaName + ".ILC_SCENARIO(NAME, SCRIPTING_EXPRESSION) VALUES(:NAME, :EXPRESSION)");
                }
            }
        }

        private static int ExecuteAndHandleNewId(DbCommand cmd, string commandText)
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "BEGIN " + commandText + "; SELECT " + schemaName + ".SQ_DATA.CURRVAL INTO :NEW_ID FROM DUAL; END;";

            DbParameter outputparam = cmd.CreateParameter();
            outputparam.Direction = ParameterDirection.Output;
            outputparam.ParameterName = "NEW_ID";
            outputparam.DbType = DbType.Int32;
            cmd.Parameters.Add(outputparam);

            cmd.ExecuteNonQuery();

            return (int) outputparam.Value;
        }

        /// <summary>
        /// Gets ILC status data
        /// </summary>
        /// <returns>IlcStatusData object</returns>
        public static IlcStatusData GetStatusData()
        {
            IlcStatusData data = new IlcStatusData();

            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = 
                        "WITH STAT AS (SELECT FINISHED, COUNT(1) AS CNT FROM (SELECT  DECODE(STATUS, 'SUCCEEDED', 1, 'FAILED', 1, 'AWAITING', DECODE(ATTEMPTS, (SELECT MAX_ATTEMPTS FROM " + 
                        schemaName + ".ILC_INSTANCE), 1, 0), 0) AS FINISHED FROM " + schemaName + ".SERVICE_VERIFICATION INNER JOIN " +
                        schemaName + ".SERVICE_VERIFICATION_SESSION ON SERVICE_VERIFICATION.REPORT_ID=SERVICE_VERIFICATION_SESSION.REPORT_ID AND SERVICE_VERIFICATION_SESSION.IS_REPORTED=0) S GROUP BY FINISHED) " +
                        "SELECT START_TIME, IS_REPORTED, NVL((SELECT CNT FROM STAT WHERE FINISHED=0), 0) AS REMAINING, NVL((SELECT CNT FROM STAT WHERE FINISHED=1), 0) AS FINISHED FROM " + 
                        schemaName + ".SERVICE_VERIFICATION_SESSION";
                    
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.lastCheckDate = (DateTime)reader[0];
                            int remained = Convert.ToInt32(reader[2]);
                            int passed = Convert.ToInt32(reader[3]);
                            data.currServiceState = (Convert.ToInt32(reader[1]) == 0) ? "Working: " + passed + " out of " + (passed + remained) + "..." 
                                                                                      : ConstExpressions.WORK_STATUS_IDLE;
                        }
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Check whether script name exist
        /// </summary>
        /// <param name="name">script name</param>
        /// <param name="id">script id</param>
        /// <returns>true if name exists</returns>
        public static bool IsScriptNameExist(string name, string id)
        {
            using (DbConnection connection = new OracleConnection(WebSettings.GetConnectionString()))
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT NAME FROM " + schemaName + ".ILC_SCENARIO " +
                        "WHERE UPPER(NAME) = :NAME AND " + schemaName + ".ILC_SCENARIO.ILC_SCENARIO_ID != :SCENARIO_ID";
                    AddInputParameter(cmd, "NAME", name.ToUpper());
                    AddInputParameter(cmd, "SCENARIO_ID", id);

                    return cmd.ExecuteScalar() != null;
                }
            }
        }

    }
}