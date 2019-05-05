using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

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
                InsertBatch();
            });


            Console.Read();
            
            
        }
        public async static void InsertBatch()
        {
            //var table = new DataTable("Batcher");
            //table.Columns.Add("Id", typeof(int));
            //table.Columns.Add("Name1", typeof(string));
            //table.Columns.Add("Name2", typeof(string));
            //table.Columns.Add("Name3", typeof(string));
            //table.Columns.Add("Name4", typeof(string));

            ////构造100000条数据
            //for (var i = 0; i < 100000; i++)
            //{
            //    table.Rows.Add(i, i.ToString(), i.ToString(), i.ToString(), i.ToString());
            //}

            var db = DbMocker.InstanceDataBase();
            ////db.InsertBatch(table, new BatchOptions() { BatchSize=10000});
            //db.InsertBulk(table);

            var list = new List<UserInfo>();
            for (var i = 0; i < 1000; i++)
            {
                list.Add(new UserInfo() { Id= i, Name = "Jesica-"+i, Age=88, DTCreate = DateTime.Now, Email = i.ToString(), Role= RoleType.普通用户, HasDelete= false, StatusCode=1} );
            }

            //await db.InsertBulkAsync(list);
            await db.InsertBatchAsync(list);


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
            for (int i = 0; i < 100000; i++)
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
