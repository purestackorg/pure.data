using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace FluentExpressionSQL.Sql
{
    public class PostgreSqlDialectProvider : SqlDialectBaseProvider
    {
        private static PostgreSqlDialectProvider _instance = null;
        public static PostgreSqlDialectProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PostgreSqlDialectProvider();

                }
                return _instance;

            }
        }
        Dictionary<Type, string> CastTypeMap = null;
        public PostgreSqlDialectProvider()
        {

            //转换类型映射
            CastTypeMap = new Dictionary<Type, string>();


            Dictionary<Type, string> castTypeMap = new Dictionary<Type, string>();
            CastTypeMap.Add(typeof(string), "TEXT");
            CastTypeMap.Add(typeof(byte), "SMALLINT");
            CastTypeMap.Add(typeof(sbyte), "SMALLINT");
            CastTypeMap.Add(typeof(Int16), "INTEGER");
            CastTypeMap.Add(typeof(UInt16), "INTEGER");
            CastTypeMap.Add(typeof(int), "INTEGER");
            CastTypeMap.Add(typeof(uint), "INTEGER");
            CastTypeMap.Add(typeof(long), "BIGINT");
            CastTypeMap.Add(typeof(ulong), "BIGINT");
            CastTypeMap.Add(typeof(DateTime), "DATE");
            CastTypeMap.Add(typeof(bool), "BOOLEAN");
            CastTypeMap.Add(typeof(Guid), "VARCHAR");

            CastTypeMap.Add(typeof(decimal), "DECIMAL");
            CastTypeMap.Add(typeof(double), "DECIMAL");
            CastTypeMap.Add(typeof(float), "REAL");

        }

        #region Base
        public ExpDbType ExpDbType
        {
            get { return ExpDbType.PostgreSQL; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT LASTVAL() AS Id";
            //get { return "select last_insert_id();"; }
        }

      
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }
            int startValue = (page - 1) * resultsPerPage;

            //int startValue = page * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int pageNumber, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} LIMIT @maxResults OFFSET @pageStartRowNbr", sql);
            parameters.Add("@maxResults", maxResults);
            parameters.Add("@pageStartRowNbr", pageNumber );
            return result;
        }

        public override string GetColumnName(string prefix, string columnName, string alias)
        {
            return base.GetColumnName(null, columnName, alias).ToLower();
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            return base.GetTableName(schemaName, tableName, alias).ToLower();
        }


        #endregion

        #region Ext



        string BuildCastState(object castObject, string targetDbTypeString, bool needToFormat = true)
        {
            return ("CAST(" + (castObject) + " AS " + targetDbTypeString + ")");

        }
        string DbFunction_DATEADD(string interval, object value, object addValue)
        {
            //datetime + interval '2 years';

            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(value);
            sb.Append(" + INTERVAL '");
            sb.Append((addValue));
            sb.Append(" ");
            sb.Append((interval));
            sb.Append("')");
            return sb.ToString();
        }
        string DbFunction_DATEPART(string interval, object value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DATE_PART('");
            sb.Append((interval));
            sb.Append("',");
            sb.Append((value));
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
            //round(extract(epoch  FROM (timestamp '2015-01-15 20:05' - timestamp '2015-01-14 19:05')) / (60*60*24*30) )
            //时间差单位默认转换为秒
            StringBuilder sb = new StringBuilder();
            sb.Append("ROUND(EXTRACT(EPOCH  FROM (");
            sb.Append((enddate));
            sb.Append(" - ");
            sb.Append((startdate));
            sb.Append("))");

            if (interval == "HOUR")
            {
                sb.Append("/ (60*60)");
            }
            else if (interval == "MINUTE")
            {
                sb.Append("/ (60)");
            }
            else if (interval == "SECOND")
            {
                // sb.Append("* 24 * 60 * 60");
            }
            else if (interval == "MILLISECOND")
            {
                sb.Append("* 1000");
            }
            else if (interval == "YEAR")
            {
                sb.Append("/ (365*24*60*60)");
            }
            else if (interval == "MONTH")
            {
                sb.Append("/ (30*24*60*60)");
            }
            else if (interval == "DAY")
            {
                sb.Append("/ (24*60*60)");
            }
            else if (interval == "MICROSECOND")
            {
                sb.Append("* (1000 * 1000)");
            }

            sb.Append(")");
            return sb.ToString();

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
            else if (v is String || v is Guid)
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

        private string ConvertDateTime(object castObject, bool addQuot = true)
        {
            string result = "";
            if (castObject == null)
            {
                return result;
            }
            if (castObject.ToString().Contains("TIMESTAMP"))
            {
                return castObject.ToString();
            }



            if (addQuot == true)
            {
                result = ("TIMESTAMP '");
                result += castObject;
                result += ("'");
            }
            else
            {
                result = ("TIMESTAMP ");
                result += castObject;
            }



            return result;
        }

        public override string ConvertSqlValue(object castObject, Type type)
        {

            string result = "";

            if (type == ResolveConstants.TypeOfDateTime)
            {

                result = ConvertDateTime(castObject, false);
                return result;
            }
            else if (type == ResolveConstants.TypeOfString)
            {
                result = ("");
                result += castObject;

                return result;
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

                result = param0 + " || '%'";
            }
            else if (functionType == DbFunctionType.EndsWith)
            {

                result = "'%' || " + param0;
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
                    result += "," + parameters[1] + "+1";
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
                throw new NotSupportedException("PostgreSQL is not supports DbFunctionType.NewGuid ");
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
                result += (param0);
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
                object precision = 10;
                if (parameters.Length > 1 && parameters[1] != null)
                {
                    precision = parameters[1];
                }
                result = ("LOG(");
                result += (precision);
                result += (",");
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
                result = ("RANDOM()");
            }
            else if (functionType == DbFunctionType.IfNull)
            {
                result = ("COALESCE(" + param0 + "," + parameters[1] + ")");
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
                sql = DbFunction_DATEPART("YEAR", v);
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
            /* PostgreSQL is not supports MILLISECOND */
            if (member == ResolveConstants.PropertyInfo_DateTime_Millisecond)
            {
                //throw new NotSupportedException("PostgreSQL is not supports MILLISECOND in datepart convertion!");
                sql = DbFunction_DATEPART("MILLISECOND", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_DayOfWeek)
            {
                //string tmp =  ("(");
                //tmp += DbFunction_DATEPART("WEEK", v);
                //tmp+=(" - 1)");
                //sql = tmp;
                sql = DbFunction_DATEPART("DOW", v);
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
                    valueOfTime = ("NOW()");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_UtcNow)
                {
                    valueOfTime = ("CURRENT_TIMESTAMP");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Today)
                {
                    valueOfTime = ("CURRENT_DATE");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Date)
                {
                    valueOfTime = "DATE(" + value + ")";
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
            str =   colName + " ~'" + regex + "'";
            return str;
        }
        #endregion
    }
}