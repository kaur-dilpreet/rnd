using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Practices.ServiceLocation;
using CommonServiceLocator.WindsorAdapter;
using System.Web.Optimization;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Script.Serialization;
using NHibernate;
using Reports.BLL.CustomAuthentication;

namespace Reports.Workers.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IWindsorContainer container;
        private static BLL.Workers.IExportWorker _exportWorker;
        private static BLL.Workers.IDataWorker _dataWorker;

        private static void BootstrapContainer()
        {
            container = new WindsorContainer().Install(FromAssembly.This());
            var controllerFactory = new Setup.WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(container));
        }

        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            BootstrapContainer();
            log4net.Config.XmlConfigurator.Configure();

            if (Core.Utilities.Settings.GetServerEnvironment() == Core.Domain.Enumerations.Environment.Production)
            {
                _exportWorker = ServiceLocator.Current.GetInstance<BLL.Workers.IExportWorker>();
                _exportWorker.Init();

                _dataWorker = ServiceLocator.Current.GetInstance<BLL.Workers.IDataWorker>();
                _dataWorker.Init();
            }

            Core.Utilities.ISettings settings = ServiceLocator.Current.GetInstance<Core.Utilities.ISettings>();

            String tempFolder = Core.Utilities.Utilities.MapPath(settings.GetSettings(Core.Domain.Enumerations.AppSettings.TempFilesPath));

            if (!System.IO.Directory.Exists(tempFolder))
            {
                if (tempFolder.EndsWith("\\"))
                    tempFolder = tempFolder.Substring(0, tempFolder.Length - 1);

                System.IO.Directory.CreateDirectory(tempFolder);
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {
                //Core.Email.IEmailService emailService = ServiceLocator.Current.GetInstance<Core.Email.IEmailService>();

                //String emailBody = String.Empty;
                //String emailSubject = String.Empty;

                //emailService.GetMessage(Core.Domain.Enumerations.EmailTemplates.Alert.ToString(), null, null, out emailSubject, out emailBody);

                if (!System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/content/") &&
                    !System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/scripts/") &&
                    !System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/favicon") &&
                    //!System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/home/keepsessionalive") &&
                    !System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/tempuploads/") &&
                    !System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/__browserlink/"))
                {
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie != null)
                    {
                        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        if (authTicket.UserData == "OAuth")
                            return;

                        CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);

                        BLL.Providers.IAccountProvider accountProvider = ServiceLocator.Current.GetInstance<BLL.Providers.IAccountProvider>();

                        Core.Domain.Entities.User user = accountProvider.GetUser(authTicket.Name, true);

                        if (System.Web.HttpContext.Current.Request.Path.ToLower().StartsWith("/home/keepsessionalive"))
                        {
                            if (HttpContext.Current.Request.Url.PathAndQuery.ToLower().Contains("timezoneoffset"))
                            {
                                Core.Utilities.IUtilities utilities = ServiceLocator.Current.GetInstance<Core.Utilities.IUtilities>();

                                String timezoneOffset = utilities.GetQueryParameter(HttpContext.Current.Request.Url, "timezoneOffset");

                                Int32 offset = 0;

                                if (Int32.TryParse(timezoneOffset, out offset))
                                {
                                    user.TimezoneOffset = -1 * offset;

                                    accountProvider.UpdateUserTimezone(user.Id, -1 * offset);
                                }
                            }
                        }

                        newUser.Id = user.Id;
                        newUser.UniqueId = user.UniqueId;
                        newUser.RoleId = user.RoleId;
                        newUser.Email = user.Email;
                        newUser.FirstName = user.FirstName;
                        newUser.LastName = user.LastName;
                        newUser.NTID = user.NTID;
                        newUser.UserCanExportAll = user.UserCanExportAll;
                        newUser.MARTAccess = user.MARTAccess;
                        newUser.AskBIAccess = user.AskBIAccess;
                        newUser.CMOChatAccess = user.CMOChatAccess;
                        newUser.SDRAIAccess = user.SDRAIAccess;
                        newUser.ChatGPIAccess = user.ChatGPIAccess;
                        newUser.ReportAccess = user.ReportAccess;
                        newUser.TimezoneOffset = user.TimezoneOffset;
                        newUser.IsApproved = user.IsApproved;

                        System.Web.HttpContext.Current.User = newUser;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.ErrorHandling.IErrorHandler errorHandler = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.ErrorHandling.IErrorHandler>();
                errorHandler.HandleError(ex, "Application", "Application_PostAuthenticateRequest", null);
            }
        }
    }
}
