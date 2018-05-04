﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;
using NLog;
using Newtonsoft.Json;

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

        [HttpPost]
        public ActionResult GetListaMedida()
        {
            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                //var query = from d in unitOfWork.MedidaRepository.GetAll()
                //            select d;
                //foreach (EPRTA_MEDIDA item in query)
                //{
                //    JObject jsonObject = new JObject
                //    {
                //        { "ID_MEDIDA", item.ID_MEDIDA },
                //        { "NOMBRE", item.NOMBRE }
                //    };
                //    jArray.Add(jsonObject);
                //}
                //total = new JObject();
                //total.Add("items", jArray);
                //total.Add("error", false);

                var query = from d in unitOfWork.MedidaRepository.GetAll()
                            select new { d.ID_MEDIDA, d.NOMBRE };
                total = new JObject();
                total.Add("data", JsonConvert.SerializeObject(query));
                total.Add("error", false);

            }
            catch (Exception ex)
            {
                total.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");
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



    }
}