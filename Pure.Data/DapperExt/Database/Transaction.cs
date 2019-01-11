using System;
using System.Data;

namespace Pure.Data
{
    public class Transaction : ITransaction
    {
        IDatabase _db;

        public Transaction(IDatabase db, IsolationLevel isolationLevel)
        {
            _db = db;
            _db.BeginTransaction(isolationLevel);
        }

        public virtual void Complete()
        {
            _db.CommitTransaction();
            _db = null;
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.RollbackTransaction();
            }
        }
    }

    public interface ITransaction : IDisposable
    {
        void Complete();
    }
}
