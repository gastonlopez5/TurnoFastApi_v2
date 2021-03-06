﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnos(int prestacionid, int nrodia, String fecha)
        {
            try
            {
                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;
                bool bandera = true;
                DateTime hora = new DateTime();

                var turnos = _context.Turnos
                    .Include(x => x.Horario)
                    .Where(x => x.Horario.PrestacionId == prestacionid && x.Horario.DiaSemana == nrodia);

                if (turnos.Count() != 0)
                {
                    foreach (Turno turno in turnos)
                    {
                        hora = turno.Horario.HoraDesdeManiana;
                        if (hora.Date != DateTime.MaxValue.Date)
                        {
                            while (hora <= turno.Horario.HoraHastaManiana)
                            {
                                turno2 = new Turno2();
                                if (turno.Fecha == fecha && turno.Hora.TimeOfDay == hora.TimeOfDay)
                                {
                                    bandera = false;
                                }

                                if (bandera)
                                {
                                    turno2.HorarioId = turno.Horario.Id;
                                    turno2.Fecha = fecha;
                                    turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                                    listaTurnos.Add(turno2);
                                }
                                else
                                {
                                    bandera = true;
                                }
                                hora = hora.AddMinutes(turno.Horario.Frecuencia);
                            }
                        }

                        hora = turno.Horario.HoraDesdeTarde;
                        bandera = true;
                        if (hora.Date != DateTime.MaxValue.Date)
                        {
                            while (hora.TimeOfDay <= turno.Horario.HoraHastaTarde.TimeOfDay)
                            {
                                turno2 = new Turno2();
                                if (turno.Fecha == fecha && turno.Hora.TimeOfDay == hora.TimeOfDay)
                                {
                                    bandera = false;
                                }
                                if (bandera)
                                {
                                    turno2.HorarioId = turno.Horario.Id;
                                    turno2.Fecha = fecha;
                                    turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                                    listaTurnos.Add(turno2);
                                }
                                else
                                {
                                    bandera = true;
                                }
                                hora = hora.AddMinutes(turno.Horario.Frecuencia);
                            }
                        }
                    }
                    return Ok(listaTurnos);
                }
                else
                {
                    var horario = await _context.Horarios
                        .FirstOrDefaultAsync(x => x.PrestacionId == prestacionid && x.DiaSemana == nrodia);

                    hora = horario.HoraDesdeManiana;
                    if (hora.Date != DateTime.MaxValue.Date)
                    {
                        while (hora <= horario.HoraHastaManiana)
                        {
                            turno2 = new Turno2();
                            
                            turno2.HorarioId = horario.Id;
                            turno2.Fecha = fecha;
                            turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                            listaTurnos.Add(turno2);
                            
                            hora = hora.AddMinutes(horario.Frecuencia);
                        }
                    }

                    hora = horario.HoraDesdeTarde;
                    if (hora.Date != DateTime.MaxValue.Date)
                    {
                        while (hora.TimeOfDay <= horario.HoraHastaTarde.TimeOfDay)
                        {
                            turno2 = new Turno2();
                            
                            turno2.HorarioId = horario.Id;
                            turno2.Fecha = fecha;
                            turno2.Hora = new Time(hora.TimeOfDay.Hours, hora.TimeOfDay.Minutes, 0, 0);

                            listaTurnos.Add(turno2);
                            
                            hora = hora.AddMinutes(horario.Frecuencia);
                        }
                    }

                    return Ok(listaTurnos);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("pormes/{mes}/{anio}")]
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnosPorMes(String mes, String anio)
        {
            try
            {
                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                var turnos = _context.Turnos.Where(x => x.UsuarioId == usuario.Id);

                if (turnos != null)
                {
                    foreach (Turno turno in turnos)
                    {
                        String[] separados = turno.Fecha.Split("-");
                        String year = separados[0];
                        String month = separados[1];

                        if (month == mes && year == anio)
                        {
                            turno2 = new Turno2();
                            turno2.HorarioId = turno.HorarioId;
                            turno2.Fecha = turno.Fecha;
                            turno2.Hora = new Time(turno.Hora.Hour, turno.Hora.Minute, 0, 0);
                            turno2.UsuarioId = usuario.Id;

                            listaTurnos.Add(turno2);
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

        [HttpGet("solicitadospormes/{mes}/{anio}")]
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnosSolicitadosPorMes(String mes, String anio)
        {
            try
            {
                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                var turnos = _context.Turnos.Where(x => x.Horario.Prestacion.ProfesionalId == usuario.Id);

                if (turnos != null)
                {
                    foreach (Turno turno in turnos)
                    {
                        String[] separados = turno.Fecha.Split("-");
                        String year = separados[0];
                        String month = separados[1];

                        if (month == mes && year == anio)
                        {
                            turno2 = new Turno2();
                            turno2.HorarioId = turno.HorarioId;
                            turno2.Fecha = turno.Fecha;
                            turno2.Hora = new Time(turno.Hora.Hour, turno.Hora.Minute, 0, 0);
                            turno2.UsuarioId = usuario.Id;

                            listaTurnos.Add(turno2);
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

        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnosSolicitadosPorProfesional()
        {
            try
            {
                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                var turnos = _context.Turnos
                    .Include(x => x.Horario)
                    .ThenInclude(y => y.Prestacion)
                    .Where(x => x.Horario.Prestacion.ProfesionalId == usuario.Id);

                if (turnos != null)
                {
                    foreach (Turno turno in turnos)
                    {
                        turno2 = new Turno2();
                        turno2.Id = turno.Id;
                        turno2.Fecha = turno.Fecha;
                        Horario2 horario2 = new Horario2
                        {
                            Prestacion = turno.Horario.Prestacion
                        };
                        turno2.Horario2 = horario2;
                        turno2.Hora = new Time(turno.Hora.Hour, turno.Hora.Minute, 0, 0);
                        turno2.UsuarioId = usuario.Id;

                        listaTurnos.Add(turno2);
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

        [HttpGet("pordia/{fecha}")]
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnosPorDia(String fecha)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                var turnos = _context.Turnos
                    .Include(x => x.Horario)
                    .ThenInclude(y => y.Prestacion)
                    .Where(x => x.UsuarioId == usuario.Id);

                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;

                if (turnos != null)
                {
                    foreach (Turno turno in turnos)
                    {
                        if (turno.Fecha == fecha)
                        {
                            turno2 = new Turno2();
                            turno2.Id = turno.Id;
                            turno2.Fecha = turno.Fecha;
                            Horario2 horario2 = new Horario2
                            {
                                Prestacion = turno.Horario.Prestacion
                            };
                            turno2.Horario2 = horario2;
                            turno2.Hora = new Time(turno.Hora.Hour, turno.Hora.Minute, 0, 0);
                            turno2.UsuarioId = usuario.Id;

                            listaTurnos.Add(turno2);
                        }
                    }

                    if (listaTurnos.Count != 0)
                    {
                        return Ok(listaTurnos);
                    }
                    else
                    {
                        return BadRequest();
                    }
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

        [HttpGet("solicitadospordia/{fecha}")]
        public async Task<ActionResult<IEnumerable<Turno2>>> GetTurnosSolicitadosPorDia(String fecha)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
                var turnos = _context.Turnos
                    .Include(x => x.Horario)
                    .ThenInclude(y => y.Prestacion)
                    .Include(x => x.Usuario)
                    .Where(x => x.Horario.Prestacion.ProfesionalId == usuario.Id);

                List<Turno2> listaTurnos = new List<Turno2>();
                Turno2 turno2 = null;

                if (turnos.Count() != 0)
                {
                    foreach (Turno turno in turnos)
                    {
                        if (turno.Fecha == fecha)
                        {
                            turno2 = new Turno2();
                            turno2.Id = turno.Id;
                            turno2.Fecha = turno.Fecha;
                            Horario2 horario2 = new Horario2
                            {
                                Prestacion = turno.Horario.Prestacion
                            };
                            turno2.Horario2 = horario2;
                            turno2.Hora = new Time(turno.Hora.Hour, turno.Hora.Minute, 0, 0);
                            turno2.Usuario = turno.Usuario;

                            listaTurnos.Add(turno2);
                        }
                    }

                    if (listaTurnos.Count() != 0)
                    {
                        return Ok(listaTurnos);
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                    
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
            try
            {
                var turno = await _context.Turnos.FindAsync(id);
                Msj mensaje = new Msj();

                if (turno == null)
                {
                    return BadRequest();
                }

                _context.Turnos.Remove(turno);
                await _context.SaveChangesAsync();

                mensaje.Mensaje = "Turno cancelado!";

                return Ok(mensaje);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }
    }
}
