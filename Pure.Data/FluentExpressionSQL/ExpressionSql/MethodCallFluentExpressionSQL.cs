
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentExpressionSQL
{

    class MethodCallFluentExpressionSQL : BaseFluentExpressionSQL<MethodCallExpression>
    {
        private static MethodCallFluentExpressionSQL _Instance = null;

        public static MethodCallFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new MethodCallFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        /// <summary>
        /// 获取方法结果
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlPack"></param>
        /// <returns></returns>
        public static string GetMethodResult(MethodCallExpression expression, SqlPack sqlPack)
        {

            string result = "";

            object methodValue = null;

            if (expression.TryGetValueOfMethodCallExpression(out methodValue))
            {
                //默认执行方法
                
                var str = sqlPack.SqlDialectProvider.FormatValue(methodValue);
                return str !=null ?str.ToString() :"";

            }




            var key = expression.Method;
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }


            Action<MethodCallExpression, SqlPack> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {
                action(expression, sqlPack);
                result = sqlPack.CurrentDbFunctionResult;
                sqlPack.CurrentDbFunctionResult = null;

                return result;
            }


             

            throw new NotImplementedException("无法解析方法" + expression.Method);
        }

        private static object GetValueOfMemberExpression(MemberExpression expression, SqlPack sqlPack)
        {
            object result = "";

            if (expression != null)
            {
                if (expression.Member.MemberType == MemberTypes.Field) //局部变量
                {
                    var value = expression.Expression.GetValueOfExpression(sqlPack);

                    var memberInfoValue = expression.Member.GetPropertyOrFieldValue(value);
                    result = sqlPack.SqlDialectProvider.FormatValue(memberInfoValue);

                }
                else
                {
                    result = expression.GetColumnName(sqlPack);

                }
            }
            return result;
        }

        private static object GetValueOfMethadCallExpression(MethodCallExpression expression, SqlPack sqlPack)
        {
            object result = "";
            if (expression.Object != null)
            {
                if (expression.Object is MemberExpression)
                {
                    var memberExp = expression.Object as MemberExpression;
                    result = GetValueOfMemberExpression(memberExp, sqlPack);

                }
                else if (expression.Object is ConstantExpression)
                {
                    var constantExp = expression.Object as ConstantExpression;
                    if (constantExp != null)
                    {
                        result =  (constantExp.GetValueOfExpression(sqlPack));
                    }
                }
                else if (expression.Object is MethodCallExpression)
                {
                    var constantExp = expression.Object as MethodCallExpression;
                    if (constantExp != null)
                    {
                       
                        result= MethodCallFluentExpressionSQL.GetMethodResult(constantExp, sqlPack);
                    }
                }

            }
            return result;
        }
        private static void AddSelectField(SqlPack sqlPack, string columnName)
        {
            if (sqlPack.CurrentColAlias != null)
            {
                columnName += " " + sqlPack.ColumnAsAliasString + sqlPack.CurrentColAlias;
            }
            sqlPack.SelectFields.Add(columnName);
        }

        static Dictionary<string, Action<MethodCallExpression, SqlPack>> _Methods = new Dictionary<string, Action<MethodCallExpression, SqlPack>>
        {
            {"Like",Like},
            {"LikeLeft",LikeLeft},
            {"LikeRight",LikeRight},
            {"LikeNot",LikeNot},
            {"LikeLeftNot",LikeLeftNot},
            {"LikeRightNot",LikeRightNot},
            {"In",In},
            {"InNot",InNot},
            {"ToString",ToString},

            {"Contains",Contains},
            {"StartsWith",StartsWith},
            {"EndsWith",EndsWith},

            {"Trim",Trim},
            {"TrimStart",TrimStart},
            {"TrimEnd",TrimEnd},
            {"ToUpper",ToUpper},
            {"ToLower",ToLower},
            {"Substring",Substring},
            {"IsNullOrEmpty",IsNullOrEmpty},
            {"IsNullOrWhiteSpace",IsNullOrEmpty},
            //{"CountSQL",Count},
            //{"SumSQL",Sum},
            //{"MaxSQL",Max},
            //{"MinSQL",Min},
            //{"AvgSQL",Avg},
             {"Count",Count},
            {"Sum",Sum},
            {"Max",Max},
            {"Min",Min},
            {"Avg",Avg},

            {"AddYears",AddYears},
            {"AddMonths",AddMonths},
            {"AddDays",AddDays},
            {"AddHours",AddHours},
            {"AddMinutes",AddMinutes},
            {"AddSeconds",AddSeconds},
            {"AddMilliseconds",AddMilliseconds},
            {"NewGuid",NewGuid},
            {"DiffYears",DiffYears},
            {"DiffMonths",DiffMonths},
            {"DiffDays",DiffDays},
            {"DiffHours",DiffHours},
            {"DiffMinutes",DiffMinutes},
            {"DiffSeconds",DiffSeconds},
            {"DiffMilliseconds",DiffMilliseconds},
            {"DiffMicroseconds",DiffMicroseconds},
            {"Abs",Abs},
            {"Round",Round},//四舍五入
            {"Ceiling",Ceiling},//向上取整截取
            {"Floor",Floor},//向下取整截取
            {"Sqrt",Sqrt},
            {"Log",Log},
            {"Pow",Pow},
            {"Sign",Sign},
            {"Truncate",Truncate},
            {"Mod",Mod},
            {"Rand",Rand},
            {"IfNull",IfNull},
            {"Parse",Parse},
        };
        #region Ext Method Resolve
        private static void ToString(MethodCallExpression expression, SqlPack sqlPack)
        {
            //if (expression.Object != null)
            //{
            //    FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            //}

            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Contains(MethodCallExpression expression, SqlPack sqlPack)
        {
             

            MethodInfo method = expression.Method;

            if (method.DeclaringType == typeof(String) || method.DeclaringType == typeof(string))
            {

                if (expression.Object != null)
                {
                    FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
                }
                string result = "";

                var column = expression.Arguments[0].GetValueOrColumnName(sqlPack);

                result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Contains, column);

                sqlPack += " LIKE ";

                sqlPack.CurrentDbFunctionResult = result;
            }
            else
            {
                 
                var declaringType = method.DeclaringType;
                // typeof(IList).IsAssignableFrom(declaringType) || typeof(IEnumerable).IsAssignableFrom(declaringType) || (declaringType.IsGenericType && typeof(ICollection<>).MakeGenericType(declaringType.GetGenericArguments()).IsAssignableFrom(declaringType))
                if (declaringType.IsListType())
                {
                    
                    var memberExp = expression.Object; 
                    FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
                    sqlPack += " IN ";
                    FluentExpressionSQLProvider.In(memberExp, sqlPack);
                     
                }
                else if (method.IsStatic && declaringType == typeof(System.Linq.Enumerable) && expression.Arguments.Count == 2)
                {
                    
                    FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
                    sqlPack += " IN ";
                    FluentExpressionSQLProvider.In(expression.Arguments[0], sqlPack);
                }
                 
            }




            //sqlPack += " LIKE '%' +";
            //FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " + '%'";
        }
        private static void StartsWith(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }

            string result = "";

            var column = expression.Arguments[0].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.StartsWith, column);

            sqlPack += " LIKE ";

            sqlPack.CurrentDbFunctionResult = result;

             
            //sqlPack += " LIKE ";
             
            //FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " + '%'";


        }
        private static void EndsWith(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }

            string result = "";

            var column = expression.Arguments[0].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.EndsWith, column);

            sqlPack += " LIKE ";

            sqlPack.CurrentDbFunctionResult = result;


            //sqlPack += " LIKE '%' +";

            //FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
        }
        private static void ToUpper(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.ToUpper, result);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void ToLower(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.ToLower, result);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void TrimStart(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.TrimStart, result);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void TrimEnd(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.TrimEnd, result);
            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void Trim(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Trim, result);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void Substring(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = GetValueOfMethadCallExpression(expression, sqlPack).ToString();
            if (expression.Arguments.Count > 1)
            {
                result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Substring, result, expression.Arguments[0], expression.Arguments[1]);

            }
            else if (expression.Arguments.Count > 0)
            {
                result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Substring, result, expression.Arguments[0]);

            }
            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void IsNullOrEmpty(MethodCallExpression expression, SqlPack sqlPack)
        {
            var memberExp = expression.Arguments[0] as MemberExpression;

            string result = memberExp.GetColumnName(sqlPack).ToString();
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.IsNullOrEmpty, result);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void Parse(MethodCallExpression expression, SqlPack sqlPack)
        {
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            Type type = expression.Method != null ? expression.Method.ReturnType : typeof(string);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Parse, v, type);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void NewGuid(MethodCallExpression expression, SqlPack sqlPack)
        {

            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.NewGuid, result);

            sqlPack.CurrentDbFunctionResult = result;


        }
        private static void Count(MethodCallExpression expression, SqlPack sqlPack)
        {
            //var startExp = expression.Arguments[0];
            //object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Count, 1);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Sum(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Sum, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Max(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Max, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Min(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Min, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Avg(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Avg, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Abs(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Abs, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Round(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            object precision = null;
            if (expression.Arguments.Count > 1)
            {
                var precisionExp = expression.Arguments[1];
                precision = precisionExp.GetValueOrColumnName(sqlPack);
            }
            
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Round, v, precision);

            sqlPack.CurrentDbFunctionResult = result;
        }
        /// <summary>
        /// 向上取整截取
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlPack"></param>
        private static void Ceiling(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Ceiling, v);

            sqlPack.CurrentDbFunctionResult = result;


        }
        /// <summary>
        /// 向下取整截取
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="sqlPack"></param>
        private static void Floor(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Floor, v);

            sqlPack.CurrentDbFunctionResult = result;


        }
        private static void Sqrt(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Sqrt, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Mod(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            object precision = null;
            if (expression.Arguments.Count > 1)
            {
                var precisionExp = expression.Arguments[1];
                precision = precisionExp.GetValueOrColumnName(sqlPack);
            }

            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Mod, v, precision);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Log(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            object precision = null;
            if (expression.Arguments.Count > 1)
            {
                var precisionExp = expression.Arguments[1];
                precision = precisionExp.GetValueOrColumnName(sqlPack);
            }

            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Log, v, precision);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Pow(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            object precision = null;
            if (expression.Arguments.Count > 1)
            {
                var precisionExp = expression.Arguments[1];
                precision = precisionExp.GetValueOrColumnName(sqlPack);
            }

            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Pow, v, precision);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Sign(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Sign, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Truncate(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Truncate, v);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void Rand(MethodCallExpression expression, SqlPack sqlPack)
        {
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Rand, result);
            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void IfNull(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object v = startExp.GetValueOrColumnName(sqlPack);
            object precision = null;
            if (expression.Arguments.Count > 1)
            {
                var precisionExp = expression.Arguments[1];
                precision = precisionExp.GetValueOrColumnName(sqlPack);
            }

            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.IfNull, v, precision);

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void AddYears(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddYears, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddMonths(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddMonths, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddDays(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddDays, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddHours(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddHours, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddMinutes(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddMinutes, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddSeconds(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddSeconds, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void AddMilliseconds(MethodCallExpression expression, SqlPack sqlPack)
        {
            var member = expression.Object as MemberExpression;
            object columnName = GetValueOfMemberExpression(member, sqlPack);
            var memberExp = expression.Arguments[0];
            object v = memberExp.GetValueOfExpression(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.AddMilliseconds, columnName, v);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void DiffYears(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffYears, start, end);

            sqlPack.CurrentDbFunctionResult = result;

        }
        private static void DiffMonths(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffMonths, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void DiffDays(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffDays, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void DiffHours(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffHours, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void DiffMinutes(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffMinutes, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void DiffSeconds(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffSeconds, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void DiffMilliseconds(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffMilliseconds, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }
        private static void DiffMicroseconds(MethodCallExpression expression, SqlPack sqlPack)
        {
            var startExp = expression.Arguments[0];
            object start = startExp.GetValueOrColumnName(sqlPack);
            var endExp = expression.Arguments[1];
            object end = endExp.GetValueOrColumnName(sqlPack);
            string result = "";
            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.DiffMicroseconds, start, end);

            sqlPack.CurrentDbFunctionResult = result;
        }

        #endregion

        private static void In(MethodCallExpression expression, SqlPack sqlPack)
        {
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            sqlPack += " IN ";
            FluentExpressionSQLProvider.In(expression.Arguments[1], sqlPack);
        }
        private static void InNot(MethodCallExpression expression, SqlPack sqlPack)
        {
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            sqlPack += " NOT IN ";
            FluentExpressionSQLProvider.In(expression.Arguments[1], sqlPack);
        }
        private static void Like(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE '%' +";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            //sqlPack += " + '%'";

            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Like, column);

            sqlPack += " LIKE ";

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void LikeLeft(MethodCallExpression expression, SqlPack sqlPack)
        {
             
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE '%' +";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.LikeLeft, column);

            sqlPack += " LIKE ";

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void LikeRight(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE ";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            //sqlPack += " + '%'";

            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.LikeRight, column);

            sqlPack += " LIKE ";

            sqlPack.CurrentDbFunctionResult = result;

            
        }

        private static void LikeNot(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE '%' +";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            //sqlPack += " + '%'";

            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Like, column);

            sqlPack += " NOT LIKE ";

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void LikeLeftNot(MethodCallExpression expression, SqlPack sqlPack)
        {


            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE '%' +";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.LikeLeft, column);

            sqlPack += " NOT LIKE ";

            sqlPack.CurrentDbFunctionResult = result;
        }

        private static void LikeRightNot(MethodCallExpression expression, SqlPack sqlPack)
        {
            if (expression.Object != null)
            {
                FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
            }
            FluentExpressionSQLProvider.Where(expression.Arguments[0], sqlPack);
            //sqlPack += " LIKE ";
            //FluentExpressionSQLProvider.Where(expression.Arguments[1], sqlPack);
            //sqlPack += " + '%'";

            string result = "";

            var column = expression.Arguments[1].GetValueOrColumnName(sqlPack);

            result = sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.LikeRight, column);

            sqlPack += " NOT LIKE ";

            sqlPack.CurrentDbFunctionResult = result;


        }
        protected override SqlPack Join(MethodCallExpression expression, SqlPack sqlPack)
        {

            return this.Where( expression,  sqlPack);
           

        }

        

        protected override SqlPack Select(MethodCallExpression expression, SqlPack sqlPack)
        {

            object methodValue = null;

            if ( expression.TryGetValueOfMethodCallExpression(out methodValue))
            {
                //默认执行方法

                var str = sqlPack.SqlDialectProvider.FormatValue(methodValue);
                AddSelectField(sqlPack, str !=null ? str.ToString() :"");
                sqlPack.CurrentDbFunctionResult = null;
 
                sqlPack.CurrentDbFunctionResult = null;

                return sqlPack;
            }



            var key = expression.Method;
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }

            Action<MethodCallExpression, SqlPack> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {
                action(expression, sqlPack);
                AddSelectField(sqlPack, sqlPack.CurrentDbFunctionResult);
                sqlPack.CurrentDbFunctionResult = null;
                return sqlPack;
            }

  

            throw new NotImplementedException("无法解析方法" + expression.Method);
        }
        protected override SqlPack Where(MethodCallExpression expression, SqlPack sqlPack)
        {
            //if (expression.Object != null && expression.Object.CanGetValue())
            //{
            //    object value = null;

            //    value = expression.Object.GetValueOfExpression(sqlPack);
            //    if (value != null)
            //    {
            //        sqlPack += " " + value;
            //        sqlPack.CurrentDbFunctionResult = null; 
            //        return sqlPack;
            //    }
            //}

            object methodValue = null;

            if ( expression.TryGetValueOfMethodCallExpression(out methodValue))
            {
                //默认执行方法

                sqlPack += " " + sqlPack.SqlDialectProvider.FormatValue(methodValue);
                sqlPack.CurrentDbFunctionResult = null;

                return sqlPack;
            }


            var key = expression.Method;
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }

          

            Action<MethodCallExpression, SqlPack> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {


                action(expression, sqlPack);
                //if (expression.Object != null)
                //{
                //    FluentExpressionSQLProvider.Where(expression.Object, sqlPack);
                //}
                //sqlPack.AddDbParameter(sqlPack.CurrentDbFunctionResult);
                sqlPack += " " + sqlPack.CurrentDbFunctionResult;
                sqlPack.CurrentDbFunctionResult = null;

                return sqlPack;
            }

            throw new NotImplementedException("无法解析方法" + expression.Method );

           
        }


        protected override SqlPack OrderBy(MethodCallExpression expression, SqlPack sqlPack)
        {

            object methodValue = null;

            if (expression.TryGetValueOfMethodCallExpression(out methodValue))
            {
                //默认执行方法

                sqlPack += " " + sqlPack.SqlDialectProvider.FormatValue(methodValue);
                sqlPack.CurrentDbFunctionResult = null;

                return sqlPack;
            }


            var key = expression.Method;
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }




            Action<MethodCallExpression, SqlPack> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {
                action(expression, sqlPack);
                sqlPack += " " + sqlPack.CurrentDbFunctionResult;
                sqlPack.CurrentDbFunctionResult = null;

                return sqlPack;
            }

            throw new NotImplementedException("无法解析方法" + expression.Method );

        }

    }
}