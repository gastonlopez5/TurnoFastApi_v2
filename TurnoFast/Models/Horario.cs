﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TurnoFast.Models;

namespace TurnoFastApi.Models
{
    public class Horario
    {
        public int Id { get; set; }
        public int DiaSemana { get; set; }
        public DateTime HoraDesdeManiana { get; set; }
        public DateTime HoraHastaManiana { get; set; }
        public DateTime HoraDesdeTarde { get; set; }
        public DateTime HoraHastaTarde { get; set; }
        public int Frecuencia { get; set; }
        public int PrestacionId { get; set; }
        [ForeignKey("PrestacionId")]
        public Prestacion Prestacion { get; set; }
        public List<Turno> Turnos { get; set; }
        [NotMapped]
        public List<String> DiasLaborables { get; set; }
    }
}
