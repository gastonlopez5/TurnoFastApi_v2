using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnoFast.Models;
using TurnoFastApi.Models;

namespace TurnoFastApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HorariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public HorariosController(DataContext context, IConfiguration config, IHostingEnvironment environment)
        {
            _context = context;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Horarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Horario>>> GetHorarios()
        {
            return await _context.Horarios.ToListAsync();
        }

        // GET: api/Horarios/5
        [HttpGet("{id}/{nrodia}")]
        public async Task<ActionResult<Horario2>> GetHorario(int id, int nrodia)
        {
            try
            {
                var horario = await _context.Horarios.Where(x => x.PrestacionId == id && x.DiaSemana == nrodia).FirstOrDefaultAsync();
                Horario2 horario2 = new Horario2();

                if (horario == null)
                {
                    return BadRequest();
                }

                horario2.DiaSemana = horario.DiaSemana;
                horario2.Frecuencia = horario.Frecuencia;
                horario2.Id = horario.Id;

                if (horario.HoraDesdeManiana.Year != DateTime.MaxValue.Year)
                {
                    horario2.HoraDesdeManiana = new Time(horario.HoraDesdeManiana.Hour, horario.HoraDesdeManiana.Minute, 0, 0);
                    horario2.HoraHastaManiana = new Time(horario.HoraHastaManiana.Hour, horario.HoraHastaManiana.Minute, 0, 0);
                }

                if (horario.HoraDesdeTarde.Year != DateTime.MaxValue.Year)
                {
                    horario2.HoraDesdeTarde = new Time(horario.HoraDesdeTarde.Hour, horario.HoraDesdeTarde.Minute, 0, 0);
                    horario2.HoraHastaTarde = new Time(horario.HoraHastaTarde.Hour, horario.HoraHastaTarde.Minute, 0, 0);
                }

                return horario2;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // PUT: api/Horarios/5
        [HttpPut]
        public async Task<IActionResult> PutHorario([FromBody] Horario2 entidad)
        {
            try
            {
                Horario horario = null;
                Msj mensaje = new Msj();

                if (ModelState.IsValid && _context.Horarios.AsNoTracking().SingleOrDefault(e => e.Id == entidad.Id) != null)
                {
                    horario = _context.Horarios.SingleOrDefault(x => x.Id == entidad.Id);
                    horario.Frecuencia = entidad.Frecuencia;
                    horario.DiaSemana = entidad.DiaSemana;

                    if (entidad.HoraDesdeManiana != null)
                    {
                        horario.HoraDesdeManiana = DateTime.Parse(entidad.HoraDesdeManiana.hour + ":" + entidad.HoraDesdeManiana.minute);
                        horario.HoraHastaManiana = DateTime.Parse(entidad.HoraHastaManiana.hour + ":" + entidad.HoraHastaManiana.minute);
                    }
                    else
                    {
                        horario.HoraDesdeManiana = DateTime.MaxValue;
                        horario.HoraHastaManiana = DateTime.MaxValue;
                    }

                    if (entidad.HoraDesdeTarde != null)
                    {
                        horario.HoraDesdeTarde = DateTime.Parse(entidad.HoraDesdeTarde.hour + ":" + entidad.HoraDesdeTarde.minute);
                        horario.HoraHastaTarde = DateTime.Parse(entidad.HoraHastaTarde.hour + ":" + entidad.HoraHastaTarde.minute);
                    }
                    else
                    {
                        horario.HoraDesdeTarde = DateTime.MaxValue;
                        horario.HoraHastaTarde = DateTime.MaxValue;
                    }

                    _context.Horarios.Update(horario);
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

        // POST: api/Horarios
        [HttpPost]
        public async Task<ActionResult<Horario>> PostHorario(Horario2 horario2)
        {
            try
            {
                Horario horario = new Horario();
                Msj msj = new Msj();

                if (horario2.HoraDesdeManiana != null)
                {
                    horario.HoraDesdeManiana = DateTime.Parse(horario2.HoraDesdeManiana.hour + ":" + horario2.HoraDesdeManiana.minute);
                    horario.HoraHastaManiana = DateTime.Parse(horario2.HoraHastaManiana.hour + ":" + horario2.HoraHastaManiana.minute);
                }
                else
                {
                    horario.HoraDesdeManiana = DateTime.MaxValue;
                    horario.HoraHastaManiana = DateTime.MaxValue;
                }

                if (horario2.HoraDesdeTarde != null)
                {
                    horario.HoraDesdeTarde = DateTime.Parse(horario2.HoraDesdeTarde.hour + ":" + horario2.HoraDesdeTarde.minute);
                    horario.HoraHastaTarde = DateTime.Parse(horario2.HoraHastaTarde.hour + ":" + horario2.HoraHastaTarde.minute);
                }
                else
                {
                    horario.HoraDesdeTarde = DateTime.MaxValue;
                    horario.HoraHastaTarde = DateTime.MaxValue;
                }

                horario.Frecuencia = horario2.Frecuencia;
                horario.PrestacionId = horario2.PrestacionId;
                horario.DiaSemana = horario2.DiaSemana;

                _context.Horarios.Add(horario);
                await _context.SaveChangesAsync();

                msj.Mensaje = "Datos guardados correctamente!";

                return Ok(msj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // DELETE: api/Horarios/5
        [HttpDelete("{id}/{nrodia}")]
        public async Task<ActionResult<Horario>> DeleteHorario(int id, int nrodia)
        {
            try
            {
                Msj mensaje = new Msj();
                var horario = await _context.Horarios.Where(x => x.PrestacionId == id && x.DiaSemana == nrodia).FirstOrDefaultAsync(); ;

                if (horario == null)
                {
                    return BadRequest();
                }

                _context.Horarios.Remove(horario);
                await _context.SaveChangesAsync();

                mensaje.Mensaje = "Horario eliminado!";

                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        private bool HorarioExists(int id)
        {
            return _context.Horarios.Any(e => e.Id == id);
        }
    }
}
