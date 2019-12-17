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
            if (database.Config.EnableDebug)
            {
                string connStr = "    (conn: " + database.Connection?.GetHashCode() + ", status: " + database.Connection?.State + ")";
                //database.LogHelper.Error(exception);
                database.LogHelper.Write("Error at: "+database?.LastSQL+ connStr, exception, MessageType.Error);

            }

        }
    }
}
