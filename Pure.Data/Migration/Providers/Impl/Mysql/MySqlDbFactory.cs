

using System;
using System.Data.Common;
using System.Reflection;
namespace Pure.Data.Migration.Providers.Mysql
{
    public class MySqlDbFactory : ReflectionBasedDbFactory
    {
        public MySqlDbFactory()
            : base("MySql.Data", "MySql.Data.MySqlClient.MySqlClientFactory")
        {
        }

        protected override DbProviderFactory CreateFactory()
        {
            //string path = GetReflectedFile("MySql.Data.dll");
            //var assembly = Assembly.LoadFile(path);
            

            var assembly = AppDomain.CurrentDomain.Load("MySql.Data");
            var type = assembly.GetType("MySql.Data.MySqlClient.MySqlClientFactory");
            var field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

            if (field == null)
            {
                return base.CreateFactory();
            }

            return (DbProviderFactory)field.GetValue(null);
        }
    }
}