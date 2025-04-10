using System.Web;
using System.Web.Optimization;
using System;

namespace Reports.Web
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
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js", "~/Scripts/AjaxCallHelper.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            //bundles.Add(new ScriptBundle("~/Scripts/Main").Include("~/Scripts/Main.js"));

            String[] mainJs = {"~/Scripts/Libraries/jquery-{version}.js",
                        "~/Scripts/Libraries/jquery-ui.min.js",
                        "~/Scripts/Libraries/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/Libraries/jquery.validate*",
                        "~/Scripts/Libraries/jquery.ui.widget.js",
                        "~/Scripts/Libraries/mvcfoolproof.unobtrusive.min.js",
                        "~/Scripts/Libraries/MvcFoolproofJQueryValidation.min.js",
                        "~/Scripts/Libraries/modernizr-*",
                        "~/Scripts/Libraries/jquery.multi-select.js",
                        "~/Scripts/Libraries/jquery.multi-select-extra.js",
                        "~/Scripts/Libraries/evol.colorpicker.min.js",
                        "~/Scripts/Libraries/jquery.scrollbar.js",
                        "~/Scripts/Libraries/tooltipster.main.js",
                        "~/Scripts/Libraries/tooltipster.bundle.js",
                        "~/Scripts/AjaxCallHelper.js",
                        "~/Scripts/Modal.js",
                        "~/Scripts/Forms.js",
                        "~/Scripts/Tables.js",
                        "~/Scripts/MultiStepForms.js",
                        "~/Scripts/Pagination.js",                        
                        "~/Scripts/rbcontextmenu.js",
                        "~/Scripts/rbSwitchCheck.js",
                        "~/Scripts/MainMenu.js",
                        "~/Scripts/Carousel.js",
                        "~/Scripts/Main.js"
                        };

            String[] mainCss = {"~/Content/Styles/Libraries/jquery-ui.min.css",
                                "~/Content/Styles/Libraries/multi-select.css",
                                "~/Content/Styles/Libraries/multi-select-extra.css",
                                "~/Content/Styles/Libraries/evol.colorpicker.min.css",
                                "~/Content/Styles/Libraries/jquery.scrollbar.css",
                                "~/Content/Styles/Libraries/tooltipster.bundle.min.css",
                                "~/Content/Styles/Libraries/tooltipster-sideTip-shadow.min.css",
                                "~/Content/Styles/Libraries/tooltipster-sideTip-borderless.min.css",
                                "~/Content/Styles/Libraries/tooltipster-sideTip-light.min.css",
                                "~/Content/Styles/Modal.less",
                                "~/Content/Styles/Forms.less",
                                "~/Content/Styles/Tables.less",
                                "~/Content/Styles/MultiStepForms.less",
                                "~/Content/Styles/Pagination.less",
                                "~/Content/Styles/MyReportsCenter.less",
                                "~/Content/Styles/treeView.less",
                                "~/Content/Styles/MultiStepForms.less",
                                "~/Content/Styles/rbcontextmenu.less",
                                "~/Content/Styles/rbSwitchCheck.less",
                                "~/Content/Styles/Carousel.less",
                                "~/Content/Styles/Site.less"
                                };

            String[] hichartsJs = { "~/Scripts/highcharts/highcharts.js",
                        "~/Scripts/highcharts/highcharts-more.js",
                        "~/Scripts/highcharts/modules/solid-gauge.js",
                        "~/Scripts/highcharts/modules/drilldown.js",
                        "~/Scripts/highcharts/modules/wordcloud.js",
                        "~/Scripts/HighchartsTheme.js",
                        "~/Scripts/highcharts/modules/exporting.js",
                        "~/Scripts/highcharts/modules/offline-exporting.js"};

            bundles.Add(new Bundle("~/Scripts/Main").Include(mainJs));
            bundles.Add(new Bundle("~/Scripts/RequestAccess").Include(mainJs).Include("~/Scripts/RequestAccess.js"));
            bundles.Add(new Bundle("~/Scripts/Requests").Include(mainJs).Include("~/Scripts/Requests.js"));
            bundles.Add(new Bundle("~/Scripts/AskBI").Include(mainJs).Include(hichartsJs).Include("~/Scripts/AskBI.js"));
            bundles.Add(new Bundle("~/Scripts/CMOChat").Include(mainJs).Include(hichartsJs).Include("~/Scripts/CMOChat.js"));

            if (Core.Utilities.Settings.GetServerEnvironment() == Core.Domain.Enumerations.Environment.Production)
                BundleTable.EnableOptimizations = true;

            Bundle lessBundle;

            lessBundle = new LessBundle("~/Content/Styles/Main").Include(mainCss);
            lessBundle.Transforms.Add(new System.Web.Optimization.LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            lessBundle = new LessBundle("~/Content/Styles/RequestAccess").Include(mainCss).Include("~/Content/Styles/RequestAccess.less");
            lessBundle.Transforms.Add(new System.Web.Optimization.LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            lessBundle = new LessBundle("~/Content/Styles/Requests").Include(mainCss).Include("~/Content/Styles/Requests.less");
            lessBundle.Transforms.Add(new System.Web.Optimization.LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            lessBundle = new LessBundle("~/Content/Styles/AskBI").Include(mainCss).Include("~/Content/Styles/AskBI.less");
            lessBundle.Transforms.Add(new System.Web.Optimization.LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            lessBundle = new LessBundle("~/Content/Styles/CMOChat").Include(mainCss).Include("~/Content/Styles/CMOChat.less");
            lessBundle.Transforms.Add(new System.Web.Optimization.LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);
        }
    }
}
