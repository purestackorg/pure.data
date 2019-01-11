

using System;
using System.Reflection;

namespace Pure.Data.Migration.Providers.SQLite
{
    public class SQLiteDbFactory : ReflectionBasedDbFactory
    {
#if NET45
           public SQLiteDbFactory()
            : base("System.Data.SQLite", "System.Data.SQLite.SQLiteFactory")
        {
        }
#else
        public SQLiteDbFactory()
         : base("Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteFactory.Instance")
        {
        }

        protected override System.Data.Common.DbProviderFactory CreateFactory()
        {
            var assembly = AppDomain.CurrentDomain.Load("Microsoft.Data.Sqlite");
            var type = assembly.GetType("Microsoft.Data.Sqlite.SqliteFactory");
            var field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

            if (field == null)
            {
                return base.CreateFactory();
            }

            return (System.Data.Common.DbProviderFactory)field.GetValue(null);
        }
#endif

    }
}