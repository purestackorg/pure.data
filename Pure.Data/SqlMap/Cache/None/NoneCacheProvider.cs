 
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Pure.Data.SqlMap
{
    public class NoneCacheProvider : ICacheProvider
    {
        public object this[CacheKey key]
        {
            get { return null; }
            set { }
        }

        public void Flush()
        {
           
        }

        public void Initialize(IDictionary properties)
        {
            
        }

        public bool Remove(CacheKey key)
        {
            return true;
        }
    }
}
