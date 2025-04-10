using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Web.Security;

namespace Reports.BLL.CustomAuthentication
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            String[] roles = role.Split(',');

            foreach (String userRole in roles)
            {
                if (userRole.ToLower().Trim().StartsWith("role_"))
                {
                    switch (userRole.Trim())
                    {
                        case Core.Domain.Enumerations.Role_Admin:
                            if (this.RoleId == (Byte)Core.Domain.Enumerations.UserRoles.Admin)
                                return true;

                            break;
                        case Core.Domain.Enumerations.Role_PowerUser:
                            if (this.RoleId == (Byte)Core.Domain.Enumerations.UserRoles.PowerUser)
                                return true;

                            break;
                        case Core.Domain.Enumerations.Role_User:
                            if (this.RoleId == (Byte)Core.Domain.Enumerations.UserRoles.User)
                                return true;

                            break;
                        case Core.Domain.Enumerations.Role_AskBI:
                            return this.AskBIAccess;
                        case Core.Domain.Enumerations.Role_CMOChat:
                            return this.CMOChatAccess;
                        case Core.Domain.Enumerations.Role_SDRAI:
                            return this.SDRAIAccess;
                        case Core.Domain.Enumerations.Role_ChatGPI:
                            return this.ChatGPIAccess;
                    }
                }
            }

            return false;
        }

        public Int64 Id { get; set; }
        public Guid UniqueId { get; set; }
        public Byte RoleId { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String NTID { get; set; }
        public Int32 TimezoneOffset { get; set; }
        public Boolean DisableDefaultProfile { get; set; }
        public Boolean IsApproved { get; set; }
        public Boolean AskBIAccess { get; set; }
        public Boolean SDRAIAccess { get; set; }
        public Boolean CMOChatAccess { get; set; }
        public Boolean ChatGPIAccess { get; set; }
        public String FullName
        {
            get
            {
                return String.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }
    }
}
