using CrystalDecisions.CrystalReports.Engine;
using Newtonsoft.Json.Linq;
using NLog;
using Proveduria.Models;
using Proveduria.Reports.DataSetReportsTableAdapters;
using Proveduria.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proveduria.Controllers
{
    public class KardexController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //GET: Kardex
        public ActionResult Kardex()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetKardex(string inicio, string fin)
        {

            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                //var searchValue = Request.Form.Get("search[value]");//Valor de busqueda
                var dataitems = unitOfWork.ArticuloBodegaRepository.GetAll();
                var totalItems = dataitems.Count();
                //Filtro por fecha inicio y fin
                if (inicio != "vacio" && fin != "vacio")
                {
                    DateTime fi = DateTime.Parse(inicio);
                    DateTime ff = DateTime.Parse(fin);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi && w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= fi);
                }
                //Filtro por fecha inicio
                else if (inicio != "vacio")
                {
                    DateTime fi = DateTime.Parse(inicio);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi);
                }
                //Filtro por fecha fin
                else if (fin != "vacio")
                {
                    DateTime ff = DateTime.Parse(fin);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= ff);
                }
                IEnumerable<EPRTA_ARTICULO_BODEGA> dataShow = dataitems.Skip(int.Parse(Request.Form.Get("start"))).Take(int.Parse(Request.Form.Get("length")));
                foreach (EPRTA_ARTICULO_BODEGA item in dataShow)
                {
                    var codigo = item.EPRTA_ITEM.CODIGO ?? "";
                    var descripcion = item.EPRTA_ITEM.DESCRIPCION ?? "";
                    JObject jsonObject = new JObject
                    {
                        { "item", codigo + " - " + descripcion },
                        { "maximo", item.CANTIDAD_MAXIMA != null ? item.CANTIDAD_MAXIMA:0 },
                        { "minimo", item.CANTIDAD_MINIMA != null ? item.CANTIDAD_MINIMA:0 },
                        { "critica", item.CANTIDAD_CRITICA != null ? item.CANTIDAD_CRITICA:0 },
                        { "inicio",  item.CANTIDAD_INICIO != null ? item.CANTIDAD_INICIO:0 },
                        { "actual",  item.CANTIDAD_ACTUAL != null ? item.CANTIDAD_ACTUAL:0 },
                        { "usado",  0},
                        { "messiete", 0 }
                    };
                    jArray.Add(jsonObject);
                }
                total.Add("draw", Request.Form.Get("draw"));
                total.Add("recordsTotal", totalItems);
                total.Add("recordsFiltered", dataitems.Count());
                total.Add("data", jArray);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");
        }


        [HttpGet]
        public FileResult ExportPdf(string inicio, string fin, string id)
        {
            Stream stream = null;
            var nombreArchivo = "";
            int idItem = 330;
            string fechaInicio = "01/01/2017";
            string fechaFin = "01/01/2018";
            
            try
            {
                object objetos = new object();
                EntitiesProveduria db = new EntitiesProveduria();
                SqlConnectionStringBuilder builderOrden = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                SP_KARDEXTableAdapter tableAdapter = new SP_KARDEXTableAdapter();
                DataTable dataTable;

                DateTime fi = DateTime.Parse(fechaInicio);
                DateTime ff = DateTime.Parse(fechaFin);
                dataTable = tableAdapter.GetData(idItem, fi, ff, out objetos);


                //if (inicio != "vacio" && fin != "vacio")
                //{
                //    DateTime fi = DateTime.Parse(inicio);
                //    DateTime ff = DateTime.Parse(fin);
                //    dataTable = tableAdapter.GetData(idItem, fi, ff, out objetos);
                //    //dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi && w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= fi);
                //}
                ////Filtro por fecha inicio
                //else if (inicio != "vacio")
                //{
                //    DateTime fi = DateTime.Parse(inicio);
                //    dataTable = tableAdapter.GetData(idItem, fi, null, out objetos);
                //    //dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi);
                //}
                ////Filtro por fecha fin
                //else if (fin != "vacio")
                //{
                //    DateTime ff = DateTime.Parse(fin);
                //    dataTable = tableAdapter.GetData(idItem, null, ff, out objetos);
                //    //dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= ff);
                //}


                //dataTable = tableAdapter.GetData(idItem, null, null, out objetos);

                String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Kardex.rpt");
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(pathReport);
                reportDocument.SetDataSource(dataTable);

                reportDocument.SetDatabaseLogon(builderOrden.UserID, builderOrden.Password);

                stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                nombreArchivo = "KARDEX.pdf";
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return File(stream, "application/pdf", nombreArchivo);
        }

    }
}