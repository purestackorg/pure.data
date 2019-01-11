using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap
{
    public interface ICacheProvider
    {
        void Initialize(IDictionary properties);
        object this[CacheKey key]
        {
            get;
            set;
        }
        bool Remove(CacheKey key);
        void Flush();
    }
}
