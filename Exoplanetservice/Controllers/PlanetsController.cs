using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExoplanetService.Models;
using Microsoft.AspNet.OData.Query;

namespace ExoplanetService.Controllers
{
    [Produces("application/json")]
    [Route("api/Planets")]
    public class PlanetsController : Controller
    {
        private readonly ExoContext _context;

        public PlanetsController(ExoContext context)
        {
            _context = context;
        }

 

        // GET: api/Planets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanet([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var planet = await _context.Planets.SingleOrDefaultAsync(m => m.Id == id);

            if (planet == null)
            {
                return NotFound();
            }

            return Ok(planet);
        }



        public IQueryable<Planet> GetPlanets(ODataQueryOptions opts)
        {
            IQueryable results = opts.ApplyTo(_context.Planets.AsQueryable());
            return results as IQueryable<Planet>;
        }


    }
}