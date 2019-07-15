using Dapper;
using System.Data;
using System.Data.Common;
using FluentExpressionSQL;
using System.Collections.Generic;
using System.Reflection;
using Pure.Data.Sql;
using System;
using System.Linq.Expressions;
using System.Linq;
using FluentExpressionSQL.Mapper;
using System.Diagnostics;
using Pure.Data.Migration;
using Pure.Data.Validations.Results;
using FluentExpressionSQL.Sql;
using System.Text;

#if ASYNC
using System.Threading.Tasks;
#endif
namespace Pure.Data
{

    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class Database : IDatabase
    {

        #region Property And Field

        private FluentExpressionSqlBuilder _FluentSqlBuilder = null; 
        public FluentExpressionSqlBuilder FluentSqlBuilder
        {
            get
            {
                if (_FluentSqlBuilder == null)
                {
                    lock (DatabaseConfigPool._DatabaseFluentSqlBuilderLock)
                    {
                        if (_FluentSqlBuilder == null)
                        {
                            _FluentSqlBuilder = FluentExpressionSqlBuilderBoostraper.Instance.Load(this);
                            return _FluentSqlBuilder;
                        }
                    }
                }
                return _FluentSqlBuilder;

                //return expressionSqlBuilder; 
            }
        }
        private SqlBuilder _SqlBuilder = null;

        public SqlBuilder SqlBuilder { get {
                if (_SqlBuilder == null)
                {
                    lock (DatabaseConfigPool._DatabaseSqlBuilderLock)
                    {
                        if (_SqlBuilder == null)
                        {
                            _SqlBuilder = new SqlBuilder(this);
                            return _SqlBuilder;
                        }
                    }
                }
                return _SqlBuilder;
            } }

        public string DatabaseName { get; private set; }
        public string ProviderName { get; private set; }

        public DbProviderFactory DbFactory
        {
            get;
            private set;
        }
        public ISqlGenerator SqlGenerator
        {
            get
            {
                return DapperImplementor.SqlGenerator;
            }
        }

        private ISqlDialectProvider _SqlDialectProvider = null;
        public ISqlDialectProvider SqlDialectProvider
        {
            get
            {
                if (_SqlDialectProvider == null)
                {
                    lock (DatabaseConfigPool._DatabaseSqlDialectProviderLock)
                    {
                        if (_SqlDialectProvider == null)
                        {
                            _SqlDialectProvider = SqlDialectProviderLoader.GetSqlProviderByDatabaseType(this.DatabaseType);
                            return _SqlDialectProvider;
                        }
                    }
                }
                return _SqlDialectProvider;
            }
        }

        
#if ASYNC
        private IDapperAsyncImplementor _DapperImplementor = null;
        public IDapperAsyncImplementor DapperImplementor
        {
            get
            {
                if (_DapperImplementor == null)
                {
                    lock (DatabaseConfigPool._DatabaseDapperImplementorLock)
                    {
                        if (_DapperImplementor == null)
                        {
                            _DapperImplementor = DapperImplementorBoostraper.Instance.LoadAsync(DatabaseType, this);//_dapper;
                            return _DapperImplementor;
                        }
                    }
                }
                return _DapperImplementor;
            }
        }
#else
        private IDapperImplementor _DapperImplementor = null;
        public IDapperImplementor DapperImplementor
        {
            get
            {
                if (_DapperImplementor == null)
                {
                    lock (DatabaseConfigPool._DatabaseDapperImplementorLock)
                    {
                        if (_DapperImplementor == null)
                        {
                            _DapperImplementor = DapperImplementorBoostraper.Instance.Load(DatabaseType, this);//_dapper;
                            return _DapperImplementor;
                        }
                    }
                }
                return _DapperImplementor;
            }
        }
#endif

        public string ConnectionString { get; private set; }

        /// <summary>
        /// 数据库连接对象
        /// </summary>
        public IDbConnection Connection
        {
            get;
            private set;
        }

        internal bool TransactionIsAborted { get; set; }
        internal int TransactionCount { get; set; }
        private bool ShouldCloseConnectionAutomatically { get; set; }

        private IDbTransaction _transaction;

        public bool HasActiveTransaction
        {
            get
            {
                return TransactionIsOk();// _transaction != null;
            }
        }
        /// <summary>
        /// 设置连接持续，如果true需要手动释放
        /// </summary>
        /// <param name="enable"></param>
        public void SetConnectionAlive(bool enable)
        {
            Config.KeepConnectionAlive = enable;
        }
        /// <summary>
        /// 数据库事务对象
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }


        public DatabaseType DatabaseType { get; private set; }
        public Stopwatch Watch { get; private set; }
        
        public string LastSQL { get; set; }
        public IDataParameterCollection LastArgs { get; set; }

        #endregion

        #region Config

        private LogHelper _logHelper = null;
        /// <summary>
        /// 日志输出助手
        /// </summary>
        public LogHelper LogHelper
        {
            get
            {
                if (_logHelper == null)
                {
                    lock (DatabaseConfigPool._logHelperLock)
                    {
                        if (_logHelper == null)
                        {
                            _logHelper = new LogHelper(Config.EnableDebug, Config.EnableOrmLog, Config.OrmLogsPath, Config.MaxServerLogSize, Config.CategoryLogType);
                            _logHelper.EnableDebug = Config.EnableDebug;
                            _logHelper.OutputAction = Config.OutputAction;
                            _logHelper.EnableOrmLog = Config.EnableOrmLog;
                            _logHelper.EnableInternalLog = Config.EnableInternalLog;
                            
                            _logHelper.OrmLogsPath = Config.OrmLogsPath;
                            _logHelper.MaxServerLogSize = Config.MaxServerLogSize;
                            _logHelper.CategoryLogType = Config.CategoryLogType;
                        }

                    }
                }
                return _logHelper;
            }
        }

        private IDatabaseConfig _DatabaseConfig = null;
        /// <summary>数据库配置</summary>
        public IDatabaseConfig Config
        {
            get
            {
                if (_DatabaseConfig == null)
                {
                    lock (DatabaseConfigPool._DatabaseConfigLock)
                    {
                        if (_DatabaseConfig == null)
                        {
                            _DatabaseConfig = new DatabaseConfig();
                        }

                    }
                }
                return _DatabaseConfig;
            }
             

        }

        /// <summary>
        /// 初始化数据库配置
        /// </summary>
        /// <param name="InitConfig"></param>
        private void InitDatabaseConfig(Action<IDatabaseConfig> InitConfig = null)
        {
            if (InitConfig != null)
            {
                InitConfig(this.Config);
            }
        }
        #endregion

        #region Construct

        /// <summary>
        /// 创建新的连接对象
        /// </summary>
        private void CreateAndInitConnection( IDbConnection initConnection = null)
        {
            if (Connection != null)
            {
                return;
            }

            if (initConnection != null) //直接通过connection初始化
            {
                if (Config.DbConnectionInit != null)
                {
                    initConnection = Config.DbConnectionInit(initConnection);
                }
                Connection = initConnection;

                return;
            }
            else
            {
                //if (Config.EnableConnectionPool == true)
                //{

                //    Connection = Pooling.DbConnectionPoolProxy.Instance.BorrowObject(this);
                //    if (Config.DbConnectionInit != null)
                //    {
                //        Connection = Config.DbConnectionInit(Connection);
                //    }

                //    if (Connection == null) throw new Exception("DB Connection failed to configure.");
                //    if (Connection.State != ConnectionState.Open)
                //    {
                //        Connection.ConnectionString = Connection.ConnectionString;
                //    }

                //    return;
                 

                //}
                //else
                //{
                    if (DbFactory == null)
                    {
                        DbFactory = DbConnectionFactory.CreateConnection(ConnectionString, ProviderName).DbFactory;
                        if (DbFactory == null)
                        {
                            throw new ArgumentException("DbFactory can not be null!");
                        }
                    }
                    IDbConnection conn = DbFactory.CreateConnection();
                    if (Config.DbConnectionInit != null)
                    {
                        conn = Config.DbConnectionInit(conn);
                    }
                    Connection = conn;
                    if (Connection == null) throw new Exception("DB Connection failed to configure.");
                    if (Connection.State != ConnectionState.Open)
                    {
                        Connection.ConnectionString = ConnectionString;

                    } 

                    return;

                //}


            }


        }

        public DbConnection CreateNewDbConnection(bool needInitAction = false) {
            if (DbFactory == null)
            {
                DbFactory = DbConnectionFactory.CreateConnection(ConnectionString, ProviderName).DbFactory;
                if (DbFactory == null)
                {
                    throw new ArgumentException("DbFactory can not be null!");
                }
            }
            DbConnection conn = DbFactory.CreateConnection();
            if (needInitAction == true && Config.DbConnectionInit != null)
            {
                conn = Config.DbConnectionInit((IDbConnection)conn) as DbConnection;
            }
          
            if (conn == null) throw new Exception("DB Connection failed to configure.");
            if (conn.State != ConnectionState.Open)
            {
                conn.ConnectionString = ConnectionString;

            }
            Connection = conn;

            return conn;
        }

        /// <summary>
        /// 初始化数据库连接信息
        /// </summary>
        /// <param name="info"></param>
        private void InitDbConnectionInfo(DbConnectionMetaInfo info, IDbConnection initConnection = null)
        {

            ProviderName = info.ProviderName;
            DbFactory = info.DbFactory;
            DatabaseType = info.DatabaseType;
            ConnectionString = info.ConnectionString;



            this.CreateAndInitConnection(initConnection);


            //设置当前数据库连接池对象
            if (string.IsNullOrEmpty(Connection.Database))
            {
                string key = DatabaseConfigPool.GetConnectionKey(Connection);

                DatabaseName = key;
            }
            else
            {
                DatabaseName = Connection.Database;// dbName;
            }


            //增加输出SQL中断器
            if (Config.EnableDebug && Config.EnableIntercept)
            {
                Watch = new Stopwatch();

                //if (Config.InitByPureDataConfiguration == false)
                //{
                //    Config.Interceptors.Add(OutputSQLIntercept.Instance);
                //    Config.Interceptors.Add(OutputExceptionIntercept.Instance);
                //}
            }


            var status  = DatabaseConfigPool.GetInitStatus(this);
            if (status.HasFirstLoadFinish == false) //初始化状态
            {
                lock (DatabaseConfigPool.FirstLoadingLock)
                {


                    //输出配置信息
                    if (Config.EnableDebug)
                    {
                        OutputMessageHandler.Instance.OutputWelcomInfo(this);
                    }

                    //初始化验证设置
                    Validations.ValidatorOptions.Init(this);

                    //设置当前配置缓存
                    //DatabaseConfigPool.Set(DatabaseConfigPool.GetConnectionHashKey(Connection).ToString(), this);

                    //初始化所有Mappers
                    FluentExpressionSqlBuilderBoostraper.Instance.LoadAllMapper(this);

                    //初始化dapper和FluentExpressSql
                    //InitSqlGenerator(DatabaseName, DatabaseType);

                    if (Config.EnableSqlMap)
                    {
                        Pure.Data.SqlMap.SqlMapManager.Instance.Init(this);
                    }


                    //是否自动迁移数据库
                    if (Config.AutoMigrate)
                    {
                        DbMigrateService.Instance.AutoMigrate(this);
                    }

                    //是否自动生成代码
                    if (Config.EnableCodeGen == true)
                    {
                        GenerateCode();
                    }

                    //设置全局忽略更新列
                    if (Config.EnableGlobalIgnoreUpdatedColumns)
                    {
                        Snapshotter.InitGlobalIgnoreUpdatedColumns(Config.GlobalIgnoreUpdatedColumns, Config.AutoFilterEmptyValueColumnsWhenTrack);
                    }
                     



                    status.HasFirstLoadFinish = true; //初始化完成

                }

            }
             

        }
     
        public Database(string connKey, Action<IDatabaseConfig> InitConfig = null)
        {
            InitDatabaseConfig(InitConfig);

            var info = DbConnectionFactory.CreateConnectionByConfig(connKey);

            InitDbConnectionInfo(info);

        }
        public Database(string configFile, OutputActionDelegate _LogAction = null, Action<IDatabaseConfig> InitConfig = null)
        {
           
            if (configFile == null || configFile =="")
            {
                configFile = "PureDataConfiguration.xml";
            }
            //开启独立配置DatabaseConfiguration,可用于读写分离
            var loadedInfo = PureDataConfigurationLoader.Instance.Load(configFile, this.Config, _LogAction);
            this._DatabaseConfig = loadedInfo.DatabaseConfig;

            
            InitDatabaseConfig(InitConfig);
           
            InitDbConnectionInfo(loadedInfo.DbConnectionMetaInfo);
   

        }
        public Database(string connKey, string providerName, Action<IDatabaseConfig> InitConfig = null)
        {
            InitDatabaseConfig(InitConfig);
            var info = DbConnectionFactory.CreateConnectionByConfig(connKey, providerName);
            InitDbConnectionInfo(info);
        }
        public Database(string connectionString, DatabaseType dbType, Action<IDatabaseConfig> InitConfig = null)
        {
            InitDatabaseConfig(InitConfig);
            string providerName = DbConnectionFactory.GetDefaultProviderNameByDbType(dbType);
            var info = DbConnectionFactory.CreateConnection(connectionString, providerName);
            InitDbConnectionInfo(info);
        }
   
        public Database(IDbConnection connection, Action<IDatabaseConfig> InitConfig = null)
        {
            InitDatabaseConfig(InitConfig);
            DbConnectionMetaInfo info = new DbConnectionMetaInfo();
            info.DatabaseType = DbConnectionFactory.Resolve(connection.GetType().Name, null);


            info.ConnectionString = Connection.ConnectionString;
            info.ProviderName = "System.Data.SqlClient";
            info.DbFactory = DbProviderFactories.GetFactory(info.ProviderName);
            InitDbConnectionInfo(info, connection);

        }
        #endregion

        #region Execute


        public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                if (transaction == null)
                {
                    transaction = Transaction;
                }
                if (commandTimeout == null || !commandTimeout.HasValue)
                {
                    commandTimeout = Config.ExecuteTimeout;
                }
                if (commandType == null)
                {
                    commandType = CommandType.Text;
                }
                return Connection.Execute(sql, param, transaction, commandTimeout, commandType, this);
            }
            catch (Exception ex)
            {
                throw new PureDataException("Execute", ex);
                 
            }
            finally
            {
                Close();
            }
           
        }
        public object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                if (transaction == null)
                {
                    transaction = Transaction;
                }
                if (commandTimeout == null || !commandTimeout.HasValue)
                {
                    commandTimeout = Config.ExecuteTimeout;
                }
                if (commandType == null)
                {
                    commandType = CommandType.Text;
                }
                return Connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType, this);
            }
            catch (Exception ex)
            {
                throw new PureDataException("ExecuteScalar", ex);
            }
            finally
            {
                Close();
            }
        }
        public T ExecuteScalar<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                if (transaction == null)
                {
                    transaction = Transaction;
                }
                if (commandTimeout == null || !commandTimeout.HasValue)
                {
                    commandTimeout = Config.ExecuteTimeout;
                }
                
                return Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType, this);
            }
            catch (Exception ex)
            {
                throw new PureDataException("ExecuteScalar", ex);

            }
            finally
            {
                Close();
            }
        }
        public IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            if (transaction == null)
            {
                transaction = Transaction;
            }
            if (commandTimeout == null || !commandTimeout.HasValue)
            {
                commandTimeout = Config.ExecuteTimeout;
            }
            if (commandType== null)
            {
                commandType = CommandType.Text;
            }
            return Connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType, this);
        }
        public List<T> ExecuteList<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {
                var result = SqlQuery<T>(sql, param, transaction, buffer, commandTimeout, commandType).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw new PureDataException("ExecuteList", ex);

            }
            finally
            {
                Close();
            }

        }
        public List<T> ExecuteListWithRowDelegate<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    List<T> data = result.ToList<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteListWithRowDelegate", ex);

            }
            finally
            {
                Close();
            }

        }
        public List<T> ExecuteListByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    List<T> data = result.ToListByEmit<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteListByEmit", ex);

            }
            finally
            {
                Close();
            }

        }

        public T ExecuteModel<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {

                    T data = result.ToModel<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteModel", ex);

            }
            finally
            {
                Close();
            }

        }
        public T ExecuteModelByEmit<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {

                    T data = result.ToModelByEmit<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteModelByEmit", ex);

            }
            finally
            {
                Close();
            }

        }
        public DataTable ExecuteDataTable(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    DataTable data = result.ToDataTable(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataTable", ex);

            }
            finally
            {
                Close();
            }

        }
        public DataTable ExecuteDataTableWithRowDelegate(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    DataTable data = result.ToDataTableWithRowDelegate(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataTableWithRowDelegate", ex);

            }
            finally
            {
                Close();
            }

        }
        public System.Data.DataSet ExecuteDataSet(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    DataSet data = result.ToDataSet(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDataSet", ex);

            }
            finally
            {
                Close();
            }

        }

        public Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("ExecuteDictionary", ex);

            }
            finally
            {
                Close();
            }

        }

        public dynamic ExecuteExpandoObject(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    dynamic data = result.ToExpandoObject(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new PureDataException("ExecuteExpandoObject", ex);

            }
            finally
            {
                Close();
            }

        }

        public IEnumerable<dynamic> ExecuteExpandoObjects(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                var result = ExecuteReader(sql, param, transaction, commandTimeout, commandType);
                if (result != null)
                {
                    IEnumerable<dynamic> data = result.ToExpandoObjects(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new PureDataException("ExecuteExpandoObjects", ex);

            }
            finally
            {
                Close();
            }

        }
        #endregion

        #region Query
        public IEnumerable<T> SqlQuery<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null)
        {

            try
            {

                if (transaction == null)
                {
                    transaction = Transaction;
                }
                return Connection.Query<T>(sql, param, transaction, buffer, commandTimeout, commandType, this);
            }
            catch (Exception ex)
            {
                throw new PureDataException("SqlQuery", ex);

            }
            finally
            {
                Close();
            }
        }
        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {

            return DapperImplementor.GetList<TEntity>(Connection, null, null, _transaction, null, true);
        }
        public T Get<T>(object id, IDbTransaction transaction, int? commandTimeout = null) where T : class
        {
            return (DapperImplementor.Get<T>(Connection, id, transaction, commandTimeout));
        }

        public T Get<T>(object id, int? commandTimeout=null) where T : class
        {
            return (DapperImplementor.Get<T>(Connection, id, _transaction, commandTimeout));
        }


        public IEnumerable<T> Query<T>(object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            return DapperImplementor.GetList<T>(Connection, predicate, sort, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> Query<T>(object predicate, IList<ISort> sort, int? commandTimeout = null, bool buffered = true) where T : class
        {
            return DapperImplementor.GetList<T>(Connection, predicate, sort, _transaction, commandTimeout, buffered);
        }

        public IDataReader QueryByKV<T>(string[] columns, IDictionary<string, object> conditions, IList<ISort> sort) where T : class
        {

            var classMap = GetMap<T>();

            return this.Query(classMap.TableName, columns, conditions, sort);
        }


        public int CountByKV<T>(IDictionary<string, object> conditions) where T : class
        {

            var classMap = GetMap<T>();

            return this.Count(classMap.TableName, conditions);
        }

        public long LongCountByKV<T>(IDictionary<string, object> conditions) where T : class
        {

            var classMap = GetMap<T>();

            return this.LongCount(classMap.TableName, conditions);
        }
        public int Count<T>() where T : class
        {
            return DapperImplementor.Count<T>(Connection, null, Transaction, Config.ExecuteTimeout);
        }

        public long LongCount<T>() where T : class
        {
            return DapperImplementor.CountLong<T>(Connection, null, Transaction, Config.ExecuteTimeout);
        }
        public int Count<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return DapperImplementor.Count<T>(Connection, predicate, transaction, commandTimeout);
        }

        public int Count<T>(object predicate, int? commandTimeout = null) where T : class
        {
            return DapperImplementor.Count<T>(Connection, predicate, _transaction, commandTimeout);
        }


        public long LongCount<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return DapperImplementor.CountLong<T>(Connection, predicate, transaction, commandTimeout);
        }

        public long LongCount<T>(object predicate, int? commandTimeout = null) where T : class
        {
            return DapperImplementor.CountLong<T>(Connection, predicate, _transaction, commandTimeout);
        }
        public bool Exists<T>(object predicate) where T : class
        {
            return Count<T>(predicate, null) > 0;
        }
        #endregion

        #region Page

        public string GetSqlOfPage(int pageIndex, int pagesize, string sql, IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            return DapperImplementor.SqlGenerator.Configuration.Dialect.GetPagingSql(sql, pageIndex, pagesize, parameters);
        }
        public string GetSqlOfCount(string sql)
        {
            return DapperImplementor.SqlGenerator.Configuration.Dialect.GetCountSql(sql);
        }
        public IDataReader GetPageReader(int pageIndex, int pagesize, string sql, string orderField, bool asc, IDictionary<string, object> parameters, out int total)
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            string orderType = "";
            if (asc)
            {
                orderType = "ASC";
            }
            else
            {
                orderType = "DESC";

            }
            string orderBy = !string.IsNullOrEmpty(orderField) ? " ORDER BY " + orderField + " " + orderType : "";
            string newSql = sql + orderBy;
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            var str = GetSqlOfPage(pageIndex, pagesize, newSql, parameters);
            string strCount = GetSqlOfCount(sql);// "SELECT COUNT(1) FROM (" + sql + ") AS __SQLCOUNT__";
            total = Convert.ToInt32(ExecuteScalar(strCount, parameters));

            var data = ExecuteReader(str, parameters);
            return data;
        }
        public List<TEntity> GetPageBySQL<TEntity>(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount) where TEntity : class
        {
            try
            {
                SetConnectionAlive(true);
                pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
                string strCount = GetSqlOfCount(sqltext);
                totalCount = Convert.ToInt32(ExecuteScalar(strCount, parameters));
                if (!string.IsNullOrEmpty(orderText))
                {
                    sqltext = sqltext + " ORDER BY " + orderText;
                }
                if (parameters == null)
                {
                    parameters = new Dictionary<string, object>();
                }
                string sqlPage = GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
                var data = SqlQuery<TEntity>(sqlPage, parameters);

                return data.ToList();
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                SetConnectionAlive(false);

                Close();
            }
              
        }
        public IDataReader GetPageReaderBySQL(int pageIndex, int pagesize, string sqltext, string orderText, IDictionary<string, object> parameters, out int totalCount)
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
            string strCount = GetSqlOfCount(sqltext);
            totalCount = Convert.ToInt32(ExecuteScalar(strCount, parameters));
            if (!string.IsNullOrEmpty(orderText))
            {
                sqltext = sqltext + " ORDER BY " + orderText;
            }
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            string sqlPage = GetSqlOfPage(pageIndex, pagesize, sqltext, parameters);
            var data = ExecuteReader(sqlPage, parameters);
            return data;

        }
        public IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
            return DapperImplementor.GetSet<T>(Connection, predicate, sort, pageIndex, pagesize, transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetSet<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
            return DapperImplementor.GetSet<T>(Connection, predicate, sort, pageIndex, pagesize, _transaction, commandTimeout, buffered);
        }
        public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, transaction, commandTimeout, buffered);
        }
        public PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize,   transaction, commandTimeout, buffered);
        }
        public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            return DapperImplementor.GetPage<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, _transaction, commandTimeout, buffered);
        }

        public IEnumerable<T> GetPage<T>(int pageIndex, int pagesize, out long allRowsCount, string sql, object param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
            transaction = transaction == null ? _transaction : transaction;

            return DapperImplementor.GetPage<T>(Connection, pageIndex,  pagesize, out allRowsCount,  sql,  param ,  allRowsCountSql ,  transaction ,  commandTimeout ,  buffered );
        }
        public PageDataResult<IEnumerable<T>> GetPage<T>(int pageIndex, int pagesize,   string sql, object param = null, string allRowsCountSql = null, IDbTransaction transaction = null, int? commandTimeout = null, bool buffered = true) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;
            transaction = transaction == null ? _transaction : transaction;

            return DapperImplementor.GetPage<T>(Connection, pageIndex, pagesize,  sql, param, allRowsCountSql, transaction, commandTimeout, buffered);
        }
        public IDataReader GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort, out int totalCount, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            return DapperImplementor.GetPageReader<T>(Connection, predicate, sort, pageIndex, pagesize, out totalCount, _transaction, commandTimeout);
        }

        public PageDataResult<IDataReader> GetPageReader<T>(int pageIndex, int pagesize, object predicate, IList<ISort> sort,   IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

            return DapperImplementor.GetPageReader<T>(Connection, predicate, sort, pageIndex, pagesize,  _transaction, commandTimeout);
        }
        #endregion

        #region Multi-Result
        public IMultipleResultReader GetMultiple(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return new GridReaderResultReader(Connection.QueryMultiple(sql, param, transaction, commandTimeout, commandType, this));
        }
        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            return DapperImplementor.GetMultiple(Connection, predicate, transaction, commandTimeout);
        }

        public IMultipleResultReader GetMultiple(GetMultiplePredicate predicate, int? commandTimeout)
        {
            return DapperImplementor.GetMultiple(Connection, predicate, _transaction, commandTimeout);
        }
        #endregion

        #region Insert
        public void InsertBulk(DataTable dt, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).Insert(this, dt); 
        }
        public void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).Insert(this, entities);
        }
        //public void InsertBulk<T>(IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        //{
        //    if (!OnInsertingInternal(new InsertContext(entities)))
        //        return;
        //    if (SqlGenerator.Configuration.Dialect.databaseType == Data.DatabaseType.SqlServer)
        //    {
        //        SqlServerBatcher batch = new SqlServerBatcher();
        //        batch.Insert(this, transaction, entities, GetMap<T>().TableName);
        //    }
        //    else
        //    {
        //        DapperImplementor.Insert<T>(Connection, entities, transaction, commandTimeout);

        //    }

        //}

        //public void InsertBulk<T>(IEnumerable<T> entities, int? commandTimeout = null) where T : class
        //{

        //    InsertBulk<T>(entities,  null, commandTimeout);

        //}
        public void InsertBatch(DataTable dt, int batchSize = 10000)  
        {
          //  options = options ?? new BatchOptions();

            BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).InsertBatch(this, dt,batchSize);

        }
        public void InsertBatch<T>(IEnumerable<T> entities, IDbTransaction transaction = null,  int batchSize = 10000, int? commandTimeout = null) where T : class
        {
            if (!OnInsertingInternal(new InsertContext(entities)))
                return ;

            //options = options ?? new BatchOptions();

            BulkOperateManage.Instance.Get(this.Config.BulkOperateClassName).InsertBatch<T>(this, entities, batchSize);


            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //string sql = "";
            //int result = 0;
            //object p = null;
            //foreach (var chunks in entities.Chunkify(options.BatchSize))
            //{
            //    sb.Clear();
            //    //var preparedInserts = chunks.Select(x => new { sql = (_dapper.PrepareInsertStament(x, out p)), parameters = p }).ToArray();
            //    var preparedInserts = chunks.Select(x => DapperImplementor.PrepareInsertStament(x, out p)).ToArray();

            //    //var preparedInserts = chunks.Select(x => new { sql = ExpressionSqlBuilder.Insert<T>(() => new { x }).ToSqlString() }).ToArray();

            //    foreach (var preparedInsertSql in preparedInserts)
            //    {
            //        sb.Append(preparedInsertSql);
            //        sb.Append(options.StatementSeperator);
            //    }
            //    sql = sb.ToString();
            //    if (!string.IsNullOrEmpty(sql))
            //    {
            //        result += this.Execute(sql, null, transaction);
            //    }
            //    //  InsertBulk<T>(chunks.AsEnumerable(), transaction, null);

            //}
            //return result;
        }

        public void InsertBatch<T>(IEnumerable<T> entities, int batchSize = 10000, int? commandTimeout = null) where T : class
        {
            //if (!OnInsertingInternal(new InsertContext(entities)))
            //    return 0;
             InsertBatch<T>(entities, null, batchSize);
        }
        public dynamic Insert<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class
        {
            if (!OnInsertingInternal(new InsertContext(entity)))
                return 0;
            return DapperImplementor.Insert<T>(Connection, entity, transaction, commandTimeout);
        }

        public dynamic Insert<T>(T entity, int? commandTimeout=null) where T : class
        {
            if (!OnInsertingInternal(new InsertContext(entity)))
                return 0;

            return DapperImplementor.Insert<T>(Connection, entity, _transaction, commandTimeout);
        }

        public int InsertByKV<T>(IDictionary<string, object> parameters) where T : class
        {

            var classMap = GetMap<T>();

            return this.Insert(classMap.TableName, parameters);
        }

        #endregion

        #region Update
        public bool Update<T>(T entity, IDbTransaction transaction, int? commandTimeout = null) where T : class
        {
            if (!OnUpdatingInternal(new UpdateContext(entity)))
                return false;
            return DapperImplementor.Update<T>(Connection, entity, transaction, commandTimeout);
        }

        public bool Update<T>(T entity, int? commandTimeout=null) where T : class
        {
            if (!OnUpdatingInternal(new UpdateContext(entity)))
                return false;
            return DapperImplementor.Update<T>(Connection, entity, _transaction, commandTimeout);
        }

        public int UpdateByKV<T>(IDictionary<string, object> parameters, IDictionary<string, object> conditions) where T : class
        {

            var classMap = GetMap<T>();

            return this.Update(classMap.TableName, conditions, parameters);
        }

        public int UpdateBySQL(string sql, object parameters, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            if (!OnUpdatingInternal(new UpdateContext(sql)))
                return 0;
            transaction = transaction ?? _transaction;
            return DapperImplementor.Update(Connection, sql, parameters, transaction, commandTimeout);
        }
        #endregion

        #region Delete
        public bool Delete<T>(T entity, IDbTransaction transaction = null, int? commandTimeout=null) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(entity)))
                return false;
            return DapperImplementor.Delete(Connection, entity, transaction, commandTimeout);
        }

        public bool Delete<T>(T entity, int? commandTimeout=null) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(entity)))
                return false;
            return DapperImplementor.Delete(Connection, entity, _transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(predicate)))
                return false;
            return DapperImplementor.Delete<T>(Connection, predicate, transaction, commandTimeout);
        }

        public bool Delete<T>(object predicate, int? commandTimeout) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(predicate)))
                return false;
            return DapperImplementor.Delete<T>(Connection, predicate, _transaction, commandTimeout);
        }

        public bool DeleteByIds<T>(string cName, string ids, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(new { cName = cName, ids = ids })))
                return false;
            transaction = transaction ?? _transaction;
            return DapperImplementor.DeleteByIds<T>(Connection, cName, ids, transaction, commandTimeout);
        }

        public bool DeleteById<T>(object id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (!OnDeletingInternal(new DeleteContext(id)))
                return false;
            transaction = transaction ?? _transaction;
            return DapperImplementor.DeleteById<T>(Connection, id, _transaction, commandTimeout);
        }

        public int DeleteByKV<T>(IDictionary<string, object> conditions) where T : class
        {

            var classMap = GetMap<T>();

            return this.Delete(classMap.TableName, conditions);
        }
        public int DeleteAll<T>(  IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            
            transaction = transaction ?? _transaction;
            return DapperImplementor.DeleteAll<T>(Connection,   _transaction, commandTimeout);
        }
        public int Truncate<T>(IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            transaction = transaction ?? _transaction;
            return DapperImplementor.Truncate<T>(Connection, _transaction, commandTimeout);
        }
        #endregion

        #region Transaction
        // Helper to create a transaction scope
        public ITransaction GetTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new Transaction(this, isolationLevel);
        }

        public void SetTransaction(IDbTransaction tran)
        {
            _transaction = tran;
        }

        /// <summary>
        /// Start a new transaction, can be nested, every call must be
        ///	matched by a call to AbortTransaction or CompleteTransaction
        /// Use `using (var scope=db.Transaction) { scope.Complete(); }` to ensure correct semantics
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_transaction == null)
            {
                TransactionCount = 0;
                OpenSharedConnectionInternal();
                _transaction = Connection.BeginTransaction(isolationLevel);
                OnBeginTransactionInternal();
            }

            if (_transaction != null)
            {
                TransactionCount++;
            }

            //_transaction = Connection.BeginTransaction(isolationLevel);
        }

        public void CommitTransaction()
        {
            CompleteTransaction();
            //_transaction.Commit();
            //_transaction = null;
        }

        public void RollbackTransaction()
        {
            TransactionIsAborted = true;
            AbortTransaction(false);

            //_transaction.Rollback();
            //_transaction = null;
        }
        private bool TransactionIsOk()
        {
            return Connection != null
                && _transaction != null
                //&& Connection.State == ConnectionState.Open;
                && _transaction.Connection != null
                && _transaction.Connection.State == ConnectionState.Open;
        }
        private void AbortTransaction(bool fromComplete)
        {
            if (_transaction == null)
                return;

            if (fromComplete == false)
            {
                TransactionCount--;
                if (TransactionCount >= 1)
                {
                    TransactionIsAborted = true;
                    return;
                }
            }

            if (TransactionIsOk())
            {
                _transaction.Rollback();
                OnRollbackTransactionInternal();
            }

            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            _transaction = null;
            TransactionIsAborted = false;

            // You cannot continue to use a connection after a transaction has been rolled back
            //if (Connection != null)
            //{
            //    Connection.Close();
            //}

            OnAbortTransactionInternal();
            CloseSharedConnectionInternal();
        }

        private void CompleteTransaction()
        {
            if (_transaction == null)
                return;

            TransactionCount--;
            if (TransactionCount >= 1)
                return;

            if (TransactionIsAborted)
            {
                AbortTransaction(true);
                return;
            }

            if (TransactionIsOk())
            {
                _transaction.Commit();
                OnCommitTransactionInternal();

            }

            if (_transaction != null)
                _transaction.Dispose();

            _transaction = null;
            //if (Connection != null)
            //{
            //    Connection.Close();
            //}
            OnCompleteTransactionInternal();
            CloseSharedConnectionInternal();
        }


        public void RunInTransaction(Action action)
        {
            BeginTransaction();
            try
            {
                action();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    RollbackTransaction();
                }
                throw new PureDataException("RunInTransaction", ex);

            }
        }

        public T RunInTransaction<T>(Func<T> func)
        {
            BeginTransaction();
            try
            {
                T result = func();
                CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                if (HasActiveTransaction)
                {
                    RollbackTransaction();
                }
                throw new PureDataException("RunInTransaction", ex);

            }
        }

        #endregion

        #region Dispose & Open & Close
        public void Dispose()
        {
            //if (Config.KeepConnectionAlive) return;
           
            if (Connection != null)
            {
                //if (Config.EnableConnectionPool == true)
                //{
                //   // Pooling.DbConnectionPoolProxy.Instance.ReturnObject(this, Connection);
                    
                //}
                //else
                //{
                    if (Connection.State != ConnectionState.Closed)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                            OnRollbackTransactionInternal();
                        }
                        //OnConnectionClosingInternal(Connection);
                        OnConnectionClosingWithIntercept();
                        Connection.Close();
                    }

                    if (Config.AutoDisposeConnection == true)
                    {
                        Connection.Dispose();
                        Connection = null;
                        //if (Config.EnableDebug)
                        //{
                        //    LogHelper.Debug("Connection has Disposed.");
                        //}
                    }

                //}
                 
            }
             

        }


         
        /// <summary>
        /// 关闭数据库连接，当开启KeepConnectionAlive为true则不关闭
        /// </summary>
        public void Close()
        {
            if (Config.KeepConnectionAlive) return;
            if (Connection == null) return;

            if (HasActiveTransaction == false)
            {

                //if (Config.EnableConnectionPool == true)
                //{
                //    Pooling.DbConnectionPoolProxy.Instance.ReturnObject(this, Connection);
                     
                //}
                //else
                //{
                    if (Connection.State != ConnectionState.Closed)
                    {
                        if (_transaction != null)
                        {
                            _transaction.Rollback();
                            OnRollbackTransactionInternal();
                        }
                       // OnConnectionClosingInternal(Connection);
                        OnConnectionClosingWithIntercept();
                        Connection.Close();

                    }
               // } 

            }
          
        }

        public void OnConnectionClosingWithIntercept() {
            OnConnectionClosingInternal(Connection);
        }


        private void CloseSharedConnectionInternal()
        {
            if (ShouldCloseConnectionAutomatically && _transaction == null)
                Close();
        }

        public void EnsureOpenConnection()
        {
            if (Connection != null)
            {
                if (Connection.State != ConnectionState.Open)
                {
                    Connection.Open();
                }
            }
        }
      
        // Open a connection (can be nested)
        public IDatabase OpenSharedConnection()
        {
            OpenSharedConnectionImp(false);
            return this;
        }

        // Open new connection
        public IDatabase OpenNewConnection()
        {
            ShouldCloseConnectionAutomatically = true;
            if (Connection.State == ConnectionState.Broken)
            {
                Connection.Close();
            }
            if (Connection.State == ConnectionState.Closed)
            {
                //Connection = DbFactory != null ? DbFactory.CreateConnection() : DbConnectionFactory.CreateConnection(ConnectionString, ProviderName).Connection;
                this.CreateAndInitConnection();

                if (Connection == null) throw new Exception("DB Connection failed to configure.");

                Connection.ConnectionString = ConnectionString;

                Connection.Open();
            }


            //if (Connection.State == ConnectionState.Broken)
            //{
            //    Connection.Close();
            //}

            //if (Connection.State == ConnectionState.Closed)
            //{
            //    Connection.Open();
            //    Connection = OnConnectionOpenedInternal(Connection);
            //}
            //LogHelper.Debug("New Connection Created." + Connection.GetHashCode());
            return this;
        }

        private void OpenSharedConnectionInternal()
        {
            OpenSharedConnectionImp(true);
        }

        private void OpenSharedConnectionImp(bool isInternal)
        {
            if (Connection != null && Connection.State != ConnectionState.Broken && Connection.State != ConnectionState.Closed)
                return;

            ShouldCloseConnectionAutomatically = isInternal;

            //Connection = DbFactory != null ? DbFactory.CreateConnection() : DbConnectionFactory.CreateConnection(ConnectionString, ProviderName).Connection; // DbFactory.CreateConnection();
            this.CreateAndInitConnection(); // DbFactory.CreateConnection());
            if (Connection == null) throw new Exception("DB Connection failed to configure.");

            Connection.ConnectionString = ConnectionString;

            if (Connection.State == ConnectionState.Broken)
            {
                Connection.Close();
            }

            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                Connection = OnConnectionOpenedInternal(Connection);
            }
            //LogHelper.Debug("New Connection has Open."+Connection.GetHashCode());

        }

        #endregion

        #region Common

        public void ClearMap()
        {
            DapperImplementor.SqlGenerator.Configuration.ClearCache();
        }
        public DynamicParameters NewDynamicParameters()
        {
            return new DynamicParameters();
        }
        public Guid GetNextGuid()
        {
            return DapperImplementor.SqlGenerator.Configuration.GetNextGuid();
        }
        public IClassMapper GetMap(string tableName) 
        {
            return DapperImplementor.SqlGenerator.Configuration.GetMap(tableName);
        }
        public IClassMapper GetMap<T>() where T : class
        {
            return DapperImplementor.SqlGenerator.Configuration.GetMap<T>();
        }
        public string GetColumnString<T>(string prefix = "T.", string spliteStr = ", ", params Expression<Func<T, object>>[] ignoreProperties) where T : class
        {
            return DapperImplementor.SqlGenerator.Configuration.GetColumnString<T>(prefix, spliteStr, ignoreProperties);
        }
        public IClassMapper GetMap(Type type)
        {
            return DapperImplementor.SqlGenerator.Configuration.GetMap(type);
        }
        public ValidationResult Validate<T>(T instance) where T : class
        {
            var mapper = GetMap<T>();
            if (mapper == null)
            {
                throw new ArgumentException("Cannot find IClassMapper of "+typeof(T).Name );
            }
            return mapper.Validate(this, instance); 
        }
        public ValidationResult Validate<T>(T instance,  params Expression<Func<T, object>>[] propertyExpressions) where T : class
        {
            var mapper = GetMap<T>();
            if (mapper == null)
            {
                throw new ArgumentException("Cannot find IClassMapper of " + typeof(T).Name);
            }
            return mapper.Validate(this, instance, propertyExpressions);
        }
        public ValidationResult Validate<T>(T instance, params string[] properties) where T : class
        {
            var mapper = GetMap<T>();
            if (mapper == null)
            {
                throw new ArgumentException("Cannot find IClassMapper of " + typeof(T).Name);
            }
            return mapper.Validate(this, instance, properties);
        }
        public void ValidateAndThrow<T>(T instance) where T : class
        {
            var mapper = GetMap<T>();
            if (mapper == null)
            {
                throw new ArgumentException("Cannot find IClassMapper of " + typeof(T).Name);
            }
             mapper.ValidateAndThrow(this, instance);
        }
        public void LoadAllMap(List<Assembly> MappingAssemblies = null, LoadMapperMode LoadMode = LoadMapperMode.FluentMapper)
        {
            DapperImplementor.SqlGenerator.Configuration.LoadAllMap(DatabaseName, MappingAssemblies, LoadMode);
        }
        public System.Collections.Concurrent.ConcurrentDictionary<Type, IClassMapper> GetAllMap()
        {
            return DapperImplementor.SqlGenerator.Configuration.GetAllMap();
        }
        #endregion

        #region Expression
        public void EnsureAddClassToTableMap<TEntity>() where TEntity : class
        {
            if (FluentSqlBuilder.TableMapperContainer.GetTable<TEntity>() == null)
            {
                IClassMapper map = GetMap<TEntity>();
                FluentSqlBuilder.TableMapperContainer.Add(map.EntityType, new TableMap<TEntity>(map.TableName));
            }
        }

        public IDataReader ExecuteReader<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            return this.ExecuteReader(expression.ToSqlString());
        }
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            //string sql = expression.RawString;
            //return this.SqlQuery<TEntity>(sql, expression.DbParams);
            return this.SqlQuery<TEntity>(expression.ToSqlString());
        }
        public TValue ExecuteScalar<TEntity, TValue>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            return this.ExecuteScalar<TValue>(expression.ToSqlString());
        }
        public int Execute<TEntity>(FluentExpressionSQLCore<TEntity> expression) where TEntity : class
        {
            return this.Execute(expression.ToSqlString());
        }
        public virtual int Insert<TEntity>(Expression<Func<object>> body) where TEntity : class
        {
            if (!OnInsertingInternal(new InsertContext(body)))
                return 0;
            EnsureAddClassToTableMap<TEntity>();
            return this.Execute<TEntity>(FluentSqlBuilder.Insert<TEntity>(body));
        }
        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            if (!OnDeletingInternal(new DeleteContext(condition)))
                return 0;
            EnsureAddClassToTableMap<TEntity>();
            return this.Execute<TEntity>(FluentSqlBuilder.Delete<TEntity>().Where(condition));
        }
        public virtual int Update<TEntity>(Expression<Func<object>> body, Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            if (!OnUpdatingInternal(new UpdateContext(body, condition)))
                return 0;
            EnsureAddClassToTableMap<TEntity>();
            var count = this.Execute<TEntity>(FluentSqlBuilder.Update<TEntity>(body).Where(condition));


            return count;
        }


        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            return Query<TEntity>(condition).FirstOrDefault();
        }
        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            string sql = FluentSqlBuilder.Select<TEntity>().Where(condition).ToSqlString();

            return this.SqlQuery<TEntity>(sql);
        }
        public IEnumerable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            var expression = FluentSqlBuilder.Select<TEntity>().Where(condition);
            if (orderAction != null)
            {
                orderAction(expression);
            }

            return this.ExecuteQuery<TEntity>(expression);
        }
        public IEnumerable<TEntity> QueryByWhere<TEntity>(string condition, string orderStr) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            return this.ExecuteQuery<TEntity>(FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr));
        }
        public IEnumerable<TEntity> QueryBySQL<TEntity>(string sql) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            return this.SqlQuery<TEntity>(sql);
        }

        public IEnumerable<TEntity> GetPageByWhere<TEntity>(int pageIndex, int pagesize, string condition, string orderStr, out int totalCount) where TEntity : class
        {
            try
            {
                SetConnectionAlive(true);
                EnsureAddClassToTableMap<TEntity>();
                pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

                totalCount = this.Count<TEntity>(condition);

                return this.ExecuteQuery<TEntity>(FluentSqlBuilder.Select<TEntity>().Where(condition).OrderByString(orderStr).TakePage(pageIndex, pagesize));
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                SetConnectionAlive(false);
                Close();
            }
           
        }

        public IEnumerable<TEntity> GetPage<TEntity>(int pageIndex, int pagesize, Expression<Func<TEntity, bool>> condition, Action<FluentExpressionSQLCore<TEntity>> orderAction, out int totalCount) where TEntity : class
        {

            try
            {

                EnsureAddClassToTableMap<TEntity>();
                pagesize = pagesize == 0 ? Config.DefaultPageSize : pagesize;

                var expression = FluentSqlBuilder.Select<TEntity>().Where(condition);
                if (orderAction != null)
                {
                    orderAction(expression);
                }
                totalCount = this.Count<TEntity>(condition);
                return this.ExecuteQuery<TEntity>(expression.TakePage(pageIndex, pagesize));
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                SetConnectionAlive(false);
                Close();
            }
           

        }

        public int Count<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            var expression = FluentSqlBuilder.Count<TEntity>().Where(condition);
            return ExecuteScalar<TEntity, int>(expression);
        }
        public long LongCount<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            var expression = FluentSqlBuilder.Count<TEntity>().Where(condition);
            return ExecuteScalar<TEntity, long>(expression);
        }
        private int Count<TEntity>(string condition) where TEntity : class
        {
            EnsureAddClassToTableMap<TEntity>();
            return this.ExecuteScalar<TEntity, int>(FluentSqlBuilder.Count<TEntity>().Where(condition));
        }
        public int Count(string sql)
        {
            return this.ExecuteScalar<int>(sql);
        }
        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> condition) where TEntity : class
        {
            return Count<TEntity>(condition) > 0;
        }
        #endregion

        #region Intercept

        #region TransactionInterceptor
        private void OnBeginTransactionInternal()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
                {
                    interceptor.OnBeginTransaction(this);
                }
            }

        }


        private void OnAbortTransactionInternal()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
                {
                    interceptor.OnAbortTransaction(this);
                }
            }
        }

        private void OnCompleteTransactionInternal()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
                {
                    interceptor.OnCompleteTransaction(this);
                }
            }
        }

        private void OnRollbackTransactionInternal()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
                {
                    interceptor.OnRollbackTransaction(this);
                }
            }
        }
        private void OnCommitTransactionInternal()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<ITransactionInterceptor>())
                {
                    interceptor.OnCommitTransaction(this);
                }
            }
        }
        #endregion

        #region ConnectionInterceptor
        public void OnDatabaseConnectionClosing()
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
                {
                    interceptor.OnConnectionClosing(this, this.Connection);
                }
            }
        }
        public void OnDatabaseException(Exception ex)
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<IExceptionInterceptor>())
                {
                    interceptor.OnException(this, ex);
                }
            }
        }
        private IDbConnection OnConnectionOpenedInternal(IDbConnection conn)
        {
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
                {
                    conn = interceptor.OnConnectionOpened(this, conn);
                }
            }
            return conn;
        }

        private void OnConnectionClosingInternal(IDbConnection conn)
        {
         
            if (Config.EnableIntercept)
            {
                foreach (var interceptor in Config.Interceptors.OfType<IConnectionInterceptor>())
                {
                    interceptor.OnConnectionClosing(this, conn);
                }
            }
        }
        #endregion

        private bool OnInsertingInternal(InsertContext insertContext)
        {
            if (Config.EnableIntercept)
            {
                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnInserting(this, insertContext));
            }
            return true;
        }
        private bool OnUpdatingInternal(UpdateContext updateContext)
        {
            if (Config.EnableIntercept)
            {
                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnUpdating(this, updateContext));
            }
            return true;
        }
        private bool OnDeletingInternal(DeleteContext deleteContext)
        {
            if (Config.EnableIntercept)
            {
                return Config.Interceptors.OfType<IDataInterceptor>().All(x => x.OnDeleting(this, deleteContext));
            }
            return true;
        }
        #endregion


       
        #region Batch Command
        /// <summary>
        /// 创建批处理命令
        /// </summary>
        /// <param name="batchSize"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        public BatchCommander NewBatchCommand(BatchOptions option, IDbTransaction tran = null)
        {
            if (option == null)
            {
                option = new BatchOptions();
            }
            if (option.BatchSize < 1)
            {
                throw new ArgumentException("batchSize must larger than 0!");
            }
            if (tran == null)
            {
                tran = this.Transaction;
            }
            return new BatchCommander(this, option, tran);
        }
        #endregion

        #region SqlMap
        public SqlMap.SqlMapStatement QuerySqlMap(string scope, string sqlID, object param = null)
        {
            return Pure.Data.SqlMap.SqlMapManager.Instance.BuildSqlMapResult(this, scope, sqlID, param);
        }
        #endregion

        #region Track
        public Snapshot<T> Track<T>(T obj) where T : class
        {
            return Snapshotter.Track(this, obj);
        }
        #endregion

        #region Ado.Ext
        public int Insert(string tableName, IDictionary<string, object> parameters)
        {
            if (!OnInsertingInternal(new InsertContext(new { tableName = tableName, parameters = parameters })))
                return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Insert(tableName, parameters, out realParameters);

            return this.Execute(sql, realParameters);
        }
        public int Update(string tableName, IDictionary<string, object> parameters, IDictionary<string, object> conditions)
        {
            if (!OnUpdatingInternal(new UpdateContext(new { tableName = tableName, parameters = parameters, conditions = conditions })))
                return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Update(tableName, parameters, conditions, out realParameters);

            return this.Execute(sql, realParameters);
        }

        public int Delete(string tableName, IDictionary<string, object> conditions)
        {
            if (!OnDeletingInternal(new DeleteContext(new { tableName = tableName, conditions = conditions })))
                return 0;
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Delete(tableName, conditions, out realParameters);

            return this.Execute(sql, realParameters);
        }

        public IDataReader Query(string tableName, string[] columns, IDictionary<string, object> conditions, IList<ISort> sort)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Select(tableName, columns, conditions, sort, out realParameters);

            return this.ExecuteReader(sql, realParameters);
        }

        public int Count(string tableName, IDictionary<string, object> conditions)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

            return this.ExecuteScalar<int>(sql, realParameters);
        }

        public long LongCount(string tableName, IDictionary<string, object> conditions)
        {
            IDictionary<string, object> realParameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SqlCustomGenerator.Count(tableName, conditions, out realParameters);

            return this.ExecuteScalar<long>(sql, realParameters);
        }
        #endregion


        #region Multi-Relations
        public IEnumerable<TRet> Query<T1, T2, TRet>(Func<T1, T2, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);

        }
        public IEnumerable<TRet> Query<T1, T2, T3, TRet>(Func<T1, T2, T3, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, T3, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);

        }
        public IEnumerable<TRet> Query<T1, T2, T3, T4, TRet>(Func<T1, T2, T3, T4, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, T3, T4, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, TRet>(Func<T1, T2, T3, T4, T5, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, T3, T4, T5, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, TRet>(Func<T1, T2, T3, T4, T5, T6, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, T3, T4, T5, T6, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        }

        public IEnumerable<TRet> Query<T1, T2, T3, T4, T5, T6, T7, TRet>(Func<T1, T2, T3, T4, T5, T6, T7, TRet> cb, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.Query<T1, T2, T3, T4, T5, T6, T7, TRet>(sql, cb, param, transaction, buffered, splitOn, commandTimeout, commandType, this);
        }
        #endregion

        #region log
        public void Debug(string msg)
        {
            LogHelper.Debug(msg);
        }
        public void Error(Exception msg)
        {
            LogHelper.Error(msg);
        }
        public void Warning(string msg)
        {
            LogHelper.Warning(msg);
        }
        #endregion

        #region Queryable
        public SqlQuery<T> Queryable<T>( ) where T : class
        {
            return new SqlQuery<T>(this.FluentSqlBuilder);
        }

        #endregion

        #region Backup & Sql Gen & Migrate
        public string Backup<T>(BackupOption option) where T : class
        {
            return BackupHelper.Instance.Backup<T>(this, option);
        }

        public string GenerateCode( )
        {
             return CodeGenHelper.Instance.Gen(this );
        }

        public Migration.Framework.ITransformationProvider CreateTransformationProvider()
        {
            return DbMigrateService.Instance.CreateTransformationProviderByDatabaseType(this);
        }
        #endregion

        #region Share 分表分库

        #endregion

        #region UpdateOnly
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        public int UpdateWithOnlyParams<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class
        {
            List<string> onlyList = ReflectionHelper.GetAllPropertyNames(onlyProperties);//  new List<string>();

            return UpdateOnly<T>(bindedObj, _PrimaryKeyValues, onlyList.ToArray());
        }

        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        public int UpdateOnly<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        {
            var sqlParamsContext =UpdateOnlyImpl<T>(bindedObj, _PrimaryKeyValues, onlyProperties);
            return this.Execute(sqlParamsContext.Sql , sqlParamsContext.Parameters);

        }

        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        internal ExecuteSqlParamsContext UpdateOnlyImpl<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        {
            if (bindedObj == null)
            {
                throw new ArgumentException("bindedObj 不能为空！");
            }
            var pd = GetMap<T>();// pocoData;

            var columns = onlyProperties;
            object poco = bindedObj;// trackedObject;
            //int defaultValue = 0;
            if (columns != null && !columns.Any())
                throw new ArgumentException("onlyProperties 不能为空！");

            var sb = new StringBuilder();
            var index = 0;
            string paramPrefix = "";
            var pkCols = pd.Properties.Where(p => p.IsPrimaryKey).ToList();
            IDictionary<string, object> PrimaryKeyValues = new Dictionary<string, object>();
            if (_PrimaryKeyValues != null && _PrimaryKeyValues.Count > 0)
            {
                PrimaryKeyValues = _PrimaryKeyValues;
            }
            else
            {
                if (pkCols == null || pkCols.Count == 0)
                {
                    throw new ArgumentException("当前对象没有主键情况下，需要指定更新的主键值");
                }
                else
                {

                    var Properties = pd.Properties.Select(p => p.PropertyInfo);// poco.GetType().GetProperties();
                    foreach (var pk in pkCols)
                    {
                        string pName = pk.Name;
                        string pColumnName = pk.Name;//  pk.ColumnName;
                        var o = Properties.FirstOrDefault(y => string.Equals(pName, y.Name, StringComparison.OrdinalIgnoreCase));
                        if (o != null)
                        {
                            var pValue = o.GetValue(poco, null);
                            PrimaryKeyValues.Add(pColumnName, pValue);

                        }

                    }
                }
            }


            IDictionary<string, object> parameters = new Dictionary<string, object>();


            var waitToUpdated = pd.Properties.Where(p => columns.Contains(p.ColumnName) || p.IsVersionColumn == true);
            foreach (var pocoColumn in waitToUpdated)
            {
                string properyName = pocoColumn.Name;//pocoColumn.ColumnName;

                // Don't update the primary key, but grab the value if we don't have it
                if (_PrimaryKeyValues == null && PrimaryKeyValues.ContainsKey(properyName))
                {
                    continue;
                }

                // Dont update result only columns
                if (pocoColumn.Ignored || pocoColumn.IsReadOnly || pocoColumn.IsPrimaryKey)
                    continue;

                var colType = pocoColumn.PropertyInfo.PropertyType;

                object value = pocoColumn.PropertyInfo.GetValue(poco, null);// GetColumnValue(pd, poco, ProcessMapper);
                if (pocoColumn.IsVersionColumn)
                {
                    if (colType == typeof(int) || colType == typeof(Int32))
                    {
                        value = Convert.ToInt32(value) + 1;


                    }
                    else if (colType == typeof(long) || colType == typeof(Int64))
                    {
                        value = Convert.ToInt64(value) + 1;


                    }
                    else if (colType == typeof(short) || colType == typeof(Int16))
                    {
                        value = Convert.ToInt16(value) + 1;

                    }
                }
                //else
                //{
                //    if (Snapshotter.GetAutoFilterEmptyValueColumnsWhenTrack())
                //    {
                //        var defaultValueOfCol = ReflectionHelper.GetDefaultValueForType(colType);
                //        if (object.Equals(defaultValueOfCol, value))//过滤空值列
                //        {
                //            continue;
                //        }
                //    }
                //}



                // Build the sql
                if (index > 0)
                    sb.Append(", ");

                //string paramName = paramPrefix + index.ToString();// pocoColumn.Name;
                string paramName = paramPrefix + properyName;
                sb.AppendFormat("{0} = {1}{2}", this.SqlGenerator.GetColumnName(pd, pocoColumn, false), this.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramName);

                parameters.Add(paramName, value);
                index++;

            }


            if (columns != null && columns.Any() && sb.Length == 0)
                throw new ArgumentException("There were no columns in the columns list that matched your table", "columns");

            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", this.SqlGenerator.GetTableName(pd), sb, BuildWhereSql(this, pd, PrimaryKeyValues, paramPrefix, ref index));
            int temIndex = parameters.Count;
            foreach (var item in PrimaryKeyValues)
            {
                parameters.Add(paramPrefix + item.Key, item.Value);
                //parameters.Add(paramPrefix + temIndex, item.Value);
                temIndex++;
            }


            


            if (this.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
            {
                IDictionary<string, object> dictParameters = parameters;
                int convertCount = LobConverter.UpdateDynamicParameterForLobColumn(pd, dictParameters);
                if (convertCount > 0)
                {
                    //return this.Execute(sql, dictParameters);
                    return new ExecuteSqlParamsContext(sql, dictParameters);
                }
                else
                {
                    //执行原始SQL
                    //return this.Execute(sql, parameters);
                    return new ExecuteSqlParamsContext(sql, parameters);

                }
            }
            else
            {
                //执行原始SQL
                //return this.Execute(sql, parameters);
                return new ExecuteSqlParamsContext(sql, parameters);

            }


        }


#if ASYNC
        /// <summary>
        /// 更新对象指定列的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        public async Task<int> UpdateWithOnlyParamsAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] onlyProperties) where T : class
        {
            List<string> ignoreList = ReflectionHelper.GetAllPropertyNames(onlyProperties);//  new List<string>();

            return await UpdateOnlyAsync<T>(bindedObj, _PrimaryKeyValues, ignoreList.ToArray());
        }
        /// <summary>
        /// 更新对象变更的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="onlyProperties"></param>
        /// <returns></returns>
        public async Task<int> UpdateOnlyAsync<T>(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] onlyProperties) where T : class
        {
            var sqlParamsContext = UpdateOnlyImpl<T>(bindedObj, _PrimaryKeyValues, onlyProperties);
            return await this.ExecuteAsync(sqlParamsContext.Sql, sqlParamsContext.Parameters);
        }

#endif

        private string BuildWhereSql(IDatabase db, IClassMapper pd, IDictionary<string, object> primaryKeyValuePair, string paramPrefix, ref int index)
        {
            var tempIndex = index;
            //return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) : string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false), db.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramPrefix + (tempIndex + i).ToString())).ToArray());

            return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ?
            string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) :
            string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false),
            db.SqlGenerator.Configuration.Dialect.ParameterPrefix,
            paramPrefix + x.Key
            )
            ).ToArray());

        }


        #endregion
    }

}
