using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data
{
    /// <summary>是否不缓存对象实体属性。</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class NoCacheAttribute : Attribute
    {

    }
   
}
