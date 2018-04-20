using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationMvc.Models;

namespace WebApplicationMvc.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private bool disposed = false;
        private EntitiesProveduria context = new EntitiesProveduria();
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