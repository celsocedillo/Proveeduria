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


    }
}