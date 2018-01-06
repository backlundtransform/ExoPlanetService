
using Microsoft.AspNetCore.Mvc;
using FluentScheduler;
using ExoplanetService.Managers;

namespace ExoplanetService.Controllers
{
   
    [Route("api/Cache")]
    public class CacheController : Controller
    {

       // PUT api/Cache/5
        [HttpPut("{week}")]
        public string Get(int week)
        {
            JobManager.Initialize(new ScheduledJobRegistry(week));
            return $"Jobb started and will run once every {week} week";
        }
       
    }
}