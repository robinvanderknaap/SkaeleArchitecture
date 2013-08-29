using System.Collections.Generic;
using System.Globalization;

namespace Infrastructure.ApplicationSettings
{
    public interface IApplicationSettings
    {
        int CoolOffPeriod { get; }
        int ExpirationPeriodNewPasswordrequest { get; }
        int MaxLoginAttempts { get; }
        string PasswordPolicy { get; }
        string ConnectionString { get; }
        string Environment { get; }
        string WindowsServiceMailSenderName { get; }
        string ApplicationVersion { get; }
        string SmtpHost { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; }
        string SmtpPassword { get; }
        CultureInfo DefaultCulture { get; }
        IEnumerable<CultureInfo> AcceptedCultures { get; }
        bool SmtpSslEnabled { get; }
    }
}
