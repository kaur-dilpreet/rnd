using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Core.Domain
{
    public class Enumerations
    {
        public const String EmailAddressRegEx = @"^[a-zA-Z0-9._%+-\\$]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
        public const String EmailCommaSeparatedRegEx = @"^([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4},?)+$";
        public const String EmailSemicolonSeparatedRegEx = @"^([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4};?)+$";
        public const String PhoneNumberRegex = @"^(\([2-9]{1}\d{2}\){1}\d{3}-{1}\d{4})|([2-9]{1}\d{2}-{1}\d{3}-{1}\d{4})|([2-9]{1}\d{9})$"; // @" ^ ([2-9]{1}\d{2}-{1}\d{3}-{1}\d{4})$"; // @"^[\d]{10}$"; //@"^(\({1}[2-9]\d{2}\){1} \d{3} \- \d{4})|((?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,15})$";
        public const String PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,20}"; // @"^[\S].{6,20}$";
        public const String GUIDRegex = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";
        public const String FloatRegex = @"[-+]?(\d*[.])?\d+";
        public const String ZipCodeRegex = @"^\d{5}$";
        public const String CVVRegex = @"^\d{3,4}$";
        public const String DigitRegex = @"^\d+$";
        public const String NumberRegex = @"^[0-9.]+$";
        public const String HashtagRegex = @"^(#\w+)( #\w+)*$";
        public const String IPAddressRegex = @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";
        public const String IPAddressesRegex = @"^((?:[0-9]{1,3}\.){3}[0-9]{1,3})(\n(?:[0-9]{1,3}\.){3}[0-9]{1,3})*$";
        public const String DomainRegex = @"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}*$";
        public const String DomainsRegex = @"^(([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,})(\n([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,})*$";
        public const String WebsiteRegex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})$";
        public const String UrlRegex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        public const String TitleRegex = @"^[a-zA-Z0-9 _\-|]*$";
        public const String NTIdRegex = @"^[a-zA-Z]+[:][a-zA-Z0-9-_]+$";
        public const String TimeRegex = @"^([0-1][0-9]|[2][0-3]):([0-5][0-9])$";
        public const String UrlNoHttpRegex = @"^([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
        public const String WebScraperNodesRegex = @"^(((([a-zA-Z0-9\-_])+=([a-zA-Z0-9\-_()#.])+(\r\n)?\s?)+)*)$";
        public const String URLNoHttpWithPortRegex = @"^([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*(?::\d+)?$";

        public const Int32 MARTMaxRowsPerPage = 1000;
        public const Int32 MaxPageNumbers = 10;

        public const String Role_Admin = "role_admin";
        public const String Role_PowerUser = "role_poweruser";
        public const String Role_User = "role_user";
        public const String Role_Admin_PowerUser = "role_admin, role_poweruser";
        public const String Role_AskBI = "role_askbi";
        public const String Role_SDRAI = "role_SDRAI";
        public const String Role_CMOChat = "role_cmochat";
        public const String Role_ChatGPI = "role_chatgpi";

        public const Int32 MaxPageNumbersToShow = 10;
        public const Int32 ItemsPerPage = 100;

        public const String PDFFolder = "~/Files";
        public const String GeneralErrorMessage = "An error happened during processing of your request. Please try again later";
        public const String InvalidFormErrorMessage = "One or more fields are invalid. Please try again.";
        public const String SharedFolderLocationRegex = @"^(\\)(\\[A-Za-z0-9-_.\s]+){2,}(\\?)$";
        public const String SourceFileRegex = @"(ABC:) * (\([a - zA - Z]+\).+)";
        public enum AppSettings
        {
            SMTPPassword,
            EmailTemplatePath,
            SMTPSSL,
            SMTPPort,
            SMTPUserName,
            SMTPHost,
            SharepointUserName,
            SharepointPassword,
            SharepointSite,
            XHeaderValue,
            DefaultFromAddress,
            DefaultFromName,
            AbsoluteExpirationDuration,
            HashKey,
            VerticaHost,
            VerticaDB,
            VerticaUsername,
            VerticaPassword,
            AuthCookieName,
            MaintenanceMode,
            BaseUrl,
            ExportPath,
            TempFilesPath,
            AssetsFilesPath,
            ErrorEmail,
            CMOCHATToken,
            ASKBIToken,
            SDRAIToken,
            ChatGPIToken,
            ChatGPIFolder,
            AskBIUrl,
            ChatGPIUrl,
            CMOChatUrl,
            SDRAIUrl,
            GenAIToken,
            GenAIUrl
        }

        public enum UserRoles
        {
            Admin = 1,
            PowerUser = 2,
            User = 3
        }

        public enum EmailTemplates
        {
            AccessApproved,
            AccessDenied,
            AccessChanged,
            AccessRequestSubmitted,
            AccessRequestAlert,
            ExportComplete,
            ExportFailed,
            Exception
        }

        public enum DependencyType
        {
            None,
            File,
            CacheKey
        }

        public enum Environment
        {
            Dev,
            QA,
            ITG,
            Production
        }

        public enum RequestMethod
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH,
            OPTIONS
        }

        public enum GenAIUsecases
        {
            AskBI = 1,
            CMOChat = 2
        }

        public enum QuestionStatuses
        {
            InProgress = 1,
            Complete = 2,
            Error = 3
        }
    }
}
