using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TurnoFastApi.Models;

namespace TurnoFast.Models
{
    public class Prestacion
    {
        [Key]
        public int Id { get; set; }

        public String Telefono { get; set; }

        public String Direccion { get; set; }

        public String Nombre { get; set; }

        public String Logo { get; set; }

        public Boolean Disponible { get; set; }

        public int ProfesionalId { get; set; }

        public int CategoriaId { get; set; }

        [ForeignKey("ProfesionalId")]
        public Usuario Profesional { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        public List<Horario> Horarios { get; set; }

    }
}
