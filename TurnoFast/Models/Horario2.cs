using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TurnoFast.Models;

namespace TurnoFastApi.Models
{
    public class Horario2
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public Time HoraDesdeManiana { get; set; }
        public Time HoraHastaManiana { get; set; }
        public Time HoraDesdeTarde { get; set; }
        public Time HoraHastaTarde { get; set; }
        public int Frecuencia { get; set; }
        public int PrestacionId { get; set; }
        public List<Turno2> Turnos { get; set; }
        [ForeignKey("PrestacionId")]
        public Prestacion Prestacion { get; set; }
    }
}
