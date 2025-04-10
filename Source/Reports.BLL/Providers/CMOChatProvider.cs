using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP = Microsoft.SharePoint.Client;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security;
using Microsoft.VisualBasic;
using System.Threading;

namespace Reports.BLL.Providers
{
    public class CMOChatProvider : ICMOChatProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.ICMOChatHistoryRepository CMOChatHistoryRepo;
        private readonly Data.Repositories.ICMOChatQuestionsRepository CMOChatQuestionsRepo;
        private readonly Data.Repositories.IUsersRepository UsersRepo;
        private readonly Data.IAPIHelper APIHelper;
        private readonly ISharePointProvider SharePointProvider;
        private readonly Core.Logging.ILogger Logger;

        private Core.Domain.Models.CMOChatCampaingsLeadIds CMOChatCampaingsLeadIds;
        private DateTime CMOChatCampaingsLeadIdsLastUpdate;
        public CMOChatProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                             Core.Utilities.ISettings settings,
                             Core.Utilities.IUtilities utilities,
                             Data.Repositories.ICMOChatHistoryRepository cmoChatHistoryRepo,
                             Data.Repositories.ICMOChatQuestionsRepository cmoChatQuestionsRepo,
                             Data.Repositories.IUsersRepository usersRepo,
                             Data.IAPIHelper apiHelper,
                             ISharePointProvider sharePointProvider,
                             Core.Logging.ILogger logger)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.CMOChatHistoryRepo = cmoChatHistoryRepo;
            this.CMOChatQuestionsRepo = cmoChatQuestionsRepo;
            this.UsersRepo = usersRepo;
            this.APIHelper = apiHelper;
            this.SharePointProvider = sharePointProvider;
            this.Logger = logger;
        }

        public void LoadQuestionsData()
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(@"C:\Temp\new 8.csv"))
            {
                while (!sr.EndOfStream)
                {
                    String text = sr.ReadLine();

                    Core.Domain.Entities.CMOChatQuestion question = new Core.Domain.Entities.CMOChatQuestion()
                    {
                        Question = text,
                        Rank = 1
                    };

                    this.CMOChatQuestionsRepo.SaveOrUpdate(question);
                }
            }
        }

        public Core.Domain.Models.CMOChatModel GetCMOChatModel(Int64 userId, Int32 skip)
        {
            try
            {
                Core.Domain.Models.CMOChatModel model = new Core.Domain.Models.CMOChatModel();

                model.History = this.CMOChatHistoryRepo.GetAll().Where(x => x.CreatedBy == userId).OrderByDescending(x => x.CreationDateTime).Skip(skip).Take(10).Select(x => new Core.Domain.Models.CMOChatQuestion()
                {
                    UniqueId = x.UniqueId,
                    Question = x.Question,
                    Answer = x.Answer,
                    NLText = x.NLText,
                    DateText = x.DateText,
                    SQL = x.SQLQuery,
                    ResponseType = x.Message,
                    IsCorrect = x.IsCorrect,
                    TicketOpened = x.TicketOpened,
                    CreationDateTime = x.CreationDateTime,
                    Suggestions = new List<String>(),
                }).ToList();

                var answer = this.AskQuestion(userId, String.Empty, false, false);

                model.Suggestions = answer.Suggestions;

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

        public Core.Domain.Models.CMOChatQuestion AskQuestion(Int64 userId, String question, Boolean displaySQL, Boolean isPredefinedQuestion)
        {
            try
            {
                //No metric identified in query. Please try again with valid metric

                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                Core.Domain.Models.CMOChatQuestionModel questionModel = new Core.Domain.Models.CMOChatQuestionModel()
                {
                    UserId = user.NTID,
                    Question = question.Trim()
                };

                NameValueCollection header = new NameValueCollection();

                Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
                String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOCHATToken));

                header.Add("Authorization", String.Format("Bearer {0}", token));

                String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOChatUrl);

                String response = this.APIHelper.GetResponse(String.Format("{0}/ask", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, this.Utilities.Serialize(questionModel), "application/json");

                Core.Domain.Models.CMOChatQuestionResponse answer = this.Utilities.Deserialize<Core.Domain.Models.CMOChatQuestionResponse>(response);

                answer.nl_text = answer.nl_text.Trim();

                if (answer.nl_text.StartsWith("b'"))
                    answer.nl_text = answer.nl_text.Substring(2);

                if (answer.nl_text.StartsWith("b\""))
                    answer.nl_text = answer.nl_text.Substring(2);

                answer.nl_text = answer.nl_text.TrimEnd(new char[] { ' ', '\'', '"' });

                Core.Domain.Models.CMOChatQuestion answerModel = new Core.Domain.Models.CMOChatQuestion();

                if (answer != null && answer.answer != null)
                {
                    if (!String.IsNullOrEmpty(question))
                    {
                        Core.Domain.Entities.CMOChatHistory history = new Core.Domain.Entities.CMOChatHistory()
                        {
                            UniqueId = Guid.NewGuid(),
                            Question = question.Trim(),
                            Answer = answer.answer == null ? String.Empty : answer.response_type == "table" ? this.Utilities.Serialize(answer.answer) : answer.answer.ToString(),
                            NLText = String.IsNullOrEmpty(answer.nl_text) ? String.Empty : answer.nl_text.Trim(),
                            DateText = String.IsNullOrEmpty(answer.date_text) ? String.Empty : answer.date_text.Trim(),
                            SQLQuery = String.IsNullOrEmpty(answer.sql) ? String.Empty : answer.sql,
                            CreatedBy = userId,
                            CreationDateTime = DateTime.UtcNow,
                            Message = String.IsNullOrEmpty(answer.response_type) ? String.Empty : answer.response_type,
                            QuestionId = answer.qid.HasValue ? answer.qid.Value : 0,
                        };

                        this.CMOChatHistoryRepo.SaveOrUpdate(history);

                        answerModel = new Core.Domain.Models.CMOChatQuestion()
                        {
                            UniqueId = history.UniqueId,
                            Question = history.Question,
                            Answer = history.Answer,
                            NLText = history.NLText,
                            DateText = history.DateText,
                            SQL = history.SQLQuery,
                            IsCorrect = null,
                            ResponseType = answer.response_type,
                            Suggestions = answer.suggestions,
                            CreationDateTime = history.CreationDateTime
                        };
                    }
                    else
                    {
                        answerModel = new Core.Domain.Models.CMOChatQuestion()
                        {
                            UniqueId = Guid.Empty,
                            Question = String.Empty,
                            Answer = String.Empty,
                            NLText = String.Empty,
                            DateText = String.Empty,
                            SQL = String.Empty,
                            IsCorrect = null,
                            ResponseType = String.Empty,
                            Suggestions = answer.suggestions,
                            CreationDateTime = DateTime.UtcNow
                        };
                    }
                }

                if (isPredefinedQuestion)
                {
                    var cmoQuestion = this.CMOChatQuestionsRepo.GetAll().Where(q => q.Question.ToLower() == question.ToLower()).FirstOrDefault();

                    if (cmoQuestion != null)
                    {
                        cmoQuestion.Rank++;

                        this.CMOChatQuestionsRepo.SaveOrUpdate(cmoQuestion);
                    }
                }

                return answerModel;
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

        public void SetQuestionIsCorrect(Int64 userId, Guid questionUniqueId, Boolean isCorrect)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                Core.Domain.Entities.CMOChatHistory question = this.CMOChatHistoryRepo.GetAll().Where(x => x.CreatedBy == userId && x.UniqueId == questionUniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("CMOChatHistory");

                Core.Domain.Models.CMOChatFeedbackModel questionModel = new Core.Domain.Models.CMOChatFeedbackModel()
                {
                    uid = user.NTID,
                    qid = question.QuestionId,
                    radio = isCorrect
                };

                NameValueCollection header = new NameValueCollection();

                Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
                String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOCHATToken));

                header.Add("Authorization", String.Format("Bearer {0}", token));

                //this.Logger.Info(this.Utilities.Serialize(questionModel));

                String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOChatUrl);

                String response = this.APIHelper.GetResponse(String.Format("{0}/feedback", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, this.Utilities.Serialize(questionModel), "application/json");

                //this.Logger.Info(response);

                Core.Domain.Models.CMOChatQuestionResponse responseModel = this.Utilities.Deserialize<Core.Domain.Models.CMOChatQuestionResponse>(response);

                //if (responseModel.message != "202 OK")
                //    throw new Core.Domain.Exceptions.GeneralException("An error happened during processing of your request. Please try again later.");

                question.IsCorrect = isCorrect;

                this.CMOChatHistoryRepo.SaveOrUpdate(question);
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

        public List<String> ListQuestions(Int64 userId, String question)
        {
            try
            {
                question = question.ToLower().Replace(",", " ").Replace(";", " ").Replace(".", " ").Replace("!", " ").Replace("?", " ");

                while (question.Contains("  "))
                    question = question.Replace("  ", " ");

                List<String> questions = this.CMOChatQuestionsRepo.GetAll().Where(q => q.Question.StartsWith(question)).OrderBy(q => q.Rank).Take(10).Select(q => q.Question).ToList();

                return questions;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());
                methodParam.Add("question", question);

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

                Core.Domain.Entities.CMOChatHistory question = this.CMOChatHistoryRepo.GetAll().Where(x => x.CreatedBy == userId && x.UniqueId == questionUniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AskBIHistory");

                Core.Domain.Models.AskBIFeedbackModel questionModel = new Core.Domain.Models.AskBIFeedbackModel()
                {
                    UserId = user.NTID,
                    qid = question.QuestionId,
                    Question = question.Question,
                    Radio = isCorrect,
                    Usecase = Core.Domain.Enumerations.GenAIUsecases.CMOChat.ToString().ToLower()
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
                this.CMOChatHistoryRepo.SaveOrUpdate(question);

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
                var question = this.CMOChatHistoryRepo.GetAll().Where(q => q.CreatedBy == userId && q.UniqueId == model.UniqueId).FirstOrDefault();

                if (question == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Question");

                this.SharePointProvider.CreateTicket(user.Email, "CMO Chat", "CMOChat", question.Question, question.Answer, question.CreationDateTime, model.IssueDescription);

                question.TicketOpened = true;
                this.CMOChatHistoryRepo.SaveOrUpdate(question);
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

        public Core.Domain.Models.CMOChatCampaingsLeadIds ListCampaignsLeadIds()
        {
            try
            {
                if (this.CMOChatCampaingsLeadIds == null)
                {
                    LoadCampaignsLeadIds();

                    return this.CMOChatCampaingsLeadIds;
                }
                else
                {
                    if (this.CMOChatCampaingsLeadIdsLastUpdate >= DateTime.UtcNow.AddMinutes(-60))
                    {
                        return this.CMOChatCampaingsLeadIds;
                    }
                    else
                    {
                        Thread thread = new Thread(LoadCampaignsLeadIds);
                        thread.Start();

                        return this.CMOChatCampaingsLeadIds;
                    }
                }
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                
                throw this.ErrorHandler.HandleError(ex, String.Format("DABLLTA.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        private void LoadCampaignsLeadIds()
        {
            NameValueCollection header = new NameValueCollection();

            Core.Encryption.BlowFish blowFish = new Core.Encryption.BlowFish(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.HashKey));
            String token = blowFish.Decrypt_ECB(this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOCHATToken));

            header.Add("Authorization", String.Format("Bearer {0}", token));

            String baseUrl = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.CMOChatUrl);

            String response = this.APIHelper.GetResponse(String.Format("{0}/get_cmochat_campaign", baseUrl), Core.Domain.Enumerations.RequestMethod.POST, header, null, "application/json");

            this.CMOChatCampaingsLeadIds = this.Utilities.Deserialize<Core.Domain.Models.CMOChatCampaingsLeadIds>(response);

            this.CMOChatCampaingsLeadIdsLastUpdate = DateTime.UtcNow;
        }
    }
}
