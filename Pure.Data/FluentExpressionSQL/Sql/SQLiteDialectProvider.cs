using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace FluentExpressionSQL.Sql
{
    public class SQLiteDialectProvider : SqlDialectBaseProvider
    {
        private static SQLiteDialectProvider _instance = null;
        public static SQLiteDialectProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SQLiteDialectProvider();
                    
                }
                return _instance;

            }
        }

        Dictionary<Type, string> CastTypeMap = null;
        public SQLiteDialectProvider()
        {

            //转换类型映射
            CastTypeMap = new Dictionary<Type, string>();
           

            Dictionary<Type, string> castTypeMap = new Dictionary<Type, string>();

             CastTypeMap.Add(typeof(string), "TEXT");
             CastTypeMap.Add(typeof(byte), "INTEGER");
             CastTypeMap.Add(typeof(sbyte), "INTEGER");
             CastTypeMap.Add(typeof(Int16), "INTEGER");
             CastTypeMap.Add(typeof(int), "INTEGER");
             CastTypeMap.Add(typeof(long), "INTEGER");
             //CastTypeMap.Add(typeof(decimal), "DECIMAL(19,0)");
             CastTypeMap.Add(typeof(decimal), "REAL");

             CastTypeMap.Add(typeof(double), "REAL");
             CastTypeMap.Add(typeof(float), "REAL");
             CastTypeMap.Add(typeof(bool), "INTEGER");
            //CastTypeMap.Add(typeof(DateTime), "DATETIME");
            //CastTypeMap.Add(typeof(Guid), "UNIQUEIDENTIFIER");
            
        }

        #region Base
        public ExpDbType ExpDbType
        {
            get { return ExpDbType.SQLite; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LAST_INSERT_ROWID() AS [Id]";
        }

        public override char OpenQuote
        {
            get { return '['; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }
            int startValue = (page-1) * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("SQL");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            var result = string.Format("{0} LIMIT @Offset, @Count", sql);
            parameters.Add("@Offset", firstResult);
            parameters.Add("@Count", maxResults);
            return result;
        }

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException(columnName, "columnName cannot be null or empty.");
            }
            var result = new StringBuilder();
            result.AppendFormat(columnName);
            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }
            return result.ToString();
        }

        #endregion

        #region Ext
            
       
        
        string BuildCastState(object castObject, string targetDbTypeString, bool needToFormat =true)
        {
            return ("CAST(" +  (castObject) + " AS " + targetDbTypeString + ")");

        }
        string DbFunction_DATEADD(string interval, object value, object addValue)
        {
            /* DATETIME(@P_0,'+' || 1 || ' years') */
            StringBuilder sb = new StringBuilder();
            sb.Append("DATETIME(");
            sb.Append(value);
            sb.Append(",'+' || ");
            sb.Append((addValue));
            sb.Append(" || ' ");
            sb.Append((interval));
            sb.Append("')");
            return sb.ToString();
        }
        string DbFunction_DATEPART( string interval, object value)
        {
            /* CAST(STRFTIME('%M','2016-08-06 09:01:24') AS INTEGER) */
            StringBuilder sb = new StringBuilder();
            sb.Append("CAST(STRFTIME('%");
            sb.Append(interval);
            sb.Append("',");
            sb.Append(( value));
            sb.Append(") AS INTEGER)");
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
            /* (CAST(STRFTIME('%Y',endDateTimeExp) as INTEGER) - CAST(STRFTIME('%Y',startDateTimeExp) as INTEGER)) */

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(DbFunction_DATEPART("Y", enddate));
            sb.Append(" - ");
            sb.Append(DbFunction_DATEPART("Y", startdate));
            sb.Append(")");
            return sb.ToString();

        }

        string Append_DiffYears(object startdate, object enddate)
        {
            /* (CAST(STRFTIME('%Y',endDateTimeExp) as INTEGER) - CAST(STRFTIME('%Y',startDateTimeExp) as INTEGER)) */

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(DbFunction_DATEPART("Y", enddate));
            sb.Append(" - ");
            sb.Append(DbFunction_DATEPART("Y", startdate));
            sb.Append(")");
            return sb.ToString();
        }
        string Append_JULIANDAY(object startdate, object enddate)
        {
            /* (JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) */
            StringBuilder sb = new StringBuilder();
            sb.Append("(JULIANDAY(");
            sb.Append(enddate);
            sb.Append(") - JULIANDAY(");
            sb.Append(startdate);
            sb.Append("))");
            return sb.ToString();
        }
        string Append_DateDiff(object startdate, object enddate, int? multiplier)
        {
            /* CAST((JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) AS INTEGER) */
            /* OR */
            /* CAST((JULIANDAY(endDateTimeExp)- JULIANDAY(startDateTimeExp)) * multiplier AS INTEGER) */
            StringBuilder sb = new StringBuilder();
            sb.Append("CAST(");
            sb.Append(Append_JULIANDAY(startdate, enddate));
            if (multiplier != null)
                sb.Append(" * " + multiplier.Value.ToString());

            sb.Append(" AS INTEGER)");
            return sb.ToString();

        }

        private string ConvertDateTime(object castObject, bool addQuot = true)
        {
            string result = "", datestr = "";
            if (castObject == null)
            {
                return result;
            }
            if (addQuot == false)
            {
                return castObject.ToString();
            }
            
            ///转换我sqlite标准时间格式
            DateTime outDate ;
            DateTime.TryParse(castObject.ToString().Replace("'", ""), out outDate);// Convert.ToDateTime(castObject.ToString().Replace("'", "")).ToString("yyyy-MM-dd HH:mm:ss");
            if (outDate != null)
            {
                datestr = outDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                datestr = castObject.ToString();
            }
            if (addQuot == true)
            {
                result = ("DATETIME('");
                result += datestr;
                result += ("')");
            }
            else
            {
                result = ("DATETIME('");
                result += datestr;
                result += ("')");
            }



            return result;
        }
        public override object FormatValue(object v, Type type = null)
        {
            //if (v is String)
            //{
            //    return "N'" + v + "'"; ;

            //}
            if (v == null)
            {
                if (type != null)
                {
                    return FormatValue(type.GetDefaultValue());
                }
                return v;
            }

            if (v is DateTime)
            {
                string result = ConvertDateTime(v);
                return result;

            }
            else if ( v is String || v is Guid)
            {
                return "'" + v + "'";
               
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
                result = ConvertDateTime(castObject, false);
                //result = "DATETIME("+castObject+ ")";
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
                result = "LENGTH(" + param0 + ")";
            }
            else if (functionType == DbFunctionType.StartsWith)
            {

                result =  param0 + " || '%'";
            }
            else if (functionType == DbFunctionType.EndsWith)
            {

                result = "'%' || " + param0 ;
            }
            else if (functionType == DbFunctionType.Contains)
            {

                result = "'%' || " + param0 + " || '%'";
            }
            else if (functionType == DbFunctionType.LikeRight)
            {

                result = param0 + " || '%'";
            }
            else if (functionType == DbFunctionType.LikeLeft)
            {
                result = "'%' || " + param0;
            }
            else if (functionType == DbFunctionType.Like)
            {

                result = "'%' || " + param0 + " || '%'";
            }
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
                result = "TRIM(" + param0 + ")";
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
                result = "SUBSTR(" + param0;

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
                throw new NotSupportedException("SQLite is not supports DbFunctionType.NewGuid");
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
                result = ("CEIL(");
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
                //throw new NotSupportedException("SQLite is not supports DbFunctionType.NewGuid");

                //object precision = 10;
                //if (parameters.Length > 1 && parameters[1] != null)
                //{
                //    precision = parameters[1];
                //}
                result = ("LOG(");
                result += (param0);

                result += (")");
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
                throw new NotSupportedException("SQLite is not supports DbFunctionType.Truncate");

                //object precision = 0;
                //if (parameters.Length > 1 && parameters[1] != null)
                //{
                //    precision = parameters[1];
                //}
                //result = ("TRUNCATE(");
                //result += (param0);
                //result += (",");
                //result += (precision);

                //result += (")");
            }
            else if (functionType == DbFunctionType.Mod)
            {
                throw new NotSupportedException("SQLite is not supports DbFunctionType.Mod");
                //object precision = 2;
                //if (parameters.Length > 1 && parameters[1] != null)
                //{
                //    precision = parameters[1];
                //}
                //result = ("MOD(");
                //result += (param0);
                //result += (",");
                //result += (precision);

                //result += (")");
            }
            else if (functionType == DbFunctionType.Rand)
            {
                result = ("RANDOM()");
            }
            else if (functionType == DbFunctionType.IfNull)
            {
                result = ("IFNULL("+param0+","+parameters[1]+")");
            }
                //Add DateTime 函数
            else if (functionType == DbFunctionType.AddYears)
            {
                result = DbFunction_DATEADD("years", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMonths)
            {
                result = DbFunction_DATEADD("months", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddDays)
            {
                result = DbFunction_DATEADD("days", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddHours)
            {
                result = DbFunction_DATEADD("hours", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMinutes)
            {
                result = DbFunction_DATEADD("minutes", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddSeconds)
            {
                result = DbFunction_DATEADD("seconds", param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.AddMilliseconds)
            {
                result = DbFunction_DATEADD("MILLISECOND", param0, parameters[1]);
            }
                //diff datetime
            else if (functionType == DbFunctionType.DiffYears)
            {
                result = Append_DiffYears( param0, parameters[1]);
            }
            else if (functionType == DbFunctionType.DiffMonths)
            {
                object startDateTimeExp = param0, endDateTimeExp = parameters[1];

                /*
                 * This method will generating sql like following:
                  (cast(STRFTIME('%Y','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%Y','2015-08-06 09:01:24') as INTEGER)) * 12  + (cast(STRFTIME('%m','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%m','2015-08-06 09:01:24') as INTEGER))
                 */
                StringBuilder sb = new StringBuilder();
                sb.Append("(");
                /* (cast(STRFTIME('%Y','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%Y','2015-08-06 09:01:24') as INTEGER)) * 12 */
                sb.Append( Append_DiffYears(startDateTimeExp, endDateTimeExp));
                sb.Append(" * 12");

                sb.Append(" + ");

                /* (cast(STRFTIME('%m','2016-07-06 09:01:24') as INTEGER) - cast(STRFTIME('%m','2015-08-06 09:01:24') as INTEGER)) */
                sb.Append("(");
                sb.Append(DbFunction_DATEPART( "m", endDateTimeExp));
                sb.Append(" - ");
                sb.Append(DbFunction_DATEPART("m", startDateTimeExp));
                sb.Append(")");

                sb.Append(")");

                result = sb.ToString();
            }
            else if (functionType == DbFunctionType.DiffDays)
            {

                result = Append_DateDiff(param0, parameters[1], null);
            }
            else if (functionType == DbFunctionType.DiffHours)
            {
                result = Append_DateDiff(param0, parameters[1], 24);
            }
            else if (functionType == DbFunctionType.DiffMinutes)
            {
                result = Append_DateDiff(param0, parameters[1], 24 * 60);
            }
            else if (functionType == DbFunctionType.DiffSeconds)
            {
                result = Append_DateDiff(param0, parameters[1], 24 * 60 * 60);
            }
            else if (functionType == DbFunctionType.DiffMilliseconds)
            {
                result = Append_DateDiff(param0, parameters[1], 24 * 60 * 60*1000);
            }
            else if (functionType == DbFunctionType.DiffMicroseconds)
            {
                throw new NotSupportedException("SQLite is not supports DbFunctionType.DiffMicroseconds");

            }
            return result;
        }
        bool IsDatePart(MemberInfo member, object v2, out string sql)
        {
            object v = ConvertSqlValue(v2, v2.GetType());
            if (member == ResolveConstants.PropertyInfo_DateTime_Year)
            {
                sql = DbFunction_DATEPART("Y", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Month)
            {
                sql = DbFunction_DATEPART("m", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Day)
            {
                sql = DbFunction_DATEPART("d", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Hour)
            {
                sql = DbFunction_DATEPART("H", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Minute)
            {
                sql = DbFunction_DATEPART("M", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Second)
            {
                sql = DbFunction_DATEPART("S", v);
                return true;
            }
            /* SQLite is not supports MILLISECOND */
            if (member == ResolveConstants.PropertyInfo_DateTime_Millisecond)
            {
                throw new NotSupportedException("SQLite is not supports MILLISECOND in datepart convertion!");
                //sql = DbFunction_DATEPART("MILLISECOND", v);
                //return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_DayOfWeek)
            {
                sql = DbFunction_DATEPART("w", v);
               
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
                    valueOfTime = ("DATETIME('NOW','LOCALTIME')");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_UtcNow)
                {
                    valueOfTime = ("DATETIME()");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Today)
                {
                    valueOfTime = ("DATE('NOW','LOCALTIME')");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Date)
                {
                    valueOfTime = ("DATETIME(DATE(");
                    valueOfTime += value;
                    valueOfTime += ("))");
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
            str = colName + " REGEXP "  + "'" + regex + "'";
            return str;

        }
        #endregion
    }
}