using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFastApi.Models
{
    public class Horario2
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public Time HoraDesdeManiana { get; set; }
        public Time HoraHAstaManiana { get; set; }
        public Time HoraDesdeTarde { get; set; }
        public Time HoraHastaTarde { get; set; }
        public int Frecuencia { get; set; }
        public int PrestacionId { get; set; }
        public List<String> DiasLaborables { get; set; }
    }
}
