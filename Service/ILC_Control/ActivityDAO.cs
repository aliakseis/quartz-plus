using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using ILC_ControlPanel.Utils;
using Utils;

namespace ILC_ControlPanel
{
    internal class ActivityDAO
    {
        public static ServiceActivity GetActivityData()
        {
            string path = ServiceInfo.GetServicePath();
            Configuration config = ServiceInfo.GetServiceConfig(path);
            ConnectionStringsSection css = config.ConnectionStrings;

            ConnectionStringSettings conStr = css.ConnectionStrings["Cron"];
            if (conStr != null)
                return GetActivityData(conStr.ConnectionString);
            return new ServiceActivity();
        }

        private static ServiceActivity GetActivityData(string connStr)
        {
            ServiceActivity activity = new ServiceActivity();
            try
            {
                using (DbConnection connection = new OracleConnection(Connect.DecryptMaster(connStr)))
                {
                    connection.Open();

                    const string strQuery =
                        "WITH STAT AS (SELECT FINISHED, COUNT(1) AS CNT FROM (SELECT  DECODE(STATUS, 'SUCCEEDED', 1, 'FAILED', 1, 'AWAITING', DECODE(ATTEMPTS, (SELECT MAX_ATTEMPTS FROM " +
                        Program.SchemaName + ".ILC_INSTANCE), 1, 0), 0) AS FINISHED FROM " + Program.SchemaName + ".SERVICE_VERIFICATION INNER JOIN " +
                        Program.SchemaName + ".SERVICE_VERIFICATION_SESSION ON SERVICE_VERIFICATION.REPORT_ID=SERVICE_VERIFICATION_SESSION.REPORT_ID AND SERVICE_VERIFICATION_SESSION.IS_REPORTED=0) S GROUP BY FINISHED) " +
                        "SELECT START_TIME, IS_REPORTED, NVL((SELECT CNT FROM STAT WHERE FINISHED=0), 0) AS REMAINING, NVL((SELECT CNT FROM STAT WHERE FINISHED=1), 0) AS FINISHED FROM " +
                        Program.SchemaName + ".SERVICE_VERIFICATION_SESSION";

                    using (DbCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = strQuery;
                        
                        int remained = 0;

                        using (DbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                activity.LastCheck = (DateTime) reader[0];
                                activity.WorkingState = Convert.ToInt32(reader[1]) == 0;
                                remained = Convert.ToInt32(reader[2]);
                                activity.PassedProgress = Convert.ToInt32(reader[3]);

                                activity.IsSucceeded = true;
                            }
                        }

                        activity.TotalProgress = remained + activity.PassedProgress;
                    }
                }
            }
            catch (Exception)
            {
                activity.IsSucceeded = false;
            }

            return activity;
        }
    }
}