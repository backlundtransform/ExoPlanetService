using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Database.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExoPlanetHunter.Database
{
    public class PostContext : IdentityDbContext<IdentityUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
           // var connectionstring = Db.Configuration.GetSection("ConnectionString")?.Value;
            optionbuilder.UseSqlite("Data Source=post.db");
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}