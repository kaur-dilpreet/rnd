using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.UI.Controllers
{
    [Filters.ReportsAuthorize]
    public class HomeController : BaseController
    {
        private readonly Core.ErrorHandling.IErrorHandler ErrorHandler;
        private readonly Core.Utilities.ISettings Settings;
        private readonly Core.Utilities.IUtilities Utilities;
        private readonly Core.Logging.ILogger Logger;
        private readonly BLL.Providers.IHomeProvider HomeProvider;

        public HomeController(Core.ErrorHandling.IErrorHandler errorHandler,
                               Core.Utilities.ISettings settings,
                               Core.Utilities.IUtilities utilities,
                               Core.Logging.ILogger logger,
                               BLL.Providers.IHomeProvider homeProvider)
        {
            this.ErrorHandler = errorHandler;
            this.Settings = settings;
            this.Utilities = utilities;
            this.Logger = logger;
            this.HomeProvider = homeProvider;
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            ViewBag.Title = "Test Page";

            return View();
        }

        public ActionResult Index()
        {
            Int64 userId = Core.Utilities.Utilities.GetLoggedInUser().Id;

            Core.Domain.Models.HomeModel model = this.HomeProvider.GetHomeModel(userId);
            ViewBag.Title = "Home Page";

            return View(model);
        }

        [Filters.ReportsAuthorize]
        public ActionResult KeepSessionAlive()
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = true,
                Object = String.Empty,
                Message = String.Empty
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult HealthCheck()
        {
            Core.Domain.Models.JsonObjectModel<String> result = new Core.Domain.Models.JsonObjectModel<String>()
            {
                Result = true,
                Object = String.Empty,
                Message = String.Empty
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
