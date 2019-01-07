using ExoPlanetHunter.Database.Entity;
using ExoPlanetHunter.PHL.Schedules;
using ExoPlanetHunter.Service.Interfaces;
using ExoPlanetHunter.Web.ViewModel;
using ExoPlanetHunterWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IExoService _planetService;
        private readonly IStatisticsService _statisticsService;

        public PostsController(IPostService postService, IExoService planetService, IStatisticsService statisticsService)
        {
            _postService = postService;
            _planetService = planetService;
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index(int? page = 1, string tag="all")
        {
            int pageSize = 5;
            var posts =  _postService.GetPosts();
            ViewData["stat"] = _statisticsService.GetStatistics();
            ViewData["Img"] = "https://i.imgur.com/FFsupGS.png";
            ViewData["Title"] = "ExoplanetHunter";
            if(tag!="all"){

               posts = posts.Where(p=>p.Tags.Any(o=>o.Name==tag)).OrderBy(i=>i.Created);
            }
            return View(PaginatedList<Post>.CreateAsync(posts, page ?? 1, pageSize));
        }

    [Produces("application/json")]
    [Route("api/GetRelatedContent")]
        public async Task<List<RelatedContent>> GetRelatedContent(string tag)
        {
           var posts = await _postService.GetRelatedContent(tag);
            return posts.Select(post=> new RelatedContent {
              Image= Regex.Match(post.Content, "(([^\"\']*.jpe?g))", RegexOptions.IgnoreCase).Groups[0].Value,
              Description=post.Title,
              Created =post.Created,
              Url =$"/details/{post.Id}/{post.Title}"
            }).ToList();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _postService.GetPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["Img"] = Regex.Match(post.Content, "(([^\"\']*.jpe?g))", RegexOptions.IgnoreCase).Groups[0].Value;

            ViewData["Title"] = post.Title;
            return View(post);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Tags")] PostsTags post)
        {
            if (ModelState.IsValid)
            {
                await _postService.CreatePostAsync(new Post
                {
                    Title = post.Title,
                    Content = post.Content,
                    Tags = post.Tags != null ? post.Tags.Split(",")
                  .Select(x => new Tag { Name = x }).ToList() : null
                });
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _postService.GetPostAsync(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(new PostsTags
            {
                Title = p.Title,
                Content = p.Content,
                Tags =p.Tags!=null? string.Join(",", p.Tags.Select(c => c.Name)):null
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Content,Tags")] PostsTags post)
        {
            if (ModelState.IsValid)
            {
                try
                {
                 
                    await _postService.EditPostAsync(new Post
                    {
                        Id = id,
                        Title = post.Title,
                        Content = post.Content,
                        Tags = post.Tags != null ? post.Tags.Split(",")
                  .Select(x => new Tag { Name = x }).ToList() : null
                    });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _postService.GetPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _postService.GetPostAsync(id);

            await _postService.DeletePostAsync(post); ;
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExecuteJob()
        {
            var job = new MyJob();
            job.Execute();
            _planetService.CacheExoPlanets();
            return Content("job done");
        }
    }
}