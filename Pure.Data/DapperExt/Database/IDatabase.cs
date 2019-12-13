 
using FluentExpressionSQL;
using FluentExpressionSQL.Sql;
using Pure.Data.Sql;
using Pure.Data.Validations.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
#if ASYNC
using System.Threading.Tasks;
#endif
namespace Pure.Data
{
    /// <summary>
    /// 数据库上下文接口
    /// </summary>
    public interface IDatabase :  IDisposable
    {
        int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        List<T> ExecuteList<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null);
        List<T> ExecuteListWithRowDelegate<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        List<T> ExecuteListByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T ExecuteModel<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T ExecuteModelByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        DataTable ExecuteDataTable(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        DataTable ExecuteDataTableWithRowDelegate(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        System.Data.DataSet ExecuteDataSet(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IDictionary<string, object> ExecuteDictionary(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        dynamic ExecuteExpandoObject(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<dynamic> ExecuteExpandoObjects(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        T SqlQueryFirstOrDefault<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> SqlQuery<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null);

        bool HasActiveTransaction { get; }
        IDbConnection Connection { get; }
        void SetConnectionAlive(bool enable);
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
        void RunInTransaction(Action action);
        T RunInTransaction<T>(Func<T> func);
        T Get<T>(object id, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        T Get<T>(object id, int? commandTimeout = null) where T : class;
        void InsertBulk(DataTable dt, IDbTransaction transaction = null, int? commandTimeout = null);
        void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        //void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        //void InsertBulk<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class;
        void InsertBatch(DataTable dt, int batchSize = 10000);
        void InsertBatch<T>(IEnumerable<T> entities, int batchSize = 10000, int? commandTimeout = null) where T : class;
        void InsertBatch<T>(IEnumerable<T> entities, IDbTransaction transaction , int batchSize = 10000, int? commandTimeout = null) where T : class;
        dynamic Insert<T>(T entity, IDbTransaction transaction , int? commandTimeout = null) where T : class;
        dynamic Insert<T>(T entity, int? commandTimeout = null) where T : class;
        int InsertByKV<T>(IDictionary<string, object> parameters) where T : class;
        bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class;
        bool Update<T>(T entity, int? commandTimeout = null) where T : class;

        int UpdateByKV<T>(IDictionary<string, object> parameters, IDictionary<string, object> conditions) where T : class;
        int UpdateBySQL(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = null);
        bool Delete<T>(T entity, IDbTransaction transaction , int? commandTimeout = null) where T : class;
        bool Delete<T>(T entity, int? commandTimeout = null) where T : class;
        bool Delete<T>(object predicate, IDbTransaction transaction , int? commandTimeout = null) where T : class;
        bool Delete<T>(object predicate, int? commandTimeout = null) where T : class;
        int DeleteByKV<T>(IDictionary<string, object> conditions) where T : class;
        bool DeleteById<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        bool DeleteByIds<T>(string cName, string ids, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        int DeleteAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        int Truncate<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        IDataReader QueryByKV<T>(string[] columns, IDictionary<string, object> conditions, IList<ISort> sort) where T : class;
        IEnumerable<T> Query<T>(object predicate, IList<ISort> sort, IDbTransaction transaction , int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> Query<T>(object predicate = null, IList<ISort> sort = null, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, int? commandTimeout = null, bool buffered = true) where T : class;
        IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, out long allRowsCount, string sql, object param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class;
        IDataReader GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered=true) where T : class;
        PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize, string sql, object param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = false) where T : class;
        PageDataResult<IDataReader> GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort,  IDbTransaction transaction , int? commandTimeout = null, bool buffered=true) where T : class;
        IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort,  int? commandTimeout = null, bool buffered = true) where T : class;
        int Count<T>(object predicate, IDbTransaction transaction , int? commandTimeout = null) where T : class;
        int Count<T>(object predicate, int? commandTimeout = null) where T : class;
        int Count<T>() where T : class;

        int CountByKV<T>(IDictionary<string, object> conditions) where T : class;
        long LongCountByKV<T>(IDictionary<string, object> conditions) where T : class;

        long LongCount<T>() where T : class;
        long LongCount<T>(object predicate, IDbTransaction transaction , int? commandTimeout = null) where T : class;
        long LongCount<T>(object predicate, int? commandTimeout = null) where T : class;
        IMultipleResultReader GetMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction , int? commandTimeout = null);
        IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout = null);
        bool Exists<T>(object predicate) where T : class;
        void ClearMap();
        Guid GetNextGuid();
        IClassMapper GetMap(string tableName);
        IClassMapper GetMap<T>() where T : class;
        string GetColumnString<T>(string prefix = "T.", string spliteStr = ", ", params Expression<Func<T, object>>[] ignoreProperties) where T : class;
        IClassMapper GetMap(Type type);
        void LoadAllMap(List<Assembly> MappingAssemblies = null, LoadMapperMode LoadMode = LoadMapperMode.FluentMapper);
        ValidationResult Validate<T>(T instance) where T : class;
        ValidationResult Validate<T>(T instance, params Expression<Func<T, object>>[] propertyExpressions) where T : class;
        ValidationResult Validate<T>(T instance, params string[] properties) where T : class;
        void ValidateAndThrow<T>(T instance) where T : class;
        System.Collections.Concurrent.ConcurrentDictionary<Type, IClassMapper> GetAllMap();
        DynamicParameters NewDynamicParameters();


        #region Expression
        IEnumerable<TEntity> ExecuteQuery<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class;
        IDataReader ExecuteReader<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class;
        TValue ExecuteScalar<TEntity, TValue>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class;
        int Execute<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class;
        int Insert<TEntity>(Expression<Func<object>> body) where TEntity : class;
        int Delete<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        int Update<TEntity>(Expression<Func<object>> body, Expression<Func<TEntity, bool>> condition) where TEntity : class;


        IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class;
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction) where TEntity : class;
        
        IEnumerable<TEntity> QueryByWhere<TEntity>(string condition, string orderStr) where TEntity : class;
        IEnumerable<TEntity> QueryBySQL<TEntity>(string sql) where TEntity : class;
        IEnumerable<TEntity> GetPageByWhere<TEntity>(int pageIndex, int pagesize, string condition, string orderStr, out int totalCount) where TEntity : class;
        List<TEntity> GetPageBySQL<TEntity>(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount) where TEntity : class;
       IDataReader GetPageReaderBySQL(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount) ;
        IEnumerable<TEntity> GetPage<TEntity>(int pageIndex, int pagesize, Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction, out int totalCount) where TEntity : class;
        string GetSqlOfPage(int pageIndex, int pagesize, string sql, IDictionary<string, object> parameters);
        string GetSqlOfCount(string sql);
        IDataReader GetPageReader(int pageIndex, int pagesize, string sql, string orderField, bool asc, IDictionary<string, object> parameters, out int total);
        long LongCount<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        int Count<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        int Count(string sql);
        bool Exists<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class;
        #endregion

        IDatabaseConfig Config { get; }
        ISqlGenerator SqlGenerator { get; }
        ISqlDialectProvider SqlDialectProvider { get; }
#if ASYNC
         IDapperAsyncImplementor DapperImplementor { get; }
#else
         IDapperImplementor DapperImplementor{ get; }
#endif
        string ConnectionString { get; }
        DatabaseType DatabaseType { get; }
       

        IDbTransaction Transaction
        {
            get;
        }
        string DatabaseName { get;  }
        string ProviderName { get; }
        System.Data.Common.DbProviderFactory DbFactory { get; }
        Stopwatch Watch { get; }
        string LastSQL { get; set; }
        IDataParameterCollection LastArgs { get; set; }
        LogHelper LogHelper { get; }
        void EnsureAddClassToTableMap<TEntity>() where TEntity : class;
        FluentExpressionSqlBuilder FluentSqlBuilder { get; } 
        SqlBuilder SqlBuilder { get ; }

        IDatabase OpenNewConnection();
        IDatabase OpenSharedConnection();
        DbConnection CreateNewDbConnection(bool needInitAction = false);

        void EnsureOpenConnection();
        void Close();
        void CloseReally();
        void OnConnectionClosingWithIntercept();
        ITransaction GetTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void SetTransaction(IDbTransaction tran);

        #region 中断器执行
        void OnDatabaseConnectionClosing();
        void OnDatabaseException(Exception ex);
        #endregion

        #region 异步操作
#if ASYNC

        //Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        //Task<object> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        //Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        
        //Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        

        ///// <summary>
        ///// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        ///// </summary>
        //Task<int> CountAsync<T>(object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
        

        ///// <summary>
        ///// Executes a query for the specified id, returning the data typed as per T.
        ///// </summary>
        //Task<T> GetAsync<T>(dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        ///// <summary>
        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        ///// </summary>
        //Task<IEnumerable<T>> GetListAsync<T>(object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;

        ///// <summary>
        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        ///// Data returned is dependent upon the specified page and resultsPerPage.
        ///// </summary>
        //Task<IEnumerable<T>> GetPageDataAsync<T>(object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;
       
        ///// <summary>
        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        ///// Data returned is dependent upon the specified firstResult and maxResults.
        ///// </summary>
        //Task<IEnumerable<T>> GetSetAsync<T>(object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class;


        //Task<IEnumerable<T>> SqlQueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null);
        

#endif
        #endregion

        BatchCommander NewBatchCommand(BatchOptions option, IDbTransaction tran = null);

        #region SqlMap
        /// <summary>
        /// 执行Sql Map配置文件中的Sql
        /// </summary>
        /// <param name="scope">区域</param>
        /// <param name="sqlID">sql编号</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Pure.Data.SqlMap.SqlMapStatement QuerySqlMap(string scope, string sqlID, object param = null);
        #endregion

        /// <summary>
        /// 追踪实体对象，可用于更新对象的指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        Snapshot<T> Track<T>(T obj) where T : class;

        #region Ado.Ext
        int Insert(string tableName, IDictionary<string, object> parameters);
        int Update(string tableName, IDictionary<string, object> parameters, IDictionary<string, object> conditions);
        int Delete(string tableName, IDictionary<string, object> conditions);
        IDataReader Query(string tableName, string[] columns, IDictionary<string, object> conditions, IList<ISort> sort);
        int Count(string tableName, IDictionary<string, object> conditions);

        long LongCount(string tableName, IDictionary<string, object> conditions);

        int Update(string tableName, IDictionary<string, object> parameters, IPredicate conditions);
        int Delete(string tableName, IPredicate conditions);
        #endregion


        #region Multi-Relations
        IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, TRet>(Func<T1, T2, T3, T4, T5, T6, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, T7, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);
        #endregion

        #region log
        void Debug(string msg);
        void Error(Exception msg);
        void Warning(string msg);
        #endregion

        #region Queryable
        SqlQuery<T> Queryable<T>() where T : class;

        #endregion

        #region Backup & Sql Gen & Migrate
        string Backup<T>(BackupOption option) where T : class;
        string GenerateCode( );

        Migration.Framework.ITransformationProvider CreateTransformationProvider(bool isCache = true);
        #endregion

        #region UpdateOnly
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        int UpdateWithOnlyParams<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class;
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        int UpdateOnly<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class;

#if ASYNC
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        Task<int> UpdateWithOnlyParamsAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class;
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        Task<int> UpdateOnlyAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class;

#endif

        #endregion

    }





}
