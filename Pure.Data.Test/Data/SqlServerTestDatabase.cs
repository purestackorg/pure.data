using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test
{
    public class SqlServerTestDatabase:Database
    {
        public static void Log(string msg,Exception ex, MessageType type= MessageType.Debug){
            ConsoleHelper.Instance.OutputMessage(msg,ex, type);
        }
        public SqlServerTestDatabase()
            //: base("Data Source=GDEGOV;Persist Security Info=False;User ID=ReviewDB;Password=123456", DatabaseType.Oracle),
            //: base(DbHelper.ConnectionString, DatabaseType.SqlServer, 
           // : base(DbHelper.ConnectionStringOracle, DatabaseType.Oracle, 
            //: base(DbHelper.ConnectionStringMysql, DatabaseType.MySql, 
            : base("PureDataConfiguration.xml", Log, 
            config => {

                //bool enableDebug = true;
                //config.EnableDebug = enableDebug;
                //config.EnableIntercept = enableDebug;
                //config.Interceptors.Add(new ConnectionTestIntercept());

                //config.EnableOrmLog = enableDebug;
                 
                //config.MappingAssemblies.Add(typeof(UserRepository).Assembly);
                //config.EnableSqlMap = true;
                ////config.SqlMapPaths.Add("sqlmap/sql_dev.xml");
                //config.SqlMapDirPaths.Add("sqlmap");
                //config.IsWatchSqlMapFile = true;
                //config.WatchSqlMapInterval = 5000;


                //config.AutoMigrate = false;
                //config.EnableAutoMigrateDebug = false;
                //config.AutoMigrateOnContainTable = "Tb_USER2";

                //config.EnableGlobalIgnoreUpdatedColumns = true;
                //config.GlobalIgnoreUpdatedColumns = "Id;VersionCol;Role;DTCReaTE";

                //config.EnableConnectionPool = false;
                //config.EnableLogConnectionPool = true;
                //config.ConnectionPoolMinSize = 1;
                //config.ConnectionPoolMaxSize = 5;
                //config.ConnectionPoolStepSize = 2;

            })
        //: base("Data Source=.;Initial Catalog=RoCompany;User Id=sa;Password=aaa0.0;", DatabaseType.SqlServer)
        {
        }
    }


    public class SqliteTestDatabase : Database
    {
        public SqliteTestDatabase()
            //: base("Data Source=GDEGOV;Persist Security Info=False;User ID=ReviewDB;Password=123456", DatabaseType.Oracle)
            //: base(DbHelper.ConnectionString, DatabaseType.SqlServer, 
            //: base(DbHelper.ConnectionStringOracle, DatabaseType.Oracle, 
            : base(DbHelper.ConnectionStringSqlite, DatabaseType.SQLite,

            config =>
            {
                config.EnableDebug = true;
                config.EnableIntercept = true;
                config.EnableOrmLog = true;
                config.MappingAssemblies.Add(typeof(DbMocker).Assembly);
                config.SqlMapFilePaths.Add("sqlmap/sql_dev.xml");
                config.AutoMigrate = false;
            })
        //: base("Data Source=.;Initial Catalog=RoCompany;User Id=sa;Password=aaa0.0;", DatabaseType.SqlServer)
        {
        }
    }
}
