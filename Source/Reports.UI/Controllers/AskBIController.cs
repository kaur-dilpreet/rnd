using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Reports.UI.Controllers
{
    [Filters.ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_AskBI)]
    public class AskBIController : BaseController
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Logging.ILogger Logger;
        private readonly BLL.Providers.IGenAIProvider GenAIProvider;

        public AskBIController(Core.ErrorHandling.IErrorHandler errorHandler,
                               Core.Utilities.ISettings settings,
                               Core.Utilities.IUtilities utilities,
                               Core.Logging.ILogger logger,
                               BLL.Providers.IGenAIProvider genAIProvider)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.Logger = logger;
            this.GenAIProvider = genAIProvider;
        }

        public ActionResult Index()
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.AskBIModel model = this.GenAIProvider.GetAskBIModel(userId, 0);

            return View(model);
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

                Core.Domain.Models.AskBIModel model = this.GenAIProvider.GetAskBIModel(userId, skip);

                List<Core.Domain.Models.AskBIQuestion> questionModel = model.History;

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
        public ActionResult AskQuestion(Core.Domain.Models.AskBIModel model)
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

                Core.Domain.Models.GenAIAnswerModel answerModel = this.GenAIProvider.AskQuestion(userId, model.SessionId, Core.Domain.Enumerations.GenAIUsecases.AskBI, model.Question);

                result.Object = this.Utilities.RenderPartialViewToString(this, "_AnswerTrRowPartial", answerModel);
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
        public ActionResult CheckQuestion(Guid id)
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

                Core.Domain.Models.GenAIAnswerModel answerModel = this.GenAIProvider.CheckQuestion(userId, id);

                if (answerModel != null)
                {
                    result.Object = this.Utilities.RenderPartialViewToString(this, "_AnswerTrRowPartial", answerModel);
                }

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

                this.GenAIProvider.Feedback(userId, Core.Domain.Enumerations.GenAIUsecases.AskBI, uniqueId, isCorrect);

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
        public ActionResult CreateTicket(Guid questionId, String response)
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.SharePointTicketModel model = new Core.Domain.Models.SharePointTicketModel()
            {
                UniqueId = questionId,
                Controller = "AskBI",
                Response = response,
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

                this.GenAIProvider.CreateTicket(userId, Core.Domain.Enumerations.GenAIUsecases.AskBI, model);

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
    }
}
