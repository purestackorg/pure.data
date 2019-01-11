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

    public class ConfigurationTest
    {

       
        public static void Test()
        {


            string title = "ConfigurationTest";
            Console.Title = title;

            CodeTimer.Time(title, 5, () => {

                Test22(); 
            });


            Console.Read();
            
            
        }

        public static void Test22()
        {
            DatabaseConfig conf = new DatabaseConfig();
            var ccconfig = PureDataConfigurationLoader.Instance.Load("PureDataConfiguration.xml",conf,   (ss, ex ,t) =>
            {
                Console.WriteLine(ss);
            });

            Console.WriteLine(ccconfig.DatabaseConfig.ToString());
        }
 

    }
}
