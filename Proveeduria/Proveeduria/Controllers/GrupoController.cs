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
using Newtonsoft.Json.Linq;
using Proveduria.Utils;

namespace Proveduria.Controllers
{
    [SessionTimeout]
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

        [HttpPost]
        public ActionResult GetListaGrupo()
        {
            JObject retorno = new JObject();
            try
            {
                var query = from d in unitOfWork.GrupoRepository.GetAll()
                            select new { d.ID_GRUPO, d.CODIGO, d.NOMBRE, d.CUENTA_CONTABLE, d.ESTADO};
                retorno.Add("resultado", "success");
                retorno.Add("data", JArray.FromObject(query));
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "success");
                retorno.Add("msg", ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetGrupo(int pid)
        {
            JObject retorno = new JObject();
            try
            {
                EPRTA_GRUPO grupo = unitOfWork.GrupoRepository.GetById(pid);
                var tmp = new { grupo.ID_GRUPO, grupo.NOMBRE, grupo.CODIGO, grupo.CUENTA_CONTABLE, grupo.ESTADO};
                retorno.Add("resultado", "success");
                retorno.Add("data", JObject.FromObject(tmp));

            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GrabarGrupo(EPRTA_GRUPO precord)
        {
            JObject retorno = new JObject();
            EPRTA_GRUPO record;
            try
            {
                if (precord.ID_GRUPO == 0)
                {
                    record = new EPRTA_GRUPO();
                    record.NOMBRE = precord.NOMBRE;
                    record.CODIGO = precord.CODIGO;
                    record.CUENTA_CONTABLE = precord.CUENTA_CONTABLE;
                    record.ESTADO = "A";
                    unitOfWork.GrupoRepository.Insert(record);
                }
                else
                {
                    record = unitOfWork.GrupoRepository.GetById(precord.ID_GRUPO);
                    record.NOMBRE = precord.NOMBRE;
                    record.CUENTA_CONTABLE = precord.CUENTA_CONTABLE;
                    unitOfWork.GrupoRepository.Update(record);
                }
                unitOfWork.Save();
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
                logger.Info("Dato Grabado");
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("data", null);
                retorno.Add("mensaje", ex.ToString());
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }



    }
}