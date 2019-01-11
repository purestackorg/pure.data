using Pure.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Pure.Data
{
    public static class ReflectionHelper
    {

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fullName">命名空间.类型名</param>
        /// <param name="assemblyName">程序集</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string fullName, string assemblyName)
        {
            //string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
            //Type o = Type.GetType(path);//加载类型
            //object obj = Activator.CreateInstance(o, true);//根据类型创建实例
            //return (T)obj;//类型转换并返回

             
            //此为第一种写法
            object ect = Assembly.Load(assemblyName).CreateInstance(fullName, true);//加载程序集，创建程序集里面的 命名空间.类型名 实例
            return (T)ect;//类型转换并返回
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T">要创建对象的类型</typeparam>
        /// <param name="assemblyName">类型所在程序集名称</param>
        /// <param name="nameSpace">类型所在命名空间</param>
        /// <param name="className">类型名</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string assemblyName, string nameSpace, string className)
        {
            try
            {
                string fullName = nameSpace + "." + className;//命名空间.类型名
                //此为第一种写法
                object ect = Assembly.Load(assemblyName).CreateInstance(fullName);//加载程序集，创建程序集里面的 命名空间.类型名 实例
                return (T)ect;//类型转换并返回
                //下面是第二种写法
                //string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
                //Type o = Type.GetType(path);//加载类型
                //object obj = Activator.CreateInstance(o, true);//根据类型创建实例
                //return (T)obj;//类型转换并返回
            }
            catch
            {
                //发生异常，返回类型的默认值
                return default(T);
            }
        }

        private static List<Type> _simpleTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };
        
        public static MemberInfo GetProperty(LambdaExpression lambda)
        {
            Expression expr = lambda;
            for (; ; )
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.MemberAccess:
                        MemberExpression memberExpression = (MemberExpression)expr;
                        MemberInfo mi = memberExpression.Member;
                        return mi;
                    default:
                        return null;
                }
            }
        }

        public static IDictionary<string, object> GetObjectValues(object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null)
            {
                return result;
            }


            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(obj, null);
                result[name] = value;
            }

            return result;
        }

        public static string AppendStrings(this IEnumerable<string> list, string seperator = ", ")
        {
            return list.Aggregate(
                new StringBuilder(),
                (sb, s) => (sb.Length == 0 ? sb : sb.Append(seperator)).Append(s),
                sb => sb.ToString());
        }

        public static bool IsSimpleType(Type type)
        {
            Type actualType = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                actualType = type.GetGenericArguments()[0];
            }

            return _simpleTypes.Contains(actualType);
        }
        public static object GetDefaultValueForType(Type targetType)
        {
            var isNullable =  IsNullableType(targetType);
            if (isNullable)
            {
                return null;
            }
            else
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            }

        }

        /// <summary>
        /// 获取所有属性名列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static List<string> GetAllPropertyNames<T>(params Expression<Func<T, object>>[] ignoreProperties) where T : class
        {

            List<string> nameList = new List<string>();

            if (ignoreProperties != null && ignoreProperties.Length > 0)
            {
                PropertyInfo propertyInfo = null;
                foreach (Expression<Func<T, object>> expression in ignoreProperties)
                {
                    propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        nameList.Add(propertyInfo.Name);
                    }

                }
            }

            return nameList;
        }
        /// <summary>
        /// 获取当前非空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }
        /// <summary>
        /// 获取当前可空类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableType(this Type type)
        {
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(new Type[] { type });
            }
            return type;
        }

        public static bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.
              GetGenericTypeDefinition().Equals
              (typeof(Nullable<>)));
        }
        public static string GetParameterName(this IDictionary<string, object> parameters, string parameterName, ISqlDialect sqlDialect)
        {
            return string.Format("{0}{1}_{2}",sqlDialect.ParameterPrefix,parameterName, parameters.Count);
        }

        public static string SetParameterName(this IDictionary<string, object> parameters, string parameterName, object value,ISqlDialect sqlDialect)
        {
            string name = parameters.GetParameterName(parameterName, sqlDialect);
            parameters.Add(name, value);
            return name;
        }

        #region Ext
        /// <summary>
        /// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
        /// </summary>
        /// <param name="givenType">Type to check</param>
        /// <param name="genericType">Generic type</param>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenType.BaseType, genericType);
        }

        /// <summary>
        /// Gets a list of attributes defined for a class member and it's declaring type including inherited attributes.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        public static List<TAttribute> GetAttributesOfMemberAndDeclaringType<TAttribute>(MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            //Add attributes on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            //Add attributes on the class
            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(memberInfo.DeclaringType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            return attributeList;
        }

        public static List<TAttribute> GetCustomAttributes<TAttribute>(this Type type)
       where TAttribute : Attribute
        {
            var attributeList = new List<TAttribute>();

            //Add attributes on the member
            if (type.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(type.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            //Add attributes on the class
            if (type.DeclaringType != null && type.DeclaringType.IsDefined(typeof(TAttribute), true))
            {
                attributeList.AddRange(type.DeclaringType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>());
            }

            return attributeList;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns null if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrNull<TAttribute>(MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), true))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();
            }

            //Get attribute from class
            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsDefined(typeof(TAttribute), true))
            {
                return memberInfo.DeclaringType.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().First();
            }

            return null;
        }


        #endregion

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public delegate object FastInvokeHandler(object target, object[] parameters);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate object FastCreateInstanceHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public delegate object FastPropertyGetHandler(object target);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="parameter"></param>
    public delegate void FastPropertySetHandler(object target, object parameter);

    /// <summary>
    /// 
    /// </summary>
    public static class DynamicCalls
    {
        /// <summary>
        /// 用于存放GetMethodInvoker的Dictionary
        /// </summary>
        private static Dictionary<MethodInfo, FastInvokeHandler> dictInvoker = new Dictionary<MethodInfo, FastInvokeHandler>();



        /// <summary>
        /// 用于存放GetInstanceCreator的Dictionary
        /// </summary>
        private static Dictionary<Type, FastCreateInstanceHandler> dictCreator = new Dictionary<Type, FastCreateInstanceHandler>();



        /// <summary>
        /// 用于存放GetPropertyGetter的Dictionary
        /// </summary>
        private static Dictionary<PropertyInfo, FastPropertyGetHandler> dictGetter = new Dictionary<PropertyInfo, FastPropertyGetHandler>();

        public static FastPropertyGetHandler GetPropertyGetter(PropertyInfo propInfo)
        {
            lock (dictGetter)
            {
                if (dictGetter.ContainsKey(propInfo)) return (FastPropertyGetHandler)dictGetter[propInfo];

                DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object) }, propInfo.DeclaringType.Module);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);

                ilGenerator.EmitCall(OpCodes.Callvirt, propInfo.GetGetMethod(), null);

                EmitBoxIfNeeded(ilGenerator, propInfo.PropertyType);

                ilGenerator.Emit(OpCodes.Ret);

                FastPropertyGetHandler getter = (FastPropertyGetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertyGetHandler));

                dictGetter.Add(propInfo, getter);

                return getter;
            }
        }

        /// <summary>
        /// 用于存放SetPropertySetter的Dictionary
        /// </summary>
        private static Dictionary<PropertyInfo, FastPropertySetHandler> dictSetter = new Dictionary<PropertyInfo, FastPropertySetHandler>();

        public static FastPropertySetHandler GetPropertySetter(PropertyInfo propInfo)
        {
            lock (dictSetter)
            {
                if (dictSetter.ContainsKey(propInfo)) return (FastPropertySetHandler)dictSetter[propInfo];

                DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, null, new Type[] { typeof(object), typeof(object) }, propInfo.DeclaringType.Module);

                ILGenerator ilGenerator = dynamicMethod.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);

                ilGenerator.Emit(OpCodes.Ldarg_1);

                EmitCastToReference(ilGenerator, propInfo.PropertyType);

                ilGenerator.EmitCall(OpCodes.Callvirt, propInfo.GetSetMethod(), null);

                ilGenerator.Emit(OpCodes.Ret);

                FastPropertySetHandler setter = (FastPropertySetHandler)dynamicMethod.CreateDelegate(typeof(FastPropertySetHandler));

                dictSetter.Add(propInfo, setter);

                return setter;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitCastToReference(ILGenerator ilGenerator, System.Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Castclass, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        private static void EmitBoxIfNeeded(ILGenerator ilGenerator, System.Type type)
        {
            if (type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="value"></param>
        private static void EmitFastInt(ILGenerator ilGenerator, int value)
        {
            switch (value)
            {
                case -1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_M1);
                    return;
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    return;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    return;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    return;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    return;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    return;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    return;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    return;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    return;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    return;
            }
            if (value > -129 && value < 128)
            {
                ilGenerator.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                ilGenerator.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }
}