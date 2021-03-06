﻿using System.Web.Mvc;
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
using System.Net.Mime;
using Proveduria.Utils;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Linq.Expressions;

namespace Proveduria.Controllers
{
    [SessionTimeout]
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
        public ActionResult MovimientoItems()
        {
            string[] arrTiposOperacion = new string[] { "I", "E" };
            List<EPRTA_TIPO_MOVIMIENTO> ltipo_movimiento = unitOfWork.TipoMovimientoRepository.GetAll().Where(p => arrTiposOperacion.Contains(p.INGRESO_EGRESO) && p.ESTADO == "A").ToList();
            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Text = "Todos los Tipos", Value = "" });
            foreach (EPRTA_TIPO_MOVIMIENTO tip in ltipo_movimiento)
            {
                SelectListItem lin = new SelectListItem { Text = tip.NOMBRE, Value = tip.ID_TIPO_MOVIMIENTO.ToString() };
                lista.Add(lin);
            }
            ViewBag.ltipo_movimiento = lista;

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

        [HttpPost]
        public ActionResult GetMovimientos(string inicio, string fin, string anioMovimiento, string numeroMovimiento, string idItem, string tipoMovimiento)
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();

            List<EPRTA_MOVIMIENTO_DETALLE> data = ConsultaMovimientos(inicio, fin, anioMovimiento, numeroMovimiento, idItem, tipoMovimiento);

            var xdata = (from p in data
                         select new
                         {
                             ANIO = p.EPRTA_MOVIMIENTO.ANIO,
                             NUMERO_MOVIMIENTO = p.EPRTA_MOVIMIENTO.NUMERO_MOVIMIENTO,
                             TIPO_MOVIMIENTO = p.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                             ITEM = p.EPRTA_ITEM.DESCRIPCION,
                             CODIGO = p.EPRTA_ITEM.CODIGO,
                             p.CANTIDAD_MOVIMIENTO,
                             p.COSTO_MOVIMIENTO,
                             FECHA_SOLICITUD = p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD.HasValue ? p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null
                         }).ToList();
            enviar.Add("resultado", "success");
            enviar.Add("data", JArray.FromObject(xdata));

            return Content(enviar.ToString(), "application/json");
        }

        public List<EPRTA_MOVIMIENTO_DETALLE> ConsultaMovimientos(string inicio, string fin, string anioMovimiento, string numeroMovimiento, string idItem, string tipoMovimiento)
        {
            List<EPRTA_MOVIMIENTO_DETALLE> data = new List<EPRTA_MOVIMIENTO_DETALLE>();
            string[] arrTiposOperacion = new string[] { "I", "E" };
            if (!String.IsNullOrEmpty(inicio) ||
                            !String.IsNullOrEmpty(fin) ||
                            !String.IsNullOrEmpty(idItem) ||
                            !String.IsNullOrEmpty(anioMovimiento) ||
                            !String.IsNullOrEmpty(numeroMovimiento) ||
                            !String.IsNullOrEmpty(tipoMovimiento)
                           )
            {

                Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> xwhere = null;

                Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> exprg = (p => arrTiposOperacion.Contains(p.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO));
                xwhere = AppendExpression(xwhere, exprg);


                //var data = (from m in unitOfWork.MovimientoDetalleRepository.GetAll()
                //               select m);

                if (!String.IsNullOrEmpty(inicio) && !String.IsNullOrEmpty(fin))
                {
                    DateTime finicio = DateTime.Parse(inicio);
                    DateTime ffin = DateTime.Parse(fin);
                    Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> expr = (p => (p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD >= finicio && p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD <= ffin));
                    xwhere = AppendExpression(xwhere, expr);
                    //data = data.Where(p => p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD >= DateTime.Parse(inicio) && p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD <= DateTime.Parse(fin));
                }

                if (!String.IsNullOrEmpty(idItem) && idItem != "null")
                {
                    int piditem = int.Parse(idItem);
                    Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> expr = (p => p.ID_ITEM == piditem);
                    xwhere = AppendExpression(xwhere, expr);

                    //data = data.Where(p => p.ID_ITEM == int.Parse(idItem));
                }

                if (!String.IsNullOrEmpty(anioMovimiento))
                {
                    Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> expr = (p => p.EPRTA_MOVIMIENTO.ANIO == int.Parse(anioMovimiento));
                    xwhere = AppendExpression(xwhere, expr);

                    //data = data.Where(p => p.EPRTA_MOVIMIENTO.ANIO == int.Parse(anioMovimiento));
                }

                if (!String.IsNullOrEmpty(numeroMovimiento))
                {
                    Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> expr = (p => p.EPRTA_MOVIMIENTO.NUMERO_MOVIMIENTO == int.Parse(numeroMovimiento));
                    xwhere = AppendExpression(xwhere, expr);

                    //data = data.Where(p => p.EPRTA_MOVIMIENTO.NUMERO_MOVIMIENTO == int.Parse(numeroMovimiento));
                }

                if (!String.IsNullOrEmpty(tipoMovimiento))
                {
                    Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> expr = (p => p.EPRTA_MOVIMIENTO.ID_TIPO_MOVIMIENTO == int.Parse(tipoMovimiento));
                    xwhere = AppendExpression(xwhere, expr);

                    //data = data.Where(p => p.EPRTA_MOVIMIENTO.ID_TIPO_MOVIMIENTO == int.Parse(tipoMovimiento));
                }

                data = (from m in unitOfWork.MovimientoDetalleRepository.Where(xwhere)
                            select m).ToList();
                
            }
            return (data);
        }

        [HttpGet]
        public FileResult GetMovimientosExcel(string inicio, string fin, string anioMovimiento, string numeroMovimiento, string idItem, string tipoMovimiento)
        {
            MemoryStream stream = new MemoryStream();
            List<EPRTA_MOVIMIENTO_DETALLE> data = ConsultaMovimientos(inicio, fin, anioMovimiento, numeroMovimiento, idItem, tipoMovimiento);
            try
            {
                DataTable dt = new DataTable("Movimientos");
                dt.Columns.Add("ANIO");
                dt.Columns.Add("NUMERO_MOVIMIENTO");
                dt.Columns.Add("TIPO_MOVIMIENTO");
                dt.Columns.Add("ITEM");
                dt.Columns.Add("CODIGO");
                dt.Columns.Add("CANTIDAD_MOVIMIENTO");
                dt.Columns.Add("COSTO_MOVIMIENTO");
                dt.Columns.Add("FECHA_SOLICITUD");
                foreach (EPRTA_MOVIMIENTO_DETALLE item in data)
                {
                    dt.Rows.Add(
                        item.EPRTA_MOVIMIENTO.ANIO,
                        item.EPRTA_MOVIMIENTO.NUMERO_MOVIMIENTO,
                        item.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                        item.EPRTA_ITEM.DESCRIPCION,
                        item.EPRTA_ITEM.CODIGO,
                        item.CANTIDAD_MOVIMIENTO,
                        item.COSTO_MOVIMIENTO,
                        item.EPRTA_MOVIMIENTO.FECHA_SOLICITUD.HasValue ? item.EPRTA_MOVIMIENTO.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null
                        );
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
            string nombrearchivo = "Movimientos_" + long.Parse(DateTime.Now.ToString("ddMMyyyy")).ToString() + ".xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombrearchivo);
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

        [HttpPost]
        public ActionResult GetPuntoReOrden(DateTime pFechaInicio, DateTime pFechaFin, string pTodos)
        {
            DateTime mesAnterior = pFechaFin.AddMonths(-1);
            DateTime primerDia = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            JArray jArray = new JArray();
            JObject enviar = new JObject();

            List<EPRTA_ARTICULO_BODEGA> lista = GetPuntoReorden(pFechaInicio, pFechaFin, pTodos);

            //var tmp = (from p in unitOfWork.ArticuloBodegaRepository.GetAll()
            var tmp = (from p in lista
                       select new
                       {
                           p.ID_ITEM,
                           p.ID_BODEGA,
                           CODIGO = p.EPRTA_ITEM.CODIGO,
                           ITEM = p.EPRTA_ITEM.DESCRIPCION,
                           p.CANTIDAD_MAXIMA,
                           p.CANTIDAD_MINIMA,
                           p.CANTIDAD_ACTUAL,
                           p.CANTIDAD_CRITICA,
                           p.CANTIDAD_INICIO,
                           USADO = ((from u in unitOfWork.MovimientoDetalleRepository.Where(u => u.ID_ITEM == p.ID_ITEM && u.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO == "E"
                                                                                            && (u.EPRTA_MOVIMIENTO.FECHA_APROBACION >= pFechaInicio
                                                                                                && u.EPRTA_MOVIMIENTO.FECHA_APROBACION <= pFechaFin)
                                                                                          )
                                    group u by u.ID_ITEM into su
                                    select new { USADO = su.Sum(x => x.CANTIDAD_MOVIMIENTO) ?? 0  }).FirstOrDefault()?.USADO) ?? 0,
                           MES_ANTERIOR = ((from u in unitOfWork.MovimientoDetalleRepository.Where(u => u.ID_ITEM == p.ID_ITEM && u.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO == "E"
                                                                                            && (u.EPRTA_MOVIMIENTO.FECHA_APROBACION >= primerDia
                                                                                                && u.EPRTA_MOVIMIENTO.FECHA_APROBACION <= ultimoDia)
                                                                                          )
                                            group u by u.ID_ITEM into su
                                            select new { USADO = su.Sum(x => x.CANTIDAD_MOVIMIENTO) ?? 0 }).FirstOrDefault()?.USADO) ?? 0,

                       }).ToList();
            enviar.Add("resultado", "success");
            enviar.Add("data", JArray.FromObject(tmp));
            return Content(enviar.ToString(), "application/json");
        }

        [HttpGet]
        public FileResult GetPuntoReOrdenExcel(DateTime pFechaInicio, string pFechaFin, string pTodos)
        {
            DateTime xfechaFin = DateTime.Parse(pFechaFin);
            DateTime mesAnterior = xfechaFin.AddMonths(-1);
            DateTime primerDia = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            MemoryStream stream = new MemoryStream();
            
            List<EPRTA_ARTICULO_BODEGA> lista = GetPuntoReorden(pFechaInicio, xfechaFin, pTodos);

            //var tmp = (from p in unitOfWork.ArticuloBodegaRepository.GetAll()
            var tmp = (from p in lista
                       select new
                       {
                           p.ID_ITEM,
                           p.ID_BODEGA,
                           CODIGO = p.EPRTA_ITEM.CODIGO,
                           ITEM = p.EPRTA_ITEM.DESCRIPCION,
                           p.CANTIDAD_MAXIMA,
                           p.CANTIDAD_MINIMA,
                           p.CANTIDAD_ACTUAL,
                           p.CANTIDAD_CRITICA,
                           p.CANTIDAD_INICIO,
                           USADO = ((from u in unitOfWork.MovimientoDetalleRepository.Where(u => u.ID_ITEM == p.ID_ITEM && u.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO == "E"
                                                                                            && (u.EPRTA_MOVIMIENTO.FECHA_APROBACION >= pFechaInicio
                                                                                                && u.EPRTA_MOVIMIENTO.FECHA_APROBACION <= xfechaFin)
                                                                                          )
                                     group u by u.ID_ITEM into su
                                     select new { USADO = su.Sum(x => x.CANTIDAD_MOVIMIENTO) ?? 0 }).FirstOrDefault()?.USADO) ?? 0,
                           MES_ANTERIOR = ((from u in unitOfWork.MovimientoDetalleRepository.Where(u => u.ID_ITEM == p.ID_ITEM && u.EPRTA_MOVIMIENTO.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO == "E"
                                                                                            && (u.EPRTA_MOVIMIENTO.FECHA_APROBACION >= primerDia
                                                                                                && u.EPRTA_MOVIMIENTO.FECHA_APROBACION <= ultimoDia)
                                                                                          )
                                            group u by u.ID_ITEM into su
                                            select new { USADO = su.Sum(x => x.CANTIDAD_MOVIMIENTO) ?? 0 }).FirstOrDefault()?.USADO) ?? 0,

                       }).ToList();
            try
            {
                DataTable dt = new DataTable("Items");
                dt.Columns.Add("CODIGO_ITEM");
                dt.Columns.Add("ITEM");
                dt.Columns.Add("MAXIMA");
                dt.Columns.Add("MINIMA");
                dt.Columns.Add("CRITICA");
                dt.Columns.Add("INICIO");
                dt.Columns.Add("ACTUAL");
                dt.Columns.Add("USADO");
                dt.Columns.Add("MES_ANTERIOR");
                foreach (var item in tmp)
                {
                    dt.Rows.Add(
                        item.CODIGO,
                        item.ITEM,
                        item.CANTIDAD_MAXIMA,
                        item.CANTIDAD_MINIMA,
                        item.CANTIDAD_CRITICA,
                        item.CANTIDAD_INICIO,
                        item.CANTIDAD_ACTUAL,
                        item.USADO,
                        item.MES_ANTERIOR
                        );
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
            string nombrearchivo = "PtosReorden_" + pFechaInicio.ToString("ddMMyyyy").ToString() + "_" + xfechaFin.ToString("ddMMyyyy").ToString() + ".xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombrearchivo);
        }
        public List<EPRTA_ARTICULO_BODEGA> GetPuntoReorden(DateTime pFechaInicio, DateTime pFechaFin, string pTodos)
        {
            DateTime mesAnterior = pFechaFin.AddMonths(-1);
            DateTime primerDia = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            JArray jArray = new JArray();
            JObject enviar = new JObject();

            List<EPRTA_ARTICULO_BODEGA> lista = (from p in unitOfWork.ArticuloBodegaRepository.GetAll() select p).ToList();

            if (pTodos == "R")
            {
                lista = lista.Where(x => x.CANTIDAD_ACTUAL <= x.CANTIDAD_MINIMA).ToList();
            }

            return lista;


        }

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
        public ActionResult ListaCorteInventario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CorteInventarioFiltro(int pid_corte, string pfiltro)
        {
            JArray jArray = new JArray();
            JObject retorna = new JObject();
            try
            {
                EPRTA_CORTE_INVENTARIO corte = unitOfWork.CorteInventarioRepository.GetById(pid_corte);
                var tmp = (from p in corte.EPRTA_CORTE_INVENTARIO_DET
                           select new
                           {
                               CODIGO = p.EPRTA_ITEM.CODIGO,
                               ITEM = p.EPRTA_ITEM.DESCRIPCION,
                               p.CANTIDAD_ACTUAL,
                               p.COSTO_PROMEDIO,
                               p.TOTAL_STOCK
                           }).ToList();
                if (pfiltro == "S")
                {
                    tmp = tmp.Where(p => p.CANTIDAD_ACTUAL > 0).ToList();

                }
                retorna.Add("resultado", "success");
                retorna.Add("data", JArray.FromObject(tmp));

            }catch(Exception ex)
            {
                retorna.Add("resultado", "error");
                retorna.Add("msg", ex.Message);
                logger.Error(ex, ex.Message);
            }

            return Content(retorna.ToString(), "application/json");

        }

        

        [HttpGet]
        public FileResult CorteInventarioExcel(int pid_corte, string pfiltro)
        {

            MemoryStream stream = new MemoryStream();
            string nombrearchivo = "CORTE_TODOS_";
            EPRTA_CORTE_INVENTARIO corte = new EPRTA_CORTE_INVENTARIO();
            try
            {
                corte = unitOfWork.CorteInventarioRepository.GetById(pid_corte);

                DataTable dt = new DataTable("Items");
                dt.Columns.Add("CODIGO");
                dt.Columns.Add("ITEM");
                dt.Columns.Add("CANTIDAD_ACTUAL");
                dt.Columns.Add("COSTO_PROMEDIO");

                var detalle = (from p in corte.EPRTA_CORTE_INVENTARIO_DET
                           select new
                           {
                               CODIGO = p.EPRTA_ITEM.CODIGO,
                               ITEM = p.EPRTA_ITEM.DESCRIPCION,
                               p.CANTIDAD_ACTUAL,
                               p.COSTO_PROMEDIO,
                               p.TOTAL_STOCK
                           }).ToList();
                if (pfiltro == "S")
                {
                    nombrearchivo = "CORTE_CONSALDO_";
                    detalle = detalle.Where(p => p.CANTIDAD_ACTUAL > 0).ToList();

                }
                foreach (var item in detalle)
                {
                    dt.Rows.Add(
                        item.CODIGO,
                        item.ITEM,
                        item.CANTIDAD_ACTUAL,
                        item.COSTO_PROMEDIO
                        );
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
            
            nombrearchivo += corte.FECHA_CORTE?.ToString("ddMMyyyy") + ".xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombrearchivo);
        }


        [HttpPost]
        public ActionResult GetListaCorte()
        {
            JArray jArray = new JArray();
            JObject retorna = new JObject();

            try
            {
                var tmp = (from p in unitOfWork.CorteInventarioRepository.GetAll().OrderByDescending(x => x.FECHA_CORTE)
                           select new {
                               BODEGA = p.EPRTA_BODEGA.NOMBRE,
                               FECHA_CORTE = p.FECHA_CORTE.HasValue ? p.FECHA_CORTE.Value.ToString("dd/MM/yyyy") : null,
                               p.NUMERO_ITEMS,
                               p.TOTAL_CORTE,
                               ACCION = "<a href='/Consulta/CorteInventario/" + p.ID_CORTE + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                            "<i class='fa fa-search' aria-hidden='true'></i>" +
                                            "</a>"
                           }).ToList();
                retorna.Add("resultado", "success");
                retorna.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                retorna.Add("resultado", "error");
                retorna.Add("msg", ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }

        [HttpGet]
        public ActionResult CorteInventario(int id)
        {
            EPRTA_CORTE_INVENTARIO corte = null;
            JObject retorno = new JObject();
            if (id > 0)
            {
                corte = unitOfWork.CorteInventarioRepository.GetById(id);
                //var tmp = new { FECHA_CIERRE = cierre.FECHA_CIERRE,
                //    DETALLLE = (from t in cierre.EPRTA_CIERRE_INVENTARIO_DET select new {
                //        CODIGO = t.EPRTA_ITEM.CODIGO,
                //        ITEM = t.EPRTA_ITEM.DESCRIPCION,
                //        t.CANTIDAD_ACTUAL,
                //        t.COSTO_PROMEDIO,
                //        }).ToList()};
                //retorno.Add("resultado", "success");
                //retorno.Add("data", JObject.FromObject(tmp));
            }
            else
            {
                corte = new EPRTA_CORTE_INVENTARIO();
                corte.FECHA_CORTE = DateTime.Now;
                corte.ID_CORTE = 0;
            }
            //return Content(retorno.ToString(), "application/json");
            return View(corte);
        }

        public ActionResult GenerarCorteInventario()
        {
            JObject retorna = new JObject();
            Int32 pid_corte= 0;
            
            using (OracleConnection con = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReports"].ConnectionString.ToString()))
            {
                
                OracleCommand oc = new OracleCommand();
                oc.Connection = con;
              
                oc.CommandText = @"eprpr_genera_corte_inventario";
                oc.CommandType = CommandType.StoredProcedure;
                oc.Parameters.Add("pusuario", OracleDbType.Varchar2).Value = Session["usuario"];
                oc.Parameters.Add("pid_bodega", OracleDbType.Int16).Value = Session["bodega_id"];
                oc.Parameters.Add("pid_corte", OracleDbType.Int32).Direction = ParameterDirection.Output;

                try
                {
                    con.Open();
                    oc.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(oc);
                    pid_corte = Int32.Parse(oc.Parameters["pid_corte"].Value.ToString());
                    EPRTA_CORTE_INVENTARIO corte = unitOfWork.CorteInventarioRepository.GetById(pid_corte);
                    var tmp = new { FECHA_CORTE = corte.FECHA_CORTE,
                                    USUARIO_CORTE = corte.USUARIO_CORTE,
                                    DETALLE = (from p in corte.EPRTA_CORTE_INVENTARIO_DET
                                               select new { p.EPRTA_ITEM.CODIGO,
                                                            ITEM = p.EPRTA_ITEM.DESCRIPCION,
                                                            p.COSTO_PROMEDIO,
                                                            p.CANTIDAD_ACTUAL,
                                                            p.CANTIDAD_BAJA,
                                                            p.CANTIDAD_CRITICA,
                                                            p.CANTIDAD_INICIO,
                                                            p.CANTIDAD_MAXIMA,
                                                            p.CANTIDAD_MINIMA,
                                                            p.CANTIDAD_OC
                                               })};
                    retorna.Add("resultado", "success");
                    retorna.Add("data", JObject.FromObject(tmp));
                }
                catch (Exception ex)
                {
                    retorna.Add("resultado", "error");
                    retorna.Add("mensaje", ex.ToString());
                    logger.Error(ex, ex.Message);
                }
                con.Close();
            }
            return Content(retorna.ToString(), "application/json");
        }

        [HttpGet]
        public FileResult ExportPdfKardex(string pinicio, string pfin, int pidItem)
        {

            Stream stream = null;
            var nombreArchivo = "";
            ReportDocument reportDocument = new ReportDocument();
            byte[] pdfByte = null;

            try
            {
                object objetos = new object();
                EntitiesProveduria db = new EntitiesProveduria();
                SqlConnectionStringBuilder builderOrden = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                SP_KARDEXTableAdapter tableAdapter = new SP_KARDEXTableAdapter();
                DataTable dataTable;

                DateTime fi = DateTime.Parse(pinicio);
                DateTime ff = DateTime.Parse(pfin);
                dataTable = tableAdapter.GetData(pidItem, fi, ff, out objetos);


                String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Kardex.rpt");
                reportDocument.Load(pathReport);
                reportDocument.SetDataSource(dataTable);
                

                reportDocument.SetDatabaseLogon(builderOrden.UserID, builderOrden.Password);
                reportDocument.SetParameterValue("fecha_inicio", fi);
                reportDocument.SetParameterValue("fecha_fin", ff);
                stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                pdfByte = ReadFully(stream);
                nombreArchivo = "Kardex";
                Response.AddHeader("content-disposition", "inline; title='';" + "filename=" + nombreArchivo + ".pdf");
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            finally
            {
                if (reportDocument != null)
                {
                    if (reportDocument.IsLoaded)
                    {
                        reportDocument.Close();
                        reportDocument.Dispose();
                    }
                }
            }
            return File(pdfByte, MediaTypeNames.Application.Pdf);
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> AppendExpression(Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> left, Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> right)
        {
            Expression<Func<EPRTA_MOVIMIENTO_DETALLE, Boolean>> result;
            if (left == null)
            {
                left = model => true;
            }
            result = ExpressionExtension<EPRTA_MOVIMIENTO_DETALLE>.AndAlso(left, right);
            return result;
        }


    }
}