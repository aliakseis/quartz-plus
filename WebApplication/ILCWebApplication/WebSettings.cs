using System.Configuration;
using Utils;

namespace ILCWebApplication
{
    public static class WebSettings
    {
        /// <summary>
        /// Gets connection string
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetConnectionString()
        {
            return Connect.DecryptMaster(
                ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Gets service location
        /// </summary>
        /// <returns>service location</returns>
        public static string GetServiceLocation()
        {
            return ConfigurationManager.AppSettings["ILC_ServiceLocation"];
        }

        /// <summary>
        /// Gets service name
        /// </summary>
        /// <returns>service name</returns>
        public static string GetServiceName()
        {
            return ConfigurationManager.AppSettings["ILC_ServiceName"];
        }

        /// <summary>
        /// Gets common logger name
        /// </summary>
        /// <returns>common logger name</returns>
        public static string GetCommonLoggerName()
        {
            return "IlcWebAppLog";
        }

        /// <summary>
        /// Gets DB schema name
        /// </summary>
        /// <returns>DB schema name</returns>
        public static string GetSchemaName()
        {
            return "ILC";
        } 
    }
}