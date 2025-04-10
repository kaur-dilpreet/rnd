using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Reports.UI.Controllers
{
    [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_CMOChat)]
    public class CMOChatController : BaseController
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Logging.ILogger Logger;
        private readonly BLL.Providers.ICMOChatProvider CMOChatProvider;

        public CMOChatController(Core.ErrorHandling.IErrorHandler errorHandler,
                               Core.Utilities.ISettings settings,
                               Core.Utilities.IUtilities utilities,
                               Core.Logging.ILogger logger,
                               BLL.Providers.ICMOChatProvider askBIProvider)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.Logger = logger;
            this.CMOChatProvider = askBIProvider;
        }

        public ActionResult Index()
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.CMOChatModel model = this.CMOChatProvider.GetCMOChatModel(userId, 0);

            return View(model);
        }

        public ActionResult LoadData()
        {
            this.CMOChatProvider.LoadQuestionsData();

            return Redirect("cmochat");
        }

        public ActionResult LoadMore(Int32 skip)
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

                Core.Domain.Models.CMOChatModel model = this.CMOChatProvider.GetCMOChatModel(userId, skip);

                List<Core.Domain.Models.CMOChatQuestion> questionModel = model.History;

                result.Object = this.Utilities.RenderPartialViewToString(this, "_HistoryTrPartial", questionModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AskQuestion(Core.Domain.Models.CMOChatModel model)
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

                Core.Domain.Models.CMOChatQuestion questionModel = this.CMOChatProvider.AskQuestion(userId, model.Question, true, false);

                result.Object = this.Utilities.RenderPartialViewToString(this, "_HistoryTrRowPartial", questionModel);
                result.Result = true;
                result.Message = this.Utilities.RenderPartialViewToString(this, "_SuggestionsPartial", questionModel.Suggestions);
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

        [HttpPost]
        public ActionResult AskSuggestedQuestion(Core.Domain.Models.CMOChatModel model)
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

                Core.Domain.Models.CMOChatQuestion questionModel = this.CMOChatProvider.AskQuestion(userId, model.Question, true, false);

                result.Object = this.Utilities.RenderPartialViewToString(this, "_HistoryTrRowPartial", questionModel);
                result.Result = true;
                result.Message = this.Utilities.RenderPartialViewToString(this, "_SuggestionsPartial", questionModel.Suggestions);
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

        [HttpPost]
        public ActionResult AskPreDefinedQuestion(String question)
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

                Core.Domain.Models.CMOChatQuestion questionModel = this.CMOChatProvider.AskQuestion(userId, question, true, true);

                result.Object = this.Utilities.RenderPartialViewToString(this, "_HistoryTrRowPartial", questionModel);
                result.Result = true;
                result.Message = this.Utilities.RenderPartialViewToString(this, "_SuggestionsPartial", questionModel.Suggestions);
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

        [HttpGet]
        public ActionResult QuestionFeedback(Guid uniqueId, Boolean isCorrect)
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

                this.CMOChatProvider.SetQuestionIsCorrect(userId, uniqueId, isCorrect);

                result.UniqueId = uniqueId;
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

        [HttpGet]
        public ActionResult CreateTicket(Guid questionId)
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.SharePointTicketModel model = new Core.Domain.Models.SharePointTicketModel() {
                UniqueId = questionId,
                Controller = "CMOChat"
            };

            return PartialView("_CreateTicketPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicket(Core.Domain.Models.SharePointTicketModel model)
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

                this.CMOChatProvider.CreateTicket(userId, model);

                result.UniqueId = model.UniqueId;
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

        [HttpGet]
        public ActionResult ListQuestions(String question)
        {
            Core.Domain.Models.JsonListModel<String> result = new Core.Domain.Models.JsonListModel<String>()
            {
                Result = false,
                List = new List<String>(),
                Message = Core.Domain.Enumerations.GeneralErrorMessage
            };

            try
            {
                Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

                result.List = this.CMOChatProvider.ListQuestions(userId, question);

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
    }
}
