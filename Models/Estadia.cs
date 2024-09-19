using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Estadia
    {
        public int IdEstadia { get; set; }
        public int IdPaquete { get; set; }
        public int IdHotel { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }

        public Paquete Paquete { get; set; }
        public Hotel Hotel { get; set; }
    }
}