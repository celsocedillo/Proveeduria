using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proveduria.Models.Enumadores
{
    public class Enumeradores
    {
        public enum EnumEstadoSolicitud
        {
            Solicitado = 'S',
            Autorizado = 'A',
            Despachado = 'D',
            Anulado = 'E'
        }
    }
}