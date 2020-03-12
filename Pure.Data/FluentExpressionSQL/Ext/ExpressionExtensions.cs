using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace FluentExpressionSQL
{
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// returns the property name including the object path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static string GetFullPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        /// <summary>
        /// gets the PropertyInfo object from an expression pointing to the associated property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<T, TProperty>(this Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
            {
                throw new ArgumentException("The expression does not point to a property.");
            }

            var propertyInfo = memberExp.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The expression does not point to a property.");
            }

            return propertyInfo;
        }

        /// <summary>
        /// tries to find the member expression regardless of conversions
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="memberExp"></param>
        /// <returns></returns>
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the current expression is a convert expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }

        #region 获取Expression的值
        public static object GetValueOfMemberExpression(this MemberExpression member)
        {
            try
            {
                var objectMember = Expression.Convert(member, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                return getter();
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        public static bool TryGetValueOfMethodCallExpression(this MethodCallExpression expression, out object val, params object[] args)
        {
            try
            {
                object result = Expression.Lambda(expression).Compile().DynamicInvoke(args);
                val = result;
                return true;
            }
            catch (Exception)
            {
                val = null;
                return false;
            }

        }
        

        public static bool TryGetValueOfMemberExpression(this MemberExpression member, out object val)
        {
            try
            {
                var objectMember = Expression.Convert(member, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                val = getter();
                return true;
            }
            catch (Exception)
            {
                val = null;
                return false;
            }

        }

        public static bool TryGetValueOfMemberExpressionWithFormat(this MemberExpression member, SqlPack sqlpack ,out object val, bool needFormat)
        {
            try
            {

                if (member != null && member.Member.MemberType == MemberTypes.Field) //局部变量
                {
                    var value = member.Expression.GetValueOfExpression(sqlpack, needFormat);

                    var memberInfoValue = member.Member.GetPropertyOrFieldValue(value);
                    val = sqlpack.SqlDialectProvider.FormatValue(memberInfoValue, needFormat);

                    return true;

                }

                var objectMember = Expression.Convert(member, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                val = getter();
                Type typeOfValue = null;
                if (val != null)
                {
                    typeOfValue = val.GetType();
                }
                if (IsListType(typeOfValue))
                {
                    var sb = new StringBuilder();
                    foreach (var item in val as IEnumerable)
                    {
                        sb.AppendFormat("{0},", FormatValue(item, sqlpack, needFormat));
                    }
                    if (sb.Length == 0)
                    {
                        val = "";
                        return true;
                    }
                    val= sb.Remove(sb.Length - 1, 1).ToString();
                }
                else
                {
                    val= FormatValue(val, sqlpack, needFormat);
                }

                return true;
            }
            catch (Exception)
            {
                val = null;
                return false;
            }

        }


         

        private static object FormatValue(object value, SqlPack sqlpack, bool needFormat )
        {
            //if (value is string)
            //{
            //     return string.Format("'{0}'", value);
            //}
            //if (value is DateTime)
            //{
            //    return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
            //}
            
            return sqlpack.SqlDialectProvider.FormatValue(value, needFormat);
            
        }
        private static string ConvertExpressionTypeToString(ExpressionType nodeType, bool useIs = false)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Or:
                    return " OR ";
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.NotEqual:
                    if (useIs)
                    {
                        return " IS NOT ";
                    }
                    else
                    {
                        return "!=";
                    }

                case ExpressionType.Equal:
                    if (useIs)
                    {
                        return " IS ";
                    }
                    else
                    {
                        return "=";
                    }
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    return "";
            }
        }

       

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
        /// 是否列表类型
        /// </summary>
        /// <param name="typeOfValue"></param>
        /// <returns></returns>
        public static bool IsListType(this Type typeOfValue)
        {
            if (typeOfValue == null)
            {
                return false;
            }
            if (typeOfValue != typeof(string) && (typeOfValue == typeof(IEnumerable)  || typeOfValue == typeof(IList) || typeOfValue == typeof(Array)
                 || IsAssignableToGenericType(typeOfValue, typeof(IList<>)) || IsAssignableToGenericType(typeOfValue, typeof(IEnumerable<>)) 
                 
                 ))
            {
                return true;
            }

            return false;
        }
        public static object GetValueWhenBoolToInt(this Expression body, SqlPack sqlpack, bool needFormat = true)
        {

            if (body is ConstantExpression)
            {
                var conExpValue = ((ConstantExpression)body).Value;
                if (conExpValue is bool)
                {
                    return Convert.ToBoolean(conExpValue) ? 1 : 0;
                }
            }

            return body.GetValueOfExpression(sqlpack, needFormat);

        }
        public static int ConvertBoolToInt(this object v)
        {
            if (v == null)
            {
                return 0;
            }
            if (v is bool || v is Boolean)
            {
                bool bv = Convert.ToBoolean(v);
                if (bv == true)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            return 0;
        }

        public static bool CanGetValue(this Expression body )
        {
            if (body == null)
            {
                return false;
            }

            if (body is ConstantExpression)
            {
                return true;
            }

            return false;
        }
        //public static object GetValueOfExpressionWithoutFormat(this Expression body )
        //{
        //    if (body == null)
        //    {
        //        return string.Empty;
        //    }
        //    if (body is ConstantExpression)
        //    {
        //        return ((ConstantExpression)body).Value;
        //    }
        //    if (body is MemberExpression)
        //    {
        //        var member = ((MemberExpression)body);
                
        //        var value = GetValueOfMemberExpression(member);
        //        Type typeOfValue = null;
        //        if (value != null)
        //        {
        //            typeOfValue = value.GetType();
        //        }

        //        return (value);

        //    }
        //    if (body is UnaryExpression)
        //    {
        //        return GetValueOfExpressionWithoutFormat(((UnaryExpression)body).Operand);
        //    }
        //    if (body is NewExpression)
        //    {
        //        var args = ((NewExpression)body).Arguments;
        //        //判断Select设置值  Select<UserInfo>(u => new {  DT3=new DateTime(2015,10,11) }).
        //        if (args.Count > 0)
        //        {
        //            return GetValueOfExpressionWithoutFormat(args[0]);
        //        }
        //        //foreach (var item in ((NewExpression)body).Arguments)
        //        //{
        //        //    GetValueOfExpression(item, sqlpack);
        //        //}
        //    }
        //    //if (body is BinaryExpression)
        //    //{
        //    //    var binary = body as BinaryExpression;
        //    //    if (binary.Right.GetValueOfExpressionWithoutFormat() == null && (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual))
        //    //    {
        //    //        return string.Format("({0}{1}{2})", GetValueOfExpression(binary.Left, sqlpack),
        //    //        ConvertExpressionTypeToString(binary.NodeType, true),
        //    //        "NULL");
        //    //    }
        //    //    else
        //    //    {
        //    //        return string.Format("({0}{1}{2})", GetValueOfExpression(binary.Left, sqlpack),
        //    //       ConvertExpressionTypeToString(binary.NodeType),
        //    //       GetValueOfExpression(binary.Right, sqlpack));
        //    //    }

        //    //}
        //    //if (body is MethodCallExpression)
        //    //{
        //    //    var method = body as MethodCallExpression;

        //    //    return MethodCallFluentExpressionSQL.GetMethodResult(method, sqlpack);

        //    //}
        //    if (body is LambdaExpression)
        //    {
        //        return GetValueOfExpressionWithoutFormat(((LambdaExpression)body).Body);
        //    }
        //    return "";
        //}

        public static object GetValueOfExpression(this Expression body, SqlPack sqlpack, bool needFormat = true)
        {
            if (body == null)
            {
                return string.Empty;
            }
            if (body is ConstantExpression)
            {
                return FormatValue(((ConstantExpression)body).Value, sqlpack, needFormat);
            }
            if (body is MemberExpression)
            {
                var member = ((MemberExpression)body);

              
                if (member.Member.MemberType == MemberTypes.Property  )
                {

                    if (member.Member.DeclaringType == ResolveConstants.TypeOfDateTime)//日期转换
                    {
                        if (member.Member == ResolveConstants.PropertyInfo_DateTime_Now ||
                            member.Member == ResolveConstants.PropertyInfo_DateTime_UtcNow ||
                            member.Member == ResolveConstants.PropertyInfo_DateTime_Today)//只能转换为常量的SQL 字符
                        {
                            object valueDatetime = sqlpack.SqlDialectProvider.ConvertDateTime(member.Member,  null);
                            return valueDatetime;
                        }
                         
                    }

                   

                    string tableName = GetTableNameByExpression(member, sqlpack);
                    sqlpack.SetTableAlias(tableName);
                    string tableAlias = sqlpack.GetTableAlias(tableName);
                    if (!string.IsNullOrWhiteSpace(tableAlias))
                    {
                        tableAlias += ".";
                    }
                    return tableAlias + member.Member.Name;

                    //return member.Member.Name;
                }
                var value = GetValueOfMemberExpression(member);
                Type typeOfValue = null;
                if (value != null)
                {
                    typeOfValue = value.GetType();
                }

                if (IsListType(typeOfValue))
                {
                    var valueList = value as IEnumerable;

                    var sb = new StringBuilder();
                    foreach (var item in value as IEnumerable)
                    {
                        sb.AppendFormat("{0},", FormatValue(item, sqlpack, needFormat));
                    }
                    if (sb.Length == 0)
                    {
                        return "";
                    }
                    return sb.Remove(sb.Length - 1, 1).ToString();
                }
                else
                {
                    return FormatValue(value, sqlpack, needFormat);
                }
            }
            if (body is UnaryExpression)
            {
                return GetValueOfExpression(((UnaryExpression)body).Operand, sqlpack, needFormat);
            }
            if (body is NewExpression)
            {
                var args = ((NewExpression)body).Arguments;
                //判断Select设置值  Select<UserInfo>(u => new {  DT3=new DateTime(2015,10,11) }).
                if (args.Count > 0)
                {
                    return GetValueOfExpression(args[0], sqlpack, needFormat);
                }
                //foreach (var item in ((NewExpression)body).Arguments)
                //{
                //    GetValueOfExpression(item, sqlpack);
                //}
            }
            if (body is BinaryExpression)
            {
                var binary = body as BinaryExpression;
                if (binary.Right.GetValueOfExpression(sqlpack, needFormat) == null && (binary.NodeType == ExpressionType.Equal || binary.NodeType == ExpressionType.NotEqual))
                {
                    return string.Format("({0}{1}{2})", GetValueOfExpression(binary.Left, sqlpack, needFormat),
                    ConvertExpressionTypeToString(binary.NodeType, true),
                    "NULL");
                }
                else
                {
                    return string.Format("({0}{1}{2})", GetValueOfExpression(binary.Left, sqlpack, needFormat),
                   ConvertExpressionTypeToString(binary.NodeType),
                   GetValueOfExpression(binary.Right, sqlpack, needFormat));
                }
               
            }
            if (body is MethodCallExpression)
            {
                var method = body as MethodCallExpression;

                return MethodCallFluentExpressionSQL.GetMethodResult(method, sqlpack, needFormat);

                //return string.Format("({0} IN ({1}))", GetValueOfExpression(method.Arguments[0], sqlpack),
                //    GetValueOfExpression(method.Object, sqlpack));
            }
            if (body is LambdaExpression)
            {
                return GetValueOfExpression(((LambdaExpression)body).Body, sqlpack, needFormat);
            }
            return "";
        }




      
        #endregion
        public static string GetTableNameByExpression(this ParameterExpression expression, SqlPack sqlPack)
        {
            if (expression != null)
            {
                string tableName = "";
                tableName = sqlPack.GetTableName((expression.Type));

                return tableName;
            }
            return "";
        }
        public static string GetTableNameByExpression(this MemberExpression expression, SqlPack sqlPack)
        {
            if (expression != null)
            {
                string tableName = "";
                if (expression.Expression != null)
                {
                    tableName = sqlPack.GetTableName((expression.Expression.Type));
                }
                else
                {
                    tableName = sqlPack.GetTableName(expression.Member.DeclaringType.UnderlyingSystemType);
                }
                return tableName;
            }
            return "";
        }
         public static string GetColumnName(this MemberExpression expression, SqlPack sqlPack)
        {
            if (expression == null)
            {
                return "";
            }
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            string colName = tableAlias + expression.Member.Name;

            return colName;
        }

        /// <summary>
        /// 返回值或者列名
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlPack"></param>
        /// <returns></returns>
         public static object GetValueOrColumnName(this Expression expression, SqlPack sqlPack, bool needFormat = true)
         {
             if (expression == null)
             {
                 return "";
             }
             object result = "";
             if (expression is MemberExpression)
             {
                 var memberExp = expression as MemberExpression;
                 if (memberExp != null && memberExp.Member.MemberType == MemberTypes.Field) //局部变量
                 {
                     var value = memberExp.Expression.GetValueOfExpression(sqlPack, needFormat);

                     var memberInfoValue = memberExp.Member.GetPropertyOrFieldValue(value);
                     result = sqlPack.SqlDialectProvider.FormatValue(memberInfoValue, needFormat);

                 }
                 else if (memberExp != null && memberExp.Member.MemberType == MemberTypes.Property) //属性值
                 {
                     object val = null;
                     if (memberExp.TryGetValueOfMemberExpression(out val))
                     {
                        
                         result = sqlPack.SqlDialectProvider.FormatValue(val, needFormat);
                     }
                     else
                     {
                         result = memberExp.GetColumnName(sqlPack);

                     }
                 }
                 else
                 {
                     result = memberExp.GetColumnName(sqlPack);
                 }
             }
             else
             {
                 result = expression.GetValueOfExpression(sqlPack, needFormat);
             }
             return result;
         }

         private static System.Collections.Generic.List<ExpressionType> EndTokenList = new System.Collections.Generic.List<ExpressionType>(){
        ExpressionType.MemberAccess,ExpressionType.Constant, ExpressionType.Parameter, ExpressionType.Convert
        };
        /// <summary>
        /// 是否是结束的表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
         public static bool IsEndToken(this Expression expression)
        {
            return EndTokenList.Contains(expression.NodeType);
        }

        #region Check
        public static BinaryExpression PreEvaluateBinary(this BinaryExpression b, object left, object right)
        {
            var visitedBinaryExp = b;

            if (IsParameterAccess(b.Left) || IsParameterAccess(b.Right))
            {
                var eLeft = !IsParameterAccess(b.Left) ? b.Left : Expression.Constant(left, b.Left.Type);
                var eRight = !IsParameterAccess(b.Right) ? b.Right : Expression.Constant(right, b.Right.Type);
                if (b.NodeType == ExpressionType.Coalesce)
                    visitedBinaryExp = Expression.Coalesce(eLeft, eRight, b.Conversion);
                else
                    visitedBinaryExp = Expression.MakeBinary(b.NodeType, eLeft, eRight, b.IsLiftedToNull, b.Method);
            }

            return visitedBinaryExp;
        }

        /// <summary>
        /// Determines whether the expression is the parameter inside MemberExpression which should be compared with TrueExpression.
        /// </summary>
        /// <returns>Returns true if the specified expression is the parameter inside MemberExpression which should be compared with TrueExpression;
        /// otherwise, false.</returns>
        public static bool IsBooleanComparison(this Expression e)
        {
            if (!(e is MemberExpression)) return false;

            var m = (MemberExpression)e;

            if (m.Member.DeclaringType.IsNullableType() &&
                m.Member.Name == "HasValue") //nameof(Nullable<bool>.HasValue)
                return false;

            return IsParameterAccess(m);
        }
 
        /// <summary>
        /// Determines whether the expression is the parameter.
        /// </summary>
        /// <returns>Returns true if the specified expression is parameter;
        /// otherwise, false.</returns>
        public static bool IsParameterAccess(this Expression e)
        {
            return CheckExpressionForTypes(e, new[] { ExpressionType.Parameter });
        }

        /// <summary>
        /// Determines whether the expression is a Parameter or Convert Expression.
        /// </summary>
        /// <returns>Returns true if the specified expression is parameter or convert;
        /// otherwise, false.</returns>
        public static bool IsParameterOrConvertAccess(Expression e)
        {
            return CheckExpressionForTypes(e, new[] { ExpressionType.Parameter, ExpressionType.Convert });
        }

        public static bool CheckExpressionForTypes(Expression e, ExpressionType[] types)
        {
            while (e != null)
            {
                if (types.Contains(e.NodeType))
                {
                    var subUnaryExpr = e as UnaryExpression;
                    var isSubExprAccess = subUnaryExpr != null ?subUnaryExpr.Operand is IndexExpression : false;
                    if (!isSubExprAccess)
                        return true;
                }

                var binaryExpr = e as BinaryExpression;
                if (binaryExpr != null)
                {
                    if (CheckExpressionForTypes(binaryExpr.Left, types))
                        return true;

                    if (CheckExpressionForTypes(binaryExpr.Right, types))
                        return true;
                }

                var methodCallExpr = e as MethodCallExpression;
                if (methodCallExpr != null)
                {
                    for (var i = 0; i < methodCallExpr.Arguments.Count; i++)
                    {
                        if (CheckExpressionForTypes(methodCallExpr.Arguments[0], types))
                            return true;
                    }
                }

                var unaryExpr = e as UnaryExpression;
                if (unaryExpr != null)
                {
                    if (CheckExpressionForTypes(unaryExpr.Operand, types))
                        return true;
                }

                var condExpr = e as ConditionalExpression;
                if (condExpr != null)
                {
                    if (CheckExpressionForTypes(condExpr.Test, types))
                        return true;

                    if (CheckExpressionForTypes(condExpr.IfTrue, types))
                        return true;

                    if (CheckExpressionForTypes(condExpr.IfFalse, types))
                        return true;
                }

                var memberExpr = e as MemberExpression;
                e = memberExpr != null ? memberExpr.Expression : null;
            }

            return false;
        }

        public static  void Swap(ref object left, ref object right)
        {
            var temp = right;
            right = left;
            left = temp;
        }
        #endregion

        #region 过滤非法字符
        public static string[] IllegalSqlFragmentTokens = { 
            "--", ";--", ";", "%", "/*", "*/", "@@", "@", 
            "char", "nchar", "varchar", "nvarchar",
            "alter", "begin", "cast", "create", "cursor", "declare", "delete",
            "drop", "end", "exec", "execute", "fetch", "insert", "kill",
            "open", "select", "sys", "sysobjects", "syscolumns", "table", "update" };
        public static object SqlVerifyFragment2(this object o)
        {
            if (o == null)
            {
                return o;
            }
            if (o is string || o is String)
            {
                return SqlVerifyFragment(o.ToString(), IllegalSqlFragmentTokens);

            }
            return o;
        }
        public static string SqlVerifyFragment(this string sqlFragment)
        {
            return SqlVerifyFragment(sqlFragment, IllegalSqlFragmentTokens);
        }

        public static string SqlVerifyFragment(this string sqlFragment, IEnumerable<string> illegalFragments)
        {
            if (sqlFragment == null)
                return null;

            var fragmentToVerify = sqlFragment
                .StripQuotedStrings('\'')
                .StripQuotedStrings('"')
                .StripQuotedStrings('`')
                .ToLower();

            //20200312  处理91445122MA518END13 91440300MA5FENDX1D  这种情况会报错 END的问题
            //foreach (var illegalFragment in illegalFragments)
            //{
            //    if ((fragmentToVerify.IndexOf(illegalFragment, StringComparison.Ordinal) >= 0))
            //        throw new ArgumentException("检测SQL中存在非法字符串: " + sqlFragment);
            //}

            return sqlFragment;
        }
        public static string StripQuotedStrings(this string text, char quote = '\'')
        {
            var sb = new StringBuilder();
            var inQuotes = false;
            foreach (var c in text)
            {
                if (c == quote)
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes)
                    sb.Append(c);
            }

            return sb.ToString();
        }

       
        #endregion

    }
}
