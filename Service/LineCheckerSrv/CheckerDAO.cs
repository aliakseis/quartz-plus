using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Text;
using Utils;

namespace LineCheckerSrv
{
    /// <summary>
    /// Implements database operations
    /// </summary>
    static internal class CheckerDAO
    {
        static private readonly string schemaName = AppSettings.GetSchemaName();

        /// <summary>
        /// Adds input parameter to DbCommand
        /// </summary>
        /// <param name="cmd">DbCommand instance</param>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        private static void AddInputParameter(DbCommand cmd, string name, object value)
        {
            DaoHelper.AddInputParameter(cmd, name, value);
        }

        private static DbConnection GetConnection()
        {
            return new OracleConnection(AppSettings.GetConnectionString());
        }

        /// <summary>
        /// Initializes session
        /// </summary>
        /// <param name="lineCheckerJob">LineCheckerJob instance</param>
        /// <param name="wasMisfired">Was session misfired</param>
        /// <param name="prjIds">Collection of project ids</param>
        /// <returns>true if session was initialized</returns>
        public static bool InitializeSession(LineCheckerJob lineCheckerJob, bool wasMisfired, ICollection<int> prjIds)
        {
            StringBuilder projectIds = new StringBuilder(",");
            foreach (int id in prjIds)
            {
                projectIds.Append(id).Append(',');
            }

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT " + schemaName + ".SQ_WORKER_ID.NEXTVAL FROM DUAL";
                    lineCheckerJob.WorkerName = "WORKER" + cmd.ExecuteScalar();
                }

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = schemaName + ".INITIALIZE_CHECKLIST";
                    cmd.CommandType = CommandType.StoredProcedure;

                    AddInputParameter(cmd, "TIME_SPAN", lineCheckerJob.GetTimeSpanSinceLastVerificationInDays());
                    AddInputParameter(cmd, "WAS_MISFIRED", wasMisfired);
                    AddInputParameter(cmd, "PROJECTS_LIST", projectIds.ToString());

                    DbParameter outputparam = cmd.CreateParameter();
                    outputparam.Direction = ParameterDirection.Output;
                    outputparam.ParameterName = "A_REPORT_ID";
                    outputparam.DbType = DbType.Int32;
                    cmd.Parameters.Add(outputparam);

                    DbParameter outputparam2 = cmd.CreateParameter();
                    outputparam2.Direction = ParameterDirection.Output;
                    outputparam2.ParameterName = "A_START_TIME";
                    outputparam2.DbType = DbType.DateTime;
                    cmd.Parameters.Add(outputparam2);

                    DbParameter outputparam3 = cmd.CreateParameter();
                    outputparam3.Direction = ParameterDirection.Output;
                    outputparam3.ParameterName = "PREV_START_TIME";
                    outputparam3.DbType = DbType.DateTime;
                    cmd.Parameters.Add(outputparam3);

                    DbParameter param2 = cmd.CreateParameter();
                    param2.Direction = ParameterDirection.Output;
                    param2.DbType = DbType.Int32;
                    param2.ParameterName = "IS_NEW_SESSION";
                    cmd.Parameters.Add(param2);

                    cmd.ExecuteNonQuery();

                    lineCheckerJob.SessionId = (int) outputparam.Value;
                    lineCheckerJob.StartTime = (DateTime) outputparam2.Value;
                    lineCheckerJob.PreviousStartTime = outputparam3.Value as DateTime?;
                    return (int) param2.Value != 0;
                }
            }
        }

        /// <summary>
        /// Gets data for checking
        /// </summary>
        /// <param name="lineCheckerJob">LineCheckerJob instance</param>
        /// <param name="actualScriptsVersion">scripts version</param>
        /// <param name="rowId">row id</param>
        /// <returns>true if data for checking was retrieved</returns>
        public static bool GetCheckData(LineCheckerJob lineCheckerJob, out int actualScriptsVersion, out object rowId)
        {
            rowId = null;
            bool retVal = false;
            actualScriptsVersion = 0;

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                string strQuery =
                    "SELECT SERVICE_VERIFICATION.ROWID, TNUMBER, USERID, PWD, SERVICE_VERIFICATION.ITEM_ID, IVR_PROJECT.NAME, IVR_SERVER.NAME, DB_CONN, ATTEMPTS, IVR_SERVER.AUTH_CHECKER, " +
                    "NVL(SERVICE_INFO.ILC_SCENARIO_ID, NVL(IVR_PROJECT.ILC_SCENARIO_ID, NVL(IVR_SERVER.ILC_SCENARIO_ID, (SELECT ILC_SCENARIO_ID FROM " +
                    schemaName + ".ILC_INSTANCE)))), (SELECT SCRIPTS_VERSION FROM " +
                    schemaName + ".ILC_INSTANCE)" +
                    " FROM " + schemaName + ".SERVICE_VERIFICATION INNER JOIN " + schemaName +
                    ".SERVICE_INFO ON SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID INNER JOIN " +
                    schemaName + ".IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID " +
                    "INNER JOIN " + schemaName +
                    ".IVR_SERVER ON IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID " +
                    "WHERE SERVICE_VERIFICATION.ROWID=" + schemaName + ".GET_WORKER_ID(:WORKER_NAME, :A_REPORT_ID, :MAX_ATTEMPTS, :TIME_SPAN)";

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = strQuery;

                    AddInputParameter(cmd, "WORKER_NAME", lineCheckerJob.WorkerName);
                    AddInputParameter(cmd, "A_REPORT_ID", lineCheckerJob.SessionId);
                    AddInputParameter(cmd, "MAX_ATTEMPTS", AppSettings.GetAttemptsMaxNumber());
                    AddInputParameter(cmd, "TIME_SPAN", AppSettings.GetTimeSpanInDays());

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            rowId = reader[0];
                            lineCheckerJob.Phone = reader[1] as string;
                            lineCheckerJob.Login = reader[2] as string;
                            lineCheckerJob.Password = reader[3] as string;
                            lineCheckerJob.ItemId = Convert.ToInt32(reader[4]);
                            lineCheckerJob.ProjectName = reader[5] as string;
                            lineCheckerJob.ServerName = reader[6] as string;
                            lineCheckerJob.AuthLoginDbConn = reader[7] as string;
                            lineCheckerJob.VerificationAttempts = Convert.ToInt32(reader[8]);
                            // Implement Authentication Checking by plug-ins
                            lineCheckerJob.AuthCheckerName = reader[9] as string;

                            lineCheckerJob.ScenarioId = Convert.ToInt32(reader[10]);

                            actualScriptsVersion = Convert.ToInt32(reader[11]);

                            retVal = true;
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Detects if session need be continued
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <returns>true if session need be continued</returns>
        public static bool DoWeNeedToContinue(int sessionId)
        {
            string strQuery = string.Format(
                "SELECT COUNT(1) FROM " + schemaName + ".SERVICE_VERIFICATION " +
                "WHERE REPORT_ID={0} AND TIME > SYSDATE - 10/1440 " + // 10 minutes frame to avoid infinite loops on unexpected runtime errors
                "AND ((STATUS='AWAITING' AND ATTEMPTS < {1}) OR STATUS LIKE 'WORKER%') AND ROWNUM = 1",
                sessionId, AppSettings.GetAttemptsMaxNumber());

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Checks are there are records with STATUS = "WORKER"
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <returns>true if there are records with STATUS = "WORKER"</returns>
        public static bool DoWeHaveWorkers(int sessionId)
        {
            string strQuery = string.Format("SELECT COUNT(1) FROM " + schemaName + ".SERVICE_VERIFICATION " +
                                            "WHERE REPORT_ID={0} AND STATUS LIKE 'WORKER%'",
                                            sessionId);

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Gets data for reporting
        /// </summary>
        /// <returns>collection of report items</returns>
        /// <param name="sessionId"> session id</param>
        public static IList<ReportItem> GetReportData(int sessionId)
        {
            string strQuery = string.Format(
                "SELECT SERVICE_VERIFICATION.STATUS, SERVICE_VERIFICATION.REASON," +
                "SERVICE_VERIFICATION.ATTEMPTS, SERVICE_VERIFICATION.TIME, SERVICE_INFO.TNUMBER," +
                "SERVICE_INFO.USERID, IVR_PROJECT.NAME AS PROJECT_NAME, IVR_SERVER.NAME AS SERVER_NAME " +
                "FROM " + schemaName + ".SERVICE_VERIFICATION INNER JOIN " + schemaName +
                ".SERVICE_INFO ON SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID INNER JOIN " +
                schemaName + ".IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID " +
                "INNER JOIN " + schemaName + ".IVR_SERVER ON IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID " +
                "WHERE SERVICE_VERIFICATION.REPORT_ID = {0} ORDER BY SERVICE_VERIFICATION.TIME",
                sessionId);

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        IList<ReportItem> reportData = new List<ReportItem>();
                        while (reader.Read())
                        {
                            ReportItem item = new ReportItem();
                            item.status = reader["STATUS"] as string;
                            item.reason = reader["REASON"] as string;
                            item.attemps = Convert.ToInt32(reader["ATTEMPTS"]);
                            item.verificationTime = (DateTime) reader["TIME"];
                            item.phone = reader["TNUMBER"] as string;
                            item.userId = reader["USERID"] as string;
                            item.projectName = reader["PROJECT_NAME"] as string;
                            item.serverName = reader["SERVER_NAME"] as string;

                            reportData.Add(item);
                        }
                        return reportData;
                    }
                }
            }
        }

        /// <summary>
        /// Gets data for summary report
        /// </summary>
        /// <returns>collection of report items</returns>
        /// <param name="startDate">start date for getting report data</param>
        /// <param name="endDate">finish date for getting report data</param>
        public static IList<SummaryReportItem> GetSummaryReportData(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string strQuery = "SELECT IVR_PROJECT.NAME," +
                              "IVR_SERVER.NAME, SERVICE_INFO.TNUMBER, SERVICE_INFO.USERID," +
                              "COUNT(1), SUM (DECODE(STATUS, 'SUCCEEDED', 0, 1)) AS FAILED_CNT," +
                              "MAX (DECODE (STATUS, 'SUCCEEDED', TIME, TO_DATE(NULL)))," +
                              "MAX (DECODE (STATUS, 'SUCCEEDED', TO_DATE(NULL), TIME)) FROM " +
                              schemaName + ".SERVICE_VERIFICATION INNER JOIN " + schemaName + ".SERVICE_INFO ON " +
                              "SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID INNER JOIN " + schemaName +
                              ".IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID " +
                              "INNER JOIN " + schemaName + ".IVR_SERVER ON " +
                              "IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID " +
                              "WHERE TIME >= :startDate AND TIME < :endDate " +
                              "GROUP BY IVR_PROJECT.NAME, IVR_SERVER.NAME, TNUMBER, USERID " +
                              "ORDER BY FAILED_CNT DESC";

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;
                    AddInputParameter(cmd, "startDate", startDate);
                    AddInputParameter(cmd, "endDate", endDate);

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
        /// Saves call result
        /// </summary>
        /// <param name="callResult">call result</param>
        /// <param name="failureReason">failure reason</param>
        /// <param name="rowId">row id</param>
        public static void SaveCallResults(string callResult, string failureReason, object rowId)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = schemaName + ".SET_RESULT_STATUS";
                    cmd.CommandType = CommandType.StoredProcedure;

                    AddInputParameter(cmd, "RESULT_STATUS", callResult);
                    AddInputParameter(cmd, "RESULT_REASON",
                                      failureReason.Substring(0, Math.Min(failureReason.Length, 254)));
                    AddInputParameter(cmd, "A_ROW_ID", rowId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets report flag
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <returns>report flag value</returns>
        public static int GetReportFlag(int sessionId)
        {
            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = schemaName + ".GET_GENERATE_REPORT_FLAG";
                    cmd.CommandType = CommandType.StoredProcedure;

                    AddInputParameter(cmd, "A_REPORT_ID", sessionId);

                    DbParameter outputparam = cmd.CreateParameter();
                    outputparam.Direction = ParameterDirection.Output;
                    outputparam.ParameterName = "GENERATE_REPORT_FLAG";
                    outputparam.DbType = DbType.Int32;
                    cmd.Parameters.Add(outputparam);

                    cmd.ExecuteNonQuery();
                    return (int) outputparam.Value;
                }
            }
        }

        /// <summary>
        /// Gets email addresses
        /// </summary>
        /// <param name="rowId">row id</param>
        /// <returns>string of email addresses</returns>
        public static string GetEmailAddresses(object rowId)
        {
            string strQuery = "SELECT EMAIL_ADDRESSES FROM " + schemaName +
                              ".IVR_PROJECT INNER JOIN " + schemaName +
                              ".SERVICE_INFO ON SERVICE_INFO.IVR_PROJECT_ID=IVR_PROJECT.IVR_PROJECT_ID " +
                              "INNER JOIN " + schemaName + ".SERVICE_VERIFICATION ON " +
                              "SERVICE_INFO.ITEM_ID=SERVICE_VERIFICATION.ITEM_ID " +
                              "WHERE " + schemaName + ".SERVICE_VERIFICATION.ROWID = :rowIdParam";

            using (DbConnection connection = GetConnection())
            {
                connection.Open();

                using (DbCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strQuery;
                    AddInputParameter(cmd, "rowIdParam", rowId);

                    return (cmd.ExecuteScalar() as string);
                }
            }
        }
    }
}
