using System;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Threading;
using ILC_ControlPanel.Properties;
using ILC_ControlPanel.Utils;
using Utils;
using System.Collections.Generic;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Form for displaying service parameters
    /// </summary>
    public partial class ControlPanelScreen : Form
    {
        DateTime lastCheckDate;
        private bool isLastConnectionFailed;
        private bool isSizeCounted;
        /// <summary>
        /// Balloon text
        /// </summary>

        private static ImageList imageList;
        readonly Object callMonitor = new Object();        

        private bool cancelTabSelecting = true;

        /// <summary>
        /// Constructor
        /// </summary>
        public ControlPanelScreen()
        {
            InitializeComponent();
            MinimumSize = Size;

            Text += " v." + Application.ProductVersion;
            
            tbDataSource.TextChanged += ControlChanged;
            tbUserId.TextChanged += ControlChanged;
            tbPassword.TextChanged += ControlChanged;
            tbEmailServer.TextChanged += ControlChanged;
            tbEmailLogin.TextChanged += ControlChanged;
            tbEmailPassword.TextChanged += ControlChanged;
            tbAppDataPath.TextChanged += ControlChanged;
            checkBox3.CheckStateChanged += ControlChanged; 
        }

        /// <summary>
        /// Set button states(enabled, disabled)
        /// </summary>
        public void SetButtonStates()
        {
            if (!Visible)
                return;
        
            if (InvokeRequired)
            {
                Invoke((SetButtonStatesCallback) SetButtonStates);
            }
            else
            {
                ServiceControllerStatus serviceStatus = Program.serviceController.Status;

                lblCurrentStatus.Text = serviceStatus.ToString();
                lblStartupMode.Text = ServiceInfo.GetSeviceStartupMode();
                lblTimeStarted.Text = Program.GetServiceStartedTime();

                LoadSettings();
                SetStateAndLastCheck();

                btnQuickCheck.Enabled = ((serviceStatus == ServiceControllerStatus.Running) && lblCurrentState.Text == "Idle");

                if (imageList != null)
                {
                    switch(serviceStatus)
                    {
                        case ServiceControllerStatus.Running:
                            pbStatus.Image = imageList.Images[1];
                            btnStartStop.Text = "Stop";
                            btnStartStop.Enabled = true;
                            break;
                        case ServiceControllerStatus.Stopped:
                            pbStatus.Image = imageList.Images[0];
                            btnStartStop.Text = "Start";
                            btnStartStop.Enabled = true;
                            break;
                        default:
                            pbStatus.Image = imageList.Images[2];
                            btnStartStop.Text = "Paused";
                            btnStartStop.Enabled = false;
                            break;
                    }
                }
            }
        }

        private void ControlPanelScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }

        private void SetStateAndLastCheck()
        {
            ServiceActivity activity = ActivityDAO.GetActivityData();
            if (!activity.IsSucceeded)
            {
                lblLastCheck.Text = "n/a";
                lblCurrentState.Text = "n/a";
                
                progressBar1.ProgressText = "";
                progressBar1.Value = 0;
                if (!isLastConnectionFailed)
                {
                    isLastConnectionFailed = true;
                    Program.ShowMessageBox("Unable to establish DB connection. Check Database settings.");
                    listView1.Items[2].Selected = true;
                    listView1.Items[2].Focused = true;
                    listView1.Select();
                }

                return;
            }

            isLastConnectionFailed = false;
            lblLastCheck.Text = activity.LastCheck.ToString(@"M/d/yyyy hh:mm:ss tt");
            lastCheckDate = activity.LastCheck;

            bool bWorkingState = (activity.WorkingState &&
                                  (lblCurrentStatus.Text == ServiceControllerStatus.Running.ToString()));
            lblCurrentState.Text = bWorkingState ? "Working" : "Idle";

            if (bWorkingState)
            {
                if (progressBar1.Maximum != activity.TotalProgress)
                {
                    progressBar1.Maximum = activity.TotalProgress;
                    progressBar1.Value = 0;
                }

                progressBar1.ProgressText = activity.PassedProgress + " out of " + activity.TotalProgress + "...";
                progressBar1.Value = activity.PassedProgress;
            }
            else
            {
                progressBar1.ProgressText = "";
                progressBar1.Value = 0;
            }
        }
        
        private void LoadSettings()
        {
            // initialize all tab once
            if (!isTabsInitialized)
            {
                InitializeTabs();
                btnApply.Enabled = false;
                isTabsInitialized = true;
            }

            ShowDiskInfo();
        }

        void ShowDiskInfo()
        {
            // run async calculation of application use space
            if (deleteFiles_Button.Enabled && !bgwSpaceCounter.IsBusy)
            {
                // calculate occupied space if service is started or if space is not calculated yet
                if ((Program.serviceController.Status == ServiceControllerStatus.Running) 
                    || (!isSizeCounted))
                {
                    bgwSpaceCounter.RunWorkerAsync(workingPath);
                }
            }

            // get local drive free space info
            string info1;
            string info2;
            DiskInfo.GetDiskSize(workingPath, out info1, out info2);
            lblFreeBytes.Text = info1;
            lblFreeGbytes.Text = info2;
        }

        private void ControlPanelScreen_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                SetButtonStates();
            }
        }

        #region Nested type: SetButtonStatesCallback

        private delegate void SetButtonStatesCallback();
        private delegate void AddCallStatusCallback(string callStatus);

        #endregion

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            OracleConnectionStringBuilder csb = new OracleConnectionStringBuilder();
            csb.DataSource = tbDataSource.Text;
            csb.UserID = tbUserId.Text;
            csb.Password = tbPassword.Text;

            DatabaseChecker.Status status = DatabaseChecker.TestConnection(Connect.DecryptMaster(csb.ConnectionString));

            switch(status)
            {
                case DatabaseChecker.Status.Compatible:
                    MessageBox.Show("Database connection was established.\nDatabase compatibility test was successful.", Application.ProductName, MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                    break;
                case DatabaseChecker.Status.Incompatible:
                    Program.ShowMessageBox("Database connection was established.\nDatabase compatibility test has failed!");
                    break;
                case DatabaseChecker.Status.QueryError:
                    Program.ShowMessageBox("Database connection was established.\nUnexpected error during database compatibility test!");
                    break;
                case DatabaseChecker.Status.ConnectionProblem:
                    Program.ShowMessageBox("Unable to establish database connection!");
                    break;
                default:
                    Program.ShowMessageBox("Unexpected error during database testing!");
                    break;
            }

        }

        private void btnQuickCheck_Click(object sender, EventArgs e)
        {
            // run quick check
            try
            {
                Program.serviceController.ExecuteCommand((int)ServiceCustomCommand.StartCheck);
            }
            catch (Exception)
            {
            }
        }

        private void ControlPanelScreen_Load(object sender, EventArgs e)
        {
            // initialize image list
            imageList = new ImageList();

            imageList.TransparentColor = Color.White;

            imageList.Images.Add(Resources.StoppedStatus);
            imageList.Images.Add(Resources.StartedStatus);
            imageList.Images.Add(Resources.PendingStatus);

            deletePeriodComboBox.SelectedIndex = 0;

            listView1.Items[0].Selected = true;
            listView1.Select();
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            ServiceOperation operation = (btnStartStop.Text == "Start") ? ServiceOperation.ServiceStart : ServiceOperation.ServiceStop;
            ServiceProgressForm progress = new ServiceProgressForm(operation);
            progress.ShowDialog(this);
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            // email server name validation
            if (SmtpEmailValidator.IsValidHost(tbEmailServer.Text))
                MessageBox.Show("Valid SMTP server", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                Program.ShowMessageBox("Invalid SMTP server");

        }

        void ControlChanged(object sender, EventArgs e)
        {
            // enable Apply button 
            btnApply.Enabled = true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            // save changes
            SaveTabs();
            btnApply.Enabled = false;
            // recalculate space next time
            isSizeCounted = false;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // dismiss changes and hide form
            InitializeTabs();
            btnApply.Enabled = false;

            Hide();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            // save changes and hide form
            if (btnApply.Enabled)
            {
                SaveTabs();
                btnApply.Enabled = false;
                // recalculate space next time
                isSizeCounted = false;

            }
            Hide();
        }


        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            // open log file 
            try
            {
                Process.Start(tbLoggingPath.Text);
            }
            catch (Exception ex)
            {
                Program.ShowMessageBox("Can't open log file " + tbLoggingPath.Text + ". Reason: " + ex.Message);
            }
        }

        private void btnSelectAppPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            try
            {
                DirectoryInfo di = new DirectoryInfo(tbAppDataPath.Text);
                dlg.SelectedPath = di.FullName;
            }
            catch (System.Exception)
            {
                // there is an invalid data in the path text box. 
                // Default SelectedPath will be used.
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbAppDataPath.Text = dlg.SelectedPath;
            }
        }

        private void OnDelete_Click(object sender, EventArgs e)
        {
            if (DialogResult.No == MessageBox.Show("Are you sure you want to delete these files?",
                Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                return;

            int days = 0;
            switch (deletePeriodComboBox.Text)
            {
                case "Everything":
                    days = -1;
                    break;
                case "Last day":
                    days = 1;
                    break;
                case "Last Week":
                    days = 7;
                    break;
            }

            // Get extensions of files to be deleted
            List<string> extensions = new List<string>();
            if (audioCheckBox.Checked)
                extensions.Add("wav");
            if (logCheckBox.Checked)
                extensions.Add("log");

            DateTime finishDate;
            if (lblCurrentState.Text == "Idle")
                finishDate = DateTime.Now.AddSeconds(-Program.UPDATE_TIMER_SECONDS);
            else
            {
                finishDate = DateTime.Now.AddMinutes(-1);
                if (lastCheckDate < finishDate)
                    finishDate = lastCheckDate;
            }

            SetEnabledLocalFilesTab(false);

            // recalculate space next time
            isSizeCounted = false;

            try
            {
                FilesCleaner.Clean(workingPath, extensions, days, finishDate, deleteCompleted);
            }
            catch (Exception ex)
            {
                Program.ShowMessageBox(ex.Message);
                SetEnabledLocalFilesTab(true);
            }
        }

        public void deleteCompleted(object sender, CompletedEventArgs obj)
        {
            if (InvokeRequired)
            {
                Invoke((CompletedEventHandler) deleteCompleted, sender, obj);
            }
            else
            {
                SetEnabledLocalFilesTab(true);
            }
        }

        void SetEnabledLocalFilesTab(bool isEnabled)
        {
            tbAppDataPath.Enabled = btnSelectAppPath.Enabled = deletePeriodComboBox.Enabled =
                deleteFiles_Button.Enabled = audioCheckBox.Enabled = logCheckBox.Enabled = isEnabled;
        }

        private void deleteFileCheckChanged(object sender, EventArgs e)
        {
            deleteFiles_Button.Enabled = audioCheckBox.Checked || logCheckBox.Checked;
        }

        private void bgwSpaceCounter_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                Program.ShowMessageBox(e.Error.Message);
            }
            else
            {
                // Finally, handle the case where the operation succeeded.
                long usedByApp = Convert.ToInt64(e.Result);
                lblUsedBytes.Text = usedByApp.ToString("###,###,###,###,###,##0") + " bytes";
                lblUsedGbytes.Text = DiskInfo.ConvertBytes(usedByApp);
                isSizeCounted = true;
            }
        }

        private void bgwSpaceCounter_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //e.Result = DiskInfo.GetDirSizeByMonteCarlo((string)e.Argument);
                e.Result = DiskInfo.GetDirSizeFast((string)e.Argument);
            }
            catch (Exception)
            {
                e.Result = 0;
            }
        }

        private void checkBox3_Click(object sender, EventArgs e)
        {
            tbEmailLogin.Enabled = checkBox3.Checked;
            tbEmailPassword.Enabled = checkBox3.Checked;
        }

        private void dgwLogging_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ControlChanged(sender, e);
        }
       
        private void callButton_Click(object sender, EventArgs e)
        {
            if (callButton.Text == "Call")
            {
                int lineDevice;
                if (devicesListBox.CheckedIndices.Count == 1)
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

                callButton.Text = "Disconnect";
                bgwCall.RunWorkerAsync(lineDevice);
            }
            else
            {
                lock (callMonitor)
                {
                    // TODO
                }
            }
        }

        private void bgwCall_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            lock (callMonitor)
            {
                try
                {
                    // TODO

                    AddCallStatus("");
                }

                catch (Exception ex)
                {
                    // TODO

                    Program.ShowMessageBox(ex.Message);
                }

            }
        }


        
        private void bgwCall_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // TODO
            
            callButton.Text = "Call";
        }

        private void cleanButton_Click(object sender, EventArgs e)
        {
            callStatusListBox.Items.Clear();
        }

        private void AddCallStatus(string callStatus)
        {
            if (InvokeRequired)
            {
                Invoke((AddCallStatusCallback) AddCallStatus, callStatus);
            }
            else
            {
                callStatusListBox.Items.Add(callStatus);
                callStatusListBox.SelectedIndex = callStatusListBox.Items.Count - 1;
            }
        }

        private void devicesListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ControlChanged(sender, e); 
        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            lock (callMonitor)
            {
                // TODO
            }                
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            cancelTabSelecting = false;            
            tabControl1.SelectTab(e.ItemIndex);
            lblConfigHeader.Visible = tbConfigFile.Visible = (e.ItemIndex != 0);
            cancelTabSelecting = true;
            
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = cancelTabSelecting;
        }
    }    
}
