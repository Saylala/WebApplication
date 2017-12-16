using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
        private readonly PageStatisticsRepo pageStatistics = new PageStatisticsRepo();
        private readonly UsersInfoRepo usersInfo = new UsersInfoRepo();

        public async Task<ActionResult> Index()
        {
            var allBoards = boards.GetBoards();
            var statistics = pageStatistics.GetPageStatistics("Forum");

            var info = GetUserInfo();
            await UpdateVisitStatistics(info);
            return View(Tuple.Create(allBoards, statistics, info));
        }

        public async Task<ActionResult> Board(string boardId)
        {
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
            var thread = threads.GetThread(threadId);
            var threadPosts = posts.GetPosts(thread.Id).ToList();
            thread.Posts = threadPosts;
            thread.BoardName = (await boards.GetBoard(thread.BoardId)).ShortName;
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
                Timestamp = DateTime.UtcNow + TimeSpan.FromHours(5),
                UserId = User.Identity.GetUserId()
            });
            return RedirectToAction("Thread", new { threadId = thread.Id });
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddPost(int threadId, string name, string text, string captchaResponse)
        {
            if (!CheckCaptcha(captchaResponse))
                return RedirectToAction("Thread", new { threadId = threadId });
            name = name == "" ? "Anonymous" : name;
            await posts.AddPost(new PostModel
            {
                ThreadId = threadId,
                Text = ValidateHtml(text),
                Username = name,
                Timestamp = DateTime.UtcNow + TimeSpan.FromHours(5),
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
                    Count = currentCount
                });
            return Json(new
            {
                Posts = newPosts.Select((p, i) => new
                {
                    Id = p.Id,
                    Timestamp = p.Timestamp.ToString("dd-MM-yyyy HH:mm:ss"),
                    Username = p.Username,
                    Index = i + 1,
                    Text = p.Text,
                }).Skip(currentCount),
                Count = currentCount
            });
        }

        public async Task UpdateVisitStatistics(UserInfoModel info)
        {
            var newVisit = Session["counterUpdated"] is bool && (bool) Session["counterUpdated"] != true;
            await pageStatistics.AddOrUpdateStatistics("Forum", newVisit);
            if (newVisit)
                Session["counterUpdated"] = true;
            if (info != null && info.UserId != null)
                await usersInfo.AddOrUpdateInfo(
                    new UserInfoModel
                    {
                        UserId = info.UserId,
                        BrowserInfo = info.BrowserInfo,
                        ResolutionWidth = info.ResolutionWidth,
                        ResolutionHeight = info.ResolutionHeight,
                        LastVisit = DateTime.UtcNow + TimeSpan.FromHours(5)
                    });
        }

        private UserInfoModel GetUserInfo()
        {
            var info = new UserInfoModel
            {
                UserId = Request.Cookies["auth"]?.Value,
                LastVisit = DateTime.UtcNow + TimeSpan.FromHours(5)
            };
            if (Request.Cookies["auth"] != null && Request.Cookies["auth"].Value != null)
            {
                var visit = usersInfo.GetInfo(Request.Cookies["auth"].Value);
                if (visit != null)
                    info.LastVisit = visit.LastVisit;
            }
            info.BrowserInfo = $"{Request.Browser.Browser} {Request.Browser.Version}";
            info.ResolutionWidth = Request.Cookies["width"]?.Value;
            info.ResolutionHeight = Request.Cookies["height"]?.Value;
            return info;
        }

        public string ValidateHtml(string raw)
        {
            var withNewLines = HttpUtility.HtmlEncode(raw.Replace(Environment.NewLine, "<br />"));
            var unescapedNewLines = withNewLines.Replace("&lt;br /&gt;", "<br />");
            var unescapedItalic = UnescapePairedTag(unescapedNewLines, "em");
            var unescapedBold = UnescapePairedTag(unescapedItalic, "b");
            return unescapedBold;
        }

        private string UnescapePairedTag(string raw, string tag)
        {
            var openingTags = Regex.Matches(raw, $"&lt;{tag}&gt;");
            var closingTags = Regex.Matches(raw, $"&lt;/{tag}&gt;");
            var unescaped = raw.Replace($"&lt;{tag}&gt;", $"<{tag}>").Replace($"&lt;/{tag}&gt;", $"</{tag}>");
            if (openingTags.Count > closingTags.Count)
                unescaped += string.Concat(Enumerable.Repeat($"</{tag}>", openingTags.Count - closingTags.Count));
            return unescaped;
        }

        public bool CheckCaptcha(string response = null)
        {
            response = response ?? Request["g-recaptcha-response"];
            const string secret = "6LecXTsUAAAAABJD0BSVjJyFJ0YCXSGhwzwrukRO";
            var client = new WebClient();
            var reply = client.DownloadString($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={response}");

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

            return captchaResponse.Success;
        }
    }
}
