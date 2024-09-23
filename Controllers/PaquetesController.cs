using LoginBD.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginBD.Controllers
{
    public class PaquetesController : Controller
    {
        public JsonResult GetPaquetes()
        {
            var paquetes = new List<Paquete>();

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

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
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


        public JsonResult GetEstadias(int paqueteId)
        {
            List<Estadia> estadias = new List<Estadia>();

            using (SqlConnection sqlConnection = new SqlConnection(Conexion.Conexion.getConexion()))
            {
                sqlConnection.Open();
                string query = @"
            SELECT 
                Estadias.IdEstadia,
                Estadias.FechaLlegada, 
                Estadias.FechaSalida, 
                Hoteles.Nombre AS Hotel, 
                Destinos.Pais AS Ubicacion
            FROM Estadias
            INNER JOIN Hoteles ON Estadias.IdHotel = Hoteles.IdHotel
            INNER JOIN Destinos ON Hoteles.IdDestino = Destinos.IdDestino
            WHERE Estadias.IdPaquete = @PaqueteId";

                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.Parameters.AddWithValue("@PaqueteId", paqueteId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            estadias.Add(new Estadia
                            {
                                IdEstadia = reader.GetInt32(reader.GetOrdinal("IdEstadia")),
                                FechaLlegada = reader.GetDateTime(reader.GetOrdinal("FechaLlegada")),
                                FechaSalida = reader.GetDateTime(reader.GetOrdinal("FechaSalida")),
                                Ubicacion = reader.GetString(reader.GetOrdinal("Ubicacion")),
                                Hotel = reader.GetString(reader.GetOrdinal("Hotel"))
                            });
                        }
                    }
                }
            }

            return Json(estadias, JsonRequestBehavior.AllowGet);
        }
    }


}
