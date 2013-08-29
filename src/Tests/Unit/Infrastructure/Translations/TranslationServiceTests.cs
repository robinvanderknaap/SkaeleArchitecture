using System;
using System.Collections.Generic;
using System.Globalization;
using Infrastructure.Translations;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.Translations
{
    class TranslationServiceTests : BaseTestFixture
    {
        [Test]
        public void CanTransLateItemWithDefaultCulture()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hello this is a test", new CultureInfo("en-US")),
                    new Translation("WelcomeMessage", "Hallo dit is een test", new CultureInfo("nl-NL"))
                };

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("Hello this is a test", translationService.Translate.WelcomeMessage);
        }

        [Test]
        public void CanTranslateItemWithSpecificCulture()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hello this is a test", new CultureInfo("en-US")),
                    new Translation("WelcomeMessage", "Hallo dit is een test", new CultureInfo("nl-NL"))
                };

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("Hallo dit is een test", translationService.Translate("WelcomeMessage", new CultureInfo("nl-NL")));
        }

        [Test]
        public void IfTranslationDoesNotExistCodeIsReturned()
        {
            var translations = new List<Translation>();

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("WelcomeMessage", translationService.Translate.WelcomeMessage);
        }

        [Test]
        public void IfTranslationForSpecificCultureDoesNotExistDefaultCutureIsUsed()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hello this is a test", new CultureInfo("en-US"))
                };

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("Hello this is a test", translationService.Translate("WelcomeMessage", new CultureInfo("nl-NL")));
        }

        [Test]
        public void WhenDefaultCultureIsNullServiceWillNotBreak()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hallo dit is een test", new CultureInfo("nl-NL"))
                };

            var translationService = new TranslationService(translations, null);

            Assert.AreEqual("WelcomeMessage", translationService.Translate.WelcomeMessage);
            Assert.AreEqual("Hallo dit is een test", translationService.Translate("WelcomeMessage", new CultureInfo("nl-NL")));
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(NullReferenceException))]
        public void ListOfTranslationCannotBeNull()
        {
            new TranslationService(null, new CultureInfo("en-US"));
        }

        [Test]
        public void CanTranslateUsingMethodCallInsteadOfProperty()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hello this is a test", new CultureInfo("en-US"))
                };

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("Hello this is a test", translationService.Translate("WelcomeMessage"));
        }

        [Test]
        public void CanTranslateUsingEnum()
        {
            var translations = new List<Translation>
                {
                    new Translation("WelcomeMessage", "Hello this is a test", new CultureInfo("en-US")),
                    new Translation("WelcomeMessage", "Hallo dit is een test", new CultureInfo("nl-NL"))
                };

            var translationService = new TranslationService(translations, new CultureInfo("en-US"));

            Assert.AreEqual("Hello this is a test", translationService.Translate(Test.WelcomeMessage));
            Assert.AreEqual("Hallo dit is een test", translationService.Translate(Test.WelcomeMessage, new CultureInfo("nl-NL")));
        }

        private enum Test
        {
            WelcomeMessage
        }
    }
}
