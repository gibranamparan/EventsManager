using System.Web;
using System.Web.Optimization;

namespace Jerry
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*SCRIPTS BUNDLES*/
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.datetimepicker.js",
                        "~/Scripts/jquery.maskedinput.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.select.min.js",
                        "~/Scripts/numeral.min.js",
                        "~/Scripts/sweetalert.min.js",
                        "~/Scripts/JavascriptRogelio.js",
                        "~/Scripts/axios.min.js",
                        "~/Scripts/notify.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                        "~/Scripts/skel.min.js",
                        "~/Scripts/util.js",
                        "~/Scripts/main.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                      "~/Scripts/moment-with-locales.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/bower").Include(
                "~/bower_components/underscore/underscore-min.js",
                "~/bower_components/bootstrap-calendar/js/calendar.js",
                "~/bower_components/bootstrap-calendar/js/language/es-MX.js"));
            
            /*STYLE BUNDLES*/
            bundles.Add(new StyleBundle("~/Content/template").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/custom.css",
                      "~/Content/jquery.datetimepicker.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/select.dataTables.min.css",
                      "~/Content/RogelioCSS.css"));

            bundles.Add(new StyleBundle("~/Content/icons").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/icomoon.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                      "~/Content/themes/base/*.css"));

            bundles.Add(new StyleBundle("~/bower_componentes/calendar").Include(
            "~/bower_components/bootstrap-calendar/css/calendar.css"));
        }
    }
}
