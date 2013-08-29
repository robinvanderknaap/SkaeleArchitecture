using System;

namespace Infrastructure.Loggers
{
    public interface ILogger
    {
        string Debug(string message, string details = "");
        string Info(string message, string details = "");
        string Warn(string message, string details = "");
        string Error(string message, string details = "");
        string Fatal(string message, Exception exception, string details = "");
    }
}