namespace Console.Admin.DemoData
{
    public class PocoLogItem
    {
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }
}
