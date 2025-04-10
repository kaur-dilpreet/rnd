using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using System.Web;
using System.Collections.Specialized;

namespace Reports.Core.Logging
{
    public class Logger : ILogger
    {
        private ILog logger;

        public Logger()
        {
            //log4net.Config.XmlConfigurator.Configure(new FileInfo(HttpContext.Current.Server.MapPath(Helpers.Environment.GetSetting(Helpers.Configuration.Enumerations.AppSettings.Log4Net))));
            logger = LogManager.GetLogger("TA");
        }

        public void Info(string message)
        {
            logger.Info("<message>" + message + "</message>");
        }

        public void Warn(string message)
        {
            logger.Warn("<message>" + message + "</message>");
        }

        public void Debug(string message)
        {
            logger.Debug("<message>" + message + "</message>");
        }

        public void Error(string message)
        {
            logger.Error("<message>" + message + "</message>");
        }

        public void Error(Exception ex, String className, String methodName, NameValueCollection methodParams)
        {
            logger.Error(BuildExceptionMessage(ex, className, methodName, methodParams));
        }

        public void Error(Exception ex)
        {
            logger.Error(BuildExceptionMessage(ex));
        }

        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        public void Fatal(Exception ex)
        {
            logger.Fatal(BuildExceptionMessage(ex));
        }

        /// <summary>
        /// Build a string error message from a given Exception
        /// </summary>
        /// <param name="x">The exception to build a message for</param>
        /// <returns>String containing the message</returns>
        public string BuildExceptionMessage(Exception x, String className, String methodName, NameValueCollection methodParams)
        {
            String tab = new string(' ', 4);

            string strErrorMsg = Environment.NewLine + "<error>";

            strErrorMsg += Environment.NewLine + tab + "<datetime>" + String.Format("{0:MM/dd/yyyy hh:mm:ss tt}", DateTime.UtcNow) + "</datetime>";

            if (Core.Utilities.Utilities.GetLoggedInUser() != null)
            {
                strErrorMsg += Environment.NewLine + tab + "<user>" + Environment.NewLine + tab + tab + "<username>" + String.Format("{0} {1}", Utilities.Utilities.GetLoggedInUser().FirstName, Utilities.Utilities.GetLoggedInUser().LastName) + "</username>";
                strErrorMsg += Environment.NewLine + tab + tab + "<useremail>" + Utilities.Utilities.GetLoggedInUser().Email + "</username>";
                strErrorMsg += Environment.NewLine + tab + "</user>";
            }

            strErrorMsg += "<errorPath>" + System.Web.HttpContext.Current.Request.Path + "</errorPath>";

            // Get the QueryString along with the Virtual Path
            strErrorMsg += Environment.NewLine + tab + "<rawUrl>" + System.Web.HttpContext.Current.Request.RawUrl + "</rawUrl>";

            strErrorMsg += Environment.NewLine + tab + "<className>" + className + "</className>";

            strErrorMsg += Environment.NewLine + tab + "<methodName>" + methodName + "</methodName>";

            strErrorMsg += Environment.NewLine + tab + "<params>";

            if (methodParams != null)
            {
                foreach (String key in methodParams)
                {
                    strErrorMsg += Environment.NewLine + tab + tab + "<param name=\"" + key + "\"><![CDATA[" + methodParams[key] + "]]></param>";
                }
            }

            strErrorMsg += Environment.NewLine + tab + "</params>";

            if (x != null)
            {
                Exception logException = x;
                if (x.InnerException != null)
                    logException = x.InnerException;

                // Get the error message
                strErrorMsg += Environment.NewLine + tab + "<message>" + logException.Message + "</message>";

                // Source of the message
                strErrorMsg += Environment.NewLine + tab + "<source>" + logException.Source + "</source>";

                // Method where the error occurred
                strErrorMsg += Environment.NewLine + tab + "<targetSite>" + logException.TargetSite + "</targetSite>";

                // Stack Trace of the error

                strErrorMsg += Environment.NewLine + tab + "<stackTrace><![CDATA[" + Environment.NewLine + logException.StackTrace + Environment.NewLine + tab + "]]></stackTrace>";

                if (logException.InnerException != null)
                {
                    // Get the error message
                    strErrorMsg += Environment.NewLine + "<innerException>";
                    strErrorMsg += Environment.NewLine + tab + "<message>" + logException.InnerException.Message + "</message>";

                    // Source of the message
                    strErrorMsg += Environment.NewLine + tab + "<source>" + logException.InnerException.Source + "</source>";

                    // Method where the error occurred
                    strErrorMsg += Environment.NewLine + tab + "<targetSite>" + logException.InnerException.TargetSite + "</targetSite>";

                    // Stack Trace of the error

                    strErrorMsg += Environment.NewLine + tab + "<stackTrace><![CDATA[" + Environment.NewLine + logException.InnerException.StackTrace + Environment.NewLine + tab + "]]></stackTrace>";
                    strErrorMsg += Environment.NewLine + "</innerException>";
                    //strErrorMsg += Environment.NewLine + tab + "<innerExceptionStackTrace><![CDATA[" + Environment.NewLine + logException.InnerException.StackTrace + Environment.NewLine + tab + "]]></innerExceptionStackTrace>";
                }

            }

            return strErrorMsg + "</error>";
        }

        private string BuildExceptionMessage(Exception x)
        {
            return BuildExceptionMessage(x, String.Empty, String.Empty, new NameValueCollection());
        }
    }
}
