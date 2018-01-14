using FluentScheduler;
using Microsoft.Extensions.Configuration;
using System;

namespace ExoPlanetHunter.PHL
{
    public class Phl
    {
        public static IConfiguration Configuration { get; set; }

        public static void Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var week = Convert.ToInt32(Configuration.GetSection("Week").Value);
            JobManager.Initialize(new ScheduledJobRegistry(week));
        }
    }
}