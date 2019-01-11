using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    /// <summary>
    /// 数据库提供程序名称
    /// </summary>
    public class DbProviderNameManage
    {
        /// <summary>
        /// 根据Provider字符串获取数据类型
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DatabaseType GetDatabaseType(string provider)
        {
            DatabaseType dbType = DatabaseType.None;
            provider = provider.ToLower();
            if (provider == SQLServer.ToLower() || provider == SQLAzure.ToLower())
            {
                dbType = DatabaseType.SqlServer;
            }
            else if (provider == SqlServerCe.ToLower() || provider == SqlServerCe40.ToLower())
            {
                dbType = DatabaseType.SqlCe;
            }
            else if (provider == SQLite.ToLower())
            {
                dbType = DatabaseType.SQLite;
            }

            else if (provider == MySql.ToLower())
            {
                dbType = DatabaseType.MySql;
            }
            else if (provider == Oracle.ToLower())
            {
                dbType = DatabaseType.Oracle;
            }
            else if (provider == OracleDataAccess.ToLower())
            {
                dbType = DatabaseType.Oracle;
            }
            else if (provider == OracleManagedDataAccess.ToLower())
            {
                dbType = DatabaseType.Oracle;
            }
            else if (provider == Access.ToLower())
            {
                dbType = DatabaseType.Access;
            }
            else if (provider == OleDb.ToLower())
            {
                dbType = DatabaseType.OleDb;
            }
            else if (provider == Odbc.ToLower())
            {
                dbType = DatabaseType.OleDb;
            }
            else if (provider == PostgreSQL.ToLower())
            {
                dbType = DatabaseType.PostgreSQL;
            }
            else if (provider == DB2.ToLower())
            {
                dbType = DatabaseType.DB2;
            }
            else if (provider == FirebirdSql.ToLower())
            {
                dbType = DatabaseType.Firebird;
            }
            else if (provider == SybaseASA.ToLower())
            {
                dbType = DatabaseType.SybaseASA;
            }
            else if (provider == SybaseASE.ToLower())
            {
                dbType = DatabaseType.SybaseASE;
            }
            else if (provider == SybaseUltraLite.ToLower())
            {
                dbType = DatabaseType.SybaseUltraLite;
            }
            else if (provider == DM.ToLower())
            {
                dbType = DatabaseType.DM;
            }
            return dbType;
        }

        /// <summary>
        /// SQLServer数据库提供程序名称：System.Data.SqlClient
        /// </summary>
        public const string SQLServer = "System.Data.SqlClient";
        /// <summary>
        /// SqlServerCe数据库提供程序名称：System.Data.SqlServerCe
        /// </summary>
        public const string SqlServerCe = "System.Data.SqlServerCe.4.0";
        /// <summary>
        /// 微软Azure云数据库
        /// </summary>
        public const string SQLAzure = "System.Data.SqlClient";
        /// <summary>
        /// SqlServerCe.4.0数据库提供程序名称：System.Data.SqlServerCe4.0
        /// </summary>
        public const string SqlServerCe40 = "System.Data.SqlServerCe.4.0";
        /// <summary>
        /// SQLite数据库提供程序名称：System.Data.SQLite
        /// </summary>
        public const string SQLite = "System.Data.SQLite";

        /// <summary>
        /// MySql数据库提供程序名称：MySql.Data.MySqlClient
        /// </summary>
        public const string MySql = "MySql.Data.MySqlClient";

        /// <summary>
        /// Oracle数据库提供程序名称：System.Data.OracleClient
        /// </summary>
        public const string Oracle = "System.Data.OracleClient";
        public const string OracleDataAccess = "Oracle.DataAccess.Client";
        public const string OracleManagedDataAccess = "Oracle.ManagedDataAccess.Client";

        
        public const string Access = "System.Data.OleDb";//"Microsoft.Jet.OLEDB.4.0";
        /// <summary>
        /// 基于OleDb驱动的数据库提供程序名称：System.Data.OleDb
        /// </summary>
        public const string OleDb = "System.Data.OleDb";
        /// <summary>
        /// 基于ODBC驱动的数据库。描述值为：Odbc。
        /// </summary>
        public const string Odbc = "System.Data.Odbc";
        /// <summary>
        /// 基于PostgreSQL驱动的数据库(加州大学伯克利分校的PostgreSQL数据库)提供程序名称：Npgsql
        /// </summary>
        public const string PostgreSQL = "Npgsql";
        public const string DB2iSeries = "IBM.Data.DB2.iSeries";
        /// <summary>
        /// IBM公司的DB2数据库。描述值为：Db2。
        /// </summary>
        public const string DB2 = "IBM.Data.DB2";
        /// <summary>
        /// Firebird数据库。描述值为：Firebird
        /// </summary>
        public const string FirebirdSql = "FirebirdSql.Data.FirebirdClient";
        /// <summary>
        /// IBM公司的Informix数据库。描述值为：Informix。
        /// </summary>
        public const string Informix = "IBM.Data.Informix";


        /// <summary>
        /// Sybase公司的SybaseASA数据库。描述值为：SybaseASA。
        /// </summary>
        public const string SybaseASA = "iAnyWhere.Data.SQLAnyWhere";

        /// <summary>
        /// Sybase公司的SybaseASE数据库。描述值为：SybaseASE。
        /// </summary>
        public const string SybaseASE = "Sybase.Data.AseClient";
        /// <summary>
        /// Sybase公司的SybaseUltraLite数据库。描述值为：SybaseUltraLite。
        /// </summary>
        public const string SybaseUltraLite = "iAnyWhere.Data.UltraLite";
        /// <summary>
        /// 国产达梦数据库。描述值为：Dm。
        /// </summary>
        public const string DM = "Dm";

        #region NoSql
        public const string NoSQL = "NoSQL";
        public const string MongoDB = "MongoDB";
        public const string Hadoop_HBase = "Hadoop_HBase";
        public const string CouchDB = "CouchDB";
        public const string Neo4J = "Neo4J";
        public const string LiteDB = "LiteDB";
        public const string None = "";

        #endregion






    }
}
