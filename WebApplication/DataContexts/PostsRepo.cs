using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.DataContexts
{
    public class PostsRepo
    {
        private readonly ApplicationDbContext db;

        public PostsRepo() : this(new ApplicationDbContext())
        {
        }

        public PostsRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddPost(PostModel post)
        {
            db.Posts.Add(post);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException)
            {
            }
        }

        public IEnumerable<PostModel> GetPosts(int threadId)
        {
            return db.Posts.Where(x => x.ThreadId == threadId);
        }

        public async Task DeletePost(int postId)
        {
            var post = db.Posts.Find(postId);
            if (post == null)
                return;
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
    }
}
