using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
#if NET45
using System.Data.OleDb;
#endif

using System.Collections.Concurrent;
using System.Collections.Generic;
namespace Pure.Data
{
    public class DbConnectionMetaInfo
    { 
        public string ProviderName { get;  set; }
        public string ConnectionString { get;  set; }
        ///// <summary>
        ///// 数据库连接对象
        ///// </summary>
        //public IDbConnection Connection
        //{
        //    get;
        //     set;
        //}
        public DbProviderFactory DbFactory
        {
            get;
            set;
        }
        public DatabaseType DatabaseType { get; set; }
    }

    internal class DbConnectionFactory
    {

        public static string GetDefaultProviderNameByDbType(DatabaseType DatabaseType)
        {
            string providerName = "";
            switch (DatabaseType)
            {
                case DatabaseType.None:
                    providerName = DbProviderNameManage.None;
                    break;
                case DatabaseType.SqlServer: providerName = DbProviderNameManage.SQLServer;
                    break;
                case DatabaseType.SqlCe: providerName = DbProviderNameManage.SqlServerCe40;
                    break;
                case DatabaseType.PostgreSQL: providerName = DbProviderNameManage.PostgreSQL;
                    break;
                case DatabaseType.MySql: providerName = DbProviderNameManage.MySql;
                    break;
                case DatabaseType.Oracle: providerName = DbProviderNameManage.Oracle;
                    break;
                case DatabaseType.SQLite: providerName = DbProviderNameManage.SQLite;
                    break;
                case DatabaseType.Access: providerName = DbProviderNameManage.Access;
                    break;
                case DatabaseType.OleDb: providerName = DbProviderNameManage.OleDb;
                    break;
                case DatabaseType.Firebird: providerName = DbProviderNameManage.FirebirdSql;
                    break;
                case DatabaseType.DB2: providerName = DbProviderNameManage.DB2;
                    break;
                case DatabaseType.DB2iSeries: providerName = DbProviderNameManage.DB2iSeries;
                    break;
                case DatabaseType.SybaseASA: providerName = DbProviderNameManage.SybaseASA;
                    break;
                case DatabaseType.SybaseASE: providerName = DbProviderNameManage.SybaseASE;
                    break;
                case DatabaseType.SybaseUltraLite: providerName = DbProviderNameManage.SybaseUltraLite;
                    break;
                case DatabaseType.DM: providerName = DbProviderNameManage.DM;
                    break;
                default: providerName = DbProviderNameManage.SQLServer;
                    break;
            }
            return providerName;
        }

        /// <summary>
        /// Look at the type and provider name being used and instantiate a suitable DatabaseType instance.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DatabaseType Resolve(string dbTypeName, string providerName)
        {
            DatabaseType dbType = DatabaseType.None;
            // 使用类型名判断
            if (dbTypeName.Contains("MySql")) dbType = DatabaseType.MySql;
            else if (dbTypeName.StartsWith("SqlCe")) dbType = DatabaseType.SqlCe;
            else if (dbTypeName.StartsWith("Npgsql") || dbTypeName.StartsWith("PgSql")) dbType = DatabaseType.PostgreSQL;
            else if (dbTypeName.StartsWith("Oracle")) dbType = DatabaseType.Oracle;
            else if (dbTypeName.StartsWith("SQLite")) dbType = DatabaseType.SQLite;
            else if (dbTypeName.StartsWith("System.Data.SqlClient.") || dbTypeName.StartsWith("SqlClientFactory")) dbType = DatabaseType.SqlServer;
            // else try with provider name
            else if (providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.MySql;
            else if (providerName.IndexOf("SqlServerCe", StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.SqlCe;
            else if (providerName.IndexOf("Npgsql", StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.PostgreSQL;
            else if (providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.Oracle;
            else if (providerName.IndexOf("SQLite", StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.SQLite;
            else if (providerName.IndexOf(DbProviderNameManage.Access, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.Access;
            else if (providerName.IndexOf(DbProviderNameManage.FirebirdSql, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.Firebird;
            else if (providerName.IndexOf(DbProviderNameManage.OleDb, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.OleDb;
            else if (providerName.IndexOf(DbProviderNameManage.DM, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.DM;
            else if (providerName.IndexOf(DbProviderNameManage.DB2, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.DB2;
            else if (providerName.IndexOf(DbProviderNameManage.DB2iSeries, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.DB2iSeries;
            else if (providerName.IndexOf(DbProviderNameManage.SybaseASA, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.SybaseASA;
            else if (providerName.IndexOf(DbProviderNameManage.SybaseASE, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.SybaseASE;
            else if (providerName.IndexOf(DbProviderNameManage.SybaseUltraLite, StringComparison.InvariantCultureIgnoreCase) >= 0) dbType = DatabaseType.SybaseUltraLite;

            return dbType;
        }

        private static DatabaseType GetDbType(string providerName, DbProviderFactory dbFactory)
        {
            string dbTypeName = dbFactory.GetType().Name;
            return Resolve( dbTypeName,  providerName);
        }

        #region 缓存数据库链接基本对象
        private static readonly ConcurrentDictionary<string, DbConnectionMetaInfo> _DbConnectionMetaInfoCached = new ConcurrentDictionary<string, DbConnectionMetaInfo>();

        private static DbConnectionMetaInfo GetDbConnectionMetaInfoFromCache(string key)
        {
            DbConnectionMetaInfo v;
            _DbConnectionMetaInfoCached.TryGetValue(key, out v);
            return v;
        }
        private static void SetDbConnectionMetaInfoToCache(string key, DbConnectionMetaInfo v)
        {
            _DbConnectionMetaInfoCached[key] = v;
        }
        #endregion

        public static DbConnectionMetaInfo CreateConnectionByConfig(string connectionKey)
        {
            string providerName = ConfigurationManager.ConnectionStrings[connectionKey].ProviderName;
            return  CreateConnectionByConfig(  connectionKey, providerName);
        }
        public static DbConnectionMetaInfo CreateConnectionByConfig(string connectionKey, string providerName)
        {
            string strConn = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;

            return CreateConnection(strConn, providerName);
        }

        public static DbConnectionMetaInfo CreateConnection(string connectionString, string providerName)
        {
            
            if (string.IsNullOrEmpty(providerName))
            {
                throw new Exception("ProviderName could not be empty !");
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString could not be empty !");
            }
            //从缓存读取
            string key = connectionString + "_" + providerName;
            DbConnectionMetaInfo info = GetDbConnectionMetaInfoFromCache(key);
            if (info != null)
            {
                //if (info.DbFactory != null)
                //{
                //    info.Connection = info.DbFactory.CreateConnection();
                //    info.Connection.ConnectionString = connectionString;
                //}
                 
                return info;
            }

            //if (connectionString.IndexOf("microsoft.jet.oledb", StringComparison.OrdinalIgnoreCase) > -1 || connectionString.IndexOf(".db3", StringComparison.OrdinalIgnoreCase) > -1)
            //{
                 
            //    string mdbPath = connectionString.Substring(connectionString.IndexOf("data source", StringComparison.OrdinalIgnoreCase) + "data source".Length + 1).TrimStart(' ', '=');
            //    if (mdbPath.ToLower().StartsWith("|datadirectory|"))
            //    {
            //        mdbPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\App_Data" + mdbPath.Substring("|datadirectory|".Length);
            //    }
            //    else if (connectionString.StartsWith("./") || connectionString.EndsWith(".\\"))
            //    {
            //        connectionString = connectionString.Replace("/", "\\").Replace(".\\", AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\') + "\\");
            //    }
            //    connectionString = connectionString.Substring(0, connectionString.ToLower().IndexOf("data source")) + "Data Source=" + mdbPath;
            //}

            //如果是~则表示当前目录
            if (connectionString.Contains("~/") || connectionString.Contains("~\\"))
            {
                connectionString = connectionString.Replace("/", "\\").Replace("~\\", PathHelper.GetBaseDirectory().TrimEnd('\\') + "\\");
            }


             info = new DbConnectionMetaInfo();
            info.ProviderName = providerName;
            info.ConnectionString = connectionString;


            DbProviderFactory _factory = null;
            try
            {
                _factory = DbProviderFactories.GetFactory(providerName);

            }
            catch
            {
                //
            }
            if (_factory == null)
            {
                try
                {
                    _factory = LoadDbProvider(providerName);
                }
                catch (Exception x)
                {
                    throw x;

                }
            }

            if (_factory != null)
            {
                //info.Connection =  _factory.CreateConnection();
                //info.Connection.ConnectionString = connectionString;
                info.DbFactory = _factory;
                info.DatabaseType = GetDbType(info.ProviderName, info.DbFactory);

                //设置缓存
                SetDbConnectionMetaInfoToCache(key, info);
            }
            else
            {
                throw new Exception(string.Format("// Failed to load provider `{0}` - {1}", providerName, "No provider found!"));
                 
            }

            return info;

        }

        #region 下载驱动

        private static DbProviderFactory LoadDbProvider(string providerName)
        {
            DbProviderFactory provider = null;
             
                switch (providerName)
                {
                    case DbProviderNameManage.SQLite:
                        //if (Runtime.Mono)
                        //{
                        //    provider = GetProviderFactory("Mono.Data.Sqlite.dll", "Mono.Data.Sqlite.SqliteFactory");
                        //}
                        //else
                        //{
                        //    provider = GetProviderFactory("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteFactory");
                        //}
                        provider = GetProviderFactory("System.Data.SQLite.dll", "System.Data.SQLite.SQLiteFactory");

                        break;
                   
                    case DbProviderNameManage.MySql: provider = GetProviderFactory("MySql.Data.dll", "MySql.Data.MySqlClient.MySqlClientFactory");
                        break;
                    case DbProviderNameManage.SqlServerCe40: provider = GetProviderFactory("System.Data.SqlServerCe.dll", "System.Data.SqlServerCe.SqlCeProviderFactory");
                        break;
                    case DbProviderNameManage.PostgreSQL: provider = GetProviderFactory("Npgsql.dll", "Npgsql.NpgsqlFactory");
                        break;
                    case DbProviderNameManage.Oracle : provider = GetProviderFactory("System.Data.OracleClient.dll", "System.Data.OracleClient.OracleClientFactory");
                        break;
                    case DbProviderNameManage.OracleDataAccess : provider = GetProviderFactory("Oracle.DataAccess.dll", "Oracle.DataAccess.Client.OracleClientFactory");
                        break; 
                    case DbProviderNameManage.OracleManagedDataAccess: provider = GetProviderFactory("Oracle.ManagedDataAccess.dll", "Oracle.ManagedDataAccess.Client.OracleClientFactory");
                        break;
                    case DbProviderNameManage.FirebirdSql: provider = GetProviderFactory("FirebirdSql.Data.FirebirdClient.dll", "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory");
                        break;
#if NET45
                         case DbProviderNameManage.OleDb: provider = OleDbFactory.Instance;
                        break;
                    #endif


                default:
                        break;
                }
         

            return provider;
        }

        /// <summary>获取提供者工厂</summary>
        /// <param name="assemblyFile"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        protected static DbProviderFactory GetProviderFactory(String assemblyFile, String className)
        {
            //反射实现获取数据库工厂
            var file = assemblyFile;
            file = System.IO.Path.Combine(PathHelper.GetBaseDirectory(), file);
            if (!File.Exists(file))
            {
                if (!File.Exists(file)) throw new System.IO.FileNotFoundException("缺少文件" + file + "！", file);
            }



            var asm = Assembly.LoadFile(file);
            var type = asm.GetType(className);

            //if (type == null)
            //{
            //    //搜索当前Assembly是否存在
            //    type = className.GetTypeEx(true);

            //}
            if (type == null) return null;

            var field = type.GetFieldEx("Instance");
            if (field == null) return Activator.CreateInstance(type) as DbProviderFactory;

            //Type[] types = new Type[0];//为构造函数准备参数类型  
            //ConstructorInfo ci = type.GetConstructor(types); //获得构造函数  
            //object[] objs = new object[0];//为构造函数准备参数值  
            //object obj = ci.Invoke(objs);//调用构造函数创建对象  

            //return ReflectHelper.GetValue(obj, field) as DbProviderFactory;

            return ReflectHelper.GetValue(null, field) as DbProviderFactory;
        }


        #endregion


    }
}
