
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Pure.Data.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            DbProviderFactories.AddDbProviderFactories("MYSQL",   MySqlClientFactory.Instance);
            DbProviderFactories.AddDbProviderFactories("SQLSERVER", SqlClientFactory.Instance);
            DbProviderFactories.AddDbProviderFactories("SQLITE",  SQLiteFactory.Instance);
            DbProviderFactories.AddDbProviderFactories("ORACLE",  OracleClientFactory.Instance);


            Console.WriteLine("Hello World!");
            //AutoMigratorTest.Test();

            // TestExpression2SQL.Test();
            //TransactionTest.Test();
            //PredicatesTest.Test();
            // CRUDTest.Test();
            //SnapShotTest.Test();
            //  LobTypeTest.Test();
            //SqlCustomGeneratorTest.Test();
            //InterceptTest.Test();
            //FluentExpressionSqlBuilderTest.Test();
            //ClassMapTest.Test();
            // PerformanceTest.Test();
            //DDDTest.Test();
            //DIContainerAndSqlQueryTest.Test();
            //SqlMapTest.Test();
            //DynamicExpressoTest.Test();
            //ConfigurationTest.Test();
            //BulkTest.Test();
            //MigratorTest.Test();
            // IdGenerateTest.Test();
            ValidationTest.Test();
            //  BackupAndGenTest.Test();

            //PoolingTest.Test();
        }
    }
}
