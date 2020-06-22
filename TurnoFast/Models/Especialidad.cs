using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TurnoFast.Models
{
    public class Especialidad
    {
        [Key]
        public int Id { get; set; }

        public String Nombre { get; set; }

        public List<Servicio> Servicios { get; set; }
    }
}
