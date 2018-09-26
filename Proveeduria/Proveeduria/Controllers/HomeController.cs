using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Models.Enumadores;
using Proveduria.Repositories;
using Newtonsoft.Json.Linq;
using NLog;

namespace Proveduria.Controllers
{
    public class HomeController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            //UsuarioController usuarioController = new UsuarioController();
            //ViewBag.usuario = UsuarioController.usuario.usuario;
            //ViewBag.nombre = UsuarioController.usuario.nombre;
            //VW_EMPLEADO empleado = unitOfWork.EmpleadoRepository.GetAll().Where(p => p.USUARIO == "MROJAS").FirstOrDefault();
            //if (empleado != null)
            //{
            //    Session["usuario"] = empleado.USUARIO;
            //    Session["nombre"] = empleado.EMPLEADO;
            //    Session["id_departamento"] = empleado.DEPARTAMENTO_ID;
            //    Session["departamento"] = empleado.DIRECCION;
            //    Session["usuario_jefe"] = empleado.USUARIO_JEFE_DEPARTAMENTO;
            //}
            return View();
        }

        public ActionResult About()
        {
            
            ViewBag.Message = "YourCierra celso application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            //Aqui Celso
            return View();
        }
    }
}