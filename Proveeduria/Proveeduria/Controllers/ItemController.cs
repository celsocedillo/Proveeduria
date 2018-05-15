using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proveduria.Models;
using Proveduria.Repositories;
using Newtonsoft.Json.Linq;
using NLog;
using Newtonsoft.Json;


namespace Proveduria.Controllers
{
    public class ItemController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Item
        public ActionResult ListaItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetListaItem()
        {
            JObject retorna = new JObject();
            try
            {
                var query = from d in unitOfWork.ItemRepository.GetAll()
                            select new { d.ID_ITEM, d.CODIGO, d.DESCRIPCION, GRUPO = d.EPRTA_GRUPO.NOMBRE, MEDIDA = d.EPRTA_MEDIDA.NOMBRE };
                retorna = new JObject();
                retorna.Add("data", JsonConvert.SerializeObject(query));
                retorna.Add("error", false);

            }
            catch (Exception ex)
            {
                retorna.Add("error", true);
                logger.Error(ex, ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }

        public ActionResult Item(int id)
        {

            EPRTA_ITEM registro;
            try
            {
                if (id >0)
                {
                    registro = unitOfWork.ItemRepository.GetById(id);
                }
                else
                {
                    registro = new EPRTA_ITEM();
                    registro.ESTADO = "N";
                }

                List<SelectListItem> lista  = new List<SelectListItem>();
                foreach (EPRTA_GRUPO reg in (from p in unitOfWork.GrupoRepository.GetAll() where p.ESTADO == "A" select p))
                {
                    SelectListItem lin = new SelectListItem { Text = reg.NOMBRE, Value = reg.ID_GRUPO.ToString() };
                    lista.Add(lin);
                }
                ViewBag.lstGrupo = lista;

                lista = new List<SelectListItem>();
                foreach (EPRTA_MEDIDA reg in (from p in unitOfWork.MedidaRepository.GetAll() where p.ESTADO == "A" select p))
                {
                    SelectListItem lin = new SelectListItem { Text = reg.NOMBRE, Value = reg.ID_MEDIDA.ToString() };
                    lista.Add(lin);
                }
                ViewBag.lstMedida = lista;
            }
            catch (Exception ex)
            {
                return Json(new { resultado = "error", data = "", mensaje = " Error al consultar las listas, favor revisar las conecciones de base de datos => [" + ex + "]" });
            }

            return View(registro);
        }

        [HttpPost]
        public ActionResult Grabar(EPRTA_ITEM precord)
        {
            JObject retorno = new JObject();
            //EPRTA_ITEM record;
            try
            {
                if (precord.ID_ITEM == 0)
                {
                    //record = new EPRTA_TIPO_MOVIMIENTO();
                    //record.NOMBRE = precord.NOMBRE;
                    //record.INGRESO_EGRESO = precord.INGRESO_EGRESO;
                    //record.ESTADO = "A";
                    //unitOfWork.TipoMovimientoRepository.Insert(record);
                    //unitOfWork.Save();
                }
                else
                {
                    unitOfWork.ItemRepository.Update(precord);
                    unitOfWork.Save();
                }
                retorno.Add("resultado", "success");
                retorno.Add("data", null);
                retorno.Add("mensaje", "");
                logger.Info("Dato Grabado");
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

    }
}