using System.Data;
using System.Data.Common;

namespace Utils
{
    public class DaoHelper
    {
        /// <summary>
        /// Adds input parameter to command
        /// </summary>
        /// <param name="cmd">command</param>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        public static void AddInputParameter(DbCommand cmd, string name, object value)
        {
            DbParameter param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
