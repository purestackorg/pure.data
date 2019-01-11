using System.Data;
using System.Text;

namespace Pure.Data
{
    public class ConnectionTestIntercept : Singleton<ConnectionTestIntercept>, IConnectionInterceptor
    {

        public System.Data.IDbConnection OnConnectionOpened(IDatabase database, System.Data.IDbConnection conn)
        {
            if (conn.State == ConnectionState.Open)
            {
                string str = string.Format(database.DatabaseName + " was opened!" + conn.GetHashCode());
                database.LogHelper.Warning(str);
               
            }
            return conn;
        }

        public void OnConnectionClosing(IDatabase database, System.Data.IDbConnection conn)
        {
            if ( conn.State != ConnectionState.Closed)
            {
                string str = string.Format(database.DatabaseName + " was closing!" + conn.GetHashCode());
                database.LogHelper.Warning(str);
            }

           
        }
    }
}
