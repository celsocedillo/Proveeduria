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
using static Proveduria.Models.Enumadores.Enumeradores;

namespace Proveduria.Controllers
{
    public class SolicitudController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Solicitud
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListaSolicitud()
        {
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
                               where p.ID_DEPARTAMENTO_SOLICITA == Convert.ToByte(Session["id_departamento"].ToString()) && p.ID_TIPO_MOVIMIENTO == 2
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
                               where p.USUARIO_SOLICITA == Session["usuario"].ToString() && p.ID_TIPO_MOVIMIENTO == 2
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
                    ViewBag.departamento_solicitud = unitOfWork.DepartamentoRepository.GetById(movimiento.ID_DEPARTAMENTO_SOLICITA).DESCRIPCION;
                }
                else
                {
                    movimiento = new EPRTA_MOVIMIENTO();
                    movimiento.ESTADO = "S";
                    movimiento.ID_DEPARTAMENTO_SOLICITA = Convert.ToByte(Session["departamento_id"]);
                    ViewBag.departamento_solicitud = Session["departamento"];
                }


                //var empleados = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                //var tmp = new
                //{
                //    movimiento.ID_MOVIMIENTO,
                //    movimiento.ANIO,
                //    movimiento.NUMERO_MOVIMIENTO,
                //    FECHA_SOLICITUD = movimiento.FECHA_SOLICITUD.HasValue ? movimiento.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null,
                //    movimiento.OBSERVACION,
                //    movimiento.ID_TIPO_MOVIMIENTO,
                //    movimiento.ESTADO,
                //    NOMBREESTADO = (
                //                        movimiento.ESTADO.Equals("D") ? "DESPACHADO" :
                //                        movimiento.ESTADO.Equals("E") ? "ANULADO" :
                //                        movimiento.ESTADO.Equals("A") ? "AUTORIZADO" :
                //                        movimiento.ESTADO.Equals("S") ? "SOLICITADO" : null
                //                   ),
                //    movimiento.USUARIO_AUTORIZA,
                //    movimiento.USUARIO_APRUEBA,
                //    FECHA_AUTORIZACION = movimiento.FECHA_AUTORIZACION.HasValue ? movimiento.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null,
                //    FECHA_APROBACION = movimiento.FECHA_APROBACION.HasValue ? movimiento.FECHA_APROBACION.Value.ToString("dd/MM/yyyy") : null,
                //    EMPLEADO_SOLICITA = (from us in empleados where us.USUARIO == movimiento.USUARIO_SOLICITA select us.EMPLEADO),
                //    EMPLEADO_APRUEBA = (from ua in empleados where ua.USUARIO == movimiento.USUARIO_APRUEBA select ua.EMPLEADO),
                //    EMPLEADO_AUTORIZA = (from ut in empleados where ut.USUARIO == movimiento.USUARIO_AUTORIZA select ut.EMPLEADO)
                //};
                //enviar.Add("resultado", "success");
                //enviar.Add("data", JObject.FromObject(tmp));
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
                    movimiento.ID_DEPARTAMENTO_SOLICITA = Convert.ToByte(Session["id_departamento"].ToString());
                    movimiento.ANIO = (short)DateTime.Now.Year;
                    movimiento.ID_TIPO_MOVIMIENTO = 2;
                    movimiento.ID_BODEGA = 1;

                    EPRTA_SECUENCIA secuencia = unitOfWork.SecuenciaRepository.GetAll().Where(p => p.ID_TIPO_MOVIMIENTO == 2 && p.ANIO == movimiento.ANIO).FirstOrDefault();
                    movimiento.NUMERO_MOVIMIENTO = (int)secuencia.SECUENCIA;

                    foreach (EPRTA_MOVIMIENTO_DETALLE detalle in pmovimiento.EPRTA_MOVIMIENTO_DETALLE)
                    {
                        detalle.ESTADO = "A";
                        movimiento.EPRTA_MOVIMIENTO_DETALLE.Add(detalle);
                    }
                    secuencia.SECUENCIA++;
                    unitOfWork.MovimientoRepository.Insert(movimiento);
                    unitOfWork.SecuenciaRepository.Update(secuencia);
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
                                movimiento.EPRTA_MOVIMIENTO_DETALLE.Where(p => p.ID_DETALLE == detalle.ID_DETALLE).FirstOrDefault().CANTIDAD_PEDIDO = detalle.CANTIDAD_PEDIDO;
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

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}