using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace FreeDnsService
{
    [RunInstaller(true)]
    public class FreeDnsServiceInstaller : Installer
    {
        public FreeDnsServiceInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            // privileges
            processInstaller.Account = ServiceAccount.LocalService;

            serviceInstaller.DisplayName = Properties.Settings.Default.ServiceName;
            serviceInstaller.ServiceName = Properties.Settings.Default.ServiceName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
