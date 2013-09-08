using System.Web.Mvc;
using Web.Controllers.Base;

namespace Web.Controllers
{
    public class ErrorController : AuthorizedController
    {
        public ActionResult Index(string logId)
        {
            if (Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(Response.StatusCode, Response.StatusDescription);
            }

            ViewBag.HttpStatusCode = Response.StatusCode;
            ViewBag.HttpStatusMessage = Response.StatusDescription;
            ViewBag.LogId = logId;

            return View("Error");
        }

        public ActionResult NotFound(string logId)
        {
            if (Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(Response.StatusCode, Response.StatusDescription);
            }

            ViewBag.HttpStatusCode = Response.StatusCode;
            ViewBag.HttpStatusMessage = Response.StatusDescription;
            ViewBag.LogId = logId;

            return View("NotFound");
        }
    }
}
