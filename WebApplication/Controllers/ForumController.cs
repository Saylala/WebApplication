using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using WebApplication.DataContexts;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ForumController : Controller
    {
        private ApplicationUserManager UserManager => Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
        private readonly BoardsRepo boards = new BoardsRepo();
        private readonly ThreadsRepo threads = new ThreadsRepo();
        private readonly PostsRepo posts = new PostsRepo();

        public ActionResult Index()
        {
            return View(boards.GetBoards());
        }

        [HttpPost]
        public async Task<ActionResult> AddBoard(string shortName, string name)
        {
            await boards.AddBoard(new BoardModel { ShortName = shortName, Name = name });
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddThread(string boardId, string name, string text)
        {
            var thread = await threads.AddThread(new ThreadModel { BoardId = boardId });
            await posts.AddPost(new PostModel
            {
                ThreadId = thread.Id,
                Text = text,
                Topic = name,
                Timestamp = DateTime.Now,
                UserId = User.Identity.GetUserId()
            });
            return RedirectToAction("Board", new { boardId = boardId });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddPost(int threadId, string text)
        {
            await posts.AddPost(new PostModel
            {
                ThreadId = threadId,
                Text = text,
                Timestamp = DateTime.Now,
                UserId = User.Identity.GetUserId()
            });
            return RedirectToAction("Thread", new { threadId = threadId });
        }

        [HttpPost]
        public ActionResult GetNewPosts(int threadId, int currentCount)
        {
            var posts = this.posts.GetPosts(threadId).ToList();
            if (posts.Count == currentCount)
                return Json(new
                {
                    Posts = new PostModel[0],
                    UserId = User.Identity.GetUserId(),
                });
            return Json(new
            {
                Posts = posts.Skip(currentCount).Select(p => new
                {
                    UserId = p.UserId,
                    Username = UserManager.Users
                        .ToList()
                        .FirstOrDefault(u => u.Id == p.UserId)
                        ?.UserName,
                    Id = p.Id,
                    Timestamp = p.Timestamp.ToString(),
                    Topic = p.Topic,
                    Text = p.Text,
                    ThreadId = p.ThreadId
                }),
                UserId = User.Identity.GetUserId(),
            });
        }

        public async Task<ActionResult> Board(string boardId)
        {
            var board = await boards.GetBoard(boardId);
            var threads = this.threads.GetThreads(boardId).ToList();
            foreach (var thread in threads)
            {
                var posts = this.posts.GetPosts(thread.Id).ToList();
                foreach (var post in posts.Take(1))
                    post.Username = UserManager.Users
                        .ToList()
                        .FirstOrDefault(x => x.Id == post.UserId)
                        ?.UserName;
                thread.Posts = posts;
            }
            board.Threads = threads;
            return View(board);
        }

        public async Task<ActionResult> Thread(int threadId)
        {
            var thread = threads.GetThread(threadId);
            var posts = this.posts.GetPosts(thread.Id).ToList();
            foreach (var post in posts)
                post.Username = UserManager.Users
                    .ToList()
                    .FirstOrDefault(x => x.Id == post.UserId)
                    ?.UserName;
            thread.Posts = posts;
            thread.BoardName = (await boards.GetBoard(thread.BoardId)).Name;
            return View(thread);
        }
    }
}
