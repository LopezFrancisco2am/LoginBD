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
            ViewBag.Pagos = ClientesController.Instancia.GetPagos();
            return View();
        }

        public ActionResult Clientes()
        {
            ViewBag.Clientes = ClientesController.Instancia.GetClientes();
            ViewBag.Deudores = ClientesController.Instancia.GetClientesDeudores();
            return View();
        }

        public ActionResult Paquetes()
        {
            ViewBag.Paquetes = PaquetesController.Instancia.GetPaquetes();
            ViewBag.Destinos = PaquetesController.Instancia.GetDestinos();
            return View();       
        }

        public ActionResult Viajes()
        {
            Paquete paquete = TempData["Paquete"] as Paquete; 

            if (paquete != null)
            {
                ViewBag.Paquete = paquete;
            }
            else
            {
                ViewBag.Error = "Paquete no encontrado.";
            }
            var estadias = PaquetesController.Instancia.GetEstadias(paquete.PaqueteId);
            if (estadias != null)
            {
                ViewBag.Estadias = estadias;
            }
            else
            {
                ViewBag.Error = "estadia no encontrada.";
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