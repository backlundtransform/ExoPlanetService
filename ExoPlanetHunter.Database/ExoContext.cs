using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Database.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExoPlanetHunter.Database
{
    public class ExoContext : IdentityDbContext<IdentityUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            var connectionstring = Db.Configuration.GetSection("ConnectionString").Value;
            optionbuilder.UseSqlite(connectionstring);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Star>().HasMany(x => x.Planets);
        }

        public DbSet<Star> Stars { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<Constellation> Constellations { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}