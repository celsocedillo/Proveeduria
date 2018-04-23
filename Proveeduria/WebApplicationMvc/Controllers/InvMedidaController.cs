using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Repositories;
using Proveduria.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using NLog;

namespace Proveduria.Controllers
{
    public class InvMedidaController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    List<INV_MEDIDA> list = unitOfWork.InvMedidaRepository.GetAll().ToList();
        //    return View(list);
        //}

        //[HttpGet]
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Create(INV_MEDIDA invMedida)
        //{
        //    JObject jObject = new JObject();
        //    try
        //    {
        //        unitOfWork.InvMedidaRepository.Insert(invMedida);
        //        unitOfWork.Save();
        //        jObject.Add("error", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        jObject.Add("error", true);
        //        logger.Error(ex, ex.Message);
        //    }
        //    return Content(jObject.ToString(), "application/json");
        //}

        //[HttpGet]
        //public ActionResult Edit(int? id)
        //{
        //    INV_MEDIDA invMedida = null;
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        invMedida = (from medida in unitOfWork.InvMedidaRepository.GetAll()
        //                    where medida.UNI_CODIGO == id.ToString()
        //                    select medida).First();
        //        if (invMedida == null)
        //        {
        //            return HttpNotFound();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, ex.Message);
        //    }
        //    return View(invMedida);
        //}

        //[HttpPost]
        //public ActionResult Edit(INV_MEDIDA invMedida)
        //{
        //    JObject jObject = new JObject();
        //    try
        //    {
        //        unitOfWork.InvMedidaRepository.Update(invMedida);
        //        unitOfWork.Save();
        //        jObject.Add("error", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        jObject.Add("error", true);
        //        logger.Error(ex, ex.Message);

        //    }
        //    return Content(jObject.ToString(), "application/json");
        //}

        //[HttpPost]
        //public ActionResult Delete(int? id)
        //{
        //    INV_MEDIDA invMedida = null;
        //    JObject jObject = new JObject();
        //    try
        //    {
        //        if (id == null)
        //        {
        //            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //        }
        //        invMedida = (from medida in unitOfWork.InvMedidaRepository.GetAll()
        //                     where medida.UNI_CODIGO == id.ToString()
        //                     select medida).First();
             
        //        if (invMedida == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        unitOfWork.InvMedidaRepository.Delete(invMedida.UNI_CODIGO);
        //        unitOfWork.Save();
        //        jObject.Add("error", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        jObject.Add("error", true);
        //        logger.Error(ex, ex.Message);
        //    }
        //    return Content(jObject.ToString(), "application/json");
        //}

    }
}