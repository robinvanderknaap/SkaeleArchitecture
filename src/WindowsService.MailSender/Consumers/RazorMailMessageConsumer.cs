using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using Infrastructure.Mailers;
using Infrastructure.QueueMessages.RazorMailMessages;
using Infrastructure.Translations;
using MassTransit;
using RazorEngine.Templating;
using RazorMailMessage;

namespace WindowsService.MailSender.Consumers
{
    public class RazorMailMessageConsumer : Consumes<ResetPasswordRequestMessage>.All, Consumes<CustomMailMessage>.All
    {
        private readonly IRazorMailMessageFactory _razorMailMessageFactory;
        private readonly ITranslationService _translationService;
        private readonly IMailer _mailer;

        public RazorMailMessageConsumer(IRazorMailMessageFactory razorMailMessageFactory, ITranslationService translationService, IMailer mailer)
        {
            _razorMailMessageFactory = razorMailMessageFactory;
            _translationService = translationService;
            _mailer = mailer;
        }

        public void Consume(ResetPasswordRequestMessage resetPasswordRequestMessage)
        {
            var subject = _translationService.Translate("RequestToResetYourPassword", resetPasswordRequestMessage.CultureInfo);
            
            dynamic viewBag = new DynamicViewBag();
            viewBag.Culture = resetPasswordRequestMessage.CultureInfo;

            var mailMessage = CreateRazorMailMessage
            (
                "ResetPassword.ResetPasswordMailMessage.cshtml", 
                new
                {
                    resetPasswordRequestMessage.DisplayName, 
                    resetPasswordRequestMessage.ResetPasswordUrl,
                    Subject = subject
                },
                viewBag
            );

            mailMessage.From = new MailAddress("no-reply@skaele.nl");
            mailMessage.To.Add(resetPasswordRequestMessage.Email);
            mailMessage.Subject = subject;

            _mailer.SendMail(mailMessage);
        }

        public void Consume(CustomMailMessage message)
        {
            dynamic viewBag = new DynamicViewBag();
            viewBag.Culture = message.CultureInfo;

            MailMessage mailMessage = CreateRazorMailMessage
            (
                "CustomMail.CustomMailMessage.cshtml",
                new
                {
                    message.Body, message.Subject
                },
                viewBag
            );

            mailMessage.From = new MailAddress("no-reply@skaele.nl");
            mailMessage.To.Add(message.To);
            if (!string.IsNullOrWhiteSpace(message.Cc))
            {
                mailMessage.CC.Add(message.Cc);
            }
            if (!string.IsNullOrWhiteSpace(message.Bcc))
            {
                mailMessage.Bcc.Add(message.Bcc);
            }
            mailMessage.Subject = message.Subject;

            _mailer.SendMail(mailMessage);
        }

        private MailMessage CreateRazorMailMessage<TModel>(string templateName, TModel model, DynamicViewBag viewBag)
        {
            // Add images (defined in layout)
            var chuckNorris = Assembly.Load("Infrastructure").GetManifestResourceStream("Infrastructure.TemplateMailMessages.Images.chuck_mailheader.png");
            var blocks = Assembly.Load("Infrastructure").GetManifestResourceStream("Infrastructure.TemplateMailMessages.Images.blocks.png");

            var linkedResources = new List<LinkedResource>();

            if (chuckNorris != null)
            {
                linkedResources.Add(new LinkedResource(chuckNorris) { ContentId = "chuckNorris" });
            }

            if (blocks != null)
            {
                linkedResources.Add(new LinkedResource(blocks) { ContentId = "blocks" });
            }

            var mailMessage = _razorMailMessageFactory.Create(templateName, model, viewBag, linkedResources);

            return mailMessage;
        }
    }
}
