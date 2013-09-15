using System.Linq;
using System.Web.Mvc;
using Domain.AbstractRepository;
using Infrastructure.Encryption;
using Web.Controllers.Base;
using Web.ViewModels.User;

namespace Web.Controllers
{
    public class UserController : AuthorizedController
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncryptor _encryptor;

        public UserController(IUserRepository userRepository, IEncryptor encryptor
        )
        {
            _userRepository = userRepository;
            _encryptor = encryptor;
        }

        public ActionResult Index()
        {
            var users = _userRepository.GetAll().ToList();
            
            var userListViewModel = new UserListViewModel
                {
                    UserViewModels = users.Select(x => new UserViewModel
                    {
                        Email = x.Email,
                        DisplayName = x.DisplayName,
                        Roles = x.Roles.Select(y => y.ToString()).ToList(),
                        IsActive = x.IsActive,
                        GravatarHash = _encryptor.Md5Encrypt(x.Email).Trim().ToLower()
                    }).ToList(),
                    TotalUsers = users.Count()
                };

            return View(userListViewModel);
        }

        public JsonResult GetUsers()
        {
            var users = _userRepository.GetAll().ToList();

            var userListViewModel = new UserListViewModel
            {
                UserViewModels = users.Select(x => new UserViewModel
                {
                    Email = x.Email,
                    DisplayName = x.DisplayName,
                    Roles = x.Roles.Select(y => y.ToString()).ToList(),
                    IsActive = x.IsActive,
                    GravatarHash = _encryptor.Md5Encrypt(x.Email).Trim().ToLower()
                }).ToList(),
                TotalUsers = users.Count()
            };

            return Json(userListViewModel);
        }
    }
}
