using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public class GenAIProvider : IGenAIProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.IGenAIHistoryRepository GenAIHistoryRepo;
        private readonly Data.Repositories.IUsersRepository UsersRepo;
        private readonly Data.IAPIHelper APIHelper;
        private readonly ISharePointProvider SharePointProvider;
        private readonly Core.Logging.ILogger Logger;

        public GenAIProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                             Core.Utilities.ISettings settings,
                             Core.Utilities.IUtilities utilities,
                             Data.Repositories.IGenAIHistoryRepository genAIHistoryRepo,
                             Data.Repositories.IUsersRepository usersRepo,
                             Data.IAPIHelper apiHelper,
                             ISharePointProvider sharePointProvider,
                             Core.Logging.ILogger logger)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.GenAIHistoryRepo = genAIHistoryRepo;
            this.UsersRepo = usersRepo;
            this.APIHelper = apiHelper;
            this.SharePointProvider = sharePointProvider;
            this.Logger = logger;
        }

        public Core.Domain.Models.AskBIModel GetAskBIModel(Int64 userId, Int32 skip)
        {
            try
            {
                Core.Domain.Models.AskBIModel model = new Core.Domain.Models.AskBIModel();

                model.History = this.GenAIHistoryRepo.GetAll().Where(x => x.CreatedBy == userId).OrderByDescending(x => x.CreationDateTime).ToList().GroupBy(h => new { h.Question }).Skip(skip).Take(100).Select(x => new Core.Domain.Models.AskBIQuestion()
                {
                    Question = x.Key.Question
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

        public Core.Domain.Models.GenAIAnswerModel AskQuestion(Int64 userId, Guid sessionId, Core.Domain.Enumerations.GenAIUsecases usecase, String question)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                String response = String.Empty;
                Guid questionId = Guid.Empty;

                if (this.GenAIHistoryRepo.GetAll().Any(h => h.Question.ToLower() == question.Trim().ToLower()))
                {
                    var history = this.GenAIHistoryRepo.GetAll().Where(h => h.Question.ToLower() == question.Trim().ToLower()).OrderByDescending(h => h.Id).FirstOrDefault();

                    response = RedisProvider.GetCache(history.QuestionId.ToString().ToLower());

                    questionId = history.QuestionId;
                }

                if (String.IsNullOrEmpty(response))
                {
                    questionId = Guid.NewGuid();

                    Core.Domain.Models.GenAIQuestionModel questionModel = new Core.Domain.Models.GenAIQuestionModel()
                    {
                        UserId = user.NTID,
                        QuestionId = questionId.ToString().ToLower(),
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

                    response = this.APIHelper.GetResponse(String.Format("{0}/ask", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, this.Utilities.Serialize(questionModel), "application/json");
                }

                this.Logger.Info(response);

                Core.Domain.Models.GenAIAnswerResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.GenAIAnswerResponse>(response);

                Core.Domain.Models.GenAIAnswerModel answer = new Core.Domain.Models.GenAIAnswerModel();

                if (responseModel != null)
                {
                    Core.Domain.Entities.GenAIHistory history = new Core.Domain.Entities.GenAIHistory()
                    {
                        UniqueId = Guid.NewGuid(),
                        QuestionId = questionId,
                        Question = question.Trim(),
                        CreatedBy = userId,
                        CreationDateTime = DateTime.UtcNow,
                        TicketOpened = false,
                        Usecase = (Byte)usecase,
                        UsecaseSpecificData = String.Empty
                    };

                    this.GenAIHistoryRepo.SaveOrUpdate(history);

                    answer = new Core.Domain.Models.GenAIAnswerModel()
                    {
                        UniqueId = history.UniqueId,
                        QuestionId = history.QuestionId,
                        Question = history.Question,
                        Answer = String.IsNullOrEmpty(responseModel.Answer) ? String.Empty : responseModel.Answer,
                        SQL = responseModel.Status == "complete" ? String.IsNullOrEmpty(responseModel.SQL) ? String.Empty : responseModel.SQL : String.Empty,
                        Message = responseModel.Message,
                        Data = responseModel.Status == "complete" ? this.Utilities.Serialize(responseModel.Data) : String.Empty,
                        IsCorrect = null,
                        TicketOpened = false,
                        Status = responseModel.Status,
                        CreationDateTime = history.CreationDateTime,
                        Response = response
                    };
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

        public Core.Domain.Models.GenAIAnswerModel CheckQuestion(Int64 userId, Guid questionUniqueId)
        {
            try
            {
                var history = this.GenAIHistoryRepo.GetAll().Where(a => a.CreatedBy == userId && a.UniqueId == questionUniqueId).FirstOrDefault();

                if (history == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Question");

                String question = history.Question.ToLower().Trim();

                while (question.Contains("  "))
                    question = question.Replace("  ", " ");

                
                String response = RedisProvider.GetCache(history.QuestionId.ToString().ToLower());

                if (!String.IsNullOrEmpty(response))
                {
                    this.Logger.Info(response);

                    Core.Domain.Models.GenAIAnswerResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.GenAIAnswerResponse>(response);

                    Core.Domain.Models.GenAIAnswerModel answer = new Core.Domain.Models.GenAIAnswerModel();

                    if (responseModel != null)
                    {
                        answer = new Core.Domain.Models.GenAIAnswerModel()
                        {
                            UniqueId = history.UniqueId,
                            QuestionId = history.QuestionId,
                            Question = history.Question,
                            Answer = String.IsNullOrEmpty(responseModel.Answer) ? String.Empty : responseModel.Answer,
                            SQL = String.IsNullOrEmpty(responseModel.SQL) ? String.Empty : responseModel.SQL,
                            Message = String.IsNullOrEmpty(responseModel.Message) ? String.Empty : responseModel.Message,
                            Data = this.Utilities.Serialize(responseModel.Data),
                            IsCorrect = null,
                            TicketOpened = false,
                            Status = responseModel.Status,
                            CreationDateTime = history.CreationDateTime,
                            Response = response
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

        public void Feedback(Int64 userId, Core.Domain.Enumerations.GenAIUsecases usecase, Guid questionUniqueId, Boolean isCorrect)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                Core.Domain.Entities.GenAIHistory question = this.GenAIHistoryRepo.GetAll().Where(x => x.CreatedBy == userId && x.UniqueId == questionUniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AskBIHistory");

                Core.Domain.Models.GenAIFeedbackRequest questionModel = new Core.Domain.Models.GenAIFeedbackRequest()
                {
                    UserId = user.NTID,
                    QuestionId = question.QuestionId.ToString().ToLower(),
                    Radio = isCorrect,
                    Usecase = usecase.ToString().ToLower()
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
                this.GenAIHistoryRepo.SaveOrUpdate(question);

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

        public void CreateTicket(Int64 userId, Core.Domain.Enumerations.GenAIUsecases usecase, Core.Domain.Models.SharePointTicketModel model)
        {
            try
            {
                var user = this.UsersRepo.Get(userId);
                var question = this.GenAIHistoryRepo.GetAll().Where(q => q.CreatedBy == userId && q.UniqueId == model.UniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Question");

                String data = String.Format("{1}{0}{0}{2}{0}{0}{3}", System.Environment.NewLine, question.QuestionId, model.Question, model.Response);

                if (usecase == Core.Domain.Enumerations.GenAIUsecases.AskBI)
                    this.SharePointProvider.CreateTicket(user.Email, "ASK BI", "AskBI", question.Question, data, question.CreationDateTime, model.IssueDescription);
                else
                    this.SharePointProvider.CreateTicket(user.Email, "CMO Chat", "CMOChat", question.Question, data, question.CreationDateTime, model.IssueDescription);

                question.TicketOpened = true;
                this.GenAIHistoryRepo.SaveOrUpdate(question);
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
    }
}
