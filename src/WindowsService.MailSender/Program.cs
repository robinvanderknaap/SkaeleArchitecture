using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Infrastructure.ApplicationSettings;
using MassTransit;
using WindowsService.MailSender.WindowsServiceManagers;

namespace WindowsService.MailSender
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] arguments)
        {
            if (arguments.Any())
            {
                new WindowsServiceManager(new ApplicationSettings())
                    .ExecuteCommand(arguments.First());
            }
            else
            {
                RunServices(arguments);
            }
        }

        private static void RunServices(string[] arguments)
        {
            var servicesToRun = new ServiceBase[]
                {
                    new MailSenderService()
                };

            if (Environment.UserInteractive)
            {
                var onStartMethodInfo = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var service in servicesToRun)
                {
                    onStartMethodInfo.Invoke(service, new object[] { arguments });
                }

                Console.WriteLine("Press any key to exit");
                Console.Read();

                foreach (var service in servicesToRun)
                {
                    service.Stop();
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}

