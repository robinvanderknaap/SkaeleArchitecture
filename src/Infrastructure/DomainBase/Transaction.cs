using System;
using NHibernate;

namespace Infrastructure.DomainBase
{
    public class Transaction : IDisposable
    {
        private readonly ISession _session;
        private readonly bool _isAlreadyInTransaction;

        public Transaction(ISession session)
        {
            _session = session;

            // Determine if session is already in transaction
            _isAlreadyInTransaction = session.Transaction.IsActive;

            // Only start transaction when session is not part of a transaction yet
            if (!_isAlreadyInTransaction)
            {
                session.BeginTransaction();
            }
        }

        public void Commit()
        {
            if (!_isAlreadyInTransaction)
            {
                _session.Transaction.Commit();
            }
        }

        public void Rollback()
        {
            _session.Transaction.Rollback();
        }

        public void Dispose()
        {
            if (!_isAlreadyInTransaction && _session.Transaction.IsActive)
            {
                _session.Transaction.Rollback();
            }
        }
    }
}
