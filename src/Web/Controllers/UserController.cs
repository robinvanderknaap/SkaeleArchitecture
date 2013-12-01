using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.AbstractRepository;
using Infrastructure.DomainBase;
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
            return View();
        }

        public ActionResult UserList()
        {
            return PartialView("Partials/UserList");
        }

        public JsonResult GetUsers()
        {
            var users = _userRepository.GetAll()
                .Select(x=> new DisplayUserViewModel(x, _encryptor))
                .OrderBy(u=>u.DisplayName)
                .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUser(UpdateUserViewModel userViewModel)
        {
            var user = _userRepository.Get(userViewModel.UserId);

            if (user == null)
            {
                throw new HttpException(500, "User not found");
            }
            
            user.SetEmail(userViewModel.NewEmail, _userRepository);
            user.DisplayName = userViewModel.NewDisplayName;
            user.IsActive = userViewModel.NewIsActive;

            foreach (var newRole in userViewModel.NewRoles)
            {
                if (newRole.IsSelected && !user.IsInRole(newRole.RoleName))
                {
                    user.Roles.Add((Domain.Users.Role)Enum.Parse(typeof(Domain.Users.Role), newRole.RoleName));
                }

                if (!newRole.IsSelected && user.IsInRole(newRole.RoleName))
                {
                    user.Roles.Remove((Domain.Users.Role)Enum.Parse(typeof(Domain.Users.Role), newRole.RoleName));
                }
            }

            using (var transaction = new Transaction(_userRepository.Session))
            {
                _userRepository.Save(user);
                transaction.Commit();
            }

            return Json(new DisplayUserViewModel(user, _encryptor));
        }
    }
}
