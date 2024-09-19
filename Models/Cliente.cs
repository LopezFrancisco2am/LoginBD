using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; } 
        public string Nombre { get; set; } 
        public string Telefono { get; set; } 
        public string Direccion { get; set; } 
        public string Email { get; set; } 
        public decimal Monto { get; set; }
    }
}