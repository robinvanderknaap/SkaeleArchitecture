using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Globalization;

namespace Infrastructure.ApplicationSettings
{
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// Total number of login attempts before user is locked out
        /// Duration of lockout is determined by CoolOffPeriod setting
        /// </summary>
        public int MaxLoginAttempts
        {
            get { return GetSetting<int>("MaxLoginAttempts"); }
        }

        /// <summary>
        /// Period in seconds user cannot login after max login attempts has been reached
        /// </summary>
        public int CoolOffPeriod
        {
            get { return GetSetting<int>("CoolOffPeriod"); }
        }

        /// <summary>
        /// Period in minutes a request for a new password is valid
        /// </summary>
        public int ExpirationPeriodNewPasswordrequest
        {
            get { return GetSetting<int>("ExpirationPeriodNewPasswordrequest"); }
        }

        /// <summary>
        /// Regular expression which is used to valide the password strength
        /// </summary>
        public string PasswordPolicy
        {
            get { return GetSetting<string>("PasswordPolicy"); }
        }

        /// <summary>
        /// Connection string of application database
        /// </summary>
        public string ConnectionString
        {
            get { return GetSetting<string>("ConnectionString"); }
        }

        /// <summary>
        /// Indicates current environment (eg. TEST, UAT, LIVE)
        /// </summary>
        public string Environment
        {
            get { return GetSetting<string>("Environment"); }
        }

        public string WindowsServiceMailSenderName
        {
            get { return GetSetting<string>("WindowsServiceMailSenderName"); }
        }

        public string ApplicationVersion
        {
            get { return GetSetting<string>("ApplicationVersion"); }
        }

        public string SmtpHost
        {
            get { return GetSetting<string>("SmtpHost"); }
        }

        public int SmtpPort
        {
            get { return GetSetting<int>("SmtpPort"); }
        }

        public string SmtpUsername
        {
            get { return GetSetting<string>("SmtpUsername"); }
        }

        public string SmtpPassword
        {
            get { return GetSetting<string>("SmtpPassword"); }
        }

        public bool SmtpSslEnabled
        {
            get { return GetSetting<bool>("SmtpSslEnabled"); }
        }

        public CultureInfo DefaultCulture
        {
            get { return new CultureInfo(GetSetting<string>("DefaultCulture")); }
        }

        public IEnumerable<CultureInfo> AcceptedCultures
        {
            get
            {
                var acceptedCultures = new List<CultureInfo>();
                var acceptedCulturesString = GetSetting<string>("AcceptedCultures");

                if (!string.IsNullOrWhiteSpace(acceptedCulturesString))
                {
                    acceptedCultures = acceptedCulturesString
                                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => new CultureInfo(x.Trim()))
                                .ToList();
                }

                return acceptedCultures;
            }
        }

        
        /// <summary>
        /// Retrieves settings from configuration file
        /// </summary>
        /// <exception cref="ApplicationException">Throws when key does not exist or does not contain a value</exception>
        public TResult GetSetting<TResult>(string key)
        {
            // Make sure setting exists
            if (!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                throw new ApplicationException(string.Format("Application setting '{0}' not found, please update configuration file.", key));
            }
            
            // Retrieve setting value
            var value = ConfigurationManager.AppSettings[key];

            // Convert value to specified type and return
            return (TResult)Convert.ChangeType(value, typeof(TResult), CultureInfo.InvariantCulture);
        }
    }
}