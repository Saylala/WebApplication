using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View("Error",
                new ErrorModel
                {
                    Error = "404 Not Found",
                    Description = "Requested page not found!"
                });
        }

        public ActionResult InternalServerError()
        {
            Response.StatusCode = 500;
            return View("Error",
                new ErrorModel
                {
                    Error = "Internal server error",
                    Description = "Something seems to be broken!"
                });
        }
    }
}