using System;

namespace SyncthingWeb.Logging
{
    public class NullLogger : ILogger
    {
        private static readonly Lazy<NullLogger> NullLoggerFactory = new Lazy<NullLogger>(() => new NullLogger());
        private NullLogger()
        {
        }

        public static ILogger Instance => NullLoggerFactory.Value;

        public void Error(string message, params object[] args)
        {
        }

        public void ErrorException(string message, Exception exception)
        {
        }

        public void ErrorException(Exception exception, string message, params object[] args)
        {
        }

        public void Warning(string message, params object[] args)
        {
        }

        public void WarningException(string message, Exception exception)
        {
        }

        public void WarningException(Exception exception, string message, params object[] args)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        public void InfoException(string message, Exception exception)
        {
        }

        public void InfoException(Exception exception, string message, params object[] args)
        {
        }
    }
}
