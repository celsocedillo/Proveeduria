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
            string[] arrTiposOperacion = new string[] { "I", "E"};
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

        public ActionResult Movimiento(int pid)
        {
            //JArray jArray = new JArray();
            //JObject enviar = new JObject();
            EPRTA_MOVIMIENTO movimiento = null;
            try
            {
                if (pid> 0)
                {
                    var empleados = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                    movimiento = unitOfWork.MovimientoRepository.GetById(pid);
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
    }
}