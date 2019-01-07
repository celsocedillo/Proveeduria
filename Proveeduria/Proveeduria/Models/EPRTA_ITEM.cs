//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proveduria.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EPRTA_ITEM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EPRTA_ITEM()
        {
            this.EPRTA_ARTICULO_BODEGA = new HashSet<EPRTA_ARTICULO_BODEGA>();
            this.EPRTA_MOVIMIENTO_DETALLE = new HashSet<EPRTA_MOVIMIENTO_DETALLE>();
            this.VW_ORDEN_COMPRA_DETALLE = new HashSet<VW_ORDEN_COMPRA_DETALLE>();
            this.EPRTA_CIERRE_INVENTARIO_DET = new HashSet<EPRTA_CIERRE_INVENTARIO_DET>();
        }
    
        public int ID_ITEM { get; set; }
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public byte ID_MEDIDA { get; set; }
        public Nullable<System.DateTime> FECHA_ULTIMO_INGRESO { get; set; }
        public Nullable<System.DateTime> FECHA_ULTIMO_EGRESO { get; set; }
        public Nullable<decimal> COSTO_ANTERIOR { get; set; }
        public Nullable<decimal> COSTO_ACTUAL { get; set; }
        public string ESTADO { get; set; }
        public string OBSERVACION { get; set; }
        public byte ID_GRUPO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_ARTICULO_BODEGA> EPRTA_ARTICULO_BODEGA { get; set; }
        public virtual EPRTA_GRUPO EPRTA_GRUPO { get; set; }
        public virtual EPRTA_MEDIDA EPRTA_MEDIDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_MOVIMIENTO_DETALLE> EPRTA_MOVIMIENTO_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VW_ORDEN_COMPRA_DETALLE> VW_ORDEN_COMPRA_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_CIERRE_INVENTARIO_DET> EPRTA_CIERRE_INVENTARIO_DET { get; set; }
    }
}
