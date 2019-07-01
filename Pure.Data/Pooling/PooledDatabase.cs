using Dapper;
using FluentExpressionSQL;
using FluentExpressionSQL.Sql;
using Pure.Data.Pooling;
using Pure.Data.Sql;
using Pure.Data.Validations.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Pure.Data
{
    //public class DatabaseConfigFileObjectPolicy : IPooledObjectPolicy<IDatabase>
    //{
    //    private string _configFile;
    //    private OutputActionDelegate _logAction;
    //    private Action<IDatabaseConfig> _initConfig;
    //    public DatabaseConfigFileObjectPolicy(string configFile, OutputActionDelegate logAction = null, Action<IDatabaseConfig> initConfig = null) {
    //        _configFile = configFile;
    //        _logAction = logAction;
    //        _initConfig = initConfig;
    //    }
    //    public IDatabase Create()
    //    {
    //        return new Database(_configFile, _logAction, _initConfig);
    //    } 
    //    public bool Return(IDatabase obj)
    //    {
    //        return true;
    //    }
         
    //    public void OnCreateEvent(ObjectWrapper<IDatabase> obj)
    //    {
    //        _logAction("OnCreateEvent -> "+obj.ToString(), null, MessageType.Debug);
    //    }

    //    public void OnDestroyEvent(ObjectWrapper<IDatabase> obj)
    //    {
    //        _logAction("OnDestroyEvent -> " + obj.ToString(), null, MessageType.Error);

    //    }

    //    public void OnGetEvent(ObjectWrapper<IDatabase> obj)
    //    {
    //        _logAction("OnGetEvent -> " + obj.ToString(), null, MessageType.Info);

    //    }

    //    public void OnReturnEvent(ObjectWrapper<IDatabase> obj)
    //    {
    //        _logAction("OnReturnEvent -> " + obj.ToString(), null, MessageType.Warning);

    //    }
    //}
    public class PooledDatabase: IDisposable
    {
        #region PooledDatabase
        //private IObjectPool<IDatabase> pool = null;
        //public PooledDatabase(string configFile, OutputActionDelegate logAction = null, Action<IDatabaseConfig> initConfig = null, int maximumRetained = 0)
        //    :this(new DatabaseConfigFileObjectPolicy(configFile, logAction, initConfig), maximumRetained)
        //{ 
        //}
        //public PooledDatabase(IPooledObjectPolicy<IDatabase> policy, int maximumRetained = 0)
        //{
        //    if (maximumRetained == 0)
        //    {
        //        maximumRetained = Environment.ProcessorCount * 2;
        //    }
        //    pool = new ObjectPool<IDatabase>(policy, maximumRetained);
        //    currentDatabaseLocal = new AsyncLocal<IDatabase>();
        //}
        public IObjectPool<PooledObjectWrapper<IDatabase>> Pool = null;
        public PooledDatabase(Func<PooledObjectWrapper<IDatabase>> createFunc, int maximumRetained = 0, EvictionSettings evictSetting = null)
        {
            if (maximumRetained == 0)
            {
                maximumRetained = Environment.ProcessorCount * 2;
            }
            //pool = new ObjectPool<IDatabase>(policy, maximumRetained);
            currentDatabaseLocal = new AsyncLocal<PooledObjectWrapper<IDatabase>>();


            Pool = new ObjectPool<PooledObjectWrapper<IDatabase>>(maximumRetained, () =>
            createFunc(), evictSetting, null);

        

        }
        private readonly ConditionalWeakTable<IDatabase, PooledObjectWrapper<IDatabase>> _wrapperMap = new ConditionalWeakTable<IDatabase, PooledObjectWrapper<IDatabase>>();

        private AsyncLocal<PooledObjectWrapper<IDatabase>> currentDatabaseLocal = null;
        public PooledObjectWrapper<IDatabase> GetPooledDatabase()
        {
            //var o = Pool.GetObject();
            //_wrapperMap.GetValue(o.InternalResource, _ => o);
            //return o;

            if (currentDatabaseLocal.Value == null)
            {
                currentDatabaseLocal.Value = Pool.GetObject();

                _wrapperMap.GetValue(currentDatabaseLocal.Value.InternalResource, _ => currentDatabaseLocal.Value);
                if (currentDatabaseLocal.Value == null)
                {
                    throw new PureDataException("PooledDatabase GetCurrentDatabaseWrapper has been null!", null);
                }
            }

            return currentDatabaseLocal.Value;
        }
        private IDatabase GetCurrentDatabase() {
            return GetPooledDatabase().InternalResource;
            
            //if (currentDatabaseLocal.Value == null)
            //{
            //    currentDatabaseLocal.Value = Pool.Get();
            //    if (currentDatabaseLocal.Value == null)
            //    {
            //        throw new PureDataException("PooledDatabase GetCurrentDatabase has been null!", null);
            //    }
            //}

            //return currentDatabaseLocal.Value;
        }
        public void ReturnPooledDatabase(IDatabase database)
        {
            //currentDatabaseLocal.Value = null;
            //pool.Return(database);

            //if (currentDatabaseLocal.Value != null)
            //{
            //    pool.ReturnObject(currentDatabaseLocal.Value);

            //}

            if (_wrapperMap.TryGetValue(database, out var pooledObject))
            {
                pooledObject?.Dispose();
            }

        }
        //public void ReturnCurrentDatabase()
        //{
        //    ReturnDatabase(GetCurrentDatabase());
        //}
        #endregion


        /////////////////////////////////////////////

        #region IDatabase Impl

        #region Property And Field
             
        public FluentExpressionSqlBuilder FluentSqlBuilder
        {
            get
            {
                return this.GetCurrentDatabase().FluentSqlBuilder;
            }
        } 
        public SqlBuilder SqlBuilder
        {
            get
            {
                return this.GetCurrentDatabase().SqlBuilder;
            }
        }

                
        public string DatabaseName { get { return this.GetCurrentDatabase().DatabaseName; } }
        public string ProviderName { get { return this.GetCurrentDatabase().ProviderName; } } 
        public DbProviderFactory DbFactory { get { return this.GetCurrentDatabase().DbFactory; } }

    
        public ISqlGenerator SqlGenerator
        {
            get
            {
                return this.GetCurrentDatabase().SqlGenerator;
            }
        }
         
        public ISqlDialectProvider SqlDialectProvider
        {
            get
            {
                return this.GetCurrentDatabase().SqlDialectProvider;
            }
        }


#if ASYNC 
        public IDapperAsyncImplementor DapperImplementor
        {
            get
            {
                return this.GetCurrentDatabase().DapperImplementor;
            }
        }
#else
        public IDapperImplementor DapperImplementor
        {
            get
            {
                return this.GetCurrentDatabase().DapperImplementor;
            }
        }
#endif

        public string ConnectionString { get { return this.GetCurrentDatabase().ConnectionString; } }
        public IDbConnection Connection { get { return this.GetCurrentDatabase().Connection; } }

        public bool HasActiveTransaction { get { return this.GetCurrentDatabase().HasActiveTransaction; } }
         
        /// <summary>
        /// 设置连接持续，如果true需要手动释放
        /// </summary>
        /// <param name="enable"></param>
        public void SetConnectionAlive(bool enable)
        {
            this.GetCurrentDatabase().SetConnectionAlive(enable);
        }

        public IDbTransaction Transaction { get { return this.GetCurrentDatabase().Transaction; } }
        public DatabaseType DatabaseType { get { return this.GetCurrentDatabase().DatabaseType; } }
        public Stopwatch Watch { get { return this.GetCurrentDatabase().Watch; } }
        public string LastSQL { get { return this.GetCurrentDatabase().LastSQL; } }
        public IDataParameterCollection LastArgs { get { return this.GetCurrentDatabase().LastArgs; } }

        
        #endregion

        #region Config
         
        /// <summary>
        /// 日志输出助手
        /// </summary>
        public LogHelper LogHelper
        {
            get
            {
                return this.GetCurrentDatabase().LogHelper;
            }
        }
          
        public IDatabaseConfig Config
        {
            get
            {
                return this.GetCurrentDatabase().Config;
            }


        }
          
        #endregion

   

        #region Execute


        public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().Execute(sql, param, transaction, commandTimeout, commandType);

        }
        public object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        { 
            return this.GetCurrentDatabase().Execute(sql, param, transaction, commandTimeout, commandType);

        }
        public T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);

        }
        public IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteReader(sql, param, transaction, commandTimeout, commandType);

        }
        public List<T> ExecuteList<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        { 
            return this.GetCurrentDatabase().ExecuteList<T>(sql, param, transaction, buffer, commandTimeout, commandType);

        }
        public List<T> ExecuteListWithRowDelegate<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteListWithRowDelegate<T>(sql, param, transaction,   commandTimeout, commandType);

        }
        public List<T> ExecuteListByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteListByEmit<T>(sql, param, transaction, commandTimeout, commandType);

        }

        public T ExecuteModel<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteModel<T>(sql, param, transaction, commandTimeout, commandType);

        }
        public T ExecuteModelByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteModelByEmit<T>(sql, param, transaction, commandTimeout, commandType);

        }
        public DataTable ExecuteDataTable(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteDataTable(sql, param, transaction, commandTimeout, commandType);

        }
        public DataTable ExecuteDataTableWithRowDelegate(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteDataTableWithRowDelegate(sql, param, transaction, commandTimeout, commandType);
        }
        public System.Data.DataSet ExecuteDataSet(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteDataSet(sql, param, transaction, commandTimeout, commandType);

        }

        public Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteDictionary<TKey, TValue>(sql, param, transaction, commandTimeout, commandType);

        }

        public dynamic ExecuteExpandoObject(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteExpandoObject(sql, param, transaction, commandTimeout, commandType);

        }

        public IEnumerable<dynamic> ExecuteExpandoObjects(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().ExecuteExpandoObjects(sql, param, transaction, commandTimeout, commandType); 
        }
        #endregion

        #region Query
        public IEnumerable<T> SqlQuery<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            return this.GetCurrentDatabase().SqlQuery<T>(sql, param, transaction, buffer, commandTimeout, commandType); 
        }
        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return this.GetCurrentDatabase().GetAll<TEntity>( );

        }
        public T Get<T>(dynamic id, IDbTransaction transaction, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().Get<T>(id,   transaction, commandTimeout );

        }

        public T Get<T>(dynamic id, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().Get<T>(id,  commandTimeout);

        }


        public IEnumerable<T> Query<T>(object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            return this.GetCurrentDatabase().Query<T>(predicate, sort, transaction, commandTimeout, buffered); 
        }

        public IEnumerable<T> Query<T>(object predicate, IList<ISort> sort, int? commandTimeout = null, bool buffered = true) where T : class
        {
            return this.GetCurrentDatabase().Query<T>(predicate, sort,  commandTimeout, buffered);

        }

        public IDataReader QueryByKV<T>(string[] columns, IDictionary<string, object> conditions, IList<ISort> sort) where T : class
        {
            return this.GetCurrentDatabase().QueryByKV<T>(columns, conditions, sort);

        }


        public int CountByKV<T>(IDictionary<string, object> conditions) where T : class
        {
            return this.GetCurrentDatabase().CountByKV<T>(conditions);

        }

        public long LongCountByKV<T>(IDictionary<string, object> conditions) where T : class
        {
            return this.GetCurrentDatabase().LongCountByKV<T>(conditions);

        }
        public int Count<T>() where T : class
        {
            return this.GetCurrentDatabase().Count<T>( );

        }

        public long LongCount<T>() where T : class
        {
            return this.GetCurrentDatabase().LongCount<T>();

        }
        public int Count<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().Count<T>(predicate, transaction, commandTimeout);

        }

        public int Count<T>(object predicate, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().Count<T>(predicate, commandTimeout);

        }


        public long LongCount<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().LongCount<T>(predicate, transaction, commandTimeout);

        }

        public long LongCount<T>(object predicate, int? commandTimeout = null) where T : class
        {
            return this.GetCurrentDatabase().LongCount<T>(predicate, commandTimeout);

        }
        public bool Exists<T>(object predicate) where T : class
        {
            return this.GetCurrentDatabase().Exists<T>(predicate ); 
        }
        #endregion



        //#region Page

        //public string GetSqlOfPage(int pageIndex, int pagesize, string sql, IDictionary<string, object> parameters)
        //{
        //    if (parameters == null)
        //    {
        //        parameters = new Dictionary<string, object>();
        //    }
        //    return DapperImplementor.SqlGenerator.Configuration.Dialect.GetPagingSql(sql, pageIndex, pagesize, parameters);
        //}
        //public string GetSqlOfCount(string sql)
        //{
        //    return DapperImplementor.SqlGenerator.Configuration.Dialect.GetCountSql(sql);
        //}
        //public IDataReader GetPageReader(int pageIndex, int pagesize, string sql, string orderField, bool asc, IDictionary<string, object> parameters, out int total)
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    string orderType = "";
        //    if (asc)
        //    {
        //        orderType = "ASC";
        //    }
        //    else
        //    {
        //        orderType = "DESC";

        //    }
        //    string orderBy = !string.IsNullOrEmpty(orderField) ? " ORDER BY " + orderField + " " + orderType : "";
        //    string newSql = sql + orderBy;
        //    if (parameters == null)
        //    {
        //        parameters = new Dictionary<string, object>();
        //    }
        //    var str = GetSqlOfPage(pageIndex, pagesize, newSql, parameters);
        //    string strCount = GetSqlOfCount(sql);// "SELECT COUNT(1) FROM (" + sql + ") AS __SQLCOUNT__";
        //    total = Convert.ToInt32(ExecuteScalar(strCount, parameters));

        //    var data = ExecuteReader(str, parameters);
        //    return data;
        //}
        //public List<TEntity> GetPageBySQL<TEntity>(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount) where TEntity : class
        //{
        //    try
        //    {
        //        SetConnectionAlive(true);
        //        pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //        string strCount = GetSqlOfCount(sqltext);
        //        totalCount = Convert.ToInt32(ExecuteScalar(strCount, parameters));
        //        if (!string.IsNullOrEmpty(orderText))
        //        {
        //            sqltext = sqltext + " ORDER BY " + orderText;
        //        }
        //        if (parameters == null)
        //        {
        //            parameters = new Dictionary<string, object>();
        //        }
        //        string sqlPage = GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
        //        var data = SqlQuery<TEntity>(sqlPage, parameters);

        //        return data.ToList();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    finally
        //    {
        //        SetConnectionAlive(false);

        //        Close();
        //    }

        //}
        //public IDataReader GetPageReaderBySQL(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount)
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //    string strCount = GetSqlOfCount(sqltext);
        //    totalCount = Convert.ToInt32(ExecuteScalar(strCount, parameters));
        //    if (!string.IsNullOrEmpty(orderText))
        //    {
        //        sqltext = sqltext + " ORDER BY " + orderText;
        //    }
        //    if (parameters == null)
        //    {
        //        parameters = new Dictionary<string, object>();
        //    }
        //    string sqlPage = GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
        //    var data = ExecuteReader(sqlPage, parameters);
        //    return data;

        //}
        //public IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //    return DapperImplementor.GetSet<T>(Connection, predicate, sort, pageIndex, pagesize, transaction, commandTimeout, buffered);
        //}

        //public IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //    return DapperImplementor.GetSet<T>(Connection, predicate, sort, pageIndex, pagesize, _transaction, commandTimeout, buffered);
        //}
        //public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, transaction, commandTimeout, buffered);
        //}
        //public PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize, transaction, commandTimeout, buffered);
        //}
        //public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, _transaction, commandTimeout, buffered);
        //}

        //public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, out long allRowsCount, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //    transaction = transaction == null ? _transaction : transaction;

        //    return DapperImplementor.GetPage<T>(Connection, pageIndex, pagesize, out allRowsCount, sql, param, allRowsCountSql, transaction, commandTimeout, buffered);
        //}
        //public PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize, string sql, dynamic param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
        //    transaction = transaction == null ? _transaction : transaction;

        //    return DapperImplementor.GetPage<T>(Connection, pageIndex, pagesize, sql, param, allRowsCountSql, transaction, commandTimeout, buffered);
        //}
        //public IDataReader GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    return DapperImplementor.GetPageReader<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, _transaction, commandTimeout);
        //}

        //public PageDataResult<IDataReader> GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //    return DapperImplementor.GetPageReader<T>(Connection, predicate, sort, pageIndex, pagesize, _transaction, commandTimeout);
        //}
        //#endregion

        //#region Multi-Result
        //public IMultipleResultReader GetMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //{
        //    return new GridReaderResultReader(Connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType, this));
        //}
        //public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        //{
        //    return DapperImplementor.GetMultiple(Connection, predicate, transaction, commandTimeout);
        //}

        //public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout)
        //{
        //    return DapperImplementor.GetMultiple(Connection, predicate, _transaction, commandTimeout);
        //}
        //#endregion

        //#region Insert
        //public void InsertBulk(DataTable dt, IDbTransaction transaction = null, int? commandTimeout = null)
        //{
        //    BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).Insert(this, dt);
        //}
        //public void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).Insert(this, entities);
        //}
        ////public void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        ////{
        ////    if (!OnInsertingInternal(new InsertContext(entities)))
        ////        return;
        ////    if (SqlGenerator.Configuration.Dialect.databaseType == Data.DatabaseType.SqlServer)
        ////    {
        ////        SqlServerBatcher batch = new SqlServerBatcher();
        ////        batch.Insert(this, transaction, entities, GetMap<T>().TableName);
        ////    }
        ////    else
        ////    {
        ////        DapperImplementor.Insert<T>(Connection, entities, transaction, commandTimeout);

        ////    }

        ////}

        ////public void InsertBulk<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class
        ////{

        ////    InsertBulk<T>(entities,  null, commandTimeout);

        ////}
        //public void InsertBatch(DataTable dt, int batchSize = 10000)
        //{
        //    //  options = options ?? new BatchOptions();

        //    BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).InsertBatch(this, dt, batchSize);

        //}
        //public void InsertBatch<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int batchSize = 10000, int? commandTimeout = null) where T : class
        //{
        //    if (!OnInsertingInternal(new InsertContext(entities)))
        //        return;

        //    //options = options ?? new BatchOptions();

        //    BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).InsertBatch<T>(this, entities, batchSize);


        //    //System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    //string sql = "";
        //    //int result = 0;
        //    //object p = null;
        //    //foreach (var chunks in entities.Chunkify(options.BatchSize))
        //    //{
        //    //    sb.Clear();
        //    //    //var preparedInserts = chunks.Select(x => new { sql = (_dapper.PrepareInsertStament(x, out p)), parameters = p }).ToArray();
        //    //    var preparedInserts = chunks.Select(x => DapperImplementor.PrepareInsertStament(x, out p)).ToArray();

        //    //    //var preparedInserts = chunks.Select(x => new { sql = ExpressionSqlBuilder.Insert<T>(() => new { x }).ToSqlString() }).ToArray();

        //    //    foreach (var preparedInsertSql in preparedInserts)
        //    //    {
        //    //        sb.Append(preparedInsertSql);
        //    //        sb.Append(options.StatementSeperator);
        //    //    }
        //    //    sql = sb.ToString();
        //    //    if (!string.IsNullOrEmpty(sql))
        //    //    {
        //    //        result += this.Execute(sql, null, transaction);
        //    //    }
        //    //    //  InsertBulk<T>(chunks.AsEnumerable(), transaction, null);

        //    //}
        //    //return result;
        //}

        //public void InsertBatch<T>(IEnumerable<T> entities, int batchSize = 10000, int? commandTimeout = null) where T : class
        //{
        //    //if (!OnInsertingInternal(new InsertContext(entities)))
        //    //    return 0;
        //    InsertBatch<T>(entities, null, batchSize);
        //}
        //public dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class
        //{
        //    if (!OnInsertingInternal(new InsertContext(entity)))
        //        return 0;
        //    return DapperImplementor.Insert<T>(Connection, entity, transaction, commandTimeout);
        //}

        //public dynamic Insert<T>(T entity, int? commandTimeout = null) where T : class
        //{
        //    if (!OnInsertingInternal(new InsertContext(entity)))
        //        return 0;

        //    return DapperImplementor.Insert<T>(Connection, entity, _transaction, commandTimeout);
        //}

        //public int InsertByKV<T>(IDictionary<string, object> parameters) where T : class
        //{

        //    var classMap = GetMap<T>();

        //    return this.Insert(classMap.TableName, parameters);
        //}

        //#endregion

        //#region Update
        //public bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class
        //{
        //    if (!OnUpdatingInternal(new UpdateContext(entity)))
        //        return false;
        //    return DapperImplementor.Update<T>(Connection, entity, transaction, commandTimeout);
        //}

        //public bool Update<T>(T entity, int? commandTimeout = null) where T : class
        //{
        //    if (!OnUpdatingInternal(new UpdateContext(entity)))
        //        return false;
        //    return DapperImplementor.Update<T>(Connection, entity, _transaction, commandTimeout);
        //}

        //public int UpdateByKV<T>(IDictionary<string, object> parameters, IDictionary<string, object> conditions) where T : class
        //{

        //    var classMap = GetMap<T>();

        //    return this.Update(classMap.TableName, conditions, parameters);
        //}

        //public int UpdateBySQL(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = null)
        //{

        //    if (!OnUpdatingInternal(new UpdateContext(sql)))
        //        return 0;
        //    transaction = transaction ?? _transaction;
        //    return DapperImplementor.Update(Connection, sql, parameters, transaction, commandTimeout);
        //}
        //#endregion

        //#region Delete
        //public bool Delete<T>(T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(entity)))
        //        return false;
        //    return DapperImplementor.Delete(Connection, entity, transaction, commandTimeout);
        //}

        //public bool Delete<T>(T entity, int? commandTimeout = null) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(entity)))
        //        return false;
        //    return DapperImplementor.Delete(Connection, entity, _transaction, commandTimeout);
        //}

        //public bool Delete<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(predicate)))
        //        return false;
        //    return DapperImplementor.Delete<T>(Connection, predicate, transaction, commandTimeout);
        //}

        //public bool Delete<T>(object predicate, int? commandTimeout) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(predicate)))
        //        return false;
        //    return DapperImplementor.Delete<T>(Connection, predicate, _transaction, commandTimeout);
        //}

        //public bool DeleteByIds<T>(string cName, string ids, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(new { cName = cName, ids = ids })))
        //        return false;
        //    transaction = transaction ?? _transaction;
        //    return DapperImplementor.DeleteByIds<T>(Connection, cName, ids, transaction, commandTimeout);
        //}

        //public bool DeleteById<T>(dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (!OnDeletingInternal(new DeleteContext(id)))
        //        return false;
        //    transaction = transaction ?? _transaction;
        //    return DapperImplementor.DeleteById<T>(Connection, id, _transaction, commandTimeout);
        //}

        //public int DeleteByKV<T>(IDictionary<string, object> conditions) where T : class
        //{

        //    var classMap = GetMap<T>();

        //    return this.Delete(classMap.TableName, conditions);
        //}
        //public int DeleteAll<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{

        //    transaction = transaction ?? _transaction;
        //    return DapperImplementor.DeleteAll<T>(Connection, _transaction, commandTimeout);
        //}
        //public int Truncate<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    transaction = transaction ?? _transaction;
        //    return DapperImplementor.Truncate<T>(Connection, _transaction, commandTimeout);
        //}
        //#endregion

        #region Transaction 
        public ITransaction GetTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return this.GetCurrentDatabase().GetTransaction(isolationLevel);
        }

        public void SetTransaction(IDbTransaction tran)
        {
              this.GetCurrentDatabase().SetTransaction(tran);

        }
 
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            this.GetCurrentDatabase().BeginTransaction(isolationLevel);

        }

        public void CommitTransaction()
        {
            this.GetCurrentDatabase().CommitTransaction();

        }

        public void RollbackTransaction()
        {
            this.GetCurrentDatabase().RollbackTransaction();

        }
       


        public void RunInTransaction(Action action)
        {
            this.GetCurrentDatabase().RunInTransaction(action);

        }

        public T RunInTransaction<T>(Func<T> func)
        {
           return  this.GetCurrentDatabase().RunInTransaction<T>(func); 
        }

        #endregion

        #region Dispose & Open & Close
        public void Dispose()
        {
            Pool.Clear();
        }
         
        public void Close()
        {
            var currentDatabase = this.GetCurrentDatabase();
            this.ReturnPooledDatabase(currentDatabase);
            currentDatabase.Close();
        }

        
        public void EnsureOpenConnection()
        {
            this.GetCurrentDatabase().EnsureOpenConnection();
        }

        // Open a connection (can be nested)
        public IDatabase OpenSharedConnection()
        {
            return this.GetCurrentDatabase().OpenSharedConnection(); 
        }
         
        public IDatabase OpenNewConnection()
        {
            return this.GetCurrentDatabase().OpenNewConnection();

        }
          
        #endregion

        #region Common

        public void ClearMap()
        {
              this.GetCurrentDatabase().ClearMap();

        }
        public DynamicParameters NewDynamicParameters()
        {
            return this.GetCurrentDatabase().NewDynamicParameters();

        }
        public Guid GetNextGuid()
        {
            return this.GetCurrentDatabase().GetNextGuid();

        }
        public IClassMapper GetMap(string tableName)
        {
            return this.GetCurrentDatabase().GetMap(tableName);

        }
        public IClassMapper GetMap<T>() where T : class
        {
            return this.GetCurrentDatabase().GetMap<T>();

        }
        public string GetColumnString<T>(string prefix = "T.", string spliteStr = ", ", params Expression<Func<T, object>>[] ignoreProperties) where T : class
        {
            return this.GetCurrentDatabase().GetColumnString<T>(prefix, spliteStr, ignoreProperties);

        }
        public IClassMapper GetMap(Type type)
        {
            return this.GetCurrentDatabase().GetMap(type);

        }
        public ValidationResult Validate<T>(T instance) where T : class
        {
            return this.GetCurrentDatabase().Validate<T>(instance);

        }
        public ValidationResult Validate<T>(T instance, params Expression<Func<T, object>>[] propertyExpressions) where T : class
        {
            return this.GetCurrentDatabase().Validate<T>(instance, propertyExpressions);

        }
        public ValidationResult Validate<T>(T instance, params string[] properties) where T : class
        {
            return this.GetCurrentDatabase().Validate<T>(instance, properties);

        }
        public void ValidateAndThrow<T>(T instance) where T : class
        {
              this.GetCurrentDatabase().Validate<T>(instance);

        }
        public void LoadAllMap(List<Assembly> MappingAssemblies = null, LoadMapperMode LoadMode = LoadMapperMode.FluentMapper)
        {
            this.GetCurrentDatabase().LoadAllMap(MappingAssemblies, LoadMode);

        }
        public System.Collections.Concurrent.ConcurrentDictionary<Type, IClassMapper> GetAllMap()
        {
            return this.GetCurrentDatabase().GetAllMap( );

        }
        #endregion

        //        #region Expression
        //        public void EnsureAddClassToTableMap<TEntity>() where TEntity : class
        //        {
        //            if (FluentSqlBuilder.TableMapperContainer.GetTable<TEntity>() == null)
        //            {
        //                IClassMapper map = GetMap<TEntity>();
        //                FluentSqlBuilder.TableMapperContainer.Add(map.EntityType, new TableMap<TEntity>(map.TableName));
        //            }
        //        }

        //        public IDataReader ExecuteReader<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        //        {
        //            return this.ExecuteReader(expression.ToSqlString());
        //        }
        //        public IEnumerable<TEntity> ExecuteQuery<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        //        {

        //            return this.SqlQuery<TEntity>(expression.ToSqlString());
        //        }
        //        public TValue ExecuteScalar<TEntity, TValue>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        //        {
        //            return this.ExecuteScalar<TValue>(expression.ToSqlString());
        //        }
        //        public int Execute<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        //        {
        //            return this.Execute(expression.ToSqlString());
        //        }
        //        public virtual int Insert<TEntity>(Expression<Func<object>> body) where TEntity : class
        //        {
        //            if (!OnInsertingInternal(new InsertContext(body)))
        //                return 0;
        //            EnsureAddClassToTableMap<TEntity>();
        //            return this.Execute<TEntity>(FluentSqlBuilder.Insert<TEntity>(body));
        //        }
        //        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            if (!OnDeletingInternal(new DeleteContext(condition)))
        //                return 0;
        //            EnsureAddClassToTableMap<TEntity>();
        //            return this.Execute<TEntity>(FluentSqlBuilder.Delete<TEntity>().Where(condition));
        //        }
        //        public virtual int Update<TEntity>(Expression<Func<object>> body, Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            if (!OnUpdatingInternal(new UpdateContext(body, condition)))
        //                return 0;
        //            EnsureAddClassToTableMap<TEntity>();
        //            var count = this.Execute<TEntity>(FluentSqlBuilder.Update<TEntity>(body).Where(condition));


        //            return count;
        //        }


        //        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            return Query<TEntity>(condition).FirstOrDefault();
        //        }
        //        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            string sql = FluentSqlBuilder.Select<TEntity>().Where(condition).ToSqlString();

        //            return this.SqlQuery<TEntity>(sql);
        //        }
        //        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            var expression = FluentSqlBuilder.Select<TEntity>().Where(condition);
        //            if (orderAction != null)
        //            {
        //                orderAction(expression);
        //            }

        //            return this.ExecuteQuery<TEntity>(expression);
        //        }
        //        public IEnumerable<TEntity> QueryByWhere<TEntity>(string condition, string orderStr) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            return this.ExecuteQuery<TEntity>(FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr));
        //        }
        //        public IEnumerable<TEntity> QueryBySQL<TEntity>(string sql) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            return this.SqlQuery<TEntity>(sql);
        //        }

        //        public IEnumerable<TEntity> GetPageByWhere<TEntity>(int pageIndex, int pagesize, string condition, string orderStr, out int totalCount) where TEntity : class
        //        {
        //            try
        //            {
        //                SetConnectionAlive(true);
        //                EnsureAddClassToTableMap<TEntity>();
        //                pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //                totalCount = this.Count<TEntity>(condition);

        //                return this.ExecuteQuery<TEntity>(FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr).TakePage(pageIndex, pagesize));
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            finally
        //            {
        //                SetConnectionAlive(false);
        //                Close();
        //            }

        //        }

        //        public IEnumerable<TEntity> GetPage<TEntity>(int pageIndex, int pagesize, Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction, out int totalCount) where TEntity : class
        //        {

        //            try
        //            {

        //                EnsureAddClassToTableMap<TEntity>();
        //                pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

        //                var expression = FluentSqlBuilder.Select<TEntity>().Where(condition);
        //                if (orderAction != null)
        //                {
        //                    orderAction(expression);
        //                }
        //                totalCount = this.Count<TEntity>(condition);
        //                return this.ExecuteQuery<TEntity>(expression.TakePage(pageIndex, pagesize));
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }
        //            finally
        //            {
        //                SetConnectionAlive(false);
        //                Close();
        //            }


        //        }

        //        public int Count<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            var expression = FluentSqlBuilder.Count<TEntity>().Where(condition);
        //            return ExecuteScalar<TEntity, int>(expression);
        //        }
        //        public long LongCount<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            var expression = FluentSqlBuilder.Count<TEntity>().Where(condition);
        //            return ExecuteScalar<TEntity, long>(expression);
        //        }
        //        private int Count<TEntity>(string condition) where TEntity : class
        //        {
        //            EnsureAddClassToTableMap<TEntity>();
        //            return this.ExecuteScalar<TEntity, int>(FluentSqlBuilder.Count<TEntity>().Where(condition));
        //        }
        //        public int Count(string sql)
        //        {
        //            return this.ExecuteScalar<int>(sql);
        //        }
        //        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        //        {
        //            return Count<TEntity>(condition) > 0;
        //        }
        //        #endregion

        //        #region Intercept

        //        #region TransactionInterceptor
        //        private void OnBeginTransactionInternal()
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
        //                {
        //                    interceptor.OnBeginTransaction(this);
        //                }
        //            }

        //        }


        //        private void OnAbortTransactionInternal()
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
        //                {
        //                    interceptor.OnAbortTransaction(this);
        //                }
        //            }
        //        }

        //        private void OnCompleteTransactionInternal()
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
        //                {
        //                    interceptor.OnCompleteTransaction(this);
        //                }
        //            }
        //        }

        //        #endregion

        //        #region ConnectionInterceptor
        //        public void OnDatabaseConnectionClosing()
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
        //                {
        //                    interceptor.OnConnectionClosing(this, this.Connection);
        //                }
        //            }
        //        }
        //        public void OnDatabaseException(Exception ex)
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<IExceptionInterceptor>())
        //                {
        //                    interceptor.OnException(this, ex);
        //                }
        //            }
        //        }
        //        private IDbConnection OnConnectionOpenedInternal(IDbConnection conn)
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
        //                {
        //                    conn = interceptor.OnConnectionOpened(this, conn);
        //                }
        //            }
        //            return conn;
        //        }

        //        private void OnConnectionClosingInternal(IDbConnection conn)
        //        {

        //            if (Config.EnableIntercept)
        //            {
        //                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
        //                {
        //                    interceptor.OnConnectionClosing(this, conn);
        //                }
        //            }
        //        }
        //        #endregion

        //        private bool OnInsertingInternal(InsertContext insertContext)
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnInserting(this, insertContext));
        //            }
        //            return true;
        //        }
        //        private bool OnUpdatingInternal(UpdateContext updateContext)
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnUpdating(this, updateContext));
        //            }
        //            return true;
        //        }
        //        private bool OnDeletingInternal(DeleteContext deleteContext)
        //        {
        //            if (Config.EnableIntercept)
        //            {
        //                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnDeleting(this, deleteContext));
        //            }
        //            return true;
        //        }
        //        #endregion


        //        #region 异步操作
        //#if ASYNC

        //        //public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //        //{
        //        //    if (transaction == null)
        //        //    {
        //        //        transaction = Transaction;
        //        //    }
        //        //    if (commandTimeout == null || !commandTimeout.HasValue)
        //        //    {
        //        //        commandTimeout = Config.ExecuteTimeout;
        //        //    }
        //        //    return Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType, this);
        //        //}
        //        //public Task<object> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //        //{
        //        //    if (transaction == null)
        //        //    {
        //        //        transaction = Transaction;
        //        //    }
        //        //    if (commandTimeout == null || !commandTimeout.HasValue)
        //        //    {
        //        //        commandTimeout = Config.ExecuteTimeout;
        //        //    }
        //        //    return Connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType, this);
        //        //}
        //        //public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //        //{
        //        //    if (transaction == null)
        //        //    {
        //        //        transaction = Transaction;
        //        //    }
        //        //    if (commandTimeout == null || !commandTimeout.HasValue)
        //        //    {
        //        //        commandTimeout = Config.ExecuteTimeout;
        //        //    }
        //        //    return Connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType, this);
        //        //}
        //        //public Task<IDataReader> ExecuteReaderAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        //        //{
        //        //    if (transaction == null)
        //        //    {
        //        //        transaction = Transaction;
        //        //    }
        //        //    if (commandTimeout == null || !commandTimeout.HasValue)
        //        //    {
        //        //        commandTimeout = Config.ExecuteTimeout;
        //        //    }
        //        //    return Connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType, this);
        //        //}


        //        // /// <summary>
        //        ///// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        //        ///// </summary>
        //        //public Task<int> CountAsync<T>(object predicate = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //        //{
        //        //    return DapperImplementor.CountAsync<T>(Connection, predicate, transaction, commandTimeout);
        //        //}

        //        ///// <summary>
        //        ///// Executes a query for the specified id, returning the data typed as per T.
        //        ///// </summary>
        //        //public Task<T> GetAsync<T>(dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //        //{
        //        //    return DapperImplementor.GetAsync<T>(Connection, id, transaction, commandTimeout);
        //        //}

        //        ///// <summary>
        //        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        //        ///// </summary>
        //        //public Task<IEnumerable<T>> GetListAsync<T>(  object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //        //{
        //        //    return DapperImplementor.GetListAsync<T>(Connection, predicate, sort, transaction, commandTimeout);
        //        //}

        //        ///// <summary>
        //        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        //        ///// Data returned is dependent upon the specified page and resultsPerPage.
        //        ///// </summary>
        //        //public Task<IEnumerable<T>> GetPageDataAsync<T>(  object predicate = null, IList<ISort> sort = null, int page = 1, int resultsPerPage = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //        //{
        //        //    return DapperImplementor.GetPageDataAsync<T>(Connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout);
        //        //}


        //        ///// <summary>
        //        ///// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        //        ///// Data returned is dependent upon the specified firstResult and maxResults.
        //        ///// </summary>
        //        //public Task<IEnumerable<T>> GetSetAsync<T>( object predicate = null, IList<ISort> sort = null, int firstResult = 1, int maxResults = 10, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //        //{
        //        //    return DapperImplementor.GetSetAsync<T>(Connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout);
        //        //}


        //        //public Task<IEnumerable<T>> SqlQueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        //        //{
        //        //    if (transaction == null)
        //        //    {
        //        //        transaction = Transaction;
        //        //    }
        //        //    return Connection.QueryAsync<T>( sql, param, transaction, commandTimeout, commandType, this);

        //        //}



        //#endif
        //        #endregion

        //        #region Batch Command
        //        /// <summary>
        //        /// 创建批处理命令
        //        /// </summary>
        //        /// <param name="batchSize"></param>
        //        /// <param name="tran"></param>
        //        /// <returns></returns>
        //        public BatchCommander NewBatchCommand(BatchOptions option, IDbTransaction tran = null)
        //        {
        //            if (option == null)
        //            {
        //                option = new BatchOptions();
        //            }
        //            if (option.BatchSize < 1)
        //            {
        //                throw new ArgumentException("batchSize must larger than 0!");
        //            }
        //            if (tran == null)
        //            {
        //                tran = this.Transaction;
        //            }
        //            return new BatchCommander(this, option, tran);
        //        }
        //        #endregion

        //        #region SqlMap
        //        public SqlMap.SqlMapStatement QuerySqlMap(string scope, string sqlID, object param = null)
        //        {
        //            return Pure.Data.SqlMap.SqlMapManager.Instance.BuildSqlMapResult(this, scope, sqlID, param);
        //        }
        //        #endregion

        //        #region Track
        //        public Snapshot<T> Track<T>(T obj) where T : class
        //        {
        //            return Snapshotter.Track(this, obj);
        //        }
        //        #endregion

        //        #region Ado.Ext
        //        public int Insert(string tableName, IDictionary<string, object> parameters)
        //        {
        //            if (!OnInsertingInternal(new InsertContext(new { tableName = tableName, parameters = parameters })))
        //                return 0;
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Insert(tableName, parameters, out realParameters);

        //            return this.Execute(sql, realParameters);
        //        }
        //        public int Update(string tableName, IDictionary<string, object> parameters, IDictionary<string, object> conditions)
        //        {
        //            if (!OnUpdatingInternal(new UpdateContext(new { tableName = tableName, parameters = parameters, conditions = conditions })))
        //                return 0;
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Update(tableName, parameters, conditions, out realParameters);

        //            return this.Execute(sql, realParameters);
        //        }

        //        public int Delete(string tableName, IDictionary<string, object> conditions)
        //        {
        //            if (!OnDeletingInternal(new DeleteContext(new { tableName = tableName, conditions = conditions })))
        //                return 0;
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Delete(tableName, conditions, out realParameters);

        //            return this.Execute(sql, realParameters);
        //        }

        //        public IDataReader Query(string tableName, string[] columns, IDictionary<string, object> conditions, IList<ISort> sort)
        //        {
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Select(tableName, columns, conditions, sort, out realParameters);

        //            return this.ExecuteReader(sql, realParameters);
        //        }

        //        public int Count(string tableName, IDictionary<string, object> conditions)
        //        {
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

        //            return this.ExecuteScalar<int>(sql, realParameters);
        //        }

        //        public long LongCount(string tableName, IDictionary<string, object> conditions)
        //        {
        //            IDictionary<string, object> realParameters = new Dictionary<string, object>();
        //            string sql = SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

        //            return this.ExecuteScalar<long>(sql, realParameters);
        //        }
        //        #endregion


        //        #region Multi-Relations
        //        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);

        //        }
        //        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, T3, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);

        //        }
        //        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, T3, T4, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        //        }

        //        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, T3, T4, T5, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        //        }

        //        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, TRet>(Func<T1, T2, T3, T4, T5, T6, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, T3, T4, T5, T6, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        //        }

        //        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, T7, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        //        {
        //            return Connection.Query<T1, T2, T3, T4, T5, T6, T7, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        //        }
        //        #endregion

        //        #region log
        //        public void Debug(string msg)
        //        {
        //            LogHelper.Debug(msg);
        //        }
        //        public void Error(Exception msg)
        //        {
        //            LogHelper.Error(msg);
        //        }
        //        public void Warning(string msg)
        //        {
        //            LogHelper.Warning(msg);
        //        }
        //        #endregion

        //        #region Queryable
        //        public SqlQuery<T> Queryable<T>() where T : class
        //        {
        //            return new SqlQuery<T>(this.FluentSqlBuilder);
        //        }

        //        #endregion

        //        #region Backup & Sql Gen & Migrate
        //        public string Backup<T>(BackupOption option) where T : class
        //        {
        //            return BackupHelper.Instance.Backup<T>(this, option);
        //        }

        //        public string GenerateCode()
        //        {
        //            return CodeGenHelper.Instance.Gen(this);
        //        }

        //        public Migration.Framework.ITransformationProvider CreateTransformationProvider()
        //        {
        //            return DbMigrateService.Instance.CreateTransformationProviderByDatabaseType(this);
        //        }
        //        #endregion

        //        #region Share 分表分库

        //        #endregion

        //        #region UpdateOnly
        //        /// <summary>
        //        /// 更新对象指定列的内容
        //        /// </summary>
        //        /// <param name="bindedObj"></param>
        //        /// <param name="_PrimaryKeyValues"></param>
        //        /// <param name="onlyProperties"></param>
        //        /// <returns></returns>
        //        public int UpdateWithOnlyParams<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class
        //        {
        //            List<string> onlyList = ReflectionHelper.GetAllPropertyNames(onlyProperties);//  new List<string>();

        //            return UpdateOnly<T>(bindedObj, _PrimaryKeyValues, onlyList.ToArray());
        //        }

        //        /// <summary>
        //        /// 更新对象指定列的内容
        //        /// </summary>
        //        /// <param name="_PrimaryKeyValues"></param>
        //        /// <param name="bindedObj"></param>
        //        /// <param name="onlyProperties"></param>
        //        /// <returns></returns>
        //        public int UpdateOnly<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        //        {
        //            var sqlParamsContext = UpdateOnlyImpl<T>(bindedObj, _PrimaryKeyValues, onlyProperties);
        //            return this.Execute(sqlParamsContext.Sql, sqlParamsContext.Parameters);

        //        }

        //        /// <summary>
        //        /// 更新对象指定列的内容
        //        /// </summary>
        //        /// <param name="_PrimaryKeyValues"></param>
        //        /// <param name="bindedObj"></param>
        //        /// <param name="onlyProperties"></param>
        //        /// <returns></returns>
        //        internal ExecuteSqlParamsContext UpdateOnlyImpl<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        //        {
        //            if (bindedObj == null)
        //            {
        //                throw new ArgumentException("bindedObj 不能为空！");
        //            }
        //            var pd = GetMap<T>();// pocoData;

        //            var columns = onlyProperties;
        //            object poco = bindedObj;// trackedObject;
        //            //int defaultValue = 0;
        //            if (columns != null && !columns.Any())
        //                throw new ArgumentException("onlyProperties 不能为空！");

        //            var sb = new StringBuilder();
        //            var index = 0;
        //            string paramPrefix = "";
        //            var pkCols = pd.Properties.Where(p => p.IsPrimaryKey).ToList();
        //            IDictionary<string, object> PrimaryKeyValues = new Dictionary<string, object>();
        //            if (_PrimaryKeyValues != null && _PrimaryKeyValues.Count > 0)
        //            {
        //                PrimaryKeyValues = _PrimaryKeyValues;
        //            }
        //            else
        //            {
        //                if (pkCols == null || pkCols.Count == 0)
        //                {
        //                    throw new ArgumentException("当前对象没有主键情况下，需要指定更新的主键值");
        //                }
        //                else
        //                {

        //                    var Properties = pd.Properties.Select(p => p.PropertyInfo);// poco.GetType().GetProperties();
        //                    foreach (var pk in pkCols)
        //                    {
        //                        string pName = pk.Name;
        //                        string pColumnName = pk.Name;//  pk.ColumnName;
        //                        var o = Properties.FirstOrDefault(y => string.Equals(pName, y.Name, StringComparison.OrdinalIgnoreCase));
        //                        if (o != null)
        //                        {
        //                            var pValue = o.GetValue(poco, null);
        //                            PrimaryKeyValues.Add(pColumnName, pValue);

        //                        }

        //                    }
        //                }
        //            }


        //            IDictionary<string, object> parameters = new Dictionary<string, object>();


        //            var waitToUpdated = pd.Properties.Where(p => columns.Contains(p.ColumnName) || p.IsVersionColumn == true);
        //            foreach (var pocoColumn in waitToUpdated)
        //            {
        //                string properyName = pocoColumn.Name;//pocoColumn.ColumnName;

        //                // Don't update the primary key, but grab the value if we don't have it
        //                if (_PrimaryKeyValues == null && PrimaryKeyValues.ContainsKey(properyName))
        //                {
        //                    continue;
        //                }

        //                // Dont update result only columns
        //                if (pocoColumn.Ignored || pocoColumn.IsReadOnly || pocoColumn.IsPrimaryKey)
        //                    continue;

        //                var colType = pocoColumn.PropertyInfo.PropertyType;

        //                object value = pocoColumn.PropertyInfo.GetValue(poco, null);// GetColumnValue(pd, poco, ProcessMapper);
        //                if (pocoColumn.IsVersionColumn)
        //                {
        //                    if (colType == typeof(int) || colType == typeof(Int32))
        //                    {
        //                        value = Convert.ToInt32(value) + 1;


        //                    }
        //                    else if (colType == typeof(long) || colType == typeof(Int64))
        //                    {
        //                        value = Convert.ToInt64(value) + 1;


        //                    }
        //                    else if (colType == typeof(short) || colType == typeof(Int16))
        //                    {
        //                        value = Convert.ToInt16(value) + 1;

        //                    }
        //                }
        //                //else
        //                //{
        //                //    if (Snapshotter.GetAutoFilterEmptyValueColumnsWhenTrack())
        //                //    {
        //                //        var defaultValueOfCol = ReflectionHelper.GetDefaultValueForType(colType);
        //                //        if (object.Equals(defaultValueOfCol, value))//过滤空值列
        //                //        {
        //                //            continue;
        //                //        }
        //                //    }
        //                //}



        //                // Build the sql
        //                if (index > 0)
        //                    sb.Append(", ");

        //                //string paramName = paramPrefix + index.ToString();// pocoColumn.Name;
        //                string paramName = paramPrefix + properyName;
        //                sb.AppendFormat("{0} = {1}{2}", this.SqlGenerator.GetColumnName(pd, pocoColumn, false), this.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramName);

        //                parameters.Add(paramName, value);
        //                index++;

        //            }


        //            if (columns != null && columns.Any() && sb.Length == 0)
        //                throw new ArgumentException("There were no columns in the columns list that matched your table", "columns");

        //            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", this.SqlGenerator.GetTableName(pd), sb, BuildWhereSql(this, pd, PrimaryKeyValues, paramPrefix, ref index));
        //            int temIndex = parameters.Count;
        //            foreach (var item in PrimaryKeyValues)
        //            {
        //                parameters.Add(paramPrefix + item.Key, item.Value);
        //                //parameters.Add(paramPrefix + temIndex, item.Value);
        //                temIndex++;
        //            }





        //            if (this.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
        //            {
        //                IDictionary<string, object> dictParameters = parameters;
        //                int convertCount = LobConverter.UpdateDynamicParameterForLobColumn(pd, dictParameters);
        //                if (convertCount > 0)
        //                {
        //                    //return this.Execute(sql, dictParameters);
        //                    return new ExecuteSqlParamsContext(sql, dictParameters);
        //                }
        //                else
        //                {
        //                    //执行原始SQL
        //                    //return this.Execute(sql, parameters);
        //                    return new ExecuteSqlParamsContext(sql, parameters);

        //                }
        //            }
        //            else
        //            {
        //                //执行原始SQL
        //                //return this.Execute(sql, parameters);
        //                return new ExecuteSqlParamsContext(sql, parameters);

        //            }


        //        }


        //#if ASYNC
        //        /// <summary>
        //        /// 更新对象指定列的内容
        //        /// </summary>
        //        /// <param name="bindedObj"></param>
        //        /// <param name="_PrimaryKeyValues"></param>
        //        /// <param name="onlyProperties"></param>
        //        /// <returns></returns>
        //        public async Task<int> UpdateWithOnlyParamsAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class
        //        {
        //            List<string> ignoreList = ReflectionHelper.GetAllPropertyNames(onlyProperties);//  new List<string>();

        //            return await UpdateOnlyAsync<T>(bindedObj, _PrimaryKeyValues, ignoreList.ToArray());
        //        }
        //        /// <summary>
        //        /// 更新对象变更的内容
        //        /// </summary>
        //        /// <param name="_PrimaryKeyValues"></param>
        //        /// <param name="bindedObj"></param>
        //        /// <param name="onlyProperties"></param>
        //        /// <returns></returns>
        //        public async Task<int> UpdateOnlyAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        //        {
        //            var sqlParamsContext = UpdateOnlyImpl<T>(bindedObj, _PrimaryKeyValues, onlyProperties);
        //            return await this.ExecuteAsync(sqlParamsContext.Sql, sqlParamsContext.Parameters);
        //        }

        //#endif

        //        private string BuildWhereSql(IDatabase db, IClassMapper pd, IDictionary<string, object> primaryKeyValuePair, string paramPrefix, ref int index)
        //        {
        //            var tempIndex = index;
        //            //return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) : string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false), db.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramPrefix + (tempIndex + i).ToString())).ToArray());

        //            return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ?
        //            string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) :
        //            string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false),
        //            db.SqlGenerator.Configuration.Dialect.ParameterPrefix,
        //            paramPrefix + x.Key
        //            )
        //            ).ToArray());

        //        }


        //        #endregion


        #endregion



    }

}
