﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Proveduria.Models;
using System.Data.OracleClient;

namespace Proveduria.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private EntitiesProveduria db = null;
        private DbSet<TEntity> table = null;

        public GenericRepository()
        {
            this.db = new EntitiesProveduria();
            table = db.Set<TEntity>();
        }

        public GenericRepository(EntitiesProveduria db)
        {
            this.db = db;
            table = db.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return table.ToList();
        }

        public TEntity GetById(object id)
        {
            return table.Find(id);
        }

        public void Insert(TEntity obj)
        {
            table.Add(obj);
        }

        public void Update(TEntity obj)
        {
            table.Attach(obj);
            db.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            TEntity existing = table.Find(id);
            table.Remove(existing);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}