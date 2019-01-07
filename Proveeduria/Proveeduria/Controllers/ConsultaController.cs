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
using System.Net.Mime;
using Proveduria.Utils;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;

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
        public ActionResult MovimientoBodega()
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
        public ActionResult ConsultaMovimiento(string inicio, string fin, string anioMovimiento, string numeroMovimiento, string idItem, string tipoMovimiento)
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();

            if (!String.IsNullOrEmpty(inicio) || 
                !String.IsNullOrEmpty(fin) || 
                !String.IsNullOrEmpty(idItem) ||
                !String.IsNullOrEmpty(anioMovimiento) ||
                !String.IsNullOrEmpty(numeroMovimiento) ||
                !String.IsNullOrEmpty(tipoMovimiento)
               )
            {
                var data = (from m in unitOfWork.MovimientoDetalleRepository.GetAll()
                               select m);

                if (!String.IsNullOrEmpty(inicio) && !String.IsNullOrEmpty(fin))
                {
                    data = data.Where(p => p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD >= DateTime.Parse(inicio) && p.EPRTA_MOVIMIENTO.FECHA_SOLICITUD <= DateTime.Parse(fin));
                }

                if (!String.IsNullOrEmpty(idItem))
                {
                    data = data.Where(p => p.ID_ITEM == int.Parse(idItem));
                }

                if (!String.IsNullOrEmpty(anioMovimiento))
                {
                    data = data.Where(p => p.EPRTA_MOVIMIENTO.ANIO == int.Parse(anioMovimiento));
                }

                if (!String.IsNullOrEmpty(numeroMovimiento))
                {
                    data = data.Where(p => p.EPRTA_MOVIMIENTO.NUMERO_MOVIMIENTO == int.Parse(numeroMovimiento));
                }

                if (!String.IsNullOrEmpty(tipoMovimiento))
                {
                    data = data.Where(p => p.EPRTA_MOVIMIENTO.ID_TIPO_MOVIMIENTO == int.Parse(tipoMovimiento));
                }
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
            }
            return Content(enviar.ToString(), "application/json");
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

        public ActionResult GetPuntoReOrden(DateTime pFechaInicio, DateTime pFechaFin)
        {
            DateTime mesAnterior = pFechaFin.AddMonths(-1);
            DateTime primerDia = new DateTime(mesAnterior.Year, mesAnterior.Month, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            JArray jArray = new JArray();
            JObject enviar = new JObject();

            var tmp = (from p in unitOfWork.ArticuloBodegaRepository.GetAll()
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
        public ActionResult ListaCierreInventario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetListaCierres()
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();

            try
            {
                var tmp = (from p in unitOfWork.CierreInventarioRepository.GetAll().OrderByDescending(x => x.FECHA_CIERRE)
                           select new {
                               BODEGA = p.EPRTA_BODEGA.NOMBRE,
                               FECHA_CIERRE = p.FECHA_CIERRE.HasValue ? p.FECHA_CIERRE.Value.ToString("dd/MM/yyyy") : null,
                               p.NUMERO_ITEMS,
                               p.TOTAL_CIERRE,
                               ACCION = "<a href='/Consulta/CierreInventario/" + p.ID_CIERRE + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                            "<i class='fa fa-search' aria-hidden='true'></i>" +
                                            "</a>"
                           }).ToList();
                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return Content(enviar.ToString(), "application/json");
        }

        [HttpGet]
        public ActionResult CierreInventario(int id)
        {
            EPRTA_CIERRE_INVENTARIO cierre = null;
            JObject retorno = new JObject();
            if (id > 0)
            {
                cierre = unitOfWork.CierreInventarioRepository.GetById(id);
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
                cierre = new EPRTA_CIERRE_INVENTARIO();
                cierre.FECHA_CIERRE = DateTime.Now;
                cierre.ID_CIERRE = 0;
            }
            //return Content(retorno.ToString(), "application/json");
            return View(cierre);
        }

        public ActionResult GenerarCorteInventario()
        {
            JObject enviar = new JObject();
            Int32 pid_cierre= 0;
            
            using (OracleConnection con = new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReports"].ConnectionString.ToString()))
            {
                
                OracleCommand oc = new OracleCommand();
                oc.Connection = con;
              
                oc.CommandText = @"eprpr_genera_corte_inventario";
                oc.CommandType = CommandType.StoredProcedure;
                oc.Parameters.Add("pusuario", OracleDbType.Varchar2).Value = Session["usuario"];
                oc.Parameters.Add("pid_bodega", OracleDbType.Int16).Value = Session["bodega_id"];
                oc.Parameters.Add("pid_cierre", OracleDbType.Int32).Direction = ParameterDirection.Output;

                try
                {
                    con.Open();
                    oc.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(oc);
                    pid_cierre = Int32.Parse(oc.Parameters["pid_cierre"].Value.ToString());
                    EPRTA_CIERRE_INVENTARIO cierre = unitOfWork.CierreInventarioRepository.GetById(pid_cierre);
                    var tmp = new { FECHA_CIERRE = cierre.FECHA_CIERRE,
                                    USUARIO_CIERRE = cierre.USUARIO_CIERRE,
                                    DETALLE = (from p in cierre.EPRTA_CIERRE_INVENTARIO_DET
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
                    enviar.Add("resultado", "success");
                    enviar.Add("data", JObject.FromObject(tmp));
                }
                catch (Exception ex)
                {
                    enviar.Add("resultado", "error");
                    enviar.Add("data", null);
                    enviar.Add("mensaje", ex.ToString());
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                }
                con.Close();
            }
            return Content(enviar.ToString(), "application/json");
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


    }
}