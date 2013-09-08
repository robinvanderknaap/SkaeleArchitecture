using System.Net.Mail;

namespace Web.ViewModels.Home
{
    public class EmailViewModel
    {
        public string From { get { return "solutiontemplate@skaele.it"; } }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailMessage MailMessage
        {
            get 
            { 
                var mailMessage = new MailMessage
                    {
                        From = new MailAddress(From), 
                        Subject = Subject, 
                        Body = Body
                    };

                mailMessage.To.Add(new MailAddress(To));
                if (!string.IsNullOrWhiteSpace(Cc))
                {
                    mailMessage.CC.Add(new MailAddress(Cc));
                }
                if (!string.IsNullOrWhiteSpace(Bcc))
                {
                    mailMessage.Bcc.Add(new MailAddress(Bcc));
                }

                return mailMessage;
            }
        }
    }
}