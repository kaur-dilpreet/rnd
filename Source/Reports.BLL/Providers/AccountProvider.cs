using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace Reports.BLL.Providers
{
    public class AccountProvider : IAccountProvider
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Data.Repositories.IUsersRepository UsersRepo;
        private readonly Core.Logging.ILogger Logger;

        public AccountProvider(Core.ErrorHandling.IErrorHandler errorHandler,
                               Core.Utilities.ISettings settings,
                               Core.Utilities.IUtilities utilities,
                               Data.Repositories.IUsersRepository usersRepo,
                               Core.Logging.ILogger logger)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.UsersRepo = usersRepo;
            this.Logger = logger;
        }

        public void UpdateUserTimezone(Int64 id, Int32 timezoneOffset)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.GetAll().Where(u => u.Id == id).FirstOrDefault();

                if (user == null)
                    return;

                user.TimezoneOffset = timezoneOffset;

                this.UsersRepo.SaveOrUpdate(user);
            }
            catch (Exception)
            {

            }
        }

        public void AddUser(String email, String firstName, String lastName,String ntID)
        {
            try
            {
                Core.Domain.Entities.User user = new Core.Domain.Entities.User()
                {
                    UniqueId = Guid.NewGuid(),
                    RoleId = (Byte)Core.Domain.Enumerations.UserRoles.User,
                    Email = email.ToLower().Trim(),
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    NTID = ntID.Trim(),
                    TimezoneOffset = 0,
                    LastActivityDateTime = DateTime.UtcNow,
                    CreationDateTime = DateTime.UtcNow,
                    LastModificationDateTime = DateTime.UtcNow
                };

                this.UsersRepo.SaveOrUpdate(user);

                user.CreatedByUserId = user.Id;
                user.LastModifiedByUserId = user.Id;

                this.UsersRepo.SaveOrUpdate(user);
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("email", email);
                methodParam.Add("firstName", firstName);
                methodParam.Add("lastName", lastName);

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void UpdateUser(String email, String firstName, String lastName,String ntID)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.GetAll().First(u => u.Email == email.ToLower());

                if (user == null)
                    return;

                user.FirstName = firstName.Trim();
                user.LastName = lastName.Trim();
                user.NTID = ntID.Trim();
                user.LastModifiedByUserId = user.Id;

                this.UsersRepo.SaveOrUpdate(user);
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("email", email);
                methodParam.Add("firstName", firstName);
                methodParam.Add("lastName", lastName);

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Boolean UserExist(String email)
        {
            try
            {
                return this.UsersRepo.GetAll().Any(u => u.Email == email.ToLower());
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("email", email);
                
                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }
        public Core.Domain.Entities.User GetUser(String email, Boolean isOnline)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.GetAll().Where(u => u.Email == email.ToLower()).FirstOrDefault();

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                if (isOnline)
                {
                    user.LastActivityDateTime = DateTime.UtcNow;
                    this.UsersRepo.SaveOrUpdate(user);
                }

                return user;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("email", email);
                methodParam.Add("isOnline", isOnline.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        

        public Core.Domain.Entities.User GetUser(Int64 userId, Boolean isOnline)
        {
            try
            {
                Core.Domain.Entities.User user = this.UsersRepo.Get(userId);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                if (isOnline)
                {
                    user.LastActivityDateTime = DateTime.UtcNow;
                    this.UsersRepo.SaveOrUpdate(user);
                }

                return user;
            }
            catch (Core.Domain.Exceptions.GeneralException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                NameValueCollection methodParam = new NameValueCollection();
                methodParam.Add("userId", userId.ToString());
                methodParam.Add("isOnline", isOnline.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.UsersModel GetUsersModel()
        {
            return this.GetUsersModel(null, -1, -1);
        }

        public Core.Domain.Models.UsersModel GetUsersModel(Int32 skip, Int32 take)
        {
            return this.GetUsersModel(null, skip, take);
        }

        public Core.Domain.Models.UsersModel GetUsersModel(Core.Domain.Models.UsersSearchModel searchModel)
        {
            return this.GetUsersModel(searchModel, -1, -1);
        }

        public Core.Domain.Models.UsersModel GetUsersModel(Core.Domain.Models.UsersSearchModel searchModel, Int32 skip, Int32 take)
        {
            try
            {
                Core.Domain.Models.UsersModel model = new Core.Domain.Models.UsersModel();

                if (searchModel == null)
                    searchModel = new Core.Domain.Models.UsersSearchModel();
                else
                    model.SearchModel = searchModel;

                if (String.IsNullOrEmpty(searchModel.SearchCriteria))
                    searchModel.SearchCriteria = String.Empty;

                IQueryable<Core.Domain.Entities.User> tempUsers = this.UsersRepo.GetAll().Where(u => String.IsNullOrEmpty(searchModel.SearchCriteria) || u.Email.Contains(searchModel.SearchCriteria.ToLower()) || u.FirstName.ToLower().Contains(searchModel.SearchCriteria.ToLower()) || u.LastName.ToLower().Contains(searchModel.SearchCriteria.ToLower())).OrderBy(u => u.RoleId).OrderBy(u => u.FirstName).OrderBy(u => u.LastName);

                IQueryable<Core.Domain.Entities.User> users = null;

                if (take >=0 && skip >= 0)
                {
                    users = tempUsers.Skip(skip).Take(take);
                }
                else
                {
                    users = tempUsers;
                }

                model.Users = users.Select(u => new Core.Domain.Models.UserListModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = String.Format("{0} {1}", u.FirstName, u.LastName),
                    UserRole = u.RoleId,
                    LastActivityDateTime = u.LastActivityDateTime
                });

                model.PaginationModel = new Core.Domain.Models.PaginationModel()
                {
                    Count = tempUsers.Count(),
                    PageSize = Core.Domain.Enumerations.ItemsPerPage,
                    RefreshUrl = "/account/getusers",
                    Target = "#users"
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

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public Core.Domain.Models.UserAddModel GetUserAddModel()
        {
            try
            {
                Core.Domain.Models.UserAddModel model = new Core.Domain.Models.UserAddModel();

                model.UserRoles = new List<KeyValuePair<Byte, String>>()
                {
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.Admin, "Admin"),
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.PowerUser, "Power User"),
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.User, "User"),
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
                
                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }

        public void AddUser(Int64 userId, Core.Domain.Models.UserAddModel model)
        {
            try
            {
                Core.Domain.Entities.User user = new Core.Domain.Entities.User()
                {
                    Email = model.Email.ToLower(),
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    NTID = String.Empty,
                    CreationDateTime = DateTime.UtcNow,
                    LastModificationDateTime = DateTime.UtcNow, 
                    CreatedByUserId = userId, 
                    LastModifiedByUserId = userId,
                    LastActivityDateTime = null,
                    RoleId = model.UserRoleId
                };

                this.UsersRepo.SaveOrUpdate(user);
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

        public Core.Domain.Models.UserUpdateModel GetUserUpdateModel(Int64 userId)
        {
            try
            {
                Core.Domain.Entities.User user = this.GetUser(userId, false);

                Core.Domain.Models.UserUpdateModel model = new Core.Domain.Models.UserUpdateModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = String.Format("{0} {1}", user.FirstName, user.LastName),
                    UserRoleId = user.RoleId
                };

                model.UserRoles = new List<KeyValuePair<Byte, String>>()
                {
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.Admin, "Admin"),
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.PowerUser, "Power User"),
                    new KeyValuePair<Byte, String>((Byte)Core.Domain.Enumerations.UserRoles.User, "User"),
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

        public void UpdateUser(Int64 userId, Core.Domain.Models.UserUpdateModel model)
        {
            try
            {
                Core.Domain.Entities.User user = this.GetUser(model.Id, false);

                if (user == null)
                    throw new Core.Domain.Exceptions.ItemNotFoundException("User");

                user.RoleId = model.UserRoleId;

                this.UsersRepo.SaveOrUpdate(user);
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

        public Core.Domain.Models.UsersAddBulkModel ReadBulkUsersCSVFile(Int64 userId, String fileName, Boolean shouldSaveToDB)
        {
            try
            {
                String file = System.Web.HttpContext.Current.Server.MapPath(fileName);

                if (!System.IO.File.Exists(file))
                    throw new System.IO.FileNotFoundException();

                Core.Domain.Models.UsersAddBulkModel model = new Core.Domain.Models.UsersAddBulkModel()
                {
                    FileName = fileName
                };

                System.IO.StreamReader reader = new System.IO.StreamReader(file);

                String usersString = reader.ReadToEnd().ToLower();

                String[] users = usersString.Split('\n');

                Regex rgx = new Regex(Core.Domain.Enumerations.EmailAddressRegEx);

                foreach (String user in users)
                {
                    String[] fields = user.Split(',');

                    Core.Domain.Models.UserAddBulkModel userModel = new Core.Domain.Models.UserAddBulkModel()
                    {
                        Email = fields[0],
                        IsValid = true,
                        UserAlreadyExists = false,
                        DuplicateUser = false,
                        InvalidEmail = false
                    };

                    if (!rgx.IsMatch(fields[0]))
                    {
                        userModel.IsValid = false;
                        userModel.InvalidEmail = true;
                    }
                    else
                    {
                        if (model.Users.Any(u => u.Email == fields[0]))
                        {
                            userModel.IsValid = false;
                            userModel.DuplicateUser = true;
                        }
                    }

                    if (this.UsersRepo.GetAll().Any(u => u.Email == fields[0]))
                    {
                        userModel.IsValid = false;
                        userModel.UserAlreadyExists = true;
                    }

                    model.Users.Add(userModel);

                    if (shouldSaveToDB)
                    {
                        if (userModel.IsValid || userModel.UserAlreadyExists)
                        {
                            Core.Domain.Entities.User newUser = null;

                            if (!userModel.UserAlreadyExists)
                            {
                                newUser = new Core.Domain.Entities.User()
                                {
                                    Email = userModel.Email.ToLower(),
                                    FirstName = String.Empty,
                                    LastName = String.Empty,
                                    NTID = String.Empty,
                                    CreatedByUserId = userId,
                                    CreationDateTime = DateTime.UtcNow,
                                    LastModifiedByUserId = userId,
                                    LastModificationDateTime = DateTime.UtcNow,
                                    UniqueId = Guid.NewGuid(),
                                    RoleId = (Byte)Core.Domain.Enumerations.UserRoles.User
                                };

                                this.UsersRepo.SaveOrUpdate(newUser);
                            }
                            else
                            {
                                newUser = this.UsersRepo.GetAll().Where(u => u.Email.ToLower() == userModel.Email.ToLower()).FirstOrDefault();
                            }
                        }
                    }
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
                methodParam.Add("fileName", fileName);
                methodParam.Add("shouldSaveToDB", shouldSaveToDB.ToString());

                throw this.ErrorHandler.HandleError(ex, String.Format("BLL.{0}", this.GetType().Name), (new System.Diagnostics.StackFrame()).GetMethod().Name, methodParam);
            }
        }
    }
}
