using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Proveduria.Repositories
{
    interface IGenericRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> WherePaged(int page, int pageSize, Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAllPaged(int page, int pageSize);
        TEntity GetById(object id);
        void Insert(TEntity obj);
        void InsertAll(IEnumerable<TEntity> objs);
        void Update(TEntity obj);
        void Delete(object id);
        void DeleteAll(IEnumerable<TEntity> objs);
        void Save();
        int Count();
        int Count(Expression<Func<TEntity, bool>> criteria);
    }
}