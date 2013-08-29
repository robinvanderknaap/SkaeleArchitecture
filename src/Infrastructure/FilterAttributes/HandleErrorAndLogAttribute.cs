using System.Web.Mvc;
using Infrastructure.Loggers;

namespace Infrastructure.FilterAttributes
{
    public class HandleErrorAndLogAttribute : HandleErrorAttribute
    {
        private readonly ILogger _logger;

        public HandleErrorAndLogAttribute(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            _logger.Fatal("Unhandled exception", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}
