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
using TurnoFastApi.Models;

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
            try 
            { 
                var prestacion = await _context.Prestaciones.Include(x => x.Categoria).FirstOrDefaultAsync(x => x.Id == id);

                if (prestacion == null)
                {
                    return BadRequest();
                }

                return prestacion;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("disponibles/{categoriaid}")]
        public async Task<ActionResult<IEnumerable<Prestacion>>> GetPrestacionesDisponibles(int categoriaid)
        {
            try
            {
                var prestaciones = await _context.Prestaciones.Include(a => a.Profesional).Where(x => x.CategoriaId == categoriaid && x.Profesional.Email != User.Identity.Name).ToListAsync();

                if (prestaciones.Count == 0)
                {
                    return BadRequest();
                }

                return prestaciones;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // PUT: api/Servicios/5
        [HttpPut]
        public async Task<IActionResult> PutServicio([FromBody] Prestacion entidad)
        {
            try
            {
                Prestacion prestacion = null;
                Msj mensaje = new Msj();

                if (ModelState.IsValid && _context.Prestaciones.AsNoTracking().SingleOrDefault(e => e.Id == entidad.Id) != null)
                {
                    prestacion = _context.Prestaciones.SingleOrDefault(x => x.Id == entidad.Id);
                    prestacion.Direccion = entidad.Direccion;
                    prestacion.Nombre = entidad.Nombre;
                    prestacion.Telefono = entidad.Telefono;
                    prestacion.CategoriaId = entidad.CategoriaId;
                    prestacion.Disponible = entidad.Disponible;

                    _context.Prestaciones.Update(prestacion);
                    _context.SaveChanges();

                    mensaje.Mensaje = "Datos actualizados correctamente!";

                    return Ok(mensaje);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
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
            try
            {
                Msj mensaje = new Msj();

                var prestacion = await _context.Prestaciones.FindAsync(id);

                var horarios = _context.Horarios.Include(x => x.Turnos).Where(x => x.PrestacionId == id);

                if (prestacion == null)
                {
                    mensaje.Mensaje = "No se encontró la prestacion!";
                    return BadRequest(mensaje);
                }

                foreach(Horario h in horarios)
                {
                    if(h.Turnos.Count != 0)
                    {
                        mensaje.Mensaje = "Hay turnos asignados para esta prestacion!";
                        return BadRequest(mensaje);
                    }
                }

                foreach(Horario h in horarios)
                {
                    _context.Horarios.Remove(h);
                }

                mensaje.Mensaje = "Prestacion eliminada correctamente!";

                _context.Prestaciones.Remove(prestacion);
                await _context.SaveChangesAsync();

                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private bool PrestacionExists(int id)
        {
            return _context.Prestaciones.Any(e => e.Id == id);
        }
    }
}
