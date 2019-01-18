using ExoPlanetHunter.Database.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoPlanetHunter.Service.Interfaces
{
    public interface IPostService
    {
        IOrderedQueryable<Post> GetPosts();
        Task<List<string>> GetTags();

        Task<Post> GetPostAsync(int? id);

        Task<List<Post>> GetRelatedContent(string tag);

        Task EditPostAsync(Post post);

        Task CreatePostAsync(Post post);

        Task DeletePostAsync(Post post);
    }
}