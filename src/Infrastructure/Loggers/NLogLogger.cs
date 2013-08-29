using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infrastructure.ApplicationSettings;
using NLog;

namespace Infrastructure.Loggers
{
    public class NLogLogger : ILogger
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly HttpContextBase _httpContextBase;
        private readonly Logger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationSettings">Application settings</param>
        public NLogLogger(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationSettings">Application settings</param>
        /// <param name="currentClassName">Name of the class which invokes the logger. Name will be displayed in logfile ('logger' field)</param>
        public NLogLogger(IApplicationSettings applicationSettings, string currentClassName)
        {
            _applicationSettings = applicationSettings;
            _logger = LogManager.GetLogger(currentClassName);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="applicationSettings">Application settings</param>
        /// <param name="currentClassName">Name of the class which invokes the logger. Name will be displayed in logfile</param>
        /// <param name="httpContextBase">Wrapper containing information from httpcontext which will be included in log item</param>
        public NLogLogger(IApplicationSettings applicationSettings, string currentClassName, HttpContextBase httpContextBase)
        {
            _applicationSettings = applicationSettings;
            _httpContextBase = httpContextBase;
            _logger = LogManager.GetLogger(currentClassName);
        }

        public string Debug(string message, string details = "")
        {
            return _logger.IsDebugEnabled ? LogEvent(LogLevel.Debug, message, details, null) : string.Empty;
        }

        public string Info(string message, string details = "")
        {
            return _logger.IsInfoEnabled ? LogEvent(LogLevel.Info, message, details, null) : string.Empty;
        }

        public string Warn(string message, string details = "")
        {
            return _logger.IsWarnEnabled ? LogEvent(LogLevel.Warn, message, details, null) : string.Empty;
        }

        public string Error(string message, string details = "")
        {
            return _logger.IsErrorEnabled ? LogEvent(LogLevel.Error, message, details, null) : string.Empty;
        }

        public string Fatal(string message, Exception exception, string details = "")
        {
            return _logger.IsFatalEnabled ? LogEvent(LogLevel.Fatal, message, details, exception) : string.Empty;
        }

        private string LogEvent(LogLevel logLevel, string message, string details, Exception exception)
        {
            // We define a unique identifier which we can optionally show in the user interface.
            // This way users are able to copy-paste the identifier and mail
            // it to the support desk. Which makes it easier to track the error.
            var uniqueIdentifier = Guid.NewGuid().ToString();

            var logEventInfo = new LogEventInfo(logLevel, _logger.Name, message) { Exception = exception };

            logEventInfo.Properties.Add("uniqueIdentifier", uniqueIdentifier);
            logEventInfo.Properties.Add("environment", _applicationSettings.Environment);
            logEventInfo.Properties.Add("details", details ?? "");

            // Log only when httpcontext is available (in web sceanario's)
            if (_httpContextBase != null)
            {
                var username = _httpContextBase.User == null ? string.Empty : _httpContextBase.User.Identity.Name;
                var requestMethod = _httpContextBase.Request.HttpMethod;
                var requestUrl = _httpContextBase.Request.RawUrl;
                var urlReferrer = _httpContextBase.Request.UrlReferrer == null ? string.Empty : _httpContextBase.Request.UrlReferrer.OriginalString;
                var clientBrowser = _httpContextBase.Request.UserAgent;
                var ipAddress = _httpContextBase.Request.UserHostAddress;

                logEventInfo.Properties.Add("username", username);
                logEventInfo.Properties.Add("requestMethod", requestMethod);
                logEventInfo.Properties.Add("requestUrl", requestUrl);
                logEventInfo.Properties.Add("urlReferrer", urlReferrer);
                logEventInfo.Properties.Add("clientBrowser", clientBrowser);
                logEventInfo.Properties.Add("ipAddress", ipAddress);

                string postParameters;

                // log ajax posts ensure we don't log files or other big things
                if (_httpContextBase.Request.IsAjaxRequest() && _httpContextBase.Request.InputStream.Length > 0 &&
                    _httpContextBase.Request.InputStream.Length < 5125)
                {
                    var inputStream = new MemoryStream();
                    _httpContextBase.Request.InputStream.Position = 0;
                    _httpContextBase.Request.InputStream.CopyTo(inputStream);
                    inputStream.Position = 0;
                    _httpContextBase.Request.InputStream.Position = 0;

                    using (var r = new StreamReader(inputStream))
                    {
                        postParameters = r.ReadToEnd();
                    }

                    inputStream.Dispose();
                }
                else
                {
                    var postedFormValues = _httpContextBase.Request.Form.AllKeys.Select(x => string.Format("{0}:{1}", x, _httpContextBase.Request.Form[x]));
                    postParameters = string.Join(";", postedFormValues);
                }

                logEventInfo.Properties.Add("postedFormValues", postParameters);
            }

            _logger.Log(logEventInfo);

            return uniqueIdentifier;
        }
    }
}
