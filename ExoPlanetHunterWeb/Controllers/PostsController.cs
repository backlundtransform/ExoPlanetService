using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.Entity;
using ExoPlanetHunter.Service.Interfaces;
using ExoPlanetHunter.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net.Http;
using ExoPlanetHunter.PHL.Schedules;
using System.Text.RegularExpressions;

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
            _planetService =planetService;
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index(int? page=1)
        {
            int pageSize = 5;
            var posts = await _postService.GetPostsAsync();
            ViewData["stat"] = _statisticsService.GetStatistics();
            ViewData["Img"] = "https://i.imgur.com/FFsupGS.png";
            ViewData["Title"] = "ExoplanetHunter";
            return View(PaginatedList<Post>.CreateAsync(posts, page ?? 1, pageSize));
           
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
        public async Task<IActionResult> Create([Bind("Title,Content")] Post post)
        {
            if (ModelState.IsValid)
            {
                await _postService.CreatePostAsync(post);
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

            var post = await _postService.GetPostAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Created")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _postService.EditPostAsync(post);
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