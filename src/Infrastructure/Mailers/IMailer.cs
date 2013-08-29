using System.Net.Mail;

namespace Infrastructure.Mailers
{
    public interface IMailer
    {
        void SendMail(MailMessage mailMessage);
    }
}
