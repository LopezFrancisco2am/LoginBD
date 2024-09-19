using LoginBD.Models;
using LoginBD.Permisos;
using System;
using System.Collections.Generic;
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

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }
    }
}