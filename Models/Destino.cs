using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Destino
    {
        public int IdDestino { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Descripcion { get; set; }
        public int NumeroVisitas { get; set; }
    }

}