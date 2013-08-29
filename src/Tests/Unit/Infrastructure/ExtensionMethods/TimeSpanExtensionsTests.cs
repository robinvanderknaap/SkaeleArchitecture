using System;
using Infrastructure.ExtensionMethods;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.ExtensionMethods
{
    class TimeSpanExtensionsTests : BaseTestFixture
    {
        [Test]
        public void CanCreateMillisecondsFromInteger()
        {
            Assert.AreEqual(TimeSpan.FromMilliseconds(5), 5.Milliseconds());
        }

        [Test]
        public void CanCreateSecondsFromInteger()
        {
            Assert.AreEqual(TimeSpan.FromSeconds(5), 5.Seconds());
        }

        [Test]
        public void CanCreateMinutesFromInteger()
        {
            Assert.AreEqual(TimeSpan.FromMinutes(5), 5.Minutes());
        }

        [Test]
        public void CanCreateHoursFromInteger()
        {
            Assert.AreEqual(TimeSpan.FromHours(5), 5.Hours());
        }

        [Test]
        public void CanCreateDaysFromInteger()
        {
            Assert.AreEqual(TimeSpan.FromDays(5), 5.Days());
        }
    }
}
