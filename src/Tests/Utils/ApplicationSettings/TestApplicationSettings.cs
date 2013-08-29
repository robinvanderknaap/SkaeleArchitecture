using System.Collections.Generic;
using System.Globalization;
using Infrastructure.ApplicationSettings;

namespace Tests.Utils.ApplicationSettings
{
    /// <summary>
    /// Application settings class used to overwrite application settings from config from code.
    /// By default values from config are used
    /// </summary>
    public class TestApplicationSettings : IApplicationSettings
    {
        public TestApplicationSettings()
        {
            // Set defaults based on application settings from config 
            var applicationSettingsFromConfig = new Infrastructure.ApplicationSettings.ApplicationSettings();

            CoolOffPeriod = applicationSettingsFromConfig.CoolOffPeriod;
            ExpirationPeriodNewPasswordrequest = applicationSettingsFromConfig.ExpirationPeriodNewPasswordrequest;
            MaxLoginAttempts = applicationSettingsFromConfig.MaxLoginAttempts;
            PasswordPolicy = applicationSettingsFromConfig.PasswordPolicy;
            ConnectionString = applicationSettingsFromConfig.ConnectionString;
            Environment = applicationSettingsFromConfig.Environment;
            WindowsServiceMailSenderName = applicationSettingsFromConfig.WindowsServiceMailSenderName;
            ApplicationVersion = applicationSettingsFromConfig.ApplicationVersion;
            SmtpHost = applicationSettingsFromConfig.SmtpHost;
            SmtpPort = applicationSettingsFromConfig.SmtpPort;
            SmtpUsername = applicationSettingsFromConfig.SmtpUsername;
            SmtpPassword = applicationSettingsFromConfig.SmtpPassword;
            DefaultCulture = applicationSettingsFromConfig.DefaultCulture;
            AcceptedCultures = applicationSettingsFromConfig.AcceptedCultures;
            SmtpSslEnabled = applicationSettingsFromConfig.SmtpSslEnabled;
        }

        public int CoolOffPeriod { get; set; }
        public int ExpirationPeriodNewPasswordrequest { get; set; }
        public int MaxLoginAttempts { get; set; }
        public string PasswordPolicy { get; set; }
        public string ConnectionString { get; set; }
        public string Environment { get; set; }
        public string WindowsServiceMailSenderName { get; set; }
        public string ApplicationVersion { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpSslEnabled { get; set; }
        public CultureInfo DefaultCulture { get; set; }
        public IEnumerable<CultureInfo> AcceptedCultures { get; set; }
    }
}
