using System.Data;
using System.Text;

namespace Pure.Data
{
    public class TransactionTestIntercept : Singleton<TransactionTestIntercept>, ITransactionInterceptor
    {


        public void OnBeginTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " Begin Trancetion!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnAbortTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " Abort Trancetion!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnCompleteTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " Complete Trancetion!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }


        public void OnRollbackTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " Rollback Transaction!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnCommitTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " Commit Transaction!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }
    }
}
