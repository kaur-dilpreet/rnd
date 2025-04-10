using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface IAccountProvider
    {
        void UpdateUserTimezone(Int64 id, Int32 timezoneOffset);
        void AddUser(String email, String firstName, String lastName, String ntID);
        void UpdateUser(String email, String firstName, String lastName, String ntID);
        Boolean UserExist(String email);
        Core.Domain.Entities.User GetUser(String email, Boolean isOnline);
        Core.Domain.Entities.User GetUser(Int64 userId, Boolean isOnline);
        
        Core.Domain.Models.UsersModel GetUsersModel(Int32 skip, Int32 take);
        Core.Domain.Models.UsersModel GetUsersModel();
        Core.Domain.Models.UsersModel GetUsersModel(Core.Domain.Models.UsersSearchModel searchModel);
        Core.Domain.Models.UsersModel GetUsersModel(Core.Domain.Models.UsersSearchModel searchModel, Int32 skip, Int32 take);
        Core.Domain.Models.UserAddModel GetUserAddModel();
        void AddUser(Int64 userId, Core.Domain.Models.UserAddModel model);
        Core.Domain.Models.UserUpdateModel GetUserUpdateModel(Int64 userId);
        void UpdateUser(Int64 userId, Core.Domain.Models.UserUpdateModel model);
        Core.Domain.Models.UsersAddBulkModel ReadBulkUsersCSVFile(Int64 userId, String fileName, Boolean shouldSaveToDB);
        
    }
}
