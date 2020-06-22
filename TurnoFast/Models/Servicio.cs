using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFast.Models
{
    public class Servicio
    {
        [Key]
        public int Id { get; set; }

        public String Email { get; set; }

        public String Telefono { get; set; }

        public String Direccion { get; set; }

        public String Nombre { get; set; }

        public String Logo { get; set; }

        public Boolean Disponible { get; set; }

        public int EspecialidadId { get; set; }

        [ForeignKey("EspecialidadId")]
        public Especialidad Especialidad { get; set; }

        public List<HorarioDisponible> HorariosDisponibles { get; set; }
    }
}
