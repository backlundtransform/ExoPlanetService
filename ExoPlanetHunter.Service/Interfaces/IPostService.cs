using ExoPlanetHunter.Database.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostAsync(int? id);

        Task EditPostAsync(Post post);

        Task CreatePostAsync(Post post);

        Task DeletePostAsync(Post post);
    }
}