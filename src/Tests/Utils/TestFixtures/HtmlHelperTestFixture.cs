using System.Web.Mvc;
using NUnit.Framework;
using Tests.Utils.WebFakers;

namespace Tests.Utils.TestFixtures
{
    public abstract class HtmlHelperTestFixture : BaseTestFixture
    {
        protected FakePrincipal CurrentUser = new FakePrincipal();
        protected HtmlHelper Html;
        protected ViewContext ViewContext;

        [SetUp]
        public void Setup()
        {
            ViewContext = new ViewContext
            {
                HttpContext = new FakeHttpContext(CurrentUser),
                TempData = new TempDataDictionary(),
                ViewData = new ViewDataDictionary()
            };

            Html = new HtmlHelper(ViewContext, new FakeViewDataContainer()); CurrentUser.ClearRoles();
        }
    }
}
