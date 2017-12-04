using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class ThreadsRepo
    {
        private readonly ApplicationDbContext db;

        public ThreadsRepo() : this(new ApplicationDbContext())
        {
        }

        public ThreadsRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<ThreadModel> AddThread(ThreadModel thread)
        {
            var added = db.Threads.Add(thread);
            await db.SaveChangesAsync();
            return added;
        }

        public IEnumerable<ThreadModel> GetThreads(string boardId)
        {
            return db.Threads.Where(x => x.BoardId == boardId);
        }

        public ThreadModel GetThread(int threadId)
        {
            return db.Threads.Find(threadId);
        }

        public async Task DeleteThread(int threadId)
        {
            var thread = db.Threads.Find(threadId);
            if (thread == null)
                return;
            db.Posts.RemoveRange(db.Posts.Where(p => p.ThreadId == threadId));
            db.Threads.Remove(thread);
            await db.SaveChangesAsync();
        }
    }
}
