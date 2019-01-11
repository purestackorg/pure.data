
namespace Pure.Data.Migration.Providers
{
    using System;
    using System.Data.Common;
    using System.Reflection;

    public class ReflectionBasedDbFactory : DbFactoryBase
    {
        private readonly string assemblyName;
        private readonly string dbProviderFactoryTypeName;

        public string BasePath = PathHelper.GetAppExecuteDirectory() ;
        public string GetReflectedFile(string file)
        {
            return System.IO.Path.Combine(BasePath, file);
        }
        public ReflectionBasedDbFactory(string assemblyName, string dbProviderFactoryTypeName)
        {
            this.assemblyName = assemblyName;
            this.dbProviderFactoryTypeName = dbProviderFactoryTypeName;
        }

        protected override DbProviderFactory CreateFactory()
        {

#if NET45
            return (DbProviderFactory)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, dbProviderFactoryTypeName);
#else
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            Type type = assembly.GetType(dbProviderFactoryTypeName);
            var instance = (DbProviderFactory)Activator.CreateInstance(type);
            return instance;
#endif 
        }
    }
}
