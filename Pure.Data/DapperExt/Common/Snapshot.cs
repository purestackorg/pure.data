using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{

    public static class BindPropertiesExts
    {
        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object</param>
        /// <param name="source">The source object</param>
        public static void BindPropertiesFrom(this object target, object source)
        {
            BindPropertiesFrom(target, source, string.Empty);
        }

        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object</param>
        /// <param name="source">The source object</param>
        /// <param name="ignoreProperty">A single property name to ignore</param>
        public static void BindPropertiesFrom(this object target, object source, string ignoreProperty)
        {
            BindPropertiesFrom(target, source, new[] { ignoreProperty });
        }

        /// <summary>
        /// Copies the readable and writable public property values from the source object to the target
        /// </summary>
        /// <remarks>The source and target objects must be of the same type.</remarks>
        /// <param name="target">The target object</param>
        /// <param name="source">The source object</param>
        /// <param name="ignoreProperties">An array of property names to ignore</param>
        public static void BindPropertiesFrom(this object target, object source, params string[] ignoreProperties)
        {
            // Get and check the object types
            Type type = source.GetType();
            if (target.GetType() != type)
            {
                throw new ArgumentException("The source type must be the same as the target");
            }

            // Build a clean list of property names to ignore
            var ignoreList = new List<string>();


            foreach (string item in ignoreProperties)
            {
                if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
                {
                    ignoreList.Add(item.ToUpper());
                }
            }

            //全局过滤的更新的列
            if (Snapshotter.GlobalIgnoreUpdatedColumns != null)
            {
                foreach (string item in Snapshotter.GlobalIgnoreUpdatedColumns)
                {
                    if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
                    {
                        ignoreList.Add(item.ToUpper());
                    }
                }
            }



            // Copy the properties
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.CanWrite
                    && property.CanRead
                    && !ignoreList.Contains(property.Name.ToUpper()))
                {
                    object val = property.GetValue(source, null);
                    property.SetValue(target, val, null);
                }
            }
        }


        public static void BindPropertiesFromForDb(object target, object source, IClassMapper classMap, params string[] ignoreProperties)
        {
            // Get and check the object types
            Type type = source.GetType();
            if (target.GetType() != type)
            {
                throw new ArgumentException("The source type must be the same as the target");
            }

            // Build a clean list of property names to ignore
            var ignoreList = new List<string>();


            foreach (string item in ignoreProperties)
            {
                if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
                {
                    ignoreList.Add(item.ToUpper());
                }
            }

            //全局过滤的更新的列
            if (Snapshotter.GlobalIgnoreUpdatedColumns != null)
            {
                foreach (string item in Snapshotter.GlobalIgnoreUpdatedColumns)
                {
                    if (!string.IsNullOrEmpty(item) && !ignoreList.Contains(item))
                    {
                        ignoreList.Add(item.ToUpper());
                    }
                }
            }

            if (classMap != null)
            {
                var columns = classMap.Properties.Where(p => (p.IsVersionColumn == false && p.Ignored == false && p.IsReadOnly == false && p.IsPrimaryKey == false));
                foreach (var col in columns)
                {
                    PropertyInfo property = col.PropertyInfo;
                    if (property.CanWrite
                    && property.CanRead
                    && !ignoreList.Contains(property.Name.ToUpper()))
                    {


                        object val = property.GetValue(source, null);
                        property.SetValue(target, val, null);
                    }
                }
            }
            else
            {
                // Copy the properties
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.CanWrite
                        && property.CanRead
                        && !ignoreList.Contains(property.Name.ToUpper()))
                    {


                        object val = property.GetValue(source, null);
                        property.SetValue(target, val, null);
                    }
                }
            }




        }
    }


    // Implementation from @SamSaffron
    // http://code.google.com/p/stack-exchange-data-explorer/source/browse/App/StackExchange.DataExplorer/Dapper/Snapshotter.cs
    public static class Snapshotter
    {
        public static Snapshot<T> Track<T>(this IDatabase d, T obj) where T : class
        {
            return new Snapshot<T>(d, obj);
        }

        #region SetGlobalIgnoreUpdatedColumns
        private static string[] _IgnoreUpdatedColumns;

        private static bool AutoFilterEmptyValueColumnsWhenTrack = false;

        public static bool GetAutoFilterEmptyValueColumnsWhenTrack()
        {
            return AutoFilterEmptyValueColumnsWhenTrack;
        }
        public static string[] GlobalIgnoreUpdatedColumns
        {
            get { return _IgnoreUpdatedColumns; }
        }
        /// <summary>
        /// 设定全局过滤更新列名
        /// </summary>
        /// <param name="cols"></param>
        public static void SetGlobalIgnoreUpdatedColumns(params string[] cols)
        {
            _IgnoreUpdatedColumns = cols;
        }
        private static bool hasInit = false;
        private static object olock = new object();

        public static void InitGlobalIgnoreUpdatedColumns(string cols, bool _AutoFilterEmptyValueColumnsWhenTrack)
        {
            if (hasInit == false)
            {
                lock (olock)
                {
                    AutoFilterEmptyValueColumnsWhenTrack = _AutoFilterEmptyValueColumnsWhenTrack;
                    if (!string.IsNullOrEmpty(cols))
                    {
                        SetGlobalIgnoreUpdatedColumns(cols.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                    }
                    hasInit = true;
                }
            }

        }

        #endregion



        //public static Snapshot<T> StartSnapshot<T>(this IDatabase d, T obj)
        //{
        //    return new Snapshot<T>(d.GetMap<T>()., obj);
        //}

        //public static int Update<T>(this IDatabase d, T obj, Snapshot<T> snapshot)
        //{
        //    return d.Update(obj, snapshot.UpdatedColumns());
        //}
    }


    internal class ExecuteSqlParamsContext
    {
        public ExecuteSqlParamsContext(string sql, IDictionary<string, object> parameters) {
            Sql = sql;
            Parameters = parameters;
        }
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; } 
    }


    public class Snapshot<T> where T : class
    {
        #region Update
        /// <summary>
        /// 更新对象变更的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public int UpdateWithIgnoreParams(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] ignoreProperties)
        {
            List<string> ignoreList = ReflectionHelper.GetAllPropertyNames(ignoreProperties);//  new List<string>();
              
            return Update(bindedObj, _PrimaryKeyValues, ignoreList.ToArray());
        }

        /// <summary>
        /// 更新对象变更的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public int Update(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] ignoreProperties)
        {
            //var pd = pocoData;

            //if (bindedObj != null)//自动绑定对象
            //{
            //    BindPropertiesExts.BindPropertiesFromForDb(trackedObject, bindedObj, pd, ignoreProperties);
            //}
            //var columns = UpdatedColumns();
            //object poco = trackedObject;
            //int defaultValue = 0;
            //if (columns != null && !columns.Any())
            //    return defaultValue;

            //var sb = new StringBuilder();
            //var index = 0;
            //string paramPrefix = "";
            //var pkCols = pd.Properties.Where(p => p.IsPrimaryKey).ToList();
            //IDictionary<string, object> PrimaryKeyValues = new Dictionary<string, object>();
            //if (_PrimaryKeyValues != null && _PrimaryKeyValues.Count > 0)
            //{
            //    PrimaryKeyValues = _PrimaryKeyValues;
            //}
            //else
            //{
            //    if (pkCols == null || pkCols.Count == 0)
            //    {
            //        throw new ArgumentException("当前对象没有主键情况下，需要指定更新的主键值");
            //    }
            //    else
            //    {

            //        var Properties = pd.Properties.Select(p => p.PropertyInfo);// poco.GetType().GetProperties();
            //        foreach (var pk in pkCols)
            //        {
            //            string pName = pk.Name;
            //            string pColumnName = pk.Name;//  pk.ColumnName;
            //            var o = Properties.FirstOrDefault(y => string.Equals(pName, y.Name, StringComparison.OrdinalIgnoreCase));
            //            if (o != null)
            //            {
            //                var pValue = o.GetValue(poco, null);
            //                PrimaryKeyValues.Add(pColumnName, pValue);

            //            }

            //        }
            //    }
            //}


            //IDictionary<string, object> parameters = new Dictionary<string, object>();


            //var waitToUpdated = pd.Properties.Where(p => columns.Contains(p.ColumnName) || p.IsVersionColumn == true);
            //foreach (var pocoColumn in waitToUpdated)
            //{
            //    string properyName = pocoColumn.Name;//pocoColumn.ColumnName;

            //    // Don't update the primary key, but grab the value if we don't have it
            //    if (_PrimaryKeyValues == null && PrimaryKeyValues.ContainsKey(properyName))
            //    {
            //        continue;
            //    }

            //    // Dont update result only columns
            //    if (pocoColumn.Ignored || pocoColumn.IsReadOnly || pocoColumn.IsPrimaryKey)
            //        continue;

            //    var colType = pocoColumn.PropertyInfo.PropertyType;

            //    object value = pocoColumn.PropertyInfo.GetValue(poco, null);// GetColumnValue(pd, poco, ProcessMapper);
            //    if (pocoColumn.IsVersionColumn)
            //    {
            //        if (colType == typeof(int) || colType == typeof(Int32))
            //        {
            //            value = Convert.ToInt32(value) + 1;


            //        }
            //        else if (colType == typeof(long) || colType == typeof(Int64))
            //        {
            //            value = Convert.ToInt64(value) + 1;


            //        }
            //        else if (colType == typeof(short) || colType == typeof(Int16))
            //        {
            //            value = Convert.ToInt16(value) + 1;

            //        }
            //    }
            //    else
            //    {
            //        if (Snapshotter.GetAutoFilterEmptyValueColumnsWhenTrack())
            //        {
            //            var defaultValueOfCol = ReflectionHelper.GetDefaultValueForType(colType);
            //            if (object.Equals(defaultValueOfCol, value))//过滤空值列
            //            {
            //                continue;
            //            }
            //        }
            //    }



            //    // Build the sql
            //    if (index > 0)
            //        sb.Append(", ");

            //    //string paramName = paramPrefix + index.ToString();// pocoColumn.Name;
            //    string paramName = paramPrefix + properyName; 
            //    sb.AppendFormat("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, pocoColumn, false), db.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramName);

            //    parameters.Add(paramName, value);
            //    index++;

            //}


            //if (columns != null && columns.Any() && sb.Length == 0)
            //    throw new ArgumentException("There were no columns in the columns list that matched your table", "columns");

            //var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", db.SqlGenerator.GetTableName(pd), sb, BuildWhereSql(db, pd, PrimaryKeyValues, paramPrefix, ref index));
            //int temIndex = parameters.Count;
            //foreach (var item in PrimaryKeyValues)
            //{ 
            //    parameters.Add(paramPrefix + item.Key, item.Value);
            //    //parameters.Add(paramPrefix + temIndex, item.Value);
            //    temIndex++;
            //}


            //if (db.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
            //{
            //    IDictionary<string, object> dictParameters = parameters;
            //    int convertCount = LobConverter.UpdateDynamicParameterForLobColumn(pd, dictParameters);
            //    if (convertCount > 0)
            //    {
            //        return db.Execute(sql, dictParameters); 
            //    }
            //    else
            //    {
            //        //执行原始SQL
            //        return db.Execute(sql, parameters);

            //    }
            //}
            //else
            //{
            //    //执行原始SQL
            //    return db.Execute(sql, parameters);

            //}

            var sqlParamsContext = UpdateInternal(bindedObj, _PrimaryKeyValues, ignoreProperties);
            if (sqlParamsContext == null)
            {
                return 0;
            }
            return db.Execute(sqlParamsContext.Sql, sqlParamsContext.Parameters);


        }


        internal ExecuteSqlParamsContext UpdateInternal(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] ignoreProperties)
        {
            var pd = pocoData;

            if (bindedObj != null)//自动绑定对象
            {
                BindPropertiesExts.BindPropertiesFromForDb(trackedObject, bindedObj, pd, ignoreProperties);
            }
            var columns = UpdatedColumns();
            object poco = trackedObject;
            //int defaultValue = 0;
            if (columns != null && !columns.Any())
                return null;

            var sb = new StringBuilder();
            var index = 0;
            string paramPrefix = "";
            var pkCols = pd.Properties.Where(p => p.IsPrimaryKey).ToList();
            IDictionary<string, object> PrimaryKeyValues = new Dictionary<string, object>();
            if (_PrimaryKeyValues != null && _PrimaryKeyValues.Count > 0)
            {
                PrimaryKeyValues = _PrimaryKeyValues;
            }
            else
            {
                if (pkCols == null || pkCols.Count == 0)
                {
                    throw new ArgumentException("当前对象没有主键情况下，需要指定更新的主键值");
                }
                else
                {

                    var Properties = pd.Properties.Select(p => p.PropertyInfo);// poco.GetType().GetProperties();
                    foreach (var pk in pkCols)
                    {
                        string pName = pk.Name;
                        string pColumnName = pk.Name;//  pk.ColumnName;
                        var o = Properties.FirstOrDefault(y => string.Equals(pName, y.Name, StringComparison.OrdinalIgnoreCase));
                        if (o != null)
                        {
                            var pValue = o.GetValue(poco, null);
                            PrimaryKeyValues.Add(pColumnName, pValue);

                        }

                    }
                }
            }


            IDictionary<string, object> parameters = new Dictionary<string, object>();


            var waitToUpdated = pd.Properties.Where(p => columns.Contains(p.ColumnName) || p.IsVersionColumn == true);
            foreach (var pocoColumn in waitToUpdated)
            {
                string properyName = pocoColumn.Name;//pocoColumn.ColumnName;

                // Don't update the primary key, but grab the value if we don't have it
                if (_PrimaryKeyValues == null && PrimaryKeyValues.ContainsKey(properyName))
                {
                    continue;
                }

                // Dont update result only columns
                if (pocoColumn.Ignored || pocoColumn.IsReadOnly || pocoColumn.IsPrimaryKey)
                    continue;

                var colType = pocoColumn.PropertyInfo.PropertyType;

                object value = pocoColumn.PropertyInfo.GetValue(poco, null);// GetColumnValue(pd, poco, ProcessMapper);
                if (pocoColumn.IsVersionColumn)
                {
                    if (colType == typeof(int) || colType == typeof(Int32))
                    {
                        value = Convert.ToInt32(value) + 1;


                    }
                    else if (colType == typeof(long) || colType == typeof(Int64))
                    {
                        value = Convert.ToInt64(value) + 1;


                    }
                    else if (colType == typeof(short) || colType == typeof(Int16))
                    {
                        value = Convert.ToInt16(value) + 1;

                    }
                }
                else
                {
                    if (Snapshotter.GetAutoFilterEmptyValueColumnsWhenTrack())
                    {
                        var defaultValueOfCol = ReflectionHelper.GetDefaultValueForType(colType);
                        if (object.Equals(defaultValueOfCol, value))//过滤空值列
                        {
                            continue;
                        }
                    }
                }



                // Build the sql
                if (index > 0)
                    sb.Append(", ");

                //string paramName = paramPrefix + index.ToString();// pocoColumn.Name;
                string paramName = paramPrefix + properyName;
                sb.AppendFormat("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, pocoColumn, false), db.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramName);

                parameters.Add(paramName, value);
                index++;

            }


            if (columns != null && columns.Any() && sb.Length == 0)
                throw new ArgumentException("There were no columns in the columns list that matched your table", "columns");

            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", db.SqlGenerator.GetTableName(pd), sb, BuildWhereSql(db, pd, PrimaryKeyValues, paramPrefix, ref index));
            int temIndex = parameters.Count;
            foreach (var item in PrimaryKeyValues)
            {
                parameters.Add(paramPrefix + item.Key, item.Value);
                //parameters.Add(paramPrefix + temIndex, item.Value);
                temIndex++;
            }


            if (db.DatabaseType == DatabaseType.Oracle && LobConverter.Enable == true)
            {
                IDictionary<string, object> dictParameters = parameters;
                int convertCount = LobConverter.UpdateDynamicParameterForLobColumn(pd, dictParameters);
                if (convertCount > 0)
                {
                    //return db.Execute(sql, dictParameters);
                    return new ExecuteSqlParamsContext(sql, dictParameters);
                }
                else
                {
                    //执行原始SQL
                    //return db.Execute(sql, parameters);

                    return new ExecuteSqlParamsContext(sql, parameters);
                }
            }
            else
            {
                //执行原始SQL
                //return db.Execute(sql, parameters);
                return new ExecuteSqlParamsContext(sql, parameters);

            }


        }




        /// <summary>
        /// 更新对象变更的内容
        /// </summary>
        /// <param name="bindedObj"></param>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public async Task<int> UpdateWithIgnoreParamsAsync(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params Expression<Func<T, object>>[] ignoreProperties)
        {
            List<string> ignoreList = ReflectionHelper.GetAllPropertyNames(ignoreProperties);//  new List<string>();

            return await UpdateAsync(bindedObj, _PrimaryKeyValues, ignoreList.ToArray());
        }
        /// <summary>
        /// 更新对象变更的内容
        /// </summary>
        /// <param name="_PrimaryKeyValues"></param>
        /// <param name="bindedObj"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public async  Task<int> UpdateAsync(object bindedObj = null, IDictionary<string, object> _PrimaryKeyValues = null, params string[] ignoreProperties)
        {
            var sqlParamsContext = UpdateInternal(bindedObj, _PrimaryKeyValues, ignoreProperties);
            if (sqlParamsContext == null)
            {
                return 0;
            }
            return await db.ExecuteAsync(sqlParamsContext.Sql, sqlParamsContext.Parameters);
             
        }


        private string BuildWhereSql(IDatabase db, IClassMapper pd, IDictionary<string, object> primaryKeyValuePair, string paramPrefix, ref int index)
        {
            var tempIndex = index; 
            //return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) : string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false), db.SqlGenerator.Configuration.Dialect.ParameterPrefix, paramPrefix + (tempIndex + i).ToString())).ToArray());

            return string.Join(" AND ", primaryKeyValuePair.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? 
            string.Format("{0} IS NULL", db.SqlGenerator.GetColumnName(pd, x.Key, false)) :
            string.Format("{0} = {1}{2}", db.SqlGenerator.GetColumnName(pd, x.Key, false), 
            db.SqlGenerator.Configuration.Dialect.ParameterPrefix, 
            paramPrefix +  x.Key
            )
            ).ToArray());

        }


        #endregion

 

        static Func<T, T> cloner;
        static Func<T, T, List<Change>> differ;
        T memberWiseClone;
        T trackedObject;

        IClassMapper pocoData;
        IDatabase db = null;
        public Snapshot(IDatabase database, T original)
        {
            memberWiseClone = Clone(original);
            trackedObject = original;
            db = database;
            pocoData = database.GetMap<T>();
        }

        public class Change
        {
            public string Name { get; set; }
            public string ColumnName { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

        public List<Change> Changes()
        {
            var changes = Diff(memberWiseClone, trackedObject);
            foreach (var c in changes)
            {
                var typeData = pocoData.Properties.FirstOrDefault(x => x.PropertyInfo.Name == c.Name);
                c.ColumnName = typeData != null ? typeData.ColumnName : c.Name;
            }

            return changes;
        }

        public List<string> UpdatedColumns()
        {
            return Changes().Select(x => x.ColumnName).ToList();
        }

        private static T Clone(T myObject)
        {
            cloner = cloner ?? GenerateCloner();
            return cloner(myObject);
        }

        private static List<Change> Diff(T original, T current)
        {
            differ = differ ?? GenerateDiffer();
            return differ(original, current);
        }

        static List<PropertyInfo> RelevantProperties()
        {
            return typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                    p.GetSetMethod() != null &&
                    p.GetGetMethod() != null &&
                    (p.PropertyType.IsValueType ||
                        p.PropertyType == typeof(string) ||
                        (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    ).ToList();
        }


        private static bool AreEqual<U>(U first, U second)
        {
            if (first == null && second == null) return true;
            if (first == null && second != null) return false;
            return first.Equals(second);
        }

        private static Func<T, T, List<Change>> GenerateDiffer()
        {

            var dm = new DynamicMethod("DoDiff", typeof(List<Change>), new Type[] { typeof(T), typeof(T) }, true);

            var il = dm.GetILGenerator();
            // change list
            il.DeclareLocal(typeof(List<Change>));
            il.DeclareLocal(typeof(Change));
            il.DeclareLocal(typeof(object)); // boxed change
            il.DeclareLocal(typeof(object)); // orig val

            il.Emit(OpCodes.Newobj, typeof(List<Change>).GetConstructor(Type.EmptyTypes));
            // [list]
            il.Emit(OpCodes.Stloc_0);

            foreach (var prop in RelevantProperties())
            {
                // []
                il.Emit(OpCodes.Ldarg_0);
                // [original]
                il.Emit(OpCodes.Callvirt, prop.GetGetMethod());
                // [original prop val]

                il.Emit(OpCodes.Dup);
                // [original prop val, original prop val]

                if (prop.PropertyType != typeof(string))
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                    // [original prop val, original prop val boxed]
                }

                il.Emit(OpCodes.Stloc_3);
                // [original prop val]

                il.Emit(OpCodes.Ldarg_1);
                // [original prop val, current]

                il.Emit(OpCodes.Callvirt, prop.GetGetMethod());
                // [original prop val, current prop val]

                il.Emit(OpCodes.Dup);
                // [original prop val, current prop val, current prop val]

                if (prop.PropertyType != typeof(string))
                {
                    il.Emit(OpCodes.Box, prop.PropertyType);
                    // [original prop val, current prop val, current prop val boxed]
                }

                il.Emit(OpCodes.Stloc_2);
                // [original prop val, current prop val]

                il.EmitCall(OpCodes.Call, typeof(Snapshot<T>).GetMethod("AreEqual", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new Type[] { prop.PropertyType }), null);
                // [result] 

                Label skip = il.DefineLabel();
                il.Emit(OpCodes.Brtrue_S, skip);
                // []

                il.Emit(OpCodes.Newobj, typeof(Change).GetConstructor(Type.EmptyTypes));
                // [change]
                il.Emit(OpCodes.Dup);
                // [change,change]

                il.Emit(OpCodes.Stloc_1);
                // [change]

                il.Emit(OpCodes.Ldstr, prop.Name);
                // [change, name]
                il.Emit(OpCodes.Callvirt, typeof(Change).GetMethod("set_Name"));
                // []

                il.Emit(OpCodes.Ldloc_1);
                // [change]

                il.Emit(OpCodes.Ldloc_3);
                // [change, original prop val boxed]

                il.Emit(OpCodes.Callvirt, typeof(Change).GetMethod("set_OldValue"));
                // []

                il.Emit(OpCodes.Ldloc_1);
                // [change]

                il.Emit(OpCodes.Ldloc_2);
                // [change, boxed]

                il.Emit(OpCodes.Callvirt, typeof(Change).GetMethod("set_NewValue"));
                // []

                il.Emit(OpCodes.Ldloc_0);
                // [change list]
                il.Emit(OpCodes.Ldloc_1);
                // [change list, change]
                il.Emit(OpCodes.Callvirt, typeof(List<Change>).GetMethod("Add"));
                // []

                il.MarkLabel(skip);
            }

            il.Emit(OpCodes.Ldloc_0);
            // [change list]
            il.Emit(OpCodes.Ret);

            return (Func<T, T, List<Change>>)dm.CreateDelegate(typeof(Func<T, T, List<Change>>));
        }


        // adapted from http://stackoverflow.com/a/966466/17174
        private static Func<T, T> GenerateCloner()
        {
            Delegate myExec = null;
            var dm = new DynamicMethod("DoClone", typeof(T), new Type[] { typeof(T) }, true);
            var ctor = typeof(T).GetConstructor(new Type[] { });

            var il = dm.GetILGenerator();

            il.DeclareLocal(typeof(T));

            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Stloc_0);

            foreach (var prop in RelevantProperties())
            {
                il.Emit(OpCodes.Ldloc_0);
                // [clone]
                il.Emit(OpCodes.Ldarg_0);
                // [clone, source]
                il.Emit(OpCodes.Callvirt, prop.GetGetMethod());
                // [clone, source val]
                il.Emit(OpCodes.Callvirt, prop.GetSetMethod());
                // []
            }

            // Load new constructed obj on eval stack -> 1 item on stack
            il.Emit(OpCodes.Ldloc_0);
            // Return constructed object.   --> 0 items on stack
            il.Emit(OpCodes.Ret);

            myExec = dm.CreateDelegate(typeof(Func<T, T>));

            return (Func<T, T>)myExec;
        }
    }
}
