using System;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Unit.Infrastructure.ApplicationSettings
{
    public class ApplicationSettingsTest : BaseTestFixture
    {
        [Test]
        public void AccessingNonExistingApplicationSettingThrowsException()
        {
            const string key = "wrongSettingKey";

            var applicationSettings = new global::Infrastructure.ApplicationSettings.ApplicationSettings();

            CustomAssert.ThrowsWithExceptionMessage<ApplicationException>(
                () => applicationSettings.GetSetting<int>(key),
                string.Format("Application setting '{0}' not found, please update configuration file.", key)
            );
        }
    }
}
