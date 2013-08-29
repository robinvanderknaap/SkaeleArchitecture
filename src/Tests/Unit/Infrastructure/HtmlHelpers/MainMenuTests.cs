using System;
using Infrastructure.HtmlHelpers;
using NUnit.Framework;
using Tests.Utils.TestFixtures;
using Tests.Utils.Various;
using Tests.Utils.WebFakers;

namespace Tests.Unit.Infrastructure.HtmlHelpers
{
    public class MainMenuTests : HtmlHelperTestFixture
    {
        [Test]
        public void Can_Create_Main_Menu_Without_Items()
        {
            var result = Html.MainMenu().ToHtmlString();

            const string expectedResult = @"<ulclass=""navnav-pills""></ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }

        [Test]
        public void Menu_Item_Is_Not_Visible_When_User_Is_Not_Allowed()
        {
            CurrentUser.AddRoles("User");

            var result = Html.MainMenu()
                .Add(new MainMenuItem()
                    .Text("Home")
                    .Icon("HomeIcon")
                    .Url("/"))
                .Add(new MainMenuItem()
                    .Text("Users")
                    .Icon("UserIcon")
                    .Url("/Users")
                    .Roles("Administrator"))
                .Add(new MainMenuItem()
                    .Text("Reports")
                    .Icon("ReportIcon")
                    .Add(new SubMenuItem()
                        .Text("User Report")
                        .Url("/Reports/User"))
                    .Add(new SubMenuItem()
                        .Text("Product Report")
                        .Url("/Reports/Product")
                        .Roles("Administrator"))
                    .Add(new Divider())
                    .Add(new SubMenuItem()
                        .Text("Stock Report")
                        .Url("/Reports/Stock")))
                .ToHtmlString();

            const string expectedResult = @"
                <ul class=""nav nav-pills"">
                    <li>
                        <a href=""/"">
                            <i class=""HomeIcon""></i>
                            <span>Home</span>
                        </a>
                    </li>
                    <li class=""dropdown"">
                        <a href=""javascript:;"" class=""dropdown-toggle"" data-toggle=""dropdown"">
                            <i class=""ReportIcon""></i>
                            <span>Reports</span>
                            <b class=""caret""></b>
                        </a>
                        <ul class=""dropdown-menu"">
                            <li><a href=""/Reports/User"">User Report</a></li>
                            <li class=""divider""></li>
                            <li><a href=""/Reports/Stock"">Stock Report</a></li>
                        </ul>
                    </li>
                </ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }

        [Test]
        public void Menu_Item_Is_Visible_When_User_Is_Allowed()
        {
            CurrentUser.AddRoles("Administrator");

            var result = Html.MainMenu()
                .Add(new MainMenuItem()
                    .Text("Home")
                    .Icon("HomeIcon")
                    .Url("/"))
                .Add(new MainMenuItem()
                    .Text("Users")
                    .Icon("UserIcon")
                    .Url("/Users")
                    .Roles("Administrator"))
                .Add(new MainMenuItem()
                    .Text("Reports")
                    .Icon("ReportIcon")
                    .Add(new SubMenuItem()
                        .Text("User Report")
                        .Url("/Reports/User"))
                    .Add(new SubMenuItem()
                        .Text("Product Report")
                        .Url("/Reports/Product")
                        .Roles("Administrator"))
                    .Add(new Divider())
                    .Add(new SubMenuItem()
                        .Text("Stock Report")
                        .Url("/Reports/Stock")))
                .ToHtmlString();

            const string expectedResult = @"
                <ul class=""nav nav-pills"">
                    <li>
                        <a href=""/"">
                            <i class=""HomeIcon""></i>
                            <span>Home</span>
                        </a>
                    </li>
                    <li>
                        <a href=""/Users"">
                            <i class=""UserIcon""></i>
                            <span>Users</span>
                        </a>
                    </li>
                    <li class=""dropdown"">
                        <a href=""javascript:;"" class=""dropdown-toggle"" data-toggle=""dropdown"">
                            <i class=""ReportIcon""></i>
                            <span>Reports</span>
                            <b class=""caret""></b>
                        </a>
                        <ul class=""dropdown-menu"">
                            <li><a href=""/Reports/User"">User Report</a></li>
                            <li><a href=""/Reports/Product"">Product Report</a></li>
                            <li class=""divider""></li>
                            <li><a href=""/Reports/Stock"">Stock Report</a></li>
                        </ul>
                    </li>
                </ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }

        [Test]
        public void Menu_Item_Is_Active_When_Action_And_Controller_Match()
        {
            ViewContext.RouteData.Values["action"] = "Index";
            ViewContext.RouteData.Values["controller"] = "Fake";

            var result = Html.MainMenu()
                .Add(new MainMenuItem()
                    .Text("Home")
                    .Icon("HomeIcon")
                    .Action<FakeController>("Index"))
                .Add(new MainMenuItem()
                    .Text("Users")
                    .Icon("UserIcon")
                    .Url("/Users"))
                .ToHtmlString();

            const string expectedResult = @"
                <ul class=""nav nav-pills"">
                    <li class=""active"">
                        <a href="""">
                            <i class=""HomeIcon""></i>
                            <span>Home</span>
                        </a>
                    </li>
                    <li>
                        <a href=""/Users"">
                            <i class=""UserIcon""></i>
                            <span>Users</span>
                        </a>
                    </li>
                </ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }

        [Test]
        public void Cannot_Set_Both_Url_And_Action()
        {
            Assert.Throws<ArgumentException>(() => 
                Html.MainMenu()
                    .Add(new MainMenuItem()
                        .Url("/Home")
                        .Action<FakeController>("Home"))
                .ToHtmlString()
            );
            Assert.Throws<ArgumentException>(() =>
                Html.MainMenu()
                    .Add(new MainMenuItem()
                        .Add(new SubMenuItem()
                            .Url("/Home")
                            .Action<FakeController>("Home")))
                .ToHtmlString()
            );
        }

        [Test]
        public void If_No_Url_Or_Action_Is_Specified_HashTag_Is_Used_As_Hyperlink()
        {
            var result = Html.MainMenu()
                .Add(new MainMenuItem()
                    .Text("Home")
                    .Icon("HomeIcon"))
                .Add(new MainMenuItem()
                    .Text("Reports")
                    .Icon("ReportIcon")
                    .Add(new SubMenuItem()
                        .Text("User Report")))
                .ToHtmlString();

            const string expectedResult = @"
                <ul class=""nav nav-pills"">
                    <li>
                        <a href=""#"">
                            <i class=""HomeIcon""></i>
                            <span>Home</span>
                        </a>
                    </li>
                    <li class=""dropdown"">
                        <a href=""javascript:;"" class=""dropdown-toggle"" data-toggle=""dropdown"">
                            <i class=""ReportIcon""></i>
                            <span>Reports</span>
                            <b class=""caret""></b>
                        </a>
                        <ul class=""dropdown-menu"">
                            <li><a href=""#"">User Report</a></li>                            
                        </ul>
                    </li>
                </ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }

        [Test]
        public void Redundant_Dividers_Are_Removed()
        {
            CurrentUser.AddRoles("User");

            var result = Html.MainMenu()
                .Add(new MainMenuItem()
                    .Text("Reports")
                    .Icon("ReportIcon")
                    .Add(new SubMenuItem()
                        .Text("User Report")
                        .Url("/Reports/User"))
                    .Add(new Divider())
                    .Add(new SubMenuItem()
                        .Text("Product Report")
                        .Url("/Reports/Product"))
                    .Add(new Divider())
                    .Add(new SubMenuItem()
                        .Text("Stock Report")
                        .Url("/Reports/Stock")
                        .Roles("Administrator"))
                        .Add(new Divider())
                        .Add(new Divider()))
                .ToHtmlString();

            const string expectedResult = @"
                <ul class=""nav nav-pills"">
                    <li class=""dropdown"">
                        <a href=""javascript:;"" class=""dropdown-toggle"" data-toggle=""dropdown"">
                            <i class=""ReportIcon""></i>
                            <span>Reports</span>
                            <b class=""caret""></b>
                        </a>
                        <ul class=""dropdown-menu"">
                            <li><a href=""/Reports/User"">User Report</a></li>
                            <li class=""divider""></li>
                            <li><a href=""/Reports/Product"">Product Report</a></li>
                        </ul>
                    </li>
                </ul>";

            Assert.AreEqual(expectedResult.StripWhiteSpace(), result.StripWhiteSpace());
        }
    }
}
