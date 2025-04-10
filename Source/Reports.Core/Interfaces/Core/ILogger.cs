using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Reports.Core.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception ex);
        void Error(Exception ex, String className, String methodName, NameValueCollection methodParams);
        void Fatal(string message);
        void Fatal(Exception ex);
        string BuildExceptionMessage(Exception x, String className, String methodName, NameValueCollection methodParams);
    }
}
