using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.UI.Controllers
{
    [Filters.ReportsAuthorize]
    public class RequestAccessController : BaseController
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly BLL.Providers.IRequestAccessProvider RequestAccessProvider;
        private readonly Core.Logging.ILogger Logger;


        public RequestAccessController(Core.ErrorHandling.IErrorHandler errorHandler,
                               Core.Utilities.ISettings settings,
                               Core.Utilities.IUtilities utilities,
                               BLL.Providers.IRequestAccessProvider requestAccessProvider,
                               Core.Logging.ILogger logger)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.RequestAccessProvider = requestAccessProvider;
            this.Logger = logger;
        }

        public ActionResult Index()
        {
            if (Core.Utilities.Utilities.GetLoggedInUser().IsApproved)
            {
                return Redirect("/");
            }
            else
            {
                Core.Domain.Models.RequestAccessModel model = this.RequestAccessProvider.GetRequestAccessModel(Core.Utilities.Utilities.GetLoggedInUser().Id);

                return View(model);
            }
        }

        public ActionResult UpdateRequest()
        {
            Core.Domain.Models.UserRequest model = this.RequestAccessProvider.GetUserRequest(Core.Utilities.Utilities.GetLoggedInUser().Id);

            if (model == null)
                return Redirect("/requestaccess");

            return View(model);
        }

        public ActionResult ThankYou()
        {
            return View("UpdateRequestThankYou");
        }

        [HttpPost]
        public ActionResult UpdateRequest(Core.Domain.Models.UserRequest model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = Core.Domain.Enumerations.GeneralErrorMessage
            };

            try
            {
                Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                this.RequestAccessProvider.UpdateUserRequest(userId, model);

                result.Result = true;
                result.Message = String.Empty;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                result.Message = this.ErrorHandler.GetErrorMessage(ex);
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();

                this.ErrorHandler.HandleError(ex, String.Format("UI.{0}",
                                                        this.GetType().Name),
                                                        (new System.Diagnostics.StackFrame()).GetMethod().Name,
                                                        methodParam);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Submit(Boolean accessAskBI, Boolean accessCMOChat, Boolean accessSDRAI, Boolean accessChatGPI)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = Core.Domain.Enumerations.GeneralErrorMessage
            };

            try
            {
                Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                this.RequestAccessProvider.RequestAccess(userId, accessAskBI, accessCMOChat, accessSDRAI, accessChatGPI);

                result.Result = true;
                result.Message = String.Empty;
            }
            catch(Core.Domain.Exceptions.AccessRequestApprovedException ex)
            {
                result.Result = true;
                result.Message = ex.Message;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                result.Message = this.ErrorHandler.GetErrorMessage(ex);
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();

                this.ErrorHandler.HandleError(ex, String.Format("UI.{0}",
                                                        this.GetType().Name),
                                                        (new System.Diagnostics.StackFrame()).GetMethod().Name,
                                                        methodParam);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        [HttpGet]
        public ActionResult Requests()
        {
            Core.Domain.Models.AccessRequestsModel model = this.RequestAccessProvider.GetAccessRequestsModel();

            return View(model);
        }

        [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        [HttpGet]
        public ActionResult ChangeAccessLevel(Guid uniqueId)
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.ChangeAccessLevelModel model = this.RequestAccessProvider.GetChangeAccessLevelModel(userId, uniqueId);

            return PartialView("_ChangeAccessLevelPartial", model);
        }

        [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        [HttpPost]
        public ActionResult ChangeAccessLevel(Core.Domain.Models.ChangeAccessLevelModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = Core.Domain.Enumerations.GeneralErrorMessage
            };

            try
            {
                Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                Core.Domain.Models.AccessRequestModel accessRequest = this.RequestAccessProvider.ChangeAccessLevel(userId, model);

                result.UniqueId = model.UniqueId;
                result.Object = this.Utilities.RenderPartialViewToString(this, "_RequestsTrRowPartial", accessRequest);
                result.Result = true;
                result.Message = String.Empty;
            }
            catch (Core.Domain.Exceptions.AccessRequestApprovedException ex)
            {
                result.Result = true;
                result.Message = ex.Message;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                result.Message = this.ErrorHandler.GetErrorMessage(ex);
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();

                this.ErrorHandler.HandleError(ex, String.Format("UI.{0}",
                                                        this.GetType().Name),
                                                        (new System.Diagnostics.StackFrame()).GetMethod().Name,
                                                        methodParam);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Deny(Guid uniqueId)
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.DenyRequestModel model = this.RequestAccessProvider.GetDenyRequestModel(userId, uniqueId);

            return PartialView("_DenyRequestPartial", model);
        }

        [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        [HttpPost]
        public ActionResult Deny(Core.Domain.Models.DenyRequestModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = Core.Domain.Enumerations.GeneralErrorMessage
            };

            try
            {
                if (ModelState.IsValid)
                {
                    Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                    Core.Domain.Models.AccessRequestModel requestModel = this.RequestAccessProvider.DenyRequest(userId, model.UniqueId, model.Comment);

                    result.UniqueId = model.UniqueId;
                    result.Object = this.Utilities.RenderPartialViewToString(this, "_RequestsTrRowPartial", requestModel);
                    result.Result = true;
                    result.Message = String.Empty;
                }
                else
                {
                    result.Message = Core.Domain.Enumerations.InvalidFormErrorMessage;
                }
            }
            catch (Core.Domain.Exceptions.AccessRequestApprovedException ex)
            {
                result.Result = true;
                result.Message = ex.Message;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                result.Message = this.ErrorHandler.GetErrorMessage(ex);
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();

                this.ErrorHandler.HandleError(ex, String.Format("UI.{0}",
                                                        this.GetType().Name),
                                                        (new System.Diagnostics.StackFrame()).GetMethod().Name,
                                                        methodParam);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
