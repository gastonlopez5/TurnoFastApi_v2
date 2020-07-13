using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TurnoFast.Models;

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

        [ForeignKey("HorarioId")]
        public Horario2 Horario2 { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
