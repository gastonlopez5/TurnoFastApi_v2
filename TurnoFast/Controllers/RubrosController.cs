using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnoFast.Models;

namespace TurnoFast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RubrosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public RubrosController(DataContext context, IConfiguration config, IHostingEnvironment environment)
        {
            _context = context;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Rubros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rubro>>> GetRubros()
        {
            return await _context.Rubros.Include(x => x.Especialidades).ToListAsync();
        }

        // GET: api/Rubros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rubro>> GetRubro(int id)
        {
            var rubro = await _context.Rubros.FindAsync(id);

            if (rubro == null)
            {
                return NotFound();
            }

            return rubro;
        }

        // PUT: api/Rubros/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRubro(int id, Rubro rubro)
        {
            if (id != rubro.Id)
            {
                return BadRequest();
            }

            _context.Entry(rubro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RubroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rubros
        [HttpPost]
        public async Task<ActionResult<Rubro>> PostRubro(Rubro rubro)
        {
            _context.Rubros.Add(rubro);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRubro", new { id = rubro.Id }, rubro);
        }

        // DELETE: api/Rubros/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rubro>> DeleteRubro(int id)
        {
            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro == null)
            {
                return NotFound();
            }

            _context.Rubros.Remove(rubro);
            await _context.SaveChangesAsync();

            return rubro;
        }

        private bool RubroExists(int id)
        {
            return _context.Rubros.Any(e => e.Id == id);
        }
    }
}
