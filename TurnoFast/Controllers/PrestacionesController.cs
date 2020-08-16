using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<Prestacion>>> GetAllPrestaciones()
        {
            try
            {
                return await _context.Prestaciones
                    .Include(x => x.Profesional)
                    .Include(a => a.Categoria)
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
                var prestaciones = await _context.Prestaciones.Include(a => a.Profesional)
                    .Where(x => x.CategoriaId == categoriaid && x.Profesional.Email != User.Identity.Name && x.Disponible == true)
                    .ToListAsync();

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

                    if(entidad.Logo != null)
                    {
                        if(prestacion.Logo != null) 
                        {
                            string wwwPath = environment.WebRootPath;
                            string fullPath = wwwPath + prestacion.Logo;
                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                var bytes = Convert.FromBase64String(entidad.Logo);
                                fileStream.Write(bytes, 0, bytes.Length);
                                fileStream.Flush();
                            }
                        }
                        else
                        {
                            var fileName = "logo.png";
                            string wwwPath = environment.WebRootPath;
                            string path = wwwPath + "/logo/" + prestacion.Id;
                            string filePath = "/logo/" + prestacion.Id + "/" + fileName;
                            string pathFull = Path.Combine(path, fileName);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            using (var fileStream = new FileStream(pathFull, FileMode.Create))
                            {
                                var bytes = Convert.FromBase64String(entidad.Logo);
                                fileStream.Write(bytes, 0, bytes.Length);
                                fileStream.Flush();
                                prestacion.Logo = filePath;
                            }

                            _context.Prestaciones.Update(prestacion);
                            _context.SaveChanges();
                        }
                    }

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
                string logo = null;
                prestacion.ProfesionalId = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name).Id;
                if(prestacion.Logo != null)
                {
                    logo = prestacion.Logo;
                    prestacion.Logo = "a";
                }
                _context.Prestaciones.Add(prestacion);
                await _context.SaveChangesAsync();

                if (prestacion.Logo != null)
                {
                    var prestation = _context.Prestaciones.Last();
                    var fileName = "logo.png";
                    string wwwPath = environment.WebRootPath;
                    string path = wwwPath + "/logo/" + prestation.Id;
                    string filePath = "/logo/" + prestation.Id + "/" + fileName;
                    string pathFull = Path.Combine(path, fileName);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (var fileStream = new FileStream(pathFull, FileMode.Create))
                    {
                        var bytes = Convert.FromBase64String(logo);
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Flush();
                        prestation.Logo = filePath;
                    }

                    _context.Prestaciones.Update(prestation);
                    _context.SaveChanges();
                }

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
                    return Ok(mensaje);
                }

                foreach (Horario h in horarios)
                {
                    if (h.Turnos.Count != 0 && h.Turnos.Any(x => DateTime.Parse(x.Fecha) >= DateTime.Now.Date))
                    {
                        mensaje.Mensaje = "Hay turnos asignados para esta prestacion!";
                        return Ok(mensaje);
                    }
                }

                foreach (Horario h in horarios)
                {
                    _context.Horarios.Remove(h);
                }

                string wwwPath = environment.WebRootPath;
                string fullPath = wwwPath + prestacion.Logo;
                System.IO.File.Delete(fullPath);

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
