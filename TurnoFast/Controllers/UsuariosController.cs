using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TurnoFast.Models;

namespace TurnoFast.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration config;
        private readonly IHostingEnvironment environment;

        public UsuariosController(DataContext context, IConfiguration config, IHostingEnvironment environment)
        {
            _context = context;
            this.config = config;
            this.environment = environment;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var email = User.Identity.Name;
                Usuario usuario = new Usuario();

                var u = _context.Usuarios.SingleOrDefault(x => x.Email == email);

                usuario.Id = u.Id;
                usuario.Nombre = u.Nombre;
                usuario.Apellido = u.Apellido;
                usuario.Email = u.Email;
                usuario.Telefono = u.Telefono;
                usuario.FotoPerfil = u.FotoPerfil;
                usuario.Estado = u.Estado;

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        [HttpPut]
        public async Task<IActionResult> Put(Usuario entidad)
        {
            try
            {
                Usuario usuario = null;

                if (ModelState.IsValid && _context.Usuarios.AsNoTracking().SingleOrDefault(e => e.Email == User.Identity.Name) != null)
                {
                    if (entidad.Clave.Length != 0)
                    {
                        entidad.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                password: entidad.Clave,
                                                salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                                                prf: KeyDerivationPrf.HMACSHA1,
                                                iterationCount: 1000,
                                                numBytesRequested: 256 / 8));
                    }

                    usuario = _context.Usuarios.SingleOrDefault(x => x.Email == User.Identity.Name);
                    usuario.Nombre = entidad.Nombre;
                    usuario.Apellido = entidad.Apellido;
                    usuario.Email = entidad.Email;
                    usuario.Telefono = entidad.Telefono;
                    usuario.Estado = entidad.Estado;
                    if(entidad.Clave.Length != 0)
                    {
                        usuario.Clave = entidad.Clave;
                    }
                    
                    _context.Usuarios.Update(usuario);
                    _context.SaveChanges();

                    if(entidad.FotoPerfil != null)
                    {
                        if(usuario.FotoPerfil != null)
                        {
                            string wwwPath = environment.WebRootPath;
                            string fullPath = wwwPath + usuario.FotoPerfil;
                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                var bytes = Convert.FromBase64String(entidad.FotoPerfil);
                                fileStream.Write(bytes, 0, bytes.Length);
                                fileStream.Flush();
                            }
                        }
                        else
                        {
                            var fileName = "fotoperfil.png";
                            string wwwPath = environment.WebRootPath;
                            string path = wwwPath + "/fotoperfil/" + usuario.Id;
                            string filePath = "/fotoperfil/" + usuario.Id + "/" + fileName;
                            string pathFull = Path.Combine(path, fileName);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            using (var fileStream = new FileStream(pathFull, FileMode.Create))
                            {
                                var bytes = Convert.FromBase64String(entidad.FotoPerfil);
                                fileStream.Write(bytes, 0, bytes.Length);
                                fileStream.Flush();
                                usuario.FotoPerfil = filePath;
                            }

                            _context.Usuarios.Update(usuario);
                            _context.SaveChanges();
                        }
                        
                    }

                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Usuarios
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: usuario.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                usuario.Estado = true;
                usuario.Clave = hashed;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                if(usuario.FotoPerfil != null)
                {
                    var user = _context.Usuarios.FirstOrDefault(x => x.Email == usuario.Email);
                    var fileName = "fotoperfil.png";
                    string wwwPath = environment.WebRootPath;
                    string path = wwwPath + "/fotoperfil/" + user.Id;
                    string filePath = "/fotoperfil/" + user.Id + "/" + fileName;
                    string pathFull = Path.Combine(path, fileName);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    using (var fileStream = new FileStream(pathFull, FileMode.Create))
                    {
                        var bytes = Convert.FromBase64String(usuario.FotoPerfil);
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Flush();
                        user.FotoPerfil = filePath;
                    }

                    _context.Usuarios.Update(user);
                    _context.SaveChanges();
                }

                return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Usuarios
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes("Salt"),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var p = _context.Usuarios.FirstOrDefault(x => x.Email == login.Email);
                if (p == null || p.Clave != hashed)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
