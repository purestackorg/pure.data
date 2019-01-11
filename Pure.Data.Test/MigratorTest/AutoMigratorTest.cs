using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using System.Data; 
using System.IO; 

namespace Pure.Data.Test
{
    public class AutoMigratorTest
    {

        public static void Test()
        {


            string title = "AutoMigratorTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () =>
            {
                //Migrate();
                Migrate2();
            });


            Console.Read();


        } 

        public static void Migrate2()
        {

            var db = DbMocker.InstanceDataBase();
            
        }
   
    }
}
