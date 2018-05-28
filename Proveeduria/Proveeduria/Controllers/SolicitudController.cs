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
                var emp = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                var tmp = (from p in unitOfWork.MovimientoRepository.GetAll()
                           join us in emp on p.USUARIO_SOLICITA equals us.USUARIO
                           join ua in emp on p.USUARIO_APRUEBA equals ua.USUARIO
                           join ut in emp on p.USUARIO_AUTORIZA equals ut.USUARIO
                           where p.USUARIO_SOLICITA == Session["usuario"].ToString() && p.ID_TIPO_MOVIMIENTO == 2
                           select new {
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
                                        p.ESTADO.Equals("S") ? "SOLICITADO":null ),
                               p.USUARIO_AUTORIZA,
                               p.USUARIO_APRUEBA,
                               FECHA_AUTORIZACION = p.FECHA_AUTORIZACION.HasValue ? p.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null,
                               FECHA_APROBACION = p.FECHA_APROBACION.HasValue ? p.FECHA_APROBACION.Value.ToString("dd/MM/yyyy") : null,
                               EMPLEADO_SOLICITA = us.EMPLEADO,
                               EMPLEADO_APRUEBA = ua.EMPLEADO,
                               EMPLEADO_AUTORIZA = ut.EMPLEADO
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
        public ActionResult GetSolicitud(int psolicitud_id)
        {
            JArray jArray = new JArray();
            JObject enviar = new JObject();
            try
            {
                var movimiento = unitOfWork.MovimientoRepository.GetById(psolicitud_id);
                var empleados = (from e in unitOfWork.EmpleadoRepository.GetAll() select e);
                var tmp = new
                {
                    movimiento.ID_MOVIMIENTO,
                    movimiento.ANIO,
                    movimiento.NUMERO_MOVIMIENTO,
                    FECHA_SOLICITUD = movimiento.FECHA_SOLICITUD.HasValue ? movimiento.FECHA_SOLICITUD.Value.ToString("dd/MM/yyyy") : null,
                    movimiento.OBSERVACION,
                    movimiento.ID_TIPO_MOVIMIENTO,
                    movimiento.ESTADO,
                    NOMBREESTADO = (
                                        movimiento.ESTADO.Equals("D") ? "DESPACHADO" :
                                        movimiento.ESTADO.Equals("E") ? "ANULADO" :
                                        movimiento.ESTADO.Equals("A") ? "AUTORIZADO" :
                                        movimiento.ESTADO.Equals("S") ? "SOLICITADO" : null),
                    movimiento.USUARIO_AUTORIZA,
                    movimiento.USUARIO_APRUEBA,
                    FECHA_AUTORIZACION = movimiento.FECHA_AUTORIZACION.HasValue ? movimiento.FECHA_AUTORIZACION.Value.ToString("dd/MM/yyyy") : null,
                    FECHA_APROBACION = movimiento.FECHA_APROBACION.HasValue ? movimiento.FECHA_APROBACION.Value.ToString("dd/MM/yyyy") : null,
                    EMPLEADO_SOLICITA = (from us in empleados where us.USUARIO == movimiento.USUARIO_SOLICITA select us.EMPLEADO),
                    EMPLEADO_APRUEBA = (from ua in empleados where ua.USUARIO == movimiento.USUARIO_APRUEBA select ua.EMPLEADO),
                    EMPLEADO_AUTORIZA = (from ut in empleados where ut.USUARIO == movimiento.USUARIO_AUTORIZA select ut.EMPLEADO)
                };
                enviar.Add("resultado", "success");
                enviar.Add("data", JObject.FromObject(tmp));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                enviar.Add("resultado", "error");
                enviar.Add("msg", ex.Message);
            }
            return Content(enviar.ToString(), "application/json");

        }
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

    }
}