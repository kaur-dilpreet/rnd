using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.UI.Filters
{
    public class RequireHttpsOnProd : System.Web.Mvc.RequireHttpsAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentException("filterContext");

            Core.Domain.Enumerations.Environment environment = Core.Utilities.Settings.GetServerEnvironment();

            if (environment == Core.Domain.Enumerations.Environment.Dev)
                return;

            base.OnAuthorization(filterContext);
        }
    }
}
