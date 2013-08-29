using System;
using System.Collections.Specialized;
using System.Web;
using Infrastructure.Loggers;
using Moq;
using NUnit.Framework;
using Npgsql;
using Tests.Utils.ApplicationSettings;
using Tests.Utils.TestFixtures;
using Tests.Utils.WebFakers;
using LogLevel = NLog.LogLevel;

namespace Tests.Integration.Infrastructure.Loggers
{
    class NLogLoggerTests : DataTestFixture
    {
        [Test]
        public void CanLogDebugMessage()
        {
            TestLogMessage(LogLevel.Debug, exception: null, includeHttpContext: false);
        }

        [Test]
        public void CanLogDebugMessageWithHttpContext()
        {
            TestLogMessage(LogLevel.Debug, exception: null, includeHttpContext:true);
        }

        [Test]
        public void CanLogInfoMessage()
        {
            TestLogMessage(LogLevel.Info, exception: null, includeHttpContext: false);
        }

        [Test]
        public void CanLogInfoMessageWithHttpContext()
        {
            TestLogMessage(LogLevel.Info, exception: null, includeHttpContext: true);
        }

        [Test]
        public void CanLogWarningMessage()
        {
            TestLogMessage(LogLevel.Warn, exception: null, includeHttpContext: false);
        }

        [Test]
        public void CanLogWarningMessageWithHttpContext()
        {
            TestLogMessage(LogLevel.Warn, exception: null, includeHttpContext: true);
        }

        [Test]
        public void CanLogErrorMessage()
        {
            TestLogMessage(LogLevel.Error, exception: null, includeHttpContext: false);
        }

        [Test]
        public void CanLogErrorMessageWithHttpContext()
        {
            TestLogMessage(LogLevel.Error, exception: null, includeHttpContext: true);
        }

        [Test]
        public void CanLogFatalMessage()
        {
            TestLogMessage(LogLevel.Fatal, new Exception("This is an exception"), includeHttpContext: false);
        }

        [Test]
        public void CanLogFatalMessageWithHttpContext()
        {
            TestLogMessage(LogLevel.Fatal, new Exception("This is an exception"), includeHttpContext: true);
        }

        [Test]
        public void NullableValuesInHttpContextDoNotBreakLogging()
        {
            // Set nullable values is true to set user to null and urlreferrer to null.
            var logger = new NLogLogger(new TestApplicationSettings(), "NLogLoggerTests", GetHttpContext(nullValues: true));

            var logMessageId = logger.Debug("This is a log message");

            var logMessage = GetLogMessageFromDatabase(logMessageId);

            Assert.AreEqual(string.Empty, logMessage.Username);
            Assert.AreEqual(string.Empty, logMessage.UrlReferrer);
        }

        [Test]
        public void WhenNoCurrentClassNameIsSpecifiedNLogUsesDefaultCurrentClassName()
        {
            var logger = new NLogLogger(new TestApplicationSettings());
            
            var logMessageId = logger.Debug("This is a log message", "Some extra log details");

            var logMessage = GetLogMessageFromDatabase(logMessageId);

            Assert.AreEqual("Infrastructure.Loggers.NLogLogger", logMessage.Source);
        }

        private void TestLogMessage(LogLevel logLevel, Exception exception, bool includeHttpContext)
        {
            var logger = includeHttpContext ?
                new NLogLogger(new TestApplicationSettings(), "NLogLoggerTests", GetHttpContext(false)) :
                new NLogLogger(new TestApplicationSettings(), "NLogLoggerTests");
            
            var timeBeforeLog = DateTime.Now;

            var logMessageId = string.Empty;

            switch (logLevel.ToString())
            {
                case "Debug":
                    logMessageId = logger.Debug("This is a log message", "Some extra log details");
                    break;
                case "Info":
                    logMessageId = logger.Info("This is a log message", "Some extra log details");
                    break;
                case "Warn":
                    logMessageId = logger.Warn("This is a log message", "Some extra log details");
                    break;
                case "Error":
                    logMessageId = logger.Error("This is a log message", "Some extra log details");
                    break;
                case "Fatal":
                    logMessageId = logger.Fatal("This is a log message", exception, "Some extra log details");
                    break;
            }
            var timeAfterLog = DateTime.Now;

            var logMessage = GetLogMessageFromDatabase(logMessageId);

            Assert.AreEqual(Guid.Parse(logMessageId), logMessage.Id);
            Assert.IsTrue(logMessage.Created >= timeBeforeLog.AddMilliseconds(-timeBeforeLog.Millisecond - 1) &&
                          logMessage.Created <= timeAfterLog.AddMilliseconds(-timeAfterLog.Millisecond));
            Assert.AreEqual(logLevel.ToString(), logMessage.Level);
            Assert.AreEqual("DEV", logMessage.Environment);
            Assert.AreEqual("NLogLoggerTests", logMessage.Source);
            Assert.AreEqual("This is a log message", logMessage.Message);
            Assert.AreEqual("Some extra log details", logMessage.Details);
            
            if (includeHttpContext)
            {
                Assert.AreEqual("robink", logMessage.Username);
                Assert.AreEqual("POST", logMessage.RequestMethod);
                Assert.AreEqual("http://skaele.nl", logMessage.RequestUrl);
                Assert.AreEqual("http://webpirates.nl", logMessage.UrlReferrer);
                Assert.AreEqual("Chrome", logMessage.ClientBrowser);
                Assert.AreEqual("127.0.0.1", logMessage.IpAddress);
                Assert.AreEqual("Firstname:Robin;Middlename:van der;Lastname:Knaap", logMessage.PostedFormValues);
            }
            else
            {
                Assert.AreEqual(string.Empty, logMessage.Username);
                Assert.AreEqual(string.Empty, logMessage.RequestMethod);
                Assert.AreEqual(string.Empty, logMessage.RequestUrl);
                Assert.AreEqual(string.Empty, logMessage.UrlReferrer);
                Assert.AreEqual(string.Empty, logMessage.ClientBrowser);
                Assert.AreEqual(string.Empty, logMessage.IpAddress);
                Assert.AreEqual(string.Empty, logMessage.PostedFormValues);
            }
            Assert.AreEqual("NLogLoggerTests.TestLogMessage => NLogLogger." + logLevel + " => NLogLogger.LogEvent", logMessage.StackTrace);

            Assert.AreEqual(exception == null
                    ? string.Empty
                    : "This is an exception, System.Exception, Exception, System.Exception: This is an exception, , "
                    , logMessage.Exception);
        }

        private static HttpContextBase GetHttpContext(bool nullValues)
        {
            var request = new Mock<HttpRequestBase>();

            request.Setup(r => r.HttpMethod).Returns("POST");
            request.Setup(r => r.RawUrl).Returns("http://skaele.nl");
            request.Setup(r => r.UrlReferrer).Returns(nullValues ? null : new Uri("http://webpirates.nl"));
            request.Setup(r => r.UserAgent).Returns("Chrome");
            request.Setup(r => r.UserHostAddress).Returns("127.0.0.1");
            request.Setup(r => r.Form).Returns(new NameValueCollection 
                {
                    {"Firstname", "Robin"},
                    {"Middlename", "van der"},
                    {"Lastname", "Knaap"}
                });
            request.Setup(r => r.QueryString).Returns(new NameValueCollection());

            var mockHttpContext = new Mock<HttpContextBase>();

            mockHttpContext.Setup(x => x.User).Returns(nullValues ? null : new FakePrincipal("robink"));
            mockHttpContext.Setup(c => c.Request).Returns(request.Object);

            return mockHttpContext.Object;
        }

        private LogMessage GetLogMessageFromDatabase(string uniqueIdentifier)
        {
            using (var sqlConnection = new NpgsqlConnection(DefaultTestApplicationSettings.ConnectionString))
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = string.Format("SELECT * FROM Log WHERE Id = '{0}'", uniqueIdentifier);
                    sqlConnection.Open();

                    var logMessage = new LogMessage();

                    using (var sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            logMessage.Id = Guid.Parse(sqlReader["Id"].ToString());
                            logMessage.Created = DateTime.Parse(sqlReader["Created"].ToString());
                            logMessage.Level = sqlReader["Level"].ToString();
                            logMessage.Environment = sqlReader["Environment"].ToString();
                            logMessage.Source = sqlReader["Source"].ToString();
                            logMessage.Message = sqlReader["Message"].ToString();
                            logMessage.Details = sqlReader["Details"].ToString();
                            logMessage.Username = sqlReader["Username"].ToString();
                            logMessage.RequestUrl = sqlReader["RequestUrl"].ToString();
                            logMessage.RequestMethod = sqlReader["RequestMethod"].ToString();
                            logMessage.UrlReferrer = sqlReader["UrlReferrer"].ToString();
                            logMessage.ClientBrowser = sqlReader["ClientBrowser"].ToString();
                            logMessage.IpAddress = sqlReader["IpAddress"].ToString();
                            logMessage.PostedFormValues = sqlReader["PostedFormValues"].ToString();
                            logMessage.StackTrace = sqlReader["StackTrace"].ToString();
                            logMessage.Exception = sqlReader["Exception"].ToString();
                        }

                    }
                    return logMessage;
                }
            }
        }
    }

    public class LogMessage
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
        public string StackTrace { get; set; }
        public string Exception { get; set; }
    }
}
