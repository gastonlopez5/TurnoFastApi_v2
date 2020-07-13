using System;
using System.ComponentModel.DataAnnotations;

namespace TurnoFast.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        public String Nombre { get; set; }
    }
}
