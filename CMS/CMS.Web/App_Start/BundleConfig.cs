using System.Web.Optimization;

namespace CMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                "~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/batchscript").Include("~/Scripts/timepicki.js"));

            bundles.Add(new ScriptBundle("~/bundles/masterfeescript").Include("~/Scripts/year-select.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/batchcss").Include(
                      "~/Content/timepicki.css",
                      "~/Content/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/StudentCSS").Include(
                      "~/Content/bootstrap-multiselect.css"));

            bundles.Add(new ScriptBundle("~/bundles/StudentJS").Include(
                      "~/Scripts/bootstrap-multiselect.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuestionJS").Include(
                      "~/Scripts/query.unobtrusive-ajax.min.js",
                      "~/Scripts/fileinput.min.js",
                      "~/Scripts/dependent-dropdown.min.js"));

            bundles.Add(new StyleBundle("~/Content/QuestionCSS").Include(
                     "~/Content/fileinput.min.css",
                     "~/Content/dependent-dropdown.min.css"));

            bundles.Add(new StyleBundle("~/Content/StudentPhotoCropCSS").Include(
                    "~/Content/jquery.Jcrop.css"));

            bundles.Add(new ScriptBundle("~/bundles/StudentPhotoCropJS").Include(
                    "~/Scripts/jquery.Jcrop.js"));

            bundles.Add(new ScriptBundle("~/bundles/InstallmentJS").Include(
                     "~/Scripts/date.format.js"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
