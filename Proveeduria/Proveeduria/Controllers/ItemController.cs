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
        public static Usuario usuario;
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
                IEnumerable < EPRTA_ITEM > filtroItems;
                if (!String.IsNullOrEmpty(searchValue))
                {
                    filtroItems = items.Where(w => w.CODIGO.ToLower().Contains(searchValue.ToLower()) || w.DESCRIPCION.Contains(searchValue));
                }
                else
                {
                    filtroItems = items;

                }
                var sortColumnIndex = Convert.ToInt32(Request.Form.Get("order[0][column]"));
                Func<EPRTA_ITEM, string> orderingFunction = (c => sortColumnIndex == 0 ? c.CODIGO.ToString() : sortColumnIndex == 1 ? c.DESCRIPCION : c.EPRTA_MEDIDA.NOMBRE);
                var sortDirection = Request.Form.Get("order[0][dir]");
                if (sortDirection == "asc")
                {
                    filtroItems = filtroItems.OrderBy(orderingFunction);
                }
                else
                {
                    filtroItems = filtroItems.OrderByDescending(orderingFunction);
                }
                IEnumerable<EPRTA_ITEM> dataShow = filtroItems.Skip(int.Parse(Request.Form.Get("start"))).Take(int.Parse(Request.Form.Get("length")));
                //foreach (EPRTA_ITEM item in dataShow)
                //{
                //    var accionModificar = "<a href='/Item/Item/" + item.ID_ITEM + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                //                            "<i class='fa fa-pencil' aria-hidden='true'></i>" +
                //                          "</a>";
                //    JObject jsonObject = new JObject
                //    {
                //        { "ID_ITEM", item.ID_ITEM},
                //        { "CODIGO", item.CODIGO },
                //        { "DESCRIPCION", item.DESCRIPCION},
                //        { "MEDIDA", item.EPRTA_MEDIDA.NOMBRE},
                //        { "GRUPO", item.EPRTA_GRUPO.NOMBRE },
                //        { "ACCION", accionModificar}
                //    };
                //    jArray.Add(jsonObject);
                    
                //}
                var enviar = from p in dataShow
                             select new
                             { p.ID_ITEM,
                               p.CODIGO,
                               p.DESCRIPCION,
                               GRUPO = p.EPRTA_GRUPO.NOMBRE,
                               MEDIDA = p.EPRTA_MEDIDA.NOMBRE,
                               ACCION = "<a href='/Item/Item/" + p.ID_ITEM + "' class='text-inverse' data-toggle='tooltip' title='Modificar'>" +
                                        "<i class='fa fa-pencil' aria-hidden='true'></i>" +
                                        "</a>"
                             };
                total.Add("draw", Request.Form.Get("draw"));
                total.Add("recordsTotal", items.Count());
                //total.Add("recordsFiltered", filtroItems.Count());
                total.Add("recordsFiltered", filtroItems.Count());
                //total.Add("data", jArray);
                total.Add("data", JArray.FromObject(enviar));
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            return Content(total.ToString(), "application/json");
            //return Json(new { datos = items, });
        }


        [HttpPost]
        public ActionResult SearchItemsFiltro(string filtro, string pagina)
        {
           JObject retorna = new JObject();
            try
            {
                if (filtro != null)
                {
                    var stock = unitOfWork.ArticuloBodegaRepository.GetAll();
                    var list = (from d in unitOfWork.ItemRepository.GetAll()
                                join st in stock on d.ID_ITEM equals st.ID_ITEM into pst from st in pst.DefaultIfEmpty()
                                where (d.DESCRIPCION.ToLower().Contains(filtro.ToLower()) || d.CODIGO.ToLower().Contains(filtro.ToLower()))
                                select new
                                {
                                    id = d.ID_ITEM,
                                    d.CODIGO,
                                    d.DESCRIPCION,
                                    d.ID_MEDIDA,
                                    MEDIDA = d.EPRTA_MEDIDA.NOMBRE,
                                    STOCK_ACTUAL = st.CANTIDAD_ACTUAL
                                }
                       ).Take(int.Parse(pagina)).ToList();
                    retorna.Add("items", JArray.FromObject(list));
                }
            }
            catch (Exception ex)
            {
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
        //[ValidateAntiForgeryToken]
        public ActionResult Grabar(EPRTA_ITEM precord)
        {
            JObject retorno = new JObject();
            //EPRTA_ITEM record;
            try
            {
                if (precord.ESTADO.Equals("N"))
                {
                    if (DisponibilidadCodigo(precord.CODIGO))
                    {
                        precord.ESTADO = "A";
                        unitOfWork.ItemRepository.Insert(precord);
                        unitOfWork.Save();
                        retorno.Add("resultado", "success");
                        retorno.Add("data", null);
                        retorno.Add("mensaje", "");
                    }
                    else
                    {
                        retorno.Add("resultado", "warning");
                        retorno.Add("data", null);
                        retorno.Add("mensaje", "Codigo de item ya se encuentra repetido");
                    }
                }
                else
                {
                    unitOfWork.ItemRepository.Update(precord);
                    unitOfWork.Save();
                    retorno.Add("resultado", "success");
                    retorno.Add("data", null);
                    retorno.Add("mensaje", "");

                }
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

        [HttpPost]
        public ActionResult GetStockBodega(int pid)
        {
            JObject retorna = new JObject();
            JArray jArray = new JArray();
            try
            {
                EPRTA_ITEM item = unitOfWork.ItemRepository.GetById(pid);
                if(item != null)
                {
                    if(item.EPRTA_ARTICULO_BODEGA != null)
                    {
                        if (item.EPRTA_ARTICULO_BODEGA.Count > 0)
                        {
                            //foreach (EPRTA_ARTICULO_BODEGA arti in item.EPRTA_ARTICULO_BODEGA)
                            //{
                            //    var stock = new
                            //    {
                            //        BODEGA = arti.EPRTA_BODEGA.NOMBRE,
                            //        arti.CANTIDAD_MAXIMA,
                            //        arti.CANTIDAD_MINIMA,
                            //        arti.CANTIDAD_ACTUAL,
                            //        arti.CANTIDAD_BAJA,
                            //        arti.CANTIDAD_CRITICA
                            //    };
                            //    //jArray.Add(JObject.FromObject(stock));
                            //    jArray.Add(stock);
                            //}

                            //var tmp2 = new { tmp.CODIGO, tmp.DESCRIPCION, tmp.EPRTA_ARTICULO_BODEGA };
                            //var tmp = unitOfWork.ArticuloBodegaRepository.GetAll().Where(c => c.ID_ITEM == pid);

                            var stock = from arti in item.EPRTA_ARTICULO_BODEGA
                                        select new
                                        {
                                            BODEGA = arti.EPRTA_BODEGA.NOMBRE,
                                            arti.CANTIDAD_MAXIMA,
                                            arti.CANTIDAD_MINIMA,
                                            arti.CANTIDAD_ACTUAL,
                                            arti.CANTIDAD_BAJA,
                                            arti.CANTIDAD_CRITICA
                                        };
                            retorna.Add("data", JArray.FromObject(stock));
                            retorna.Add("resultado", "success");
               
                        }
                    }
                }


          
                

            }catch(Exception ex)
            {
                retorna.Add("resultado", "error");
                retorna.Add("msg", ex.Message);
            }
            return Content(retorna.ToString(), "application/json");
        }

        private bool DisponibilidadCodigo(string pcodigo)
        {
            EPRTA_ITEM item = (from p in unitOfWork.ItemRepository.GetAll() where p.CODIGO == pcodigo select p).FirstOrDefault();
            if (item == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }


    }
}