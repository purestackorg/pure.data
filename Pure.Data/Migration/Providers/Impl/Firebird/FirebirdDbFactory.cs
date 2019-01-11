

using System;
using System.Data.Common;
using System.Reflection;
namespace Pure.Data.Migration.Providers.Firebird
{
    public class FirebirdDbFactory : ReflectionBasedDbFactory
    {
        public FirebirdDbFactory()
            : base("FirebirdSql.Data.FirebirdClient", "FirebirdSql.Data.FirebirdClient.FirebirdClientFactory")
        {
        }

        protected override DbProviderFactory CreateFactory()
        {
            string path = GetReflectedFile("FirebirdSql.Data.dll");
            var assembly = Assembly.LoadFile(path);
            //var assembly = AppDomain.CurrentDomain.Load("FirebirdSql.Data.FirebirdClient");
            var type = assembly.GetType("FirebirdSql.Data.FirebirdClient.FirebirdClientFactory");
            var field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

            if (field == null)
            {
                return base.CreateFactory();
            }

            return (DbProviderFactory)field.GetValue(null);
        }

    }
     
}