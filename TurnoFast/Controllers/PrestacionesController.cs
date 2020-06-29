using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TurnoFast.Models;

namespace TurnoFast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PrestacionesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public PrestacionesController(DataContext context, IConfiguration config, IHostingEnvironment environment)
        {
            _context = context;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prestacion>>> GetPrestaciones()
        {
            try
            {
                return await _context.Prestaciones
                    .Include(x => x.Profesional)
                    .Include(a => a.Categoria)
                    .Where(x => x.Profesional.Email == User.Identity.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prestacion>> GetPrestacion(int id)
        {
            var prestacion = await _context.Prestaciones.FindAsync(id);

            if (prestacion == null)
            {
                return NotFound();
            }

            return prestacion;
        }

        // PUT: api/Servicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(int id, Prestacion prestacion)
        {
            if (id != prestacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(prestacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrestacionExists(id))
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

        // POST: api/Servicios
        [HttpPost]
        public async Task<ActionResult<Prestacion>> PostPrestacion(Prestacion prestacion)
        {
            
            try
            {
                prestacion.ProfesionalId = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name).Id;   
                _context.Prestaciones.Add(prestacion);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPrestacion", new { id = prestacion.Id }, prestacion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Prestacion>> DeleteServicio(int id)
        {
            var prestacion = await _context.Prestaciones.FindAsync(id);
            if (prestacion == null)
            {
                return NotFound();
            }

            _context.Prestaciones.Remove(prestacion);
            await _context.SaveChangesAsync();

            return prestacion;
        }

        private bool PrestacionExists(int id)
        {
            return _context.Prestaciones.Any(e => e.Id == id);
        }
    }
}
