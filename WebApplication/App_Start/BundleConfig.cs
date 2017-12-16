using System.Web.Optimization;

namespace WebApplication
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/gallery").Include("~/Scripts/gallery.js"));
            bundles.Add(new ScriptBundle("~/Scripts/forum").Include("~/Scripts/forum.js"));
            bundles.Add(new ScriptBundle("~/Scripts/screen").Include("~/Scripts/screen.js"));
            bundles.Add(new ScriptBundle("~/Scripts/ajax").Include("~/Scripts/ajax.js"));
            bundles.Add(new ScriptBundle("~/Scripts/ajax-update-thread").Include("~/Scripts/ajax-update-thread.js"));

            bundles.Add(new StyleBundle("~/Styles/bootstrap").Include("~/Styles/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Styles/font-awesome").Include("~/Styles/font-awesome.css"));
            bundles.Add(new StyleBundle("~/Styles/custom").Include("~/Styles/custom.css"));
            bundles.Add(new StyleBundle("~/Styles/gallery").Include("~/Styles/gallery.css"));
            bundles.Add(new StyleBundle("~/Styles/forum").Include("~/Styles/forum.css"));
        }
    }
}
