using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proveduria.Models
{
    public class ConsultaMovimiento
    {
        public int ID_MOVIMIENTO { get; set; }
        public short ANIO { get; set; }
        public int NUMERO_MOVIMIENTO { get; set; }
        public string TIPO_MOVIMIENTO { get; set; }
        public string OBSERVACION { get; set; }
        public string ESTADO { get; set; }
        public byte ID_BODEGA { get; set; }
        public string USUARIO_APRUEBA { get; set; }
        public string USUARIO_AUTORIZA { get; set; }
        public string USUARIO_SOLICITA { get; set; }
        public Nullable<byte> ID_DEPARTAMENTO_SOLICITA { get; set; }
        public Nullable<System.DateTime> FECHA_SOLICITUD { get; set; }
        public Nullable<System.DateTime> FECHA_AUTORIZACION { get; set; }
        public Nullable<System.DateTime> FECHA_APROBACION { get; set; }
        public string USUARIO_ANULA { get; set; }
        public Nullable<System.DateTime> FECHA_ANULA { get; set; }
        public string MOTIVO_ANULA { get; set; }
        public Nullable<int> ID_MOVIMIENTO_RELACION { get; set; }
        public Nullable<short> ANIO_DOCUMENTO_REFERENCIA { get; set; }
        public Nullable<int> NUMERO_DOCUMENTO_REFERENCIA { get; set; }
        public long ID_DETALLE { get; set; }
        public string ITEM { get; set; }
        public Nullable<decimal> COSTO_ACTUAL { get; set; }
        public Nullable<decimal> STOCK_ACTUAL { get; set; }
        public Nullable<decimal> COSTO_MOVIMIENTO { get; set; }
        public Nullable<decimal> CANTIDAD_MOVIMIENTO { get; set; }
        public string DESCRIPCION_ADICIONAL { get; set; }
        public Nullable<decimal> CANTIDAD_NUEVO_STOCK { get; set; }

    }
}