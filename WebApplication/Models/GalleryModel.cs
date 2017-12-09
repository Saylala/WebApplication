namespace WebApplication.Models
{
    public class GalleryModel
    {
        public int MaxImages { get; set; }

        public string[] Images { get; set; }

        public string[] ImagePreviews { get; set; }

        public string[] GalleryHelp { get; set; }
    }
}