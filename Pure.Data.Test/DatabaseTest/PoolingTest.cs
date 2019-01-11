//using FluentExpressionSQL;
//using FluentExpressionSQL.Mapper;
//using Expression2SqlTest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Collections.Concurrent;
//using System.Threading;
//using Pure.Data.Pooling.Impl;

//namespace Pure.Data.Test
//{
//    public class PoolingTest
//    {

//        public static void Test()
//        {


//            string title = "PoolingTest";
//            Console.Title = title;

//            CodeTimer.Time(title, 10, () => {

//                //TestRun();
//                TestDbContext();


//            });


//            Console.Read();


//        }
//        public static void TestRun()
//        {

//            var s = new GenericPoolSample();
//            //s.Test();
//            //Console.WriteLine("test is finish");
//            //Console.ReadKey();
//            s.TestParalle();
//            Console.WriteLine("Finish");
//            Console.ReadLine();

//        }
//        public static void TestPoolWithKey()
//        {
//            var hashSet = new HashSet<int>();
//            var pool = new GenericKeyedObjectPool<string, VerifyStruct>(new VerifyStructFactory(), new GenericKeyedObjectPoolConfig() { TestOnBorrow = true, TestOnReturn = true });
//            for (int i = 0; i < 100; i++)
//            {
//                var obj = pool.BorrowObject("test");

//                Console.WriteLine("test is BorrowObject-" + obj.CreateTime);
//                pool.ReturnObject("test", obj);
//                hashSet.Add(obj.GetHashCode());
//                Console.WriteLine("Finish");
//                Thread.Sleep(1000);
//            }

//        }

//        private static void ExeTExt(){
//            var db = DbMocker.NewDataBase();
//            var o = db.Get<UserInfo>(1);
//            o.DTCreate = DateTime.Now;
//             db.Update<UserInfo>(o);
//        }  
//        public static void TestDbContext()
//        {
//           // ExeTExt();

//            Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 5 },
//                  () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();
//                  }, () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();

//                  }, () =>
//                  {
//                      ExeTExt();

//                  }
//              );

//            //var runResult = Parallel.For(
//            //    0,
//            //    4,
//            //    (i) =>
//            //    {
//            //        var db = DbMocker.NewDataBase();
//            //        var o = db.Get<UserInfo>(1);
//            //    });
//            //if (!runResult.IsCompleted)
//            //{
//            //    Thread.Sleep(100);
//            //}

//            //using (var db = DbMocker.NewDataBase())
//            //{


//            //}

//        }


//    }







//}
