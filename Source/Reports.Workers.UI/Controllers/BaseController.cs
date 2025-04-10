using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Microsoft.Practices.ServiceLocation;

namespace Reports.Workers.UI.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Core.Utilities.ISettings settings = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.Utilities.ISettings>();

                if (settings.GetSettings(Core.Domain.Enumerations.AppSettings.MaintenanceMode) == "1")
                {
                    if (!Request.IsAjaxRequest())
                    {
                        String controller = this.ControllerContext.RouteData.Values["controller"].ToString().ToLower();
                        String action = this.ControllerContext.RouteData.Values["action"].ToString().ToLower();

                        if (controller != "home" || action != "maintenance")
                            filterContext.Result = new RedirectResult(Core.Utilities.Settings.GetServerEnvironment() == Core.Domain.Enumerations.Environment.QA ? "/accountinsights/maintenance" : "/maintenance");
                    }
                }
                else
                {
                    if (!Request.IsAjaxRequest())
                    {
                        Core.Utilities.IUtilities utilities = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.Utilities.IUtilities>();

                        String controller = this.ControllerContext.RouteData.Values["controller"].ToString().ToLower();
                        String action = this.ControllerContext.RouteData.Values["action"].ToString().ToLower();

                        Core.Domain.Models.LayoutModel layoutModel = new Core.Domain.Models.LayoutModel();

                        if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            layoutModel.UserFullName = String.Format("{0} {1}", Core.Utilities.Utilities.GetLoggedInUser().FirstName, Core.Utilities.Utilities.GetLoggedInUser().LastName);
                            layoutModel.UserId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                        }
                        else
                        {
                            layoutModel.UserFullName = String.Empty;
                            layoutModel.UserId = 0;

                        }

                        ViewBag.Layout = layoutModel;

                        if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            if (controller != "account" || action != "logout")
                            {
                                if (!Core.Utilities.Utilities.GetLoggedInUser().IsApproved)
                                {
                                    if (controller != "requestaccess")
                                        filterContext.Result = new RedirectResult("/requestaccess");
                                }
                            }

                            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));

                            Core.Domain.Models.WebSocketUserId notificationUserId = new Core.Domain.Models.WebSocketUserId()
                            {
                                UserId = Core.Utilities.Utilities.GetLoggedInUser().Id,
                                UserUniqueId = Core.Utilities.Utilities.GetLoggedInUser().UniqueId,
                                UniqueId = Guid.NewGuid()
                            };

                            layoutModel.WebSocketUserId = Convert.ToBase64String(utilities.ToByteArray(blowFish.Encrypt_ECB(utilities.Serialize(notificationUserId))));
                        }
                    }
                }

                base.OnActionExecuting(filterContext);
            }
            catch (Core.Domain.Exceptions.ItemNotFoundException ex)
            {
                throw new System.Web.HttpException(404, ex.Message);
            }
            catch (Exception ex)
            {
                Core.ErrorHandling.IErrorHandler errorHandler = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.ErrorHandling.IErrorHandler>();

                throw errorHandler.HandleError(ex, String.Format("UI.{0}",
                                                        this.GetType().Name),
                                                        (new System.Diagnostics.StackFrame()).GetMethod().Name,
                                                        null);
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            var error = filterContext.Exception;
            var code = 500;

            if (error.InnerException != null)
                code = (error.InnerException is HttpException) ? (error.InnerException as HttpException).GetHttpCode() : 500;
            else
                code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                filterContext.ExceptionHandled = true;

                try
                {
                    Core.Logging.ILogger logger = ServiceLocator.Current.GetInstance<Core.Logging.ILogger>();
                    Core.Email.IEmailService emailService = ServiceLocator.Current.GetInstance<Core.Email.IEmailService>();
                    Core.Utilities.ISettings settings = ServiceLocator.Current.GetInstance<Core.Utilities.ISettings>();

                    String errorMessage = logger.BuildExceptionMessage(error, "Application", "Application_Error", null);

                    errorMessage = errorMessage.Replace("<", "&lt;").Replace(">", "&gt;");

                    System.Collections.Hashtable body = new System.Collections.Hashtable();
                    body.Add("<%ENV%>", Core.Utilities.Settings.GetServerEnvironment());
                    body.Add("<%ERROR%>", errorMessage);

                    //emailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.Exception, settings.GetSettings(Core.Domain.Enumerations.AppSettings.ErrorEmail), String.Empty, null, body);

                    Core.ErrorHandling.IErrorHandler errorHandler = ServiceLocator.Current.GetInstance<Core.ErrorHandling.IErrorHandler>();
                    errorHandler.HandleError(error, "Application", "Application_Error", null);
                }
                catch (Exception ex)
                {
                    Core.ErrorHandling.IErrorHandler errorHandler = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.ErrorHandling.IErrorHandler>();
                    errorHandler.HandleError(ex, "Application", "Application_Error", null);
                }

                if (Request.IsAjaxRequest())
                {
                    Core.Domain.Models.JsonModel result = new Core.Domain.Models.JsonModel()
                    {
                        Result = false,
                        Message = error.Message
                    };

                    if (error.GetType() != typeof(Core.Domain.Exceptions.GeneralException))
                        result.Message = "An error happened during processing of your request. Please try again later.";

                    filterContext.Result = Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    filterContext.ExceptionHandled = false;
                    Core.ErrorHandling.IErrorHandler errorHandler = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Core.ErrorHandling.IErrorHandler>();
                    errorHandler.HandleError(filterContext.Exception, "Application", "Application_Error", null);
                    //filterContext.Result = RedirectToAction("Index", "Home");
                }
            }            
        }

    }
}
