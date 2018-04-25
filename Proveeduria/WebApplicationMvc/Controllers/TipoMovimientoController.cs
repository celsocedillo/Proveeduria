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

        public ActionResult ListaTipoMovimiento221()
        {
            List<EPRTA_TIPO_MOVIMIENTO> ltipmov = unitOfWork.TipoMovimientoRepository.GetAll().ToList();
            return View(ltipmov);
        }
        public ActionResult ListaTipoMovimiento5555221()
        {
            List<EPRTA_TIPO_MOVIMIENTO> ltipmov = unitOfWork.TipoMovimientoRepository.GetAll().ToList();
            return View(ltipmov);
        }

        public void Prueba()
        {
            //
        }
    }
}