using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Form for showing service operations progress
    /// </summary>
    public partial class ServiceProgressForm : Form
    {
        private readonly ServiceOperation serviceOperation;
        private bool isReadyToClose = false;
        public ServiceProgressForm(ServiceOperation operation)
        {
            serviceOperation = operation;
            InitializeComponent();
        }

        private void ServiceProgressForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                switch (serviceOperation)
                {
                    case ServiceOperation.ServiceStart:
                        this.Text = "Service starting...";
                        break;
                    case ServiceOperation.ServiceStop:
                        this.Text = "Service stopping...";
                        break;
                    case ServiceOperation.ServiceRestart:
                        this.Text = "Service restarting...";
                        break;
                }
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (serviceOperation)
            {
                case ServiceOperation.ServiceStart:
                    Program.StartService(sender, e);
                    break;
                case ServiceOperation.ServiceStop:
                    Program.StopService(sender, e);
                    break;
                case ServiceOperation.ServiceRestart:
                    Program.StopService(sender, e);
                    Program.StartService(sender, e);
                    break;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isReadyToClose = true;
            this.Close();
        }

        private void ServiceProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isReadyToClose)
                e.Cancel = true;
        }
    }

    /// <summary>
    /// Service operations enum
    /// </summary>
    public enum ServiceOperation
    {
        ServiceStart,
        ServiceStop,
        ServiceRestart
    }
}