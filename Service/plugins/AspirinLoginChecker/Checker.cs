using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using Common.Logging;
using Utils;

namespace AspirinLoginChecker
{
    public class Checker
    {
        #region error messages

        private static readonly string[] authErrors =
            {
                "User account issue",
                "SQL / Network / Unknown error",
                "User, Site or Project marked as disabled",
                "User is locked",
                "Either UserId or Pin entered is empty",
                "No active projects for this user",
                "No project prompt method for the usercat",
                "Fatal: Could not create account on the Oracle database",
                "Project name for the Croner not found",
                "Fatal: Could not open User Connection",
                "Fatal: Some runtime error",
                "Unknown error (-11)",
                "Creation of a COM Object failed",
                "Unknown error (-13)",
                "Croner account must not be used interactively",
            };

        #endregion

        /// <summary> 
        /// does authentication checking
        /// </summary>
        /// <param name="dbConn">DB connection string</param>
        /// <param name="login">user login</param>
        /// <param name="beforeLoginTime">time before password sending attempt was made</param>
        /// <param name="log">logger reference</param>
        /// <param name="failureReason">reason of failed authentication</param>
        /// <returns>true if authentication was successfull</returns>
        public static bool DoCheck(string dbConn, string login, DateTime beforeLoginTime, ILog log, out string failureReason)
        {
            log.Info("AspirinLoginChecker: start check.");
            failureReason = "User authentication was not detected";
            int success;
            if (GetAuthLoginStatus(out success, dbConn, login, beforeLoginTime, log))
            {
                if (success != 2)
                {
                    failureReason = authErrors[-success];
                }
                else
                {
                    failureReason = "";
                    return true;
                }
            }
            return false;
        }


        private static bool GetAuthLoginStatus(out int success, string dbConn, string login, DateTime beforeLoginTime, ILog log)
        {
            DateTime? currentTime;
            DateTime? loginTime;
            DateTime? hangUpTime;
            if (CheckAuthLogin(
                    dbConn, login,
                    out success, out currentTime, out loginTime, out hangUpTime))
            {
                log.Info("AspirinLoginChecker: login=" + login + ", success=" + success + ", currentTime=" + currentTime +
                         ", loginTime=" + loginTime + ", hangUpTime=" + hangUpTime);
                return loginTime != null
                       && currentTime - loginTime <= DateTime.Now.AddSeconds(1) - beforeLoginTime;
            }

            return false;
        }

        /// <summary>
        /// Checks authentication login
        /// </summary>
        /// <param name="authLoginDbConn">AuthLogin DB connection string</param>
        /// <param name="userId">user id</param>
        /// <param name="success">success(error) value</param>
        /// <param name="currentTime">Database current time (SYSDATE)</param>
        /// <param name="loginTime">login time</param>
        /// <param name="hangUpTime">hangup time</param>
        /// <returns>true if checks</returns>
        private static bool CheckAuthLogin(
            string authLoginDbConn,
            string userId,
            out int success,
            out DateTime? currentTime,
            out DateTime? loginTime,
            out DateTime? hangUpTime
            )
        {
            if (string.IsNullOrEmpty(authLoginDbConn))
                authLoginDbConn = ConfigurationManager.ConnectionStrings["Cron"].ConnectionString;

            using (DbConnection conn = new OracleConnection(Connect.DecryptMaster(authLoginDbConn)))
            {
                conn.Open();
                return CheckAuthLogin(conn, userId, out success, out currentTime, out loginTime, out hangUpTime);
            }
        }

        private static bool CheckAuthLogin(
            DbConnection conn,
            string userId,
            out int success,
            out DateTime? currentTime,
            out DateTime? loginTime,
            out DateTime? hangUpTime
            )
        {
            success = 0;
            currentTime = null;
            loginTime = null;
            hangUpTime = null;

            bool retVal = false;

            const string strQuery = "SELECT SUCCESS, SYSDATE, LOGINTIME, HANGUPTIME FROM ASPIRIN.AUTHLOGIN " +
                                    "WHERE USERID = :UserId AND RECID = (SELECT MAX(RECID) FROM ASPIRIN.AUTHLOGIN  WHERE USERID = :UserId)";

            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strQuery;
                DaoHelper.AddInputParameter(cmd, "UserId", userId);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        success = Convert.ToInt32(reader[0]);
                        currentTime = reader[1] as DateTime?;
                        loginTime = reader[2] as DateTime?;
                        hangUpTime = reader[3] as DateTime?;

                        retVal = true;
                    }
                }
            }

            return retVal;
        }
    }
}
