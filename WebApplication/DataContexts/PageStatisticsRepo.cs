using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class PageStatisticsRepo
    {
        private readonly ApplicationDbContext db;

        public PageStatisticsRepo() : this(new ApplicationDbContext())
        {
        }

        public PageStatisticsRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddOrUpdateStatistics(string name, bool newVisit)
        {
            var pageStatistics = db.PageStatistics.Find(name);
            if (pageStatistics == null)
                pageStatistics = new PageStatisticsModel
                {
                    Name = name,
                    AllVisits = 1,
                    AllHits = 1,
                    CurrentDay = (DateTime.UtcNow + TimeSpan.FromHours(5)).Date,
                    VisitsToday = 1,
                    HitsToday = 1
                };
            else
            {
                if (newVisit)
                    pageStatistics.AllVisits++;
                pageStatistics.AllHits++;
                if (pageStatistics.CurrentDay.Date == (DateTime.UtcNow + TimeSpan.FromHours(5)).Date)
                {
                    if (newVisit)
                        pageStatistics.VisitsToday++;
                    pageStatistics.HitsToday++;
                }
                else
                {
                    pageStatistics.CurrentDay = (DateTime.UtcNow + TimeSpan.FromHours(5)).Date;
                    pageStatistics.VisitsToday = 1;
                    pageStatistics.HitsToday = 1;
                }
            }
            db.PageStatistics.AddOrUpdate(pageStatistics);
            await db.SaveChangesAsync();
        }

        public PageStatisticsModel GetPageStatistics(string name)
        {
            return db.PageStatistics.Find(name);
        }
    }
}