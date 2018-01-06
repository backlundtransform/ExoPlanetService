using ExoplanetService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExoplanetService.Controllers
{
    [Produces("application/json")]
    [Route("api/Stars")]
    public class StarsController : Controller
    {
        private ExoContext context = new ExoContext() { };

        [HttpGet]
        public async Task<IEnumerable<Star>> Get()
        {
            return await Task.Run(() => context.Stars.Where(p => true));
        }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            
           var star = await context.Stars.SingleOrDefaultAsync(c => c.Id == id);

           var planets = await Task.Run(() => context.Planets.Where(c => c.Star.Id == id));

            return new { Star = star, Planets = planets.Count() };
     
        }

        [HttpGet("{id}/planets")]
        public async Task<object> GetPlanets(int id)
        {

            var planets = await Task.Run(() => context.Planets.Where(c => c.Star.Id == id));
            return new { Star = id, Planets = planets};

        }
    }
}