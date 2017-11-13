using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly GalleryModel gallery = new GalleryModel
        {
            MaxImages = 6,
            Images = new[]
            {
                "/Images/1.jpg",
                "/Images/2.jpg",
                "/Images/3.jpg",
                "/Images/4.jpg",
                "/Images/5.jpg",
                "/Images/6.jpg",
            },
            ImagePreviews = new[]
            {
                "/Images/1_thumbnail.jpg",
                "/Images/2_thumbnail.jpg",
                "/Images/3_thumbnail.jpg",
                "/Images/4_thumbnail.jpg",
                "/Images/5_thumbnail.jpg",
                "/Images/6_thumbnail.jpg",
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