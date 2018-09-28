using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.AccessControl;
using System.IO;
using Microsoft.Win32;

namespace ILCWebApplication
{
    /// <summary>
    /// Provides the foundation for custom installations
    /// </summary>
    [RunInstaller(true)]
    public partial class CustomInstaller : Installer
    {
        public CustomInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When installing set the permissions on the App_Data directory
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            string[] directoryNames;

            try
            {
                directoryNames = new string[]
                {
                    Path.Combine(Context.Parameters["TheInstallFolder"], "App_Data"),
                    Environment.GetEnvironmentVariable("TEMP"),
                    GetSystemTempDirectory()
                };
            }
            catch (Exception)
            {
                //TODO log these issues
                return;
            }

            SetAccessRights(directoryNames, Environment.MachineName + @"\ASPNET");
            SetAccessRights(directoryNames, @"NT AUTHORITY\NETWORK SERVICE");
        }

        private static void SetAccessRights(IEnumerable<string> directoryNames, string account)
        {
            foreach (string directoryName in directoryNames)
            {
                try
                {
                    // Add the access control entry to the directory.
                    AddDirectorySecurity(directoryName, account, FileSystemRights.Modify, AccessControlType.Allow);
                    AddDirectorySecurity(directoryName, account, FileSystemRights.Write, AccessControlType.Allow);
                    RemoveDirectorySecurity(directoryName, account, FileSystemRights.Delete, AccessControlType.Deny);
                }
                catch (Exception)
                {

                }
            }
        }

        public static void Main()
        {

        }


        /// <summary>
        /// Adds an ACL entry on the specified directory for the specified account.
        /// </summary>
        /// <param name="folderName">Folder path to add the permissions to</param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        private static void AddDirectorySecurity(string folderName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(folderName);

            // Get a DirectorySecurity object that represents the current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, controlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);

        }

        /// <summary>
        /// Removes an ACL entry on the specified directory for the specified account.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="account"></param>
        /// <param name="rights"></param>
        /// <param name="controlType"></param>
        public static void RemoveDirectorySecurity(string folderName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            // Create a new DirectoryInfo object.
            DirectoryInfo dInfo = new DirectoryInfo(folderName);

            // Get a DirectorySecurity object that represents the current security settings.
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Add the FileSystemAccessRule to the security settings. 
            dSecurity.RemoveAccessRule(new FileSystemAccessRule(account, rights, controlType));

            // Set the new access settings.
            dInfo.SetAccessControl(dSecurity);
        }

        private static string GetSystemTempDirectory()
        {
            string path = "";
            try
            {
                RegistryKey hklmEnvironment = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment");

                if (hklmEnvironment == null)
                {
                    return "";
                }

                path = (string)hklmEnvironment.GetValue("TEMP");
                path = path.Replace("\"", "");
            }
            catch (Exception ex)
            {
                return "";
            }

            return path;
        }
    }
}
