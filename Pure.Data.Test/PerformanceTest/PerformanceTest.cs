using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
//using Dapper;
//using ServiceStack.OrmLite;
//using ServiceStack.OrmLite.SqlServer;
namespace Pure.Data.Test
{
    public class PerformanceTest
    {

        public static void Test()
        {


            string title = "PerformanceTest";
            Console.Title = title;

            //var db = DbMocker.InstanceDataBase();
            //db.DatabaseConfig.EnableOrmLog = false;

            ////db.DatabaseConfig.EnableDebug = false;
            //db.DatabaseConfig.EnableDebug = true;
            //db.DatabaseConfig.EnableIntercept = true;
            //db.DatabaseConfig.Interceptors.Add(new ConnectionTestIntercept());
            int round = 5;
            CodeTimer.Time(title, 1, () =>
            {
                ConstructTest();
            });
            //CodeTimer.Time(title, round, () =>
            //{
            //    PetapocoNormal(50);
            //});
            //CodeTimer.Time(title, round, () =>
            //{
            //    PetapocoFast(50);
            //});
            //CodeTimer.Time(title, round, () =>
            //{
            //    OrmLiteNormal(50);
            //});
            //CodeTimer.Time(title, round, () =>
            //{
            //    SimpleDataFast(50);
            //});

            //CodeTimer.Time(title, round, () =>
            //{
            //    DapperQueryTest(50, 5);
            //});
            //CodeTimer.Time(title, round, () =>
            //{

            //    //Insert();
            //    //Update();
            //    //Page();
            //    PureDataQueryTest(50, 5);
            //});
            //CodeTimer.Time(title, round, () =>
            //{

            //    //Insert();
            //    //Update();
            //    //Page();
            //    PureDataQueryTest2(50, 5);
            //});
            //CodeTimer.Time(title, round, () =>
            //{

            //    //Insert();
            //    //Update();
            //    //Page();
            //    PureDataQueryTest3(50, 5);
            //});
            //CodeTimer.Time(title, round, () =>
            //{

            //    //Insert();
            //    //Update();
            //    //Page();
            //    PureDataQueryTest4(50, 5);
            //});
            Console.Read();


        }

        //public static void PetapocoNormal(int takeCount)
        //{
        //    using (var petapoco = new PetaPoco.Database(DbHelper.ConnectionString, "System.Data.SqlClient"))
        //    {
        //        petapoco.OpenSharedConnection();
        //        var list = petapoco.Fetch<UserInfo>(string.Format("select top {0} * from TB_USER", takeCount.ToString()));

        //    }


        //}
        //public static void PetapocoFast(int takeCount)
        //{
        //    using (var petapoco = new PetaPoco.Database(DbHelper.ConnectionString, "System.Data.SqlClient"))
        //    {
        //        petapoco.OpenSharedConnection();
        //        petapoco.EnableAutoSelect = false;
        //        petapoco.EnableNamedParams = false;
        //        petapoco.ForceDateTimesToUtc = false;
        //        var list = petapoco.Fetch<UserInfo>(string.Format("select top {0} * from TB_USER", takeCount.ToString()));

        //    }


        //}

        //public static void OrmLiteNormal(int takeCount)
        //{

        //    OrmLiteConfig.DialectProvider = SqlServerOrmLiteDialectProvider.Instance; //Using SQL Server
        //    var c = DbHelper.CreateConnection();
        //    c.Close();
        //    IDbCommand ormLiteCmd = c.CreateCommand();
        //    ormLiteCmd.Select<UserInfo>(string.Format("select top {0} * from TB_USER", takeCount.ToString()));

        //}

        //public static void SimpleDataFast(int takeCount)
        //{
        //    // Simple.Data
        //    var sdb = Simple.Data.Database.OpenConnection(DbHelper.ConnectionString);

        //    IEnumerable<dynamic> d = sdb.UserInfo.All();
        //    var data = d.Take(takeCount);



        //}

        public static void Page()
        {
            var db = DbMocker.NewDataBase();
            int total = 0;
            var list = db.GetPageByWhere<UserInfo>(1, 5, "Age > 10", "", out total);
            PrintHelper.WriteLine(total + "=" + list.Count());
        }

        static void ConstructTest()
        {
            var db1 = DbMocker.NewDataBase();
            db1.EnsureOpenConnection();
                    //PrintHelper.WriteLine(db1.Connection?.State + "=" + db1.GetHashCode());
            for (var b = 0; b < 50; b++)
            { 
                //Task task = new Task( () => {
                    var db   = DbMocker.NewDataBase();
                   // db.EnsureOpenConnection();
                    var list3 = db.ExecuteScalar<int>("select id from tb_user where id = 1");//.ExecuteReader("select * from tb_user").ToList<UserInfo>();// db.Query<UserInfo>().ToList();
                var ddd = db.Get<UserInfo>(1);
                    //}
                    //PrintHelper.WriteLine(db.Connection?.State + "=" + db.GetHashCode());
                //});
                //task.Start();
                 
            }


            //Task.Factory.StartNew(() =>
            //{
            //    using (var db = DbMocker.NewDataBase())
            //    {
            //        var list = db.ExecuteScalar("select 1 from tb_user");// db.Query<UserInfo>().ToList();

            //        var db1 = DbMocker.NewDataBase();
            //        int total = 0;
            //        var list2 = db1.GetPageByWhere<UserInfo>(1, 5, "Age > 10", "", out total).ToList();
            //        var db2 = DbMocker.NewDataBase();

            //        var list3 = db2.ExecuteScalar("select 1 from tb_user");//.ExecuteReader("select * from tb_user").ToList<UserInfo>();// db.Query<UserInfo>().ToList();

            //        db2.Close();

            //        db1.Close();

            //        db.Close();

            //    }


            //});


            //
            //System.Threading.Thread.Sleep(500);


            //var db = DbMocker.NewDataBase();
            // var db = DbMocker.InstanceDataBase().OpenNewConnection();
        }
        static void DapperQueryTest(int takeCount, int id)
        {
            using (IDbConnection conn = DbHelper.CreateConnection())
            {
                var list = conn.Query<UserInfo>(string.Format("select  * from TB_USER where Id>@Id"), new { Id = id }).ToList();
                //PrintHelper.WriteLine(list.Count.ToString());

            }
        }

        static void DapperQueryTest(int takeCount)
        {
            using (IDbConnection conn = DbHelper.CreateConnection())
            {
                var list = conn.Query<UserInfo>(string.Format("select top {0} * from TB_USER", takeCount.ToString())).ToList();
            }
        }


        static void PureDataQueryTest(int takeCount)
        {
            var db = DbMocker.InstanceDataBase().OpenSharedConnection();
            db.Config.EnableOrmLog = false;
            db.Config.EnableDebug = false;
            db.Config.EnableIntercept = false;
            var list = db.QueryBySQL<UserInfo>(string.Format("select top {0} * from TB_USER", takeCount.ToString())).ToList();

        }

        static void PureDataQueryTest(int takeCount, int id)
        {
            //using (var db = DbMocker.NewDataBase())
            {
                //single
                //var db = DbMocker.InstanceDataBase();
                //single but new connection
                var db = DbMocker.InstanceDataBase().OpenNewConnection();
                db.Config.EnableOrmLog = false;
                db.Config.EnableDebug = false;
                db.Config.EnableIntercept = false;
                //db.DatabaseConfig.EnableDebug = true;
                //db.DatabaseConfig.Interceptors.Add(new ConnectionTestIntercept());

                // var list = db.Query<UserInfo>(p => p.Id > id, (p) => { p.Top(takeCount); }).ToList(); // string.Format("select top {0} * from TB_USER where Id>@Id", takeCount.ToString(), new { Id = id })).ToList();
                var predict = Predicates.Field<UserInfo>(f => f.Id, Operator.Gt, id, false);
                var list = db.Query<UserInfo>(predict).ToList();
                //PrintHelper.WriteLine(list.Count.ToString());
            }

        }

        static void PureDataQueryTest2(int takeCount, int id)
        {
            //using (var db = DbMocker.NewDataBase())
            {
                //single
                var db = DbMocker.InstanceDataBase();
                //single but new connection
                //var db = DbMocker.InstanceDataBase().CreateNewConnection();
                db.Config.EnableOrmLog = false;
                db.Config.EnableDebug = false;
                db.Config.EnableIntercept = false;
                //db.DatabaseConfig.EnableDebug = true;
                //db.DatabaseConfig.Interceptors.Add(new ConnectionTestIntercept());

                // var list = db.Query<UserInfo>(p => p.Id > id, (p) => { p.Top(takeCount); }).ToList(); // string.Format("select top {0} * from TB_USER where Id>@Id", takeCount.ToString(), new { Id = id })).ToList();
                var predict = Predicates.Field<UserInfo>(f => f.Id, Operator.Gt, id, false);
                var list = db.Query<UserInfo>(predict).ToList();
                //PrintHelper.WriteLine(list.Count.ToString());
            }

        }
        static void PureDataQueryTest3(int takeCount, int id)
        {
            using (var db = DbMocker.NewDataBase())
            {
                //single
                //var db = DbMocker.InstanceDataBase();
                //single but new connection
                //var db = DbMocker.InstanceDataBase().CreateNewConnection();
                db.Config.EnableOrmLog = false;
                db.Config.EnableDebug = false;
                db.Config.EnableIntercept = false;
                //db.DatabaseConfig.EnableDebug = true;
                //db.DatabaseConfig.Interceptors.Add(new ConnectionTestIntercept());

                // var list = db.Query<UserInfo>(p => p.Id > id, (p) => { p.Top(takeCount); }).ToList(); // string.Format("select top {0} * from TB_USER where Id>@Id", takeCount.ToString(), new { Id = id })).ToList();
                var predict = Predicates.Field<UserInfo>(f => f.Id, Operator.Gt, id, false);
                var list = db.Query<UserInfo>(predict).ToList();
                //PrintHelper.WriteLine(list.Count.ToString());
            }

        }

        //static void PureDataQueryTest4(int takeCount, int id)
        //{
        //    using (var cx = new SqlServerTestDbContext())
        //    {
        //        var db = cx.Database;

        //        db.Config.EnableOrmLog = false;
        //        db.Config.EnableDebug = false;
        //        db.Config.EnableIntercept = false;
        //        //db.DatabaseConfig.EnableDebug = true;
        //        //db.DatabaseConfig.Interceptors.Add(new ConnectionTestIntercept());

        //        // var list = db.Query<UserInfo>(p => p.Id > id, (p) => { p.Top(takeCount); }).ToList(); // string.Format("select top {0} * from TB_USER where Id>@Id", takeCount.ToString(), new { Id = id })).ToList();
        //        var predict = Predicates.Field<UserInfo>(f => f.Id, Operator.Gt, id, false);
        //        var list = db.Query<UserInfo>(predict).ToList();
        //        //PrintHelper.WriteLine(list.Count.ToString());
        //    }

        //}
    }
}
