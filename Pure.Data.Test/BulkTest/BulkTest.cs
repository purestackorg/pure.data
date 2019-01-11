using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Pure.Data.Test
{
    public class BulkTest
    {

        public static void Test()
        {


            string title = "BulkTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {
              //  InsertBulk();
            });


            Console.Read();
            
            
        }

        //public static void InsertBulk()
        //{
        //    var mapping = MapBuilder
        //        .MapAllProperties<MyDomainEntity>()
        //        .DestinationTable("MyDomainEntities")       // Optional
        //        .MapProperty(x => x.Id, x => x.DoNotMap())  // Required for ID properties where you want to use the DBs auto-increment feature.
        //        .Build();
        //    //sql server
        //    //var option = new BulkOption(DbHelper.ConnectionString, SqlBulkType.SqlServer);

        //    //Mysql
        //    //var option = new BulkOption(DbHelper.ConnectionStringMysql, SqlBulkType.MySQL);
        //    //Oracle
        //    //var option = new BulkOption(DbHelper.ConnectionStringOracle, SqlBulkType.Oracle);

        //    //sqlite
        //    var option = new BulkOption(DbHelper.ConnectionStringSqlite, SqlBulkType.Sqlite);
        //    option.EnableTransaction = true;
        //    //option.BatchSize = 10000;
        //    using (var bulkWriter = mapping.CreateBulkWriter(option))
        //    {
        //        var items = GetDomainEntities();
        //        bulkWriter.WriteToDatabase(items);
        //    }
        //}

        private static IEnumerable<MyDomainEntity> GetDomainEntities()
        {
            for (int i = 0; i < 1000000; i++)
            {
                yield return new MyDomainEntity();
            }
        }

        public class MyDomainEntity
        {
            public int Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
            public MyDomainEntity()
            {
                FirstName = "";
                LastName = "";
                //LastName = Guid.NewGuid().ToString();
            }
        }

    }
}
