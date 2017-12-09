using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class UserInfoModel
    {
        [Key]
        [StringLength(36)]
        public string UserId { get; set; }

        public string BrowserInfo { get; set; }

        public string ResolutionWidth { get; set; }

        public string ResolutionHeight { get; set; }

        public DateTime LastVisit { get; set; }
    }
}