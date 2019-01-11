using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using NLog;
using Proveduria.Repositories;
using Proveduria.Models.Enumadores;
using Newtonsoft.Json.Linq;
using Proveduria.Utils;


namespace Proveduria.Controllers
{
    [SessionTimeout]
    public class ConfiguracionController : Controller
    {

        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Configuracion
        public ActionResult Parametros()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetListaMedida()
        {
            JObject retorna = new JObject();
            try
            {
                var query = from d in unitOfWork.MedidaRepository.GetAll()
                            select new {
                                d.ID_MEDIDA,
                                d.NOMBRE,
                                ESTADO_REGISTRO = ((EnumEstadoRegistro)Convert.ToChar(d.ESTADO)).ToString()
                            };
                retorna = new JObject();
                retorna.Add("resultado", "success");
                retorna.Add("data", JArray.FromObject(query));
                retorna.Add("error", false);
            }
            catch (Exception ex)
            {
                retorna.Add("resultado", "error");
                retorna.Add("msg", "[ConfiguracionController.GetListaMedida]"+ ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetMedida(int pid)
        {
            JObject retorno = new JObject();
            try
            {
                EPRTA_MEDIDA medida = unitOfWork.MedidaRepository.GetById(pid);

                var tmp = new { medida.ID_MEDIDA, medida.NOMBRE, medida.ESTADO };
                retorno.Add("resultado", "success");
                retorno.Add("data", JObject.FromObject(tmp));

            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", "[ConfiguracionController.GetMedida]" + ex.Message);
                logger.Error(ex, ex.Message);

            }
            return Content(retorno.ToString(), "application/json");

        }


        [HttpPost]
        public ActionResult GrabarMedida(EPRTA_MEDIDA precord)
        {
            JObject retorno = new JObject();
            EPRTA_MEDIDA record;
            try
            {
                if (precord.ID_MEDIDA == 0)
                {
                    precord.ESTADO = "A";
                    unitOfWork.MedidaRepository.Insert(precord);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.MedidaRepository.GetById(precord.ID_MEDIDA);
                    record.NOMBRE = precord.NOMBRE;
                    record.ESTADO = precord.ESTADO;
                    unitOfWork.MedidaRepository.Update(record);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", "[ConfiguracionController.GrabarMedida]" + ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetListaGrupo()
        {
            JObject retorno = new JObject();
            try
            {
                var query = from d in unitOfWork.GrupoRepository.GetAll()
                            select new {
                                d.ID_GRUPO,
                                d.CODIGO,
                                d.NOMBRE,
                                d.CUENTA_CONTABLE,
                                d.ESTADO, 
                                ESTADO_REGISTRO = ((EnumEstadoRegistro)Convert.ToChar(d.ESTADO)).ToString()
                            };
                retorno.Add("resultado", "success");
                retorno.Add("data", JArray.FromObject(query));
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", "[ConfiguracionController.GetListaGrupo]" + ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetGrupo(int pid)
        {
            JObject retorno = new JObject();
            try
            {
                EPRTA_GRUPO grupo = unitOfWork.GrupoRepository.GetById(pid);
                var tmp = new { grupo.ID_GRUPO, grupo.NOMBRE, grupo.CODIGO, grupo.CUENTA_CONTABLE, grupo.ESTADO };
                retorno.Add("resultado", "success");
                retorno.Add("data", JObject.FromObject(tmp));
                
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", "[ConfiguracionController.GetGrupo]" + ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GrabarGrupo(EPRTA_GRUPO precord)
        {
            JObject retorno = new JObject();
            EPRTA_GRUPO record;
            try
            {
                if (precord.ID_GRUPO == 0)
                {
                    //Buscar si ya existe el codigo
                    record = unitOfWork.GrupoRepository.Where(x => x.CODIGO == precord.CODIGO).FirstOrDefault();

                    if (record != null)  
                    {
                        retorno.Add("resultado", "error");
                        retorno.Add("msg", "El codigo [" + precord.CODIGO + "] ya existe.");
                        logger.Info("Codigo de grupo repetido [CODIGO:"+ precord.CODIGO + "]");
                        return Content(retorno.ToString(), "application/json");
                    }

                    record = new EPRTA_GRUPO();
                    record.NOMBRE = precord.NOMBRE;
                    record.CODIGO = precord.CODIGO;
                    record.CUENTA_CONTABLE = precord.CUENTA_CONTABLE;
                    record.ESTADO = "A";
                    unitOfWork.GrupoRepository.Insert(record);
                }
                else
                {
                    record = unitOfWork.GrupoRepository.GetById(precord.ID_GRUPO);
                    record.NOMBRE = precord.NOMBRE;
                    record.CUENTA_CONTABLE = precord.CUENTA_CONTABLE;
                    record.ESTADO = precord.ESTADO;
                    unitOfWork.GrupoRepository.Update(record);
                }
                unitOfWork.Save();
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", "[ConfiguracionController.GrabarGrupo]" + ex.Message);
                logger.Error(ex, ex.Message);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetTipoMovimiento(int pid)
        {
            JObject retorno = new JObject();
            try
            {
                EPRTA_TIPO_MOVIMIENTO tipomov = unitOfWork.TipoMovimientoRepository.GetById(pid);
                var tmp = new { tipomov.ID_TIPO_MOVIMIENTO, tipomov.NOMBRE, tipomov.INGRESO_EGRESO, tipomov.ESTADO };
                retorno.Add("resultado", "success");
                retorno.Add("data", JObject.FromObject(tmp));
                //return Json(new { resultado = "success", data = new { tipomov.ID_TIPO_MOVIMIENTO, tipomov.NOMBRE, tipomov.INGRESO_EGRESO, tipomov.ESTADO }, mensaje = "" });

            }
            catch (Exception ex)
            {
                retorno.Add("resultado", "error");
                retorno.Add("msg", ex.Message);

                //return Json(new { resultado = "error", data = "", mensaje = " Error al consultar el tipo de movimiento, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GetListaTipoMovimiento()
        {
            JObject retorno = new JObject();
            try
            {
                string f = "I";
                EnumEstadoRegistro estado = (EnumEstadoRegistro)Convert.ToChar(f);
                string x = estado.ToString();
                
                var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                            select new {
                                d.ID_TIPO_MOVIMIENTO,
                                d.NOMBRE,
                                d.INGRESO_EGRESO,
                                d.ESTADO,
                                ESTADO_REGISTRO = ((EnumEstadoRegistro)Convert.ToChar(d.ESTADO)).ToString(),
                                TIPO_INGEGR = ((EnumIngresoEgreso)Convert.ToChar(d.INGRESO_EGRESO)).ToString(),
                            };
                retorno.Add("resultado", "success");
                retorno.Add("data", JArray.FromObject(query));
            }
            catch (Exception ex)
            {
                string msg = "[ConfiguracionController.GetListaTipoMovimiento]" + ex.Message;
                retorno.Add("resultado", "error");
                retorno.Add("msg", msg);
                logger.Error(ex, msg);
            }
            return Content(retorno.ToString(), "application/json");
        }

        [HttpPost]
        public ActionResult GrabarTipo(EPRTA_TIPO_MOVIMIENTO precord)
        {
            JObject retorno = new JObject();
            EPRTA_TIPO_MOVIMIENTO record;
            try
            {
                if (precord.ID_TIPO_MOVIMIENTO == 0)
                {
                    record = new EPRTA_TIPO_MOVIMIENTO();
                    record.NOMBRE = precord.NOMBRE;
                    record.INGRESO_EGRESO = precord.INGRESO_EGRESO;
                    record.ESTADO = "A";
                    unitOfWork.TipoMovimientoRepository.Insert(record);
                    unitOfWork.Save();
                }
                else
                {
                    record = unitOfWork.TipoMovimientoRepository.GetById(precord.ID_TIPO_MOVIMIENTO);
                    record.NOMBRE = precord.NOMBRE;
                    record.INGRESO_EGRESO = precord.INGRESO_EGRESO;
                    record.ESTADO = precord.ESTADO;
                    unitOfWork.TipoMovimientoRepository.Update(record);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
                logger.Info("Dato Grabado");
            }
            catch (Exception ex)
            {
                string msg = "[ConfiguracionController.GrabarTipo]" + ex.Message;
                retorno.Add("resultado", "error");
                retorno.Add("msg", msg);
                logger.Error(ex, msg);
            }
            return Content(retorno.ToString(), "application/json");
        }


    }
}