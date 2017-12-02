using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class PostModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(32)]
        public string Username { get; set; }

        [StringLength(32)]
        public string Topic { get; set; }

        [StringLength(2000)]
        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public int ThreadId { get; set; }
    }
}
