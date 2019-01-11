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
    public class ValidationsTest
    {

        public static void Test()
        {


            string title = "CRUDTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {


                Valid(); 

            });


            Console.Read();
            
            
        }
      
        public static void Valid()
        {
             
            //var db = DbMocker.NewDataBase();

            //UserInfoValid o = new UserInfoValid();

            //UserInfoValidMapper map = new UserInfoValidMapper();
            //var result = map.Validate(o);
            

        }
        

        
    }
}
