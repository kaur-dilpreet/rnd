using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.UI.Filters
{
    public class XFrameOptionsAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnResultExecuting(System.Web.Mvc.ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("X-Frame-Options", "ALLOW-FROM https://insights.its.hpecorp.net");
        }
    }
}
