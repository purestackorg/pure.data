
using System;
using System.Xml.Serialization;
using System.IO;
using Pure.Data.SqlMap;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Pure.Data.Validations;

namespace Pure.Data
{
    /// <summary>
    /// 缓存的配置加载信息
    /// </summary>
    public class LoadedDatabaseConfig
    {
        public string Path { get; set; }

        public PureDataConfiguration PureDataConfiguration { get; set; }
        public DbConnectionMetaInfo DbConnectionMetaInfo { get; set; }
        public IDatabaseConfig DatabaseConfig { get; set; }

    }
    /// <summary>
    /// 本地数据库配置加载器
    /// </summary>
    public class PureDataConfigurationLoader : Singleton<PureDataConfigurationLoader>
    {
         
        public static List<Assembly> MapperSourceAssemblies = new List<Assembly>();
        public static List<string> SqlMapDirPaths = new List<string>();
        public static List<string> SqlMapFilePaths = new List<string>();

        public OutputActionDelegate LogAction { get; private set; }
        public void Log(string str, Exception ex = null, MessageType typ = MessageType.Debug)
        {
            if (LogAction != null)
            {
                LogAction(str, ex, typ);
            }
        }

        /// <summary>
        /// 全局数据库配置
        /// </summary>
        public PureDataConfiguration PureDataConfiguration { get; private set; }
        private static readonly ConcurrentDictionary<string, LoadedDatabaseConfig> _DbConfigCached = new ConcurrentDictionary<string, LoadedDatabaseConfig>();

        private static object olock = new object();
        public LoadedDatabaseConfig Load(string path, IDatabaseConfig databaseConfig, OutputActionDelegate _LogAction = null)
        {
            lock (olock)
            {
                LoadedDatabaseConfig __LoadedDatabaseConfig = null;
                if (_DbConfigCached.TryGetValue(path, out __LoadedDatabaseConfig))
                {
                    return __LoadedDatabaseConfig;
                }
                else
                {

                    //string fullConfigPath = "";

                    //lock (olock)
                    //{
                    //    if (_LogAction == null)
                    //    {
                    //        _LogAction = (str, ex, type) =>
                    //            {
                    //                ConsoleHelper.Instance.OutputMessage(str, ex, type);
                    //            };
                    //    }
                    //    LogAction = _LogAction;

                    //    if (PureDataConfiguration == null)
                    //    {

                    //        fullConfigPath = FileLoader.GetPath(path);
                    //        Log(string.Format("LocalDatabaseConfigLoader Load: {0} Starting", fullConfigPath));
                    //        var configStream = LoadConfigStream(path);

                    //        try
                    //        {

                    //            using (configStream.Stream)
                    //            {

                    //                fullConfigPath = configStream.Path;
                    //                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PureDataConfiguration));

                    //                PureDataConfiguration config = xmlSerializer.Deserialize(configStream.Stream) as PureDataConfiguration;
                    //                config.Path = configStream.Path;

                    //                PureDataConfiguration = config;
                    //            }


                    //        }
                    //        catch (Exception ex)
                    //        {

                    //            Log(string.Format("LocalDatabaseConfigLoader Load fail: {0} ", ex.Message + "------" + ex));

                    //        }


                    //    }

                    //}

                    //__LoadedDatabaseConfig = new LoadedDatabaseConfig();
                    //__LoadedDatabaseConfig.PureDataConfiguration = PureDataConfiguration;
                    //__LoadedDatabaseConfig.Path = path;

                    ////设定配置
                    //var metaInfo = MapToDatabseConfig(PureDataConfiguration, databaseConfig);
                    //databaseConfig.OutputAction = _LogAction;
                    //databaseConfig.InitByPureDataConfiguration = true;

                    //__LoadedDatabaseConfig.DatabaseConfig = databaseConfig;
                    //__LoadedDatabaseConfig.DbConnectionMetaInfo = metaInfo;

                    //_DbConfigCached[path] = __LoadedDatabaseConfig;


                    //Log(string.Format("Init DatabaseConfig Successfully : {0} ", fullConfigPath));


                    //return __LoadedDatabaseConfig;



                    //#region Watch
                    __LoadedDatabaseConfig = LoadDatabaseConfig(path, databaseConfig, _LogAction);

                    ////watch pure config
                    //var fullpath = FileLoader.GetPath(path);
                    //string dir = System.IO.Path.GetDirectoryName(fullpath);
                    //string filename = System.IO.Path.GetFileName(fullpath);
                    //Log("Watch PureDataConfiguration changed on :" + fullpath);
                    //FileWatcherLoader.Instance.Watch(dir, filename, () =>
                    //{
                    //    __LoadedDatabaseConfig = LoadDatabaseConfig(path, databaseConfig, _LogAction);
                    //    Log(__LoadedDatabaseConfig.DatabaseConfig.ToString());
                    //    Log(string.Format(" PureDataConfiguration has reloaded at : {0} ", path));


                    //}, 500);

                    return __LoadedDatabaseConfig;
                    //#endregion


                }
            }
           


        }


        private LoadedDatabaseConfig LoadDatabaseConfig(string path, IDatabaseConfig databaseConfig, OutputActionDelegate _LogAction = null)
        {
            string fullConfigPath = "";
            LoadedDatabaseConfig __LoadedDatabaseConfig = null;
            
                if (_LogAction == null)
                {
                    _LogAction = (str, ex, type) =>
                    {
                        ConsoleHelper.Instance.OutputMessage(str, ex, type);
                    };
                }
                LogAction = _LogAction;

                //if (PureDataConfiguration == null)
                //{

                fullConfigPath = FileLoader.GetPath(path);
                Log(string.Format("LocalDatabaseConfigLoader Load: {0} Starting", fullConfigPath));

                try
                {
                    var configStream = LoadConfigStream(path);

                    using (configStream.Stream)
                    {

                        fullConfigPath = configStream.Path;
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(PureDataConfiguration));

                        PureDataConfiguration config = xmlSerializer.Deserialize(configStream.Stream) as PureDataConfiguration;
                        config.Path = configStream.Path;

                        PureDataConfiguration = config;
                    }


                }
                catch (Exception ex)
                {
                    Log(string.Format("LocalDatabaseConfigLoader Load fail: {0} ", ex.Message + "------" + ex));
                    return __LoadedDatabaseConfig;
                }


                // }



                __LoadedDatabaseConfig = new LoadedDatabaseConfig();
                __LoadedDatabaseConfig.PureDataConfiguration = PureDataConfiguration;
                __LoadedDatabaseConfig.Path = path;

                //设定配置
                var metaInfo = MapToDatabseConfig(PureDataConfiguration, databaseConfig);
                databaseConfig.OutputAction = _LogAction;
                databaseConfig.InitByPureDataConfiguration = true;

                __LoadedDatabaseConfig.DatabaseConfig = databaseConfig;
                __LoadedDatabaseConfig.DbConnectionMetaInfo = metaInfo;

                _DbConfigCached[path] = __LoadedDatabaseConfig;

                Log(string.Format("Load DatabaseConfig Successfully : {0} ", fullConfigPath));


                return __LoadedDatabaseConfig;


             
          

        }



        private DbConnectionMetaInfo MapToDatabseConfig(PureDataConfiguration config, IDatabaseConfig databaseConfig)
        {

            DbConnectionMetaInfo metaInfo = null;

            if (config != null && databaseConfig != null)
            {
                if (config.Settings != null)
                {
                    var settings = config.Settings;
                    databaseConfig.ParameterPrefix = settings.ParameterPrefix;
                    databaseConfig.ParameterSuffix = settings.ParameterSuffix;
                    databaseConfig.GlobalTablePrefix = settings.GlobalTablePrefix;
                    databaseConfig.ExecuteTimeout = settings.ExecuteTimeout;
                    databaseConfig.DefaultPageSize = settings.DefaultPageSize;
                    databaseConfig.AutoDisposeConnection = settings.AutoDisposeConnection;
                    databaseConfig.ValidateStopOnFirstFailure = settings.ValidateStopOnFirstFailure;
                    databaseConfig.LoadMapperMode = settings.LoadMapperMode;


                    databaseConfig.EnableDebug = settings.EnableDebug;
                    databaseConfig.EnableIntercept = settings.EnableIntercept;
                    databaseConfig.EnableLogError = settings.EnableLogError;
                    databaseConfig.EnableOrmLog = settings.EnableOrmLog;
                    databaseConfig.EnableInternalLog = settings.EnableInternalLog;
                    databaseConfig.LogWithRawSql = settings.LogWithRawSql;
                    databaseConfig.CategoryLogType = settings.CategoryLogType; 
                    if (!string.IsNullOrEmpty(settings.OrmLogsPath))
                    {
                        databaseConfig.OrmLogsPath = settings.OrmLogsPath;
                    }
                    databaseConfig.MaxServerLogSize = settings.MaxServerLogSize;

                    databaseConfig.EnableOrmCache = settings.EnableOrmCache;
                    databaseConfig.CacheOrmTime = settings.CacheOrmTime;
                    databaseConfig.CacheOrmSingleTime = settings.CacheOrmSingleTime;
                    databaseConfig.OrmCacheCheckPeriod = settings.OrmCacheCheckPeriod;

                    databaseConfig.EnableSqlMap = settings.EnableSqlMap;
                    databaseConfig.NameSpacePrefix = settings.NameSpacePrefix;
                    databaseConfig.FormatSql = settings.FormatSql;
                    databaseConfig.IsWatchSqlMapFile = settings.IsWatchSqlMapFile;
                    databaseConfig.WatchSqlMapInterval = settings.WatchSqlMapInterval;

                    databaseConfig.AutoMigrate = settings.AutoMigrate;
                    databaseConfig.AutoRemoveUnuseColumnInTable = settings.AutoRemoveUnuseColumnInTable;
                    databaseConfig.CanUpdatedWhenTableExisted = settings.CanUpdatedWhenTableExisted;
                    databaseConfig.EnableAutoMigrateLog = settings.EnableAutoMigrateLog;
                    databaseConfig.EnableAutoMigrateDebug = settings.EnableAutoMigrateDebug;
                    databaseConfig.AutoMigrateOnContainTable = settings.AutoMigrateOnContainTable;
                    databaseConfig.AutoMigrateWithoutTable = settings.AutoMigrateWithoutTable;
                    if (!string.IsNullOrEmpty(settings.GlobalIgnoreUpdatedColumns))
                    {
                        databaseConfig.GlobalIgnoreUpdatedColumns = settings.GlobalIgnoreUpdatedColumns;

                    }
                    databaseConfig.EnableGlobalIgnoreUpdatedColumns = settings.EnableGlobalIgnoreUpdatedColumns;
                    databaseConfig.AutoFilterEmptyValueColumnsWhenTrack = settings.AutoFilterEmptyValueColumnsWhenTrack;

                    //代码生成设置
                    databaseConfig.EnableCodeGen = settings.EnableCodeGen;
                    databaseConfig.CodeGenType = settings.CodeGenType;
                    databaseConfig.CodeGenClassNameMode = settings.CodeGenClassNameMode;
                    databaseConfig.CodeGenPropertyNameMode = settings.CodeGenPropertyNameMode;
                    
                    databaseConfig.CodeGenProjectName = settings.CodeGenProjectName;
                    databaseConfig.CodeGenNameSpace = settings.CodeGenNameSpace;
                    databaseConfig.CodeGenTableFilter = settings.CodeGenTableFilter;
                    string _basePath = "";

                    //if (databaseConfig.EnableCodeGen)
                    //{
                        string[] basePaths = settings.CodeGenBaseDirectory.Split(';');
                        if (basePaths.Length >0)
                        {
                            foreach (var basePath in basePaths)
                            {
                                if (System.IO.Directory.Exists(basePath))
                                {
                                    _basePath = basePath;
                                    break;
                                }
                            }
                        }

                        if (_basePath == "")
                        {
                            string baseRootPath =System.IO.Path.Combine( PathHelper.GetBaseDirectory(), "generate");
                        //if (Directory.Exists(baseRootPath))
                        //{
                        //    DirectoryInfo info = new DirectoryInfo(baseRootPath);
                        //    _basePath = info.Parent.FullName;
                        //}
                        _basePath = baseRootPath;

                    }
                    //}

                    databaseConfig.CodeGenBaseDirectory = _basePath;



                    //databaseConfig.EnableConnectionPool = settings.EnableConnectionPool;
                    //databaseConfig.EnableLogConnectionPool = settings.EnableLogConnectionPool;
                    //databaseConfig.MinIdle = settings.MinIdle;
                    //databaseConfig.MaxIdle = settings.MaxIdle;
                    //databaseConfig.InitialSize = settings.InitialSize;

                    //databaseConfig.MaxWaitMillis = settings.MaxWaitMillis;
                    //databaseConfig.MaxTotal = settings.MaxTotal;
                    //databaseConfig.TimeBetweenEvictionRunsMillis = settings.TimeBetweenEvictionRunsMillis;
                    //databaseConfig.MinEvictableIdleTimeMillis = settings.MinEvictableIdleTimeMillis;
                    //databaseConfig.EnableRemoveAbandoned = settings.EnableRemoveAbandoned;
                    //databaseConfig.RemoveAbandonedOnBorrow = settings.RemoveAbandonedOnBorrow;
                    //databaseConfig.RemoveAbandonedOnMaintenance = settings.RemoveAbandonedOnMaintenance;
                    //databaseConfig.RemoveAbandonedTimeout = settings.RemoveAbandonedTimeout;
                    //databaseConfig.NumTestsPerEvictionRun = settings.NumTestsPerEvictionRun;
                    //databaseConfig.SoftMinEvictableIdleTimeMillis = settings.SoftMinEvictableIdleTimeMillis;
                    //databaseConfig.TestOnBorrow = settings.TestOnBorrow;
                    //databaseConfig.TestOnCreate = settings.TestOnCreate;
                    //databaseConfig.TestOnReturn = settings.TestOnReturn;
                    //databaseConfig.TestWhileIdle = settings.TestWhileIdle;
                    //databaseConfig.ValidationQuery = settings.ValidationQuery;
                    
                     

                    databaseConfig.EnableLobConverter = settings.EnableLobConverter;
                    databaseConfig.LobConverterClassName = settings.LobConverterClassName;
                    if (databaseConfig.EnableLobConverter == true)
                    {

                        try
                        {
                            Type type = Type.GetType(databaseConfig.LobConverterClassName);

                            if (type != null)
                            {
                                object obj = Activator.CreateInstance(type, true);

                                LobConverter.Init((ILobParameterConverter)(obj), databaseConfig.EnableLobConverter);
                            }
                            else
                            {
                                Log("没有找到相关ILobParameterConverter实现类,LobConverterClassName: " + databaseConfig.LobConverterClassName, new ArgumentException("没有找到相关ILobParameterConverter实现类,LobConverterClassName: " + databaseConfig.LobConverterClassName), MessageType.Error);

                            }
                        }
                        catch (Exception ex)
                        {
                            Log("没有找到相关ILobParameterConverter实现类,LobConverterClassName: " + databaseConfig.LobConverterClassName, new ArgumentException("没有找到相关ILobParameterConverter实现类,LobConverterClassName: " + databaseConfig.LobConverterClassName), MessageType.Error);

                            throw new ArgumentException("没有找到相关ILobParameterConverter实现类,LobConverterClassName: " + databaseConfig.LobConverterClassName, ex);

                        }

                    }
                    databaseConfig.BulkOperateClassName = settings.BulkOperateClassName;
                    databaseConfig.EnableDefaultPropertySecurityValidate = settings.EnableDefaultPropertySecurityValidate;

                    databaseConfig.PropertySecurityValidateClassName = settings.PropertySecurityValidateClassName;

                    if (!string.IsNullOrEmpty(settings.BulkOperateClassName))
                    {

                        try
                        {
                            Type type = Type.GetType(settings.BulkOperateClassName);

                            if (type != null)
                            {
                                IBulkOperate obj = (IBulkOperate)Activator.CreateInstance(type, true);
                                BulkOperateManage.Instance.Register(settings.BulkOperateClassName, obj);
                                 
                            }
                            else
                            {
                                Log("没有找到相关IBulkOperate实现类,BulkOperateClassName: " + settings.BulkOperateClassName, new ArgumentException("没有找到相关IBulkOperate实现类,BulkOperateClassName: " + settings.BulkOperateClassName), MessageType.Error);

                            }
                        }
                        catch (Exception ex)
                        {
                            Log("没有找到相关IBulkOperate实现类,BulkOperateClassName: " + settings.BulkOperateClassName, new ArgumentException("没有找到相关IBulkOperate实现类,BulkOperateClassName: " + settings.BulkOperateClassName), MessageType.Error);

                            throw new ArgumentException("没有找到相关IBulkOperate实现类,BulkOperateClassName: " + settings.BulkOperateClassName, ex);

                        }

                    }


                    if (!string.IsNullOrEmpty(settings.PropertySecurityValidateClassName))
                    {

                        try
                        {
                            Type type = Type.GetType(settings.PropertySecurityValidateClassName);

                            if (type != null)
                            {
                                IPropertySecurityValidate obj = (IPropertySecurityValidate)Activator.CreateInstance(type, true);
                                PropertySecurityValidateManage.Instance.Register(settings.PropertySecurityValidateClassName, obj);

                            }
                            else
                            {
                                Log("IPropertySecurityValidate 无法找到实现类, 将启用默认的安全校验器！" );

                            }
                        }
                        catch (Exception ex)
                        {
                            Log("IPropertySecurityValidate 无法找到实现类, 将启用默认的安全校验器！");

                            // throw new ArgumentException("没有找到相关IBulkOperate实现类,PropertySecurityValidateClassName: " + settings.PropertySecurityValidateClassName, ex);

                        }

                    }

                }

                try
                {
                    if (config.DataSources != null)
                    {
                        string ConnectionString = "", ProviderName = "";
                        databaseConfig.DataSources = config.DataSources;

                        foreach (var item in config.DataSources)
                        {

                            if (item.IsMaster == true)//是否主数据库
                            {
                                ConnectionString = item.ConnectionString;
                                ProviderName = item.Provider;
                                break;
                            }

                        }

                        metaInfo = DbConnectionFactory.CreateConnection(ConnectionString, ProviderName);

                    }
                    else
                    {
                        Log("DataSources 不能为空，且必须存在一个Master的数据源！", new ArgumentException("DataSources 不能为空，且必须存在一个Master的数据源！"), MessageType.Error);
                    }
                }
                catch (Exception ex)
                {
                    Log("DataSources 不能为空，且必须存在一个Master的数据源！", ex, MessageType.Error);

                    throw new ArgumentException("DataSources 不能为空，且必须存在一个Master的数据源！", ex);

                }




                if (config.MapperSources != null)
                {
                    Assembly ass = null;
                    string _MapperSourcePath = "";
                    string path = "";
                    MapperSource.MapperAssemblyType _MapperAssemblyType = MapperSource.MapperAssemblyType.ClassType;
                    try
                    {
                        databaseConfig.MappingAssemblies.Clear();

                        foreach (var item in config.MapperSources)
                        {
                            _MapperAssemblyType = item.Type;
                            if (item.Type == MapperSource.MapperAssemblyType.File)
                            {
                                _MapperSourcePath = item.Path;
                                path = FileLoader.GetPath(item.Path);
                                ass = Assembly.LoadFrom(path);
                                databaseConfig.MappingAssemblies.Add(ass);

                            }
                            else if (item.Type == MapperSource.MapperAssemblyType.ClassType)
                            {
                                path = item.ClassName;

                                Type type = Type.GetType(item.ClassName);
                                if (type != null)
                                {
                                    ass = type.Assembly;
                                    if (ass != null)
                                    {
                                        databaseConfig.MappingAssemblies.Add(ass);

                                    }

                                }
                                else
                                {
                                    Log("MapperSource 中存在不正确的Type：" + item.ClassName, new ArgumentException("MapperSource 中存在不正确的Type：" + item.ClassName), MessageType.Error);


                                }

                            }
                        }


                        foreach (var item in PureDataConfigurationLoader.MapperSourceAssemblies)
                        {
                            if (item != null)
                            {
                                databaseConfig.MappingAssemblies.Add(item);

                            }
                        }




                    }
                    catch (Exception ex)
                    {
                        Log("Load MapperSource error on ：" + _MapperAssemblyType + ", " + _MapperSourcePath+" , path:"+ path, ex, MessageType.Error);
                        throw new ArgumentException("Load MapperSource error on ：" + _MapperAssemblyType + ", " + _MapperSourcePath, ex);

                    }
                }





                if (config.SqlMapSources != null)
                {
                    var dirSqlMappers = config.SqlMapSources.Where(p => p.Type == ResourceType.Directory);
                    var fileSqlMappers = config.SqlMapSources.Where(p => p.Type == ResourceType.File);

                    databaseConfig.SqlMapDirPaths = dirSqlMappers.Select(p => p.Path).ToList();
                    databaseConfig.SqlMapFilePaths = fileSqlMappers.Select(p => p.Path).ToList();

                    foreach (var item in PureDataConfigurationLoader.SqlMapDirPaths)
                    {
                        if (item != null)
                        {
                            databaseConfig.SqlMapDirPaths.Add(item);

                        }
                    }
                    foreach (var item in PureDataConfigurationLoader.SqlMapFilePaths)
                    {
                        if (item != null)
                        {
                            databaseConfig.SqlMapFilePaths.Add(item);

                        }
                    }
                }

                try
                {
                    if (databaseConfig.EnableIntercept == true)
                    {
                        databaseConfig.Interceptors.Clear();

                        //if (databaseConfig.EnableDebug && databaseConfig.EnableIntercept)
                        //{
                        //    databaseConfig.Interceptors.Add(OutputSQLIntercept.Instance);
                        //    databaseConfig.Interceptors.Add(OutputExceptionIntercept.Instance);
                        //}

                        if (config.Interceptors != null)
                        {
                            IInterceptor ass = null;
                            string path = "";

                            foreach (var item in config.Interceptors)
                            {

                                ass = null;
                                if (string.IsNullOrEmpty(item.ClassFullName) || string.IsNullOrEmpty(item.AssemblyName))
                                {
                                    continue;
                                }
                                if (item.Type == InterceptorSource.InterceptorType.ConnectionInterceptor)
                                {
                                    ass = ReflectionHelper.CreateInstance<IConnectionInterceptor>(item.ClassFullName, item.AssemblyName);

                                }
                                else if (item.Type == InterceptorSource.InterceptorType.ExecutingInterceptor)
                                {
                                    ass = ReflectionHelper.CreateInstance<IExecutingInterceptor>(item.ClassFullName, item.AssemblyName);

                                }
                                else if (item.Type == InterceptorSource.InterceptorType.ExceptionInterceptor)
                                {
                                    ass = ReflectionHelper.CreateInstance<IExceptionInterceptor>(item.ClassFullName, item.AssemblyName);

                                }
                                else if (item.Type == InterceptorSource.InterceptorType.DataInterceptor)
                                {
                                    ass = ReflectionHelper.CreateInstance<IDataInterceptor>(item.ClassFullName, item.AssemblyName);

                                }
                                else if (item.Type == InterceptorSource.InterceptorType.TransactionInterceptor)
                                {
                                    ass = ReflectionHelper.CreateInstance<ITransactionInterceptor>(item.ClassFullName, item.AssemblyName);

                                }

                                if (ass != null)
                                {
                                    databaseConfig.Interceptors.Add(ass);
                                }

                            }


                        }


                    }
                }
                catch (Exception ex)
                {
                    Log("Load Intercept error  !", ex, MessageType.Error);
                    throw new ArgumentException("Load Intercept error  !", ex);

                }



                if (config.CodeGenTemplates != null)
                {
                    databaseConfig.CodeGenTemplates = config.CodeGenTemplates;
                }


            }

            return metaInfo;
        }


        public ConfigStream LoadConfigStream(string path)
        {
            var configStream = new ConfigStream
            {
                Path = path,
                Stream = FileLoader.Load(path)
            };
            return configStream;
        }


    }

    public class ConfigStream
    {
        public String Path { get; set; }
        public Stream Stream { get; set; }
    }
}
