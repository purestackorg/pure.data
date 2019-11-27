 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace FluentExpressionSQL.Sql
{
    public class SqlServerDialectProvider : SqlDialectBaseProvider
    {
        private static SqlServerDialectProvider _instance = null;
        public static SqlServerDialectProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SqlServerDialectProvider();
                    
                }
                return _instance;

            }
        }
        Dictionary<Type, string> CastTypeMap = null;
        public SqlServerDialectProvider()
        {

            //转换类型映射
            CastTypeMap = new Dictionary<Type, string>();
            CastTypeMap.Add(typeof(string), "NVARCHAR(MAX)");
            CastTypeMap.Add(typeof(byte), "TINYINT");
            CastTypeMap.Add(typeof(sbyte), "TINYINT"); 
            CastTypeMap.Add(typeof(Int16), "SMALLINT");
            CastTypeMap.Add(typeof(int), "INT"); 
            CastTypeMap.Add(typeof(long), "BIGINT"); 
            CastTypeMap.Add(typeof(decimal), "DECIMAL(19,4)");
            CastTypeMap.Add(typeof(double), "FLOAT");
            CastTypeMap.Add(typeof(float), "REAL");
            CastTypeMap.Add(typeof(bool), "BIT");
            CastTypeMap.Add(typeof(DateTime), "DATETIME");
            CastTypeMap.Add(typeof(Guid), "UNIQUEIDENTIFIER");
        }

        #region Base
        public ExpDbType ExpDbType
        {
            get { return ExpDbType.SQLServer; }
        }


        public override char ParameterPrefix
        {
            get { return '@'; }
        }

        public override char OpenQuote
        {
            get { return '['; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return string.Format(@"SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [Id]");
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }
            int startValue = ((page - 1) * resultsPerPage) + 1;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("sql");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            int selectIndex = GetSelectEnd(sql) + 1;
            string orderByClause = GetOrderByClause(sql) ?? "ORDER BY CURRENT_TIMESTAMP";


            string projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_proj", s, null)), sb => sb.ToString());
            string newSql = sql
                .Replace(" " + orderByClause, string.Empty)
                .Insert(selectIndex, string.Format("ROW_NUMBER() OVER(ORDER BY {0}) AS {1}, ", orderByClause.Substring(9), GetColumnName(null, "_row_number", null)));

            string result = string.Format("SELECT TOP({0}) {1} FROM ({2}) [_proj] WHERE {3} >= @_pageStartRow ORDER BY {3}",
                maxResults, projectedColumns.Trim(), newSql, GetColumnName("_proj", "_row_number", null));

            parameters.Add("@_pageStartRow", firstResult);
            return result;
        }

        protected string GetOrderByClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            if (orderByIndex == -1)
            {
                return null;
            }

            string result = sql.Substring(orderByIndex).Trim();

            int whereIndex = result.IndexOf(" WHERE ", StringComparison.InvariantCultureIgnoreCase);
            if (whereIndex == -1)
            {
                return result;
            }

            return result.Substring(0, whereIndex).Trim();
        }

        protected int GetFromStart(string sql)
        {
            int selectCount = 0;
            int fromIndex = 0;
           
            string[] words = sql.Split(' ');

            foreach (var word in words)
            {
                if (word.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        protected virtual int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", "sql");
        }

        protected virtual IList<string> GetColumnNames(string sql)
        {
            int start = GetSelectEnd(sql);
            int stop = GetFromStart(sql);
            string[] columnSql = sql.Substring(start, stop - start).Split(',');
            List<string> result = new List<string>();
            foreach (string c in columnSql)
            {
                int index = c.LastIndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                string[] colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }

            return result;
        }

        #endregion

        #region Ext
            
       
        
        string BuildCastState(object castObject, string targetDbTypeString, bool needToFormat =true)
        {
            //if (needToFormat)
            //{
            //    return ("CAST(" + FormatValue(castObject) + " AS " + targetDbTypeString + ")");
            //}
            //else
            return ("CAST(" +  (castObject) + " AS " + targetDbTypeString + ")");

        }
        string DbFunction_DATEADD(string interval, object value, object addValue)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATEADD(");
            sb.Append(interval);
            sb.Append(",");
            sb.Append((addValue));
            sb.Append(",");
            sb.Append((value));
            sb.Append(")");
            return sb.ToString();
        }
        string DbFunction_DATEPART( string interval, object value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATEPART(");
            sb.Append(interval);
            sb.Append(",");
            sb.Append(( value));
            sb.Append(")");
            return sb.ToString();
        }
        /// <summary>
        /// 返回两个日期差值
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        string DbFunction_DATEDIFF(string interval, object startdate, object enddate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATEDIFF(");
            sb.Append(interval);
            sb.Append(",");
            sb.Append((startdate));
            sb.Append(",");
            sb.Append((enddate));
            sb.Append(")");
            return sb.ToString();

        }


        public override object FormatValue(object v, bool needFormat, Type type = null)
        {
            //if (v is String)
            //{
            //    return "N'" + v + "'"; ;

            //}


            if (v == null)
            {
                if (type != null)
                {
                    return FormatValue(type.GetDefaultValue(), needFormat);
                }
                return v;
            }

           

            if (v is DateTime || v is String || v is Guid)
            {
                if (needFormat == true)
                {
                    if ( v != null && (v is string || v is String))
                    {
                        v = ReplaceSQLInjectChar(v.ToString());
                    }
                    return "'" + v + "'";
                }

            }
            else if (v is bool || v is Boolean)
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
            else if (v.GetType().IsEnum())
            {
                return (int)v;
            }

            return v;
        }

        public override string ConvertSqlValue(object castObject, Type type)
        {
            string result = "";
            if (type ==ResolveConstants.TypeOfDateTime)
            {
                result = BuildCastState((castObject), "DATETIME", false);
            }
            else
            {
                string dbTypeString = "";
                if (CastTypeMap.TryGetValue(type, out dbTypeString))
                {
                    result = BuildCastState((castObject), dbTypeString, false);
                }
                else
                {
                    result = castObject.ToString();
                }
            }
            
            return result;
        }

        public override string ConvertDbFunction(DbFunctionType functionType, params object[] parameters)
        {
            string result = "''";
            object param0 = parameters[0];
            if (functionType == DbFunctionType.Length)
            {
                result = "LEN(" + param0 + ")";
            }
                //模糊搜索
            else if (functionType == DbFunctionType.StartsWith)
            {

                result =  param0 + "+'%'";
            }
            else if (functionType == DbFunctionType.EndsWith)
            {

                result = "'%'+" + param0 ;
            }
            else if (functionType == DbFunctionType.Contains)
            {

                result = "'%'+" + param0 + "+'%'";
            }
            else if (functionType == DbFunctionType.LikeRight)
            {

                result = param0 + "+'%'";
            }
            else if (functionType == DbFunctionType.LikeLeft)
            {
                result = "'%'+" + param0;
            }
            else if (functionType == DbFunctionType.Like)
            {

                result = "'%'+" + param0 + "+'%'";
            }
                //转换
            else if (functionType == DbFunctionType.ToUpper)
            {
                result = "UPPER(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.ToLower)
            {
                result = "LOWER(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.Trim)
            {
                result = "RTRIM(LTRIM(" + param0 + "))";
            }
            else if (functionType == DbFunctionType.TrimEnd)
            {
                result = "RTRIM(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.TrimStart)
            {
                result = "LTRIM(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.Substring)
            {
                result = "SUBSTRING(" + param0 ;

                if (parameters.Length > 1)
                {
                    result += "," + parameters[1] +"+1";
                }
                ///截取长度
                if (parameters.Length > 2)
                {
                    result += "," + parameters[2];
                }
                else
                {
                    result += "," + ConvertDbFunction(DbFunctionType.Length, param0);

                }
                result += (")");

            }
            else if (functionType == DbFunctionType.IsNullOrEmpty)
            {
                result = "(" + param0 + " IS NULL OR " + param0 + "= '')";
            }
            else if (functionType == DbFunctionType.Parse)
            {
                Type type = parameters[1] as Type;
                result = ConvertSqlValue(param0, type);
            }
            else if (functionType == DbFunctionType.NewGuid)
            {
                result = "NEWID()";
            }
                //聚合函数
            else if (functionType == DbFunctionType.Count)
            {
                result = "COUNT(1)";
            }
            else if (functionType == DbFunctionType.Sum)
            {
                result = "SUM(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.Max)
            {
                result = "MAX(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.Min)
            {
                result = "MIN(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.Avg)
            {
                result = "AVG(" + param0 + ")";
            }
                //数学函数
            else if (functionType == DbFunctionType.Abs)
            {
                result = ("ABS(");
                result +=(param0);
                result += (")"); 
            }
            else if (functionType == DbFunctionType.Round)
            {
                object precision = 2;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];
                }
                result = ("ROUND(");
                result += (param0);
                result += (",");
                result += (precision);

                result += (")");
            }
            else if (functionType == DbFunctionType.Ceiling)
            {
                result = ("CEILING(");
                result += (param0);
                result += (")");
            }
            else if (functionType == DbFunctionType.Floor)
            {
                result = ("FLOOR(");
                result += (param0);
                result += (")");
            }
            else if (functionType == DbFunctionType.Sqrt)
            {
                result = ("SQRT(");
                result += (param0);
                result += (")");
            }
            else if (functionType == DbFunctionType.Log)
            {
                object precision = 10;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];

                }
                if (Convert.ToInt32(precision) == 10)
                {
                    result = ("LOG10(");
                    result += (param0);
                    result += (")");
                }
                else
                {
                    result = ("LOG(");
                    result += (param0);
                    result += (")");
                }
               
                
            }
            else if (functionType == DbFunctionType.Pow)
            {
                object precision = 10;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];
                }
                result = ("POWER(");
                result += (param0);
                result += (",");
                result += (precision);

                result += (")");
            }
            else if (functionType == DbFunctionType.Sign)
            {
                result = ("SIGN(");
                result += (param0);
                result += (")");
            }
            else if (functionType == DbFunctionType.Truncate)
            {
                object precision = 0;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];
                }
                result = ("TRUNC(");
                result += (param0);
                result += (",");
                result += (precision);

                result += (")");
            }
            else if (functionType == DbFunctionType.Mod)
            {
                object precision = 2;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];
                }
                result = ("MOD(");
                result += (param0);
                result += (",");
                result += (precision);

                result += (")");
            }
            else if (functionType == DbFunctionType.Rand)
            {
                result = ("RAND()");
            }
            else if (functionType == DbFunctionType.IfNull)
            {
                result = ("ISNULL(" + param0 + "," + parameters[1] + ")");
            }
                //Add DateTime 函数
            else if (functionType == DbFunctionType.AddYears)
            {
                result = DbFunction_DATEADD("YEAR", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMonths)
            {
                result = DbFunction_DATEADD("MONTH", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddDays)
            {
                result = DbFunction_DATEADD("DAY", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddHours)
            {
                result = DbFunction_DATEADD("HOUR", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMinutes)
            {
                result = DbFunction_DATEADD("MINUTE", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddSeconds)
            {
                result = DbFunction_DATEADD("SECOND", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMilliseconds)
            {
                result = DbFunction_DATEADD("MILLISECOND", param0, parameters[1]);
            }
                //diff datetime
            else if (functionType == DbFunctionType.DiffYears)
            {
                result = DbFunction_DATEDIFF("YEAR", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffMonths)
            {
                result = DbFunction_DATEDIFF("MONTH", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffDays)
            {
                result = DbFunction_DATEDIFF("DAY", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffHours)
            {
                result = DbFunction_DATEDIFF("HOUR", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffMinutes)
            {
                result = DbFunction_DATEDIFF("MINUTE", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffSeconds)
            {
                result = DbFunction_DATEDIFF("SECOND", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffMilliseconds)
            {
                result = DbFunction_DATEDIFF("MILLISECOND", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffMicroseconds)
            {
                result = DbFunction_DATEDIFF("MICROSECOND", param0, parameters[1]);
            }
            return result;
        }
        bool IsDatePart(MemberInfo member, object v2, out string sql)
        {
            object v = ConvertSqlValue(v2, v2.GetType());
            if (member == ResolveConstants.PropertyInfo_DateTime_Year)
            {
                sql = DbFunction_DATEPART( "YEAR", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Month)
            {
                sql = DbFunction_DATEPART("MONTH", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Day)
            {
                sql = DbFunction_DATEPART("DAY", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Hour)
            {
                sql = DbFunction_DATEPART("HOUR", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Minute)
            {
                sql = DbFunction_DATEPART("MINUTE", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Second)
            {
                sql = DbFunction_DATEPART("SECOND", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Millisecond)
            {
                sql = DbFunction_DATEPART("MILLISECOND", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_DayOfWeek)
            {
                string tmp =  ("(");
                tmp += DbFunction_DATEPART("WEEKDAY", v);
                tmp+=(" - 1)");
                sql = tmp;
                return true;
            }
            sql = "";
            return false;
        }
        public override DateTimeDto ConvertDateTime(MemberInfo member, object value)
        {
            DateTimeDto result = new DateTimeDto();
            if (member != null)
            {

                result.Member = member;

                string valueOfTime = null;
                if (member == ResolveConstants.PropertyInfo_DateTime_Now)
                {
                    valueOfTime = ("GETDATE()");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_UtcNow)
                {
                    valueOfTime = ("GETUTCDATE()");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Today)
                {
                    valueOfTime = BuildCastState("GETDATE()", "DATE", false);
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Date)
                {
                    valueOfTime = BuildCastState(value, "DATE", false);
                }
                else if (this.IsDatePart(member, value, out valueOfTime))
                {
                    // return exp;
                }
                result.Text = valueOfTime;
            }

            return result;
        }

        public override string DoCaseWhen(List<CaseThenExpressionPair> listCase)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (CASE");
            foreach (CaseThenExpressionPair item in listCase)
            {
                if (item.Case != null)
                {
                    sb.Append(" WHEN ");
                    sb.Append(item.Case);
                    sb.Append(" THEN ");
                    sb.Append(item.Then);
                }
                else
                {
                    //else case
                    sb.Append(" ELSE ");
                    sb.Append(item.Then);
                    sb.Append(" END");
                }
            }
            sb.Append(")");

            return sb.ToString();
        }



        public override string ConvertRegexStr(string colName, string regex)
        {
            string str = "";
            str = colName + " like " + "'%[" + regex + "]%'";
            return str;
            //NAME like '%[^A-Z]%'
        }
        #endregion
    }
}