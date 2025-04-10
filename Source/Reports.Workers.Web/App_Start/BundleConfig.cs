using System.Web;
using System.Web.Optimization;
using System;

namespace Reports.Workers.Web
{
    //public class LessTransform : IBundleTransform
    //{
    //    public void Process(BundleContext context, BundleResponse response)
    //    {
    //        response.Content = dotless.Core.Less.Parse(response.Content);
    //        response.ContentType = "text/css";
    //    }
    //}
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            
        }
    }
}
