using ExoPlanetHunter.Database.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExoPlanetHunter.Database
{
    public class Db
    {
        public static IConfiguration Configuration { get; set; }
        public static void Startup(IServiceCollection services, IConfiguration configuration)
        {
            Configuration = configuration;
            services.AddDbContext<ExoContext>();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ExoContext>().AddDefaultTokenProviders();

        }
    }
}
