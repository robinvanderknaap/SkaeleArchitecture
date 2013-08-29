using System;
using System.Net;
using System.Net.Mail;
using Infrastructure.Loggers;

namespace Infrastructure.Mailers
{
    public class SmtpMailer : IMailer
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _isSslEnabled;
        private readonly ILogger _logger;

        public SmtpMailer(string smtpHost, int smtpPort, string smtpUsername, string smtpPassword, bool isSslEnabled, ILogger logger)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _isSslEnabled = isSslEnabled;
            _logger = logger;
        }

        public void SendMail(MailMessage mailMessage)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpHost, _smtpPort);

                if(!string.IsNullOrWhiteSpace(_smtpUsername) && !string.IsNullOrWhiteSpace(_smtpPassword))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                }

                smtpClient.EnableSsl = _isSslEnabled;

                smtpClient.Send(mailMessage);
            }
            catch (Exception exception)
            {
                _logger.Fatal("Sending email message failed", exception);
                throw;
            }
        }
    }
}