using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start()
        {
            // Some crutch so that Session_End would fire
            Session["dummy"] = 0;
            Session["counterUpdated"] = false;

            if (Response.Cookies["auth"] == null)
            {
                var id = Guid.NewGuid();
                var cookie = new HttpCookie("auth");
                cookie.Values.Add("id", id.ToString());
                cookie.Expires = DateTime.MaxValue;
                Response.Cookies.Add(cookie);
            }
        }

        protected void Session_End()
        {
            Session["dummy"] = 0;
        }
    }
}
