using NUnit.Framework;
using Tests.Utils.ApplicationSettings;

namespace Tests.Utils.TestFixtures
{
    [TestFixture]
    public abstract class BaseTestFixture
    {
        protected TestApplicationSettings DefaultTestApplicationSettings;

        protected BaseTestFixture()
        {
            DefaultTestApplicationSettings = new TestApplicationSettings();
        }
    }
}
