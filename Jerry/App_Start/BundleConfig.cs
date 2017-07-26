using System.Web;
using System.Web.Optimization;

namespace Jerry
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*",
            //            "~/Scripts/jquery.dataTables.min.js"));

            /*SCRIPTS BUNDLES*/
            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                        "~/Scripts/skel.min.js",
                        "~/Scripts/util.js",
                        "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.maskedinput.js",
                        "~/Scripts/jquery.metisMenu.js",
                        "~/Scripts/morris/morris.js",
                        "~/Scripts/morris/raphael-2.1.0.min.js",
                        "~/Scripts/jquery.datetimepicker.full.min.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/custom.js",
                        "~/Scripts/JavascriptRogelio.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            /*STYLE BUNDLES*/
            bundles.Add(new StyleBundle("~/Content/template").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/custom.css",
                      "~/Content/jquery.datetimepicker.min.css",
                      //"//cdn.datatables.net/1.10.15/css/jquery.dataTables.min.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/RogelioCSS.css"));

            bundles.Add(new StyleBundle("~/Content/icons").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/icomoon.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                      "~/Content/themes/base/*.css"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
        }
    }
}
