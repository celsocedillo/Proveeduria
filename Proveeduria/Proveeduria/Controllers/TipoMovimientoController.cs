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
        public ActionResult GetTipoMovimiento(int pid)
        {
            try
            {
                EPRTA_TIPO_MOVIMIENTO tipomov = unitOfWork.TipoMovimientoRepository.GetById(pid);
                return Json(new { resultado = "success", data = new { tipomov.ID_TIPO_MOVIMIENTO, tipomov.NOMBRE, tipomov.INGRESO_EGRESO, tipomov.ESTADO }, mensaje = "" });

            }
            catch (Exception ex)
            {
                return Json(new { resultado = "error", data = "", mensaje = " Error al consultar el tipo de movimiento, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
        }

        [HttpPost]
        public ActionResult GetListaTipoMovimiento()
        {
            JObject retorna = new JObject();
            try
            {
                var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                            select new { d.ID_TIPO_MOVIMIENTO, d.NOMBRE, d.INGRESO_EGRESO, d.ESTADO, TIPO_INGEGR = d.INGRESO_EGRESO.Equals("I") ? "INGRESO" : "EGRESO" };
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
        public ActionResult Grabar(EPRTA_TIPO_MOVIMIENTO precord)
        {
            JObject retorno = new JObject();
            EPRTA_TIPO_MOVIMIENTO record;
            try
            {
                if (precord.ID_TIPO_MOVIMIENTO == 0)
                {
                    record = new EPRTA_TIPO_MOVIMIENTO();
                    record.NOMBRE = precord.NOMBRE;
                    record.INGRESO_EGRESO = precord.INGRESO_EGRESO;
                    record.ESTADO = "A";
                    unitOfWork.TipoMovimientoRepository.Insert(record);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.TipoMovimientoRepository.GetById(precord.ID_TIPO_MOVIMIENTO);
                    record.NOMBRE = precord.NOMBRE;
                    record.INGRESO_EGRESO = precord.INGRESO_EGRESO;
                    record.ESTADO = precord.ESTADO;
                    unitOfWork.TipoMovimientoRepository.Update(record);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
                logger.Info("Dato Grabado");
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