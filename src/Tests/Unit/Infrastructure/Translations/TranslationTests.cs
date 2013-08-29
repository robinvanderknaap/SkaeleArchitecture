using System.Globalization;
using Infrastructure.DomainBase;
using Infrastructure.Translations;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.Translations
{
    class TranslationTests : BaseTestFixture
    {
        [Test]
        public void CanCreateTranslation()
        {
            const string code = "WelcomeMessage";
            const string text = "Hello, this is test";
            var culture = new CultureInfo("en-US");

            var translation = new Translation(code, text, culture);

            Assert.AreEqual(code, translation.Code);
            Assert.AreEqual(text, translation.Text);
            Assert.AreEqual(culture, translation.Culture);
        }

        [Test]
        public void CannotCreateInvalidTranslation()
        {
            Assert.Throws<BusinessRuleViolationException>(() => new Translation(" ", "text", new CultureInfo("en-US")));
            Assert.Throws<BusinessRuleViolationException>(() => new Translation("code", " ", new CultureInfo("en-US")));
            Assert.Throws<BusinessRuleViolationException>(() => new Translation("code", "text", null));
        }
    }
}
