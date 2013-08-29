using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Infrastructure.ApplicationSettings;
using Infrastructure.TemplateMailMessages;
using Infrastructure.Translations;
using Moq;
using NUnit.Framework;
using RazorMailMessage;
using RazorMailMessage.TemplateResolvers;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Unit.Infrastructure.TemplateMailMessages
{
    public class TemplateMailMessagesTests : BaseTestFixture
    {
        [Test]
        public void CanCreateCustomMailMessage()
        {
            var factory = GetRazorMailMessageFactory();

            const string template = "CustomMail.CustomMailMessage.cshtml";
            var model = new { Body = "This is a test", Subject = "Just testing" };

            var mailMessage = factory.Create(template, model);

            var expectedHtmlResult = File.ReadAllText("Unit/Infrastructure/TemplateMailMessages/ExpectedResults/CustomMailHtmlResult.txt").StripWhiteSpace();
            var expectedTextResult = File.ReadAllText("Unit/Infrastructure/TemplateMailMessages/ExpectedResults/CustomMailTextResult.txt").StripWhiteSpace();

            Assert.AreEqual(expectedHtmlResult, new StreamReader(mailMessage.AlternateViews[0].ContentStream).ReadToEnd().StripWhiteSpace());
            Assert.AreEqual(expectedTextResult, mailMessage.Body.StripWhiteSpace());
        }

        [Test]
        public void CanCreateResetPasswordMailMessage()
        {
            var factory = GetRazorMailMessageFactory();

            const string template = "ResetPassword.ResetPasswordMailMessage.cshtml";
            var model = new { DisplayName = "Chuck Norris", Subject = "Just testing", ResetPasswordUrl = "http://someurl" };

            var mailMessage = factory.Create(template, model);

            var expectedHtmlResult = File.ReadAllText("Unit/Infrastructure/TemplateMailMessages/ExpectedResults/ResetPasswordMailHtmlResult.txt").StripWhiteSpace();
            var expectedTextResult = File.ReadAllText("Unit/Infrastructure/TemplateMailMessages/ExpectedResults/ResetPasswordMailTextResult.txt").StripWhiteSpace();

            Assert.AreEqual(expectedHtmlResult, new StreamReader(mailMessage.AlternateViews[0].ContentStream).ReadToEnd().StripWhiteSpace());
            Assert.AreEqual(expectedTextResult, mailMessage.Body.StripWhiteSpace());
        }

        private static RazorMailMessageFactory GetRazorMailMessageFactory()
        {
            var culture = new CultureInfo("en-US");

            var templateResolver = new DefaultTemplateResolver("Infrastructure", "TemplateMailMessages");

            var translations = new List<Translation>
                {
                    new Translation("KindRegards", "Kind regards", culture),
                    new Translation("DearSirOrMadam", "Dear sir or madam", culture),
                    new Translation("ResetPasswordMailMessage", "ResetPasswordMailMessage", culture),
                    new Translation("ResetPassword", "Reset password", culture)
                };

            var translationService = new TranslationService(translations, culture);

            var applicationSettingsMock = new Mock<IApplicationSettings>();
            applicationSettingsMock.Setup(x => x.DefaultCulture).Returns(culture);

            Func<Type, Object> dependencyResolver = t =>
                {
                    if (t == typeof (ITranslationService))
                    {
                        return translationService;
                    }
                    if (t == typeof (IApplicationSettings))
                    {
                        return applicationSettingsMock.Object;
                    }
                    return null;
                };

            var factory = new RazorMailMessageFactory(templateResolver, typeof (ViewBaseClass<>), dependencyResolver);
            return factory;
        }
    }
}
