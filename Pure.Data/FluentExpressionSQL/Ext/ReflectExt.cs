using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace  FluentExpressionSQL
{
    internal static class ReflectExt
    {
        public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
        {
            return GetFieldsAndProperties(typeof(T), bindingAttr);
        }

        public static List<MemberInfo> GetFieldsAndPropertiesForClasses(Type type)
        {
            if (type.GetTypeInfo().IsValueType || type == typeof(string) || type == typeof(byte[]) || type == typeof(Dictionary<string, object>) || type.IsArray)
                return new List<MemberInfo>();

            return GetFieldsAndProperties(type);
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type)
        {
            return GetFieldsAndProperties(type, BindingFlags.Instance | BindingFlags.Public);
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> targetMembers = new List<MemberInfo>();

            targetMembers.AddRange(type.GetFields(bindingAttr).Where(x => !x.IsInitOnly).ToArray());
            targetMembers.AddRange(type.GetProperties(bindingAttr));

            return targetMembers;
        }
        public static bool IsValueType(this Type type)
        {
#if DNXCORE50
            return typeof(ValueType).IsAssignableFrom(type) && type != typeof(ValueType);
#else
            return type.IsValueType;
#endif
        }
        public static bool IsEnum(this Type type)
        {
#if DNXCORE50
            return typeof(Enum).IsAssignableFrom(type) && type != typeof(Enum);
#else
            return type.IsEnum;
#endif
        }

  


        public static bool IsField(this MemberInfo member)
        {
            return member is FieldInfo;
        }

        public static MethodInfo GetSetMethodOnDeclaringType(this PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod(true);
            return methodInfo ?? propertyInfo
                                    .DeclaringType
                                    .GetProperty(propertyInfo.Name)
                                    .GetSetMethod(true);
        }

        public static bool IsOrHasGenericInterfaceTypeOf(this Type type, Type genericTypeDefinition)
        {
            return type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition) != null;
        }

        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }

        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.GetTypeInfo().IsGenericType)
                    return type;

                type = type.GetTypeInfo().BaseType;
            }
            return null;
        }

        public static Type GetTypeWithInterfaceOf(this Type type, Type interfaceType)
        {
            if (type == interfaceType) return interfaceType;

            foreach (var t in type.GetInterfaces())
            {
                if (t == interfaceType)
                    return t;
            }

            return null;
        }

        public static bool IsOfGenericType(this Type instanceType, Type genericType)
        {
            Type type = instanceType;
            while (type != null)
            {
                if (type.GetTypeInfo().IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                type = type.GetTypeInfo().BaseType;
            }

            foreach (var i in instanceType.GetInterfaces())
            {
                if (i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 根据枚举值解析枚举项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(this object value)
        {
            var type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (value is int || value is float || value is double || value is decimal)
            {
                value = Convert.ChangeType(value, Enum.GetUnderlyingType(type), System.Globalization.CultureInfo.InvariantCulture);
            }
            return (T)Enum.ToObject(type, value);
        }

        public static bool IsNullableType(this Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        public static object GetDefaultValue(this Type targetType)
        {
            

            if (targetType == typeof(string) || targetType == typeof(String) )
            {
                return "";
            }
            if (targetType.IsNullableType())
            {
                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                NullableConverter nullableConverter = new NullableConverter(targetType);
                //将convertsionType转换为nullable对的基础基元类型
                targetType = nullableConverter.UnderlyingType;
                return GetDefaultValue(targetType);

            }
          
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null; 
        }
        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }
        public static Type GetMemberInfoType(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).PropertyType;
            if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).FieldType;
            if (member is MethodInfo)
                return ((MethodInfo)member).ReturnType;
            if (member is ConstructorInfo)
                return ((ConstructorInfo)member).ReflectedType;

            return null;
        }

        public static Type GetPropertyOrFieldType(this MemberInfo propertyOrField)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).PropertyType;
            if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).FieldType;

            throw new NotSupportedException();
        }

        public static void SetPropertyOrFieldValue(this MemberInfo propertyOrField, object obj, object value)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                ((PropertyInfo)propertyOrField).SetValue(obj, value, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                ((FieldInfo)propertyOrField).SetValue(obj, value);

            throw new ArgumentException();
        }

        public static object GetPropertyOrFieldValue(this MemberInfo propertyOrField, object obj)
        {
            if (propertyOrField.MemberType == MemberTypes.Property)
                return ((PropertyInfo)propertyOrField).GetValue(obj, null);
            else if (propertyOrField.MemberType == MemberTypes.Field)
                return ((FieldInfo)propertyOrField).GetValue(obj);

            throw new ArgumentException();
        }

        public static MemberInfo AsReflectedMemberOf(this MemberInfo memberInfo, Type type)
        {
            if (memberInfo.ReflectedType != type)
            {
                MemberInfo tempMember = null;
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    tempMember = type.GetProperty(memberInfo.Name);
                }
                else if (memberInfo.MemberType == MemberTypes.Field)
                {
                    tempMember = type.GetField(memberInfo.Name);
                }

                if (tempMember != null)
                    memberInfo = tempMember;
            }

            return memberInfo;
        }

    }
}
