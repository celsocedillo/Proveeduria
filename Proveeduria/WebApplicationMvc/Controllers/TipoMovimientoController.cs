using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using NLog;
using Newtonsoft.Json;

namespace Proveduria.Controllers
{
    public class TipoMovimientoController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

        [HttpPost]
        public ActionResult GetListaTipoMovimiento()
        {
            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                //var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                //            select d;
                //foreach (EPRTA_TIPO_MOVIMIENTO item in query)
                //{
                //    JObject jsonObject = new JObject
                //    {
                //        { "ID_TIPO_MOVIMIENTO", item.ID_TIPO_MOVIMIENTO },
                //        { "NOMBRE", item.NOMBRE }
                //    };
                //    jArray.Add(jsonObject);
                //}
                //total = new JObject();
                //total.Add("items", jArray);
                //total.Add("error", false);

                var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                            select new { d.ID_TIPO_MOVIMIENTO, d.NOMBRE, d.INGRESO_EGRESO };
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


    }
}