using LoginBD.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace LoginBD.Controllers
{
    public class AccesoController : Controller
    {
           
        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario usuario)
        {
            usuario.Clave = Encriptar(usuario.Clave);

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                    SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", sqlConnection);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                    cmd.Parameters.AddWithValue("@Clave", usuario.Clave);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConnection.Open();
                    usuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                
                
            }

            if(usuario.IdUsuario != 0) 
            {
                Session["usuario"] = usuario;
                return RedirectToAction("Pagos", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }
        }

        public JsonResult GetClientesDeudores()
        {
            var clientes = new List<Cliente>();

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();

                var query = "SELECT TOP 5 Clientes.Nombre, Clientes.Direccion, Clientes.Email, Clientes.Telefono, sum(Paquete.Precio) AS Deuda FROM Clientes" +
                    "\r\ninner join paquete ON Paquete.ClienteId =Clientes.ClienteId and Paquete.IdEstadoPaquete = 1" +
                    "\r\nGROUP BY Clientes.Nombre, Clientes.Direccion, Clientes.Email, Clientes.Telefono\r\n" +
                    "ORDER BY Deuda DESC\r\n";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente
                            {
                             //   ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Monto = reader.GetDecimal(reader.GetOrdinal("Deuda"))
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }

            return Json(clientes, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetClientes()
        {
            var clientes = new List<Cliente>();

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();

                var query = "SELECT ClienteId, Nombre, Telefono, Direccion, Email FROM Clientes";
                using (var command = new SqlCommand(query, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente
                            {
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };
                            clientes.Add(cliente);
                        }
                    }
                }
            }

            return Json(clientes, JsonRequestBehavior.AllowGet); 
        }


        public JsonResult GetPaquetes()
        {
            var paquetes = new List<Paquete>();

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();

                // Consulta para obtener paquetes con la descripción del estado
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
            INNER JOIN EstadoPaquete e ON p.IdEstadoPaquete = e.IdEstadoPaquete";

                using (var command = new SqlCommand(queryPaquetes, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var paquete = new Paquete
                            {
                                PaqueteId = reader.GetInt32(reader.GetOrdinal("PaqueteId")),
                                Descripcion = reader.GetString(reader.GetOrdinal("PaqueteDescripcion")),
                                CantidadPasajeros = reader.GetInt32(reader.GetOrdinal("CantidadPasajeros")),
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                DescripcionEstado = reader.GetString(reader.GetOrdinal("EstadoPaqueteDescripcion")) // Agregar la descripción del estado
                            };
                            paquetes.Add(paquete);
                        }
                    }
                }

                // Luego, obtenemos los clientes asociados a los paquetes
                var clienteIds = paquetes.Select(p => p.ClienteId).Distinct().ToList();
                var queryClientes = "SELECT * FROM Clientes WHERE ClienteId IN (" + string.Join(",", clienteIds) + ")";
                var clientes = new Dictionary<int, Cliente>();

                using (var command = new SqlCommand(queryClientes, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cliente = new Cliente
                            {
                                ClienteId = reader.GetInt32(reader.GetOrdinal("ClienteId")),
                                Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                                Email = reader.GetString(reader.GetOrdinal("Email"))
                            };
                            clientes[cliente.ClienteId] = cliente;
                        }
                    }
                }

                // Asignamos los clientes a los paquetes
                foreach (var paquete in paquetes)
                {
                    if (clientes.TryGetValue(paquete.ClienteId, out var cliente))
                    {
                        paquete.Cliente = cliente;
                    }
                }
            }

            return Json(paquetes, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetLugares()
        {
            var destinos = new List<Destino>();

            using (SqlConnection sqlConnection =new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();

                var query = @"
                SELECT TOP 3
                    d.IdDestino,
                    d.Pais,
                    d.Provincia,
                    d.Descripcion,
                    COUNT(e.IdEstadia) AS NumeroDeVisitas
                FROM
                    Destinos d
                    INNER JOIN Hoteles h ON d.IdDestino = h.IdDestino
                    INNER JOIN Estadias e ON h.IdHotel = e.IdHotel
                GROUP BY
                    d.IdDestino,
                    d.Pais,
                    d.Provincia,
                    d.Descripcion
                ORDER BY
                    NumeroDeVisitas DESC;
            ";

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var destino = new Destino
                            {
                                IdDestino = reader.GetInt32(reader.GetOrdinal("IdDestino")),
                                Pais = reader.GetString(reader.GetOrdinal("Pais")),
                                Provincia = reader.GetString(reader.GetOrdinal("Provincia")),
                                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                                NumeroVisitas = reader.GetInt32(reader.GetOrdinal("NumeroDeVisitas"))
                            };
                            destinos.Add(destino);
                        }
                    }
                }
            }

            return Json(destinos, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetPagos()
        {
            var pagos = new List<Pago>();

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();

                var query = "SELECT Idpago, Monto,Fechapago,Paquete.Descripcion, Clientes.Nombre from Pagos" +
                    "\r\ninner join Paquete on Paquete.PaqueteId = Pagos.IdPaquete\r\ninner join Clientes on Clientes.ClienteId = Paquete.ClienteId";

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pago = new Pago
                            {
                                IdPago = reader.GetInt32(reader.GetOrdinal("IdPago")),
                                Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                FechaPago = reader.GetDateTime(reader.GetOrdinal("FechaPago")).ToString("dd/MM/yyyy"),
                                Paquete = reader.GetString(reader.GetOrdinal("Descripcion")),
                                Cliente = reader.GetString(reader.GetOrdinal("Nombre"))
                            };
                            pagos.Add(pago);
                        }
                    }
                }
            }

            return Json(pagos, JsonRequestBehavior.AllowGet);
        }



        public static string Encriptar(string String)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 sha256 = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                if(!String.IsNullOrEmpty(String))
                {
                    byte[] resoult = sha256.ComputeHash(enc.GetBytes(String));
                    foreach (byte b in resoult)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                }            
            }
            return sb.ToString();
        }
    }
}