using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFastApi.Models
{
    public class Turno2
    {
        [Key]
        public int Id { get; set; }

        public String Fecha { get; set; }

        public Time Hora { get; set; }

        public int HorarioId { get; set; }

        public int UsuarioId { get; set; }
    }
}
