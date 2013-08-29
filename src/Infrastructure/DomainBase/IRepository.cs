using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;

namespace Infrastructure.DomainBase
{
    /// <summary>
    /// Provides basic operations used by every repository
    /// </summary>
    public interface IRepository<TEntity> where TEntity : Entity
    {
        ISession Session { get; }
        TEntity Get(int id);
        TEntity GetOne(Expression<Func<TEntity, bool>> criteria);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> criteria);
        void Save(TEntity entity);
    }
}
