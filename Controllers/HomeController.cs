using LoginBD.Models;
using LoginBD.Permisos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginBD.Controllers
{
    public class HomeController : Controller
    {
        [ValidarSeisonAtribute]

        public ActionResult Pagos()
        {
            return View();
        }

        public ActionResult Clientes()
        {
            return View();
        }

        public ActionResult Paquetes()
        {
            return View();
        }

        public ActionResult Viajes(int? paqueteId)
        {
            if (paqueteId.HasValue)
            {
                Paquete paquete = null; 

                using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
                {
                    sqlConnection.Open();
                    var queryPaquetes = @"
            SELECT 
                p.PaqueteId, 
                p.Descripcion AS PaqueteDescripcion,
                p.CantidadPasajeros,
                p.ClienteId,
                p.Precio,
                p.IdEstadoPaquete,
                e.Descripcion AS EstadoPaqueteDescripcion
            FROM Paquete p
            INNER JOIN EstadoPaquete e ON p.IdEstadoPaquete = e.IdEstadoPaquete 
            WHERE PaqueteId = @paqueteId";

                 
                    using (var command = new SqlCommand(queryPaquetes, sqlConnection))
                    {
                  
                        command.Parameters.AddWithValue("@paqueteId", paqueteId.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())  
                            {
                                paquete = new Paquete
                                {
                                    PaqueteId = reader.GetInt32(reader.GetOrdinal("PaqueteId")),
                                    Descripcion = reader.GetString(reader.GetOrdinal("PaqueteDescripcion")),
                                    CantidadPasajeros = reader.GetInt32(reader.GetOrdinal("CantidadPasajeros")),
                                    ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                    Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                    DescripcionEstado = reader.GetString(reader.GetOrdinal("EstadoPaqueteDescripcion"))
                                };
                            }
                        }
                    }
                }

                if (paquete != null)
                {
                    ViewBag.Paquete = paquete; 
                }
                else
                {
                    ViewBag.Error = "Paquete no encontrado.";
                }
            }
            else
            {
                ViewBag.Error = "No se proporcionó ningún ID de paquete.";
            }

            return View();
        }


        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }
    }
}