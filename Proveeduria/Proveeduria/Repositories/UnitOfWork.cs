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
        private GenericRepository<EPRTA_ITEM> itemRepository = null;
        private GenericRepository<EPRTA_ARTICULO_BODEGA> articuloBodegaRepository = null;
        private GenericRepository<EPRTA_MOVIMIENTO> movimientoRepository = null;
        private GenericRepository<EPRTA_SECUENCIA> secuenciaRepository = null;
        private GenericRepository<EPRTA_BODEGA> bodegaRepository = null;
        private GenericRepository<VW_DEPARTAMENTO> departamentoRepository = null;
        private GenericRepository<VW_EMPLEADO> empleadoRepository = null;

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

        public GenericRepository<EPRTA_ITEM> ItemRepository
        {
            get
            {
                if (itemRepository == null)
                {
                    itemRepository = new GenericRepository<EPRTA_ITEM>(context);
                }
                return itemRepository;
            }
        }


        public GenericRepository<EPRTA_ARTICULO_BODEGA> ArticuloBodegaRepository
        {
            get
            {
                if (articuloBodegaRepository == null)
                {
                    articuloBodegaRepository = new GenericRepository<EPRTA_ARTICULO_BODEGA>(context);
                }
                return articuloBodegaRepository;
            }
        }

        public GenericRepository<EPRTA_MOVIMIENTO> MovimientoRepository
        {
            get
            {
                if (movimientoRepository == null)
                {
                    movimientoRepository = new GenericRepository<EPRTA_MOVIMIENTO>(context);
                }
                return movimientoRepository;
            }
        }

        public GenericRepository<EPRTA_SECUENCIA> SecuenciaRepository
        {
            get
            {
                if (secuenciaRepository == null)
                {
                    secuenciaRepository = new GenericRepository<EPRTA_SECUENCIA>(context);
                }
                return secuenciaRepository;
            }
        }


        public GenericRepository<VW_EMPLEADO> EmpleadoRepository
        {
            get
            {
                if (empleadoRepository == null)
                {
                    empleadoRepository = new GenericRepository<VW_EMPLEADO>(context);
                }
                return empleadoRepository;
            }
        }
        public GenericRepository<EPRTA_BODEGA> BodegaRepository
        {
            get
            {
                if (bodegaRepository == null)
                {
                    bodegaRepository = new GenericRepository<EPRTA_BODEGA>(context);
                }
                return bodegaRepository;
            }
        }

        public GenericRepository<VW_DEPARTAMENTO> DepartamentoRepository
        {
            get
            {
                if (departamentoRepository == null)
                {
                    departamentoRepository = new GenericRepository<VW_DEPARTAMENTO>(context);
                }
                return departamentoRepository;
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