using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginBD.Conexion
{
    public class Conexion
    {
        static string conexion = "Data Source=DESKTOP-0E8GHKI;Initial Catalog = pruebasLogin; Integrated Security = True";
        public static string getConexion()
        {
            return conexion;
        }

    }
}