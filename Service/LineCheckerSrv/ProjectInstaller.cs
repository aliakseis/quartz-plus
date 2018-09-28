using System.ComponentModel;
using System.Configuration.Install;

namespace LineCheckerSrv
{
    /// <summary>
    /// Container for System.ServiceProcess.ServiceInstaller 
    /// and System.ServiceProcess.ServiceProcessInstaller instances
    /// (see design view)
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}