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
    public class ValidationTest
    {

        public static void Test()
        {


            string title = "ValidationTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {
                 
                Convert();
                

            });


            Console.Read();
            
            
        }
        public static void Convert()
        {

            var db = DbMocker.NewDataBase();
            Expression2SqlTest.UserInfoMapper mapper = new Expression2SqlTest.UserInfoMapper();
            UserInfo v = new UserInfo();
            v.Name = "http://www.sdfd.com";
            v.Age = 1;
            v.Email = "324234234234@qq.com";
            v.Name = "<script>alert(3242)</script>";

            var result = mapper.Validate(db, v, "Age");


            var result2 = db.Validate(v, m => m.Name, m => m.Age);
            var result3 = mapper.Validate(db, v );

            Console.WriteLine(result.ToString());
            Console.WriteLine(result2.ToString());
            Console.WriteLine(result3.ToString());

        }
      
    }
}
