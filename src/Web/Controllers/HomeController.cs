using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.ApplicationSettings;
using Infrastructure.FlashMessages;
using Infrastructure.QueueMessages.RazorMailMessages;
using MassTransit;
using Web.Controllers.Base;
using Web.Cultures;
using Web.ViewModels.Home;

namespace Web.Controllers
{
    public class HomeController : AuthorizedController
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly ICultureService _cultureService;
        private readonly IServiceBus _serviceBus;

        public HomeController(
            IApplicationSettings applicationSettings,
            ICultureService cultureService,
            IServiceBus serviceBus
        )
        {
            _applicationSettings = applicationSettings;
            _cultureService = cultureService;
            _serviceBus = serviceBus;
        }

        public ActionResult Index()
        {
            return View();
        }

        // Used to test forbidden error page
        public ActionResult Forbidden()
        {
            throw new HttpException(403, "You don't have access to this page");
        }

        // Used to test general error page (for unhandled exceptions)
        public ActionResult Error()
        {
            throw new Exception("Yes, this controller throws an exception");
        }

        public ActionResult FlashMessages()
        {
            AddFlashMessage("Success", "This is a server side success message", FlashMessageType.Success);
            AddFlashMessage("Success", "This is a server side success inline message", FlashMessageType.Success, "inlineContainer");
            return View();
        }

        [HttpGet]
        public ActionResult ChangeLanguage(string culture, string returnUrl)
        {
            var cultureInfo = new CultureInfo(culture);

            if (!_applicationSettings.AcceptedCultures.Contains(cultureInfo))
            {
                throw new Exception(string.Format("{0} is not an accepted culture", culture));
            }

            _cultureService.SetCulture(cultureInfo, HttpContext);

            if (!Url.IsLocalUrl(returnUrl))
            {
                return Redirect("/");
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public ActionResult Email()
        {
            return View(new EmailViewModel());
        }

        [HttpPost]
        public JsonResult Email(EmailViewModel emailViewModel)
        {
            _serviceBus.Publish(new CustomMailMessage
            {
                Email = emailViewModel.To,
                Body = emailViewModel.Body,
                CultureInfo = ((Domain.Users.User)User).Culture,
                Subject = emailViewModel.Subject
            });

            //_mailer.SendMail(emailViewModel.MailMessage);
            return Json("Email sent successfully");
        }
    }
}
