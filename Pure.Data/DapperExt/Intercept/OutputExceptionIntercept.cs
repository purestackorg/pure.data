using System.Data;
using System.Text;

namespace Pure.Data
{
    public class OutputExceptionIntercept : Singleton<OutputExceptionIntercept>, IExceptionInterceptor
    {

        public void OnException(IDatabase database, System.Exception exception)
        {
            if (database.Connection != null && database.Connection.State != ConnectionState.Closed)
            {
                database.Connection.Close();
            }
            if (database.Config.EnableDebug )
            {
                database.LogHelper.Error(exception);
                
            }

        }
    }
}
