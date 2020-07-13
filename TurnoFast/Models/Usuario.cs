using System;
using System.ComponentModel.DataAnnotations;

namespace TurnoFast.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public string FotoPerfil { get; set; }

        public string Clave { get; set; }

        public Boolean Estado { get; set; }
    }
}
