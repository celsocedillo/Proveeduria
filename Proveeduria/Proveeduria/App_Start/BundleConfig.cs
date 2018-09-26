using System.Web;
using System.Web.Optimization;

namespace Proveduria
{
    public class BundleConfig
    {
        // For more information** on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/Bodega").Include("~/Scripts/App/Bodega.js"));
            bundles.Add(new ScriptBundle("~/bundles/Consulta").Include("~/Scripts/App/Consulta.js"));
            bundles.Add(new ScriptBundle("~/bundles/InvMedida").Include("~/Scripts/App/InvMedida.js"));
            bundles.Add(new ScriptBundle("~/bundles/Item").Include("~/Scripts/App/Item.js"));
            bundles.Add(new ScriptBundle("~/bundles/ListaItem").Include("~/Scripts/App/ListaItem.js"));
            bundles.Add(new ScriptBundle("~/bundles/ListaMovimiento").Include("~/Scripts/App/ListaMovimiento.js"));
            bundles.Add(new ScriptBundle("~/bundles/ListaSolicitud").Include("~/Scripts/App/ListaSolicitud.js"));
            bundles.Add(new ScriptBundle("~/bundles/Medida").Include("~/Scripts/App/Medida.js"));
            bundles.Add(new ScriptBundle("~/bundles/Movimiento").Include("~/Scripts/App/Movimiento.js"));
            bundles.Add(new ScriptBundle("~/bundles/Solicitud").Include("~/Scripts/App/Solicitud.js"));
            bundles.Add(new ScriptBundle("~/bundles/TipoMovimiento").Include("~/Scripts/App/TipoMovimiento.js"));
            bundles.Add(new ScriptBundle("~/bundles/Util").Include("~/Scripts/App/Util.js"));
            //BundleTable.EnableOptimizations = true;

        }
    }
}
