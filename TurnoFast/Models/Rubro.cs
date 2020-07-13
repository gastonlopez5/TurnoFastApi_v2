using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurnoFast.Models
{
    public class Rubro
    {
        [Key]
        public int Id { get; set; }

        public String Tipo { get; set; }

        public String RutaFoto { get; set; }

        public List<Categoria> Especialidades { get; set; }
    }
}
