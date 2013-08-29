using System.Web.Mvc;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Infrastructure.HtmlHelpers;
using Tests.Utils.WebFakers;

namespace Tests.Unit.Infrastructure.HtmlHelpers
{
    class FocusHelperTests : HtmlHelperTestFixture
    {
        [Test]
        public void CanSetFocusOnElement()
        {
            var result = Html.Focus("TestId");
            const string expectedResult = "<script type=\"text/javascript\">$(document).ready(function() { $('#TestId').focus(); });</script>";

            Assert.AreEqual(expectedResult, result.ToHtmlString());
        }

        [Test]
        public void CanSetStronglyTypedFocusOnElement()
        {
            var stronglyTypedHtmlHelper = new HtmlHelper<TestModel>(ViewContext, new FakeViewDataContainer()); CurrentUser.ClearRoles();

            var result = stronglyTypedHtmlHelper.FocusFor(x=>x.Name);
            const string expectedResult = "<script type=\"text/javascript\">$(document).ready(function() { $('#Name').focus(); });</script>";

            Assert.AreEqual(expectedResult, result.ToHtmlString());
        }

        public class TestModel
        {
            public string Name { get; set; }
        }
    }
}
