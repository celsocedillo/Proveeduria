using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using System.Net;
using System.Data.Entity;
using Newtonsoft.Json.Linq;
using NLog;
using Newtonsoft.Json;

namespace Proveduria.Controllers
{
    public class GrupoController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Grupo
        public ActionResult ListaGrupo()
        {
            List<EPRTA_GRUPO> lgrupo = unitOfWork.GrupoRepository.GetAll().ToList();
            return View(lgrupo);
        }

        [HttpPost]
        public JsonResult GetGrupo(int pid)
        {
            try
            {
                EPRTA_GRUPO jGrupo = unitOfWork.GrupoRepository.GetById(pid);
                return Json(new { resultado = "success", data = jGrupo, mensaje = "" });

            }
            catch (Exception ex)
            {
                return Json(new { resultado = "error", data = "", mensaje = " Error al consultar el Grupo, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }
        }

        [HttpPost]
        public ActionResult GetListaGrupo()
        {
            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                //var query = from d in unitOfWork.TipoMovimientoRepository.GetAll()
                //            select d;
                //foreach (EPRTA_TIPO_MOVIMIENTO item in query)
                //{
                //    JObject jsonObject = new JObject
                //    {
                //        { "ID_TIPO_MOVIMIENTO", item.ID_TIPO_MOVIMIENTO },
                //        { "NOMBRE", item.NOMBRE }
                //    };
                //    jArray.Add(jsonObject);
                //}
                //total = new JObject();
                //total.Add("items", jArray);
                //total.Add("error", false);

                var query = from d in unitOfWork.GrupoRepository.GetAll()
                            select new { d.ID_GRUPO, d.NOMBRE, d.ESTADO,d.CUENTA_CONTABLE,d.CODIGO_GRUPO };
                total = new JObject();
                total.Add("data", JsonConvert.SerializeObject(query));
                total.Add("error", false);

            }
            catch (Exception ex)
            {
                total.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");
        }


    }
}