using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public string Cliente { get; set; }
        public string Paquete { get; set; }
        public decimal Monto { get; set; }
        public string FechaPago { get; set; } 
    }
}