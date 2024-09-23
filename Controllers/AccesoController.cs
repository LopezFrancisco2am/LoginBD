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