using System.Web;
using System.Web.Optimization;

namespace DemandMetalFab
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
           
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            #region chartjs

            bundles.Add(new ScriptBundle("~/bundles/ChartJS").Include(
                      "~/Scripts/Chart.bundle.js",
                      "~/Scripts/Chart.bundle.min.js",
                      "~/Scripts/Chart.js",
                      "~/Scripts/Chart.min.js",
                      "~/Scripts/chartjs-plugin-datalabels.js",
                      "~/Scripts/chartjs-plugin-datalabels.min.js"
                      ));

            #endregion
            #region datatables

            bundles.Add(new StyleBundle("~/Content/DataTablesCss").Include(
                     "~/Content/datatables/css/jquery.dataTables.css"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                      "~/Content/datatables/js/jquery.dataTables.js",
                      "~/Content/datatables/js/jquery.dataTables.min.js"
                      ));

            #endregion
            #region blockui

            bundles.Add(new ScriptBundle("~/bundles/Block").Include(
                     "~/Scripts/jquery.blockUI.js"));

            #endregion

            

        }
    }
}
