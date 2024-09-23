using LoginBD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginBD.Controllers
{
    public class ClientesController : Controller
    {
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


    }
}
