using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Domain.AbstractRepository;
using Infrastructure.FilterAttributes;
using Infrastructure.FlashMessages;
using Infrastructure.PasswordPolicies;
using Infrastructure.QueueMessages.RazorMailMessages;
using Infrastructure.Translations;
using MassTransit;
using Web.Controllers.Base;
using Web.ViewModels.Login;

namespace Web.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceBus _bus;
        private readonly ITranslationService _translationService;
        private readonly IPasswordPolicy _passwordPolicy;

        public LoginController(IUserRepository userRepository, IServiceBus bus, ITranslationService translationService, IPasswordPolicy passwordPolicy)
        {
            _userRepository = userRepository;
            _bus = bus;
            _translationService = translationService;
            _passwordPolicy = passwordPolicy;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel loginViewModel, string returnUrl)
        {
            if (_userRepository.Authenticate(loginViewModel.Email, loginViewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(loginViewModel.Email, loginViewModel.RememberMe);

                // Prevent open redirection attack (http://weblogs.asp.net/jgalloway/archive/2011/01/25/preventing-open-redirection-attacks-in-asp-net-mvc.aspx#comments)
                if (Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                {
                    Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            AddFlashMessage(null, _translationService.Translate.EmailAndPasswordCombinationNotValid, FlashMessageType.Error, "messageContainer");

            return View(loginViewModel);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = _userRepository.GetOne(x => x.Email == resetPasswordViewModel.Email.Trim());

            if (user == null)
            {
                AddFlashMessage(null, string.Format(_translationService.Translate.UserWithEmailAddressWasNotFound, resetPasswordViewModel.Email), FlashMessageType.Success, "messageContainer");
                return View(resetPasswordViewModel);
            }

            var resetPasswordUrl = Url.Action("ChangePassword", "Login", new { userId = user.Id, hash = _userRepository.GetHashForNewPasswordRequest(user)}, Request.Url.Scheme);

            _bus.Publish(new ResetPasswordRequestMessage { DisplayName = user.DisplayName, Email = user.Email, ResetPasswordUrl = resetPasswordUrl, CultureInfo = user.Culture});

            AddFlashMessage(null, _translationService.Translate.AnEmailWasSendToYourEmailaddressWithInstructionsOnHowToResetYourPassword, FlashMessageType.Success, "messageContainer");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangePassword(int userId, string hash)
        {
            var user = _userRepository.Get(userId);
            
            if (!_userRepository.IsHashForNewPasswordRequestValid(user, hash))
            {
                throw new HttpException(403, "You don't have access to this page");
            }

            var changePasswordViewModel = new ChangePasswordViewModel { Hash = hash, UserId = userId };

            return View(changePasswordViewModel);
        }

        [HttpPost]
        [UnitOfWork]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var user = _userRepository.Get(changePasswordViewModel.UserId);

            if (!_userRepository.IsHashForNewPasswordRequestValid(user, changePasswordViewModel.Hash))
            {
                throw new HttpException(403, "You don't have access to this page");
            }

            if (!_passwordPolicy.Validate(changePasswordViewModel.Password))
            {
                AddFlashMessage(null, _translationService.Translate.PasswordShouldContainAtLeast5Characters, FlashMessageType.Error, "messageContainer");
                return View(changePasswordViewModel);
            }

            _userRepository.ChangePassword(user, changePasswordViewModel.Password);

            AddFlashMessage(null, _translationService.Translate.YourPasswordWasChangedSuccessfully, FlashMessageType.Success, "messageContainer");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

    }
}
