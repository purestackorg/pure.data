using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Pure.Data.Test
{


    public class SqlCustomGeneratorTest
    {

       
        public static void Test()
        {


            string title = "SqlCustomGeneratorTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                //Insert(); 
                InsertExecute();
            });


            Console.Read();
            
            
        }

        public static void Insert()
        {
            var db = DbMocker.NewDataBase();

            IDictionary<string, object> pars = new Dictionary<string, object>();
            IDictionary<string, object> RealParameters = new Dictionary<string, object>();
            pars.Add("Id",2);
            pars.Add("Name", "dd3");
            pars.Add("Sex", null);
            pars.Add("DTCreate",DateTime.Now);

            IDictionary<string, object> conditions = new Dictionary<string, object>();
            conditions.Add("Id", 2);
            conditions.Add("Name", "552");
            conditions.Add("Sex", true);

            string sql = db.SqlGenerator.SqlCustomGenerator.Insert("TB_USER", pars, out RealParameters);

           //  sql = db.SqlGenerator.SqlCustomGenerator.Update("TB_USER", pars,conditions, out RealParameters);

           //  sql = db.SqlGenerator.SqlCustomGenerator.Delete("TB_USER", conditions, out RealParameters);

            string[] selectCols = { "Id","Name as dd"};
            var sorts =Predicates.SortGroup();
            sorts.Add(Predicates.Sort("DTCreate",true));
            sorts.Add(Predicates.Sort("Id",false));
            sql = db.SqlGenerator.SqlCustomGenerator.Select("TB_USER", selectCols, conditions, sorts, out RealParameters);


            sql = db.SqlGenerator.SqlCustomGenerator.Count("TB_USER",   conditions,  out RealParameters);


            Console.WriteLine(sql);

            foreach (var item in RealParameters)
            {
                Console.WriteLine(item.Key + "=" + item.Value);

            }
             
              
        }

        public static void InsertExecute()
        {
            var db = DbMocker.NewDataBase();

            IDictionary<string, object> pars = new Dictionary<string, object>();
            IDictionary<string, object> RealParameters = new Dictionary<string, object>();
            //pars.Add("Id", 2);
            pars.Add("Name", "dd3");
            pars.Add("Sex", null);
            pars.Add("DTCreate", DateTime.Now);

            IDictionary<string, object> conditions = new Dictionary<string, object>();
            conditions.Add("Id", 2);
            conditions.Add("Name", "552");
            conditions.Add("Sex", true);

             int exeCount =  db.Insert("TB_USER", pars);

             exeCount = db.Update("TB_USER", pars, conditions);

             exeCount = db.Delete("TB_USER", conditions);

            string[] selectCols = { "Id", "Name as dd" };
            var sorts = Predicates.SortGroup();
            sorts.Add(Predicates.Sort("DTCreate", true));
            sorts.Add(Predicates.Sort("Id", false));
           // var data = db.Query("TB_USER", selectCols, conditions, sorts).ToDictionary<string, object>();


            exeCount = db.Count("TB_USER", conditions);


            Console.WriteLine(exeCount);

            foreach (var item in RealParameters)
            {
                Console.WriteLine(item.Key + "=" + item.Value);

            }


        }
 

    }
}
