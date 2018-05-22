using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using Newtonsoft.Json.Linq;
using NLog;


namespace Proveduria.Controllers
{
    public class UsuarioController : Controller
    {
        public static Usuario usuario;
        public UsuarioController()
        {
            usuario = new Usuario
            {
                usuario = "CCEDILLO",
                nombre = "Celso Cedillo F."
            };
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
    }
}