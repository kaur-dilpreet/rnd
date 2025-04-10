using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public class RequestAccessProvider : IRequestAccessProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Logging.ILogger Logger;
        private readonly Core.Email.IEmailService EmailService;
        private readonly Data.Repositories.IAccessRequestsRepository AccessRequestsRepo;
        private readonly Data.Repositories.IUsersRepository UsersRepo;

        public RequestAccessProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                             Core.Utilities.ISettings settings,
                             Core.Utilities.IUtilities utilities,
                             Core.Logging.ILogger logger,
                             Core.Email.IEmailService emailService,
                             Data.Repositories.IAccessRequestsRepository accessRequestsRepo,
                             Data.Repositories.IUsersRepository usersRepo)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.Logger = logger;
            this.EmailService = emailService;
            this.AccessRequestsRepo = accessRequestsRepo;
            this.UsersRepo = usersRepo;
        }

        public Core.Domain.Models.RequestAccessModel GetRequestAccessModel(Int64 userId)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.CreatedBy == userId).OrderByDescending(ar => ar.Id).FirstOrDefault();

                Core.Domain.Models.RequestAccessModel model = new Core.Domain.Models.RequestAccessModel();

                if (accessRequest == null || accessRequest.IsApproved)
                {
                    model.HasPendingRequest = false;
                }
                else
                {
                    model.HasPendingRequest = true;
                    model.IsApproved = accessRequest.IsApproved;
                    model.IsDenied = accessRequest.IsDenied;
                    model.DenyReason = accessRequest.DenyReason;
                    model.AccessRequestSubmitionDateTime = accessRequest.CreationDateTime.AddMinutes(Core.Utilities.Utilities.GetLoggedInUserTimezoneOffset());
                    model.ResponseDateTime = accessRequest.ResponseDateTime;
                }

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

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.UserRequest GetUserRequest(Int64 userId)
        {
            try
            {
                var request = this.AccessRequestsRepo.GetAll().Where(ar => ar.CreatedBy == userId).FirstOrDefault();

                if (request != null)
                {
                    var user = this.UsersRepo.Get(userId);

                    return new Core.Domain.Models.UserRequest()
                    {
                        AskBIAccess = user.AskBIAccess,
                        ChatGPIAccess = user.ChatGPIAccess,
                        CMOChatAccess = user.CMOChatAccess,
                        SDRAIAccess = user.SDRAIAccess
                    };
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

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void UpdateUserRequest(Int64 userId, Core.Domain.Models.UserRequest model)
        {
            try
            {
                var request = this.AccessRequestsRepo.GetAll().Where(ar => ar.CreatedBy == userId).FirstOrDefault();

                if (request == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("Access Request");

                request.AskBIAccess = model.AskBIAccess;
                request.ChatGPIAccess = model.ChatGPIAccess;
                request.CMOChatAccess = model.CMOChatAccess;
                request.SDRAIAccess = model.SDRAIAccess;
                request.LastModificationDateTime = DateTime.UtcNow;

                this.AccessRequestsRepo.Save(request);

                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                System.Collections.Hashtable body = new System.Collections.Hashtable();
                body.Add("<%FIRSTNAME%>", user.FirstName);

                this.EmailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.AccessRequestSubmitted, user.Email, String.Empty, null, body);

                IEnumerable<Core.Domain.Entities.User> admins = this.UsersRepo.GetAll().Where(u => u.IsApproved && u.RoleId == (Byte)Core.Domain.Enumerations.UserRoles.Admin);

                String requestedReports = String.Empty;

                if (model.AskBIAccess)
                    requestedReports = String.Format("{0}, AskBI", requestedReports);

                if (model.ChatGPIAccess)
                    requestedReports = String.Format("{0}, ChatGPI", requestedReports);

                if (model.CMOChatAccess)
                    requestedReports = String.Format("{0}, CMOChat", requestedReports);

                if (model.SDRAIAccess)
                    requestedReports = String.Format("{0}, SDRAI", requestedReports);

                requestedReports = requestedReports.Trim(new char[] { ',', ' ' });

                foreach (var admin in admins)
                {
                    body = new System.Collections.Hashtable();

                    body.Add("<%FIRSTNAME%>", admin.FirstName);
                    body.Add("<%USERNAME%>", String.Format("{0} {1}", user.FirstName, user.LastName));
                    body.Add("<%REPORTS%>", requestedReports);
                    body.Add("<%URL%>", String.Format("{0}://{1}/requestaccess/requests", System.Web.HttpContext.Current.Request.Url.Scheme, System.Web.HttpContext.Current.Request.Url.Authority));

                    this.EmailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.AccessRequestAlert, admin.Email, String.Empty, null, body);
                }
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());
                methodParam.Add("model", this.Utilities.Serialize(model));

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.AccessRequestsModel GetAccessRequestsModel() {
            try
            {
                Core.Domain.Models.AccessRequestsModel model = new Core.Domain.Models.AccessRequestsModel();

                model.Requests = this.AccessRequestsRepo.GetAll().OrderByDescending(r => r.LastModificationDateTime).Select(r => new Core.Domain.Models.AccessRequestModel()
                {
                    UniqueId = r.UniqueId,
                    IsApproved = r.IsApproved,
                    IsDenied = r.IsDenied,
                    DenyReason = r.DenyReason,
                    Requester = new Core.Domain.Models.UserModel()
                    {
                        UniqueId = r.CreatedByUser.UniqueId,
                        Email = r.CreatedByUser.Email,
                        FullName = String.Format("{0} {1}", r.CreatedByUser.FirstName, r.CreatedByUser.LastName),
                        Id = r.CreatedByUser.Id,
                        AskBIAccess = r.CreatedByUser.AskBIAccess,
                        CMOChatAccess = r.CreatedByUser.CMOChatAccess,
                        SDRAIAccess = r.CreatedByUser.SDRAIAccess,
                        ChatGPIAccess = r.CreatedByUser.ChatGPIAccess,
                        NTID = r.CreatedByUser.NTID
                    },
                    Responder = new Core.Domain.Models.UserModel()
                    {
                        Id = r.ResponseByUser != null ? r.ResponseByUser.Id : 0,
                        UniqueId = r.ResponseByUser != null ? r.ResponseByUser.UniqueId : Guid.Empty,
                        Email = r.ResponseByUser != null ? r.ResponseByUser.Email : String.Empty,
                        FullName = r.ResponseByUser != null ? String.Format("{0} {1}", r.ResponseByUser.FirstName, r.ResponseByUser.LastName) : String.Empty,
                        NTID = r.ResponseByUser != null ? r.ResponseByUser.NTID : String.Empty
                    },
                    ResponseDateTime = r.ResponseDateTime,
                    CreationDateTime = r.CreationDateTime
                }).ToList();

                return model;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void RequestAccess(Int64 userId, Boolean accessAskBI, Boolean accessCMOChat, Boolean accessSDRAI, Boolean accessChatGPI)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.CreatedBy == userId).OrderByDescending(ar => ar.Id).FirstOrDefault();

                if (accessRequest != null && (accessRequest.IsApproved || (!accessRequest.IsApproved && !accessRequest.IsDenied)))
                {
                    if (!accessRequest.IsApproved && !accessRequest.IsDenied)
                        throw new Core.Domain.Exceptions.AlreadyHavePendingAccessRequestException();
                    else
                        throw new Core.Domain.Exceptions.AccessRequestApprovedException();
                }
                else
                {
                    accessRequest = new Core.Domain.Entities.AccessRequest()
                    {
                        UniqueId = Guid.NewGuid(),
                        CreatedBy = userId,
                        AskBIAccess = accessAskBI,
                        CMOChatAccess = accessCMOChat,
                        SDRAIAccess = accessSDRAI,
                        ChatGPIAccess = accessChatGPI,
                        CreationDateTime = DateTime.UtcNow,
                        IsApproved = false,
                        IsDenied = false,
                        LastModificationDateTime = DateTime.UtcNow,
                        DenyReason = String.Empty
                    };

                    this.AccessRequestsRepo.SaveOrUpdate(accessRequest);

                    Core.Domain.Entities.User user = this.UsersRepo.Get(accessRequest.CreatedBy);

                    System.Collections.Hashtable body = new System.Collections.Hashtable();
                    body.Add("<%FIRSTNAME%>", user.FirstName);

                    this.EmailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.AccessRequestSubmitted, user.Email, String.Empty, null, body);

                    IEnumerable<Core.Domain.Entities.User> admins = this.UsersRepo.GetAll().Where(u => u.IsApproved && u.RoleId == (Byte)Core.Domain.Enumerations.UserRoles.Admin);

                    String requestedReports = String.Empty;

                    if (accessAskBI)
                        requestedReports = String.Format("{0}, AskBI", requestedReports);

                    if (accessChatGPI)
                        requestedReports = String.Format("{0}, ChatGPI", requestedReports);

                    if (accessCMOChat)
                        requestedReports = String.Format("{0}, CMOChat", requestedReports);

                    if (accessSDRAI)
                        requestedReports = String.Format("{0}, SDRAI", requestedReports);

                    requestedReports = requestedReports.Trim(new char[] { ',', ' ' });

                    foreach (var admin in admins)
                    {
                        body = new System.Collections.Hashtable();

                        body.Add("<%FIRSTNAME%>", admin.FirstName);
                        body.Add("<%USERNAME%>", String.Format("{0} {1}", user.FirstName, user.LastName));
                        body.Add("<%REPORTS%>", requestedReports);
                        body.Add("<%URL%>", String.Format("{0}://{1}/requestaccess/requests", System.Web.HttpContext.Current.Request.Url.Scheme, System.Web.HttpContext.Current.Request.Url.Authority));

                        this.EmailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.AccessRequestAlert, admin.Email, String.Empty, null, body);
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
                methodParam.Add("userId", userId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.ChangeAccessLevelModel GetChangeAccessLevelModel(Int64 userId, Guid requestId)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.UniqueId == requestId).FirstOrDefault();

                if (accessRequest == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AccessRequest");

                Core.Domain.Entities.User user = this.UsersRepo.Get(accessRequest.CreatedBy);

                Core.Domain.Models.ChangeAccessLevelModel model = new Core.Domain.Models.ChangeAccessLevelModel()
                {
                    UniqueId = accessRequest.UniqueId,
                    RequesterEmail = user.Email,
                    RequesterFullName = String.Format("{0} {1}", user.FirstName, user.LastName),
                    AskBIAccess = user.AskBIAccess,
                    ChatGPIAccess = user.ChatGPIAccess,
                    CMOChatAccess = user.CMOChatAccess,
                    SDRAIAccess = user.SDRAIAccess,
                    UserRequest = new Core.Domain.Models.UserRequest()
                    {
                        AskBIAccess = accessRequest.AskBIAccess,
                        CMOChatAccess = accessRequest.CMOChatAccess,
                        SDRAIAccess = accessRequest.SDRAIAccess,
                        ChatGPIAccess = accessRequest.ChatGPIAccess
                    }
                };

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
                methodParam.Add("requestId", requestId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.AccessRequestModel ChangeAccessLevel(Int64 userId, Core.Domain.Models.ChangeAccessLevelModel model)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.UniqueId == model.UniqueId).FirstOrDefault();

                if (accessRequest == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AccessRequest");

                accessRequest.IsDenied = false;
                accessRequest.DenyReason = String.Empty;
                accessRequest.IsApproved = true;
                //accessRequest.ReportAccess = model.ReportAccess;
                //accessRequest.MARTAccess = model.MARTAccess;
                //accessRequest.AskBIAccess = model.AskBIAccess;
                accessRequest.ResponseBy = userId;
                accessRequest.ResponseDateTime = DateTime.UtcNow;

                this.AccessRequestsRepo.SaveOrUpdate(accessRequest);

                Core.Domain.Entities.User user = this.UsersRepo.Get(accessRequest.CreatedBy);

                Core.Domain.Enumerations.EmailTemplates emailTemplate = Core.Domain.Enumerations.EmailTemplates.AccessApproved;

                if (user.IsApproved)
                    emailTemplate = Core.Domain.Enumerations.EmailTemplates.AccessChanged;

                user.IsApproved = true;
                user.AskBIAccess = model.AskBIAccess;
                user.CMOChatAccess = model.CMOChatAccess;
                user.SDRAIAccess = model.SDRAIAccess;
                user.ChatGPIAccess = model.ChatGPIAccess;

                this.UsersRepo.SaveOrUpdate(user);

                System.Collections.Hashtable body = new System.Collections.Hashtable();
                body.Add("<%FIRSTNAME%>", user.FirstName);

                String apps = String.Empty;
                Int32 count = 0;

                if (user.AskBIAccess)
                {
                    apps = String.Format(" {0} <b>Ask BI</b>,", apps);
                    count++;
                }

                if (user.SDRAIAccess)
                {
                    apps = String.Format(" {0} <b>SDR AI</b>,", apps);
                    count++;
                }

                if (user.CMOChatAccess)
                {
                    apps = String.Format(" {0} <b>CMO Chat</b>,", apps);
                    count++;
                }

                if (user.ChatGPIAccess)
                {
                    apps = String.Format(" {0} <b>ChatGPI</b>,", apps);
                    count++;
                }

                apps = String.Format("{0} app{1}", apps.Trim(new char[] { ',', ' ' }), count > 1 ? "s" : String.Empty);

                if (emailTemplate == Core.Domain.Enumerations.EmailTemplates.AccessApproved)
                {
                    apps = String.Format("to {0} on", apps);
                }
                else
                {
                    apps = String.Format("You now have access to {0}", apps);
                }

                var lastComma = apps.LastIndexOf(',');
                if (lastComma != -1) apps = apps.Remove(lastComma, 1).Insert(lastComma, " and");

                body.Add("<%APPS%>", apps);

                body.Add("<%URL%>", "https://magenai.hpecorp.net/");

                this.EmailService.SendEmail(emailTemplate, user.Email, String.Empty, null, body);

                Core.Domain.Entities.User responder = this.UsersRepo.Get(userId);

                Core.Domain.Models.AccessRequestModel requests = new Core.Domain.Models.AccessRequestModel()
                {
                    UniqueId = accessRequest.UniqueId,
                    IsApproved = accessRequest.IsApproved,
                    IsDenied = accessRequest.IsDenied,
                    DenyReason = accessRequest.DenyReason,
                    Requester = new Core.Domain.Models.UserModel()
                    {
                        UniqueId = accessRequest.CreatedByUser.UniqueId,
                        Email = accessRequest.CreatedByUser.Email,
                        FullName = String.Format("{0} {1}", accessRequest.CreatedByUser.FirstName, accessRequest.CreatedByUser.LastName),
                        Id = accessRequest.CreatedByUser.Id,
                        AskBIAccess = user.AskBIAccess,
                        CMOChatAccess = user.CMOChatAccess,
                        SDRAIAccess = user.SDRAIAccess,
                        ChatGPIAccess = user.ChatGPIAccess,
                        NTID = accessRequest.CreatedByUser.NTID
                    },
                    Responder = new Core.Domain.Models.UserModel()
                    {
                        UniqueId = responder.UniqueId,
                        Email = responder.Email,
                        FullName = String.Format("{0} {1}", responder.FirstName, responder.LastName),
                        Id = responder.Id,
                        NTID = responder.NTID
                    },
                    ResponseDateTime = accessRequest.ResponseDateTime,
                    CreationDateTime = accessRequest.CreationDateTime
                };

                return requests;
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

        public Core.Domain.Models.DenyRequestModel GetDenyRequestModel(Int64 userId, Guid requestId)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.UniqueId == requestId).FirstOrDefault();

                if (accessRequest == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AccessRequest");

                if (accessRequest.IsDenied)
                    throw new Core.Domain.Exceptions.RequestAlreadyDeniedException();

                Core.Domain.Entities.User user = this.UsersRepo.Get(accessRequest.CreatedBy);

                Core.Domain.Models.DenyRequestModel model = new Core.Domain.Models.DenyRequestModel()
                {
                    UniqueId = accessRequest.UniqueId,
                    RequesterEmail = user.Email,
                    RequesterFullName = String.Format("{0} {1}", user.FirstName, user.LastName),
                    Comment = String.Empty
                };

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
                methodParam.Add("requestId", requestId.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.AccessRequestModel DenyRequest(Int64 userId, Guid requestId, String reason)
        {
            try
            {
                Core.Domain.Entities.AccessRequest accessRequest = this.AccessRequestsRepo.GetAll().Where(ar => ar.UniqueId == requestId).FirstOrDefault();

                if (accessRequest == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("AccessRequest");

                if (accessRequest.IsDenied)
                    throw new Core.Domain.Exceptions.RequestAlreadyDeniedException();

                accessRequest.IsDenied = true;
                accessRequest.DenyReason = String.IsNullOrEmpty(reason) ? String.Empty : reason;
                accessRequest.IsApproved = false;
                accessRequest.ResponseBy = userId;
                accessRequest.ResponseDateTime = DateTime.UtcNow;

                this.AccessRequestsRepo.SaveOrUpdate(accessRequest);

                Core.Domain.Entities.User user = this.UsersRepo.Get(accessRequest.CreatedBy);

                user.IsApproved = false;

                this.UsersRepo.SaveOrUpdate(user);

                System.Collections.Hashtable body = new System.Collections.Hashtable();
                body.Add("<%FIRSTNAME%>", user.FirstName);
                body.Add("<%DENYREASON%>", accessRequest.DenyReason);

                this.EmailService.SendEmail(Core.Domain.Enumerations.EmailTemplates.AccessDenied, user.Email, String.Empty, null, body);

                Core.Domain.Entities.User responder = this.UsersRepo.Get(userId);

                Core.Domain.Models.AccessRequestModel model = new Core.Domain.Models.AccessRequestModel()
                {
                    UniqueId = accessRequest.UniqueId,
                    IsApproved = accessRequest.IsApproved,
                    IsDenied = accessRequest.IsDenied,
                    DenyReason = accessRequest.DenyReason,
                    Requester = new Core.Domain.Models.UserModel()
                    {
                        UniqueId = accessRequest.CreatedByUser.UniqueId,
                        Email = accessRequest.CreatedByUser.Email,
                        FullName = String.Format("{0} {1}", accessRequest.CreatedByUser.FirstName, accessRequest.CreatedByUser.LastName),
                        Id = accessRequest.CreatedByUser.Id,
                        AskBIAccess = user.AskBIAccess,
                        CMOChatAccess = user.CMOChatAccess,
                        SDRAIAccess = user.SDRAIAccess,
                        ChatGPIAccess = user.ChatGPIAccess,
                        NTID = accessRequest.CreatedByUser.NTID
                    },
                    Responder = new Core.Domain.Models.UserModel()
                    {
                        UniqueId = responder.UniqueId,
                        Email = responder.Email,
                        FullName = String.Format("{0} {1}", responder.FirstName, responder.LastName),
                        Id = responder.Id,
                        NTID = responder.NTID
                    },
                    ResponseDateTime = accessRequest.ResponseDateTime,
                    CreationDateTime = accessRequest.CreationDateTime
                };

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

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }
    }
}
