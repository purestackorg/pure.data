using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.IO;
using System.Text;

namespace Pure.Data
{
#if NETSTANDARD2_0
      public static class DbProviderFactories
    {

        private static readonly ConcurrentDictionary<string, DbProviderFactory> DbProviderFactoryPool = new ConcurrentDictionary<string, DbProviderFactory>();
        public static void AddDbProviderFactories(string providerInvariantName,  DbProviderFactory createDbProviderFactory)
        {
            DbProviderFactoryPool[providerInvariantName] = createDbProviderFactory;
        }
       
            public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (string.IsNullOrEmpty(providerInvariantName)) throw new Exception("不存在指定名称"+ providerInvariantName + "的DbProviderFactory！");
            DbProviderFactory fac = null;
            if (DbProviderFactoryPool.TryGetValue(providerInvariantName, out fac))
            {
                return fac;
            }

            throw new Exception("不存在指定名称" + providerInvariantName + "的DbProviderFactory！");

        }
    }
#endif

}
