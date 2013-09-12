using Infrastructure.ApplicationSettings;
using Topshelf;

namespace WindowsService.MailSender
{
    public class Program
    {
        public static void Main()
        {
            var applicationSettings = new ApplicationSettings();

            // Use topshelf for installing and activating the Windows service
            // http://topshelf-project.com/
            HostFactory.Run(x =>                                 
            {
                x.Service<MailSenderService>(s =>                       
                {
                    s.ConstructUsing(name => new MailSenderService());    
                    s.WhenStarted(tc => tc.Start());              
                    s.WhenStopped(tc => tc.Stop());               
                });

                // http://4sysops.com/archives/service-account-best-practices-part-1-choosing-a-service-account/
                x.RunAsLocalSystem();

                x.SetServiceName(applicationSettings.WindowsServiceMailSenderName);
                x.SetDisplayName(applicationSettings.WindowsServiceMailSenderName);
                x.SetDescription("Service for sending email");

                x.EnableServiceRecovery(rc => rc.RestartService(1));
            });                                               
        }
    }
}

