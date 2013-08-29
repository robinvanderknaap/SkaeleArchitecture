using System;

namespace Infrastructure.Loggers
{

    public class LogItem
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public string Level { get; set; }
        public string Environment { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Username { get; set; }
        public string RequestMethod { get; set; }
        public string RequestUrl { get; set; }
        public string UrlReferrer { get; set; }
        public string ClientBrowser { get; set; }
        public string IpAddress { get; set; }
        public string PostedFormValues { get; set; }
        public string Stacktrace { get; set; }
        public string Exception { get; set; }
    }
}