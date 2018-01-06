
using Microsoft.EntityFrameworkCore;


namespace ExoplanetService.Models
{
    public class ExoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {


            optionbuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=exoplanets;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Star>().HasMany(x => x.Planets);
        }

        public DbSet<Star> Stars{ get; set; }
        public DbSet<Planet> Planets { get; set; }

        public DbSet<Constellation> Constellations { get; set; }
    }
}
