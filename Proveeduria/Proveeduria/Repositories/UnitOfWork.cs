using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Proveduria.Models;

namespace Proveduria.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private bool disposed = false;
        private EntitiesProveduria context = new EntitiesProveduria();
        private GenericRepository<EPRTA_TIPO_MOVIMIENTO> tipoMovimientoRepository = null;
        private GenericRepository<EPRTA_GRUPO> grupoRepository = null;
        private GenericRepository<EPRTA_MEDIDA> medidaRepository = null;

        //private GenericRepository<INV_MEDIDA> invMedidaRepository = null;


        //public GenericRepository<INV_MEDIDA> InvMedidaRepository
        //{
        //    get
        //    {
        //        if (invMedidaRepository == null)
        //        {
        //            invMedidaRepository = new GenericRepository<INV_MEDIDA>(context);
        //        }
        //        return invMedidaRepository;
        //    }
        //}

        public GenericRepository<EPRTA_TIPO_MOVIMIENTO> TipoMovimientoRepository
        {
            get
            {
                //context.Configuration.ProxyCreationEnabled = false;
                if (tipoMovimientoRepository == null)
                {
                    tipoMovimientoRepository = new GenericRepository<EPRTA_TIPO_MOVIMIENTO>(context);
                }
                return tipoMovimientoRepository;
            }
        }

        public GenericRepository<EPRTA_GRUPO> GrupoRepository
        {
            get
            {
                //context.Configuration.ProxyCreationEnabled = false;
                if (grupoRepository == null)
                {
                    grupoRepository = new GenericRepository<EPRTA_GRUPO>(context);
                }
                return grupoRepository;
            }
        }

        public GenericRepository<EPRTA_MEDIDA> MedidaRepository
        {
            get
            {
                if (medidaRepository == null)
                {
                    medidaRepository = new GenericRepository<EPRTA_MEDIDA>(context);
                }
                return medidaRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}