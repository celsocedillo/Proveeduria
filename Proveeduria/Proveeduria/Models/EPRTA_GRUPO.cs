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
    
    public partial class EPRTA_GRUPO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EPRTA_GRUPO()
        {
            this.EPRTA_ITEM = new HashSet<EPRTA_ITEM>();
        }
    
        public byte ID_GRUPO { get; set; }
        public string CODIGO { get; set; }
        public string NOMBRE { get; set; }
        public string ESTADO { get; set; }
        public string CUENTA_CONTABLE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EPRTA_ITEM> EPRTA_ITEM { get; set; }
    }
}
