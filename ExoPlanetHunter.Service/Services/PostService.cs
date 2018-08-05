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

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _context.Posts.OrderByDescending(p=>p.Created).ToListAsync();
        }

        public async Task<Post> GetPostAsync(int? id)
        {
            return await _context.Posts
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task EditPostAsync(Post post)
        {
            post.LastModified = DateTime.Now;
            _context.Update(post);
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