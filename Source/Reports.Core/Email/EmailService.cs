using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Mail;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using Reports.Core.Domain;

namespace Reports.Core.Email
{
    public class EmailService : IEmailService
    {
        private readonly String VARIABLE_PREFIX = String.Empty;//"<%%";
        private readonly String VARIABLE_SUFFIX = String.Empty;//"%%>";

        private readonly Caching.IDataCache DataCache;
        private readonly Logging.ILogger Logger;
        private readonly ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Utilities.ISettings Settings; 

        public EmailService(Caching.IDataCache dataCache, 
                           Logging.ILogger logger,
                           ErrorHandling.IErrorHandler errorHandler,
                           Utilities.ISettings settings)
        {
            DataCache = dataCache;
            Logger = logger;
            ErrorHandler = errorHandler;
            Settings = settings;
        }

        #region SendEmail
        /// <summary>
        /// Used to send an email with an existing subject and body template.  The location of the template is specified in the
        /// config file.  There should be a subject template (ex. mytemplate_subject.txt) and body template (ex. mytemplate_body.html).
        /// When calling the SendEmail with the templates, the variables are case sensitive. 
        /// 
        /// Note: the from name and from email address is specified in the config file
        /// 
        /// 
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     System.Net.Mail.MailMessage.From is null.-or-System.Net.Mail.MailMessage.To
        ///     is null.-or- message is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     There are no recipients in System.Net.Mail.MailMessage.To, System.Net.Mail.MailMessage.CC,
        ///     and System.Net.Mail.MailMessage.BCC.
        ///
        ///   System.InvalidOperationException:
        ///     This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
        ///     call in progress.-or- System.Net.Mail.SmtpClient.Host is null.-or-System.Net.Mail.SmtpClient.Host
        ///     is equal to the empty String ("").-or- System.Net.Mail.SmtpClient.Port is
        ///     zero.
        ///
        ///   System.ObjectDisposedException:
        ///     This object has been disposed.
        ///
        ///   System.Net.Mail.SmtpException:
        ///     The connection to the SMTP server failed.-or-Authentication failed.-or-The
        ///     operation timed out.
        ///
        ///   System.Net.Mail.SmtpFailedRecipientsException:
        ///     The message could not be delivered to one or more of the recipients in System.Net.Mail.MailMessage.To,
        ///     System.Net.Mail.MailMessage.CC, or System.Net.Mail.MailMessage.BCC.
        ///     
        /// </summary>
        /// <param name="templateName">The name of the template to use.  There needs to be a [templatename]_body.html and 
        /// [templatename]_subject.txt for each template
        /// </param>
        /// <param name="toAddress">the email address of the receipient</param>
        /// <param name="subjectVariableTable">The hashtable of key-value pairs of each of the subject variables.  
        /// Note: The varaible names are case sensitive.  Null is a valid parameter if the template does not take any variables</param>
        /// <param name="bodyVariableTable">The hashtable of key-value pairs of each of the body variables.  
        /// Note: The varaible names are case sensitive.  Null is a valid parameter if the template does not take any variables</param>
        /// <returns>true if successful, false otherwise</returns>        
        public Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, Hashtable subjectVariableTable, Hashtable bodyVariableTable)
        {
            return this.SendEmail(emailTemplate, toAddress, ccAddress, this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromAddress), this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromName), subjectVariableTable, bodyVariableTable);
        }

        /// <summary>
        /// Used to send an email with an existing subject and body template.  The location of the template is specified in the
        /// config file.  There should be a subject template (ex. mytemplate_subject.txt) and body template (ex. mytemplate_body.html).
        /// When calling the SendEmail with the templates, the variables are case sensitive. 
        /// 
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     System.Net.Mail.MailMessage.From is null.-or-System.Net.Mail.MailMessage.To
        ///     is null.-or- message is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     There are no recipients in System.Net.Mail.MailMessage.To, System.Net.Mail.MailMessage.CC,
        ///     and System.Net.Mail.MailMessage.BCC.
        ///
        ///   System.InvalidOperationException:
        ///     This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
        ///     call in progress.-or- System.Net.Mail.SmtpClient.Host is null.-or-System.Net.Mail.SmtpClient.Host
        ///     is equal to the empty String ("").-or- System.Net.Mail.SmtpClient.Port is
        ///     zero.
        ///
        ///   System.ObjectDisposedException:
        ///     This object has been disposed.
        ///
        ///   System.Net.Mail.SmtpException:
        ///     The connection to the SMTP server failed.-or-Authentication failed.-or-The
        ///     operation timed out.
        ///
        ///   System.Net.Mail.SmtpFailedRecipientsException:
        ///     The message could not be delivered to one or more of the recipients in System.Net.Mail.MailMessage.To,
        ///     System.Net.Mail.MailMessage.CC, or System.Net.Mail.MailMessage.BCC.
        ///     
        /// </summary>
        /// <param name="templateName">The name of the template to use.  There needs to be a [templatename]_body.html and 
        /// [templatename]_subject.txt for each template
        /// </param>
        /// <param name="toAddress">the email address of the receipient</param>
        /// <param name="fromAddress">the email address of the sender</param>
        /// <param name="fromName">the display name of the sender</param>
        /// <param name="subjectVariableTable">The hashtable of key-value pairs of each of the subject variables.  
        /// Note: The varaible names are case sensitive.  Null is a valid parameter if the template does not take any variables</param>
        /// <param name="bodyVariableTable">The hashtable of key-value pairs of each of the body variables.  
        /// Note: The varaible names are case sensitive.  Null is a valid parameter if the template does not take any variables</param>
        /// <returns>true if successful, false otherwise</returns>        
        public Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, String fromAddress, String fromName, Hashtable subjectVariableTable, Hashtable bodyVariableTable)
        {
            return SendEmail(emailTemplate, toAddress, ccAddress, fromAddress, fromName, subjectVariableTable, bodyVariableTable, MailPriority.Normal, true);
        }

        public Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, Hashtable subjectVariableTable, Hashtable bodyVariableTable, MailPriority priority, Boolean isHTML)
        {
            return this.SendEmail(emailTemplate, toAddress, ccAddress, this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromAddress), this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromName), subjectVariableTable, bodyVariableTable, priority, isHTML);
        }

        public Boolean SendEmail(Enumerations.EmailTemplates emailTemplate, String toAddress, String ccAddress, String fromAddress, String fromName, Hashtable subjectVariableTable, Hashtable bodyVariableTable, MailPriority priority, Boolean isHTML)
        {
            String subject = String.Empty;
            String messageBody = String.Empty;
            GetMessage(emailTemplate.ToString(), subjectVariableTable, bodyVariableTable, out subject, out messageBody);
            //System.Diagnostics.Debug.WriteLine(String.Format("Sending email to {0}: {1}", toAddress, messageBody));
            return this.SendEmail(subject, messageBody, toAddress,ccAddress, fromAddress, fromName, priority, isHTML);
        }

        /// <summary>
        /// Sends an email to the target toAddress.
        /// Note the from name and from email address is specified in the config file
        /// 
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     System.Net.Mail.MailMessage.From is null.-or-System.Net.Mail.MailMessage.To
        ///     is null.-or- message is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     There are no recipients in System.Net.Mail.MailMessage.To, System.Net.Mail.MailMessage.CC,
        ///     and System.Net.Mail.MailMessage.BCC.
        ///
        ///   System.InvalidOperationException:
        ///     This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
        ///     call in progress.-or- System.Net.Mail.SmtpClient.Host is null.-or-System.Net.Mail.SmtpClient.Host
        ///     is equal to the empty String ("").-or- System.Net.Mail.SmtpClient.Port is
        ///     zero.
        ///
        ///   System.ObjectDisposedException:
        ///     This object has been disposed.
        ///
        ///   System.Net.Mail.SmtpException:
        ///     The connection to the SMTP server failed.-or-Authentication failed.-or-The
        ///     operation timed out.
        ///
        ///   System.Net.Mail.SmtpFailedRecipientsException:
        ///     The message could not be delivered to one or more of the recipients in System.Net.Mail.MailMessage.To,
        ///     System.Net.Mail.MailMessage.CC, or System.Net.Mail.MailMessage.BCC.
        ///     
        /// </summary>
        /// <param name="subject">The subject of the email</param>
        /// <param name="message">The body of the email</param>        
        /// <param name="toAddress">the email address of the receipient</param>
        /// <returns>true if successful, false otherwise</returns>        
        public Boolean SendEmail(String subject, String message, String toAddress, String ccAddress)
        {
            return this.SendEmail(subject, message, toAddress, ccAddress, this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromAddress), this.Settings.GetSettings(Enumerations.AppSettings.DefaultFromName));
        }

        /// <summary>
        /// Sends an email to the target toAddress.
        ///  
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     System.Net.Mail.MailMessage.From is null.-or-System.Net.Mail.MailMessage.To
        ///     is null.-or- message is null.
        ///
        ///   System.ArgumentOutOfRangeException:
        ///     There are no recipients in System.Net.Mail.MailMessage.To, System.Net.Mail.MailMessage.CC,
        ///     and System.Net.Mail.MailMessage.BCC.
        ///
        ///   System.InvalidOperationException:
        ///     This System.Net.Mail.SmtpClient has a Overload:System.Net.Mail.SmtpClient.SendAsync
        ///     call in progress.-or- System.Net.Mail.SmtpClient.Host is null.-or-System.Net.Mail.SmtpClient.Host
        ///     is equal to the empty String ("").-or- System.Net.Mail.SmtpClient.Port is
        ///     zero.
        ///
        ///   System.ObjectDisposedException:
        ///     This object has been disposed.
        ///
        ///   System.Net.Mail.SmtpException:
        ///     The connection to the SMTP server failed.-or-Authentication failed.-or-The
        ///     operation timed out.
        ///
        ///   System.Net.Mail.SmtpFailedRecipientsException:
        ///     The message could not be delivered to one or more of the recipients in System.Net.Mail.MailMessage.To,
        ///     System.Net.Mail.MailMessage.CC, or System.Net.Mail.MailMessage.BCC.
        ///     
        /// </summary>
        /// <param name="subject">The subject of the email</param>
        /// <param name="messageBody">The body of the email</param>        
        /// <param name="toAddress">the email address of the receipient</param>
        /// <param name="fromAddress">the email address of the sender</param>
        /// <param name="fromName">the display name of the sender</param>
        /// <returns>true if successful, false otherwise</returns>        
        /// 
        public Boolean SendEmail(String subject, String messageBody, String toAddress, String ccAddress, String fromAddress, String fromName)
        {
            return SendEmail(subject, messageBody, toAddress, ccAddress, fromAddress, fromName, MailPriority.Normal, true);
        }

        public Boolean SendEmail(String subject, String messageBody, String toAddress, String ccAddress, String fromAddress, String fromName, MailPriority priority, Boolean isHTML)
        {
            subject = subject.Replace('\r', ' ').Replace('\n', ' '); 
            Boolean result = false;

            if (String.IsNullOrEmpty(toAddress)) return result;

            SmtpClient smtpClient = new SmtpClient(this.Settings.GetSettings(Enumerations.AppSettings.SMTPHost), Int32.Parse(this.Settings.GetSettings(Enumerations.AppSettings.SMTPPort)));
            smtpClient.EnableSsl = Boolean.Parse(this.Settings.GetSettings(Enumerations.AppSettings.SMTPSSL));

            if (this.Settings.GetSettings(Enumerations.AppSettings.SMTPUserName) != String.Empty)
                smtpClient.Credentials = new System.Net.NetworkCredential(this.Settings.GetSettings(Enumerations.AppSettings.SMTPUserName), this.Settings.GetSettings(Enumerations.AppSettings.SMTPPassword));

            MailMessage message = new MailMessage();
            message.IsBodyHtml = isHTML;
            message.Subject = subject;
            message.Body = messageBody;
            message.Priority = priority;
            String[] toMultipleAddresses = toAddress.Split(new char[] { ';', ',' });
            

            foreach (String emailAddress in toMultipleAddresses)
            {
                if (emailAddress.Trim() != "")
                    message.To.Add(new MailAddress(emailAddress));
            }

            if (!String.IsNullOrEmpty(ccAddress))
            {
                String[] ccMultipleAddresses = ccAddress.Split(new char[] { ';', ',' });
                foreach (String emailAddress in ccMultipleAddresses)
                {
                    if (emailAddress.Trim() != "")
                        message.CC.Add(new MailAddress(emailAddress));
                }
            }
            if (!String.IsNullOrEmpty(fromAddress)) message.From = new MailAddress(fromAddress, fromName);
            //message.Headers.Add("X-SP-Transact-Id", this.Settings.GetSettings(Enumerations.AppSettings.XHeaderValue)); //Needed for SilverPop to identify sender

            try
            {
                smtpClient.Send(message);
                result = true;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                
                methodParam.Add("subject", subject);
                methodParam.Add("messageBody", messageBody);
                methodParam.Add("toAddress", toAddress);
                methodParam.Add("fromAddress", fromAddress);
                methodParam.Add("fromName", fromName);
                methodParam.Add("priority", priority.ToString());
                methodParam.Add("isHTML", isHTML.ToString());

                //this.ErrorHandler.HandleError(ex, String.Format("Core.EmailService.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
                result = false;
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Helper function to return the contents of the email
        /// </summary>
        /// <param name="templateName">template to use.  The template path should be specified in the config file</param>
        /// <param name="subjectVariableTable">variable table that contains name-value pairs of the replacing variable values</param>
        /// <param name="bodyVariableTable">variable table that contains name-value pairs of the replacing variable values</param>
        /// <param name="subject">The subject line with the replaced variable values</param>
        /// <param name="messageBody">Message body with the replaced variable values</param>
        public void GetMessage(String templateName, Hashtable subjectVariableTable, Hashtable bodyVariableTable, out String subject, out String messageBody)
        {
            subject = this.GetSubject(templateName, subjectVariableTable);
            messageBody = this.GetBody(templateName, bodyVariableTable);
        }

        /// <summary>
        /// Helper function to return the contents of the email subject
        /// </summary>
        /// <param name="templateName">template to use.  The template path should be specified in the config file</param>
        /// <param name="subjectVariableTable">variable table that contains name-value pairs of the replacing variable values</param>
        /// <returns>The subject line with the replaced variable values</returns>
        private String GetSubject(String templateName, System.Collections.Hashtable subjectVariableTable)
        {
            String rawSubjectName = templateName + "_subject.txt";

            //Check to see if the subject exists in the data cache
            String subjectLine = this.DataCache.GetItem<String>(rawSubjectName);

            //If not read it from the file and store it in the data cache
            if (subjectLine == null)
            {
                subjectLine = System.IO.File.ReadAllText(Core.Utilities.Utilities.MapPath(this.Settings.GetSettings(Enumerations.AppSettings.EmailTemplatePath) + "\\" + rawSubjectName));
                
                this.DataCache.Add(rawSubjectName, subjectLine, Enumerations.DependencyType.File, Core.Utilities.Utilities.MapPath(this.Settings.GetSettings(Enumerations.AppSettings.EmailTemplatePath) + "\\" + rawSubjectName));
            }

            //Replace the variables in the email template with those set in the variable table
            if (subjectVariableTable != null)
            {

                foreach (String name in subjectVariableTable.Keys)
                {
                    String replacement = VARIABLE_PREFIX + name + VARIABLE_SUFFIX;
                    subjectLine = subjectLine.Replace(replacement, subjectVariableTable[name].ToString());
                }
            }
            return subjectLine;
        }

        /// <summary>
        /// Helper function to return the contents of the email body 
        /// </summary>
        /// <param name="templateName">template to use.  The template path should be specified in the config file</param>
        /// <param name="bodyVariableTable">variable table that contains name-value pairs of the replacing variable values</param>
        /// <returns>Message body with the replaced variable values</returns>
        private String GetBody(String templateName, System.Collections.Hashtable bodyVariableTable)
        {
            String rawMessageBodyName = templateName + "_body.html";

            String messageBody = this.GetEmailBody(rawMessageBodyName);

            if (messageBody.Contains("<%%HASMASTER%%>"))
            {
                String masterName = "Master.html";

                String title = "SociaLiner<sup>&trade;</sup>";
                Int32 startIndex = 0;
                Int32 endIndex = 0;

                if (messageBody.Contains("<%%MASTER="))
                {
                    startIndex = messageBody.IndexOf("<%%MASTER=") + 10;
                    endIndex = messageBody.IndexOf("%%>", startIndex);

                    masterName = messageBody.Substring(startIndex, endIndex - startIndex);
                }

                if (messageBody.Contains("<%%TITLE="))
                {
                    startIndex = messageBody.IndexOf("<%%TITLE=") + 9;
                    endIndex = messageBody.IndexOf("%%>", startIndex);

                    title = messageBody.Substring(startIndex, endIndex - startIndex);
                }

                String masterBody = this.GetEmailBody(masterName);

                if (masterBody.Contains("<%%HASMASTER%%>"))
                {
                    String secondaryMasterName = "Master.html";

                    startIndex = masterBody.IndexOf("<%%MASTER=") + 10;
                    endIndex = masterBody.IndexOf("%%>", startIndex);

                    secondaryMasterName = masterBody.Substring(startIndex, endIndex - startIndex);

                    String secondaryMasterBody = this.GetEmailBody(secondaryMasterName);

                    String placeHolder1 = String.Empty;

                    if (masterBody.Contains("<%%PLACEHOLDER1%%>"))
                    {
                        startIndex = masterBody.IndexOf("<%%PLACEHOLDER1%%>") + 18;
                        endIndex = masterBody.IndexOf("<%%ENDPLACEHOLDER1%%>");
                        placeHolder1 = masterBody.Substring(startIndex, endIndex - startIndex);
                    }

                    masterBody = secondaryMasterBody.Replace("<%%PLACEHOLDER1%%>", placeHolder1);
                }

                //In case it didn't have secondary master
                masterBody = masterBody.Replace("<%%PLACEHOLDER1%%>", String.Empty);

                messageBody = messageBody.Substring(messageBody.IndexOf("<%%ENDHEADER%%>") + 15);

                messageBody = masterBody.Replace("<%%BODY%%>", messageBody).Replace("<%%TITLE%%>", title);
            }

            //Replace the variables in the email template with those set in the variable table
            if (bodyVariableTable != null)
            {
                foreach (String name in bodyVariableTable.Keys)
                {
                    String replacement = VARIABLE_PREFIX + name + VARIABLE_SUFFIX;
                    messageBody = messageBody.Replace(replacement, bodyVariableTable[name].ToString());
                }
            }

            return messageBody;
        }

        private String GetEmailBody(String fileName)
        {
            String emailBody = this.DataCache.GetItem<String>(fileName);

            //If not read it from the file and store it in the data cache
            if (emailBody == null)
            {
                emailBody = System.IO.File.ReadAllText(Core.Utilities.Utilities.MapPath(this.Settings.GetSettings(Enumerations.AppSettings.EmailTemplatePath) + "\\" + fileName));

                this.DataCache.Add(fileName, emailBody, Enumerations.DependencyType.File, Core.Utilities.Utilities.MapPath(this.Settings.GetSettings(Enumerations.AppSettings.EmailTemplatePath) + "\\" + fileName));
            }

            return emailBody;
        }
    }
}
