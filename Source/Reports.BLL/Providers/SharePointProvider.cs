using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SP = Microsoft.SharePoint.Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security;

namespace Reports.BLL.Providers
{
    public class SharePointProvider : ISharePointProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.IUsersRepository UsersRepo;
        private readonly Core.Logging.ILogger Logger;

        public SharePointProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                                  Core.Utilities.ISettings settings,
                                  Core.Utilities.IUtilities utilities,
                                  Data.Repositories.IUsersRepository usersRepo,
                                  Core.Logging.ILogger logger)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.UsersRepo = usersRepo;
            this.Logger = logger;
        }

        public void CreateTicket(String userEmail, String title, String source, String question, String answer, DateTime creationDateTime, String issueDescription)
        {

            SecureString password = new SecureString();
            password = GetPasswordsecure(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.SharepointPassword));

            try
            {
                using (SP.ClientContext clientContext = new SP.ClientContext(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.SharepointSite)))
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        bool validationResult = true;
                        return validationResult;
                    };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(CustomXertificateValidation);
                    clientContext.ExecutingWebRequest += new EventHandler<SP.WebRequestEventArgs>(Context_ExecutingWebRequest);
                    clientContext.RequestTimeout = -1;
                    // context.Credentials = CredentialCache.DefaultNetworkCredentials;
                    clientContext.Credentials = new SP.SharePointOnlineCredentials(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.SharepointUserName), password);

                    SP.List oList = clientContext.Web.Lists.GetById(Guid.Parse("7771ced3-1221-4865-b400-6ff6e7547c80"));
                    SP.ListItemCreationInformation listCreationInformation = new SP.ListItemCreationInformation();
                    SP.ListItem oListItem = oList.AddItem(listCreationInformation);

                    oListItem["Title"] = title;
                    oListItem["IssueDescription"] = issueDescription;
                    oListItem["Question"] = question;
                    oListItem["Answer"] = answer;
                    oListItem["SourceApp"] = source;
                    oListItem["QuestionDate"] = creationDateTime;
                    oListItem["DateReported"] = DateTime.UtcNow;
                    oListItem["Status"] = "New";
                    oListItem["SubmittedBy"] = userEmail;
                    oListItem.Update();
                    clientContext.Load(oListItem);
                    clientContext.ExecuteQuery();
                }
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userEmail", userEmail);
                methodParam.Add("title", title);
                methodParam.Add("source", source);
                methodParam.Add("question", question);
                methodParam.Add("answer", answer);
                methodParam.Add("creationDateTime", creationDateTime.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        private bool CustomXertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void Context_ExecutingWebRequest(object sender, SP.WebRequestEventArgs e)
        {
            IntPtr ptr = IntPtr.Zero;
            //X509Certificate2 certificate = null;
            //X509Certificate t = null;
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            // Nothing to do if no cert found.
            HttpWebRequest webReq = e.WebRequestExecutor.WebRequest;
            //webReq.Proxy = new WebProxy("http://[ProxyAddress]"); 
            //Specify a proxy address if you need to 
            // X509Certificate cert = pki.GetClientCertificate();
            String SPSerialno = "FF-0E-0F-00-00-00-66-3F-61-2A-31-DF-9B-9A-FF-0E-0F-00-16";
            foreach (X509Certificate c in store.Certificates)
            {
                byte[] cert = c.GetSerialNumber();
                String serialnumber = BitConverter.ToString(cert);
                if (serialnumber == SPSerialno)
                    webReq.ClientCertificates.Add(c);
            }
            ///************** Old Code ********************************//

            ////        HttpWebRequest webReq = e.WebRequestExecutor.WebRequest;
            ////            //webReq.Proxy = new WebProxy("http://[ProxyAddress]"); //Specify a proxy address if you need to    
            ////        X509Certificate cert = X509Certificate.CreateFromCertFile(@"‪‪C:\Jyoti\CAHPESharepoint.cer"); //Replace the certificate path to where you have exported the certificate.    
            ////        webReq.ClientCertificates.Add(cert);

            //    e.WebRequestExecutor.WebRequest.PreAuthenticate = true;

            //System.Net.WebProxy proxy = new System.Net.WebProxy("proxy.houston.hpecorp.net", 8080);

            //proxy.Credentials = new System.Net.NetworkCredential(); // System.Net.CredentialCache.DefaultCredentials; // new System.Net.NetworkCredential("jack_reacher", "<password>", "<domain>");
            //e.WebRequestExecutor.WebRequest.Proxy = proxy;
            ///******************************************************//
        }

        private SecureString GetPasswordsecure(string password)
        {
            SecureString securePassword = new SecureString();

            char[] arrPassword = password.ToCharArray();

            foreach (char c in arrPassword)
            {
                securePassword.AppendChar(c);
            }

            return securePassword;
        }
    }
}
