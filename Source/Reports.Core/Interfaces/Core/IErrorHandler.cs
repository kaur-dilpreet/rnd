using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Reports.Core.ErrorHandling
{
    public interface IErrorHandler
    {
        Domain.Exceptions.GeneralException HandleError(Exception ex, String className, String methodName, NameValueCollection methodParams);
        String GetErrorMessage(Core.Domain.Exceptions.GeneralException ex);
    }
}
