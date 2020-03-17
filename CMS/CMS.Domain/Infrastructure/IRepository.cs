using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CMS.Domain.Infrastructure
{
    public interface IRepository : IDisposable
    {
        Entity Add<Entity>(Entity entity) where Entity : class;
        Entity Load<Entity>(Expression<Func<Entity, bool>> predicate, params Expression<Func<Entity, object>>[] includes) where Entity : class;
        IQueryable<Entity> LoadList<Entity>(Expression<Func<Entity, bool>> predicate, params Expression<Func<Entity, object>>[] includes) where Entity : class;
        IQueryable<Entity> LoadAll<Entity>(params Expression<Func<Entity, object>>[] includes) where Entity : class;
        EntityProjection Project<Entity, EntityProjection>(Func<IQueryable<Entity>, EntityProjection> query) where Entity : class;
        void CommitChanges();
        void Update<Entity>(Entity entity) where Entity : class;
        void Delete<Entity>(Entity entity) where Entity : class;
        List<Entity> SqlQuery<Entity>(string sql);
        void Remove<Entity>(Entity entity) where Entity : class;
        void RemoveRange<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class;
        IQueryable<Entity> LoadListByStoredProcedure<Entity>(string spName, params SqlParameter[] paramerers) where Entity : class;
        void Detach<Entity>(Entity entity) where Entity : class;
    }
}
