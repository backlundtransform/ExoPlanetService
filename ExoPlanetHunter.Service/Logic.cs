using ExoPlanetHunter.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ExoPlanetHunter.Service.Services;
using ExoPlanetHunter.Database;
using ExoPlanetHunter.PHL;
using ExoPlanetHunter.Service.Profiles;
using Microsoft.AspNetCore.Hosting;

namespace ExoPlanetHunter.Service
{
    public class Logic
    {

      

        public static IConfigurationRoot Configuration { get; set; }

        public static void Startup(IServiceCollection services, IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            
            Configuration = builder.Build();
            services.AddTransient<IPlanetService, PlanetService>();
            services.AddTransient<IStarService, StarService>();
            services.AddTransient<IConstellationService, ConstellationService>();

            AutoMapper.Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Db.Startup(services, Configuration);

            if (env.IsProduction())
            {
                Phl.Startup(Configuration);
            }
          
        }
    }
}