using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnoFastApi.Models;

namespace TurnoFast.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Prestacion> Prestaciones { get; set; }
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Horario> Horarios { get; set; }
    }
}
