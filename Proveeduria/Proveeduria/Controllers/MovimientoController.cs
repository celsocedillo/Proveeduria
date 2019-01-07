using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Models.Enumadores;
using Proveduria.Repositories;
using Newtonsoft.Json.Linq;
using NLog;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mime;
using Proveduria.Reports.DataSetReportsTableAdapters;
using System.Linq.Expressions;
using Proveduria.Utils;

namespace Proveduria.Controllers
{
    [SessionTimeout]
    public class MovimientoController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Movimiento
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ListaMovimiento()
        {
            string[] arrTiposOperacion = new string[] { "I", "E" };
            //List<EPRTA_TIPO_MOVIMIENTO> ltipo_movimiento = unitOfWork.TipoMovimientoRepository.GetAll().Where(p => arrTiposOperacion.Contains(p.INGRESO_EGRESO) && p.ESTADO == "A").ToList();//

            List<EPRTA_TIPO_MOVIMIENTO> ltipo_movimiento = unitOfWork.TipoMovimientoRepository.Where(x=> arrTiposOperacion.Contains(x.INGRESO_EGRESO) && x.ESTADO == "A").ToList();

            List<SelectListItem> lista = new List<SelectListItem>();
            lista.Add(new SelectListItem { Text = "Todos los Tipos", Value = "0" });
            foreach (EPRTA_TIPO_MOVIMIENTO tip in ltipo_movimiento)
            {
                SelectListItem lin = new SelectListItem { Text = tip.NOMBRE, Value = tip.ID_TIPO_MOVIMIENTO.ToString() };
                lista.Add(lin);
            }
            ViewBag.ltipo_movimiento = lista;
            return View();
        }

        [HttpPost]
        public ActionResult GetListaMovimiento(DateTime pfecha_inicio, DateTime pfecha_fin, int pid_tipo_movimiento)
        {
            string[] arrTiposOperacion = new string[] { "I", "E" };
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);

                //var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                //           select p);

                Expression<Func<EPRTA_MOVIMIENTO, Boolean>> xwhere = null;


                if (pid_tipo_movimiento != 0)
                {
                    Expression<Func<EPRTA_MOVIMIENTO, Boolean>> expr = (p => p.ID_TIPO_MOVIMIENTO == pid_tipo_movimiento);
                    xwhere = AppendExpression(xwhere, expr);
                    //tmp = tmp.Where(p => p.ID_TIPO_MOVIMIENTO == pid_tipo_movimiento);
                }
                else
                {
                    Expression<Func<EPRTA_MOVIMIENTO, Boolean>> expr = (p => arrTiposOperacion.Contains(p.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO));
                    xwhere = AppendExpression(xwhere, expr);

                    //tmp = tmp.Where(p => arrTiposOperacion.Contains(p.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO));
                }
                if (pfecha_inicio != null || pfecha_fin != null)
                {
                    Expression<Func<EPRTA_MOVIMIENTO, Boolean>> expr = (p => (p.FECHA_SOLICITUD >= pfecha_inicio && p.FECHA_SOLICITUD <= pfecha_fin));
                    xwhere = AppendExpression(xwhere, expr);

                    //tmp = tmp.Where(p =>( p.FECHA_SOLICITUD >= pfecha_inicio && p.FECHA_SOLICITUD <= pfecha_fin));
                }

                var tmp = unitOfWork.MovimientoRepository.Where(xwhere).ToList();

                var lis = (from p in tmp
                       select new
                       {
                           p.ID_MOVIMIENTO,
                           p.ANIO,
                           p.NUMERO_MOVIMIENTO,
                           FECHA_SOLICITUD = p.FECHA_SOLICITUD.HasValue ? p.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null,
                           p.OBSERVACION,
                           p.ID_TIPO_MOVIMIENTO,
                           MOVIMIENTO = p.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                           p.ESTADO,
                           NOMBREESTADO = (
                           p.ESTADO.Equals("D") ? "Despachado" :
                           p.ESTADO.Equals("E") ? "Anulado" :
                           p.ESTADO.Equals("A") ? "Autorizado" :
                           p.ESTADO.Equals("S") ? "Solicitado" : null),
                           p.USUARIO_SOLICITA,
                           p.USUARIO_AUTORIZA,
                           p.USUARIO_APRUEBA,
                           FECHA_AUTORIZACION = p.FECHA_AUTORIZACION.HasValue ? p.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null,
                           FECHA_APROBACION = p.FECHA_APROBACION.HasValue ? p.FECHA_APROBACION.Value.ToString("dd/MM/yyyy") : null,
                           ACCION = "<a href='/Movimiento/Movimiento/" + p.ID_MOVIMIENTO + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                         "<i class='fa fa-search' aria-hidden='true'></i>" +
                         "</a>"
                       }).ToList();

                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(lis));

            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);
            }
            return Content(enviar.ToString(), "application/json");

        }

        [HttpPost]
        public ActionResult GetListaSolicitudAutorizada()
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                var dep = (from d in unitOfWork.DepartamentoRepository.GetAll() select d);
                var dir = (from d in unitOfWork.DireccionRepository.GetAll() select d);
                var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                           join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                           join di in dir on p.ID_DIRECCION_SOLICITA equals di.CODIGO
                           where p.ESTADO == "A" && p.ID_TIPO_MOVIMIENTO == 12
                           select new
                           {
                               p.ID_MOVIMIENTO,
                               p.ANIO,
                               p.NUMERO_MOVIMIENTO,
                               p.USUARIO_SOLICITA,
                               MOVIMIENTO = p.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                               EMPLEADO = us.EMPLEADO,
                               DEPARTAMENTO = di.DESCRIPCION,
                               FECHA = p.FECHA_AUTORIZACION.HasValue ? p.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null
                           }).ToList();
                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);

            }
            return Content(enviar.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetListaOrdenCompra()
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var tmp = (from p in unitOfWork.OrdenCompraRepository.GetAll()
                           where p.ESTADO == "A"
                           select new
                           {
                               p.ANIO,
                               NUMERO_MOVIMIENTO = p.NUMERO_ORDEN,
                               PROVEEDOR = p.PROVEEDOR,
                               FACTURA = p.FACTURA,
                               FECHA_EMISION = p.FECHA_EMISION,
                               FECHA = p.FECHA_AUTORIZACION.HasValue ? p.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null
                           }).ToList();
                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);

            }
            return Content(enviar.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetDetalleSolicitudAutorizada(int pid_movimiento)
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                EPRTA_MOVIMIENTO movimiento = unitOfWork.MovimientoRepository.GetById(pid_movimiento);
                var stock = unitOfWork.ArticuloBodegaRepository.GetAll();
                var tmp = (from p in movimiento.EPRTA_MOVIMIENTO_DETALLE
                               //join st in stock on p.ID_ITEM equals st.ID_ITEM  into pst from st in pst.DefaultIfEmpty()
                           join st in stock on new { id_bodega = movimiento.ID_BODEGA,
                                                     id_item = p.ID_ITEM} equals 
                                               new { id_bodega = st.ID_BODEGA, id_item = st.ID_ITEM }
                           into pst
                           from st in pst.DefaultIfEmpty()
                           select new
                           { CODIGO = p.EPRTA_ITEM.CODIGO,
                               ITEM = p.EPRTA_ITEM.DESCRIPCION,
                               CANTIDAD = p.CANTIDAD_MOVIMIENTO,
                               MEDIDA = p.EPRTA_ITEM.EPRTA_MEDIDA.NOMBRE,
                               STOCK_ACTUAL = st.CANTIDAD_ACTUAL,
                               COSTO_ACTUAL = st.COSTO_PROMEDIO,
                               ID_ITEM = p.ID_ITEM
                           }).ToList();
                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);
            }
            return Content(enviar.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetDetalleOrdenCompra(int panio, int pnumero_orden)
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                VW_ORDEN_COMPRA orden = unitOfWork.OrdenCompraRepository.GetAll().Where(p => p.ANIO == panio && p.NUMERO_ORDEN == pnumero_orden).FirstOrDefault();
                var stock = unitOfWork.ArticuloBodegaRepository.GetAll();
                var tmp = (from p in orden.VW_ORDEN_COMPRA_DETALLE 
                           join st in stock on p.ID_ITEM equals st.ID_ITEM into pst
                           from st in pst.DefaultIfEmpty()
                           select new
                           {
                               CODIGO = p.EPRTA_ITEM.CODIGO,
                               ITEM = p.EPRTA_ITEM.DESCRIPCION,
                               CANTIDAD = p.CANTIDAD_PEDIDA,
                               MEDIDA = p.EPRTA_ITEM.EPRTA_MEDIDA.NOMBRE,
                               STOCK_ACTUAL = st.CANTIDAD_ACTUAL,
                               VALOR_UNITARIO = p.VALOR_UNITARIO,
                               ID_ITEM = p.ID_ITEM
                           }).ToList();
                enviar.Add("resultado", "success");
                enviar.Add("data", JArray.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);
            }
            return Content(enviar.ToString(), "application/json");
        }

        //[HttpGet]
        //public FileResult ViewPDF(int? id)
        //{
        //    Stream stream = null;
        //    var nombreArchivo = "";
        //    ReportDocument reportDocument = new ReportDocument();
        //    byte[] pdfByte = null;
        //    try
        //    {
        //        FacturaVenta facturaVenta = unitOfWork.FacturaVentaRepository.GetById(id);
        //        ERPDBEntities db = new ERPDBEntities();
        //        SqlConnectionStringBuilder builderVenta = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
        //        SpFacturaElectronicaTableAdapter tableAdapter = new SpFacturaElectronicaTableAdapter();
        //        DataTable dataTable = tableAdapter.GetData(id);
        //        String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Report\\CRFEFactura.rpt");

        //        reportDocument.Load(pathReport);
        //        reportDocument.SetDataSource(dataTable);

        //        SpDetalleFormaPagoTableAdapter detalleFormaPagoTableAdapter = new SpDetalleFormaPagoTableAdapter();
        //        DataTable dataTableDetalleFormaPago = detalleFormaPagoTableAdapter.GetData(facturaVenta.Id);
        //        reportDocument.Subreports[0].SetDataSource(dataTableDetalleFormaPago);

        //        SpInformacionAdicionalTableAdapter informacionAdicionalTableAdapter = new SpInformacionAdicionalTableAdapter();
        //        DataTable dataTableInformacionAdicional = informacionAdicionalTableAdapter.GetData(facturaVenta.Id, Util.GetEnumDescription(EnumCodigoDocumento.Factura));
        //        reportDocument.Subreports[1].SetDataSource(dataTableInformacionAdicional);

        //        reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

        //        stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
        //        pdfByte = Util.ReadFully(stream);
        //        nombreArchivo = "FACTURA-" + facturaVenta.Establecimiento + "-" + facturaVenta.PuntoEmision + "-" + facturaVenta.Secuencial;
        //        Response.AddHeader("content-disposition", "inline; title='';" + "filename=" + nombreArchivo + ".pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, ex.Message);
        //    }
        //    finally
        //    {
        //        if (reportDocument != null)
        //        {
        //            if (reportDocument.IsLoaded)
        //            {
        //                reportDocument.Close();
        //                reportDocument.Dispose();
        //            }
        //        }
        //    }
        //    return File(pdfByte, MediaTypeNames.Application.Pdf);
        //}


        [HttpGet]
        public ActionResult Movimiento(int id)
        {
            //JArray jArray = new JArray();
            //JObject enviar = new JObject();
            EPRTA_MOVIMIENTO movimiento = null;
            string[] arrTiposOperacion = new string[] { "I", "E" };
            List<EPRTA_TIPO_MOVIMIENTO> ltipo_movimiento = unitOfWork.TipoMovimientoRepository.GetAll().Where(p => arrTiposOperacion.Contains(p.INGRESO_EGRESO) && p.ESTADO =="A").ToList();
            ViewBag.ltipo_movimiento = ltipo_movimiento;
            try
            {
                if (id> 0)
                {
                    var empleados = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                    movimiento = unitOfWork.MovimientoRepository.GetById(id);
                    ViewBag.usuario_solicita = (string)(from p in empleados where p.USUARIO == movimiento.USUARIO_SOLICITA select p.EMPLEADO).FirstOrDefault();
                    ViewBag.usuario_aprueba = movimiento.USUARIO_APRUEBA != null ? (from p in empleados where p.USUARIO == movimiento.USUARIO_APRUEBA select p.EMPLEADO).FirstOrDefault() : null;
                    ViewBag.usuario_autoriza = movimiento.USUARIO_AUTORIZA != null ? (from p in empleados where p.USUARIO == movimiento.USUARIO_AUTORIZA select p.EMPLEADO).FirstOrDefault() : null;
                    //ViewBag.departamento_solicitud = unitOfWork.DepartamentoRepository.GetById(movimiento.ID_DEPARTAMENTO_SOLICITA).DESCRIPCION;
                    ViewBag.direccion_solicitud = unitOfWork.DireccionRepository.GetById(movimiento.ID_DIRECCION_SOLICITA).DESCRIPCION;
                }
                else
                {
                    movimiento = new EPRTA_MOVIMIENTO();
                    movimiento.ESTADO = "S";
                    movimiento.ID_DIRECCION_SOLICITA = Convert.ToByte(Session["id_direccion"]);
                    ViewBag.direccion_solicitud = Session["direccion"];
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return View(movimiento);
        }

        [HttpPost]
        public ActionResult Grabar(EPRTA_MOVIMIENTO pmovimiento)
        {
            JObject retorno = new JObject();
            try
            {
                //EPRTA_MOVIMIENTO movimiento = new EPRTA_MOVIMIENTO();
                pmovimiento.USUARIO_SOLICITA = Session["usuario"].ToString();
                pmovimiento.FECHA_SOLICITUD = DateTime.Now;
                pmovimiento.ESTADO = "D";
                pmovimiento.ANIO = (short)DateTime.Now.Year;
                pmovimiento.ID_BODEGA = Byte.Parse(Session["bodega_id"].ToString());
                foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                {
                    detalle.ESTADO = "A";
                    EPRTA_ARTICULO_BODEGA itemstock = unitOfWork.ArticuloBodegaRepository.GetAll().Where(p => p.ID_ITEM == detalle.ID_ITEM && p.ID_BODEGA == Byte.Parse(Session["bodega_id"].ToString())).FirstOrDefault();
                    if (pmovimiento.ID_TIPO_MOVIMIENTO == (int)EnumTipoMovimiento.REQUISICION_BODEGA || pmovimiento.ID_TIPO_MOVIMIENTO == (int)EnumTipoMovimiento.AJUSTE_DE_BODEGA_POR_EGRESO)
                    {
                        if (itemstock.CANTIDAD_ACTUAL == 0)
                        {
                            throw new ArgumentException("El item " + itemstock.EPRTA_ITEM.DESCRIPCION + " tiene stock 0, y no se puede rebajar");
                        }
                    }

                    detalle.COSTO_ACTUAL = itemstock == null ? 0 : itemstock.COSTO_PROMEDIO;
                    detalle.STOCK_ACTUAL = itemstock == null ? 0 : itemstock.CANTIDAD_ACTUAL;
                    if (pmovimiento.ID_TIPO_MOVIMIENTO != 4)
                    {
                        detalle.COSTO_MOVIMIENTO = itemstock.COSTO_PROMEDIO;
                    }
                }
                EPRTA_SECUENCIA secuencia = unitOfWork.SecuenciaRepository.GetAll().Where(p => p.ID_TIPO_MOVIMIENTO == pmovimiento.ID_TIPO_MOVIMIENTO && p.ANIO == pmovimiento.ANIO).FirstOrDefault();
                pmovimiento.NUMERO_MOVIMIENTO = (int)secuencia.SECUENCIA;
                secuencia.SECUENCIA++;
                unitOfWork.MovimientoRepository.Insert(pmovimiento);
                unitOfWork.SecuenciaRepository.Update(secuencia);
                //EPRTA_MOVIMIENTO x = unitOfWork.MovimientoRepository.GetById(movimiento.ID_MOVIMIENTO);
                string ingreso_egreso = unitOfWork.TipoMovimientoRepository.GetById(pmovimiento.ID_TIPO_MOVIMIENTO).INGRESO_EGRESO;
                ActualizaStock(pmovimiento, ingreso_egreso);
                unitOfWork.Save();
                pmovimiento = unitOfWork.MovimientoRepository.GetById(pmovimiento.ID_MOVIMIENTO);
                
                retorno.Add("resultado", "success");
                retorno.Add("data", pmovimiento.ID_MOVIMIENTO);
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

        private void ActualizaStock(EPRTA_MOVIMIENTO pmovimiento, string pingreso_egreso)
        {
            try  {
                foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                {
                    //Obtengo el registro del item
                    EPRTA_ITEM item = unitOfWork.ItemRepository.GetById(detalle.ID_ITEM);

                    //Obtengo el registro del stock del item
                    EPRTA_ARTICULO_BODEGA itemStock = unitOfWork.ArticuloBodegaRepository.GetAll().Where(p => p.ID_BODEGA == pmovimiento.ID_BODEGA && p.ID_ITEM == detalle.ID_ITEM).FirstOrDefault();
                    if (pingreso_egreso == "E")
                    {
                        //Actualizo el stock y el costo unitario
                        itemStock.CANTIDAD_ACTUAL -= detalle.CANTIDAD_MOVIMIENTO;
                        unitOfWork.ArticuloBodegaRepository.Update(itemStock);

                        //Actualizo la fecha del ultimo egreso
                        item.FECHA_ULTIMO_EGRESO = DateTime.Now;
                        unitOfWork.ItemRepository.Update(item);
                    }
                    else if (pingreso_egreso == "I")
                    {
                        //Calcular nuevo costo promedio
                        decimal nuevo_costo_promedio = 0;
                        decimal nuevo_cantidad_stock = 0;

                        if (itemStock == null)
                        {
                            nuevo_costo_promedio = Convert.ToDecimal(detalle.COSTO_MOVIMIENTO);
                        }
                        else
                        {
                            decimal costo_total_actual = Convert.ToDecimal(itemStock.CANTIDAD_ACTUAL) * Convert.ToDecimal(itemStock.COSTO_PROMEDIO);
                            decimal costo_total_movimiento = Convert.ToDecimal(detalle.COSTO_MOVIMIENTO) * Convert.ToDecimal(detalle.CANTIDAD_MOVIMIENTO);
                            nuevo_cantidad_stock = Convert.ToDecimal(itemStock.CANTIDAD_ACTUAL) + Convert.ToDecimal(detalle.CANTIDAD_MOVIMIENTO);
                            nuevo_costo_promedio = Math.Round((costo_total_actual + costo_total_movimiento) / nuevo_cantidad_stock, 3);
                        }

                        //Si no tiene registro de stock
                        if (itemStock == null)
                        {
                            itemStock = new EPRTA_ARTICULO_BODEGA();
                            itemStock.ID_ITEM = detalle.ID_ITEM;
                            itemStock.ID_BODEGA = pmovimiento.ID_BODEGA;
                            itemStock.CANTIDAD_MAXIMA = 0;
                            itemStock.CANTIDAD_ACTUAL = detalle.CANTIDAD_MOVIMIENTO;
                            itemStock.CANTIDAD_INICIO = detalle.CANTIDAD_MOVIMIENTO;
                            itemStock.CANTIDAD_CRITICA = 0;
                            itemStock.CANTIDAD_MINIMA = 0;
                            itemStock.CANTIDAD_BAJA = 0;
                            itemStock.COSTO_PROMEDIO = nuevo_costo_promedio;
                            unitOfWork.ArticuloBodegaRepository.Insert(itemStock);
                        }
                        else   //Si ya tiene registro de stock, se procede a acutalizar valores
                        {
                            itemStock.COSTO_PROMEDIO = nuevo_costo_promedio;
                            itemStock.CANTIDAD_ACTUAL = nuevo_cantidad_stock;
                            //Actualizo la cantidad maxima

                            if (itemStock.CANTIDAD_MAXIMA < itemStock.CANTIDAD_ACTUAL)
                            {
                                itemStock.CANTIDAD_MAXIMA = itemStock.CANTIDAD_ACTUAL;
                            }
                            unitOfWork.ArticuloBodegaRepository.Update(itemStock);
                        }

                        //Actualizo la fecha del ultimo ingreso y el historial del costo
                        item.FECHA_ULTIMO_INGRESO = DateTime.Now;
                        item.COSTO_ANTERIOR = item.COSTO_ACTUAL;
                        item.COSTO_ACTUAL = nuevo_costo_promedio;
                        unitOfWork.ItemRepository.Update(item);
                    }

                }
            } 
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public FileResult ViewPDF(int? id)
        {
            Stream stream = null;
            var nombreArchivo = "";
            ReportDocument reportDocument = new ReportDocument();
            byte[] pdfByte = null;
            try
            {

                EPRTA_MOVIMIENTO solicitud = unitOfWork.MovimientoRepository.GetById(id);
                EntitiesProveduria db = new EntitiesProveduria();
                SqlConnectionStringBuilder builderVenta = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                String pathReport = "";
                if (solicitud.ID_TIPO_MOVIMIENTO == 2)
                {
                    SP_EGRESO_BODEGATableAdapter tableAdapter = new SP_EGRESO_BODEGATableAdapter();
                    object objetos = new object();
                    DataTable dataTable = tableAdapter.GetData(id,  out objetos);
                    pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Egreso_Bodega.rpt");
                    reportDocument.Load(pathReport);
                    reportDocument.SetDataSource(dataTable);
                }
                else if (solicitud.ID_TIPO_MOVIMIENTO == 4)
                {
                    SP_INGRESO_BODEGATableAdapter tableAdapter = new SP_INGRESO_BODEGATableAdapter();
                    object objetos = new object();
                    DataTable dataTable = tableAdapter.GetData(id, out objetos);
                    pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Ingreso_Bodega.rpt");
                    reportDocument.Load(pathReport);
                    reportDocument.SetDataSource(dataTable);
                }
                else if (solicitud.ID_TIPO_MOVIMIENTO == 10 || solicitud.ID_TIPO_MOVIMIENTO == 11)
                {
                    SP_AJUSTE_BODEGATableAdapter tableAdapter = new SP_AJUSTE_BODEGATableAdapter();
                    object objetos = new object();
                    DataTable dataTable = tableAdapter.GetData(id, out objetos);
                    pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Ajuste_Bodega.rpt");
                    reportDocument.Load(pathReport);
                    reportDocument.SetDataSource(dataTable);
                }

                reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

                stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                pdfByte = ReadFully(stream);
                nombreArchivo = "Movimiento_bodega";
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

        public Expression<Func<EPRTA_MOVIMIENTO, Boolean>> AppendExpression(Expression<Func<EPRTA_MOVIMIENTO, Boolean>> left, Expression<Func<EPRTA_MOVIMIENTO, Boolean>> right)
        {
            Expression<Func<EPRTA_MOVIMIENTO, Boolean>> result;
            if (left == null)
            {
                left = model => true;
            }
            result = ExpressionExtension<EPRTA_MOVIMIENTO>.AndAlso(left, right);
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}