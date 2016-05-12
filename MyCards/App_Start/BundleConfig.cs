using System.Web;
using System.Web.Optimization;

namespace MyCards
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/addedJs").Include(
                       "~/Scripts/jquery-1.10.2.js",
                       "~/Scripte/site/navMobile.js",
                       "~/Scripts/site/site.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/starRating/star-rating.js",
                       "~/Scripts/site/rating.js"
                      ));

            //bundles.Add(new ScriptBundle("~/bundles/gmaps").Include(
            //         "~/Scripts/gmaps.js",
            //        "~/Scripts/site/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/bootstrap-toggle/bootstrap2-toggle.min.css",
                      "~/Content/starRating/star-rating.css",
                      "~/Content/starRating/theme-krajee-fa.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/rating.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap-toggle").Include(
                      "~/Content/bootstrap-toggle/bootstrap2-toggle.min.js"));

        }
    }
}
