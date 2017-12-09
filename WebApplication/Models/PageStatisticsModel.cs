using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class PageStatisticsModel
    {
        [Key]
        [StringLength(128)]
        public string Name { get; set; }

        public int AllVisits { get; set; }

        public int AllHits { get; set; }

        public DateTime CurrentDay { get; set; }

        public int VisitsToday { get; set; }

        public int HitsToday { get; set; }
    }
}