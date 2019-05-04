using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pure.Data
{
    public delegate void OutputActionDelegate(string msg, Exception ex, MessageType type);
    /// <summary>
    /// 数据库配置
    /// </summary>
    public interface IDatabaseConfig
    {
        /// <summary>
        /// 是否由PureDataConfiguration初始化
        /// </summary>
        bool InitByPureDataConfiguration { get; set; }
        /// <summary>
        /// 是否当验证出现错误即可停止
        /// </summary>
        bool ValidateStopOnFirstFailure { get; set; } 

        /// <summary>
        /// sql参数前缀
        /// </summary>
        string ParameterPrefix { get; set; }
        /// <summary>
        /// sql参数后缀
        /// </summary>
        string ParameterSuffix { get; set; }
        /// <summary>
        /// 全局表前缀
        /// </summary>
        string GlobalTablePrefix { get; set; }

        /// <summary> 
        /// 是否启用监听器
        /// </summary>
        bool EnableIntercept { get; set; } 
        /// <summary>
        /// 中断器集合
        /// </summary>
        List<IInterceptor> Interceptors { get; }
        /// <summary>
        /// 执行超时单位秒，默认30秒
        /// </summary>
        int ExecuteTimeout { get; set; }
        /// <summary>
        /// 是否保持数据库连接状态直到对象释放
        /// </summary>
        bool KeepConnectionAlive { get; set; }
        /// <summary>
        /// 是否自动释放Connection对象，false则只保留一个Connection对象
        /// </summary>
        bool AutoDisposeConnection { get; set; }
        /// <summary>
        /// 是否启动调试模式
        /// </summary>
        bool EnableDebug { get; set; }
        /// <summary>
        /// 是否记录错误
        /// </summary>
        bool EnableLogError { get; set; }
        /// <summary>
        /// 是否启用ORM缓存
        /// </summary>
        bool EnableOrmCache { get; set; }
        /// <summary>
        /// 列表对象缓存过期时间，默认60秒
        /// </summary>
        int CacheOrmTime { get; set; }

        /// <summary>
        /// 单对象缓存过期时间，默认60秒
        /// </summary>
        int CacheOrmSingleTime { get; set; }

        /// <summary>缓存维护定时器的检查周期，默认5秒</summary>
        int OrmCacheCheckPeriod { get; set; }

        /// <summary>
        /// 是否启用自定义日志输出
        /// </summary>
        bool EnableOrmLog { get; set; }
        /// <summary>
        /// 是否启用内部日志输出
        /// </summary>
        bool EnableInternalLog { get; set; }

        /// <summary>
        /// 输出日志方法代理
        /// </summary>
        OutputActionDelegate OutputAction { get; set; }

        /// <summary>
        /// 日志输出路径
        /// </summary>
        string OrmLogsPath { get; set; }

        /// <summary>
        /// 最大日志行数
        /// </summary>
        int MaxServerLogSize { get; set; }
        /// <summary>
        /// 默认分页大小，默认10
        /// </summary>
        int DefaultPageSize { get; set; }
        /// <summary>
        /// 日志分类类型:Info/Debug/Error
        /// </summary>
        bool CategoryLogType { get; set; }
        /// <summary>
        /// 组装实体映射dll的Assembly,默认读取当前程序下所有Assembly
        /// </summary>
        List<Assembly> MappingAssemblies { get; set; }

        /// <summary>
        /// 加载Mapper 的模式: 1：fluent mapper ， 2：Attribute  ， 3：sql map
        /// </summary>
        LoadMapperMode LoadMapperMode { get; set; }

        /// <summary>
        /// 读写分离数据源集合
        /// </summary>
        List<DataSource> DataSources { get; set; }


        /// <summary>
        /// Sql Map  Scope前缀命名空间
        /// </summary>
        string NameSpacePrefix { get; set; }
        /// <summary>
        /// 是否监听SqlMap配置文件
        /// </summary>
        bool IsWatchSqlMapFile { get; set; }
        /// <summary>
        /// 间隔监听SqlMap配置文件时间（毫秒）
        /// </summary>
        int WatchSqlMapInterval { get; set; }
        /// <summary>
        /// sql map配置路径
        /// </summary>
        List<string> SqlMapFilePaths { get; set; }

        /// <summary>
        /// sql map所在目录路径
        /// </summary>
        List<string> SqlMapDirPaths { get; set; }

        /// <summary>
        /// 输出Sql map加载日志代理
        /// </summary>
        Action<string> OutputSqlMapLoaderLogs { get; set; }
        /// <summary>
        /// 是否启用Sql Map功能
        /// </summary>
        bool EnableSqlMap { get; set; }

        /// <summary>
        /// 是否启用SQL格式化
        /// </summary>
        bool FormatSql { get; set; }
        /// <summary>
        /// 是否用原始格式化SQL输出到日志
        /// </summary>
        bool LogWithRawSql { get; set; }
        /// <summary>
        /// 当表存在时候能否更新
        /// </summary>
        bool CanUpdatedWhenTableExisted { get; set; }


        //IList<SqlMapInfo> SqlMapSets { get; set; }
        /// <summary>
        /// 是否自动迁移数据库脚本
        /// </summary>
        bool AutoMigrate { get; set; }
        /// <summary>
        /// 自动清空没用的属性列，用于CodeFirst模式
        /// </summary>
        bool AutoRemoveUnuseColumnInTable { get; set; }
        /// <summary>
        /// 是否启用自动迁移日志
        /// </summary>
        bool EnableAutoMigrateLog { get; set; }

        /// <summary>
        /// 是否启用自动迁移调试
        /// </summary>
        bool EnableAutoMigrateDebug { get; set; }

        /// <summary>
        /// 指定某些表能自动迁移（以;分割）
        /// </summary>
        string AutoMigrateOnContainTable { get; set; }
        /// <summary>
        /// 不迁移某些表（以;分割）
        /// </summary>
        string AutoMigrateWithoutTable { get; set; }

        /// <summary>
        /// 数据库连接初始化事件
        /// </summary>
        Func<System.Data.IDbConnection, System.Data.IDbConnection> DbConnectionInit { get; set; }

        /// <summary>
        /// 是否启用全局忽略列的更新
        /// </summary>
        bool EnableGlobalIgnoreUpdatedColumns { get; set; }

        /// <summary>
        /// 是否自动过滤空值列的更新
        /// </summary>
        bool AutoFilterEmptyValueColumnsWhenTrack { get; set; }


        /// <summary>
        /// Track模式下,全局过滤指定更新列名（以;分割）
        /// </summary>
        string GlobalIgnoreUpdatedColumns { get; set; }

        ///// <summary>
        ///// 是否启用数据库连接池，默认true
        ///// </summary>
        //bool EnableConnectionPool { get; set; }
        ///// <summary>
        ///// 是否开启连接池日志,需要先启用EnableDebug
        ///// </summary>
        //bool EnableLogConnectionPool { get; set; }

        ///// <summary>
        ///// 池里不会被释放的最多空闲连接数量。,默认0
        ///// 连接池中最小的空闲的连接数，低于这个数量会被创建新的连接（默认为0，调整为5，该参数越接近maxIdle，性能越好，因为连接的创建和销毁，都是需要消耗资源的；但是不能太大，因为在机器很空闲的时候，也会创建低于minidle个数的连接，类似于jvm参数中的Xmn设置）
        ///// </summary>
        //int MinIdle { get; set; }
        ///// <summary>
        ///// 同一时间可以从池分配的最多连接数量，默认8,设-1为没有限制（对象池中对象最大个数）
        ///// 连接池中最大的空闲的连接数，超过的空闲连接将被释放，如果设置为负数表示不限制（默认为8个，maxIdle不能设置太小，因为假如在高负载的情况下，连接的打开时间比关闭的时间快，会引起连接池中idle的个数 上升超过maxIdle，而造成频繁的连接销毁和创建，类似于jvm参数中的Xmx设置)
        ///// </summary>
        //int MaxIdle { get; set; }
        ///// <summary>
        ///// 数据库连接增加容量步长，默认0
        ///// </summary>
        //int InitialSize { get; set; }
        ///// <summary>
        /////  配置获取连接等待超时的时间,以毫秒为单位，在抛出异常之前，池等待连接被回收的最长时间（当没有可用连接时）。设置为-1表示无限等待。
        /////  最大等待时间，当没有可用连接时，连接池等待连接释放的最大时间，超过该时间限制会抛出异常，如果设置-1表示无限等待（默认为无限，建议调整为60000ms，避免因线程池不够用，而导致请求被无限制挂起）
        ///// </summary>
        //long MaxWaitMillis { get; set; }
        ///// <summary>
        /////  连接池中可同时连接的最大的连接数，默认-1全部激活不限制
        ///// </summary>
        //int MaxTotal { get; set; }
        ///// <summary>
        ///// 失效检查线程运行时间间隔，如果小于等于-1，不会启动检查线程，默认-1
        ///// </summary>
        //long TimeBetweenEvictionRunsMillis { get; set; }
        ///// <summary>
        /////  配置一个连接在池中最小生存的时间，单位是毫秒，默认30分钟
        /////  连接池中连接，在时间段内一直空闲， 被逐出连接池的时间
        ///// </summary>
        //long MinEvictableIdleTimeMillis { get; set; }

        ///// <summary>
        /////  代表每次检查链接的数量，默认3，建议设置和maxActive一样大，这样每次可以有效检查所有的链接.
        ///// </summary>
        //int NumTestsPerEvictionRun { get; set; }
        ///// <summary>
        ///// 软删除失效驱逐的时间，默认-1，单位毫秒
        ///// </summary>
        //long SoftMinEvictableIdleTimeMillis { get; set; }
        ///// <summary>
        /////   取得对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //bool TestOnBorrow { get; set; }
        ///// <summary>
        /////   创建对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //bool TestOnCreate { get; set; }
        ///// <summary>
        ///// 返回对象时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //bool TestOnReturn { get; set; }
        ///// <summary>
        /////  空闲时是否进行验证，检查对象是否有效，默认为false
        ///// </summary>
        //bool TestWhileIdle { get; set; }
        ///// <summary>
        ///// SQL查询,用来验证从连接池取出的连接,在将连接返回给调用者之前.如果指定,则查询必须是一个SQL SELECT并且必须返回至少一行记录
        ///// </summary>
        //string ValidationQuery { get; set; }
        

        ///// <summary>
        ///// 是否启用失效清除对象
        ///// </summary>
        //bool EnableRemoveAbandoned { get; set; }
        ///// <summary>
        ///// 是否在获取对象连接时,自动检测是否含有失效的连接，如果有则执行清除
        ///// </summary>
        //bool RemoveAbandonedOnBorrow { get; set; }
        ///// <summary>
        ///// 是否运行期间，自动检测是否含有失效的连接，如果有则执行清除
        ///// </summary>
        //bool RemoveAbandonedOnMaintenance { get; set; }
        ///// <summary>
        /////  自动回收超时时间(以秒数为单位)，默认300 
        /////  超过时间限制，回收没有用(废弃)的连接（建议调整为180）
        ///// </summary>
        //int RemoveAbandonedTimeout { get; set; }



        /// <summary>
        /// 是否启用Lob类型转换器（Clob和Blob）
        /// </summary>
        bool EnableLobConverter { get; set; }
        /// <summary>
        ///  Lob类型转换器接口实现类
        /// </summary>
        string LobConverterClassName { get; set; }


        #region 代码生成设置
        /// <summary>
        /// 是否启用代码生成
        /// </summary>
        bool EnableCodeGen { get; set; }
        /// <summary>
        /// 代码生成类型
        /// </summary>
        CodeGenType CodeGenType { get; set; }
        /// <summary>
        /// 代码生成显示类名
        /// </summary>
        CodeGenClassNameMode CodeGenClassNameMode { get; set; }
        /// <summary>
        /// 属性名称类型
        /// </summary>
        CodeGenClassNameMode CodeGenPropertyNameMode { get; set; }

        
        /// <summary>
        /// 项目名称（英文缩写）
        /// </summary>
        string CodeGenProjectName { get; set; }
        /// <summary>
        /// 代码生成根目录
        /// </summary>
        string CodeGenBaseDirectory { get; set; }
        /// <summary>
        /// 命名空间前缀
        /// </summary>
        string CodeGenNameSpace { get; set; }
        /// <summary>
        /// 代码生成过滤表名前缀
        /// </summary>
        string CodeGenTableFilter { get; set; }
        /// <summary>
        /// 代码生成模板
        /// </summary>
        List<CodeGenTemplate> CodeGenTemplates { get; set; }
        #endregion

        /// <summary>
        /// 批量处理器
        /// </summary>
        string BulkOperateClassName { get; set; }
    }
}
