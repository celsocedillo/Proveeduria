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
    
    public partial class EPRTA_MOVIMIENTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EPRTA_MOVIMIENTO()
        {
            this.EPRTA_MOVIMIENTO_DETALLE = new HashSet<EPRTA_MOVIMIENTO_DETALLE>();
            this.EPRTA_MOVIMIENTO1 = new HashSet<EPRTA_MOVIMIENTO>();
        }
    
        public int ID_MOVIMIENTO { get; set; }
        public short ANIO { get; set; }
        public int NUMERO_MOVIMIENTO { get; set; }
        public byte ID_TIPO_MOVIMIENTO { get; set; }
        public string OBSERVACION { get; set; }
        public string ESTADO { get; set; }
        public byte ID_BODEGA { get; set; }
        public string USUARIO_APRUEBA { get; set; }
        public string USUARIO_AUTORIZA { get; set; }
        public string USUARIO_SOLICITA { get; set; }
        public Nullable<System.DateTime> FECHA_SOLICITUD { get; set; }
        public Nullable<System.DateTime> FECHA_AUTORIZACION { get; set; }
        public Nullable<System.DateTime> FECHA_APROBACION { get; set; }
        public string USUARIO_ANULA { get; set; }
        public Nullable<System.DateTime> FECHA_ANULA { get; set; }
        public string MOTIVO_ANULA { get; set; }
        public Nullable<int> ID_MOVIMIENTO_RELACION { get; set; }
        public Nullable<short> ANIO_DOCUMENTO_REFERENCIA { get; set; }
        public Nullable<int> NUMERO_DOCUMENTO_REFERENCIA { get; set; }
        public Nullable<byte> ID_DIRECCION_SOLICITA { get; set; }
    
        public virtual EPRTA_BODEGA EPRTA_BODEGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_MOVIMIENTO_DETALLE> EPRTA_MOVIMIENTO_DETALLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_MOVIMIENTO> EPRTA_MOVIMIENTO1 { get; set; }
        public virtual EPRTA_MOVIMIENTO EPRTA_MOVIMIENTO2 { get; set; }
        public virtual EPRTA_TIPO_MOVIMIENTO EPRTA_TIPO_MOVIMIENTO { get; set; }
        public virtual VW_ORDEN_COMPRA VW_ORDEN_COMPRA { get; set; }
        public virtual VW_DIRECCION VW_DIRECCION { get; set; }
    }
}
