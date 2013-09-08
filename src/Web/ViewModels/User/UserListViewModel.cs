using System.Collections.Generic;

namespace Web.ViewModels.User
{
    public class UserListViewModel
    {
        public List<UserViewModel> UserViewModels { get; set; }
        public int TotalUsers { get; set; }
    }
}