

using System;
using System.Data.Common;
using System.Reflection;
namespace Pure.Data.Migration.Providers.Db2
{
    public class Db2DbFactory : ReflectionBasedDbFactory
    {
        #region Constructors

        public Db2DbFactory()
            : base("IBM.Data.DB2.iSeries", "IBM.Data.DB2.iSeries.iDB2Factory")
        {
        }

        #endregion Constructors

        #region Methods

        protected override DbProviderFactory CreateFactory()
        {
            string path = GetReflectedFile("IBM.Data.DB2.dll");
            var assembly = Assembly.LoadFile(path);
            //var assembly = AppDomain.CurrentDomain.Load("IBM.Data.DB2.iSeries, Version=12.0.0.0, Culture=neutral, PublicKeyToken=9cdb2ebfb1f93a26");
            var type = assembly.GetType("IBM.Data.DB2.iSeries.iDB2Factory");
            var field = type.GetField("Instance", BindingFlags.Static | BindingFlags.Public);

            if (field == null)
            {
                return base.CreateFactory();
            }

            return (DbProviderFactory)field.GetValue(null);
        }

        #endregion Methods
    }

    
}