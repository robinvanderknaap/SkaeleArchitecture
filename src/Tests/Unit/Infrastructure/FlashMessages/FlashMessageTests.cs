using Infrastructure.FlashMessages;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.FlashMessages
{
    class FlashMessageTests : BaseTestFixture
    {
        [Test]
        public void CanCreateFlashMessage()
        {
            var flashMessage = new FlashMessage("Attention", "This is a message", FlashMessageType.Info);

            Assert.AreEqual("Attention", flashMessage.Title);
            Assert.AreEqual("This is a message", flashMessage.Message);
            Assert.AreEqual(FlashMessageType.Info, flashMessage.Type);
        }
    }
}
