using CMS.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace CMS.Domain.Storage
{
    public class Repository : IRepository
    {
        readonly CMSDbContext _context;

        public Repository(CMSDbContext context)
        {
            _context = context;
        }

        public Entity Add<Entity>(Entity entity) where Entity : class
        {
            return GetDbSet<Entity>().Add(entity);
        }

        public void CommitChanges()
        {
            //if (_context == null) return;
            _context.SaveChanges();
        }

        public void Delete<Entity>(Entity entity) where Entity : class
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                GetDbSet<Entity>().Attach(entity);
            }
            GetDbSet<Entity>().Remove(entity);
        }

        public void Dispose()
        {
        }

        public Entity Load<Entity>(Expression<Func<Entity, bool>> predicate, params Expression<Func<Entity, object>>[] includes) where Entity : class
        {
            var queryable = GetDbSet<Entity>().AsQueryable();
            if (includes != null)
            {
                queryable = includes.Aggregate(queryable, (set, inc) => set.Include(inc));
            }
            return queryable.SingleOrDefault(predicate);
        }

        public IQueryable<Entity> LoadAll<Entity>(params Expression<Func<Entity, object>>[] includes) where Entity : class
        {
            var queryable = GetDbSet<Entity>().AsQueryable();
            if (includes != null)
            {
                queryable = includes.Aggregate(queryable, (set, inc) => set.Include(inc));
            }
            return queryable.AsQueryable();
        }

        public IQueryable<Entity> LoadList<Entity>(Expression<Func<Entity, bool>> predicate, params Expression<Func<Entity, object>>[] includes) where Entity : class
        {
            var queryable = GetDbSet<Entity>().AsQueryable();
            if (includes != null)
            {
                queryable = includes.Aggregate(queryable, (set, inc) => set.Include(inc));
            }
            return queryable.Where(predicate).AsQueryable();
        }

        public IQueryable<Entity> LoadListByStoredProcedure<Entity>(string spName, params SqlParameter[] paramerers) where Entity : class
        {
            return _context.Database.SqlQuery<Entity>(spName + " " + string.Join(",", paramerers.Select(x => x.ParameterName).ToList()), paramerers).AsQueryable();
        }

        public EntityProjection Project<Entity, EntityProjection>(Func<IQueryable<Entity>, EntityProjection> query) where Entity : class
        {
            return query(GetDbSet<Entity>().AsQueryable());
        }

        public void Remove<Entity>(Entity entity) where Entity : class
        {
            GetDbSet<Entity>().Remove(entity);
        }

        public void RemoveRange<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class
        {
            IQueryable<Entity> queryable = GetDbSet<Entity>().AsQueryable();
            GetDbSet<Entity>().RemoveRange(queryable.Where(predicate).AsQueryable());
        }

        public List<Entity> SqlQuery<Entity>(string sql)
        {
            return _context.Database.SqlQuery<Entity>(sql).ToList();
        }

        public void Update<Entity>(Entity entity) where Entity : class
        {
            GetDbSet<Entity>().AddOrUpdate(entity);
        }

        DbSet<Entity> GetDbSet<Entity>() where Entity : class
        {
            return _context.Set<Entity>();
        }

        public void Detach<Entity>(Entity entity) where Entity : class
        {
            if (_context.Entry(entity).State == EntityState.Modified
                || _context.Entry(entity).State == EntityState.Added
                || _context.Entry(entity).State == EntityState.Deleted)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
        }
    }
}
