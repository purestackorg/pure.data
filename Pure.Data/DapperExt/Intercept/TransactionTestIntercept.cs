using System.Data;
using System.Text;

namespace Pure.Data
{
    public class TransactionTestIntercept : Singleton<TransactionTestIntercept>, ITransactionInterceptor
    {


        public void OnBeginTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnBegin!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnAbortTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnAbort!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnCompleteTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnComplete!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }
    }
}
