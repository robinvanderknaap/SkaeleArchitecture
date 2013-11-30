using System.Collections.Generic;
using Infrastructure.Encryption;
using WebGrease.Css.Extensions;

namespace Web.ViewModels.User
{
    public class DisplayUserViewModel
    {
        public DisplayUserViewModel(Domain.Users.User user, IEncryptor encryptor)
        {
            UserId = user.Id;
            Email = user.Email;
            DisplayName = user.DisplayName;
            Roles = string.Join(", ", user.Roles);
            IsActive = user.IsActive;
            GravatarHash = encryptor.Md5Encrypt(Email).Trim().ToLower();

            SelectedRoleIds = new List<int>();
            user.Roles.ForEach(r => SelectedRoleIds.Add((int)r));
        }

        public int UserId { get; private set; }
        public string Email { get; private set; }
        public string DisplayName { get; private set; }
        public string Roles { get; private set; }
        public List<int> SelectedRoleIds { get; private set; }
        public bool IsActive { get; private set; }
        public string GravatarHash { get; private set; }
    }
}