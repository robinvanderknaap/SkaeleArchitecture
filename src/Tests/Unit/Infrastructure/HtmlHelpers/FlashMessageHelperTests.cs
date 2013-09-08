using System.Collections.Generic;
using Infrastructure.FlashMessages;
using Infrastructure.HtmlHelpers;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;

namespace Tests.Unit.Infrastructure.HtmlHelpers
{
    class FlashMessageHelperTests : HtmlHelperTestFixture
    {
        [Test]
        public void CanCreateFlashMessageFromViewData()
        {
            var flashMessages = new List<FlashMessage>
                {
                    new FlashMessage("Notice", "This is a flashmessage", FlashMessageType.Success)
                };

            ViewContext.ViewData.Add("flashMessage", flashMessages);

            var flashMessage = Html.FlashMessage("testId");

            const string expectedResult = @"
                <div id=""testId"">
                    <div class=""alert alert-success alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Notice</h4>
                        This is a flashmessage
                    </div> 
                </div>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), flashMessage.ToHtmlString().StripWhiteSpace());
        }
        
        [Test]
        public void CanCreateFlashMessageFromTempData()
        {
            var flashMessages = new List<FlashMessage>
                {
                    new FlashMessage("Notice", "This is a flashmessage", FlashMessageType.Warning)
                };

            ViewContext.TempData.Add("flashMessage", flashMessages);

            var flashMessage = Html.FlashMessage("testId");

            const string expectedResult = @"
                <div id=""testId"">
                    <div class=""alert alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Notice</h4>
                        This is a flashmessage
                    </div> 
                </div>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), flashMessage.ToHtmlString().StripWhiteSpace());
        }

        [Test]
        public void CanCreateMultipleFlashMessagesFromTempData()
        {
            var flashMessages = new List<FlashMessage>
                {
                    new FlashMessage("Notice", "This is a flashmessage", FlashMessageType.Success),
                    new FlashMessage("Another notice", "This is a another flashmessage", FlashMessageType.Info)
                };

            ViewContext.TempData.Add("flashMessage", flashMessages);

            var flashMessage = Html.FlashMessage("testId");

            const string expectedResult = @"
                <div id=""testId"">
                    <div class=""alert alert-success alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Notice</h4>
                        This is a flashmessage
                    </div>
                    <div class=""alert alert-info alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Another notice</h4>
                        This is a another flashmessage
                    </div>
                </div>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), flashMessage.ToHtmlString().StripWhiteSpace());
        }

        [Test]
        public void CanCreateMultipleFlashMessagesFromViewData()
        {
            var flashMessages = new List<FlashMessage>
                {
                    new FlashMessage("Notice", "This is a flashmessage", FlashMessageType.Success),
                    new FlashMessage("Another notice", "This is a another flashmessage", FlashMessageType.Error)
                };

            ViewContext.ViewData.Add("flashMessage", flashMessages);

            var flashMessage = Html.FlashMessage("testId");

            const string expectedResult = @"
                <div id=""testId"">
                    <div class=""alert alert-success alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Notice</h4>
                        This is a flashmessage
                    </div>
                    <div class=""alert alert-danger alert-block""  onclick='$(this).fadeOut()'>
                        <h4>Another notice</h4>
                        This is a another flashmessage
                    </div>
                </div>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), flashMessage.ToHtmlString().StripWhiteSpace());
        }

        [Test]
        public void EmptyFlashMessageContainerIsRenderedWhenNoFlashMessagesAreAvailable()
        {
            var flashMessage = Html.FlashMessage("testId");

            Assert.AreEqual("<div id='testId'></div>", flashMessage.ToHtmlString());
        }
    }
}
