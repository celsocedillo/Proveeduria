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
            JArray jArray = new JArray();
            JObject total = new JObject();
            try
            {
                var searchValue = Request.Form.Get("search[value]");
                var items = unitOfWork.ItemRepository.GetAll();
                IEnumerable<EPRTA_ITEM> filteredFacturas;
                if (!String.IsNullOrEmpty(searchValue))
                {
                    filteredFacturas = items.Where(w => w.CODIGO.ToLower().Contains(searchValue.ToLower()) || w.DESCRIPCION.Contains(searchValue));
                }
                else
                {
                    filteredFacturas = items;
                }
                var sortColumnIndex = Convert.ToInt32(Request.Form.Get("order[0][column]"));
                Func<EPRTA_ITEM, string> orderingFunction = (c => sortColumnIndex == 0 ? c.CODIGO.ToString() : sortColumnIndex == 1 ? c.DESCRIPCION : c.EPRTA_MEDIDA.NOMBRE);
                var sortDirection = Request.Form.Get("order[0][dir]");
                if (sortDirection == "asc")
                {
                    filteredFacturas = filteredFacturas.OrderBy(orderingFunction);
                }
                else
                {
                    filteredFacturas = filteredFacturas.OrderByDescending(orderingFunction);
                }
                IEnumerable<EPRTA_ITEM> dataShow = filteredFacturas.Skip(int.Parse(Request.Form.Get("start"))).Take(int.Parse(Request.Form.Get("length")));
                foreach (EPRTA_ITEM item in dataShow)
                {
                    var accionModificar = "<a href='/Item/Item/" + item.ID_ITEM + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                            "<i class='fa fa-pencil' aria-hidden='true'></i>" +
                                          "</a>";
                    var enviar = new { item.ID_ITEM, item.CODIGO, item.DESCRIPCION, MEDIDA = item.EPRTA_MEDIDA.NOMBRE, GRUPO = item.EPRTA_GRUPO.NOMBRE };


                    JObject jsonObject = new JObject
                    {
                        { "ID_ITEM", item.ID_ITEM},
                        { "CODIGO", item.CODIGO },
                        { "DESCRIPCION", item.DESCRIPCION},
                        { "MEDIDA", item.EPRTA_MEDIDA.NOMBRE},
                        { "GRUPO", item.EPRTA_GRUPO.NOMBRE },
                        { "ACCION", accionModificar}
                    };
                    jArray.Add(jsonObject);
                }

                total.Add("draw", Request.Form.Get("draw"));
                total.Add("recordsTotal", items.Count());
                total.Add("recordsFiltered", filteredFacturas.Count());
                total.Add("data", jArray);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");


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