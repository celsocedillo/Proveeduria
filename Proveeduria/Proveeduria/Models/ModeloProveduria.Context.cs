﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EntitiesProveduria : DbContext
    {
        public EntitiesProveduria()
            : base("name=EntitiesProveduria")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<EPRTA_ARTICULO_BODEGA> EPRTA_ARTICULO_BODEGA { get; set; }
        public virtual DbSet<EPRTA_BODEGA> EPRTA_BODEGA { get; set; }
        public virtual DbSet<EPRTA_GRUPO> EPRTA_GRUPO { get; set; }
        public virtual DbSet<EPRTA_ITEM> EPRTA_ITEM { get; set; }
        public virtual DbSet<EPRTA_MEDIDA> EPRTA_MEDIDA { get; set; }
        public virtual DbSet<EPRTA_MOVIMIENTO> EPRTA_MOVIMIENTO { get; set; }
        public virtual DbSet<EPRTA_MOVIMIENTO_DETALLE> EPRTA_MOVIMIENTO_DETALLE { get; set; }
        public virtual DbSet<EPRTA_SECUENCIA> EPRTA_SECUENCIA { get; set; }
        public virtual DbSet<EPRTA_TIPO_MOVIMIENTO> EPRTA_TIPO_MOVIMIENTO { get; set; }
        public virtual DbSet<VW_EMPLEADO> VW_EMPLEADO { get; set; }
    }
}
