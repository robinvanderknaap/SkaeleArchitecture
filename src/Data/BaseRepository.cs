using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.DomainBase;
using NHibernate;
using NHibernate.Linq;

namespace Data
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        public abstract ISession Session { get; }
        
        public virtual TEntity Get(int id)
        {
            return Session.Get<TEntity>(id);
        }

        public virtual TEntity GetOne(Expression<Func<TEntity, bool>> criteria)
        {
            return Session.Query<TEntity>().SingleOrDefault(criteria);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Session.Query<TEntity>();
        }
        
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> criteria)
        {
            return Session.Query<TEntity>().Where(criteria);
        }

        public virtual void Save(TEntity entity)
        {
            typeof(Entity)
                  .GetProperty("Modified")
                  .SetValue(entity, DateTime.UtcNow, null);

            if (entity.IsTransient())
            {
                typeof(Entity)
                    .GetProperty("Created")
                    .SetValue(entity, DateTime.UtcNow, null);
            }
            
            Session.Save(entity);
        }
    }
}
