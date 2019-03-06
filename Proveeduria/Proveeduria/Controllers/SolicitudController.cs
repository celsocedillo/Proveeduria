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
//using static Proveduria.Models.Enumadores.Enumeradores;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mime;
using Proveduria.Reports.DataSetReportsTableAdapters;
using Proveduria.Utils;

namespace Proveduria.Controllers
{
    //[SessionTimeout]
    public class SolicitudController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Solicitud
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaSolicitud(string pusuario)
        {
            //logger.Info("Intento usuario " + pusuario);
            if (pusuario != null)
            {
                CreaSesion(pusuario);
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetListaSolicitud()
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                if (Session["usuario"].ToString().Equals(Session["usuario_jefe"].ToString()))
                {
                    var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                    var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                               join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                               join ua in emp on p.USUARIO_APRUEBA equals ua.USUARIO into pa
                               from ua in pa.DefaultIfEmpty()
                               join ut in emp on p.USUARIO_AUTORIZA equals ut.USUARIO into pt
                               from ut in pt.DefaultIfEmpty()
                               where p.ID_DIRECCION_SOLICITA == Convert.ToByte(Session["id_direccion"].ToString()) && p.ID_TIPO_MOVIMIENTO == 12
                               select new
                               {
                                   p.ID_MOVIMIENTO,
                                   p.ANIO,
                                   p.NUMERO_MOVIMIENTO,
                                   FECHA_SOLICITUD = p.FECHA_SOLICITUD.HasValue ? p.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null,
                                   p.OBSERVACION,
                                   p.ID_TIPO_MOVIMIENTO,
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
                                   ACCION = "<a href='/Solicitud/Solicitud/" + p.ID_MOVIMIENTO + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                            "<i class='fa fa-search' aria-hidden='true'></i>" +
                                            "</a>"

                               }).ToList();
                    enviar.Add("resultado", "success");
                    enviar.Add("data", JArray.FromObject(tmp));
                }
                else
                {
                    var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                    var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                               join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                               join ua in emp on p.USUARIO_APRUEBA equals ua.USUARIO into pa
                               from ua in pa.DefaultIfEmpty()
                               join ut in emp on p.USUARIO_AUTORIZA equals ut.USUARIO into pt
                               from ut in pt.DefaultIfEmpty()
                               where p.USUARIO_SOLICITA == Session["usuario"].ToString() && p.ID_TIPO_MOVIMIENTO == 12
                               select new
                               {
                                   p.ID_MOVIMIENTO,
                                   p.ANIO,
                                   p.NUMERO_MOVIMIENTO,
                                   FECHA_SOLICITUD = p.FECHA_SOLICITUD.HasValue ? p.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null,
                                   p.OBSERVACION,
                                   p.ID_TIPO_MOVIMIENTO,
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
                                   ACCION = "<a href='/Solicitud/Solicitud/" + p.ID_MOVIMIENTO + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                            "<i class='fa fa-search' aria-hidden='true'></i>" +
                                            "</a>"

                               }).ToList();
                    enviar.Add("resultado", "success");
                    enviar.Add("data", JArray.FromObject(tmp));
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);
            }
            return Content(enviar.ToString(), "application/json");
        }

        [HttpGet]
        public ActionResult Solicitud(int id)
        {
            //JArray jArray = new JArray();
            //JObject enviar = new JObject();
            EPRTA_MOVIMIENTO movimiento = null;
            try
            {

                if (id>0)
                {
                    var empleados = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                    movimiento = unitOfWork.MovimientoRepository.GetById(id);
                    ViewBag.usuario_solicita = (string)(from p in empleados where p.USUARIO == movimiento.USUARIO_SOLICITA select p.EMPLEADO).FirstOrDefault();
                    ViewBag.usuario_aprueba = movimiento.USUARIO_APRUEBA != null ? (from p in empleados where p.USUARIO == movimiento.USUARIO_APRUEBA select p.EMPLEADO).FirstOrDefault() : null;
                    ViewBag.usuario_autoriza = movimiento.USUARIO_AUTORIZA != null ?  (from p in empleados where p.USUARIO == movimiento.USUARIO_AUTORIZA select p.EMPLEADO).FirstOrDefault() : null;
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
                //enviar.Add("resultado", "error");
                //enviar.Add("msg", ex.Message);
            }
            return View(movimiento);
        }

        [HttpPost]
        public ActionResult Grabar(EPRTA_MOVIMIENTO pmovimiento, bool pregistro_nuevo)
        {
            string msgErr = "";
            JObject retorno = new JObject();
            if (pregistro_nuevo)
            {
                try
                {
                    EPRTA_MOVIMIENTO movimiento = new EPRTA_MOVIMIENTO();
                    movimiento.OBSERVACION = pmovimiento.OBSERVACION.ToUpper();
                    movimiento.USUARIO_SOLICITA = Session["usuario"].ToString();
                    movimiento.FECHA_SOLICITUD = DateTime.Now;
                    movimiento.ESTADO = "S";
                    movimiento.ID_DIRECCION_SOLICITA = Convert.ToByte(Session["id_direccion"].ToString());
                    movimiento.ANIO = (short)DateTime.Now.Year;
                    movimiento.ID_TIPO_MOVIMIENTO = 12;
                    movimiento.ID_BODEGA = 1;

                    msgErr = "[Intentando obtener secuencia] ";
                    EPRTA_SECUENCIA secuencia = unitOfWork.SecuenciaRepository.GetAll().Where(p => p.ID_TIPO_MOVIMIENTO == 12 && p.ANIO == movimiento.ANIO).FirstOrDefault();
                    movimiento.NUMERO_MOVIMIENTO = (int)secuencia.SECUENCIA;
                    msgErr = "";
                    foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                    {
                        detalle.ESTADO = "A";
                        movimiento.EPRTA_MOVIMIENTO_DETALLE.Add(detalle);
                    }
                    secuencia.SECUENCIA++;
                    msgErr = "[Intentando grabar en Base de datos] ";
                    unitOfWork.MovimientoRepository.Insert(movimiento);
                    unitOfWork.SecuenciaRepository.Update(secuencia);
                    unitOfWork.Save();
                    retorno.Add("resultado", "success");
                    retorno.Add("data", null);
                }
                catch(Exception ex)
                {
                    retorno.Add("resultado", "error");
                    retorno.Add("data", null);
                    retorno.Add("msg",  msgErr + ex.ToString());
                    logger.Error(ex, ex.Message);
                }

            }
            else //Registro a modificar
            {
                try
                {
                    EPRTA_MOVIMIENTO movimiento = unitOfWork.MovimientoRepository.GetById(pmovimiento.ID_MOVIMIENTO);
                    movimiento.OBSERVACION = pmovimiento.OBSERVACION;
                    if (pmovimiento.ESTADO == "A")
                    {
                        movimiento.ESTADO = "A";
                        movimiento.USUARIO_AUTORIZA = Session["usuario"].ToString();
                        movimiento.FECHA_AUTORIZACION = DateTime.Now;
                    }else if(pmovimiento.ESTADO == "S")
                    {
                        movimiento.ESTADO = "S";
                        movimiento.USUARIO_AUTORIZA = null;
                        movimiento.FECHA_AUTORIZACION = null;
                    }


                    foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                    {
                        if (detalle.ID_DETALLE == 0) //Si no tiene un id no existe en la base y se tiene que registrar
                        {
                            detalle.ID_MOVIMIENTO = movimiento.ID_MOVIMIENTO;
                            detalle.EPRTA_MOVIMIENTO = movimiento;
                            detalle.ESTADO = "A";
                            movimiento.EPRTA_MOVIMIENTO_DETALLE.Add(detalle);
                        }
                        else
                        {
                            if (detalle.ESTADO == "E" && detalle.ID_DETALLE > 0)  //Si tiene un id se busca en la base para eliminarlo
                            {
                                //EPRTA_MOVIMIENTO_DETALLE registro_eliminar = unitOfWork.MovimientoDetalleRepository.GetById(detalle.ID_DETALLE);
                                unitOfWork.MovimientoDetalleRepository.Delete(detalle.ID_DETALLE);
                            }
                            else
                            {
                                movimiento.EPRTA_MOVIMIENTO_DETALLE.Where(p => p.ID_DETALLE == detalle.ID_DETALLE).FirstOrDefault().CANTIDAD_MOVIMIENTO = detalle.CANTIDAD_MOVIMIENTO;
                            }
                        }
                    }
                    unitOfWork.MovimientoRepository.Update(movimiento);
                    unitOfWork.Save();
                    retorno.Add("resultado", "success");
                    retorno.Add("data", null);
                    retorno.Add("mensaje", "");
                }
                catch(Exception ex)
                {
                    retorno.Add("resultado", "error");
                    retorno.Add("data", null);
                    retorno.Add("mensaje", ex.ToString());
                    logger.Error(ex, ex.Message);
                }
            }
            return Content(retorno.ToString(), "application/json");

        }

        [HttpGet]
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
                SP_REQUISICION_BODEGATableAdapter tableAdapter = new SP_REQUISICION_BODEGATableAdapter();
                object objetos = new object();
                DataTable dataTable = tableAdapter.GetData(id, out objetos);
                String pathReport = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports\\Cr_Requisicion_Bodega.rpt");

                reportDocument.Load(pathReport);
                reportDocument.SetDataSource(dataTable);


                reportDocument.SetDatabaseLogon(builderVenta.UserID, builderVenta.Password);

                stream = reportDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                pdfByte = ReadFully(stream);
                nombreArchivo = "REQUISICION DE BODEGA ";
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

        private void CreaSesion(string pusuario)
        {
            UsuarioController usuarioController = new UsuarioController();
            ViewBag.usuario = UsuarioController.usuario.usuario;
            ViewBag.nombre = UsuarioController.usuario.nombre;
            VW_EMPLEADO empleado = unitOfWork.EmpleadoRepository.GetAll().Where(p => p.USUARIO == pusuario.ToUpper()).FirstOrDefault();
            if (empleado != null)
            {
                Session["usuario"] = empleado.USUARIO;
                Session["nombre"] = empleado.EMPLEADO;
                Session["id_direccion"] = empleado.DIRECCION_ID;
                Session["direccion"] = empleado.DIRECCION;
                Session["usuario_jefe"] = empleado.USUARIO_JEFE_DEPARTAMENTO;
                Session["bodega_id"] = 1;
                Session["bodega"] = "BODEGA PROVEDURIA";
                logger.Info(Session["usuario"] + " : Inicio de Sesion  ");
            }
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}