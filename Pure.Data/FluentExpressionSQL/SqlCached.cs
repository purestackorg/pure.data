using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentExpressionSQL
{
    internal class SqlCacheItem
    {
        public string Sql{get;set;}
        public string RawSql{get;set;}
        public Dictionary<string, object> DbParams{get;set;}
        
    }

    internal class SqlCached
    {
        private static readonly ConcurrentDictionary<int, SqlCacheItem> _SQLCache = new ConcurrentDictionary<int, SqlCacheItem>();

        public static void SetSQLCache(int key, SqlCacheItem v)
        {
            _SQLCache[key] = v;
        }

        public static SqlCacheItem GetSQLCache(int key)
        {
            SqlCacheItem v;
            _SQLCache.TryGetValue(key, out v);
            return v;
        }
    }
}
