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
using System.IO;
using System.Data;
using ClosedXML.Excel;
using Proveduria.Utils;

namespace Proveduria.Controllers
{
    [SessionTimeout]
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
            JObject retorno = new JObject();
            try
            {
                EPRTA_TIPO_MOVIMIENTO tipomov = unitOfWork.TipoMovimientoRepository.GetById(pid);
                var tmp = new { tipomov.ID_TIPO_MOVIMIENTO, tipomov.NOMBRE, tipomov.INGRESO_EGRESO, tipomov.ESTADO};
                retorno.Add("resultado", "success");
                retorno.Add("data", JObject.FromObject(tmp)); 
                //return Json(new { resultado = "success", data = new { tipomov.ID_TIPO_MOVIMIENTO, tipomov.NOMBRE, tipomov.INGRESO_EGRESO, tipomov.ESTADO }, mensaje = "" });

            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", ex.Message);

                //return Json(new { resultado = "error", data = "", mensaje = " Error al consultar el tipo de movimiento, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetListaTipoMovimiento()
        {
            JObject retorno = new JObject();
            try
            {
                var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                            select new { d.ID_TIPO_MOVIMIENTO, d.NOMBRE, d.INGRESO_EGRESO, d.ESTADO, TIPO_INGEGR = d.INGRESO_EGRESO.Equals("I") ? "INGRESO" : "EGRESO" };
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


        [HttpGet]
        public FileResult ExportToExcel() //String fechaInicio, String fechaFin
        {
            System.IO.MemoryStream stream = new MemoryStream();
            try
            {
                //DateTime fi = DateTime.Parse(fechaInicio);
                //DateTime ff = DateTime.Parse(fechaFin);
                var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                                //where (d.FechaEmision >= fi && d.FechaEmision <= ff)
                            select d;
                DataTable dt = new DataTable("TipoMovimiento");
                dt.Columns.Add("IdTipoMovimiento");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Tipo");
                dt.Columns.Add("Estado");
                foreach (EPRTA_TIPO_MOVIMIENTO item in query)
                {
                    dt.Rows.Add(item.ID_TIPO_MOVIMIENTO,
                        item.NOMBRE,
                        //item.INGRESO_EGRESO,
                        item.INGRESO_EGRESO.Equals("I") ? "INGRESO" : "EGRESO",
                        item.ESTADO);
                }
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    workbook.Worksheets.Add(dt);
                    using (stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TipoMovimiento.xlsx");
        }


        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}