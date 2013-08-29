using System.Web.Mvc;
using Infrastructure.Loggers;

namespace Infrastructure.FilterAttributes
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger _logger;

        public LogActionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var details = string.Format("Controller: {0}, Action: {1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);

            _logger.Debug("Action started", details);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var details = string.Format("Controller: {0}, Action: {1}", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
            
            _logger.Debug("Action ended", details);
        }
    }
}
