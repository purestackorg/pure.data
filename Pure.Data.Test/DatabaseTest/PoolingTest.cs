using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
//using Pure.Data.Pooling.Impl;

namespace Pure.Data.Test
{
    public class PoolingTest
    {

        public static void Test()
        {


            string title = "PoolingTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () =>
            {
                //TestGetAndReturn();
                //TestRun();
                //TestDbContext();

                TestDbContext();
            });

            string info = pooldb.Pool.ShowStatisticsInfo();

            Log(info, null, MessageType.Debug);
            Console.Read();


        }
        //public static void TestRun()
        //{

        //    var s = new GenericPoolSample();
        //    //s.Test();
        //    //Console.WriteLine("test is finish");
        //    //Console.ReadKey();
        //    s.TestParalle();
        //    Console.WriteLine("Finish");
        //    Console.ReadLine();

        //}
        //public static void TestPoolWithKey()
        //{
        //    var hashSet = new HashSet<int>();
        //    var pool = new GenericKeyedObjectPool<string, VerifyStruct>(new VerifyStructFactory(), new GenericKeyedObjectPoolConfig() { TestOnBorrow = true, TestOnReturn = true });
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var obj = pool.BorrowObject("test");

        //        Console.WriteLine("test is BorrowObject-" + obj.CreateTime);
        //        pool.ReturnObject("test", obj);
        //        hashSet.Add(obj.GetHashCode());
        //        Console.WriteLine("Finish");
        //        Thread.Sleep(1000);
        //    }

        //}
        public static void Log(string msg, Exception ex, MessageType type = MessageType.Debug)
        {
            ConsoleHelper.Instance.OutputMessage(msg, ex, type);
            FastLogger.WriteLog(msg);
        }
        public static PooledDatabase pooldb = new PooledDatabase(() => new Pooling.PooledObjectWrapper<IDatabase>(new Database("PureDataConfiguration.xml", Log,
         config => {

         })) {
            LogAction = Log,
            OnValidateObject = (ctx) =>
            {
                if (ctx.Direction == Pooling.PooledObjectDirection.Inbound)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            },
            OnReleaseResources = (resource) => { Log("Release -> " + resource.GetHashCode() +" , connection: "+ resource.Connection.GetHashCode() +", status : "+resource.Connection.State, null, MessageType.Error);
                resource?.Close();
            },
            OnResetState = (resource) => { Log("Reset -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info);
                resource?.Close();
            },
            OnCreateResource = (resource) => { Log("Create -> "+ resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning); },
            OnEvictResource = (resource) => { Log("Evict -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Error);
                resource?.Close();
            },
            OnGetResource = (resource) => { Log("Get -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Info); },
            OnReturnResource = (resource) => { Log("Return -> " + resource.GetHashCode() + " , connection: " + resource.Connection.GetHashCode() + ", status : " + resource.Connection.State, null, MessageType.Warning); },


        }, Environment.ProcessorCount * 2
            , new Pooling.EvictionSettings() { Enabled = true, Period =TimeSpan.FromMilliseconds(1000)}
            );
        public static void TestGetAndReturn() {

            var db = pooldb.GetPooledDatabase().InternalResource;
            Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
            pooldb.ReturnPooledDatabase(db);
            Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);

            db = pooldb.GetPooledDatabase().InternalResource;
            Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
            pooldb.ReturnPooledDatabase(db);
            Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);

            //for (var a = 0; a < 10; a++)
            //{
            //    new Thread(() =>
            //    {

            //        for (var b = 0; b < 10; b++)
            //        {
            //              db = pooldb.GetCurrentDatabase();
            //            Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
            //            pooldb.ReturnDatabase(db);
            //            Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);
            //        }

            //        Log("filish thread:" + a.ToString(), null);


            //    }).Start();
            //}



        }

        private static void ExeTExt()
        {
            //var db = DbMocker.NewDataBase();
            //var o = db.Get<UserInfo>(1);
            //o.DTCreate = DateTime.Now;
            //db.Update<UserInfo>(o);

            //using (var db = pooldb.GetCurrentDatabase())
            //{
            //    //Thread.Sleep(100);

            //}


            using (var pdb = pooldb.GetPooledDatabase())
            {
                var oo = pdb.InternalResource.FirstOrDefault<UserInfo>(p=>p.Id == 9);
                 
            }
            //var db = pooldb.GetPooledDatabase().InternalResource;
            //Log("GetCurrentDatabase:" + db.GetHashCode().ToString(), null);
            ////Thread.Sleep(100);
            //pooldb.ReturnPooledDatabase(db);
            ////pooldb.Close();
            //Log("ReturnDatabase:" + db.GetHashCode().ToString(), null);


        }
        public static void TestDbContext()
        {
            // ExeTExt();
            //Thread.Sleep(1000);
            Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 50 },
                  () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();
                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  }, () =>
                  {
                      ExeTExt();

                  } 
                  /////////////////////////////////////
                  //() =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}, () =>
                  //{
                  //    ExeTExt();

                  //}
              );

            

        }


    }







}
