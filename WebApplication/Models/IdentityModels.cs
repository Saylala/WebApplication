using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<BoardModel> Boards { get; set; }

        public DbSet<ThreadModel> Threads { get; set; }

        public DbSet<PostModel> Posts { get; set; }

        public DbSet<UserInfoModel> UsersInfo { get; set; }

        public DbSet<PageStatisticsModel> PageStatistics { get; set; }
        
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}