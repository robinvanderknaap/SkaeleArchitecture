using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Infrastructure.ApplicationSettings;

namespace WindowsService.MailSender.WindowsServiceManagers
{
    public class WindowsServiceManager
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly string _serviceName;
        private readonly string _assemblyLocation;

        public WindowsServiceManager(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
            _serviceName = _applicationSettings.WindowsServiceMailSenderName;
            _assemblyLocation = Assembly.GetExecutingAssembly().Location;
        }

        public void ExecuteCommand(string command)
        {
            EventLog.WriteEntry(_applicationSettings.WindowsServiceMailSenderName,
                                string.Format("Executing command: {0}", command));

            switch (command)
            {
                case "--install":
                    InstallService(_serviceName, _assemblyLocation);
                    break;
                case "--install-and-start":
                    InstallService(_serviceName, _assemblyLocation);
                    StartService(_serviceName);
                    break;
                case "--uninstall":
                    UnInstallService(_serviceName, _assemblyLocation);
                    break;
                case "--reinstall":
                    ReinstallService(_serviceName, _assemblyLocation);
                    break;
                case "--start":
                    StartService(_serviceName);
                    break;
                case "--stop":
                    StopService(_serviceName);
                    break;
                case "--restart":
                    RestartService(_serviceName);
                    break;
                default:
                    throw new ArgumentException("Unknown command");
            }
        }

        public void InstallService(
            string serviceName,
            string assemblyLocation
            )
        {
            if (IsInstalled(serviceName))
            {
                throw new Exception("Service is already installed");
            }

            var arguments = new[]
                {
                    "/i",
                    assemblyLocation
                };

            ManagedInstallerClass.InstallHelper(arguments);
        }

        public void UnInstallService(
            string serviceName,
            string assemblyLocation
            )
        {
            if (!IsInstalled(serviceName))
            {
                throw new Exception("Service is not installed");
            }

            var arguments = new[]
                {
                    @"/u",
                    assemblyLocation
                };

            ManagedInstallerClass.InstallHelper(arguments);
        }

        public void ReinstallService(
            string serviceName,
            string assemblyLocation
            )
        {
            UnInstallService(serviceName, assemblyLocation);
            InstallService(serviceName, assemblyLocation);
        }

        public void StartService(
            string serviceName
            )
        {
            ServiceController service = GetService(serviceName);

            service.Start();

            TimeSpan timeout = TimeSpan.FromMinutes(2);
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);

            if (service.Status != ServiceControllerStatus.Running)
            {
                throw new Exception("Failed to Start Windows Service");
            }
        }

        public bool IsInstalled(string serviceName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == serviceName);
        }

        public void RestartService(
            string serviceName
            )
        {
            if (IsRunning(serviceName))
            {
                StopService(serviceName);
            }

            StartService(serviceName);
        }

        public void StopService(string serviceName)
        {
            if (!IsRunning(serviceName))
            {
                return;
            }

            ServiceController service = GetService(serviceName);

            service.Stop();

            TimeSpan timeout = TimeSpan.FromMinutes(2);
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            if (service.Status != ServiceControllerStatus.Stopped)
            {
                throw new Exception("Failed to Stop Windows Service");
            }
        }

        public bool IsRunning(string serviceName)
        {
            ServiceController service = GetService(serviceName);

            return service.Status == ServiceControllerStatus.Running;
        }

        public ServiceController GetService(string serviceName)
        {
            ServiceController service = ServiceController.GetServices()
                                                         .FirstOrDefault(s => s.ServiceName == serviceName);

            if (service == null)
            {
                throw new Exception("Service doesn't exists");
            }

            return service;
        }
    }
}