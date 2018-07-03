using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using NLog;

namespace Proveduria.Controllers
{
    public class GrupoController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Grupo
        public ActionResult ListaGrupo()
        {
            List<EPRTA_GRUPO> lgrupo = unitOfWork.GrupoRepository.GetAll().ToList();
            return View(lgrupo);
        }

        
    }
}