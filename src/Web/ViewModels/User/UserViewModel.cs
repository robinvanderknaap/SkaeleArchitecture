using System.Collections.Generic;

namespace Web.ViewModels.User
{
    public class UserViewModel
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public string GravatarHash { get; set; }
    }
}