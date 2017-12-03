using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class VisitsRepo
    {
        private readonly ApplicationDbContext db;

        public VisitsRepo() : this(new ApplicationDbContext())
        {
        }

        public VisitsRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddOrUpdateVisit(VisitModel visit)
        {
            db.Visits.AddOrUpdate(visit);
            await db.SaveChangesAsync();
        }

        public async Task AddOrUpdateStatistics(string name)
        {
            var pageStatistics = db.PageStatistics.Find(name);
            if (pageStatistics == null)
                pageStatistics = new PageStatisticsModel
                {
                    Name = name,
                    AllVisits = 1,
                    CurrentDay = DateTime.Today,
                    VisitsToday = 1
                };
            else
            {
                pageStatistics.AllVisits++;
                if (pageStatistics.CurrentDay.Date == DateTime.Today)
                    pageStatistics.VisitsToday++;
                else
                {
                    pageStatistics.CurrentDay = DateTime.Today;
                    pageStatistics.VisitsToday = 1;
                }
            }
            db.PageStatistics.AddOrUpdate(pageStatistics);
            await db.SaveChangesAsync();
        }

        public VisitModel GetVisit(string userId)
        {
            return db.Visits.Find(userId);
        }

        public PageStatisticsModel GetPageStatistics(string name)
        {
            return db.PageStatistics.Find(name);
        }
    }
}