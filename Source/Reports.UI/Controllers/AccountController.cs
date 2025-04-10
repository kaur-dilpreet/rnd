using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Reports.Core.Domain.Entities;
using Reports.UI.Filters;
using System.IO;
using System.Linq;
using PFAuth;
    
namespace Reports.UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly BLL.Providers.IAccountProvider AccountProvider;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Logging.ILogger Logger;
        private readonly Core.Utilities.ISettings Settings;

        public AccountController(BLL.Providers.IAccountProvider accountProvider,
                                 Core.ErrorHandling.IErrorHandler errorHandler,
                                 Core.Utilities.IUtilities utilities,
                                 Core.Logging.ILogger logger,
                                 Core.Utilities.ISettings settings)
        {
            this.AccountProvider = accountProvider;
            this.ErrorHandler = errorHandler;
            this.Utilities = utilities;
            this.Logger = logger;
            this.Settings = settings;            
        }

        public ActionResult Login(String returnUrl)
        {
            String uid = String.Empty;
            
            uid = _getAuth();                
            
            String authCookieName = this.Settings.GetSettings(Core.Domain.Enumerations.AppSettings.AuthCookieName);

            if (Request.Cookies[authCookieName] != null)
            {
                var info = _getAuthInfo();

                int count = 0;
                while (count < 1 && (!Utilities.ValidateEmail(info.Email) || !Utilities.ValidateNTID(info.NTID)))
                {
                    info = _getAuthInfo();
                    count++;
                }

                if (!Utilities.ValidateEmail(info.Email) || !Utilities.ValidateNTID(info.NTID))
                {
                    this.Logger.Error(String.Format("Signin Error: Email{0} NTID: {1}", info.Email, info.NTID));
                    return Redirect("/SignInError");
                }

                if (!this.AccountProvider.UserExist(uid))
                {
                    this.AccountProvider.AddUser(info.Email, info.FirstName, info.LastName, info.NTID);
                }
                else
                {
                    Core.Domain.Entities.User user = this.AccountProvider.GetUser(uid, true);

                    if (String.IsNullOrEmpty(user.FirstName) || String.IsNullOrEmpty(user.LastName) || String.IsNullOrEmpty(user.NTID))
                    {

                        this.AccountProvider.UpdateUser(uid, info.FirstName, info.LastName, info.NTID);
                    }
                }

                System.Web.Security.FormsAuthentication.SetAuthCookie(uid, true);

                if (String.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Home");
                else
                    return Redirect(returnUrl);
            }

            return null;
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();

            return Redirect(Url.Content("~/reports"));
        }

        //protected String getHeaders(String key_UID)
        //{
        //    string output = String.Empty;
        //    NameValueCollection headers = Request.Headers;
        //    for (int i = 0; i < headers.Count; i++)
        //    {
        //        string key = headers.GetKey(i);
        //        string value = headers.Get(i);
        //        if (key == key_UID)
        //        {
        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                output = value;
        //            }
        //        }
        //    }

        //    return output;
        //}

        private string _getAuth()
        {
            try
            {
                //return Request.Headers
                return HPUID.PF_Authentication("~/PF_Auth/PF_Auth.aspx", "uid");
                //return getHeaders("PF_AUTH_uid");
            }
            catch (Exception ex)
            {
                this.Logger.Info(ex.ToString());
                return null;
            }
        }

        private User _getAuthInfo()
        {
            return new Core.Domain.Entities.User
            {
                Email = HPUID.PF_Authentication("~/PF_Auth/PF_Auth.aspx", "uid"),
                FirstName = HPUID.PF_Authentication("~/PF_Auth/PF_Auth.aspx", "givenName"),
                LastName = HPUID.PF_Authentication("~/PF_Auth/PF_Auth.aspx", "sn"),
                NTID = HPUID.PF_Authentication("~/PF_Auth/PF_Auth.aspx", "ntUserDomainID")
            };
        }

        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult Users(Int32? page, String search)
        {
            Int32 pageNumber = page.HasValue ? page.Value : 1;

            Int32 take = Core.Domain.Enumerations.ItemsPerPage;
            Int32 skip = (pageNumber - 1) * take;

            Core.Domain.Models.UsersSearchModel searchModel = null;

            if (!String.IsNullOrEmpty(search))
            {
                searchModel = new Core.Domain.Models.UsersSearchModel() {
                    SearchCriteria = search
                };
            }

            Core.Domain.Models.UsersModel model = this.AccountProvider.GetUsersModel(searchModel, skip, take);

            model.PaginationModel.CurrentPage = pageNumber;

            return View(model);
        }

        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult GetUsers(Int32 id, String search)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = "An error happened during processing of your request. Please try again later"
            };

            try
            {
                Int32 pageNumber = id;

                Int32 take = Core.Domain.Enumerations.ItemsPerPage;
                Int32 skip = (pageNumber - 1) * take;

                Core.Domain.Models.UsersSearchModel searchModel = null;

                if (!String.IsNullOrEmpty(search))
                {
                    searchModel = new Core.Domain.Models.UsersSearchModel()
                    {
                        SearchCriteria = search
                    };
                }

                Core.Domain.Models.UsersModel model = this.AccountProvider.GetUsersModel(searchModel, skip, take);

                model.PaginationModel.CurrentPage = pageNumber;

                result.Object = this.Utilities.RenderPartialViewToString(this, "_UsersPartial", model);
                result.Result = true;
                result.Message = String.Empty;
            }
            catch (Core.Domain.Exceptions.DuplicateValueException ex)
            {
                result.Message = ex.Message;
            }
            catch (Core.Domain.Exceptions.GeneralException)
            {
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

        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult AddUser()
        {
            Core.Domain.Models.UserAddModel model = this.AccountProvider.GetUserAddModel();

            return PartialView("_UserAddPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult AddUser(Core.Domain.Models.UserAddModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = "An error happened during processing of your request. Please try again later"
            };

            try
            {
                this.AccountProvider.AddUser(Core.Utilities.Utilities.GetLoggedInUser().Id, model);
                result.Result = true;
                result.Message = String.Empty;

                Core.Domain.Models.UsersModel users = this.AccountProvider.GetUsersModel();
                result.Object = this.Utilities.RenderPartialViewToString(this, "_UsersPartial", users.Users);
            }
            catch (Core.Domain.Exceptions.DuplicateValueException ex)
            {
                result.Message = ex.Message;
            }
            catch (Core.Domain.Exceptions.GeneralException)
            {
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

        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult EditUser(Int64 id)
        {
            Core.Domain.Models.UserUpdateModel model = this.AccountProvider.GetUserUpdateModel(id);

            return PartialView("_UserEditPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult EditUser(Core.Domain.Models.UserUpdateModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = "An error happened during processing of your request. Please try again later"
            };

            try
            {
                this.AccountProvider.UpdateUser(Core.Utilities.Utilities.GetLoggedInUser().Id, model);
                result.Result = true;
                result.Message = model.Id.ToString();

                result.Object = ((Core.Domain.Enumerations.UserRoles)model.UserRoleId).ToString();
            }
            catch (Core.Domain.Exceptions.GeneralException)
            {
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
        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult SearchUsers(Core.Domain.Models.UsersSearchModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = "An error happened during processing of your request. Please try again later"
            };

            try
            {
                if (ModelState.IsValid)
                {
                    Int32 pageNumber = 1;

                    Int32 take = Core.Domain.Enumerations.ItemsPerPage;
                    Int32 skip = (pageNumber - 1) * take;

                    Core.Domain.Models.UsersModel users = this.AccountProvider.GetUsersModel(model, skip, take);

                    users.PaginationModel.CurrentPage = 1;

                    result.Object = this.Utilities.RenderPartialViewToString(this, "_UsersPartial", users);
                    result.Result = true;
                    result.Message = String.Empty;
                }
            }
            catch (Core.Domain.Exceptions.GeneralException)
            {
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
        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult ReadUsersCSVFile(String fileName)
        {
            Core.Domain.Models.UsersAddBulkModel model = this.AccountProvider.ReadBulkUsersCSVFile(Core.Utilities.Utilities.GetLoggedInUser().Id, fileName, false);

            return PartialView("_UserAddFromCSVPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ReportsAuthorize(Roles = Core.Domain.Enumerations.Role_Admin)]
        public ActionResult SubmitUsersCSVFile(Core.Domain.Models.UsersAddBulkModel model)
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = false,
                Object = String.Empty,
                Message = "An error happened during processing of your request. Please try again later"
            };

            try
            {
                if (ModelState.IsValid)
                {
                    this.AccountProvider.ReadBulkUsersCSVFile(Core.Utilities.Utilities.GetLoggedInUser().Id, model.FileName, true);
                    result.Result = true;
                    result.Message = String.Empty;
                }
            }
            catch (Core.Domain.Exceptions.GeneralException)
            {
            }
            catch (System.IO.FileNotFoundException)
            {
                result.Message = "File not found.";
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
