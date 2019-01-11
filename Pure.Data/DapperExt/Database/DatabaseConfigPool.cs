using System.Data;
using System.Collections.Concurrent;
using System;
using System.Linq;
using System.Collections.Generic;
using FluentExpressionSQL;
namespace Pure.Data
{
    /*
     SQLServer简易连接:
    Server=服务器地址;Database=数据库名称;User Id=用户名;Password=密码;

SQLServer本地文件可信连接:
    Server=.\SQLExpress;AttachDbFilename=|DataDirectory|mydbfile.mdf;Database=数据库名称;Trusted_Connection=sspi;

SqlServer自定义连接:
    Data Source=(LOCAL);Initial Catalog=数据库名称;User ID=用户名;Password=密码;Persist Security Info=True;Enlist=true;Max Pool Size=300;Min Pool Size=0;Connection Lifetime=300;Packet Size=1000;

Oracle简易连接:
    Data Source=orclsid_127.0.0.1;User Id=用户名;Password=密码;
    //这个数据源是从Oracle的安装目录下tnsnames.ora文件中去找的。而并非是在系统的“管理工具”下的“数据源(ODBC)”中找。这个tnsnames.ora文件是在Oracle的安装目录下的“client_1/network/admin/”下。
    Data Source=GDEGOV;Persist Security Info=False;User ID=ReviewDB;Password=123456
Oracle自定义连接:
    Server=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=服务器地址)(PORT=端口号)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=数据库名称)));User Id=用户名;Password=密码;Persist Security Info=True;Enlist=true;Max Pool Size=300;Min Pool Size=0;Connection Lifetime=300;

Access简易连接:
    Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\mydatabase.mdb;User Id=用户名;Password=密码;
    Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\mydatabase.mdb;Jet OLEDB:Database Password=密码;
     
     *
 Mysql
    // MySQL Connector/Net 指定端口写法，默认端口：3306     
Server=myServerAddress;Port=1234;Database=myDataBase;Uid=myUsername;Pwd=myPassword;   
     */
    public class ConnectionStringSet
    {
        public ConnectionStringSet()
        {
            Items = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Items { get; private set; }
        public string DataSource
        {
            get
            {
                return Items.FirstOrDefault(p => p.Key.Contains("SOURCE") || p.Key.Contains("SERVER")).Value;
            }
        }
        public string Database
        {
            get
            {
                return Items.FirstOrDefault(p => p.Key.Contains("DATABASE") || p.Key.Contains("CATALOG") || p.Key.Contains("SERVICE_NAME")).Value;
            }
        }

        public string UserID
        {
            get
            {
                return Items.FirstOrDefault(p => p.Key.Contains("USER ID") || p.Key.Contains("UID")).Value;
            }
        }
        public string Password
        {
            get
            {
                return Items.FirstOrDefault(p => p.Key.Contains("PASSWORD") || p.Key.Contains("PWD")).Value;
            }
        }

        public string UniqueKey
        {
            get
            {
                return DataSource + "_" + Database + "_" + UserID;
            }
        }
    }

    //public class DbKernelSet
    //{
    //    public DbKernelSet(FluentExpressionSqlBuilder _builder, IDapperImplementor _dapper)
    //    {
    //        FluentSqlBuilder = _builder;
    //        Dapper = _dapper;
    //    }
    //    public FluentExpressionSqlBuilder FluentSqlBuilder { get; private set; }
    //    public IDapperImplementor Dapper { get; private set; }

    //}
    /// <summary>
    /// 初始化状态进程
    /// </summary>
    public class InitStatusProcess
    {
        public bool HasFirstLoadFinish { get; set; }
        public bool HasLoadAllClassMap { get; set; }
        public bool HasInitValidation { get; set; }
        public bool HasInitSqlMap { get; set; }
    }
    public class DatabaseConfigPool
    {
        //static Dictionary<string, IDatabase> pool = new Dictionary<string, IDatabase>();
        //private static readonly ConcurrentDictionary<string, IDatabase> pool = new ConcurrentDictionary<string, IDatabase>();
        private static readonly ConcurrentDictionary<string, string> KeyPool = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, ConnectionStringSet> SetPool = new ConcurrentDictionary<string, ConnectionStringSet>();
        private static readonly ConcurrentDictionary<string, InitStatusProcess> InitStatusPool = new ConcurrentDictionary<string, InitStatusProcess>();
        //private static readonly ConcurrentDictionary<string, DbKernelSet> DbKernelSetPool = new ConcurrentDictionary<string, DbKernelSet>();
        public static object FirstLoadingLock = new object();

        /// <summary>
        /// 获取初始化进度状态 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static InitStatusProcess GetInitStatus(IDatabase db)
        {
            var key = db.DatabaseName;
            InitStatusProcess status = null;
            if (InitStatusPool.TryGetValue(key, out status))
            {
                return status;
            }
            else
            {
                status = new InitStatusProcess();
                InitStatusPool[key] = status;
                return status;
            }
        }


        public static readonly object _logHelperLock = new object();
        public static readonly object _DatabaseConfigLock = new object();
        public static readonly object _DatabaseConnectionLock = new object();

        public static ConnectionStringSet GetConnectionStringSet(string connStr)
        {
            ConnectionStringSet set;
            if (!SetPool.TryGetValue(connStr, out set))
            {
                string[] result = connStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                set = new ConnectionStringSet();
                foreach (string item in result)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string[] block = item.Split('=');
                        if (block.Length >= 2)
                        {
                            set.Items.Add(block[0].ToUpper(), block[1]);
                        }
                    }
                }

                SetPool[connStr] = set;
            }

            return set;
        }

        public static int GetConnectionHashKey(IDbConnection cnn)
        {
            return cnn.GetHashCode();
        }
        public static string GetConnectionKey(IDbConnection cnn)
        {
            if (string.IsNullOrEmpty(cnn.Database))
            {
                //二级缓存数据库链接字符键值
                string key = GetKey(cnn.ConnectionString);
                if (!string.IsNullOrEmpty(key))
                {
                    return key;
                }
                else
                {
                    ConnectionStringSet set = GetConnectionStringSet(cnn.ConnectionString);
                    if (set != null)
                    {
                        string dbName = set.UniqueKey;
                        SetKey(cnn.ConnectionString, dbName);
                        return dbName;
                    }
                }

                return cnn.ConnectionString;

            }
            else
            {
                return cnn.Database;

            }
        }


        //public static void Set(string key, IDatabase config)
        //{
        //    pool[key] = config;
        //}

       
        //public static IDatabase Get(string key)
        //{
        //    IDatabase db;
        //    pool.TryGetValue(key, out db);
        //    return db;
            
        //}
        private static void SetKey(string key, string dbName)
        {
            if (!KeyPool.ContainsKey(key))
            {
                KeyPool[key] = dbName;
            }
        }

        private static string GetKey(string key)
        {
            string str;
            KeyPool.TryGetValue(key, out str);
            return str;
        }
        //二级缓存启动配置文件
        //public static void SetDbKernel(string key, DbKernelSet dbName)
        //{
        //    if (!DbKernelSetPool.ContainsKey(key))
        //    {
        //        DbKernelSetPool[key] = dbName;
        //    }
        //}

        //public static DbKernelSet GetDbKernel(string key)
        //{
        //    DbKernelSet str;
        //    DbKernelSetPool.TryGetValue(key, out str);
        //    return str;
        //}

    }
}
