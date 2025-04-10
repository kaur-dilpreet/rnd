using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Reports.Workers.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{*less}", new { less = @".*\.less(/.*)?" });
            routes.IgnoreRoute("{*css}", new { css = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*js}", new { js = @".*\.js(/.*)?" });
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("content/{*content}");
            routes.IgnoreRoute("scripts/{*scripts}");
            routes.IgnoreRoute("pf_auth/{*pathInfo}");

            routes.RouteExistingFiles = true;

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
