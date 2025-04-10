using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Reports.UI.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Error403()
        {
            return Redirect("/requestaccess");
        }
    }
}
