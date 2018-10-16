using System.Web.Mvc;
using Proveduria.Repositories;
using NLog;
using Newtonsoft.Json.Linq;
using System;
using Proveduria.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using ClosedXML.Excel;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using Proveduria.Reports.DataSetReportsTableAdapters;
using System.Web;

namespace Proveduria.Controllers
{
    public class ConsultaController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public ActionResult PuntoReOrden()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetPuntosReOrden(string inicio, string fin)
        {

            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                var searchValue = Request.Form.Get("search[value]");//Valor de busqueda
                var  dataitems = unitOfWork.ArticuloBodegaRepository.GetAll();
                var totalItems = dataitems.Count();
                //Filtro por nombre
                if (!String.IsNullOrEmpty(searchValue))
                {
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.DESCRIPCION.ToLower().Contains(searchValue.ToLower()));
                }
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
                //var sortColumnIndex = Convert.ToInt32(Request.Form.Get("order[0][column]"));
                //Func<EPRTA_MOVIMIENTO, string> orderingFunction = (c => sortColumnIndex == 0 ? c.Region.ToString() : sortColumnIndex == 1 ? c.Categoria.Nombre : c.Nombre);
                //var sortDirection = Request.Form.Get("order[0][dir]");
                //if (sortDirection == "asc")
                //{
                //    dataitems = dataitems.OrderBy(orderingFunction);
                //}
                //else
                //{
                //    dataitems = dataitems.OrderByDescending(orderingFunction);
                //}
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
        public ActionResult MovimientoBodega()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MovimientoBodega(string inicio, string fin, string idItem, string codigoMovimiento, string tipoMovimiento)
        {

            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                var searchValue = Request.Form.Get("search[value]");
                List<ConsultaMovimiento> data = (from m in unitOfWork.MovimientoRepository.GetAll()
                           join md in unitOfWork.MovimientoDetalleRepository.GetAll()
                           on m.ID_MOVIMIENTO equals md.ID_MOVIMIENTO
                           select new ConsultaMovimiento
                           {
                               ID_MOVIMIENTO = m.ID_MOVIMIENTO,
                               TIPO_MOVIMIENTO = m.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                               ITEM = md.EPRTA_ITEM.DESCRIPCION,
                               NUMERO_MOVIMIENTO = m.NUMERO_MOVIMIENTO,
                               OBSERVACION = m.OBSERVACION,
                               CANTIDAD_MOVIMIENTO = md.CANTIDAD_MOVIMIENTO,
                               FECHA_SOLICITUD = m.FECHA_SOLICITUD
                           }).ToList();

                #region Filtro
                IEnumerable<ConsultaMovimiento> filtered;
                if (!String.IsNullOrEmpty(searchValue))
                {
                    filtered = data.Where(w => w.NUMERO_MOVIMIENTO.ToString().Contains(searchValue.ToLower()));
                }
                else
                {
                    filtered = data;
                }
                //if (numero != "todos")
                //{
                //    filtered = filtered.Where(w => w.Numero == numero);
                //}
                //if (idTipo != "todos")
                //{
                //    int tipoMovimiento = int.Parse(idTipo);
                //    filtered = filtered.Where(w => w.IdMovimientoTipo == tipoMovimiento);
                //}
                //if (idBodega != "todos")
                //{
                //    int idBodegaMovimiento = int.Parse(idBodega);
                //    filtered = filtered.Where(w => w.IdBodegaOrigen == idBodegaMovimiento || w.IdBodegaDestino == idBodegaMovimiento);
                //}
                //if (idProducto != "todos")
                //{
                //    int idProductoMovimiento = int.Parse(idProducto);
                //    filtered = filtered.Where(w => w.IdMovimientoTipo == idProductoMovimiento);
                //}
                if (inicio != "todos" && fin != "todos")
                {
                    DateTime fi = DateTime.Parse(inicio);
                    DateTime ff = DateTime.Parse(fin);
                    filtered = filtered.Where(w => w.FECHA_SOLICITUD >= fi && w.FECHA_SOLICITUD <= ff);
                }
                else if (inicio != "todos")
                {
                    DateTime fi = DateTime.Parse(inicio);
                    filtered = filtered.Where(w => w.FECHA_SOLICITUD >= fi);
                }
                else if (fin != "todos")
                {
                    DateTime ff = DateTime.Parse(fin);
                    filtered = filtered.Where(w => w.FECHA_SOLICITUD <= ff);
                }
                #endregion  

                #region Orden
                //var sortColumnIndex = Convert.ToInt32(Request.Form.Get("order[0][column]"));
                //Func<EPRTA_MOVIMIENTO, string> orderingFunction = (c => sortColumnIndex == 0 ? c.Region.ToString() : sortColumnIndex == 1 ? c.Categoria.Nombre : c.Nombre);
                //var sortDirection = Request.Form.Get("order[0][dir]");
                //if (sortDirection == "asc")
                //{
                //    dataitems = dataitems.OrderBy(orderingFunction);
                //}
                //else
                //{
                //    dataitems = dataitems.OrderByDescending(orderingFunction);
                //}
                #endregion

                #region Json
                IEnumerable<ConsultaMovimiento> dataShow = filtered.Skip(int.Parse(Request.Form.Get("start"))).Take(int.Parse(Request.Form.Get("length")));
                foreach (ConsultaMovimiento item in dataShow)
                {
                    JObject jsonObject = new JObject
                    {
                        { "codigoMovimiento", item.NUMERO_MOVIMIENTO},
                        { "tipoMovimiento", item.TIPO_MOVIMIENTO },
                        { "item", item.ITEM},
                        { "descripcion",  item.OBSERVACION },
                        { "cantidad",  item.CANTIDAD_MOVIMIENTO },
                        { "valor",  0},
                        { "fecha", item.FECHA_SOLICITUD },
                    };
                    jArray.Add(jsonObject);
                }
                total.Add("draw", Request.Form.Get("draw"));
                total.Add("recordsTotal", data.Count());
                total.Add("recordsFiltered", filtered.Count());
                total.Add("data", jArray);
                #endregion



            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");
        }

        [HttpGet]
        public FileResult ExportToExcelPuntosReOrden(string fechaInicio, string fechaFin) 
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                var dataitems = unitOfWork.ArticuloBodegaRepository.GetAll();
                var totalItems = dataitems.Count();
                //Filtro por fecha inicio y fin
                if (fechaInicio != "vacio" && fechaFin != "vacio")
                {
                    DateTime fi = DateTime.Parse(fechaInicio);
                    DateTime ff = DateTime.Parse(fechaFin);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi && w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= fi);
                }
                //Filtro por fecha inicio
                else if (fechaInicio != "vacio")
                {
                    DateTime fi = DateTime.Parse(fechaInicio);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_INGRESO >= fi);
                }
                //Filtro por fecha fin
                else if (fechaFin != "vacio")
                {
                    DateTime ff = DateTime.Parse(fechaFin);
                    dataitems = dataitems.Where(w => w.EPRTA_ITEM.FECHA_ULTIMO_EGRESO <= ff);
                }
                DataTable dt = new DataTable("PuntosReOrden");
                dt.Columns.Add("Codigo");
                dt.Columns.Add("Item");
                dt.Columns.Add("Maximo");
                dt.Columns.Add("Minimo");
                dt.Columns.Add("Critica");
                dt.Columns.Add("Inicio");
                dt.Columns.Add("Actual");
                dt.Columns.Add("Usado");
                dt.Columns.Add("MesSiete");
                foreach (EPRTA_ARTICULO_BODEGA item in dataitems)
                {
                    dt.Rows.Add(item.EPRTA_ITEM.CODIGO,
                        item.EPRTA_ITEM.DESCRIPCION,
                        item.CANTIDAD_MAXIMA,
                        item.CANTIDAD_MINIMA,
                        item.CANTIDAD_CRITICA,
                        item.CANTIDAD_INICIO,
                        item.CANTIDAD_ACTUAL,
                        0,
                        0);
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
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PuntosReOrden.xlsx");
        }

        /*Kardex*/

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
        public FileResult ExportPdfKardex(string inicio, string fin, string id)
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