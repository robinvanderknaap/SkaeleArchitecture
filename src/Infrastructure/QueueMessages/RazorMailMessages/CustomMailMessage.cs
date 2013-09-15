using System.Globalization;

namespace Infrastructure.QueueMessages.RazorMailMessages
{
    public class CustomMailMessage
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}
