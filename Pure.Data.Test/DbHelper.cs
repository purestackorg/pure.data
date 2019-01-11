using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test
{

    //<!--OLE DB Services=-1表示打开连接池-->
    //<add name="Access" connectionString="Provider=Microsoft.Jet.OLEDB.4.0; Data Source=~\App_Data\Test.mdb;Persist Security Info=False;OLE DB Services=-1"/>
    //<add name="MSSQL" connectionString="Server=.;Integrated Security=SSPI;Database=Test" providerName="System.Data.SqlClient"/>
    //<!--DataPath指定数据库文件目录，反向工程创建数据库时用-->
    //<add name="MSSQL" connectionString="Server=.;User ID=sa;Password=sa;Database=Test;datapath=~\App_Data" providerName="System.Data.SqlClient"/>
    //<add name="SqlCe" connectionString="Data Source=test.sdf;" providerName="SqlCe"/>
    //<add name="SQLite" connectionString="Data Source=test.db;Version=3;" providerName="Sqlite"/>
    //<add name="MySql" connectionString="Server=127.0.0.1;Port=3306;Database=master;Uid=root;Pwd=root;" providerName="MySql.Data.MySqlClient"/>
    //<!--DllPath指定OCI目录，内部自动计算ORACLE_HOME目录；Owner指定不同于UserID的拥有者-->
    //<add name="Oracle" connectionString="Data Source=orc;User ID=sys;Password=admin;DllPath=..\oci" providerName="System.Data.OracleClient"/>
    //<!--角色名作为点前缀来约束表名，支持所有数据库-->
    //<add name="Oracle" connectionString="Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.34)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = ORC)));User ID=sys;Password=admin;DllPath=C:\OracleClient;Role=mis" providerName="System.Data.OracleClient"/>
    //<add name="Oracle" connectionString="Data Source=orc;Integrated Security=yes;" providerName="System.Data.OracleClient"/>
    //<add name="Oracle_OLEDB" connectionString="Provider=OraOLEDB.Oracle;Data Source=orc;User Id=sys;Password=admin;"/>
    //<add name="Firebird" connectionString="Server=.;Database=test.fdb;User=SYSDBA;Password=masterkey;" providerName="FirebirdSql.Data.FirebirdClient"/>
    //<!--ServerType可取值"0"或者"1"，"0"表明连接对象为普通服务器，"1"才是嵌入式服务器-->
    //<add name="Firebird_Embed" connectionString="Database=test;User=SYSDBA;Password=masterkey;ServerType=1" providerName="FirebirdSql.Data.FirebirdClient"/>
    //<add name="PostgreSQL" connectionString="Server=.;Database=master;Uid=root;Pwd=root;" providerName="PostgreSQL.Data.PostgreSQLClient"/>
    //<add name="Network" connectionString="Server=tcp://data.NewLifeX.com:8089;User ID=test;Password=test" providerName="Network"/>
    //<add name="Distributed" connectionString="WriteServer='ConnA*1,ConnB';ReadServer='ConnC*8, ConnD';" providerName="Distributed"/>

    public class DbHelper
    {
        //public static readonly string ConnectionString = "Data Source=.;Initial Catalog=Company;User Id=sa;Password=aaa0.0;";//// ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
        public static readonly string ConnectionString = "Data Source=.;Initial Catalog=RoCompany;User Id=sa;Password=aaa0.0;";//// ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        //oracle
        public static readonly string ConnectionStringOracle = "Data Source=GDEGOV;Persist Security Info=False;User ID=ReviewDB;Password=123456";

        public static readonly string ConnectionStringOracleDNT = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.6.51)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=gdcic11db)));Persist Security Info=True;User ID=ReviewDB;Password=123456;";
        //mysql
        public static readonly string ConnectionStringMysql = "Database='test';Data Source='127.0.0.1';Port=3306;User Id='root';Password='root';pooling=true";

        //sqlite
        
        //public static readonly string ConnectionStringSqlite = @"Data Source=F:\Benson\Source\RoGenerator\RoGenerator.Test\testdb\TestSqlite.db;Version=3";
        public static readonly string ConnectionStringSqlite = @"Data Source=D:\Life\Source\Ro\Pure\Pure.Data\Pure.Data\Pure.Data.Test\Data\sqlitetest.db;Version=3";
        public static DbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    return ds;
                }
            }

        }
        /// <summary>
        /// 返回结果集的第一行第一列
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return obj;
                }
            }
        }
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static int ExecuteNonQurey(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// 返回SqlDataReader 调用完此方法需将reader关闭
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParams)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(sqlString, conn);
            if (cmdParams != null)
            {
                cmd.Parameters.AddRange(cmdParams);
            }
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        public static DataTable FillDataTable(IDataReader reader)
        {
            DataTable dt = new DataTable();
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                DataColumn dc = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                dt.Columns.Add(dc);
            }
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < fieldCount; i++)
                {
                    dr[i] = reader[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static DataSet FillDataSet(IDataReader reader)
        {
            DataSet ds = new DataSet();
            var dt = FillDataTable(reader);
            ds.Tables.Add(dt);

            while (reader.NextResult())
            {
                dt = FillDataTable(reader);
                ds.Tables.Add(dt);
            }

            return ds;
        }

    }

}
