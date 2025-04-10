using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.Core.Domain.Exceptions
{
    public class GeneralException : BaseException
    {
        public GeneralException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GeneralException(String message)
            : base(message)
        {
        }
    }

    public class InvalidParameterException : GeneralException
    {
        public InvalidParameterException(String message)
            : base(message)
        {
        }
    }

    public class ItemNotFoundException : GeneralException
    {
        public ItemNotFoundException(String item)
            : base(String.Format("{0} not found.", item))
        {
        }
    }

    public class WebException : GeneralException
    {
        public String Response;

        public WebException(String message, String response)
            : base(message)
        {
            this.Response = response;
        }
    }

    public class DeleteReferenceException : GeneralException
    {
        public DeleteReferenceException()
            : base("You cannot delete this item due to refrence constraint.")
        {
            
        }
    }

    public class PermissionDenied : GeneralException
    {
        public PermissionDenied()
            : base("Permission Denied.")
        {

        }
    }

    public class DuplicateValueException : GeneralException
    {
        public DuplicateValueException()
            : base("Cannot insert duplicate value.")
        {

        }
    }

    public class ProductAPIException : GeneralException
    {
        public ProductAPIException(String message)
            : base(message)
        {

        }
    }

    public class AlreadyHavePendingAccessRequestException : GeneralException
    {
        public AlreadyHavePendingAccessRequestException()
            : base("You already have a pending reuqest.")
        {

        }
    }

    public class AccessRequestApprovedException : GeneralException
    {
        public AccessRequestApprovedException()
            : base("Your access request has already been approved.")
        {

        }
    }

    public class RequestAlreadyApprovedException : GeneralException
    {
        public RequestAlreadyApprovedException()
            : base("This request has already been approved.")
        {

        }
    }

    public class RequestAlreadyDeniedException : GeneralException
    {
        public RequestAlreadyDeniedException()
            : base("This request has already been denied.")
        {

        }
    }

    public class CantDeleteTaskException : GeneralException
    {
        public CantDeleteTaskException()
            : base("The task cannot be deleted at this time.")
        {

        }
    }

    public class CantEditTaskException : GeneralException
    {
        public CantEditTaskException()
            : base("The task cannot be edited at this time.")
        {

        }
    }

    public class DelimiterShouldBeOneCharachterException : GeneralException
    {
        public DelimiterShouldBeOneCharachterException()
            : base("Delimiter should be one charachter. For tab, you may use \\t.")
        {

        }
    }

    public class TaskIsRunningCantRunAgainException : GeneralException
    {
        public TaskIsRunningCantRunAgainException()
            : base("The task is running. Please wait until the current execution is over, then run the task.")
        {

        }
    }

    public class MaxRowPerFileCantBeZeroException : GeneralException
    {
        public MaxRowPerFileCantBeZeroException()
            : base("When spliting the file, Max Rows Per File can't be 0.")
        {

        }
    }

    public class OrderByCantBeEmptyException : GeneralException
    {
        public OrderByCantBeEmptyException()
            : base("When spliting the file, Order By Can't be empty.")
        {

        }
    }

    public class ReportIsNotReadyException : GeneralException
    {
        public ReportIsNotReadyException()
            : base("Report is getting prepared. Please try again later.")
        {
        }
    }

    public class ExcelNotInstalledException : GeneralException
    {
        public ExcelNotInstalledException()
            : base("Excel is not properly installed.")
        {
        }
    }

    public class ReportNotReadyException : GeneralException
    {
        public ReportNotReadyException()
            : base("Report is not ready. Please try again in a few minutes.")
        {
        }
    }

    public class ReportIsSavingException : GeneralException
    {
        public ReportIsSavingException()
            : base("Report is being saved to the disk. Please try again in a few minutes.")
        {
        }
    }
}
