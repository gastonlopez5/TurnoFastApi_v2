using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFast.Models
{
    public class Turno
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        public DateTime Hora { get; set; }

        public String Observaciones { get; set; }

        public String ArchivoAdjunto { get; set; }

        public int HorarioDisponibleId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("HorarioDisponibleId")]
        public Prestacion HorarioDisponible { get; set; }
    }
}
