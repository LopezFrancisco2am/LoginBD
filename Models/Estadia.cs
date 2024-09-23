using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Estadia
    {
        public int IdEstadia { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Ubicacion { get; set; }
        public String Hotel { get; set; }
    }
}