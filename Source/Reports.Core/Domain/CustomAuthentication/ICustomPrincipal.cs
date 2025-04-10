using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace Reports.BLL.CustomAuthentication
{
    public interface ICustomPrincipal : IPrincipal
    {
        Int64 Id { get; set; }
        Guid UniqueId { get; set; }
        String Email { get; set; }
        String FirstName { get; set; }
        String LastName { get; set; }
        String NTID { get; set; }
    }
}
