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
    public class CRUDTest
    {

        public static void Test()
        {


            string title = "CRUDTest";
            Console.Title = title;

            CodeTimer.Time(title, 5, () => {

                //Constuct();
                //Batch();
                //Convert();
                Query();
                //CreateDataBase();
                //Insert();
                //InsertBatch();
               //Update();
                //Page();
                //TestOptExt();
                //QueryMutil();

            });


            Console.Read();
            
            
        }
        public static void CreateDataBase()
        {
            using (var db = DbMocker.NewDataBase())
            {

            }


        }
        public static void Insert()
        {
            var db = DbMocker.NewDataBase();
            //db.Config.EnableOrmLog = false;
            //db.Config.EnableDebug = true;
            //db.Config.KeepConnectionAlive = true;
            //db.Config.Interceptors.Add(new ConnectionTestIntercept());

            var data1 = db.FluentSqlBuilder.Select<UserInfo>().ExecuteList();
            data1 = db.FluentSqlBuilder.Select<UserInfo>().ExecuteList();

            UserInfo info = new UserInfo();
            info.Age = 67;
            info.Email = "sdfsd@qq.com";
            info.Name = "李威法";
            info.DTCreate = DateTime.Now;
            var ddd = db.Insert<UserInfo>(info, null);


            var user1 = new UserInfo
            {
                Name = "NameInsert",
                Age = 20 + 16,
                DTCreate = DateTime.Now
            };
            db.Insert(user1);
            //db.Connection.ExecuteScalarAsync

           
        }

        public static void InsertBatch()
        {
            var db = DbMocker.NewDataBase();
         //   db.Config.EnableOrmLog = false;
          //  db.Config.EnableDebug = true;
       //     db.Config.Interceptors.Add(new ConnectionTestIntercept());
            //var user1 = new UserInfo
            //{
            //    Name = "NameInsert",
            //    Age = 20 + 16,
            //    DTCreate = new DateTime(1985, 1, 1)
            //};
            //db.Insert(user1);



            List<UserInfo> list = new List<UserInfo>();
            for (int i = 0; i < 100; i++)
            {
                var user1 = new UserInfo
                {
                    Name = "NameInsert" + i,
                    Age = 20 + 16,
                    DTCreate = new DateTime(1985, 1, 1),
                    Role = RoleType.经理
                };
                list.Add(user1);
                //db.Insert(user1);
            }

            db.InsertBulk(list);
           // db.InsertBatch(list, 1000);
        }

        public static void Update()
        {
            var db = DbMocker.NewDataBase();


            var user1 = new UserInfo
            {
                Name = "NameTest" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1985, 1, 1)
            };
            int id = 5;
            var dd = db.Get<UserInfo>(id);

            dd.Name = "5345435";
            var co = db.Update<UserInfo>(dd);
            db.Update<UserInfo>(() => new UserInfo { Age = 88, Name = "李玮峰" }, p => p.Id == 5);
           // db.Update<UserInfo>(() => {return user1;}, p => p.Id == 5);

        }

        public static void Batch()
        {



            var db = DbMocker.NewDataBase();
            int total = 0;

            var batch = db.NewBatchCommand(new BatchOptions() { BatchSize = 3, StatementSeperator=";"});


            string command = "update tb_user2  set name ='sdfdf' where id = 3";
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));
            batch.AddOrProcess(batch.CreateCommand(command));

            PrintHelper.WriteLine("" );



        }

        public static void Convert()
        {



            var db = DbMocker.NewDataBase();
            int total = 0;

            var pgp = Predicates.Group(GroupOperator.And);
            var sortGroup = Predicates.SortGroup();
            sortGroup.Add(Predicates.Sort("", false));

            var datapage = db.GetPage<UserInfo>(1, 10, pgp, sortGroup, out total).ToList();
           
            var list22 = db.ExecuteReader("SELECT * FROM TB_USER ");
          //  var list33 = list22.ToModel<object>();
            var list = db.ExecuteList<object>("SELECT * FROM TB_USER2 ");

           // var list = db.ExecuteReader("SELECT * FROM TB_USER2 ").ToEnumerable<UserInfo>();
              
            //var Model = db.ExecuteModel<UserInfo>("SELECT * FROM TB_USER2 ");

            //var o = db.ExecuteDictionary<string, int>("SELECT T.NAME, SUM(T.Id) FROM TB_USER T GROUP BY T.NAME");


            PrintHelper.WriteLine(list.Count.ToString());



        }


        public static void Constuct()
        {
            using (var db = DbMocker.NewDataBase())
            {

            }
             
        }
            public static void Query()
        {



            var db = DbMocker.NewDataBase();
            int total = 0;

             
            string name = null;
            UserInfo predict = new UserInfo();
            predict.Id = 2;
            predict.Name = "lee";
            predict.DTCreate = DateTime.Now;

            //string[] names = null;

            var names = new List<string>(){ "23", "44" };

            var one = db.Get<UserInfo>(5);

            var o = db.ExecuteDictionary<string, int>("SELECT T.NAME, SUM(T.Id) FROM TB_USER T GROUP BY T.NAME");
            db = DbMocker.NewDataBase();
            db.Delete<UserInfo>(p=>names.Contains(p.Name));
            string name1 = "gsfdgdf";
            var list2 = db.Query<UserInfo>(p => p.Name == name1 && SqlFuncs.In(p.Name, names));
            db = DbMocker.NewDataBase();
            var list3 = db.Query<UserInfo>(p => p.Name == name1 && names.Contains(p.Name));


            //var list = db.Query<UserInfo>(p => predict.Id < p.Id).ToList();
            //var list1 = db.Query<UserInfo>(p =>p.Id > predict.Id &&  p.Name.Contains(predict.Name)  && p.DTCreate  > predict.DTCreate || p.Id  > 0).ToList();
            //var list2 = db.Query<UserInfo>(p => predict.DTCreate<p.DTCreate).ToList();
           
            PrintHelper.WriteLine("");



        }

        public static void TestOptExt()
        {
            var db = DbMocker.NewDataBase();

            //var one = db.FirstOrDefault<UserInfo>(p => p.Id > 1);
            var one2 = db.Get<UserInfo>(500);
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("Name", "dsfsdfdsf");
            parameters.Add("Age", 33);


            Dictionary<string, object> conditons = new Dictionary<string, object>();
            conditons.Add("Name", "dsfsdfdsf");
            conditons.Add("Age", 33);

            db.InsertByKV<UserInfo>(parameters);

            var count = db.CountByKV<UserInfo>(conditons);
            var countLong = db.LongCountByKV<UserInfo>(conditons);

           // db.UpdateBySQL("update TB_USER2 set name = 'sdfdfs' where id = 400");
             var dd2 = db.UpdateByKV<UserInfo>(parameters, conditons);

             var dd3 = db.DeleteByKV<UserInfo>(conditons);


            var o = db.DeleteById<UserInfo>(500);
               db.DeleteByIds<UserInfo>("Id"," 1,2,3");

        
        }

        public static void Page()
        {

            

            var db = DbMocker.NewDataBase(); 

            UserInfo user = new UserInfo();
            user.Age = 2;
            user.Id = 1;
            user.Name = "sdf";
            var existss2 = db.Exists<UserInfo>(f =>  f.Age == RoleType.管理员.GetHashCode());
            var existss = db.Exists<UserInfo>(f => f.Age > user.Age && f.Name == user.Name && f.Id != user.Id);

            string sql = "select * from TB_USER2 where Age > 10 AND AGE > :P0";
            int totalCount = 0;

            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("P0", 34);

            var reader = db.ExecuteReader(sql, ps);
            //var expando = reader.ToExpandoObjects();
            var data = db.GetPageBySQL<UserInfo>(1, 10, sql, " Age ASC ", ps, out totalCount);

            int total = 0;

             var predicateGroup = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };

            predicateGroup.Predicates.Add(Predicates.Field<UserInfo>(f => f.Age, Operator.Gt, 10));
            predicateGroup.Predicates.Add(Predicates.Field<UserInfo>(f => f.Name, Operator.Eq, null, true));
            predicateGroup.Predicates.Add(Predicates.Field<UserInfo>(f => f.Name, Operator.Like, "NameInse%"));
            predicateGroup.Predicates.Add(Predicates.Field<UserInfo>(f => f.DTCreate, Operator.Ge, new DateTime(1982,5,8)));
            predicateGroup.Predicates.Add(Predicates.Field<UserInfo>(f => f.DTCreate, Operator.Le, DateTime.Now));

            IList<ISort> sortGroup = new List<ISort>();
            var Sort = Predicates.Sort<UserInfo>(f => f.DTCreate, false);
            sortGroup.Add(Sort);


            var listExt = db.GetPage<UserInfo>(0, 5, predicateGroup, sortGroup, out total );
            //db = DbMocker.NewDataBase(DatabaseType.SQLite);
            //listExt = db.GetPage<UserInfo>(1, 5, predicateGroup, sortGroup, out total);

            var list = db.GetPageByWhere<UserInfo>(0, 5, "Age > 10", "", out total);
            PrintHelper.WriteLine(total + "=" + list.Count());



        }
        

        public static void QueryMutil()
        {
            var db = DbMocker.NewDataBase();
           string sqlDemo1 = @"SELECT * FROM TB_USER AS u WHERE u.Id=@UserId 
                                SELECT * FROM TB_USER AS ui WHERE ui.Id=2";
           using(var multi = db.GetMultiple(sqlDemo1, new { UserId = 1 }))
            {
                var user = multi.Read<UserInfo>().FirstOrDefault();
                var userinfo = multi.Read<UserInfo>().FirstOrDefault();
            }
        }
    }
}
