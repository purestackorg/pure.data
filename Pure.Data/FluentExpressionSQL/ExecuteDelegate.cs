
using System.Data;
using System.Threading.Tasks;

namespace FluentExpressionSQL
{
    public delegate object ExecuteScalarDelegate(string sql);
    public delegate int ExecuteDelegate(string sql);
    public delegate IDataReader ExecuteReaderDelegate(string sql);
    public delegate Task<object> ExecuteScalarAsyncDelegate(string sql);
    public delegate Task<int> ExecuteAsyncDelegate(string sql); 
    public delegate  Task<IDataReader> ExecuteReaderAsyncDelegate(string sql);
    //public delegate System.Collections.Generic.IEnumerable<TEntity> ExecuteQueryDelegate<TEntity>(string sql);
    //public delegate object ExecuteQueryDelegate(string sql);
    //public delegate TResult Func<T, TResult>(T arg);

}
