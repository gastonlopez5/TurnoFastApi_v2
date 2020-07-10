using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TurnoFastApi.Models;

namespace TurnoFast.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }
        
        public String Fecha { get; set; }
        
        public DateTime Hora { get; set; }

        public int HorarioId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("HorarioId")]
        public Horario Horario { get; set; }
    }
}
