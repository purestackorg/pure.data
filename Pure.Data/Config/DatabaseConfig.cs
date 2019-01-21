using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace Pure.Data
{
    public class DatabaseConfig : IDatabaseConfig
    {
        private List<IInterceptor> _interceptors = new List<IInterceptor>();
        //拦截器
        public List<IInterceptor> Interceptors { get { return _interceptors; } }
        string _ParameterPrefix = "$";
        string _ParameterSuffix = "";
        string _GlobalTablePrefix = "";

        public string ParameterPrefix { get { return _ParameterPrefix; } set { _ParameterPrefix = value; } }
         
        public string ParameterSuffix { get { return _ParameterSuffix; } set { _ParameterSuffix = value; } }
        public string GlobalTablePrefix { get { return _GlobalTablePrefix; } set { _GlobalTablePrefix = value; } }


        int _ExecuteTimeout = 30;

        public int ExecuteTimeout { get { return _ExecuteTimeout; } set { _ExecuteTimeout = value; } }
        /// <summary>
        ///  Set to true to keep the first opened connection alive until this object is disposed
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        bool initByPureDataConfiguration = false;
         
        public bool InitByPureDataConfiguration
        {
            get { return initByPureDataConfiguration; }
            set { initByPureDataConfiguration = value; }
        }

        /// <summary>
        /// 是否当验证出现错误即可停止
        /// </summary>
        private bool _ValidateStopOnFirstFailure = false;
        public bool ValidateStopOnFirstFailure
        {
            get
            {
                return _ValidateStopOnFirstFailure;
            }
            set
            {
                _ValidateStopOnFirstFailure = value;
            }
        }

        bool autoDisposeConnection = true;

        /// <summary>
        /// 是否自动释放Connection对象，false则只保留一个Connection对象
        /// </summary>
        public bool AutoDisposeConnection
        {
            get { return autoDisposeConnection; }
            set { autoDisposeConnection = value; }
        }

        bool enbleDebug = true;
        public bool EnableDebug
        {
            get { return enbleDebug; }
            set { enbleDebug = value; }
        }
        bool enableIntercept = true;
        public bool EnableIntercept
        {
            get { return enableIntercept; }
            set { enableIntercept = value; }
        }

        bool enbleCacheOrm = false;
        public bool EnableOrmCache
        {
            get { return enbleCacheOrm; }
            set { enbleCacheOrm = value; }

        }
        bool enbleLogError = true;
        public bool EnableLogError
        {
            get { return enbleLogError; }
            set { enbleLogError = value; }
        }
        int cacheOrmTime = 60;
        /// <summary>实体缓存过期时间，默认60秒</summary>
        public int CacheOrmTime
        {
            get { return cacheOrmTime; }
            set { cacheOrmTime = value; }

        }
        int cacheOrmSingleTime = 60;
        /// <summary>单对象缓存过期时间，默认60秒</summary>
        public int CacheOrmSingleTime
        {
            get { return cacheOrmSingleTime; }
            set { cacheOrmSingleTime = value; }

        }

        int ormCacheCheckPeriod = 5;
        /// <summary>缓存维护定时器的检查周期，默认5秒</summary>
        public int OrmCacheCheckPeriod
        {
            get { return ormCacheCheckPeriod; }
            set { ormCacheCheckPeriod = value; }

        }

        bool enableOrmLog = true;
        /// <summary>
        /// 是否启用自定义日志输出
        /// </summary>
        public bool EnableOrmLog
        {
            get { return enableOrmLog; }
            set { enableOrmLog = value; }
        }
        bool _EnableInternalLog = false;
        /// <summary>
        /// 是否启用内部日志输出
        /// </summary>
        public bool EnableInternalLog
        {
            get { return _EnableInternalLog; }
            set { _EnableInternalLog = value; }
        }
        bool categoryLogType = false;
        /// <summary>
        /// 是否用原始格式化SQL输出到日志
        /// </summary>
        public bool LogWithRawSql { get; set; } = false;
        /// <summary>
        /// 是否按日志类型保存日志文件
        /// </summary>
        public bool CategoryLogType
        {
            get { return categoryLogType; }
            set { categoryLogType = value; }
        }

        OutputActionDelegate outputAction = (str, ex, type) =>
        {
            ConsoleHelper.Instance.OutputMessage(str, ex, type);
        };
        public OutputActionDelegate OutputAction
        {
            get { return outputAction; }
            set { outputAction = value; }
        }

        string ormLogsPath = System.IO.Path.Combine( PathHelper.GetBaseDirectory(), "logs"); 
        /// <summary>
        /// 日志输出路径
        /// </summary>
        public string OrmLogsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ormLogsPath))
                {
                    if (!System.IO.Directory.Exists(ormLogsPath))
                    {
                        System.IO.Directory.CreateDirectory(ormLogsPath);
                    }
                }
                return ormLogsPath;
            }
            set { ormLogsPath = value; }

        }
        int maxServerLogSize = 10000;
        /// <summary>最大日志行数</summary>
        public int MaxServerLogSize
        {
            get { return maxServerLogSize; }
            set { maxServerLogSize = value; }

        }
        int defaultPageSize = 10;
        /// <summary>默认分页大小</summary>
        public int DefaultPageSize
        {
            get { return defaultPageSize; }
            set { defaultPageSize = value; }

        }


        LoadMapperMode _LoadMapperMode  = LoadMapperMode.FluentMapper;
        /// <summary>
        /// 加载Mapper 的模式: 1：fluent mapper ， 2：Attribute  ， 3：sql map
        /// </summary>
        public LoadMapperMode LoadMapperMode
        {
            get { return _LoadMapperMode; }
            set { _LoadMapperMode = value; }

        }
         
        List<Assembly> _MappingAssemblies = new List<Assembly>();
        public List<Assembly> MappingAssemblies
        {
            get
            {
                return _MappingAssemblies;
            }
            set
            {
                _MappingAssemblies = value;
            }
        }


        List<DataSource> _DataSources = new List<DataSource>();
        public List<DataSource> DataSources
        {
            get
            {
                return _DataSources;
            }
            set
            {
                _DataSources = value;
            }
        }

        private bool _IsWatchConfigFile = false;
        /// <summary>
        /// 是否监听Sql map配置文件
        /// </summary>
        public bool IsWatchSqlMapFile
        {
            get
            {
                return _IsWatchConfigFile;
            }
            set
            {
               _IsWatchConfigFile = value;
            }
        }
        private int _WatchConfigInterval = 5000;
        /// <summary>
        /// 监听sql map文件间隔时间
        /// </summary>
        public int WatchSqlMapInterval
        {
            get
            {
                return _WatchConfigInterval;
            }
            set
            {
                _WatchConfigInterval = value;
            }
        }

        List<string> _SqlMapPaths = new List<string>();

        public List<string> SqlMapFilePaths
        {
            get
            {
                return _SqlMapPaths;
            }
            set
            {
                _SqlMapPaths = value;
            }
        }
        List<string> _SqlMapDirs = new List<string>();

        /// <summary>
        /// sql map所在目录路径
        /// </summary>
        public List<string> SqlMapDirPaths
        {
            get
            {
                return _SqlMapDirs;
            }
            set
            {
                _SqlMapDirs = value;
            }
        }
        /// <summary>
        /// 输出Sql map加载日志代理
        /// </summary>
        public Action<string> OutputSqlMapLoaderLogs { get; set; }
         
        private bool _EnableSqlMap = false; 
        public bool EnableSqlMap
        {
            get
            {
                return _EnableSqlMap;
            }
            set
            {
                _EnableSqlMap = value;
            }
        }
        public string NameSpacePrefix { get; set; }
        private bool _FormatSql = false;
        /// <summary>
        /// 是否启用SQL格式化
        /// </summary>
        public bool FormatSql
        {
            get
            {
                return _FormatSql;
            }
            set
            {
                _FormatSql = value;
            }
        }
        private bool _CanUpdatedWhenTableExisted = false;
        /// <summary>
        /// 当表存在时候能否更新
        /// </summary>
        public bool CanUpdatedWhenTableExisted
        {
            get
            {
                return _CanUpdatedWhenTableExisted;
            }
            set
            {
                _CanUpdatedWhenTableExisted = value;
            }
        }
       

        private bool _AutoMigrate = false;
        public bool AutoMigrate
        {
            get
            {
                return _AutoMigrate;
            }
            set
            {
                _AutoMigrate = value;
            }
        }

        private bool _EnableAutoMigrateLog = false;
        public bool EnableAutoMigrateLog
        {
            get
            {
                return _EnableAutoMigrateLog;
            }
            set
            {
                _EnableAutoMigrateLog = value;
            }
        }

        private bool _EnableAutoMigrateDebug = false;
        public bool EnableAutoMigrateDebug
        {
            get
            {
                return _EnableAutoMigrateDebug;
            }
            set
            {
                _EnableAutoMigrateDebug = value;
            }
        }
        private string _AutoMigrateOnContainTable = "";
        /// <summary>
        /// 指定某些表能自动迁移（以;分割）
        /// </summary>
        public string AutoMigrateOnContainTable
        {
            get
            {
                return _AutoMigrateOnContainTable;
            }
            set
            {
                _AutoMigrateOnContainTable = value;
            }
        }

        private string _AutoMigrateWithoutContainTable = ""; 
        public string AutoMigrateWithoutTable
        {
            get
            {
                return _AutoMigrateWithoutContainTable;
            }
            set
            {
                _AutoMigrateWithoutContainTable = value;
            }
        }
        private bool _AutoRemoveUnuseColumnInTable = false;
        /// <summary>
        /// 自动清空没用的属性列，用于CodeFirst模式
        /// </summary>
        public bool AutoRemoveUnuseColumnInTable
        {
            get
            {
                return _AutoRemoveUnuseColumnInTable;
            }
            set
            {
                _AutoRemoveUnuseColumnInTable = value;
            }
        }

        System.Func<System.Data.IDbConnection, System.Data.IDbConnection> _DbConnectionInit;
        public System.Func<System.Data.IDbConnection, System.Data.IDbConnection> DbConnectionInit
        {
            get
            {
                return _DbConnectionInit;
            }
            set
            {
                _DbConnectionInit = value;
            }
        }


        private bool _EnableGlobalIgnoreUpdatedColumns = true;
        public bool EnableGlobalIgnoreUpdatedColumns
        {
            get
            {
                return _EnableGlobalIgnoreUpdatedColumns;
            }
            set
            {
                _EnableGlobalIgnoreUpdatedColumns = value;
            }
        }

        private bool _AutoFilterEmptyValueColumnsWhenTrack = true;
        /// <summary>
        /// 是否自动过滤空值列的更新
        /// </summary>
        public bool AutoFilterEmptyValueColumnsWhenTrack
        {
            get
            {
                return _AutoFilterEmptyValueColumnsWhenTrack;
            }
            set
            {
                _AutoFilterEmptyValueColumnsWhenTrack = value;
            }
        }
       

        private string _GlobalIgnoreUpdatedColumns = "";
        /// <summary>
        /// Track模式下,全局过滤指定更新列名（以;分割）
        /// </summary>
        public string GlobalIgnoreUpdatedColumns
        {
            get
            {
                return _GlobalIgnoreUpdatedColumns;
            }
            set
            {
                _GlobalIgnoreUpdatedColumns = value;
            }
        }





        private bool _EnableConnectionPool = false;
        /// <summary>
        /// 是否启用数据库连接池
        /// </summary>
        public bool EnableConnectionPool
        {
            get
            {
                return _EnableConnectionPool;
            }
            set
            {
                _EnableConnectionPool = value;
            }
        }


        private bool _EnableLogConnectionPool = false;
        /// <summary>
        /// 是否开启连接池日志,需要先启用EnableDebug
        /// </summary>
        public bool EnableLogConnectionPool
        {
            get
            {
                return _EnableLogConnectionPool;
            }
            set
            {
                _EnableLogConnectionPool = value;
            }
        }

        private int _ConnectionPoolMinSize = 0;

        /// <summary>
        /// 连接池最小数量
        /// </summary>
        public int MinIdle
        {
            get
            {
                return _ConnectionPoolMinSize;
            }
            set
            {
                _ConnectionPoolMinSize = value;
            }
        }

        private int _ConnectionPoolMaxSize = 8;

        /// <summary>
        /// 连接池最大数量
        /// </summary>
        public int MaxIdle
        {
            get
            {
                return _ConnectionPoolMaxSize;
            }
            set
            {
                _ConnectionPoolMaxSize = value;
            }
        }

        /// <summary>
        /// 连接池初始化步长
        /// </summary>
        private int _ConnectionPoolStepSize = 0;
        /// <summary>
        /// 连接池初始化步长
        /// </summary>
        public int InitialSize
        {
            get
            {
                return _ConnectionPoolStepSize;
            }
            set
            {
                _ConnectionPoolStepSize = value;
            }
        }

        /// <summary>
        ///  配置获取连接等待超时的时间,以毫秒为单位，在抛出异常之前，池等待连接被回收的最长时间（当没有可用连接时）。设置为-1表示无限等待。
        ///  最大等待时间，当没有可用连接时，连接池等待连接释放的最大时间，超过该时间限制会抛出异常，如果设置-1表示无限等待（默认为无限，建议调整为60000ms，避免因线程池不够用，而导致请求被无限制挂起）
        /// </summary>
        public long MaxWaitMillis { get; set; } = -1;
        /// <summary>
        ///  连接池中可同时连接的最大的连接数，默认-1全部激活不限制
        /// </summary>
        public int MaxTotal { get; set; } = -1;
        /// <summary>
        /// 失效检查线程运行时间间隔，如果小于等于-1，不会启动检查线程，默认-1
        /// </summary>
        public long TimeBetweenEvictionRunsMillis { get; set; } = -1;
        /// <summary>
        ///  配置一个连接在池中最小生存的时间，单位是毫秒，默认30分钟
        ///  连接池中连接，在时间段内一直空闲， 被逐出连接池的时间
        /// </summary>
        public long MinEvictableIdleTimeMillis { get; set; } = 1800000;
        /// <summary>
        /// 是否启用失效清除对象
        /// </summary>
        public bool EnableRemoveAbandoned { get; set; } = true;
        /// <summary>
        /// 是否在获取对象连接时,自动检测是否含有失效的连接，如果有则执行清除
        /// </summary>
        public bool RemoveAbandonedOnBorrow { get; set; } = true;
        /// <summary>
        /// 是否运行期间，自动检测是否含有失效的连接，如果有则执行清除
        /// </summary>
        public bool RemoveAbandonedOnMaintenance { get; set; } = true;
        /// <summary>
        ///  自动回收超时时间(以秒数为单位)，默认300 
        ///  超过时间限制，回收没有用(废弃)的连接（建议调整为180）
        /// </summary>
        public int RemoveAbandonedTimeout { get; set; } = 300;


        /// <summary>
        ///  代表每次检查链接的数量，默认3，建议设置和maxActive一样大，这样每次可以有效检查所有的链接.
        /// </summary>
        public int NumTestsPerEvictionRun { get; set; } = 3;
        /// <summary>
        /// 软删除失效驱逐的时间，默认-1，单位毫秒
        /// </summary>
        public long SoftMinEvictableIdleTimeMillis { get; set; } = -1;
        /// <summary>
        ///   取得对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnBorrow { get; set; } = false;
        /// <summary>
        ///   创建对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnCreate { get; set; } = false;
        /// <summary>
        /// 返回对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnReturn { get; set; } = false;
        /// <summary>
        ///  空闲时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestWhileIdle { get; set; } = false;
        /// <summary>
        /// SQL查询,用来验证从连接池取出的连接,在将连接返回给调用者之前.如果指定,则查询必须是一个SQL SELECT并且必须返回至少一行记录
        /// </summary>
        public string ValidationQuery { get; set; } = "";





        bool _EnableLobConverter = false;
        /// <summary>
        /// 是否启用Lob类型转换器（Clob和Blob）
        /// </summary>
        public bool EnableLobConverter
        {
            get { return _EnableLobConverter; }
            set { _EnableLobConverter = value; }
        }
        string _LobConverterClassName = "";
        /// <summary>
        ///  Lob类型转换器接口实现类
        /// </summary>
        public string LobConverterClassName
        {
            get { return _LobConverterClassName; }
            set { _LobConverterClassName = value; }
        }

        #region 代码生成设置
        private bool _EnableCodeGen = false;

        /// <summary>
        /// 是否启用代码生成
        /// </summary> 
        public bool EnableCodeGen
        {
            get { return _EnableCodeGen; }
            set { _EnableCodeGen = value; }
        }
        private CodeGenType _CodeGenType = CodeGenType.CodeFirst;

        /// <summary>
        /// 代码生成类型
        /// </summary> 
        public CodeGenType CodeGenType
        {
            get { return _CodeGenType; }
            set
            {
                _CodeGenType = value;
            }
        }
        private CodeGenClassNameMode _ClassNameMode = CodeGenClassNameMode.UpperAll;

        /// <summary>
        /// 类名显示模式
        /// </summary> 
        public CodeGenClassNameMode CodeGenClassNameMode
        {
            get { return _ClassNameMode; }
            set
            {
                _ClassNameMode = value;
            }
        }
        private CodeGenClassNameMode _CodeGenPropertyNameMode = CodeGenClassNameMode.Origin;

        /// <summary>
        /// 属性名显示模式
        /// </summary> 
        public CodeGenClassNameMode CodeGenPropertyNameMode
        {
            get { return _CodeGenPropertyNameMode; }
            set
            {
                _CodeGenPropertyNameMode = value;
            }
        }
         
    private string _CodeGenProjectName = "";
        /// <summary>
        ///  项目名称（英文缩写）
        /// </summary> 
        public string CodeGenProjectName
        {
            get { return _CodeGenProjectName; }
            set { _CodeGenProjectName = value; }
        }


        private string _CodeGenNameSpace = "";
        /// <summary>
        ///  代码生成命名空间前缀
        /// </summary> 
        public string CodeGenNameSpace
        {
            get { return _CodeGenNameSpace; }
            set { _CodeGenNameSpace = value; }
        }
        private string _CodeGenBaseDirectory = PathHelper.GetBaseDirectory();
 
        public string CodeGenBaseDirectory
        {
            get { return _CodeGenBaseDirectory; }
            set { _CodeGenBaseDirectory = value; }
        }
        private string _CodeGenTableFilter = "A_;B_;C_;D_;T_;TB_";
        /// <summary>
        /// 代码生成过滤表名前缀
        /// </summary> 
        public string CodeGenTableFilter
        {
            get { return _CodeGenTableFilter; }
            set { _CodeGenTableFilter = value; }
        }
        private List<CodeGenTemplate> _CodeGenTemplates = new List<CodeGenTemplate>();
        /// <summary>
        /// 代码生成模板
        /// </summary>
        public List<CodeGenTemplate> CodeGenTemplates { get { return _CodeGenTemplates; }set { _CodeGenTemplates = value; } }
        #endregion


        public override string ToString()
        {
            return @"-----------DatabaseConfig-----------
                    ParameterPrefix=" + ParameterPrefix + @" 
                    ParameterSuffix=" + ParameterSuffix + @"  
                    GlobalTablePrefix=" + GlobalTablePrefix + @"  
                    ExecuteTimeout=" + ExecuteTimeout + @"
                    DefaultPageSize=" + DefaultPageSize + @"
                    AutoDisposeConnection=" + AutoDisposeConnection + @"
                    ValidateStopOnFirstFailure=" + ValidateStopOnFirstFailure + @"
                    LoadMapperMode=" + LoadMapperMode + @"
                    EnableLobConverter=" + EnableLobConverter + @"
                    LobConverterClassName=" + LobConverterClassName + @"
    
                    EnableDebug=" + EnableDebug + @"
                    EnableIntercept=" + EnableIntercept + @"
                    EnableLogError=" + EnableLogError + @"
                    EnableOrmLog=" + EnableOrmLog + @"
                    EnableInternalLog=" + EnableInternalLog + @"
                    LogWithRawSql=" + LogWithRawSql + @"
                    CategoryLogType=" + CategoryLogType + @"
                    OrmLogsPath=" + OrmLogsPath + @"
                    MaxServerLogSize=" + MaxServerLogSize + @"
    
                    EnableOrmCache=" + EnableOrmCache + @"
                    CacheOrmTime=" + CacheOrmTime + @"
                    CacheOrmSingleTime=" + CacheOrmSingleTime + @"
                    OrmCacheCheckPeriod=" + OrmCacheCheckPeriod + @"
    
                    EnableSqlMap=" + EnableSqlMap + @"
                    NameSpacePrefix=" + NameSpacePrefix + @"
                    CanUpdatedWhenTableExisted=" + CanUpdatedWhenTableExisted + @"
                    FormatSql=" + FormatSql + @"
                    IsWatchSqlMapFile=" + IsWatchSqlMapFile + @"
                    WatchSqlMapInterval=" + WatchSqlMapInterval + @"
                    SqlMapDirPaths=" + string.Join(";", SqlMapDirPaths.Select(p => p)) + @"
                    SqlMapFilePaths=" + string.Join(";", SqlMapFilePaths.Select(p => p)) + @"
    
                    AutoMigrate=" + AutoMigrate + @"
                    AutoRemoveUnuseColumnInTable=" + AutoRemoveUnuseColumnInTable + @"
                    EnableAutoMigrateLog=" + EnableAutoMigrateLog + @"
                    EnableAutoMigrateDebug=" + EnableAutoMigrateDebug + @"
                    AutoMigrateOnContainTable=" + AutoMigrateOnContainTable + @"
                    AutoMigrateWithoutTable=" + AutoMigrateWithoutTable + @"
                    EnableGlobalIgnoreUpdatedColumns=" + EnableGlobalIgnoreUpdatedColumns + @"
                    AutoFilterEmptyValueColumnsWhenTrack=" + AutoFilterEmptyValueColumnsWhenTrack + @"
                    GlobalIgnoreUpdatedColumns=" + GlobalIgnoreUpdatedColumns + @"

                    EnableCodeGen=" + EnableCodeGen + @"
                    CodeGenType=" + CodeGenType + @"
                    CodeGenBaseDirectory=" + CodeGenBaseDirectory + @"
                    CodeGenClassNameMode=" + CodeGenClassNameMode + @"
                    CodeGenPropertyNameMode=" + CodeGenPropertyNameMode + @"
                    CodeGenProjectName=" + CodeGenProjectName + @"
                    CodeGenNameSpace=" + CodeGenNameSpace + @"
                    CodeGenTableFilter=" + CodeGenTableFilter + @"
    
                    EnableConnectionPool=" + EnableConnectionPool + @"
                    EnableLogConnectionPool=" + EnableLogConnectionPool + @"
                    MinIdle=" + MinIdle + @"
                    MaxIdle=" + MaxIdle + @"
                    MaxTotal=" + MaxTotal + @"
                    InitialSize=" + InitialSize + @"
                    MaxWaitMillis=" + MaxWaitMillis + @"
                    TimeBetweenEvictionRunsMillis=" + TimeBetweenEvictionRunsMillis + @"
                    MinEvictableIdleTimeMillis=" + MinEvictableIdleTimeMillis + @"
                    SoftMinEvictableIdleTimeMillis=" + SoftMinEvictableIdleTimeMillis + @"
                    EnableRemoveAbandoned=" + EnableRemoveAbandoned + @"
                    RemoveAbandonedOnBorrow=" + RemoveAbandonedOnBorrow + @"
                    RemoveAbandonedOnMaintenance=" + RemoveAbandonedOnMaintenance + @"
                    RemoveAbandonedTimeout=" + RemoveAbandonedTimeout + @"
                    NumTestsPerEvictionRun=" + NumTestsPerEvictionRun + @"
                    ValidationQuery=" + ValidationQuery + @"
                    TestOnBorrow=" + TestOnBorrow + @"
                    TestOnCreate=" + TestOnCreate + @"
                    TestOnReturn=" + TestOnReturn + @"
                    TestWhileIdle=" + TestWhileIdle + @"
                          
                    Interceptors=" + string.Join( ";", Interceptors.Select(p=> (p!=null? p.ToString():""))) + @"
                    MappingAssemblies=" + string.Join(";", MappingAssemblies.Select(p => (p != null ? p.ToString() : ""))) + @"
                    DataSources=" + string.Join(";", DataSources.Select(p => p.Name + "_" + p.Type + "_" + p.Weight)) + @"
                    CodeGenTemplates=" + string.Join(";", CodeGenTemplates.Select(p => p.Name + "_" + p.OutputType + "_" + p.Enabled + "_" + p.OutputDirectory)) + @"
                    OutputAction=" + OutputAction + @"
                    DbConnectionInit=" + (DbConnectionInit != null? DbConnectionInit.ToString():"") + @"
                    -----------Load DatabaseConfig End-----------"+
                    @""
                                             
                                             ;
        }
 

    }

    /// <summary>
    /// 加载mapper方式
    /// </summary>
    public enum LoadMapperMode
    {
        
        FluentMapper =  1,
        AttributeMapper =  2,
        SqlMapper =  3,
        All = 10,
    }
}
