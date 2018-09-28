using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace ILC_ControlPanel
{
    static class DatabaseChecker
    {
        private const string TEST_QUERY_BY_COLUMN_NAME = "select count(1) from ALL_TAB_COLUMNS where OWNER='{0}' AND TABLE_NAME='{1}' AND COLUMN_NAME='{2}'";

        /// <summary>
        /// database connection test status
        /// </summary>
        public enum Status
        {
            Compatible = 0, // Connection succeeded, version is compatible
            Incompatible = 1, // Connection succeeded, version is not compatible
            QueryError = 2, // Connection succeeded, unexpected error during querying
            ConnectionProblem = 3, // Connection failed
        };

        /// <summary>
        /// Tests database connection and database compatibility
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <returns>status of the test</returns>
        public static Status TestConnection(string connectionString)
        {
            try
            {
                using (DbConnection testConnection = new OracleConnection(connectionString))
                {
                    testConnection.Open();

                    return TestByColumnName(testConnection, Program.SchemaName, "ILC_INSTANCE", "ILC_SCENARIO_ID");
                }
            }
            catch (Exception)
            {
                return Status.ConnectionProblem;
            }
        }

        private static Status TestByColumnName(DbConnection testConnection, string shemaName, string tableName, string columnName)
        {
            try
            {
                using (DbCommand cmd = testConnection.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format(TEST_QUERY_BY_COLUMN_NAME, shemaName, tableName, columnName);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count == 0 ? Status.Incompatible : Status.Compatible;
                }
            }
            catch (Exception)
            {
                return Status.QueryError;
            }
        }

    }
}
