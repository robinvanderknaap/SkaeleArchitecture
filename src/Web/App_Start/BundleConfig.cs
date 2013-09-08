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
                new StyleBundle("~/Bundle/Css")
                .Include(
                    "~/Public/Css/FontAwesome/font-awesome.min.css",
                    "~/Public/Css/Bootstrap/bootstrap.min.css",
                    "~/Public/Css/main.css"
                    //"~/Themes/Default/Css/Libraries/Select2/select2.css",
                    //"~/Themes/Default/Css/Libraries/SlickGrid/slick.grid.css",
                    //"~/Themes/Default/Css/Libraries/JqGrid/ui.jqgrid.css",
                    //"~/Themes/Default/Css/slick.grid.custom.css",
                )
            );

            bundles.Add(
                new ScriptBundle("~/Bundle/Js")
                .Include(
                    "~/Themes/Default/Js/dummyConsole.js",
                    "~/Public/Js/Bootstrap/bootstrap.min.js",
                    "~/Public/Js/Jquery/jquery-2.0.3.min.js",
                    "~/Public/Js/Jquery.Validate/jquery.validate.min.js",
                    "~/Public/Js/Jquery.Validate/jquery.validate.unobtrusive.min.js",
                    "~/Public/Js/Jquery.Form/jquery.form.js",
                    "~/Public/Js/defaultAjaxForm.js",
                    "~/Public/Js/flashMessage.js",
                    "~/Public/Js/main.js"
                    //"~/Themes/Default/Js/Libraries/Select2/select2.min.js",
                    //"~/Themes/Default/Js/Libraries/Jquery.Event.Drag/jquery.Event.Drag-2.2.js",
                    //"~/Themes/Default/Js/Libraries/SlickGrid/slick.core.js",
                    //"~/Themes/Default/Js/Libraries/SlickGrid/slick.grid.js",
                    //"~/Themes/Default/Js/Libraries/JqGrid/jquery-ui-1.10.1.min,js",
                    //"~/Themes/Default/Js/Libraries/JqGrid/i18n/grid.locale-nl.js",
                    //"~/Themes/Default/Js/Libraries/JqGrid/jquery.jqGrid.src.js",
                    //"~/Themes/Default/Js/userIndexController.js",
                    //"~/Themes/Default/Js/logGridController.js"
                )
            );

            // Uncomment to test minimized bundle
            //BundleTable.EnableOptimizations = true;
        }
    }
}