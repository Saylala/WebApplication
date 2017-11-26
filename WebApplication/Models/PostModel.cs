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

        public string Username;

        [StringLength(32)]
        public string Topic { get; set; }

        [StringLength(200)]
        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public int ThreadId { get; set; }
    }
}
