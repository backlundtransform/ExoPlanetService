using AutoMapper;
using ExoPlanetHunter.Database;
using ExoPlanetHunter.PHL;
using ExoPlanetHunter.Service.Interfaces;
using ExoPlanetHunter.Service.Profiles;
using ExoPlanetHunter.Service.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

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
            services.AddTransient<IPlanetService, PlanetService>().AddTransient<IStarService, StarService>()
                .AddTransient<IConstellationService, ConstellationService>().AddTransient<IPostService, PostService>()
                 .AddTransient<IExoService,ExoService>().AddTransient<IStatisticsService, StatisticsService>()
                  .AddTransient<IChartService, ChartService>()
                    .AddTransient<IMastService, MastService>();
   
            Mapper.Reset();
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Db.Startup(services, Configuration);


         
        }
    }
}