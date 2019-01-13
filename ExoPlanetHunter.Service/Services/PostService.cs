using ExoPlanetHunter.Database;
using ExoPlanetHunter.Database.Entity;
using ExoPlanetHunter.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace ExoPlanetHunter.Service.Services
{
    public class PostService : IPostService
    {
        private readonly PostContext _context;

        public PostService(PostContext context)
        {
            _context = context;
        }

        public IOrderedQueryable<Post> GetPosts()
        {
            return _context.Posts.Include(p=>p.Tags).OrderByDescending(p=>p.Created);
        }

        public async Task<List<Post>> GetRelatedContent(string tag)
        {
            return await _context.Posts.Include(b => b.Tags).Where(p=>p.Tags.Any(c=>c.Name==tag)).ToListAsync();
        }
        public async Task<List<string>> GetTags()
        {
            return await _context.Posts.Include(b => b.Tags).SelectMany(p=>p.Tags).GroupBy(c=>c.Name).Select(p=> p.Key).ToListAsync();
        }

        public async Task<Post> GetPostAsync(int? id)
        {
            return await _context.Posts.Include(b => b.Tags)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task EditPostAsync(Post post)
        {
           var p = await GetPostAsync(post.Id);

            p.LastModified = DateTime.Now;
            post.Created = DateTime.Now;

            p.Tags= post.Tags;

            _context.Update(p);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePostAsync(Post post)
        {
            post.Created = DateTime.Now;
            post.LastModified = DateTime.Now;
            _context.Add(post);

            await _context.SaveChangesAsync();
        }

        public async Task DeletePostAsync(Post post)
        {
          
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }



    }
}