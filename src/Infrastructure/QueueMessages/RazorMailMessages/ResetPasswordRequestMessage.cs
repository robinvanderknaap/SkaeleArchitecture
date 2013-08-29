using System.Globalization;

namespace Infrastructure.QueueMessages.RazorMailMessages
{
    public class ResetPasswordRequestMessage
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ResetPasswordUrl { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
