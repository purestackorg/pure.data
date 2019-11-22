using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Pure.Data
{
    /// <summary>
    /// DataReader Extensions
    /// </summary>
    public static class DataReaderExtensions
    {
        public static string FormatColumnName(this string str, IDatabase database)
        {
            if (database!= null && database.Config !=null )
            {
                return NameConvertHelper.ConvertNameCase(database.Config.CodeGenClassNameMode, str);
            }
            return str;//.ToUpper()
        }

        /// <summary>
        ///  将IDataReader转换为DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this IDataReader reader, bool autoClose = true)
        {
            try
            {
                if (autoClose)
                {

                    DataTable dt = new DataTable("Table");
                    while (!reader.IsClosed)
                    {
                        dt.Load(reader);
                    }

                    return dt;


                }
                else
                {

                    DataTable dt = new DataTable("Table");
                    while (!reader.IsClosed)
                    {
                        dt.Load(reader);
                    }

                    return dt;


                }

            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }


        }

        /// <summary>
        ///  将IDataReader转换为DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable ToDataTableWithRowDelegate(this IDataReader reader, bool autoClose = true, IDatabase database = null)
        {
            try
            {
                if (autoClose)
                {
                    DataTable objDataTable = new DataTable("Table");
                    using (reader)
                    {
                        int intFieldCount = reader.FieldCount;
                        for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                        {
                            objDataTable.Columns.Add(reader.GetName(intCounter).FormatColumnName(database), reader.GetFieldType(intCounter));
                        }
                        objDataTable.BeginLoadData();
                        object[] objValues = new object[intFieldCount];
                        while (reader.Read())
                        {
                            reader.GetValues(objValues);
                            objDataTable.LoadDataRow(objValues, true);
                        }
                        //reader.Close();
                        objDataTable.EndLoadData();
                    }

                    return objDataTable;
                }
                else
                {
                    DataTable objDataTable = new DataTable("Table");

                    int intFieldCount = reader.FieldCount;
                    for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                    {
                        objDataTable.Columns.Add(reader.GetName(intCounter).FormatColumnName(database), reader.GetFieldType(intCounter));
                    }
                    objDataTable.BeginLoadData();
                    object[] objValues = new object[intFieldCount];
                    while (reader.Read())
                    {
                        reader.GetValues(objValues);
                        objDataTable.LoadDataRow(objValues, true);
                    }
                    //reader.Close();
                    objDataTable.EndLoadData();

                    return objDataTable;
                }

            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }


        }
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader, bool autoClose = true)
        {
            try
            {
                if (autoClose)
                {
                    var newDict = new Dictionary<TKey, TValue>();
                    if (reader != null)
                    {
                        using (reader)
                        {
                            if (reader.FieldCount >= 2)
                            {
                                object key, value;
                                while (reader.Read())
                                {

                                    key = reader.GetValue(0);
                                    value = reader.GetValue(1);
                                    var keyConverted = (TKey)HackType(key, typeof(TKey));

                                    var valueType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

                                    var valConverted = value != null ? (TValue)HackType(value, valueType) : default(TValue);

                                    if (keyConverted != null)
                                    {
                                        newDict.Add(keyConverted, valConverted);
                                    }

                                }
                            }
                        }
                    }

                    return newDict;
                }
                else
                {
                    var newDict = new Dictionary<TKey, TValue>();
                    if (reader != null)
                    {

                        if (reader.FieldCount >= 2)
                        {
                            object key, value;
                            while (reader.Read())
                            {

                                key = reader.GetValue(0);
                                value = reader.GetValue(1);
                                var keyConverted = (TKey)HackType(key, typeof(TKey));

                                var valueType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

                                var valConverted = value != null ? (TValue)HackType(value, valueType) : default(TValue);

                                if (keyConverted != null)
                                {
                                    newDict.Add(keyConverted, valConverted);
                                }

                            }
                        }

                    }

                    return newDict;
                }

            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }

        }
        public static T ToModel<T>(this IDataReader dr, bool autoClose = true, IDatabase database = null)
        {
            try
            {

                var type = typeof(T);
                if (type == typeof(object) || type == typeof(Object) || type == typeof(ExpandoObject))
                {
                    return ToExpandoObject(dr, autoClose, database);
                }

                if (autoClose)
                {
                    using (dr)
                    {
                        if (dr.Read())
                        {
                            List<string> list = new List<string>(dr.FieldCount);
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                list.Add(dr.GetName(i).ToLower());
                            }
                            T model = Activator.CreateInstance<T>();
                            foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                            {
                                if (list.Contains(pi.Name.ToLower()))
                                {
                                    if (!IsNullOrDBNull(dr[pi.Name]))
                                    {
                                        pi.SetValue(model, HackType(dr[pi.Name], pi.PropertyType), null);
                                    }
                                }
                            }
                            return model;
                        }

                        dr.Close();
                    }
                }
                else
                {
                    if (dr.Read())
                    {
                        List<string> list = new List<string>(dr.FieldCount);
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            list.Add(dr.GetName(i).ToLower());
                        }
                        T model = Activator.CreateInstance<T>();
                        foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (list.Contains(pi.Name.ToLower()))
                            {
                                if (!IsNullOrDBNull(dr[pi.Name]))
                                {
                                    pi.SetValue(model, HackType(dr[pi.Name], pi.PropertyType), null);
                                }
                            }
                        }
                        return model;
                    }
                }


                return default(T);
            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }
        }

        public static T ToModelByEmit<T>(this IDataReader dr, bool autoClose = true)
        {
            try
            {
                if (autoClose)
                {
                    using (dr)
                    {

                        var result = EmitConverter.ReaderToModel<T>(dr);

                        return result;
                    }
                }
                else
                {
                    var result = EmitConverter.ReaderToModel<T>(dr);

                    return result;
                }


                return default(T);
            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }
        }




        public static List<T> ToList<T>(this IDataReader dr, bool autoClose = true, IDatabase database = null)
        {
            try
            {
                var type = typeof(T);
                if (type == typeof(object) || type == typeof(Object) || type == typeof(ExpandoObject))
                {
                    return ToExpandoObjects(dr, autoClose, database) as List<T>;
                }


                if (autoClose)
                {
                    using (dr)
                    {
                        List<string> field = new List<string>(dr.FieldCount);
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            field.Add(dr.GetName(i).ToLower());
                        }
                        List<T> list = new List<T>();
                        while (dr.Read())
                        {
                            T model = Activator.CreateInstance<T>();
                            foreach (PropertyInfo property in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                            {
                                if (field.Contains(property.Name.ToLower()))
                                {
                                    if (!IsNullOrDBNull(dr[property.Name]))
                                    {
                                        property.SetValue(model, HackType(dr[property.Name], property.PropertyType), null);
                                    }
                                }
                            }
                            list.Add(model);
                        }
                        return list;


                    }
                }
                else
                {
                    List<string> field = new List<string>(dr.FieldCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        field.Add(dr.GetName(i).ToLower());
                    }
                    List<T> list = new List<T>();
                    while (dr.Read())
                    {
                        T model = Activator.CreateInstance<T>();
                        foreach (PropertyInfo property in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (field.Contains(property.Name.ToLower()))
                            {
                                if (!IsNullOrDBNull(dr[property.Name]))
                                {
                                    property.SetValue(model, HackType(dr[property.Name], property.PropertyType), null);
                                }
                            }
                        }
                        list.Add(model);
                    }
                    return list;

                }

            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }

        }

        public static List<T> ToListByEmit<T>(this IDataReader dr, bool autoClose = true)
        {
            try
            {
                if (autoClose)
                {
                    using (dr)
                    {

                        List<T> list = EmitConverter.ReaderToEnumerable<T>(dr).ToList();

                        return list;
                    }
                }
                else
                {
                    List<T> list = EmitConverter.ReaderToEnumerable<T>(dr).ToList();

                    return list;

                }

            }
            catch (Exception ex)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }

        }

        public static IDictionary<string, object> ToDictionary(this IDataReader @this, bool autoClose = true, IDatabase database = null)
        {
            IDictionary<string, object> expandoDict = new Dictionary<string, object>();
            try
            {
                if (autoClose)
                {
                    using (@this)
                    {
                        Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                        .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                        .ToDictionary(pair => pair.Key);

                        
                        while (@this.Read())
                        {

                            Enumerable.Range(0, @this.FieldCount)
                               .ToList()
                               .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                            break;
                        }

                    }

                    return expandoDict;
                }
                else
                {

                    Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                    .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                    .ToDictionary(pair => pair.Key);
                     

                    while (@this.Read())
                    {

                        Enumerable.Range(0, @this.FieldCount)
                           .ToList()
                           .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                        break;
                    }


                    return expandoDict;
                }

            }
            catch (Exception ex)
            {
                if (@this != null)
                {
                    @this.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }

        }
        /// <summary>
        ///     An IDataReader extension method that converts the @this to an expando object.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a dynamic.</returns>
        public static dynamic ToExpandoObject(this IDataReader @this, bool autoClose = true, IDatabase database = null)
        {
            dynamic entity = new ExpandoObject();
            try
            {
                if (autoClose)
                {
                    using (@this)
                    {
                        Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                        .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                        .ToDictionary(pair => pair.Key);

                        var expandoDict = (IDictionary<string, object>)entity;

                        while (@this.Read())
                        {

                            Enumerable.Range(0, @this.FieldCount)
                               .ToList()
                               .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                            break;
                        }

                    }

                    return entity;
                }
                else
                {

                    Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                    .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                    .ToDictionary(pair => pair.Key);

                    var expandoDict = (IDictionary<string, object>)entity;

                    while (@this.Read())
                    {

                        Enumerable.Range(0, @this.FieldCount)
                           .ToList()
                           .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                        break;
                    }


                    return entity;
                }

            }
            catch (Exception ex)
            {
                if (@this != null)
                {
                    @this.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }

        }
        /// <summary>
        ///     Enumerates to expando objects in this collection.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as an IEnumerable&lt;dynamic&gt;</returns>
        public static IEnumerable<dynamic> ToExpandoObjects(this IDataReader @this, bool autoClose = true, IDatabase database = null)
        {
            try
            {
                if (autoClose)
                {
                    Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                .ToDictionary(pair => pair.Key);

                    var list = new List<dynamic>();
                    using (@this)
                    {
                        while (@this.Read())
                        {
                            dynamic entity = new ExpandoObject();
                            var expandoDict = (IDictionary<string, object>)entity;



                            Enumerable.Range(0, @this.FieldCount)
                                .ToList()
                                .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                            list.Add(entity);
                        }
                    }


                    return list;
                }
                else
                {
                    Dictionary<int, KeyValuePair<int, string>> columnNames = Enumerable.Range(0, @this.FieldCount)
                .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                .ToDictionary(pair => pair.Key);

                    var list = new List<dynamic>();

                    while (@this.Read())
                    {
                        dynamic entity = new ExpandoObject();
                        var expandoDict = (IDictionary<string, object>)entity;

                        Enumerable.Range(0, @this.FieldCount)
                            .ToList()
                            .ForEach(x => expandoDict.Add(columnNames[x].Value.FormatColumnName(database), @this[x]));

                        list.Add(entity);
                    }

                    return list;
                }

            }
            catch (Exception ex)
            {
                if (@this != null)
                {
                    @this.Close();
                }
                throw new PureDataException("DataReaderExtensions", ex);
            }


        }

        private static object HackType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            ///如果是枚举则转换
            if (conversionType.IsEnum)
            {
                return Enum.ToObject(conversionType, Convert.ToInt32(value));
            }

            return Convert.ChangeType(value, conversionType);
        }

        private static bool IsNullOrDBNull(object obj)
        {
            return ((obj is DBNull) || string.IsNullOrEmpty(obj.ToString())) ? true : false;
        }

        #region DataTable to List
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    object val = prop.GetValue(item);
                    row[prop.Name] = val == null ? DBNull.Value : val;//  prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataSet ToDataSet(this IDataReader reader, bool autoClose = true)
        {
            DataTable table = reader.ToDataTable(autoClose);
            DataSet ds = new DataSet();
            ds.Tables.Add(table);
            return ds;
        }
        public static IList<T> ToList<T>(this IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ToList<T>(this DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ToList<T>(rows);
        }

        /// <summary>
        ///     A T[] extension method that converts the @this to a data table.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a DataTable.</returns>
        public static DataTable ToDataTable<T>(this T[] @this)
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var dt = new DataTable();

            foreach (PropertyInfo property in properties)
            {
                dt.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (FieldInfo field in fields)
            {
                dt.Columns.Add(field.Name, field.FieldType);
            }

            foreach (T item in @this)
            {
                DataRow dr = dt.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    object val = property.GetValue(item, null);
                    dr[property.Name] = val == null ? DBNull.Value : val; //property.GetValue(item, null);

                }

                foreach (FieldInfo field in fields)
                {
                    object val = field.GetValue(item);
                    dr[field.Name] = val == null ? DBNull.Value : val;// field.GetValue(item);
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }


        public static T CreateItem<T>(this DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {


                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    var type = typeof(T);
                    PropertyInfo prop = type.GetProperty(column.ColumnName, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, HackType(value, prop.PropertyType), null);
                    }
                    catch
                    {
                        // You can log something here  
                        //throw;
                    }
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                Type colType = prop.PropertyType;
                if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                    NullableConverter nullableConverter = new NullableConverter(colType);
                    //将convertsionType转换为nullable对的基础基元类型
                    colType = nullableConverter.UnderlyingType;

                    //colType = colType.GetGenericArguments()[0];

                }

                table.Columns.Add(new DataColumn(prop.Name, colType));

            }

            return table;
        }


        #endregion


        /// <summary>  
        /// 将对象属性转换为key-value对  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            if (obj != null)
            {

                if (obj is IDictionary<string, object>)
                {

                    var o = obj as IDictionary<string, object>;
                    return o;
                }
                else
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();

                    Type t = obj.GetType();

                    PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (PropertyInfo p in pi)
                    {
                        var value = p.GetValue(obj);
                        if (!map.ContainsKey(p.Name))
                        {
                            map.Add(p.Name, value);

                        }
                    }

                    return map;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取实体类Hashtable键值 
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public static System.Collections.Hashtable ToHashtable(this object entity)
        {
            Type type = entity.GetType();
            
            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            PropertyInfo[] pi = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {

                object value = p.GetValue(entity);
                string name = p.Name;
                ht[name] = value;

            } 
            return ht;

        }


       
    }
}
