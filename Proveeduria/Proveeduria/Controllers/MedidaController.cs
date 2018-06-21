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
using System.IO;
using System.Data.SqlClient;
using Proveduria.Reports.DataSetReportsTableAdapters;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using ClosedXML.Excel;

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
        public ActionResult GetMedida(int pid)
        {
            JObject jsonObject = new JObject();
            try
            {
                EPRTA_MEDIDA medida = unitOfWork.MedidaRepository.GetById(pid);
                jsonObject.Add("ID_MEDIDA", medida.ID_MEDIDA);
                jsonObject.Add("NOMBRE", medida.NOMBRE);
                jsonObject.Add("resultado", "success");
                jsonObject.Add("mensaje", "success");
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                jsonObject.Add("resultado", "error");
            }
            return Content(jsonObject.ToString(), "application/json");

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
            catch (Exception ex)
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
                    precord.ESTADO = "A";
                    unitOfWork.MedidaRepository.Insert(precord);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.MedidaRepository.GetById(precord.ID_MEDIDA);
                    record.NOMBRE = precord.NOMBRE;
                    record.ESTADO = "A";
                    unitOfWork.MedidaRepository.Update(record);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
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

        [HttpGet]
        public FileResult ExportPdf()
        {
            Stream stream = null;
            var nombreArchivo = "";
            int IdMovimiento = 1313;
            try
            {
                //object objetos = new object();
                //EntitiesProveduria db = new EntitiesProveduria();
                //SqlConnectionStringBuilder builderVenta = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                //SP_REQUISICION_BODEGATableAdapter tableAdapter = new SP_REQUISICION_BODEGATableAdapter();

                //DataTable dataTable = tableAdapter.GetData(IdMovimiento, out objetos);
                //String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Requisicion_Bodega.rpt");
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathReport);
                //reportDocument.SetDataSource(dataTable);

                //reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

                //stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //stream.Seek(0, SeekOrigin.Begin);
                //nombreArchivo = "REQUISICION.pdf";

                //object objetos = new object();
                //EntitiesProveduria db = new EntitiesProveduria();
                //SqlConnectionStringBuilder builderVenta = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                //SP_EGRESO_BODEGATableAdapter tableAdapter = new SP_EGRESO_BODEGATableAdapter();

                //DataTable dataTable = tableAdapter.GetData(IdMovimiento, out objetos);
                //String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Egreso_Bodega.rpt");
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathReport);
                //reportDocument.SetDataSource(dataTable);

                //reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

                //stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                //stream.Seek(0, SeekOrigin.Begin);
                //nombreArchivo = "EGRESO.pdf";

                object objetos = new object();
                EntitiesProveduria db = new EntitiesProveduria();
                SqlConnectionStringBuilder builderVenta = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                SP_CORTE_INVENTARIOTableAdapter tableAdapter = new SP_CORTE_INVENTARIOTableAdapter();

                DataTable dataTable = tableAdapter.GetData(out objetos);
                String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Corte_Inventario.rpt");
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(pathReport);
                reportDocument.SetDataSource(dataTable);

                reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

                stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                nombreArchivo = "CORTE_INVENTARIO.pdf";
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return File(stream, "application/pdf", nombreArchivo);
        }

        [HttpGet]
        public FileResult ExportToExcel() //String fechaInicio, String fechaFin
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                //DateTime fi = DateTime.Parse(fechaInicio);
                //DateTime ff = DateTime.Parse(fechaFin);
                var query = from d in unitOfWork.MedidaRepository.GetAll()
                            //where (d.FechaEmision >= fi && d.FechaEmision <= ff)
                            select d;
                DataTable dt = new DataTable("Medidas");
                dt.Columns.Add("IdMedida");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Estado");
                //dt.Columns.Add("Establecimiento");
                //dt.Columns.Add("PuntoEmision");
                //dt.Columns.Add("Secuencial");
                //dt.Columns.Add("NumeroAutorizacion");
                //dt.Columns.Add("Estado");
                //dt.Columns.Add("Descuento", typeof(decimal));
                //dt.Columns.Add("PorcentajeIva", typeof(decimal));
                //dt.Columns.Add("BaseIvaCero", typeof(decimal));
                //dt.Columns.Add("BaseIva", typeof(decimal));
                //dt.Columns.Add("ValorIva", typeof(decimal));
                //dt.Columns.Add("ImporteTotal", typeof(decimal));
                foreach (EPRTA_MEDIDA item in query)
                {
                    dt.Rows.Add(item.ID_MEDIDA,
                        item.NOMBRE,
                        item.ESTADO);
                        //item.Establecimiento,
                        //item.PuntoEmision,
                        //item.Secuencial,
                        //item.NumeroAutorizacion,
                        //item.Estado,
                        //item.TotalDescuento,
                        //item.PorcentajeIva,
                        //item.BaseIvaCero,
                        //item.BaseIva,
                        //item.ValorIva,
                        //item.ImporteTotal);
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
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Medidas.xlsx");
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}
