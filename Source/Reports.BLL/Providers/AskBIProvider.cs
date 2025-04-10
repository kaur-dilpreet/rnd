using Microsoft.SharePoint.Portal.WebControls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public class AskBIProvider : IAskBIProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.IAskBIHistoryRepository AskBIHistoryRepo;
        private readonly Data.Repositories.IUsersRepository UsersRepo;
        private readonly Data.IAPIHelper APIHelper;
        private readonly ISharePointProvider SharePointProvider;
        private readonly Core.Logging.ILogger Logger;
        private readonly NHibernate.ISessionFactory SessionFactory;

        public AskBIProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                             Core.Utilities.ISettings settings,
                             Core.Utilities.IUtilities utilities,
                             Data.Repositories.IAskBIHistoryRepository AskBIHistoryRepo,
                             Data.Repositories.IUsersRepository usersRepo,
                             Data.IAPIHelper apiHelper,
                             ISharePointProvider sharePointProvider,
                             Core.Logging.ILogger logger,
                             NHibernate.ISessionFactory sessionFactory)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.AskBIHistoryRepo = AskBIHistoryRepo;
            this.UsersRepo = usersRepo;
            this.APIHelper = apiHelper;
            this.SharePointProvider = sharePointProvider;
            this.Logger = logger;
            this.SessionFactory = sessionFactory;
        }

        public Core.Domain.Models.AskBIModel GetAskBIModel(Int64 userId, Int32 skip)
        {
            try
            {
                Core.Domain.Models.AskBIModel model = new Core.Domain.Models.AskBIModel();

                model.History = this.AskBIHistoryRepo.GetAll().Where(x => x.CreatedBy == userId).OrderBy(x => x.TableCategory).ThenByDescending(x => x.CreationDateTime).ToList().GroupBy(h => new { h.Question, h.TableCategory }).Skip(skip).Take(100).Select(x => new Core.Domain.Models.AskBIQuestion()
                {
                    Question = x.Key.Question,
                }).ToList();

                NameValueCollection header = new NameValueCollection();

                Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
                String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIToken));

                header.Add("Authorization", String.Format("Bearer {0}", token));

                String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIUrl);

                String response = this.APIHelper.GetResponse(String.Format("{0}/get_version", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, null, "application/json");

                this.Logger.Info(response);

                model.Version = this.Utilities.Deserialize<Core.Domain.Models.AskBIVersionModel>(response);

                if (model.Version.TableCategories == null || model.Version.DataUntilDates == null)
                    model.ErrorMessage = model.Version.Message;

                return model;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("DABLLTA.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.AskBIQuestion AskQuestion(Int64 userId, Guid sessionId, Core.Domain.Enumerations.GenAIUsecases usecase, String question, String tableCategory, Boolean displaySQL)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                Guid questionId = Guid.NewGuid();

                Core.Domain.Models.AskBIQuestionModel questionModel = new Core.Domain.Models.AskBIQuestionModel()
                {
                    UserId = user.NTID,
                    Question = question.Trim().ToLower(),
                    Usecase = usecase.ToString().ToLower(),
                    SessionId = sessionId.ToString()
                };

                while (questionModel.Question.Contains("  "))
                    questionModel.Question = questionModel.Question.Replace("  ", " ");

                NameValueCollection header = new NameValueCollection();

                Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
                String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIToken));

                header.Add("Authorization", String.Format("Bearer {0}", token));

                String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIUrl);

                String response = this.APIHelper.GetResponse(String.Format("{0}/ask", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, this.Utilities.Serialize(questionModel), "application/json");

                this.Logger.Info(response);

                Core.Domain.Models.AskBIQuestionResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.AskBIQuestionResponse>(response);

                Core.Domain.Models.AskBIQuestion answer = new Core.Domain.Models.AskBIQuestion();

                if (responseModel != null)
                {
                    Core.Domain.Entities.AskBIHistory history = new Core.Domain.Entities.AskBIHistory()
                    {
                        UniqueId = questionId,
                        Question = question.Trim(),
                        TableCategory = tableCategory,
                        Answer = String.IsNullOrEmpty(responseModel.answer) ? String.Empty : responseModel.answer,
                        SQLQuery = responseModel.status == "complete" ? String.IsNullOrEmpty(responseModel.sql) ? String.Empty : responseModel.sql : String.Empty,
                        Data = responseModel.status == "complete" ? this.Utilities.Serialize(responseModel.data) : String.Empty,
                        CreatedBy = userId,
                        CreationDateTime = DateTime.UtcNow,
                        Message = responseModel.status == "complete" ? String.IsNullOrEmpty(responseModel.message) ? String.Empty : responseModel.message : String.Empty,
                        QuestionId = responseModel.qid.HasValue ? responseModel.qid.Value : 0,
                        TicketOpened = false,
                        Status = responseModel.status == "complete" ? (Byte)Core.Domain.Enumerations.QuestionStatuses.Complete : (Byte)Core.Domain.Enumerations.QuestionStatuses.InProgress,
                        SessionId = sessionId
                    };

                    this.AskBIHistoryRepo.SaveOrUpdate(history);

                    //if (responseModel.status == "complete")
                    //{
                        answer = new Core.Domain.Models.AskBIQuestion()
                        {
                            UniqueId = history.UniqueId,
                            Question = history.Question,
                            Answer = history.Answer,
                            SQL = history.SQLQuery,
                            Message = history.Message,
                            Data = history.Data,
                            IsCorrect = null,
                            TicketOpened = false,
                            Status = responseModel.status,
                            CreationDateTime = history.CreationDateTime
                        };
                    //}
                    //else
                    //{
                    //    answer = new Core.Domain.Models.AskBIQuestion()
                    //    {
                    //        UniqueId = history.UniqueId,
                    //        Question = question.Trim(),
                    //        Answer = responseModel.answer,
                    //        SQL = String.Empty,
                    //        Message = String.Empty,
                    //        TableCategory = tableCategory,
                    //        Data = String.Empty,
                    //        IsCorrect = null,
                    //        TicketOpened = false,
                    //        Status = responseModel.status,
                    //        CreationDateTime = DateTime.UtcNow
                    //    };
                    //}
                }

                return answer;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("DABLLTA.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.AskBIQuestion CheckQuestion(Int64 userId, Guid questionId)
        {
            try
            {
                var history = this.AskBIHistoryRepo.GetAll().Where(a => a.CreatedBy == userId && a.UniqueId == questionId).FirstOrDefault();

                if (history == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Question");

                String question = history.Question.ToLower().Trim();

                while (question.Contains("  "))
                    question = question.Replace("  ", " ");

                String response = RedisProvider.GetCache(question);

                if (!String.IsNullOrEmpty(response))
                {
                    this.Logger.Info(response);

                    Core.Domain.Models.AskBIQuestionResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.AskBIQuestionResponse>(response);

                    Core.Domain.Models.AskBIQuestion answer = new Core.Domain.Models.AskBIQuestion();

                    if (responseModel != null)
                    {
                        history.Answer = String.IsNullOrEmpty(responseModel.answer) ? String.Empty : responseModel.answer;
                        history.SQLQuery = String.IsNullOrEmpty(responseModel.sql) ? String.Empty : responseModel.sql;
                        history.Data = this.Utilities.Serialize(responseModel.data);
                        history.Message = String.IsNullOrEmpty(responseModel.message) ? String.Empty : responseModel.message;

                        this.AskBIHistoryRepo.SaveOrUpdate(history);

                        answer = new Core.Domain.Models.AskBIQuestion()
                        {
                            UniqueId = history.UniqueId,
                            Question = history.Question,
                            Answer = history.Answer,
                            SQL = history.SQLQuery,
                            Message = history.Message,
                            Data = history.Data,
                            IsCorrect = null,
                            TicketOpened = false,
                            Status = responseModel.status,
                            CreationDateTime = history.CreationDateTime
                        };
                    }

                    return answer;
                }
                return null;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("DABLLTA.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void Feedback(Int64 userId, Guid questionUniqueId, Boolean isCorrect)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                Core.Domain.Entities.AskBIHistory question = this.AskBIHistoryRepo.GetAll().Where(x => x.CreatedBy == userId && x.UniqueId == questionUniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AskBIHistory");

                Core.Domain.Models.AskBIFeedbackModel questionModel = new Core.Domain.Models.AskBIFeedbackModel()
                {
                    UserId = user.NTID,
                    qid = question.QuestionId,
                    Question = question.Question,
                    Radio = isCorrect
                };

                NameValueCollection header = new NameValueCollection();

                Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
                String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIToken));

                header.Add("Authorization", String.Format("Bearer {0}", token));

                String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.GenAIUrl);

                this.Logger.Info($"Calling Feedback: {this.Utilities.Serialize(questionModel)}");

                String response = this.APIHelper.GetResponse(String.Format("{0}/feedback", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, this.Utilities.Serialize(questionModel), "application/json");

                this.Logger.Info($"Feedback API Response: {response}");

                Core.Domain.Models.AskBIQuestionResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.AskBIQuestionResponse>(response);

                if (responseModel.message != "202 OK")
                    throw new Core.Domain.Exceptions.GeneralException("An error happened during processing of your request. Please try again later.");

                question.IsCorrect = isCorrect;
                this.AskBIHistoryRepo.SaveOrUpdate(question);

                this.Logger.Info($"Feedback Saved");
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("DABLLTA.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void CreateTicket(Int64 userId, Core.Domain.Models.SharePointTicketModel model)
        {
            try
            {
                var user = this.UsersRepo.Get(userId);
                var question = this.AskBIHistoryRepo.GetAll().Where(q => q.CreatedBy == userId && q.UniqueId == model.UniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Question");

                String data = String.Format("{1}{0}{0}{2}{0}{0}{3}", System.Environment.NewLine, question.Answer, question.SQLQuery, question.Data);

                this.SharePointProvider.CreateTicket(user.Email, "ASK BI", "AskBI", question.Question, data, question.CreationDateTime, model.IssueDescription);

                question.TicketOpened = true;
                this.AskBIHistoryRepo.SaveOrUpdate(question);
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public List<Core.Domain.Models.AskBIQuestion> ListSesstionQuestions(Int64 userId, Guid sessionId)
        {
            try
            {
                using (NHibernate.ISession session = this.SessionFactory.OpenSession())
                {
                    var questions = session.Query<Core.Domain.Entities.AskBIHistory>().Where(q => q.CreatedBy == userId && q.SessionId == sessionId).Select(q => new Core.Domain.Models.AskBIQuestion()
                    {
                        UniqueId = q.UniqueId,
                        Question = q.Question,
                        Answer = q.Answer,
                        SQL = q.SQLQuery,
                        Message = q.Message,
                        Data = q.Data,
                        IsCorrect = null,
                        TicketOpened = false,
                        Status = q.Status == 1 ? "in-progress" : "complete",
                        CreationDateTime = q.CreationDateTime
                    }).ToList();

                    return questions;
                }
            }
            catch (Exception ex)
            {
                return new List<Core.Domain.Models.AskBIQuestion>();
            }
        }
    }
}
