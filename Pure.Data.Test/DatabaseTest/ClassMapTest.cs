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
    public class ClassMapTest
    {

        public static void Test()
        {


            string title = "ClassMapTest";
            Console.Title = title;
            var db = DbMocker.InstanceDataBase();
            db.Config.EnableDebug = false;
            db.Config.EnableOrmCache = false;
            db.Config.EnableOrmLog = false;
            db.Config.EnableIntercept = false;

            CodeTimer.Time(title, 1, () => {

                GetAttributeMap(db);
                //LoadAll(db);
            });


            Console.Read();
            
            
        }



        public static void LoadAll(IDatabase db)
        {
           
            //db.LoadAllMap(null);

            //var query = db.FluentSqlBuilder.Select<UserInfo>(p => new { p.Id, p.Name }).Where(p => p.Age > 50).OrderBy(p => p.Age).EndOrder().GetPage(1, 5);
            var query = db.FluentSqlBuilder.Select<UserInfo>(p => new { p.Id, p.Name }).Where(p => p.Age > 50).OrderBy(p => p.Age).TakePage(1, 5);
            //var reader = query.ExecuteReader();
            db.ExecuteReader<UserInfo>(query);
            //var l = reader.ToList<UserInfo>();
        }
        
        public static void GetAttributeMap(IDatabase db)
        {

           var map = db.GetMap<Account>();
        }
        
    }
}
