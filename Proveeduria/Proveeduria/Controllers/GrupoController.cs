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
    public class GrupoController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        // GET: Grupo
        public ActionResult ListaGrupo()
        {
            List<EPRTA_GRUPO> lgrupo = unitOfWork.GrupoRepository.GetAll().ToList();
            return View(lgrupo);
        }
    }
}