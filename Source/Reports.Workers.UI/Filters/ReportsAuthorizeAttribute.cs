﻿using System;
using System.Web.Mvc;

namespace Reports.Workers.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TAAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Result = false,
                            Message = "You do not have access to the requested resource.",
                            ErrorCode = 403
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Result = false,
                            Message = "Your session has been expired.",
                            ErrorCode = 401
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            else
            {
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("/error403");
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
            }
        }
    }
}
