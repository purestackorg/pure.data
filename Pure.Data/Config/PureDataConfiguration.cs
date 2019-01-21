using Pure.Data.SqlMap;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pure.Data
{
    /// <summary>
    /// 数据库配置文件
    /// </summary>
    [XmlRoot(Namespace = "http://PureData.net/schemas/PureDataConfiguration.xsd")]
    public class PureDataConfiguration
    {

        [XmlIgnore]
        public String Path { get; set; }
        public Settings Settings { get; set; }

        [XmlArray("DataSources")]
        [XmlArrayItem("DataSource")]
        public List<DataSource> DataSources { get; set; }

        [XmlArray("MapperSources")]
        [XmlArrayItem("MapperSource")]
        public List<MapperSource> MapperSources { get; set; }

        [XmlArray("SqlMaps")]
        [XmlArrayItem("SqlMap")]
        public List<SqlMapSource> SqlMapSources { get; set; }

        [XmlArray("Interceptors")]
        [XmlArrayItem("Interceptor")]
        public List<InterceptorSource> Interceptors { get; set; }


        [XmlArray("CodeGenTemplates")]
        [XmlArrayItem("CodeGenTemplate")]
        public List<CodeGenTemplate> CodeGenTemplates { get; set; }


         
    }

    public class Settings
    {


        private string _ParameterPrefix = "$";
        [XmlAttribute]
        public string ParameterPrefix { get { return _ParameterPrefix; } set { _ParameterPrefix = value; } }
        [XmlAttribute]
        public string ParameterSuffix { get; set; }

        [XmlAttribute]
        public string GlobalTablePrefix { get; set; }

        int _ExecuteTimeout = 30;
        [XmlAttribute]
        public int ExecuteTimeout { get { return _ExecuteTimeout; } set { _ExecuteTimeout = value; } }
        /// <summary>
        ///  Set to true to keep the first opened connection alive until this object is disposed
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        bool autoDisposeConnection = true;

        /// <summary>
        /// 是否自动释放Connection对象，false则只保留一个Connection对象
        /// </summary>
        /// 
        [XmlAttribute]
        public bool AutoDisposeConnection
        {
            get { return autoDisposeConnection; }
            set { autoDisposeConnection = value; }
        }


        private bool _ValidateStopOnFirstFailure = false;
        /// <summary>
        /// 是否当验证出现错误即可停止
        /// </summary>
        [XmlAttribute]
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

        bool enbleDebug = true;
        [XmlAttribute]
        public bool EnableDebug
        {
            get { return enbleDebug; }
            set { enbleDebug = value; }
        }
        bool enableIntercept = true;
        [XmlAttribute]
        public bool EnableIntercept
        {
            get { return enableIntercept; }
            set { enableIntercept = value; }
        }

        bool enbleCacheOrm = false;
        [XmlAttribute]
        public bool EnableOrmCache
        {
            get { return enbleCacheOrm; }
            set { enbleCacheOrm = value; }

        }
        bool enbleLogError = true;
        [XmlAttribute]
        public bool EnableLogError
        {
            get { return enbleLogError; }
            set { enbleLogError = value; }
        }
        int cacheOrmTime = 60;
        /// <summary>实体缓存过期时间，默认60秒</summary>
        /// 
        [XmlAttribute]
        public int CacheOrmTime
        {
            get { return cacheOrmTime; }
            set { cacheOrmTime = value; }

        }
        int cacheOrmSingleTime = 60;
        /// <summary>单对象缓存过期时间，默认60秒</summary>
        /// 
        [XmlAttribute]
        public int CacheOrmSingleTime
        {
            get { return cacheOrmSingleTime; }
            set { cacheOrmSingleTime = value; }

        }

        int ormCacheCheckPeriod = 5;
        /// <summary>缓存维护定时器的检查周期，默认5秒</summary>
        /// 
        [XmlAttribute]
        public int OrmCacheCheckPeriod
        {
            get { return ormCacheCheckPeriod; }
            set { ormCacheCheckPeriod = value; }

        }

        bool enableOrmLog = true;
        [XmlAttribute]
        /// <summary>
        /// 是否启用自定义日志输出
        /// </summary>
        public bool EnableOrmLog
        {
            get { return enableOrmLog; }
            set { enableOrmLog = value; }
        }
        bool _EnableInternalLog = false;
        [XmlAttribute]
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
        /// 是否按日志类型保存日志文件
        /// </summary>
        /// 
        [XmlAttribute]
        public bool CategoryLogType
        {
            get { return categoryLogType; }
            set { categoryLogType = value; }
        }

        string ormLogsPath = PathHelper.GetBaseDirectory() + @"\logs";
        /// <summary>
        /// 日志输出路径
        /// </summary>
        /// 
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
        public int MaxServerLogSize
        {
            get { return maxServerLogSize; }
            set { maxServerLogSize = value; }

        }
        int defaultPageSize = 10;
        /// <summary>默认分页大小</summary>
        /// 
        [XmlAttribute]
        public int DefaultPageSize
        {
            get { return defaultPageSize; }
            set { defaultPageSize = value; }

        }


        LoadMapperMode _LoadMapperMode = LoadMapperMode.FluentMapper;
        /// <summary>
        /// 加载Mapper 的模式: 1：fluent mapper ， 2：Attribute  ， 3：sql map
        /// </summary>
        /// 
        [XmlAttribute]
        public LoadMapperMode LoadMapperMode
        {
            get { return _LoadMapperMode; }
            set { _LoadMapperMode = value; }

        }

        private bool _IsWatchConfigFile = false;
        /// <summary>
        /// 是否监听Sql map配置文件
        /// </summary>
        /// 
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
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


        private bool _EnableSqlMap = false;

        [XmlAttribute]
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

        /// <summary>
        /// 是否启用SQL格式化
        /// </summary>
        /// 
        [XmlAttribute]
        public string NameSpacePrefix
        {
            get;
            set;
        }

        private bool _FormatSql = false;
        /// <summary>
        /// 是否启用SQL格式化
        /// </summary>
        /// 
        [XmlAttribute]
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
        /// <summary>
        /// 是否用原始格式化SQL输出到日志
        /// </summary>
        [XmlAttribute]
        public bool LogWithRawSql { get; set; } = false;

        private bool _AutoMigrate = false;

        [XmlAttribute]
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

        private bool _AutoRemoveUnuseColumnInTable = false;

        [XmlAttribute]
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
        private bool _CanUpdatedWhenTableExisted = false;

        [XmlAttribute]
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

        private bool _EnableAutoMigrateLog = false;
        [XmlAttribute]
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
        [XmlAttribute]
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
        /// 

        [XmlAttribute]
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
        [XmlAttribute]
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


        private bool _EnableGlobalIgnoreUpdatedColumns = true;
        [XmlAttribute]
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
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
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

        //private bool _EnableConnectionPool = false;
        ///// <summary>
        ///// 是否启用数据库连接池
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public bool EnableConnectionPool
        //{
        //    get
        //    {
        //        return _EnableConnectionPool;
        //    }
        //    set
        //    {
        //        _EnableConnectionPool = value;
        //    }
        //}


        //private bool _EnableLogConnectionPool = false;
        ///// <summary>
        ///// 是否开启连接池日志,需要先启用EnableDebug
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public bool EnableLogConnectionPool
        //{
        //    get
        //    {
        //        return _EnableLogConnectionPool;
        //    }
        //    set
        //    {
        //        _EnableLogConnectionPool = value;
        //    }
        //}

        //private int _ConnectionPoolMinSize = 0;

        ///// <summary>
        ///// 连接池最小数量
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public int MinIdle
        //{
        //    get
        //    {
        //        return _ConnectionPoolMinSize;
        //    }
        //    set
        //    {
        //        _ConnectionPoolMinSize = value;
        //    }
        //}

        //private int _ConnectionPoolMaxSize = 8;

        ///// <summary>
        ///// 连接池最大数量
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public int MaxIdle
        //{
        //    get
        //    {
        //        return _ConnectionPoolMaxSize;
        //    }
        //    set
        //    {
        //        _ConnectionPoolMaxSize = value;
        //    }
        //}

        ///// <summary>
        ///// 连接池初始化步长
        ///// </summary>
        ///// 

        //private int _ConnectionPoolStepSize = 0;
        ///// <summary>
        ///// 连接池初始化步长
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public int InitialSize
        //{
        //    get
        //    {
        //        return _ConnectionPoolStepSize;
        //    }
        //    set
        //    {
        //        _ConnectionPoolStepSize = value;
        //    }
        //}


        ///// <summary>
        /////  配置获取连接等待超时的时间,以毫秒为单位，在抛出异常之前，池等待连接被回收的最长时间（当没有可用连接时）。设置为-1表示无限等待。
        /////  最大等待时间，当没有可用连接时，连接池等待连接释放的最大时间，超过该时间限制会抛出异常，如果设置-1表示无限等待（默认为无限，建议调整为60000ms，避免因线程池不够用，而导致请求被无限制挂起）
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public long MaxWaitMillis { get; set; } = -1;
        ///// <summary>
        /////  连接池中可同时连接的最大的连接数，默认-1全部激活不限制
        ///// </summary>
        //[XmlAttribute]
        //public int MaxTotal { get; set; } = -1;
        ///// <summary>
        ///// 失效检查线程运行时间间隔，如果小于等于-1，不会启动检查线程，默认-1
        ///// </summary>
        //[XmlAttribute]
        //public long TimeBetweenEvictionRunsMillis { get; set; } = -1;
        ///// <summary>
        /////  配置一个连接在池中最小生存的时间，单位是毫秒，默认30分钟
        /////  连接池中连接，在时间段内一直空闲， 被逐出连接池的时间
        ///// </summary>
        //[XmlAttribute]
        //public long MinEvictableIdleTimeMillis { get; set; } = 1800000;
        ///// <summary>
        ///// 是否启用失效清除对象
        ///// </summary>
        //[XmlAttribute]
        //public bool EnableRemoveAbandoned { get; set; } = true;
        ///// <summary>
        ///// 是否在获取对象连接时,自动检测是否含有失效的连接，如果有则执行清除
        ///// </summary>
        //[XmlAttribute]
        //public bool RemoveAbandonedOnBorrow { get; set; } = true;
        ///// <summary>
        ///// 是否运行期间，自动检测是否含有失效的连接，如果有则执行清除
        ///// </summary>
        //[XmlAttribute]
        //public bool RemoveAbandonedOnMaintenance { get; set; } = true;
        ///// <summary>
        /////  自动回收超时时间(以秒数为单位)，默认300 
        /////  超过时间限制，回收没有用(废弃)的连接（建议调整为180）
        ///// </summary>
        //[XmlAttribute]
        //public int RemoveAbandonedTimeout { get; set; } = 300;
        ///// <summary>
        /////  代表每次检查链接的数量，默认3，建议设置和maxActive一样大，这样每次可以有效检查所有的链接.
        ///// </summary>
        //[XmlAttribute]
        //public int NumTestsPerEvictionRun { get; set; } = 3;
        ///// <summary>
        ///// 软删除失效驱逐的时间，默认-1，单位毫秒
        ///// </summary>
        //[XmlAttribute]
        //public long SoftMinEvictableIdleTimeMillis { get; set; } = -1;
        ///// <summary>
        /////   取得对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //[XmlAttribute]
        //public bool TestOnBorrow { get; set; } = false;
        ///// <summary>
        /////   创建对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //[XmlAttribute]
        //public bool TestOnCreate { get; set; } = false;
        ///// <summary>
        ///// 返回对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //[XmlAttribute]
        //public bool TestOnReturn { get; set; } = false;
        ///// <summary>
        /////  空闲时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //[XmlAttribute]
        //public bool TestWhileIdle { get; set; } = false;
        ///// <summary>
        ///// SQL查询,用来验证从连接池取出的连接,在将连接返回给调用者之前.如果指定,则查询必须是一个SQL SELECT并且必须返回至少一行记录
        ///// </summary>
        //[XmlAttribute]
        //public string ValidationQuery { get; set; } = "";





        private bool _EnableLobConverter = false;
        /// <summary>
        /// 是否启用Lob类型转换器（Clob和Blob）
        /// </summary>
        /// 
        [XmlAttribute]
        public bool EnableLobConverter
        {
            get { return _EnableLobConverter; }
            set { _EnableLobConverter = value; }
        }
        private string _LobConverterClassName = "";
        /// <summary>
        ///  Lob类型转换器接口实现类
        /// </summary>
        /// 
        [XmlAttribute]
        public string LobConverterClassName
        {
            get { return _LobConverterClassName; }
            set { _LobConverterClassName = value; }
        }


        #region CodeGen Setting
        private bool _EnableCodeGen = false;

        /// <summary>
        /// 是否启用代码生成
        /// </summary>
        /// 
        [XmlAttribute]
        public bool EnableCodeGen
        {
            get { return _EnableCodeGen; }
            set { _EnableCodeGen = value; }
        }
        private CodeGenType _CodeGenType = CodeGenType.CodeFirst;

        /// <summary>
        /// 代码生成类型
        /// </summary>
        /// 
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
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
        /// 
        [XmlAttribute]
        public string CodeGenProjectName
        {
            get { return _CodeGenProjectName; }
            set { _CodeGenProjectName = value; }
        }


        private string _CodeGenNameSpace = "";
        /// <summary>
        ///  代码生成命名空间前缀
        /// </summary>
        /// 
        [XmlAttribute]
        public string CodeGenNameSpace
        {
            get { return _CodeGenNameSpace; }
            set { _CodeGenNameSpace = value; }
        }
        private string _CodeGenTableFilter = "A_;B_;C_;D_;T_;TB_";
        /// <summary>
        /// 代码生成过滤表名前缀
        /// </summary>
        /// 
        [XmlAttribute]
        public string CodeGenTableFilter
        {
            get { return _CodeGenTableFilter; }
            set { _CodeGenTableFilter = value; }
        }

        private string _CodeGenBaseDirectory = PathHelper.GetBaseDirectory();
        /// <summary>
        /// 代码生成根目录
        /// </summary>
        /// 
        [XmlAttribute]
        public string CodeGenBaseDirectory
        {
            get { return _CodeGenBaseDirectory; }
            set { _CodeGenBaseDirectory = value; }
        }

        //private string _CodeGenClassPrefix = "";
        ///// <summary>
        /////  代码生成类名前缀
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public string CodeGenClassPrefix
        //{
        //    get { return _CodeGenClassPrefix; }
        //    set { _CodeGenClassPrefix = value; }
        //}

        //private string _CodeGenClassSuffix = "";
        ///// <summary>
        ///// 代码生成类名后缀
        ///// </summary>
        ///// 
        //[XmlAttribute]
        //public string CodeGenClassSuffix
        //{
        //    get { return _CodeGenClassSuffix; }
        //    set { _CodeGenClassSuffix = value; }
        //}


        #endregion


    }


    public class DataSource : IDataSource
    {
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String ConnectionString { get; set; }
        [XmlAttribute]
        public String Provider { get; set; }

        [XmlAttribute]
        public String ParameterPrefix { get; set; }
        [XmlAttribute]
        public DataSourceType Type { get; set; }
        [XmlAttribute]
        public int Weight { get; set; }

        [XmlAttribute]
        public bool IsMaster { get; set; }
    }


    public class InterceptorSource
    {
        [XmlAttribute]
        public String ClassFullName { get; set; }

        [XmlAttribute]
        public String AssemblyName { get; set; }

        private InterceptorType _Type = InterceptorType.ConnectionInterceptor;
        [XmlAttribute]
        public InterceptorType Type { get { return _Type; } set { _Type = value; } }
        public enum InterceptorType
        {
            ConnectionInterceptor = 1,
            ExecutingInterceptor = 2,
            ExceptionInterceptor = 3,
            DataInterceptor = 4,
            TransactionInterceptor = 5
        }
    }

    public class SqlMapSource
    {
        [XmlAttribute]
        public String Path { get; set; }

        private ResourceType _Type = ResourceType.File;
        [XmlAttribute]
        public ResourceType Type { get { return _Type; } set { _Type = value; } }

    }
    /// <summary>
    /// 资源来源类型
    /// </summary>
    public enum ResourceType
    {
        File = 1,
        Directory = 2,
    }

    /// <summary>
    /// 代码生成类型
    /// </summary>
    public enum CodeGenType
    {
        /// <summary>
        /// 代码优先(根据实体和Mapper生成)
        /// </summary>
        CodeFirst = 1,
        /// <summary>
        /// 数据库优先(根据数据库表生成)
        /// </summary>
        DbFirst = 2
    }

    /// <summary>
    /// 代码生成类名模式
    /// </summary>
    public enum CodeGenClassNameMode
    {
        /// <summary>
        /// 全大写Camel PROJECTWORKER
        /// </summary>
        UpperAll = 1,
        /// <summary>
        /// 第一个词的首字母，以及后面每个词的首字母都大写 . ProjectWorker
        /// </summary>
        PascalCase = 2,
        /// <summary>
        /// 第一个词的首字母小写，后面每个词的首字母大写 . projectWorker
        /// </summary>
        CamelCase = 3,
        /// <summary>
        /// 
        /// </summary>
        TitleCase = 4,
        /// <summary>
        /// 
        /// </summary>
        HumanCase = 5,
        /// <summary>
        /// 去除前缀后全大写 PROJECT_WORKER
        /// </summary>
        UpperOrigin = 6,
        /// <summary>
        /// 去除前缀后全小写 PROJECT_WORKER
        /// </summary>
        LowerOrigin = 7,
        /// <summary>
        /// 保留原有 Project_WORKER
        /// </summary>
        Origin = 8
    }

    public class MapperSource
    {
        [XmlAttribute]
        public String Path { get; set; }


        [XmlAttribute]
        public String ClassName { get; set; }

        private MapperAssemblyType _Type = MapperAssemblyType.File;
        [XmlAttribute]
        public MapperAssemblyType Type { get { return _Type; } set { _Type = value; } }
        public enum MapperAssemblyType
        {
            File = 1,
            ClassType = 2,
        }

    }


    /// <summary>
    /// 输出类型
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// 输出表/视图
        /// </summary>
        Table = 1,
        ///// <summary>
        ///// 输出函数/存储过程
        ///// </summary>
        //DataFunction = 1,
        ///// <summary>
        ///// 输出Schema
        ///// </summary>
        //DataSchema = 3,
        ///// <summary>
        ///// 输出SQL: DDL
        ///// </summary>
        //SQL = 5,
        /// <summary>
        /// 输出上下文
        /// </summary>
        OutputContext = 2,
        ///// <summary>
        ///// 输出自定义对象
        ///// </summary>
        //Other = 9

    }
    /// <summary>
    /// 代码生成模板
    /// </summary>
    public class CodeGenTemplate
    {

        private string _name;
        //private string _nameSpace;
        private string _filePrefix;
        private string _fileSuffix;
        private string _fileNameFormat;

        private string _templateFileName;
        //private string _template;
        private string _outputDirectory;
        private string _outputFileExtension;
        private bool _enabled;
        private bool _append;
        private string _encoding;
        private OutputType _outputType;
        [XmlAttribute]
        public OutputType OutputType
        {
            get { return _outputType; }
            set
            {
                _outputType = value;

            }
        }
        [XmlAttribute]
        public string OutputFileExtension
        {
            get { return _outputFileExtension; }
            set
            {
                _outputFileExtension = value;

            }
        }
        //   [XmlAttribute]
        //public string NameSpace
        //{
        //    get { return _nameSpace; }
        //    set
        //    {
        //        if (_nameSpace != value)
        //        {
        //            _nameSpace = value;
        //     //       RaisePropertyChanged("NameSpace");
        //        }
        //    }
        //}
        [XmlAttribute]
        public string FilePrefix
        {
            get { return _filePrefix; }
            set
            {
                _filePrefix = value;

            }
        }
        [XmlAttribute]
        public string FileSuffix
        {
            get { return _fileSuffix; }
            set
            {
                _fileSuffix = value;

            }
        }
     
        [XmlAttribute]
        public string FileNameFormat
        {
            get { return _fileNameFormat; }
            set
            {
                _fileNameFormat = value;

            }
        }
        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

            }
        }
        [XmlAttribute]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;

            }
        }

        [XmlAttribute]
        public string TemplateFileName
        {
            get
            {
                return _templateFileName;
            }
            set
            {
                _templateFileName = value;

            }
        }
        //[XmlAttribute]
        //public string Template
        //{
        //    get
        //    {
        //        return _template;
        //    }
        //    set
        //    {
        //        _template = value;

        //    }
        //}

        [XmlAttribute]
        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set
            {
                _outputDirectory = value;

            }
        }


        [XmlAttribute]
        public bool Append
        {
            get { return _append; }
            set
            {
                _append = value;

            }
        }
        [XmlAttribute]
        public string Encoding
        {
            get { return _encoding; }
            set
            {
                _encoding = value;

            }
        }

    }
}
