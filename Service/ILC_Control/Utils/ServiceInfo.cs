using System;
using System.Configuration;
using System.Management;
using Microsoft.Win32;

namespace ILC_ControlPanel.Utils
{
    /// <summary>
    /// Class implements functionality for getting service information
    /// </summary>
    static class ServiceInfo
    {
        /// <summary>
        /// Gets service startup mode
        /// </summary>
        /// <returns>service startup mode</returns>
        public static string GetSeviceStartupMode()
        {
            string filter = String.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", Program.ServerName);
            using (ManagementObjectSearcher query = new ManagementObjectSearcher(filter))
            {
                using (ManagementObjectCollection services = query.Get())
                {
                    foreach (ManagementObject service in services)
                    {
                        Object obj = service.GetPropertyValue("startmode");
                        if (obj != null)
                            return obj.ToString();
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Gets service path
        /// </summary>
        /// <returns>service path</returns>
        public static string GetServicePath()
        {
            string path = "";
            try
            {
                RegistryKey hklmService = Registry.LocalMachine.OpenSubKey(
                    "System\\CurrentControlSet\\Services\\" + Program.serviceController.ServiceName);

                if (hklmService == null)
                {
                    Program.ExitController(Program.serviceController.ServiceName + " service registration is absent.");
                    return path;
                }

                path = (string)hklmService.GetValue("ImagePath");
                path = path.Replace("\"", "");
            }
            catch (Exception ex)
            {
                Program.ExitController(ex.Message);
            }

            return path;
        }

        /// <summary>
        /// Gets configuration that represents service configuration file 
        /// </summary>
        /// <param name="path">service path</param>
        /// <returns>service configuration</returns>
        public static Configuration GetServiceConfig(string path)
        {
            Configuration config = null;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(path);
            }
            catch (Exception ex)
            {
                Program.ExitController(ex.Message);
            }

            if (config != null && !config.HasFile)
                Program.ExitController("Configuration file " + config.FilePath + " is missing");

            return config;
        }
    }
}
