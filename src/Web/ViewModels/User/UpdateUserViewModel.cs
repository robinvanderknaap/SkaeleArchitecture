using System;

namespace Web.ViewModels.User
{
    public class UpdateUserViewModel
    {
        public int UserId { get; set; }
        public string NewDisplayName { get; set; }
        public string NewEmail {  get; set;}
    }
}