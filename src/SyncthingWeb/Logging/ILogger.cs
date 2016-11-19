using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Logging
{
    public interface ILogger
    {
        void Error(string message, params object[] args);
        void ErrorException(string message, Exception exception);
        void ErrorException(Exception exception, string message, params object[] args);

        void Warning(string message, params object[] args);
        void WarningException(string message, Exception exception);
        void WarningException(Exception exception, string message, params object[] args);

        void Info(string message, params object[] args);
        void InfoException(string message, Exception exception);
        void InfoException(Exception exception, string message, params object[] args);
    }
}
