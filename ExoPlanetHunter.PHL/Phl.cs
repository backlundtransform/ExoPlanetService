using ExoPlanetHunter.PHL.Integration;
using ExoPlanetHunter.PHL.Schedules;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


namespace ExoPlanetHunter.PHL
{
    public class Phl
    {
        public static IConfiguration Configuration { get; set; }

        public static void Startup(IConfiguration configuration)
        {

           
            Configuration = configuration;
            var week = Convert.ToInt32(Configuration.GetSection("Week").Value);
            if (!JobManager.AllSchedules.Any())
            {
                JobManager.Initialize(new ScheduledJobRegistry(week));

            }
            
        }
    }
}