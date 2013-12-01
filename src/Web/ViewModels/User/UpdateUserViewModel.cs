using System;
using System.Collections.Generic;

namespace Web.ViewModels.User
{
    public class UpdateUserViewModel
    {
        public int UserId { get; set; }
        public string NewDisplayName { get; set; }
        public string NewEmail {  get; set;}
        public List<RoleViewModel> NewRoles { get; set; }
        public bool NewIsActive { get; set; }

    }
}