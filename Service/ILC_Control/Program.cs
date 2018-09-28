using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ILC_ControlPanel.Properties;
using Timer=System.Timers.Timer;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Class for running service control in system tray. Creates icon, control panel screen, adds context menu
    /// </summary>
    internal static class Program
    {
        public static readonly string ServerName = "IVRSLineChecker";
        public const string SchemaName = "ILC";
        private static readonly string guidForMutex = "A584D582-5FE0-4444-B764-B7423939C5C9";
        public const int UPDATE_TIMER_SECONDS = 2;
        private static Point balloonPosition;
        private static Timer balloonTimer;
        private static ContextMenuStrip contextMenuStrip;

        private static ControlPanelScreen ctrlPanel;
        private static ImageList imageList;
        private static volatile bool isBalloonShown;
        private static Mutex mutex;
        private static NotifyIcon notifyIcon;
        /// <summary>
        /// Represents a Windows service and allows you to connect to a running or stopped service, 
        /// manipulate it, or get information about it.
        /// </summary>
        public static ServiceController serviceController;
        private static Timer timer;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            bool bNew;
            mutex = new Mutex(true, guidForMutex, out bNew);

            if (bNew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                serviceController = new ServiceController(ServerName);
                notifyIcon = new NotifyIcon();
                contextMenuStrip = new ContextMenuStrip();
                CreateMenu();
                notifyIcon.ContextMenuStrip = contextMenuStrip;

                ctrlPanel = new ControlPanelScreen();
                GetServiceStatus();
                SetUpTimers();
                notifyIcon.DoubleClick += ShowControlPanel;
                notifyIcon.MouseMove += ShowBalloon;
                notifyIcon.Visible = true;

                Application.Run();
            }
        }

        private static void SetUpTimers()
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = UPDATE_TIMER_SECONDS * 1000;
            timer.Elapsed += OnTimedEvent;

            balloonTimer = new Timer();
            balloonTimer.AutoReset = true;
            balloonTimer.Interval = 10;
            balloonTimer.Elapsed += OnBalloonTimedEvent;

            timer.Start();
            balloonTimer.Start();
        }

        private static void CreateMenu()
        {
            imageList = new ImageList();

            imageList.TransparentColor = Color.Black;

            imageList.Images.Add(Resources.FormRunHS);
            imageList.Images.Add(Resources.StopHS);
            imageList.Images.Add(Resources.RestartHS);
            imageList.Images.Add(Resources.ThumbnailViewHS);

            contextMenuStrip.Items.Add("Stop", imageList.Images[1], OnStopServiceMenu);
            contextMenuStrip.Items.Add("Start", imageList.Images[0], OnStartServiceMenu);
            contextMenuStrip.Items.Add("Show Control Panel screen", imageList.Images[3], ShowControlPanel);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add("Exit", imageList.Images[2], ExitController);
        }

        private static void ShowControlPanel(object sender, EventArgs e)
        {
            if (!ctrlPanel.Visible)
            {
                ctrlPanel.Show();
                GetServiceStatus();
            }
        }

        private static void ShowBalloon(object sender, MouseEventArgs e)
        {
            if (!isBalloonShown && !notifyIcon.ContextMenuStrip.Visible)
            {
                StringBuilder strBalloon = new StringBuilder();
                strBalloon.AppendLine("Current service status  " + serviceController.Status);
                ServiceActivity activity = ActivityDAO.GetActivityData();

                strBalloon.AppendLine("Time when service ran the last check  " +
                    (activity.IsSucceeded ? activity.LastCheck.ToString(@"M/d/yyyy hh:mm:ss tt") : "n/a"));
                if (!activity.IsSucceeded)
                {
                    strBalloon.AppendLine("Current state  n/a");
                }
                else
                {
                    if ((activity.WorkingState &&
                                            (serviceController.Status == ServiceControllerStatus.Running)))
                    {
                        strBalloon.AppendLine("Current state  Working: " +
                            activity.PassedProgress + " out of " + activity.TotalProgress + "...");
                    }
                    else
                        strBalloon.AppendLine("Current state  Idle");
                }

                string path = Utils.ServiceInfo.GetServicePath();
                Configuration config = Utils.ServiceInfo.GetServiceConfig(path);
                if (config.AppSettings.Settings["EmailServerName"] != null)
                    strBalloon.AppendLine("SMTP/Exchange server name/IP  " + config.AppSettings.Settings["EmailServerName"].Value);

                string sBalloon = strBalloon.ToString();

                balloonPosition = Cursor.Position;
                notifyIcon.ShowBalloonTip(5000, "Service information",
                                          sBalloon,
                                          ToolTipIcon.Info);

                isBalloonShown = true;
            }
        }

        /// <summary>
        /// Stop service event handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event arguments</param>
        public static void StopService(object sender, EventArgs e)
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                if (serviceController.CanStop)
                {
                    notifyIcon.Icon = Resources.Paused;
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    GetServiceStatus();
                }
            }
        }

        /// <summary>
        /// Start service event handler
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event arguments</param>
        public static void StartService(object sender, EventArgs e)
        {
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                serviceController.Start();
                try
                {
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(50000000));
                }
                catch (Exception)
                {
                }
                GetServiceStatus();
            }
        }

        public static void OnStartServiceMenu(object sender, EventArgs e)
        {
            if (ctrlPanel.Visible)
            {
                ServiceProgressForm progress = new ServiceProgressForm(ServiceOperation.ServiceStart);
                progress.ShowDialog(ctrlPanel);
            }
            else
            {
                StartService(sender, e);
            }
        }

        public static void OnStopServiceMenu(object sender, EventArgs e)
        {
            if (ctrlPanel.Visible)
            {
                ServiceProgressForm progress = new ServiceProgressForm(ServiceOperation.ServiceStop);
                progress.ShowDialog(ctrlPanel);
            }
            else
            {
                StopService(sender, e);
            }
        }

        private static void ExitController(object sender, EventArgs e)
        {
            ExitController(null);
        }
        /// <summary>
        /// shows message box
        /// </summary>
        /// <param name="errorMessage">message to show</param>
        public static void ShowMessageBox(string errorMessage)
        {
            MessageBox.Show(errorMessage, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// closes control panel application
        /// </summary>
        /// <param name="errorMessage">message to show within message box</param>
        public static void ExitController(string errorMessage)
        {
            notifyIcon.MouseMove -= ShowBalloon;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            if (balloonTimer != null)
            {
                balloonTimer.Stop();
                balloonTimer.Dispose();
            }

            if (errorMessage != null)
                ShowMessageBox(errorMessage);

            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            serviceController.Dispose();
            ctrlPanel.Dispose();
            Application.Exit();
            // Application doesn't exit after uninstall if Control Panel Screen is opened   
            Environment.Exit(1);
        }

        private static void GetServiceStatus()
        {
            if (contextMenuStrip.InvokeRequired)
            {
                contextMenuStrip.Invoke((GetServiceStatusCallback) GetServiceStatus);
                return;
            }
            try
            {
                serviceController.Refresh();
                switch (serviceController.Status)
                {
                    case ServiceControllerStatus.Running:
                        notifyIcon.Icon = Resources.Running;
                        contextMenuStrip.Items[0].Enabled = true;
                        contextMenuStrip.Items[1].Enabled = false;
                        break;
                    case ServiceControllerStatus.Stopped:
                        notifyIcon.Icon = Resources.Stopped;
                        contextMenuStrip.Items[0].Enabled = false;
                        contextMenuStrip.Items[1].Enabled = true;
                        break;
                    case ServiceControllerStatus.ContinuePending:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.StopPending:
                        notifyIcon.Icon = Resources.Paused;
                        contextMenuStrip.Items[0].Enabled = false;
                        contextMenuStrip.Items[1].Enabled = false;
                        break;
                }
                ctrlPanel.SetButtonStates();
            }
            catch (Exception ex)
            {
                ExitController(ex.Message);
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timer.Stop();
            GetServiceStatus();
            timer.Start();
        }

        private static void OnBalloonTimedEvent(object source, ElapsedEventArgs e)
        {
            if (isBalloonShown)
            {
                Point curPosition = Cursor.Position;
                if (curPosition.X <= (balloonPosition.X - notifyIcon.Icon.Width) ||
                    curPosition.X >= (balloonPosition.X + notifyIcon.Icon.Width) ||
                    curPosition.Y <= (balloonPosition.Y - notifyIcon.Icon.Height) ||
                    curPosition.Y >= (balloonPosition.Y + notifyIcon.Icon.Height))
                {
                    isBalloonShown = false;
                }
            }
        }

        #region Nested type: GetServiceStatusCallback

        private delegate void GetServiceStatusCallback();

        #endregion

        public static string GetServiceStartedTime()
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                Process[] pArray = Process.GetProcessesByName("LineCheckerSrv");
                if (pArray.Length != 0)
                {
                    return pArray[0].StartTime.ToString(@"M/d/yyyy hh:mm:ss tt");
                }
            }
            return "";
        }
    }
}
