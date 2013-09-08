namespace Web.ViewModels.Login
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string Hash { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}