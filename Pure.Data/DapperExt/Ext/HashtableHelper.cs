using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace Pure.Data
{
    /// <summary>
    /// Hashtable帮助类
    /// </summary>
    public static class HashtableHelper
    {
        public static string HashtableToXml(this Hashtable ht)
        {
            StringBuilder xml = new StringBuilder("<root>");
            xml.Append(HashtableToNode(ht));
            xml.Append("</root>");
            return xml.ToString();
        }

        private static string HashtableToNode(this Hashtable ht)
        {
            StringBuilder xml = new StringBuilder("");
            foreach (string key in ht.Keys)
            {
                object value = ht[key];
                xml.Append("<").Append(key).Append(">").Append(value).Append("</").Append(key).Append(">");
            }
            xml.Append("");
            return xml.ToString();
        }

        public static string IListToXML(this IList<Hashtable> datas)
        {
            StringBuilder xml = new StringBuilder("<root>");
            foreach (Hashtable ht in datas)
            {
                xml.Append(HashtableToNode(ht));
            }
            xml.Append("</root>");
            return xml.ToString();
        }

        /// <summary>
        /// 实体类Model转Hashtable(反射)
        /// </summary>
        public static Hashtable GetModelToHashtable<T>(this T model)
        {
            Hashtable ht = new Hashtable();
            PropertyInfo[] properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo item in properties)
            {
                string key = item.Name;
                ht[key] = item.GetValue(model, null);
            }
            return ht;
        }
    }
}