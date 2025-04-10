using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.BLL.Providers
{
    public interface IRequestAccessProvider
    {
        Core.Domain.Models.RequestAccessModel GetRequestAccessModel(Int64 userId);
        Core.Domain.Models.UserRequest GetUserRequest(Int64 userId);
        void UpdateUserRequest(Int64 userId, Core.Domain.Models.UserRequest model);
        Core.Domain.Models.AccessRequestsModel GetAccessRequestsModel();
        void RequestAccess(Int64 userId, Boolean accessAskBI, Boolean accessCMOChat, Boolean accessSDRAI, Boolean accessChatGPI);
        Core.Domain.Models.AccessRequestModel ChangeAccessLevel(Int64 userId, Core.Domain.Models.ChangeAccessLevelModel model);
        Core.Domain.Models.ChangeAccessLevelModel GetChangeAccessLevelModel(Int64 userId, Guid requestId);
        Core.Domain.Models.DenyRequestModel GetDenyRequestModel(Int64 userId, Guid requestId);
        Core.Domain.Models.AccessRequestModel DenyRequest(Int64 userId, Guid requestId, String reason);
    }
}
