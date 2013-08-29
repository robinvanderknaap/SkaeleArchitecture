using Data;
using Infrastructure.Loggers;
using MvcJqGrid;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Integration.Infrastructure.Loggers
{
    class LogRepositoryTests : DataTestFixture
    {
        [Test]
        public void GetLogItems_AddedOneLogItem_ContainsOneLogItem()
        {
            var logRepository = new LogRepository(Session);
            var logger = new NLogLogger(DefaultTestApplicationSettings);
            var gridSettings = new GridSettings { PageSize = 10, PageIndex = 0 };

            const string message = "Test";
            logger.Debug(message);

            var logItems = logRepository.GetLogItems(gridSettings);

            Assert.AreEqual(1, logItems.Count);
            Assert.AreEqual(message, logItems[0].Message);
        }

        [Test]
        public void CountLogItems_AddedTenLogItems_CollectionContainsTenLogItems()
        {
            var logRepository = new LogRepository(Session);
            var logger = new NLogLogger(DefaultTestApplicationSettings);
            var gridSettings = new GridSettings { PageSize = 20, PageIndex = 0 };

            const int totalMessages = 10;
            
            for (var messageCounter = 0; messageCounter < totalMessages; messageCounter++)
            {
                logger.Debug("Test");
            }

            var logItems = logRepository.GetLogItems(gridSettings);

            Assert.AreEqual(10, logItems.Count);
        }
    }
}
