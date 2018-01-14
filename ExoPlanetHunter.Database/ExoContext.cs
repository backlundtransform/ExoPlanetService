using ExoPlanetHunter.Pocos;
using Microsoft.EntityFrameworkCore;

namespace ExoPlanetHunter.Database
{
    public class ExoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            var connectionstring = Db.Configuration.GetSection("ConnectionString").Value.ToString();
            optionbuilder.UseSqlServer(connectionstring);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Star>().HasMany(x => x.Planets);
        }

        public DbSet<Star> Stars { get; set; }
        public DbSet<Planet> Planets { get; set; }

        public DbSet<Constellation> Constellations { get; set; }
    }
}