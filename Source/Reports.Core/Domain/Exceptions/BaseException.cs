using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.Core.Domain.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException()
        {
        }

        public BaseException(String message)
            : base(message)
        {
        }

        public BaseException(String message,
          Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
