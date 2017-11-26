using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class ThreadModel
    {
        [Key]
        public int Id { get; set; }

        public string BoardId { get; set; }

        public IEnumerable<PostModel> Posts;
        public string BoardName;
    }
}
