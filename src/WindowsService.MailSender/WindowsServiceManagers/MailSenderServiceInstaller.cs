using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using Infrastructure.ApplicationSettings;

namespace WindowsService.MailSender.WindowsServiceManagers
{
    [RunInstaller(true)]
    public partial class MailSenderServiceInstaller : Installer
    {
        public MailSenderServiceInstaller()
        {
            InitializeComponent();

            var applicationSettings = new ApplicationSettings();

            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = applicationSettings.WindowsServiceMailSenderName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = applicationSettings.WindowsServiceMailSenderName;

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}