using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class VisitModel
    {
        [Key]
        [StringLength(36)]
        public string UserId { get; set; }

        public DateTime LastVisit { get; set; }
    }
}