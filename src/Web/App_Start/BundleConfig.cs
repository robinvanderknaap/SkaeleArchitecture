using System.Web.Optimization;

namespace Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Virtual path of bundle cannot point to actual folder:
            // http://stackoverflow.com/q/13673759/426840
            // http://forums.asp.net/post/5012037.aspx

            bundles.Add(
                new StyleBundle("~/Bundle/Public/Css")
                .Include(
                    "~/Public/Css/FontAwesome/font-awesome.min.css",
                    "~/Public/Css/Bootstrap/bootstrap.min.css",
                    "~/Public/Css/Jquery.Ui/jquery-ui-1.10.3.custom.min.css",
                    //"~/Public/Css/BootstrapSwitch/bootstrap-switch.css",
                    "~/Public/Css/JqGrid/ui.jqgrid.css",
                    "~/Public/Css/JqGrid/ui.jqgrid.custom.css",
                    "~/Public/Css/main.css"
                )
            );

            bundles.Add(
                new ScriptBundle("~/Bundle/Public/Js")
                .Include(
                    "~/Themes/Default/Js/dummyConsole.js",
                    "~/Public/Js/Bootstrap/bootstrap.min.js",
                    "~/Public/Js/Jquery/jquery-2.0.3.min.js",
                    "~/Public/Js/Jquery.Ui/jquery-ui-1.10.3.custom.min.js",
                    "~/Public/Js/Jquery.Validate/jquery.validate.min.js",
                    "~/Public/Js/Jquery.Validate/jquery.validate.unobtrusive.min.js",
                    "~/Public/Js/Angular/angular.min.js",
                    "~/Public/Js/Angular/angular-route.min.js",
                    //"~/Public/Js/BootstrapSwitch/bootstrap-switch.min.js",
                    "~/Public/Js/JqGrid/JqGrid/i18n/grid.locale-nl.js",
                    "~/Public/Js/JqGrid/jquery.jqGrid.min.js",
                    "~/Public/Js/Jquery.Form/jquery.form.js",
                    "~/Public/Js/defaultAjaxForm.js",
                    "~/Public/Js/flashMessage.js",
                    "~/Public/Js/main.js",
                    "~/Public/Js/Modules/User/app.js",
                    "~/Public/Js/Modules/User/directives.js",
                    "~/Public/Js/Modules/User/userControllers.js"
                )
            );

            // Uncomment to test minimized bundle
            //BundleTable.EnableOptimizations = true;
        }
    }
}