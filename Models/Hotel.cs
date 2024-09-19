using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Hotel
    {
        public int IdHotel { get; set; }
        public int IdDestino { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }

        public Destino Destino { get; set; }
    }

}