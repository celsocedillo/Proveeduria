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
    public class MedidaController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Medida
        public ActionResult ListaMedida()
        {
            List<EPRTA_MEDIDA> lmedida = unitOfWork.MedidaRepository.GetAll().ToList();
            return View(lmedida);
        }
    }
}