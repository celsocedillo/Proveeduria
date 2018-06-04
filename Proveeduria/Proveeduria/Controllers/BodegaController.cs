using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;
using NLog;


namespace Proveduria.Controllers
{
    public class BodegaController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Bodega
        public ActionResult ListaBodega()
        {
            List<EPRTA_BODEGA> lbodega = unitOfWork.BodegaRepository.GetAll().ToList();
            return View(lbodega);
        }

        [HttpPost]
        public ActionResult GetBodega(int pid)
        {
            JObject jsonObject = new JObject();
            try
            {

                EPRTA_BODEGA bodega = unitOfWork.BodegaRepository.GetById(pid);
                jsonObject.Add("ID_BODEGA", bodega.ID_BODEGA);
                jsonObject.Add("NOMBRE", bodega.NOMBRE);
                jsonObject.Add("CUENTA_CONTABLE", bodega.CUENTA_CONTABLE);
                jsonObject.Add("ESTADO", bodega.ESTADO);
                jsonObject.Add("resultado", "success");
                jsonObject.Add("mensaje", "success");
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                jsonObject.Add("resultado", "error");
                jsonObject.Add("msg", ex.Message);
            }
            return Content(jsonObject.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetListaBodega()
        {
            JObject retorna = new JObject();
            try
            {
                var query = from d in unitOfWork.BodegaRepository.GetAll()
                            select new { d.ID_BODEGA, d.NOMBRE, d.CUENTA_CONTABLE, d.ESTADO };
                retorna = new JObject();
                retorna.Add("data", JsonConvert.SerializeObject(query));
                retorna.Add("error", false);
            }
            catch (Exception ex)
            {
                retorna.Add("error", true);
                retorna.Add("resultado", "error");
                retorna.Add("msg", ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult Create(EPRTA_BODEGA bodega)
        {
            JObject jObject = new JObject();
            try
            {
                unitOfWork.BodegaRepository.Insert(bodega);
                unitOfWork.Save();
                //unitOfWork.Dispose();
                jObject.Add("error", false);
            }
            catch (Exception ex)
            {
                jObject.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(jObject.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult Grabar(EPRTA_BODEGA precord)
        {
            JObject retorno = new JObject();
            EPRTA_BODEGA record = new EPRTA_BODEGA();
            try
            {
                if (precord.ID_BODEGA == 0)
                {
                    precord.ESTADO = "A";
                    unitOfWork.BodegaRepository.Insert(precord);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.BodegaRepository.GetById(precord.ID_BODEGA);
                    record.NOMBRE = precord.NOMBRE;
                    record.CUENTA_CONTABLE = precord.CUENTA_CONTABLE;
                    record.ESTADO = "A";
                    unitOfWork.BodegaRepository.Update(record);
                    unitOfWork.Save();
                }
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

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }



    }
}