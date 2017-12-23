using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class UsersInfoRepo
    {
        private readonly ApplicationDbContext db;

        public UsersInfoRepo() : this(new ApplicationDbContext())
        {
        }

        public UsersInfoRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddOrUpdateInfo(UserInfoModel userInfo)
        {
            db.UsersInfo.AddOrUpdate(userInfo);
            await db.SaveChangesAsync();
        }

        public UserInfoModel GetInfo(string userId)
        {
            return db.UsersInfo.Find(userId);
        }
    }
}