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
    public class ConnectionTestIntercept : IConnectionInterceptor
    {

        public System.Data.IDbConnection OnConnectionOpened(IDatabase database, System.Data.IDbConnection conn)
        {
            string str = string.Format(database.DatabaseName + " was opened!" + conn.GetHashCode());
            database.LogHelper.Warning(str);
            return conn;
        }

        public void OnConnectionClosing(IDatabase database, System.Data.IDbConnection conn)
        {
            string str = string.Format(database.DatabaseName + " was closing!" + conn.GetHashCode());
            database.LogHelper.Warning(str);
        }
    }

    public class TransactionTestIntercept : ITransactionInterceptor
    {


        public void OnBeginTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnBegin!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnAbortTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnAbort!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }

        public void OnCompleteTransaction(IDatabase database)
        {
            string str = string.Format(database.DatabaseName + " was OnComplete!" + database.Connection.GetHashCode());
            database.LogHelper.Warning(str);
        }
    }

    public class InterceptTest
    {

        public static void Test()
        {


            string title = "CRUDTest";
            Console.Title = title;
            var db = DbMocker.InstanceDataBase();
            db.Config.EnableOrmLog = false;
            db.Config.EnableDebug = true;
            db.Config.Interceptors.Add(new ConnectionTestIntercept());
            PrintHelper.WriteLine("Add ConnectionTestIntercept");
            db.LogHelper.Write("Add ConnectionTestIntercept");
            CodeTimer.Time(title, 100, () => {

                //Insert();
                Update(db);
            });


            Console.Read();
            
            
        }


        public static void Update(IDatabase db)
        {
            var user1 = new UserInfo
            {
                Name = "NameTest" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1985, 1, 1)
            };

            db.Update<UserInfo>(() => new UserInfo { Age = 88, Name = "李玮峰" }, p => p.Id == 5);

        }
       
        
    }
}
