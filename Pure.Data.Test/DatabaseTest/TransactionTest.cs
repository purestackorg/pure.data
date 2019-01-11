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
    public class TransactionTest
    {

        public static void Test()
        {

            
            string title = "TransactionTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                using (var db = DbMocker.NewDataBase())
                {
                    var count = db.ExecuteScalar<long>("SELECT COUNT(*) FROM TB_USER");

                    PrintHelper.WriteLine("count=" + count);

                }

                //NestedTransactionThatFailsAbortsWholeUsingBeginAbort();
                //NestedTransactionsBothCompleteUsingBeginAbort();
            });


            Console.Read();
            
            
        }


        public static void NestedTransactionThatFailsAbortsWholeUsingBeginAbort()
        {
            var db = DbMocker.NewDataBase();

            db.BeginTransaction();
            db.BeginTransaction();


            var user1 = new UserInfo
            {
                Name = "Name" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1985, 1, 1)
            };
            db.Insert(user1);


            var extra1 = new UserInfo
            {
                Id = user1.Id,
                Email = "email" + 16 + "@email.com",
                Name = "Name" + 17

            };
            db.Insert(extra1);


            db.RollbackTransaction();

            var user3 = new UserInfo
            {
                Name = "Name" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1970, 1, 1).AddYears(16)
            };
            db.Insert(user3);



            db.CommitTransaction();

            var count = db.ExecuteScalar<long>("SELECT COUNT(*) FROM TB_USER");
        }


        public static void NestedTransactionsBothCompleteUsingBeginAbort()
        {
            var db = DbMocker.NewDataBase();

            db.BeginTransaction();

            var user = new UserInfo
            {
                Name = "Name" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1970, 1, 1).AddYears(16)
            };
            db.Insert(user);

            db.BeginTransaction();

            var user1 = new UserInfo
            {
                Name = "Name" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1970, 1, 1).AddYears(16)
            };
            db.Insert(user1);

            var extra1 = new UserInfo
            {
                Name = "Name" + 16,
                Age = 20 + 16,
                DTCreate = new DateTime(1970, 1, 1).AddYears(16)
            };
            db.Insert(extra1);


            db.CommitTransaction(); 
            
            db.CommitTransaction();

            PrintHelper.WriteLine("Finish");
        }

        
    }
}
