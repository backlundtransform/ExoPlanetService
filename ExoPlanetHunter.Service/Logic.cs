using ExoPlanetHunter.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

using ExoPlanetHunter.Database;
using ExoPlanetHunter.PHL;

namespace ExoPlanetHunter.Service
{
    public class Logic
    {

        public static IConfigurationRoot Configuration { get; set; }

        public static void Startup(IServiceCollection services)
        {

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            
            Configuration = builder.Build();
            services.AddTransient<IPlanetService, PlanetService>();
          
            services.AddTransient<IStarService, StarService>();
            Db.Startup(services, Configuration);
          Phl.Startup(Configuration);
        }
    }
}