using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using WebApplication.DataContexts;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ForumController : Controller
    {
        private readonly BoardsRepo boards = new BoardsRepo();
        private readonly ThreadsRepo threads = new ThreadsRepo();
        private readonly PostsRepo posts = new PostsRepo();
        private readonly VisitsRepo visits = new VisitsRepo();

        public async Task<ActionResult> Index()
        {
            var userId = Request.Cookies["auth"]?["id"];
            await UpdateVisitStatistics(userId);
            var allBoards = boards.GetBoards();
            var pageStatistics = visits.GetPageStatistics("Forum");
            var visit = default(VisitModel);
            if (Request.Cookies["auth"] != null)
                visit = visits.GetVisit(Request.Cookies["auth"]["id"]);
            return View(Tuple.Create(allBoards, pageStatistics, visit));
        }

        public async Task<ActionResult> Board(string boardId)
        {
            var userId = Request.Cookies["auth"]?["id"];
            await UpdateVisitStatistics(userId);
            var board = await boards.GetBoard(boardId);
            var boardThreads = threads.GetThreads(boardId).ToList();
            foreach (var thread in boardThreads)
            {
                var threadPosts = posts.GetPosts(thread.Id).ToList();
                thread.Posts = threadPosts;
            }
            board.Threads = boardThreads;
            return View(board);
        }

        public async Task<ActionResult> Thread(int threadId)
        {
            var userId = Request.Cookies["auth"]?["id"];
            await UpdateVisitStatistics(userId);
            var thread = threads.GetThread(threadId);
            var threadPosts = posts.GetPosts(thread.Id).ToList();
            thread.Posts = threadPosts;
            thread.BoardName = (await boards.GetBoard(thread.BoardId)).Name;
            return View(thread);
        }

        [HttpPost]
        public async Task<ActionResult> AddBoard(string shortName, string name)
        {
            await boards.AddBoard(new BoardModel { ShortName = shortName, Name = name });
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddThread(string boardId, string name, string subject, string text)
        {
            if (!CheckCaptcha())
                return RedirectToAction("Board", new { boardId = boardId });
            name = name == "" ? "Anonymous" : name;
            var thread = await threads.AddThread(new ThreadModel { BoardId = boardId });
            await posts.AddPost(new PostModel
            {
                ThreadId = thread.Id,
                Text = ValidateHtml(text),
                Username = name,
                Topic = subject,
                Timestamp = DateTime.Now,
                UserId = User.Identity.GetUserId()
            });
            return RedirectToAction("Board", new { boardId = boardId });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddPost(int threadId, string name, string text)
        {
            if (!CheckCaptcha())
                return RedirectToAction("Thread", new { threadId = threadId });
            name = name == "" ? "Anonymous" : name;
            await posts.AddPost(new PostModel
            {
                ThreadId = threadId,
                Text = ValidateHtml(text),
                Username = name,
                Timestamp = DateTime.Now,
                UserId = User.Identity.GetUserId()
            });
            return RedirectToAction("Thread", new { threadId = threadId });
        }

        [HttpPost]
        public ActionResult GetNewPosts(int threadId, int currentCount)
        {
            var newPosts = posts.GetPosts(threadId).ToList();
            if (newPosts.Count == currentCount)
                return Json(new
                {
                    Posts = new PostModel[0],
                    UserId = User.Identity.GetUserId(),
                });
            return Json(new
            {
                Posts = newPosts.Skip(currentCount).Select(p => new
                {
                    UserId = p.UserId,
                    Id = p.Id,
                    Timestamp = p.Timestamp.ToString(CultureInfo.InvariantCulture),
                    Topic = p.Topic,
                    Text = p.Text,
                    ThreadId = p.ThreadId
                }),
                UserId = User.Identity.GetUserId(),
            });
        }

        public async Task UpdateVisitStatistics(string userId)
        {
            if (Session["counterUpdated"] is bool && (bool)Session["counterUpdated"] != true)
            {
                await visits.AddOrUpdateStatistics("Forum");
                Session["counterUpdated"] = true;
                return;
            }
            if (userId != null)
            {
                await visits.AddOrUpdateVisit(
                    new VisitModel
                    {
                        UserId = userId,
                        LastVisit = DateTime.Now
                    });
            }
        }

        public string ValidateHtml(string raw)
        {
            var withNewLines = HttpUtility.HtmlEncode(raw.Replace(Environment.NewLine, "<br />"));
            var unescapedNewLines = withNewLines.Replace("&lt;br /&gt;", "<br />");
            var unescapedItalic = unescapedNewLines.Replace("&lt;em&gt;", "<em>").Replace("&lt;/em&gt;", "</em>");
            var unescapedBold = unescapedItalic.Replace("&lt;b&gt;", "<b>").Replace("&lt;/b&gt;", "</b>");
            // <strike>
            // <u>
            return unescapedBold;
        }

        public bool CheckCaptcha()
        {
            var response = Request["g-recaptcha-response"];
            const string secret = "6LecXTsUAAAAABJD0BSVjJyFJ0YCXSGhwzwrukRO";
            var client = new WebClient();
            var reply = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}");

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            return captchaResponse.Success;
        }
    }
}
