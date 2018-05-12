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
    public class MedidaController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Medida
        public ActionResult ListaMedida()
        {
            List<EPRTA_MEDIDA> lmedida = unitOfWork.MedidaRepository.GetAll().ToList();
            return View(lmedida);
        }


        [HttpPost]
        public JsonResult GetMedida(int pid)
        {
            try
            {
                EPRTA_MEDIDA medida = unitOfWork.MedidaRepository.GetById(pid);
                return Json(new { resultado = "success", data = medida, mensaje = "" });

            }
            catch (Exception ex)
            {
                return Json(new { resultado = "error", data = "", mensaje = " Error al consultar la medida, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
        }

        //[HttpPost]
        //public ActionResult GetListaMedida()
        //{
        //    JArray jArray = new JArray();
        //    JObject total = new JObject();
        //    try
        //    {
        //        var query = from d in unitOfWork.MedidaRepository.GetAll()
        //                    select d;
        //        foreach (EPRTA_MEDIDA item in query)
        //        {
        //            JObject jsonObject = new JObject
        //            {
        //                { "ID_MEDIDA", item.ID_MEDIDA },
        //                { "NOMBRE", item.NOMBRE }
        //            };
        //            jArray.Add(jsonObject);
        //        }
        //        total = new JObject();
        //        total.Add("items", jArray);
        //        total.Add("error", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        total.Add("error", true);
        //        logger.Error(ex, ex.Message);
        //    }
        //    return Content(total.ToString(), "application/json");
        //}
        [HttpPost]
        public ActionResult GetListaMedida()
        {
            JObject retorna = new JObject();
            try
            {
                var query = from d in unitOfWork.MedidaRepository.GetAll()
                            select new { d.ID_MEDIDA, d.NOMBRE };
                retorna = new JObject();
                retorna.Add("data", JsonConvert.SerializeObject(query));
                retorna.Add("error", false);
            }
            catch (Exception ex)
            {
                retorna.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }


        [HttpPost]
        public ActionResult Create(EPRTA_MEDIDA medida)
        {
            JObject jObject = new JObject();
            try
            {
                unitOfWork.MedidaRepository.Insert(medida);
                unitOfWork.Save();
                //unitOfWork.Dispose();
                jObject.Add("error", false);
            }
            catch(Exception ex)
            {
                jObject.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(jObject.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult Grabar(EPRTA_MEDIDA precord)
        {
            JObject retorno = new JObject();
            EPRTA_MEDIDA record;
            try
            {
                if (precord.ID_MEDIDA == 0)
                {
                    unitOfWork.MedidaRepository.Insert(precord);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.MedidaRepository.GetById(precord.ID_MEDIDA);
                    record.NOMBRE = precord.NOMBRE;
                    unitOfWork.MedidaRepository.Update(record);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
            }
            catch(Exception ex)
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