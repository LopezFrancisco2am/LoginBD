using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Paquete
    {
        public int PaqueteId { get; set; }
        public string Descripcion { get; set; }
        public int CantidadPasajeros { get; set; }
        public int ClienteId { get; set; }
        public decimal Precio { get; set; }
        public string DescripcionEstado { get; set; }
        public Cliente Cliente { get; set; }

    }
}