using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
namespace Pure.Data
{
    public static class AttributeHelper
    {
        private static Dictionary<string, object> AttributePools = new Dictionary<string, object>();
        /// <summary>获取自定义类特性</summary>
        /// <returns></returns>
         public static IList<TAttribute> GetCustomClassAttributes<TAttribute>(this Type t, Boolean inherit = true) where TAttribute : Attribute
        {
            if (t == null) return null;
            string key = t.FullName +"_"+typeof(TAttribute).Name;
            if (AttributePools.Keys.Contains(key))
            {
                return AttributePools[key] as IList<TAttribute>;
            }
            //取类上的自定义特性
            object[] objs = t.GetCustomAttributes(typeof(TAttribute), inherit);
             IList<TAttribute> result = new List<TAttribute>();
            foreach (object obj in objs)
            {
                TAttribute attr = obj as TAttribute;
                if (attr != null)
                {
                    result.Add(attr);
                }
            }
            AttributePools.Add(key, result);//cache pool
            return result;
        }

        /// <summary>获取自定义属性和特性</summary>
         public static Dictionary<PropertyInfo, IList<TAttribute>> GetCustomPropertyAttributes<TAttribute>(this Type t, Boolean inherit = true) where TAttribute : Attribute
         {
             if (t == null) return null;
             string key =t.FullName + "_" + typeof(TAttribute).Name + "_p";
             if (AttributePools.Keys.Contains(key))
             {
                 return AttributePools[key] as Dictionary<PropertyInfo, IList<TAttribute>>;
             }

             Dictionary<PropertyInfo, IList<TAttribute>> dic = new Dictionary<PropertyInfo, IList<TAttribute>>();
             //取属性上的自定义特性
             foreach (PropertyInfo propInfo in t.GetProperties())
             {
                 object[] objAttrs = propInfo.GetCustomAttributes(typeof(TAttribute), inherit);
                 IList<TAttribute> result = new List<TAttribute>();
                 foreach (object obj in objAttrs)
                 {
                     TAttribute attr = obj as TAttribute;
                     if (attr != null)
                     {
                         result.Add(attr);
                     }
                 }
                 //if (result.Count > 0)
                 //{
                 //    dic.Add(propInfo, result);
                 //}
                 dic.Add(propInfo, result);
             }
             AttributePools.Add(key, dic);//cache pool
             return dic;
         }
     }
}
