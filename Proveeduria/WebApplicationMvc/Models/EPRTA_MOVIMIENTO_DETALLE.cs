//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplicationMvc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EPRTA_MOVIMIENTO_DETALLE
    {
        public long ID_DETALLE { get; set; }
        public int ID_MOVIMIENTO { get; set; }
        public int ID_ITEM { get; set; }
        public Nullable<int> CANTIDAD_PEDIDO { get; set; }
        public string ESTADO { get; set; }
        public string IVA { get; set; }
        public string OBSERVACION { get; set; }
        public string DESCRIPCION_ADICIONAL { get; set; }
    
        public virtual EPRTA_ITEM EPRTA_ITEM { get; set; }
        public virtual EPRTA_MOVIMIENTO EPRTA_MOVIMIENTO { get; set; }
    }
}
