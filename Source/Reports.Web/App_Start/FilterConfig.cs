﻿using System.Web;
using System.Web.Mvc;

namespace Reports.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new UI.Filters.XFrameOptionsAttribute());
            //filters.Add(new UI.Filters.RequireHttpsOnProd());
        }
    }
}
