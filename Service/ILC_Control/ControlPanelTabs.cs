using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.InteropServices;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Form for displaying service parameters
    /// </summary>
    public partial class ControlPanelScreen
    {
        private bool isTabsInitialized = false;

        private void InitializeTabs()
        {
            string path = Utils.ServiceInfo.GetServicePath();
            Configuration config = Utils.ServiceInfo.GetServiceConfig(path);
            InitializeDataBaseTab(config);
            InitializeSmtpTab(config);
            InitializeLocalFilesTab(config);
            InitializePhoneTab(config);
            InitializeLoggingTab(path);

            lblServiceVersion.Text = "IVRS Line Checker v." + FileVersionInfo.GetVersionInfo(path).ProductVersion;
            tbConfigFile.Text = path + ".config";

            // tooltip to show too long strings
            toolTipConfigFile.SetToolTip(tbConfigFile, tbConfigFile.Text);
            toolTipLogFile.SetToolTip(tbLoggingPath, tbLoggingPath.Text);
        }

        private void SaveTabs()
        {
            try
            {
                string path = Utils.ServiceInfo.GetServicePath();
                Configuration config = Utils.ServiceInfo.GetServiceConfig(path);

                SaveDataBaseTab(config);
                SaveSmtpTab(config);
                SaveLocalFilesTab(config);
                SavePhoneTab(config);

                config.Save();

                SaveLoggingTab(path);
            }
            catch(Exception e)
            {
                Program.ShowMessageBox("Can't save settings." + e.Message);
            }
            // reinitialize control panel after saving
            InitializeTabs();
        }

        #region Database tab

        private void InitializeDataBaseTab(Configuration config)
        {
            ConnectionStringsSection css = config.ConnectionStrings;
            if (css != null)
            {
                ConnectionStringSettings conStr = css.ConnectionStrings["Cron"];
                if (conStr != null)
                {
                    OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder(conStr.ConnectionString);

                    tbConnectionString.Text = csb.ConnectionString;
                    tbDataSource.Text = csb.DataSource;
                    tbUserId.Text = csb.UserID;
                    tbPassword.Text = csb.Password;
                }
                else
                {
                    tbConnectionString.Text = "";
                    tbDataSource.Text = "";
                    tbUserId.Text = "";
                    tbPassword.Text = "";
                }
            }

        }

        private void SaveDataBaseTab(Configuration config)
        {
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder();

            csb.DataSource = tbDataSource.Text;
            csb.UserID = tbUserId.Text;
            csb.Password = tbPassword.Text;

            if(config.ConnectionStrings.ConnectionStrings["Cron"] == null)
            {
                config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("Cron",csb.ConnectionString));
            }
            else
                config.ConnectionStrings.ConnectionStrings["Cron"].ConnectionString = csb.ConnectionString;
        }

        #endregion

        #region SMTP tab

        private void InitializeSmtpTab(Configuration config)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings["EmailServerName"];
            tbEmailServer.Text = element != null ? element.Value : "";

            element = config.AppSettings.Settings["EmailClientLogin"];
            tbEmailLogin.Text = element != null ? element.Value : "";

            element = config.AppSettings.Settings["EmailClientPassword"];
            tbEmailPassword.Text = element != null ? element.Value : "";
            
            checkBox3.Checked = !(string.IsNullOrEmpty(tbEmailLogin.Text) &&
                                    string.IsNullOrEmpty(tbEmailPassword.Text));
            tbEmailLogin.Enabled = checkBox3.Checked;
            tbEmailPassword.Enabled = checkBox3.Checked;
        }

        private void SaveSmtpTab(Configuration config)
        {
            if (config.AppSettings.Settings["EmailServerName"] == null)
                config.AppSettings.Settings.Add("EmailServerName", tbEmailServer.Text);
            else
                config.AppSettings.Settings["EmailServerName"].Value = tbEmailServer.Text;

            if (!checkBox3.Checked)
            {
                tbEmailLogin.Text = "";
                tbEmailPassword.Text = "";
            }

            if (config.AppSettings.Settings["EmailClientLogin"] == null)
                config.AppSettings.Settings.Add("EmailClientLogin", tbEmailLogin.Text);
            else
                config.AppSettings.Settings["EmailClientLogin"].Value = tbEmailLogin.Text;

            if (config.AppSettings.Settings["EmailClientPassword"] == null)
                config.AppSettings.Settings.Add("EmailClientPassword", tbEmailPassword.Text);
            else
                config.AppSettings.Settings["EmailClientPassword"].Value = tbEmailPassword.Text;
        }

        #endregion

        #region Local Files tab

        // real path to application outputs
        private string workingPath;

        private void InitializeLocalFilesTab(Configuration config)
        {

            if (config.AppSettings.Settings["ApplicationDataPath"] == null)
                workingPath = tbAppDataPath.Text = "";
            else
                workingPath = tbAppDataPath.Text = config.AppSettings.Settings["ApplicationDataPath"].Value;
           
            ShowDiskInfo();
        }

        private void SaveLocalFilesTab(Configuration config)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(tbAppDataPath.Text);
            if(!dirInfo.Exists)
            {
                try
                {
                    dirInfo.Create();
                }
                catch (IOException e)
                {
                    Program.ShowMessageBox("Can't save Application data path. " + e.Message);
                    return;
                }

            }

            if (config.AppSettings.Settings["ApplicationDataPath"] == null)
                config.AppSettings.Settings.Add("ApplicationDataPath", tbAppDataPath.Text);
            else
                config.AppSettings.Settings["ApplicationDataPath"].Value = tbAppDataPath.Text;
        }

        #endregion

        #region Logging tab

        private List<LogSetting> logSettings = new List<LogSetting>();

        private void InitializeLoggingTab(string path)
        {
            XPathDocument doc = new XPathDocument(path + ".config");
            XPathNavigator nav = doc.CreateNavigator();
            XPathNodeIterator iterator = nav.Select("//log4net/appender");
            // use first appender
            iterator.MoveNext();
            XPathNavigator nodesNavigator = iterator.Current;
            if (iterator.Current.Name != "appender")
            {
                tbLoggingPath.Text = "log4net/appender section is invalid or missing in the config file.";
                return;
            }
                

            iterator = nodesNavigator.SelectChildren(XPathNodeType.Element);

            logSettings.Clear();
            // iterates throw appender elements
            while (iterator.MoveNext())
            {
                if (iterator.Current.Name == "file")
                {
                    tbLoggingPath.Text = Path.GetDirectoryName(path) + "\\" +
                                                               iterator.Current.GetAttribute("value", String.Empty);
                }
                // we don't need to display layout element
                if (iterator.Current.Name == "layout")
                    continue;

                logSettings.Add(new LogSetting(iterator.Current.Name, iterator.Current.GetAttribute("value", String.Empty)));
            }

            dgwLogging.DataSource = logSettings;
        }

        private void SaveLoggingTab(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path + ".config");
            XmlNode appenderNode = doc.SelectSingleNode("//log4net/appender");
            foreach(LogSetting setting in logSettings)
            {
                XmlNode node = appenderNode.SelectSingleNode(setting.Name);
                if(node != null)
                {
                    XmlAttribute attr = doc.CreateAttribute("value");
                    attr.Value = setting.Val;
                    node.Attributes.Append(attr);
                }
            }
            doc.Save(path + ".config");
        }

        #endregion

        #region Phone tab

        private void InitializePhoneTab(Configuration config)
        {
            //uint num;
            //for (uint index = 1; index <= num; index++)
            //{
            //    devicesListBox.Items.Add(strBuilder + " Serial#" + info.SerialNumber);
            //}

            string strLineDevice = "";
            if (config.AppSettings.Settings["LineDevice"] != null)
                strLineDevice = config.AppSettings.Settings["LineDevice"].Value;

            int lineDevice = string.IsNullOrEmpty(strLineDevice)
                            ? -1
                            : Convert.ToInt32(strLineDevice);
            if (lineDevice == -1)
            {
                for(int i = 0; i < devicesListBox.Items.Count; i++)
                {
                    devicesListBox.SetItemChecked(i, true);
                }
            }
            else if (lineDevice < devicesListBox.Items.Count)
            {
                devicesListBox.SetItemChecked(lineDevice, true);
            }
        }
        
        private void SavePhoneTab(Configuration config)
        {
            int lineDevice;
            if(devicesListBox.CheckedIndices.Count == 1)
            {
                lineDevice = devicesListBox.CheckedIndices[0];
            }
            else if (devicesListBox.CheckedIndices.Count == devicesListBox.Items.Count)
            {
                lineDevice = -1;
            }
            else
            {
                Program.ShowMessageBox("One or all telephone adapters must be selected");
                return;
            }

            if (config.AppSettings.Settings["LineDevice"] == null)
                config.AppSettings.Settings.Add("LineDevice", lineDevice.ToString());
            else
                config.AppSettings.Settings["LineDevice"].Value = lineDevice.ToString();
        }

        #endregion
    }
}
