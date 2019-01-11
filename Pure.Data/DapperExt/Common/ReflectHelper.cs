using System;
using System.Collections.Generic;
using System.Reflection;
namespace Pure.Data
{
    /// <summary>反射工具类</summary>
    public static class ReflectHelper
    {
        /// <summary>获取字段。搜索私有、静态、基类，优先返回大小写精确匹配成员</summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static FieldInfo GetFieldEx(this Type type, String name, Boolean ignoreCase = false)
        {
            if (String.IsNullOrEmpty(name)) return null;

            return GetField(type, name, ignoreCase);
        }
        static BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        static BindingFlags bfic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase;

        /// <summary>获取字段</summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns></returns>
        public static FieldInfo GetField(Type type, String name, Boolean ignoreCase)
        {
            // 父类私有字段的获取需要递归，可见范围则不需要，有些类型的父类为空，比如接口
            while (type != null && type != typeof(Object))
            {
                //var fi = type.GetField(name, ignoreCase ? bfic : bf);
                var fi = type.GetField(name, bf);
                if (fi != null) return fi;
                if (ignoreCase)
                {
                    fi = type.GetField(name, bfic);
                    if (fi != null) return fi;
                }

                type = type.BaseType;
            }
            return null;
        }
        public static Object GetValue(this Object target, MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                var propertyInfo = member as PropertyInfo;
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(target, null);
                return value;
            }
            else if (member is FieldInfo)
            {
                var propertyInfo = member as FieldInfo;
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(target);
                return value;
            }
            else
                throw new ArgumentOutOfRangeException("member");
        }

        public static Object GetValue(this Object obj, String propertyName)
        {
            if (obj != null)
            {
                Type t = obj.GetType();
                var p = t.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (p != null)
                {
                    return p.GetValue(obj);
                }
                else if (obj is IDictionary<string, object>)
                {

                    var o = obj as IDictionary<string, object>;
                    if (o != null)
                    {
                        if (o.ContainsKey(propertyName))
                        {
                            return o[propertyName];
                        }
                    }

                }
                else if (obj is IEnumerable<KeyValuePair<string, object>>)
                {

                    var o = obj as IEnumerable<KeyValuePair<string, object>>;
                    if (o != null)
                    {
                        foreach (var item in o)
                        {
                            if (item.Key == propertyName)
                            {
                                return item.Value;
                            }
                        }

                    }

                }
            }
            return null;// obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        }

    }
}
