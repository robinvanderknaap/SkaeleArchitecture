using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Data.Utils;
using Domain.AbstractRepository;
using Infrastructure.Loggers;
using NHibernate;
using Web.App_Start;
using Web.Controllers;
using Web.Cultures;

namespace Web
{
    public class MvcApplication : HttpApplication
    {
        public const string SessionKey = "NHibernate.Session"; // We need this key to idetify the name of the nhibernate session in the http context items (used in NinjectSetup.cs in App_Start folder)
        public static ISessionFactory SessionFactory { get; private set; } // Initiated only once in Application_Start()
        
        protected void Application_Start()
        {
            // Initiate NHibernate session factory, this should be done once for every application start.
            // Creating a session factory is an time-consuming operation (all mapping files are processed for example)
            SessionFactory = NHibernateHelper.SessionFactory; 

            AreaRegistration.RegisterAllAreas();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoFac.Setup();

            // Avoids unexpected required attributes on value types like integer.
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }

        protected void Application_EndRequest(object sender, System.EventArgs e)
        {
            if (Context.Response.StatusCode == 302 && (new HttpContextWrapper(Context)).Request.IsAjaxRequest() && !string.IsNullOrWhiteSpace(Response.RedirectLocation))
            {
                Context.Response.StatusCode = 200;
                Response.Headers.Add("ForceRedirect", "1"); // Indicator for client side 
            }

            // Dispose NHibernate session if exists
            if (!Context.Items.Contains(SessionKey)) return;

            var session = (ISession)Context.Items[SessionKey];
            session.Dispose();
            Context.Items[SessionKey] = null;
        }

        protected void Application_AcquireRequestState()
        {

            // The setting <modules runAllManagedModulesForAllRequests="true"> causes all requests including images, css, js etc to be parsed through the app.
            // We can't turn it off, because that causes other errors with mvc. This check makes sure no null reference exceptions occur on static content
            if (HttpContext.Current.CurrentHandler == null)
            {
                return;
            }

            // With cassini (and probable windows server 2003 with wildcard mapping) all requests (also images, css, js) etc trigger this method
            // With such requests, it's not allowed to access session, not even examine if it's null, which fires an exception
            // This check ensures only when it's an mvc request the session is accessed. Other request are handled by the
            // default handler.
            if (HttpContext.Current.CurrentHandler.GetType() != typeof(MvcHandler))
            {
                return;
            }

            if (User.Identity.IsAuthenticated)
            {
                Context.User = AutoFac.Container.Resolve<IUserRepository>().GetOne(x => x.Email == User.Identity.Name);
            }

            // Set culture for current request
            AutoFac.Container.Resolve<ICultureService>().SetCulture(new HttpContextWrapper(HttpContext.Current));
        }

        // Handles all application errors and calls for the right view on the errorcontroller http://stackoverflow.com/a/5229581/426840
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;

            var logger = AutoFac.Container.Resolve<ILogger>();

            var logId = exception != null ?
                logger.Fatal(exception.Message, exception) :
                logger.Error("Unhandled error"); // This will probably never be called, but added to be sure

            // Don't execute error controller for custom error mesages when custom error is disabled
            if (!HttpContext.Current.IsCustomErrorEnabled) return;

            Response.Clear();
            Server.ClearError();

            Response.StatusCode = 500;

            // Avoid IIS messing with custome errors http://stackoverflow.com/a/1719474/426840
            // Also http://stackoverflow.com/a/2345742/426840 for web.config
            Response.TrySkipIisCustomErrors = true;

            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();
            }

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = Response.StatusCode != 404 ? "Index" : "NotFound";
            routeData.Values["exception"] = exception;
            routeData.Values["logId"] = logId;

            IController errorsController = new ErrorController();

            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);

            errorsController.Execute(rc);
        }
    }
}