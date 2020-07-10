using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TurnoFast.Models;
using TurnoFastApi.Models;

namespace TurnoFastApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TurnosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public TurnosController(DataContext context, IConfiguration config, IHostingEnvironment environment)
        {
            _context = context;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Turnos
        [HttpGet("{prestacionid}/{nrodia}/{fecha}")]
        public async Task<ActionResult<IEnumerable<Turno>>> GetTurnos(int prestacionid, int nrodia, String fecha)
        {
            try
            {
                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;
                bool bandera = true;
                DateTime hora = new DateTime();

                var horarioPrestacion = await _context.Horarios.Include(x => x.Turnos).FirstOrDefaultAsync(x => x.PrestacionId == prestacionid && x.DiaSemana == nrodia);
                if(horarioPrestacion != null)
                {
                    var turnos = horarioPrestacion.Turnos;
                    hora = horarioPrestacion.HoraDesdeManiana;
                    if (hora.Date != DateTime.MaxValue.Date)
                    {
                        while (hora <= horarioPrestacion.HoraHastaManiana)
                        {
                            turno2 = new Turno2();
                            for (int i = 0; i < turnos.Count; i++)
                            {
                                if (turnos[i].Fecha == fecha && turnos[i].Hora == hora)
                                {
                                    bandera = false;
                                }
                            }
                            if (bandera)
                            {
                                turno2.HorarioId = horarioPrestacion.Id;
                                turno2.Fecha = fecha;
                                turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                                listaTurnos.Add(turno2);
                            }
                            hora.AddMinutes(horarioPrestacion.Frecuencia);
                        }
                    }

                    hora = horarioPrestacion.HoraDesdeTarde;
                    bandera = true;
                    if (hora.Date != DateTime.MaxValue.Date)
                    {
                        while (hora.TimeOfDay <= horarioPrestacion.HoraHastaTarde.TimeOfDay)
                        {
                            turno2 = new Turno2();
                            for (int i = 0; i < turnos.Count; i++)
                            {
                                if (turnos[i].Fecha == fecha && turnos[i].Hora == hora)
                                {
                                    bandera = false;
                                }
                            }
                            if (bandera)
                            {
                                turno2.HorarioId = horarioPrestacion.Id;
                                turno2.Fecha = fecha;
                                turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                                listaTurnos.Add(turno2);
                            }
                            else
                            {
                                bandera = true;
                            }
                            hora = hora.AddMinutes(horarioPrestacion.Frecuencia);
                        }
                    }

                    return Ok(listaTurnos);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Turnos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Turno>> GetTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);

            if (turno == null)
            {
                return NotFound();
            }

            return turno;
        }

        // PUT: api/Turnos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTurno(int id, Turno turno)
        {
            if (id != turno.Id)
            {
                return BadRequest();
            }

            _context.Entry(turno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurnoExists(id))
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

        // POST: api/Turnos
        [HttpPost]
        public async Task<ActionResult<Turno>> PostTurno(Turno2 turno2)
        {
            try
            {
                Turno turno = new Turno();
                Msj msj = new Msj();
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

                turno.Fecha = turno2.Fecha;
                turno.Hora = DateTime.Parse(turno2.Hora.hour + ":" + turno2.Hora.minute);
                turno.HorarioId = turno2.HorarioId;
                turno.UsuarioId = usuario.Id;

                _context.Turnos.Add(turno);
                await _context.SaveChangesAsync();

                msj.Mensaje = "Datos guardados correctamente!";

                return Ok(msj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Turnos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Turno>> DeleteTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
            {
                return NotFound();
            }

            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();

            return turno;
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }
    }
}
