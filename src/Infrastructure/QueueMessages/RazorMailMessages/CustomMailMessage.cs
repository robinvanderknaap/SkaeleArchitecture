using System.Globalization;

namespace Infrastructure.QueueMessages.RazorMailMessages
{
    public class CustomMailMessage
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
