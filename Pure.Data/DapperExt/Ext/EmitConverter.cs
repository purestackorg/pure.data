using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
namespace Pure.Data
{

    internal class DataUtils
    {
        internal static class DBConvert
        {
            public static bool IsDBNull(object value)
            {
                return object.Equals(DBNull.Value, value);
            }
            public static short ToInt16(object value)
            {
                if (value is short)
                {
                    return (short)value;
                }
                try
                {
                    return Convert.ToInt16(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static ushort ToUInt16(object value)
            {
                if (value is ushort)
                {
                    return (ushort)value;
                }
                try
                {
                    return Convert.ToUInt16(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static int ToInt32(object value)
            {
                if (value is int)
                {
                    return (int)value;
                }
                try
                {
                    return Convert.ToInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static uint ToUInt32(object value)
            {
                if (value is uint)
                {
                    return (uint)value;
                }
                try
                {
                    return Convert.ToUInt32(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static long ToInt64(object value)
            {
                if (value is long)
                {
                    return (long)value;
                }
                try
                {
                    return Convert.ToInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static ulong ToUInt64(object value)
            {
                if (value is long)
                {
                    return (ulong)value;
                }
                try
                {
                    return Convert.ToUInt64(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static bool ToBoolean(object value)
            {
                if (value == null)
                {
                    return false;
                }
                if (value is bool)
                {
                    return (bool)value;
                }
                if (value.Equals("1") || value.Equals("-1"))
                {
                    value = "true";
                }
                else if (value.Equals("0"))
                {
                    value = "false";
                }

                try
                {
                    return Convert.ToBoolean(value);
                }
                catch
                {
                    return false;
                }
            }
            public static DateTime ToDateTime(object value)
            {
                if (value is DateTime)
                {
                    return (DateTime)value;
                }
                try
                {
                    return Convert.ToDateTime(value);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            public static decimal ToDecimal(object value)
            {
                if (value is decimal)
                {
                    return (decimal)value;
                }
                try
                {
                    return Convert.ToDecimal(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static double ToDouble(object value)
            {
                if (value is double)
                {
                    return (double)value;
                }
                try
                {
                    return Convert.ToDouble(value);
                }
                catch
                {
                    return 0;
                }
            }
            //2015-09-22
            public static float ToFloat(object value)
            {
                if (value is Single || value is float)
                {
                    return (float)value;
                }
                try
                {
                    return Convert.ToSingle(value);
                }
                catch
                {
                    return 0;
                }
            }
            public static Guid ToGuid(object value)
            {
                if (value is Guid)
                {
                    return (Guid)value;
                }
                try
                {
                    return Guid.Parse(value.ToString());
                }
                catch
                {
                    return new Guid();
                }
            }
            public static byte[] ToByteArr(object value)
            {
                var arr = value as byte[];
                return arr;
            }

            public static Nullable<short> ToNInt16(object value)
            {
                if (value is short)
                {
                    return new Nullable<short>((short)value);
                }
                try
                {
                    return new Nullable<short>(Convert.ToInt16(value));
                }
                catch
                {
                    return new Nullable<short>();
                }
            }
            public static Nullable<ushort> ToNUInt16(object value)
            {
                if (value is ushort)
                {
                    return new Nullable<ushort>((ushort)value);
                }
                try
                {
                    return new Nullable<ushort>(Convert.ToUInt16(value));
                }
                catch
                {
                    return new Nullable<ushort>();
                }
            }
            public static Nullable<int> ToNInt32(object value)
            {
                if (value is int)
                {
                    return new Nullable<int>((int)value);
                }
                try
                {
                    return new Nullable<int>(Convert.ToInt32(value));
                }
                catch
                {
                    return new Nullable<int>();
                }
            }
            public static Nullable<uint> ToNUInt32(object value)
            {
                if (value is uint)
                {
                    return new Nullable<uint>((uint)value);
                }
                try
                {
                    return new Nullable<uint>(Convert.ToUInt32(value));
                }
                catch
                {
                    return new Nullable<uint>();
                }
            }
            public static Nullable<long> ToNInt64(object value)
            {
                if (value is long)
                {
                    return new Nullable<long>((long)value);
                }
                try
                {
                    return new Nullable<long>(Convert.ToInt64(value));
                }
                catch
                {
                    return new Nullable<long>();
                }
            }
            public static Nullable<ulong> ToNUInt64(object value)
            {
                if (value is long)
                {
                    return new Nullable<ulong>((ulong)value);
                }
                try
                {
                    return new Nullable<ulong>(Convert.ToUInt64(value));
                }
                catch
                {
                    return new Nullable<ulong>();
                }
            }
            public static Nullable<bool> ToNBoolean(object value)
            {
                if (value is bool)
                {
                    return new Nullable<bool>((bool)value);
                }
                try
                {
                    return new Nullable<bool>(Convert.ToBoolean(value));
                }
                catch
                {
                    return new Nullable<bool>();
                }
            }
            public static Nullable<DateTime> ToNDateTime(object value)
            {
                if (value is DateTime)
                {
                    return new Nullable<DateTime>((DateTime)value);
                }
                try
                {
                    return new Nullable<DateTime>(Convert.ToDateTime(value));
                }
                catch
                {
                    return new Nullable<DateTime>();
                }
            }
            public static Nullable<decimal> ToNDecimal(object value)
            {
                if (value is decimal)
                {
                    return new Nullable<decimal>((decimal)value);
                }
                try
                {
                    return new Nullable<decimal>(Convert.ToDecimal(value));
                }
                catch
                {
                    return new Nullable<decimal>();
                }
            }
            public static Nullable<double> ToNDouble(object value)
            {
                if (value is double)
                {
                    return new Nullable<double>((double)value);
                }
                try
                {
                    return new Nullable<double>(Convert.ToDouble(value));
                }
                catch
                {
                    return new Nullable<double>();
                }
            }
            public static Nullable<float> ToNFloat(object value)
            {
                if (value is Single || value is float)
                {
                    return new Nullable<float>((float)value);
                }
                try
                {
                    return new Nullable<float>(Convert.ToSingle(value));
                }
                catch
                {
                    return new Nullable<float>();
                }
            }
            public static Nullable<Guid> ToNGuid(object value)
            {
                if (value is Guid)
                {
                    return new Nullable<Guid>((Guid)value);
                }
                try
                {
                    return new Nullable<Guid>(Guid.Parse(value.ToString()));
                }
                catch
                {
                    return new Nullable<Guid>();
                }
            }
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("此方法仅供内部使用", false)]
        public static char ReadChar(object value)
        {
            if (value == null || value is DBNull) throw new ArgumentNullException("value");
            string s = value as string;
            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", "value");
            return s[0];
        }
        /// <summary>
        /// Internal use only
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is for internal usage only", false)]
        public static char? ReadNullableChar(object value)
        {
            if (value == null || value is DBNull) return null;
            string s = value as string;
            if (s == null || s.Length != 1) throw new ArgumentException("A single-character was expected", "value");
            return s[0];
        }

         /// <summary>
        /// Throws a data exception, only used internally
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="index"></param>
        /// <param name="reader"></param>
        public static void ThrowDataException(Exception ex, int index, IDataReader reader)
        {
            string name = "(n/a)", value = "(n/a)";
            if (reader != null && index >= 0 && index < reader.FieldCount)
            {
                name = reader.GetName(index);
                object val = reader.GetValue(index);
                if (val == null || val is DBNull)
                {
                    value = "<null>";
                }
                else
                {
                    value = Convert.ToString(val) + " - " + Type.GetTypeCode(val.GetType());
                }
            }
            if (!(index >= reader.FieldCount))
            {
                throw new DataException(string.Format("Error parsing column {0} ({1}={2})", index, name, value), ex);
            }
        }
    }
    /// <summary>
    /// EmitConverter
    /// </summary>
    internal static class EmitConverter
    {
         

          
        private static readonly MethodInfo Object_ToString = typeof(object).GetMethod("ToString");
        private static readonly MethodInfo Reader_Read = typeof(IDataReader).GetMethod("Read");
        private static readonly MethodInfo Reader_GetValues = typeof(IDataRecord).GetMethod("GetValues", new Type[] { typeof(object[]) });
        private static readonly MethodInfo Convert_IsDBNull = typeof(DataUtils.DBConvert).GetMethod("IsDBNull", new Type[] { typeof(object) });

        private static readonly MethodInfo Convert_ToInt16 = typeof(DataUtils.DBConvert).GetMethod("ToInt16", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToUInt16 = typeof(DataUtils.DBConvert).GetMethod("ToUInt16", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToInt32 = typeof(DataUtils.DBConvert).GetMethod("ToInt32", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToUInt32 = typeof(DataUtils.DBConvert).GetMethod("ToUInt32", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToInt64 = typeof(DataUtils.DBConvert).GetMethod("ToInt64", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToUInt64 = typeof(DataUtils.DBConvert).GetMethod("ToUInt64", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToBoolean = typeof(DataUtils.DBConvert).GetMethod("ToBoolean", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToDateTime = typeof(DataUtils.DBConvert).GetMethod("ToDateTime", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToDecimal = typeof(DataUtils.DBConvert).GetMethod("ToDecimal", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToDouble = typeof(DataUtils.DBConvert).GetMethod("ToDouble", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToFloat = typeof(DataUtils.DBConvert).GetMethod("ToFloat", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToGuid = typeof(DataUtils.DBConvert).GetMethod("ToGuid", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToByteArr = typeof(DataUtils.DBConvert).GetMethod("ToByteArr", new Type[] { typeof(object) });

        private static readonly MethodInfo Convert_ToNullInt16 = typeof(DataUtils.DBConvert).GetMethod("ToNInt16", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullUInt16 = typeof(DataUtils.DBConvert).GetMethod("ToNUInt16", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullInt32 = typeof(DataUtils.DBConvert).GetMethod("ToNInt32", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullUInt32 = typeof(DataUtils.DBConvert).GetMethod("ToNUInt32", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullInt64 = typeof(DataUtils.DBConvert).GetMethod("ToNInt64", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullUInt64 = typeof(DataUtils.DBConvert).GetMethod("ToNUInt64", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullBoolean = typeof(DataUtils.DBConvert).GetMethod("ToNBoolean", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullDateTime = typeof(DataUtils.DBConvert).GetMethod("ToNDateTime", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullDecimal = typeof(DataUtils.DBConvert).GetMethod("ToNDecimal", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullDouble = typeof(DataUtils.DBConvert).GetMethod("ToNDouble", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullFloat = typeof(DataUtils.DBConvert).GetMethod("ToNFloat", new Type[] { typeof(object) });
        private static readonly MethodInfo Convert_ToNullGuid = typeof(DataUtils.DBConvert).GetMethod("ToNGuid", new Type[] { typeof(object) });
        private delegate T ReadEntityInvoker<T>(IDataReader dr);
        private static Dictionary<string, DynamicMethod> m_CatchMethod;
        private static void ConvertValue(ILGenerator ilg, Type pi)//PropertyInfo pi
        {
            TypeCode code = Type.GetTypeCode(pi);
            switch (code)
            {
                case TypeCode.Int16:
                    ilg.Emit(OpCodes.Call, Convert_ToInt16);
                    return;
                case TypeCode.UInt16:
                    ilg.Emit(OpCodes.Call, Convert_ToUInt16);
                    return;
                case TypeCode.Int32:
                    ilg.Emit(OpCodes.Call, Convert_ToInt32);
                    return;
                case TypeCode.UInt32:
                    ilg.Emit(OpCodes.Call, Convert_ToUInt32);
                    return;
                case TypeCode.Int64:
                    ilg.Emit(OpCodes.Call, Convert_ToInt64);
                    return;
                case TypeCode.UInt64:
                    ilg.Emit(OpCodes.Call, Convert_ToUInt64);
                    return;
                case TypeCode.Boolean:
                    ilg.Emit(OpCodes.Call, Convert_ToBoolean);
                    return;
                case TypeCode.String:
                    ilg.Emit(OpCodes.Callvirt, Object_ToString);
                    return;
                case TypeCode.DateTime:
                    ilg.Emit(OpCodes.Call, Convert_ToDateTime);
                    return;
                case TypeCode.Decimal:
                    ilg.Emit(OpCodes.Call, Convert_ToDecimal);
                    return;
                case TypeCode.Double:
                    ilg.Emit(OpCodes.Call, Convert_ToDouble);
                    return;
                case TypeCode.Single:
                    ilg.Emit(OpCodes.Call, Convert_ToFloat);
                    return;
            }
            Type type = Nullable.GetUnderlyingType(pi);
            if (type != null)
            {
                code = Type.GetTypeCode(type);
                switch (code)
                {
                    case TypeCode.Int16:
                        ilg.Emit(OpCodes.Call, Convert_ToNullInt16);
                        return;
                    case TypeCode.UInt16:
                        ilg.Emit(OpCodes.Call, Convert_ToNullUInt16);
                        return;
                    case TypeCode.Int32:
                        ilg.Emit(OpCodes.Call, Convert_ToNullInt32);
                        return;
                    case TypeCode.UInt32:
                        ilg.Emit(OpCodes.Call, Convert_ToNullUInt32);
                        return;
                    case TypeCode.Int64:
                        ilg.Emit(OpCodes.Call, Convert_ToNullInt64);
                        return;
                    case TypeCode.UInt64:
                        ilg.Emit(OpCodes.Call, Convert_ToNullUInt64);
                        return;
                    case TypeCode.Boolean:
                        ilg.Emit(OpCodes.Call, Convert_ToNullBoolean);
                        return;
                    case TypeCode.DateTime:
                        ilg.Emit(OpCodes.Call, Convert_ToNullDateTime);
                        return;
                    case TypeCode.Decimal:
                        ilg.Emit(OpCodes.Call, Convert_ToNullDecimal);
                        return;
                    case TypeCode.Double:
                        ilg.Emit(OpCodes.Call, Convert_ToNullDouble);
                        return;
                    case TypeCode.Single:
                        ilg.Emit(OpCodes.Call, Convert_ToNullFloat);
                        return;
                }
                if (type.Name == "Guid")
                {
                    ilg.Emit(OpCodes.Call, Convert_ToNullGuid);
                    return;
                }
            }
            if (pi.Name == "Guid")
            {
                ilg.Emit(OpCodes.Call, Convert_ToGuid);
                return;
            }
            if (pi.Name == "Byte[]")
            {
                ilg.Emit(OpCodes.Call, Convert_ToByteArr);
                return;
            }
            throw new Exception(string.Format("不支持\"{0}\"类型的转换！", pi.Name));
        }
        static readonly Dictionary<Type, DbType> typeMap;
        static EmitConverter()
        {
            typeMap = new Dictionary<Type, DbType>();
            typeMap[typeof(byte)] = DbType.Byte;
            typeMap[typeof(sbyte)] = DbType.SByte;
            typeMap[typeof(short)] = DbType.Int16;
            typeMap[typeof(ushort)] = DbType.UInt16;
            typeMap[typeof(int)] = DbType.Int32;
            typeMap[typeof(uint)] = DbType.UInt32;
            typeMap[typeof(long)] = DbType.Int64;
            typeMap[typeof(ulong)] = DbType.UInt64;
            typeMap[typeof(float)] = DbType.Single;
            typeMap[typeof(double)] = DbType.Double;
            typeMap[typeof(decimal)] = DbType.Decimal;
            typeMap[typeof(bool)] = DbType.Boolean;
            typeMap[typeof(string)] = DbType.String;
            typeMap[typeof(char)] = DbType.StringFixedLength;
            typeMap[typeof(Guid)] = DbType.Guid;
            typeMap[typeof(DateTime)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            typeMap[typeof(byte[])] = DbType.Binary;
            typeMap[typeof(byte?)] = DbType.Byte;
            typeMap[typeof(sbyte?)] = DbType.SByte;
            typeMap[typeof(short?)] = DbType.Int16;
            typeMap[typeof(ushort?)] = DbType.UInt16;
            typeMap[typeof(int?)] = DbType.Int32;
            typeMap[typeof(uint?)] = DbType.UInt32;
            typeMap[typeof(long?)] = DbType.Int64;
            typeMap[typeof(ulong?)] = DbType.UInt64;
            typeMap[typeof(float?)] = DbType.Single;
            typeMap[typeof(double?)] = DbType.Double;
            typeMap[typeof(decimal?)] = DbType.Decimal;
            typeMap[typeof(bool?)] = DbType.Boolean;
            typeMap[typeof(char?)] = DbType.StringFixedLength;
            typeMap[typeof(Guid?)] = DbType.Guid;
            typeMap[typeof(DateTime?)] = DbType.DateTime;
            typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;

            FastExpandoDescriptionProvider provider = new FastExpandoDescriptionProvider();
            TypeDescriptor.AddProvider(provider, typeof(FastExpando));
        }
        private const string LinqBinary = "System.Data.Linq.Binary";
        /// <summary>
        ///// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <param name="startBound"></param>
        /// <param name="length"></param>
        /// <param name="returnNullIfFirstMissing"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetDeserializer(Type type, IDataReader reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            if (type == typeof(object)
                || type == typeof(FastExpando))
            {
                return GetDynamicDeserializer(reader, startBound, length, returnNullIfFirstMissing);
            }

            if (!(typeMap.ContainsKey(type) || type.FullName == LinqBinary))
            {
                return GetTypeDeserializer(type, reader, startBound, length, returnNullIfFirstMissing);
            }
            return GetStructDeserializer(type, startBound);
        }/// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static Func<IDataReader, object> GetStructDeserializer(Type type, int index)
        {
            if (type == typeof(char))
            {
                return r => DataUtils.ReadChar(r.GetValue(index));
            }
            if (type == typeof(char?))
            {
                return r => DataUtils.ReadNullableChar(r.GetValue(index));
            }
            if (type.FullName == LinqBinary)
            {
                return r => Activator.CreateInstance(type, r.GetValue(index));
            }
            if (type == typeof(bool))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val == DBNull.Value ? false : (val.GetType() == type ? val : Convert.ToBoolean(val));
                };
            }
            if (type == typeof(bool?))
            {
                return r =>
                {
                    var val = r.GetValue(index);
                    return val == DBNull.Value ? null : (val.GetType() == type ? val : Convert.ToBoolean(val));
                };
            }
            return r =>
            {
                var val = r.GetValue(index);
                return val is DBNull ? null : val;
            };
        }
        class PropInfo
        {
            public string Name { get; set; }
            public MethodInfo Setter { get; set; }
            public Type Type { get; set; }
        }/// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static List<PropInfo> GetSettableProps(Type t)
        {
            return t
                  .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                  .Select(p => new PropInfo
                  {
                     
                      Name = (p.Name) ,
                      Setter = p.DeclaringType == t ? p.GetSetMethod(true) : p.DeclaringType.GetProperty(p.Name).GetSetMethod(true),
                      Type = p.PropertyType
                  })
                  .Where(info => info.Setter != null)
                  .ToList();
        }/// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static List<FieldInfo> GetSettableFields(Type t)
        {
            return t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        }/// <summary>
        /// 
        /// </summary>
        /// <param name="il"></param>
        /// <param name="value"></param>
        private static void EmitInt32(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1: il.Emit(OpCodes.Ldc_I4_M1); break;
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: il.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    break;
            }
        }
        static readonly MethodInfo
                enumParse = typeof(Enum).GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetIndexParameters().Any() && p.GetIndexParameters()[0].ParameterType == typeof(int))
                    .Select(p => p.GetGetMethod()).First();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="reader"></param>
        /// <param name="startBound"></param>
        /// <param name="length"></param>
        /// <param name="returnNullIfFirstMissing"></param>
        /// <returns></returns>
        public static Func<IDataReader, object> GetTypeDeserializer(Type type, IDataReader reader, int startBound = 0, int length = -1, bool returnNullIfFirstMissing = false)
        {
            var dm = new DynamicMethod(string.Format("Deserialize{0}", Guid.NewGuid()), typeof(object), new[] { typeof(IDataReader) }, true);

            var il = dm.GetILGenerator();
            il.DeclareLocal(typeof(int));
            il.DeclareLocal(type);
            bool haveEnumLocal = false;
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_0);
            var properties = GetSettableProps(type);
            var fields = GetSettableFields(type);
            if (length == -1)
            {
                length = reader.FieldCount - startBound;
            }

            if (reader.FieldCount <= startBound)
            {
                throw new ArgumentException("reader.FieldCount <= startBound", "splitOn");
            }

            var names = new List<string>();

            #region 2016-09-27 暂时修改:将循环改写成从properties获取names
            for (int i = startBound; i < startBound + length; i++)
            {
                names.Add(reader.GetName(i));
            }
            //names = properties.Select(d => d.Name).ToList();
            #endregion
            var setters = (
                            from n in names
                            let prop = properties.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.Ordinal))
                                  ?? properties.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase))
                            let field = prop != null ? null : (fields.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.Ordinal))
                                ?? fields.FirstOrDefault(p => string.Equals(p.Name, n, StringComparison.OrdinalIgnoreCase)))
                            select new { Name = n, Property = prop, Field = field }
                          ).ToList();

            int index = startBound;

            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)1);
                il.Emit(OpCodes.Initobj, type);
            }
            else
            {
                il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
                il.Emit(OpCodes.Stloc_1);
            }
            il.BeginExceptionBlock();
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Ldloca_S, (byte)1);
            }
            else
            {
                il.Emit(OpCodes.Ldloc_1);
            }
            bool first = true;
            var allDone = il.DefineLabel();
            foreach (var item in setters)
            {
                if (item.Property != null || item.Field != null)
                {
                    il.Emit(OpCodes.Dup);
                    Label isDbNullLabel = il.DefineLabel();
                    Label finishLabel = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_0);
                    EmitInt32(il, index);
                    il.Emit(OpCodes.Dup);
                    il.Emit(OpCodes.Stloc_0);
                    il.Emit(OpCodes.Callvirt, getItem);

                    Type memberType = item.Property != null ? item.Property.Type : item.Field.FieldType;

                    if (memberType == typeof(char) || memberType == typeof(char?))
                    {
                        il.EmitCall(OpCodes.Call, typeof(DataUtils).GetMethod(
                            memberType == typeof(char) ? "ReadChar" : "ReadNullableChar", BindingFlags.Static | BindingFlags.Public), null);
                    }
                    //else if (memberType == typeof(bool) || memberType == typeof(bool?))
                    //{
                    //    il.EmitCall(OpCodes.Call, typeof(SqlMapper).GetMethod(
                    //        memberType == typeof(bool) ? "ReadBoolean" : "ReadNullableBoolean", BindingFlags.Static | BindingFlags.Public), null);
                    //}
                    else
                    {
                        il.Emit(OpCodes.Dup);
                        il.Emit(OpCodes.Isinst, typeof(DBNull));
                        il.Emit(OpCodes.Brtrue_S, isDbNullLabel);
                        var nullUnderlyingType = Nullable.GetUnderlyingType(memberType);
                        var unboxType = nullUnderlyingType != null && nullUnderlyingType.IsEnum ? nullUnderlyingType : memberType;
                        if (unboxType.IsEnum)
                        {
                            if (!haveEnumLocal)
                            {
                                il.DeclareLocal(typeof(string));
                                haveEnumLocal = true;
                            }

                            Label isNotString = il.DefineLabel();
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Isinst, typeof(string));
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Stloc_2);
                            il.Emit(OpCodes.Brfalse_S, isNotString);
                            il.Emit(OpCodes.Pop);
                            il.Emit(OpCodes.Ldtoken, unboxType);
                            il.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
                            il.Emit(OpCodes.Ldloc_2);
                            il.Emit(OpCodes.Ldc_I4_1);
                            il.EmitCall(OpCodes.Call, enumParse, null);
                            il.Emit(OpCodes.Unbox_Any, unboxType);
                            if (nullUnderlyingType != null)
                            {
                                il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType }));
                            }
                            if (item.Property != null)
                            {
                                il.Emit(OpCodes.Callvirt, item.Property.Setter);
                            }
                            else
                            {
                                il.Emit(OpCodes.Stfld, item.Field);
                            }
                            il.Emit(OpCodes.Br_S, finishLabel);
                            il.MarkLabel(isNotString);
                        }
                        if (memberType.FullName == LinqBinary)
                        {
                            il.Emit(OpCodes.Unbox_Any, typeof(byte[]));
                            il.Emit(OpCodes.Newobj, memberType.GetConstructor(new Type[] { typeof(byte[]) }));
                        }
                        else if (memberType == typeof(bool) || memberType == typeof(bool?))
                        {
                            il.EmitCall(OpCodes.Call, typeof(Convert).GetMethod("ToBoolean", new Type[] { typeof(object) }), null);
                        }
                        else
                        {
                            ConvertValue(il, properties.First(d => String.Equals(d.Name, item.Name, StringComparison.CurrentCultureIgnoreCase)).Type);
                        }
                        if (nullUnderlyingType != null && (nullUnderlyingType.IsEnum || nullUnderlyingType == typeof(bool)))
                        {
                            il.Emit(OpCodes.Newobj, memberType.GetConstructor(new[] { nullUnderlyingType }));
                        }
                    }
                    if (item.Property != null)
                    {
                        il.Emit(type.IsValueType ? OpCodes.Call : OpCodes.Callvirt, item.Property.Setter);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stfld, item.Field);
                    }
                    il.Emit(OpCodes.Br_S, finishLabel);
                    il.MarkLabel(isDbNullLabel);
                    il.Emit(OpCodes.Pop);
                    il.Emit(OpCodes.Pop);

                    if (first && returnNullIfFirstMissing)
                    {
                        il.Emit(OpCodes.Pop);
                        il.Emit(OpCodes.Ldnull);
                        il.Emit(OpCodes.Stloc_1);
                        il.Emit(OpCodes.Br, allDone);
                    }
                    il.MarkLabel(finishLabel);
                    first = false;
                }
                index += 1;
            }
            il.Emit(type.IsValueType ? OpCodes.Pop : OpCodes.Stloc_1);
            il.MarkLabel(allDone);
            il.BeginCatchBlock(typeof(Exception));
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Call, typeof(DataUtils).GetMethod("ThrowDataException"), null);
            il.EndExceptionBlock();
            il.Emit(OpCodes.Ldloc_1);
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
            il.Emit(OpCodes.Ret);
            return (Func<IDataReader, object>)dm.CreateDelegate(typeof(Func<IDataReader, object>));
        }
        static MethodInfo GetOperator(Type from, Type to)
        {
            if (to == null) return null;
            MethodInfo[] fromMethods, toMethods;
            return ResolveOperator(fromMethods = from.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(toMethods = to.GetMethods(BindingFlags.Static | BindingFlags.Public), from, to, "op_Implicit")
                ?? ResolveOperator(fromMethods, from, to, "op_Explicit")
                ?? ResolveOperator(toMethods, from, to, "op_Explicit");

        }
        static MethodInfo ResolveOperator(MethodInfo[] methods, Type from, Type to, string name)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].Name != name || methods[i].ReturnType != to) continue;
                var args = methods[i].GetParameters();
                if (args.Length != 1 || args[0].ParameterType != from) continue;
                return methods[i];
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public class FastExpando : System.Dynamic.DynamicObject, IDictionary<string, object>
        {
            IDictionary<string, object> data;
            public IDictionary<string, object> Data
            {
                get { return data; }
                set { data = value; }
            }
            public static FastExpando Attach(IDictionary<string, object> data)
            {
                return new FastExpando { data = data };
            }
            public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
            {
                data[binder.Name] = value;
                return true;
            }
            public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
            {
                return data.TryGetValue(binder.Name, out result);
            }
            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return data.Keys;
            }
            void IDictionary<string, object>.Add(string key, object value)
            {
                throw new NotImplementedException();
            }
            bool IDictionary<string, object>.ContainsKey(string key)
            {
                return data.ContainsKey(key);
            }
            ICollection<string> IDictionary<string, object>.Keys
            {
                get { return data.Keys; }
            }
            bool IDictionary<string, object>.Remove(string key)
            {
                throw new NotImplementedException();
            }
            bool IDictionary<string, object>.TryGetValue(string key, out object value)
            {
                return data.TryGetValue(key, out value);
            }
            ICollection<object> IDictionary<string, object>.Values
            {
                get { return data.Values; }
            }
            object IDictionary<string, object>.this[string key]
            {
                get
                {
                    return data[key];
                }
                set
                {
                    if (!data.ContainsKey(key))
                    {
                        throw new NotImplementedException();
                    }
                    data[key] = value;
                }
            }
            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }
            void ICollection<KeyValuePair<string, object>>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return data.Contains(item);
            }

            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                data.CopyTo(array, arrayIndex);
            }
            int ICollection<KeyValuePair<string, object>>.Count
            {
                get { return data.Count; }
            }
            bool ICollection<KeyValuePair<string, object>>.IsReadOnly
            {
                get { return true; }
            }
            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }
            IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
            {
                return data.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return data.GetEnumerator();
            }
        }

        class FastExpandoDescriptionProvider : TypeDescriptionProvider
        {
            public FastExpandoDescriptionProvider() : base() { }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new FastExpandoCustomTypeDescriptor(objectType, instance);
            }
        }

        class FastExpandoCustomTypeDescriptor : CustomTypeDescriptor
        {
            public FastExpandoCustomTypeDescriptor(Type objectType, object instance)
                : base()
            {
                if (instance != null)
                {
                    var tmp = (FastExpando)instance;
                    var names = tmp.GetDynamicMemberNames();
                    foreach (var name in names)
                    {
                        customFields.Add(new DynamicPropertyDescriptor(name, instance));
                    }
                }
            }
            List<PropertyDescriptor> customFields = new List<PropertyDescriptor>();
            public override PropertyDescriptorCollection GetProperties()
            {
                return new PropertyDescriptorCollection(customFields.ToArray());
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return new PropertyDescriptorCollection(customFields.ToArray());
            }
        }
        class DynamicPropertyDescriptor : PropertyDescriptor
        {
            Type propertyType = typeof(object);
            public DynamicPropertyDescriptor(string name, object instance)
                : base(name, null)
            {
                var obj = (IDictionary<string, object>)instance;
                propertyType = obj[name].GetType();
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get
                {
                    return typeof(FastExpando);
                }
            }

            public override object GetValue(object component)
            {
                IDictionary<string, object> obj = (IDictionary<string, object>)component;
                return obj[Name];
            }

            public override bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return propertyType;
                }
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                IDictionary<string, object> obj = (IDictionary<string, object>)component;
                obj[Name] = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }

        private static Func<IDataReader, object> GetDynamicDeserializer(IDataRecord reader, int startBound, int length, bool returnNullIfFirstMissing)
        {
            var fieldCount = reader.FieldCount;
            if (length == -1)
            {
                length = fieldCount - startBound;
            }

            if (fieldCount <= startBound)
            {
                throw new ArgumentException("fieldCount <= startBound", "splitOn");
            }
            return
                 r =>
                 {
                     IDictionary<string, object> row = new Dictionary<string, object>(length);
                     for (var i = startBound; i < startBound + length; i++)
                     {
                         var tmp = r.GetValue(i);
                         tmp = tmp == DBNull.Value ? null : tmp;
                         row[r.GetName(i)] = tmp;
                         if (returnNullIfFirstMissing && i == startBound && tmp == null)
                         {
                             return null;
                         }
                     }
                     return FastExpando.Attach(row);
                 };
        }
        private static int collect;
        private const int COLLECT_PER_ITEMS = 1000, COLLECT_HIT_COUNT_MIN = 0;
        public class CacheInfo
        {
            public Func<IDataReader, object> Deserializer { get; set; }
            public Func<IDataReader, object>[] OtherDeserializers { get; set; }
            public Action<IDbCommand, object> ParamReader { get; set; }
            private int hitCount;
            public int GetHitCount() { return Interlocked.CompareExchange(ref hitCount, 0, 0); }
            public void RecordHit() { Interlocked.Increment(ref hitCount); }
        }
        /// <summary>
        /// Emit转换DataReader为对象列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> ReaderToEnumerable<T>(IDataReader reader)
        {
            var info = new CacheInfo
            {
                Deserializer = GetDeserializer(typeof(T), reader, 0, -1, false)
            };
            while (reader.Read())
            {
                dynamic next = info.Deserializer(reader);
                yield return (T)next;
            }
        }

        /// <summary>
        /// Emit转换DataReader为单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static T ReaderToModel<T>(IDataReader reader)
        {
            var info = new CacheInfo
            {
                Deserializer = GetDeserializer(typeof(T), reader, 0, -1, false)
            };

            if (reader.Read())
            {
                dynamic next = info.Deserializer(reader);
                return (T)next;
            }
            else
            {
                return default(T);
            }
            
        }
    }
}