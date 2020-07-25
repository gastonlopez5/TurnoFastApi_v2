using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TurnoFast.Models;

namespace TurnoFastApi.Models
{
    public class CambiosListaPrestaciones
    {
        public int Cantidad { get; set; }

        public Prestacion Prestacion { get; set; }
    }
}
