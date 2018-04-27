using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;

namespace Proveduria.Controllers
{
    public class TipoMovimientoController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: TipoMovimiento
        public ActionResult ListaTipoMovimiento()
        {
            List<EPRTA_TIPO_MOVIMIENTO> ltipmov = unitOfWork.TipoMovimientoRepository.GetAll().ToList();
            return View(ltipmov);
        }

        [HttpPost]
        public JsonResult GetTipoMovimiento(int pid)
        {
            try
            {
                EPRTA_TIPO_MOVIMIENTO tipomov = unitOfWork.TipoMovimientoRepository.GetById(pid);
                return Json(new { resultado = "success", data = tipomov, mensaje = "" });

            }
            catch (Exception ex)
            {
                return Json(new { resultado = "error", data = "", mensaje = " Error al consultar el tipo de movimiento, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
        }
   

    }
}