﻿using System;
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
using System.Net.Mime;

namespace Proveduria.Controllers
{
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
            return View();
        }

        [HttpPost]
        public ActionResult GetListaMovimiento()
        {
            string[] arrTiposOperacion = new string[] { "I", "E" };
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                           join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                           join ua in emp on p.USUARIO_APRUEBA equals ua.USUARIO into pa
                           from ua in pa.DefaultIfEmpty()
                           join ut in emp on p.USUARIO_AUTORIZA equals ut.USUARIO into pt
                           from ut in pt.DefaultIfEmpty()
                           where arrTiposOperacion.Contains(p.EPRTA_TIPO_MOVIMIENTO.INGRESO_EGRESO)
                           && p.ANIO == 2018
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
                                       p.ESTADO.Equals("D") ? "DESPACHADO" :
                                       p.ESTADO.Equals("E") ? "ANULADO" :
                                       p.ESTADO.Equals("A") ? "AUTORIZADO" :
                                       p.ESTADO.Equals("S") ? "SOLICITADO" : null),
                               p.USUARIO_SOLICITA,
                               p.USUARIO_AUTORIZA,
                               p.USUARIO_APRUEBA,
                               FECHA_AUTORIZACION = p.FECHA_AUTORIZACION.HasValue ? p.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null,
                               FECHA_APROBACION = p.FECHA_APROBACION.HasValue ? p.FECHA_APROBACION.Value.ToString("dd/MM/yyyy") : null,
                               EMPLEADO_SOLICITA = us.EMPLEADO,
                               EMPLEADO_APRUEBA = ua == null ? null : ua.EMPLEADO,
                               EMPLEADO_AUTORIZA = ut == null ? null : ut.EMPLEADO,
                               ACCION = "<a href='/Movimiento/Movimiento/" + p.ID_MOVIMIENTO + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                       "<i class='fa fa-search' aria-hidden='true'></i>" +
                                       "</a>"

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
        public ActionResult GetListaSolicitudAutorizada()
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                var dep = (from d in unitOfWork.DepartamentoRepository.GetAll() select d);
                var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                           join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                           join dp in dep on p.ID_DEPARTAMENTO_SOLICITA equals dp.CODIGO
                           where p.ESTADO == "A" && p.ID_TIPO_MOVIMIENTO == 12
                           select new
                           {
                               p.ID_MOVIMIENTO,
                               p.ANIO,
                               p.NUMERO_MOVIMIENTO,
                               MOVIMIENTO = p.EPRTA_TIPO_MOVIMIENTO.NOMBRE,
                               REFERENCIA = us.EMPLEADO + " - " + dp.DESCRIPCION,
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
                           select new
                           {
                               ID_MOVIMIENTO = 0,
                               p.ANIO,
                               NUMERO_MOVIMIENTO = p.NUMERO_ORDEN,
                               REFERENCIA = p.PROVEEDOR,
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
                           join st in stock on p.ID_ITEM equals st.ID_ITEM into pst from st in pst.DefaultIfEmpty()
                           select new
                           { CODIGO = p.EPRTA_ITEM.CODIGO,
                               ITEM = p.EPRTA_ITEM.DESCRIPCION,
                               CANTIDAD = p.CANTIDAD_PEDIDO,
                               MEDIDA = p.EPRTA_ITEM.EPRTA_MEDIDA.NOMBRE,
                               STOCK_ACTUAL = st.CANTIDAD_ACTUAL,
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
            List<EPRTA_TIPO_MOVIMIENTO> ltipo_movimiento = unitOfWork.TipoMovimientoRepository.GetAll().Where(p => arrTiposOperacion.Contains(p.INGRESO_EGRESO)).ToList();
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
                    ViewBag.departamento_solicitud = unitOfWork.DepartamentoRepository.GetById(movimiento.ID_DEPARTAMENTO_SOLICITA).DESCRIPCION;
                }
                else
                {
                    movimiento = new EPRTA_MOVIMIENTO();
                    movimiento.ESTADO = "S";
                    movimiento.ID_DEPARTAMENTO_SOLICITA = Convert.ToByte(Session["departamento_id"]);
                    ViewBag.departamento_solicitud = Session["departamento"];
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
                EPRTA_MOVIMIENTO movimiento = new EPRTA_MOVIMIENTO();
                movimiento.OBSERVACION = pmovimiento.OBSERVACION.ToUpper();
                movimiento.USUARIO_SOLICITA = Session["usuario"].ToString();
                movimiento.FECHA_SOLICITUD = DateTime.Now;
                movimiento.ESTADO = "D";
                movimiento.ANIO = (short)DateTime.Now.Year;
                movimiento.ID_TIPO_MOVIMIENTO = pmovimiento.ID_TIPO_MOVIMIENTO;
                movimiento.ID_BODEGA = 1;
                EPRTA_SECUENCIA secuencia = unitOfWork.SecuenciaRepository.GetAll().Where(p => p.ID_TIPO_MOVIMIENTO == pmovimiento.ID_TIPO_MOVIMIENTO && p.ANIO == movimiento.ANIO).FirstOrDefault();
                movimiento.NUMERO_MOVIMIENTO = (int)secuencia.SECUENCIA;

                foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                {
                    detalle.ESTADO = "A";
                    movimiento.EPRTA_MOVIMIENTO_DETALLE.Add(detalle);
                }
                secuencia.SECUENCIA++;
                unitOfWork.MovimientoRepository.Insert(movimiento);
                unitOfWork.SecuenciaRepository.Update(secuencia);
                EPRTA_MOVIMIENTO x = unitOfWork.MovimientoRepository.GetById(movimiento.ID_MOVIMIENTO);
                string ingreso_egreso = unitOfWork.TipoMovimientoRepository.GetById(movimiento.ID_TIPO_MOVIMIENTO).INGRESO_EGRESO;
                ActualizaStock(x, ingreso_egreso);
                unitOfWork.Save();
                movimiento = unitOfWork.MovimientoRepository.GetById(movimiento.ID_MOVIMIENTO);
                
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

        private void ActualizaStock(EPRTA_MOVIMIENTO pmovimiento, string pingreso_egreso)
        {
            foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
            {
                decimal costo_unitario;
                if (pingreso_egreso == "E")
                {
                    //Obtengo el registro del item
                    EPRTA_ITEM item = unitOfWork.ItemRepository.GetById(detalle.ID_ITEM);
                    //Actualizo la fecha del ultimo egreso
                    item.FECHA_ULTIMO_EGRESO = DateTime.Now;
                    //Obtengo el registro del stock del item
                    EPRTA_ARTICULO_BODEGA itemStock = unitOfWork.ArticuloBodegaRepository.GetAll().Where(p => p.ID_BODEGA == pmovimiento.ID_BODEGA && p.ID_ITEM == detalle.ID_ITEM).FirstOrDefault();
                    //Obtengo el costo unitario del item
                    costo_unitario = itemStock.VALOR.Value / itemStock.CANTIDAD_ACTUAL.Value;
                    //Actualizo el stock y el costo unitario
                    itemStock.CANTIDAD_ACTUAL -= detalle.CANTIDAD_PEDIDO;
                    itemStock.VALOR = itemStock.VALOR - (detalle.CANTIDAD_PEDIDO * costo_unitario);


                }




                //if (pingreso_egreso == "I")
                //{
                //    EPRTA_ARTICULO_BODEGA item_stock = unitOfWork.ArticuloBodegaRepository.GetById(detalle.ID_ITEM);
                //    if (pingreso_egreso == "I")
                //    {
                //        item_stock.CANTIDAD_ACTUAL += detalle.CANTIDAD_PEDIDO;
                //    }
                //    else if (pingreso_egreso == "E")
                //    {
                //        item_stock.CANTIDAD_ACTUAL -= detalle.CANTIDAD_PEDIDO;
                //    }
                //    unitOfWork.ArticuloBodegaRepository.Update(item_stock);
                //}
            }
        }
    }
}