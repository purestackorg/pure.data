using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public static class StaticMethodHelper
    {
        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T To<T>(this object val)
        {
            T defaulVal = default(T);
            if (val != null)
            {
                Type t = typeof(T);
                try
                {
                    T obj = (T)Convert.ChangeType(val, t);
                    return obj;
                }
                catch (Exception)
                {

                }

            }
            return defaulVal;
        }
    }
}
