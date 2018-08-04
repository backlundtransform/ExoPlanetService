using ExoPlanetHunter.Database.entity;
using ExoPlanetHunter.Database.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExoPlanetHunter.Database
{
    public class ExoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
           // var connectionstring = Db.Configuration.GetSection("ConnectionString")?.Value;
            optionbuilder.UseSqlite("Data Source=exo.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Star>().HasMany(x => x.Planets);
            modelBuilder.Entity<Star>().Property(e => e.Id)
            .ValueGeneratedOnAdd();
            modelBuilder.Entity<Constellation>().Property(e => e.Id)
            .ValueGeneratedOnAdd();
       modelBuilder.Entity<Planet>().Property(e => e.Id)
    .ValueGeneratedOnAdd();
        }

        public DbSet<Star> Stars { get; set; }
        public DbSet<Planet> Planets { get; set; }
        public DbSet<Constellation> Constellations { get; set; }
      
    }
}