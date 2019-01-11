using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Dynamic;
using System.Linq;
namespace Pure.Data
{
    internal static class ExpandoExts
    {
        /// <summary>
        /// 转换为动态类型
        /// </summary>
        /// <param name="o">The object to convert.</param>
        /// <returns>a new expando object with the values of the passed in object</returns>
        public static dynamic ToExpandoByObject(this object o)
        {
            if (o is ExpandoObject)
            {
                return o;
            }
            var result = new ExpandoObject();
            var d = (IDictionary<string, object>)result; //work with the Expando as a Dictionary
            if (o.GetType() == typeof(NameValueCollection) || o.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nv = (NameValueCollection)o;
                nv.Cast<string>().Select(key => new KeyValuePair<string, object>(key, nv[key])).ToList().ForEach(i => d.Add(i));
            }
            else
            {
                var props = o.GetType().GetProperties();
                foreach (var item in props)
                {
                    d.Add(item.Name, item.GetValue(o, null));
                }
            }
            return result;
        }


        /// <summary>
        /// 转换为IDictionary
        /// </summary>
        /// <param name="thingy">The object to convert to a dictionary.</param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionaryByObject(this object thingy)
        {
            return (IDictionary<string, object>)thingy.ToExpandoByObject();
        }


        /// <summary>
        /// 转换为DataTable
        /// </summary>
        /// <param name="items"></param>
        /// <returns>A DataTable with the copied dynamic data.</returns>
        /// <remarks>Credit given to Brian Vallelunga http://stackoverflow.com/a/6298704/5262210 </remarks>
        public static DataTable ToDataTable(this IEnumerable<dynamic> items)
        {
            var data = items.ToArray();
            var toReturn = new DataTable();
            if (!data.Any())
            {
                return toReturn;
            }
            foreach (var kvp in (IDictionary<string, object>)data[0])
            {
                // for now we'll fall back to string if the value is null, as we don't know any type information on null values.
                var type = kvp.Value == null ? typeof(string) : kvp.Value.GetType();
                toReturn.Columns.Add(kvp.Key, type);
            }
            return data.ToDataTable(toReturn);
        }


        /// <summary>
        /// 转换为DataTable
        /// </summary>
        /// <param name="items">The items to convert to data rows.</param>
        /// <param name="toFill">The datatable to fill. It's required this datatable has the proper columns setup.</param>
        /// <returns>
        /// toFill with the data from items.
        /// </returns>
        /// <remarks>
        /// Credit given to Brian Vallelunga http://stackoverflow.com/a/6298704/5262210
        /// </remarks>
        public static DataTable ToDataTable(this IEnumerable<dynamic> items, DataTable toFill)
        {
            dynamic[] data = items is dynamic[] ? (dynamic[])items : items.ToArray();
            if (toFill == null || toFill.Columns.Count <= 0)
            {
                return toFill;
            }
            foreach (var d in data)
            {
                toFill.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return toFill;
        }


    }
}
