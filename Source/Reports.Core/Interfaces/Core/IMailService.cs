using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net.Mail;
using Reports.Core.Domain;

namespace Reports.Core.Email
{
    public interface IEmailService
    {
        Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, Hashtable subjectVariableTable, Hashtable bodyVariableTable);
        Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, Hashtable subjectVariableTable, Hashtable bodyVariableTable, MailPriority priority, Boolean isHTML);
        Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, String fromAddress, String fromName, Hashtable subjectVariableTable, Hashtable bodyVariableTable);
        Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, String fromAddress, String fromName, Hashtable subjectVariableTable, Hashtable bodyVariableTable, MailPriority priority, Boolean isHTML);
        Boolean SendEmail(String subject, String message, String toAddress, String ccAddress);
        Boolean SendEmail(String subject, String messageBody, String toAddress, String ccAddress, String fromAddress, String fromName);
        Boolean SendEmail(String subject, String messageBody, String toAddress, String ccAddress, String fromAddress, String fromName, MailPriority priority, Boolean isHTML);
        void GetMessage(String templateName, Hashtable subjectVariableTable, Hashtable bodyVariableTable, out String subject, out String messageBody);
    }
}
