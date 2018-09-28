using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace ILC_ControlPanel
{
    /// <summary>
    /// Installer class
    /// </summary>
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run control panel after install completing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Installer1_AfterInstall(object sender, InstallEventArgs e)
        {
            using (Process p = new Process())
            {
                InstallContext cont = Context;

                ProcessStartInfo inf = new ProcessStartInfo(cont.Parameters["assemblypath"]);
                p.StartInfo = inf;
                p.Start();
            }
        }
    }
}