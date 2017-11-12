using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly GalleryModel gallery = new GalleryModel
        {
            MaxImages = 3,
            Images = new[]
            {
                "/Images/City.jpg",
                "/Images/Galaxy.jpg",
                "/Images/Lake.jpg",
                "/Images/Planet.jpg"
            },
            ImagePreviews = new[]
            {
                "/Images/City.jpg",
                "/Images/Galaxy.jpg",
                "/Images/Lake.jpg",
                "/Images/Planet.jpg"
            }
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View(gallery);
        }
    }
}