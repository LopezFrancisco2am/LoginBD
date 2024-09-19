using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

namespace LoginBD.Permisos
{
    public class ValidarSeisonAtribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (HttpContext.Current.Session["Usuario"]==null)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }
        }
    }
}