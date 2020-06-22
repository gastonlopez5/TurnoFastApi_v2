using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFast.Models
{
    public class HorarioDisponible
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime HoraInicio { get; set; }

        public DateTime HoraFin { get; set; }

        public Boolean TurnoManiana { get; set; }

        public Boolean TurnoTarde { get; set; }

        public int ProfesionalId { get; set; }

        public int ServicioId { get; set; }

        [ForeignKey("ProfesionalId")]
        public Usuario Profesional { get; set; }

        [ForeignKey("ServicioId")]
        public Servicio Servicio { get; set; }

        public List<Turno> Turnos { get; set; }
    }
}
