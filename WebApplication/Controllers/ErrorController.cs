using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("Error");
        }
    }
}