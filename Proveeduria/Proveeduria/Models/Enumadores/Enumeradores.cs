using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proveduria.Models.Enumadores
{
        public enum EnumEstadoSolicitud
        {
            Solicitado = 'S',
            Autorizado = 'A',
            Despachado = 'D',
            Anulado = 'E'
        }

    public enum EnumTipoMovimiento
    {
        REQUISICION_BODEGA = 2,
        INGRESO_DE_ORDEN_DE_COMPRA = 4,
        AJUSTE_DE_BODEGA_POR_EGRESO = 11,
        AJUSTE_DE_BODEGA_POR_INGRESO = 10
    }

    public enum EnumEstadoRegistro
    {
        Activo = 'A',
        Inactivo = 'I'
    }

    public enum EnumIngresoEgreso
    {
        INGRESO = 'I',
        EGRESO = 'E',
        NEUTRAL = 'N'
    }


}