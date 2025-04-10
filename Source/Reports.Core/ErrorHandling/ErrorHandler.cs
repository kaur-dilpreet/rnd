using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Reports.Core.ErrorHandling
{
    public class ErrorHandler : IErrorHandler
    {
        private Logging.ILogger _logger;

        public ErrorHandler(Logging.ILogger logger)
        {
            this._logger = logger;
        }

        public Domain.Exceptions.GeneralException HandleError(Exception ex, String className, String methodName, NameValueCollection methodParams)
        {
            _logger.Error(ex, className, methodName, methodParams);

            if (ex.InnerException != null && ex.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException))
            {
                if (((System.Data.SqlClient.SqlException)ex.InnerException).Number == 2627 || ((System.Data.SqlClient.SqlException)ex.InnerException).Number == 2601)
                {
                    throw new Core.Domain.Exceptions.DuplicateValueException();
                }
            }

            if (ex.InnerException != null && ex.InnerException.GetType() == typeof(System.Data.SqlClient.SqlException))
            {
                if (((System.Data.SqlClient.SqlException)ex.InnerException).Number == 547)
                {
                    throw new Core.Domain.Exceptions.DeleteReferenceException();
                }
            }

            //if (ex.InnerException != null && ex.InnerException.GetType().FullName == "Vertica.Data.Internal.DotNetDSI.DSIException")
            //{
            //    throw new Core.Domain.Exceptions.VerticalDBConnectionException();
            //}

            return new Domain.Exceptions.GeneralException(ex.Message, ex);
        }

        public String GetErrorMessage(Core.Domain.Exceptions.GeneralException ex)
        {
            if (ex.GetType().BaseType == typeof(Domain.Exceptions.GeneralException) || ex.GetType() == typeof(Domain.Exceptions.GeneralException))
                return ex.Message;
            else
                return Core.Domain.Enumerations.GeneralErrorMessage;
        }
    }
}
