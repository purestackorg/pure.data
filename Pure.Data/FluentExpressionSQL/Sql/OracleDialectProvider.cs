using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
namespace FluentExpressionSQL.Sql
{
    public class OracleDialectProvider : SqlDialectBaseProvider
    {
        private static OracleDialectProvider _instance = null;
        public static OracleDialectProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OracleDialectProvider();

                }
                return _instance;

            }
        }
        Dictionary<Type, string> CastTypeMap = null;
        public OracleDialectProvider()
        {

            //转换类型映射
            CastTypeMap = new Dictionary<Type, string>();

            //CastTypeMap.Add(typeof(string), "NVARCHAR2"); // instead of using to_char(exp) 
            CastTypeMap.Add(typeof(byte), "NUMBER(4,0)");
            CastTypeMap.Add(typeof(sbyte), "NUMBER(4,0)");
            CastTypeMap.Add(typeof(Int16), "NUMBER(8,0)");
            CastTypeMap.Add(typeof(int), "NUMBER(13,0)");
            CastTypeMap.Add(typeof(long), "NUMBER(18,0)");
            CastTypeMap.Add(typeof(decimal), "NUMBER");
            CastTypeMap.Add(typeof(double), "BINARY_DOUBLE");
            CastTypeMap.Add(typeof(float), "BINARY_FLOAT");
            CastTypeMap.Add(typeof(bool), "NUMBER(4,0)");
            //CastTypeMap.Add(typeof(DateTime), "DATE"); // instead of using TO_TIMESTAMP(exp) 
            //CastTypeMap.Add(typeof(Guid), "NVARCHAR2");

        }

        #region Base
        public ExpDbType ExpDbType
        {
            get { return ExpDbType.Oracle; }
        }


        //from Simple.Data.Oracle implementation https://github.com/flq/Simple.Data.Oracle/blob/master/Simple.Data.Oracle/OraclePager.cs
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }

            var toSkip = (page-1) * resultsPerPage;
            var topLimit = (page ) * resultsPerPage;

            var sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT \"_ss_dapper_1_\".*, ROWNUM RNUM FROM (");
            sb.Append(sql);
            sb.AppendLine(") \"_ss_dapper_1_\"");
            sb.AppendLine("WHERE ROWNUM <= :topLimit) \"_ss_dapper_2_\" ");
            sb.AppendLine("WHERE \"_ss_dapper_2_\".RNUM > :toSkip");

            parameters.Add(":topLimit", topLimit);
            parameters.Add(":toSkip", toSkip);

            return sb.ToString();
        }
        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT \"_ss_dapper_1_\".*, ROWNUM RNUM FROM (");
            sb.Append(sql);
            sb.AppendLine(") \"_ss_dapper_1_\"");
            sb.AppendLine("WHERE ROWNUM <= :topLimit) \"_ss_dapper_2_\" ");
            sb.AppendLine("WHERE \"_ss_dapper_2_\".RNUM > :toSkip");

            parameters.Add(":topLimit", maxResults + firstResult);
            parameters.Add(":toSkip", firstResult);

            return sb.ToString();
        }
        public override string QuoteString(string value)
        {
            if (value != null && value[0] == '`')
            {
                return string.Format("{0}{1}{2}", OpenQuote, value.Substring(1, value.Length - 2), CloseQuote);
            }
            return value.ToUpper();
        }
        public override char ParameterPrefix
        {
            get { return ':'; }
        }


        public override char OpenQuote
        {
            get { return '"'; }
        }

        public override char CloseQuote
        {
            get { return '"'; }
        }

        public override string GetIdentitySql(string tableName)
        {
            return "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
        }


        public override bool SupportsMultipleStatements
        {
            get { return false; }
        }

        //from Simple.Data.Oracle implementation https://github.com/flq/Simple.Data.Oracle/blob/master/Simple.Data.Oracle/OraclePager.cs
        //public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        //{
        //    if (page <= 0)
        //    {
        //        page = 1;
        //    }
        //    int startValue = (page - 1) * resultsPerPage;
        //    return GetSetSql(sql, startValue, resultsPerPage, parameters);
        //}

        //public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        //{
        //    var toSkip = firstResult;// page * resultsPerPage;
        //    var topLimit = firstResult + maxResults;

        //    var sb = new StringBuilder();
        //    sb.AppendLine("SELECT * FROM (");
        //    sb.AppendLine("SELECT \"_ss_dapper_1_\".*, ROWNUM RNUM FROM (");
        //    sb.Append(sql);
        //    sb.AppendLine(") \"_ss_dapper_1_\"");
        //    sb.AppendLine("WHERE ROWNUM <= :topLimit) \"_ss_dapper_2_\" ");
        //    sb.AppendLine("WHERE \"_ss_dapper_2_\".RNUM > :toSkip");
        //    parameters.Add(":topLimit", topLimit);
        //    parameters.Add(":toSkip", toSkip);
        //    return sb.ToString();
        //}


        #endregion

        #region Ext



        string BuildCastState(object castObject, string targetDbTypeString, bool needToFormat = true)
        {
            
            return ("CAST(" + (castObject) + " AS " + targetDbTypeString + ")");

        }
        string DbFunction_DATEADD(string interval, object value, object addValue)
        {
            /*
             * Just support hour/minute/second
             * systimestamp + numtodsinterval(1,'HOUR')
             * sysdate + numtodsinterval(50,'MINUTE')
             * sysdate + numtodsinterval(45,'SECOND')
             */
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(value);
            sb.Append(" + NUMTODSINTERVAL(");
            sb.Append((addValue));
            sb.Append(",'");
            sb.Append((interval));
            sb.Append("'))");
            return sb.ToString();
        }
        string DbFunction_DATEPART(string interval, object value, bool castToTimestamp = false)
        {
            /* cast(to_char(sysdate,'yyyy') as number) */

            StringBuilder sb = new StringBuilder();
            sb.Append("CAST(TO_CHAR(");
            if (castToTimestamp)
            {
                sb.Append(BuildCastState(value, "TIMESTAMP"));
            }
            else
                sb.Append(value);

            sb.Append(",'");
            sb.Append((interval));
            sb.Append("') AS NUMBER)");
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
            sb.Append("ROUND(TO_NUMBER(");
            sb.Append(enddate);
            sb.Append(" - ");
            sb.Append(startdate);
            sb.Append(") ");
            if (interval == "HOUR")
            {
                sb.Append("* 24");
            }
            else if (interval == "MINUTE")
            {
                sb.Append("* 24 * 60");
            }
            else if (interval == "SECOND")
            {
                sb.Append("* 24 * 60 * 60");
            }
            else if (interval == "MILLISECOND")
            {
                sb.Append("* 24 * 60 * 60 * 1000");
            }
            else if (interval == "YEAR")
            {
                sb.Append("/ 365");
            }
            else if (interval == "MONTH")
            {
                sb.Append("/ 30");
            }
            else if (interval == "DAY")
            {
            }
            else if (interval == "MICROSECOND")
            {
                sb.Append("* 24 * 60 * 60 * 1000 * 1000");
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
                string result = ConvertDateTimeInOracle(v);
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

        private string ConvertDateTimeInOracle(object castObject, bool addQuot = true)
        {
            string result = "";
            //result = ("TO_TIMESTAMP('");
            //result += castObject;
            //result += ("','yyyy-mm-dd hh24:mi:ssxff')");

            if (addQuot == true)
            {
                result = ("TO_DATE('");
                result += castObject;
                result += ("','yy-mm-dd hh24:mi:ss')");
            }
            else
            {
                result = ("TO_DATE(");
                result += castObject;
                result += (",'yy-mm-dd hh24:mi:ss')");
            }
            


            return result;
        }
        public override string ConvertSqlValue(object castObject, Type type)
        {
            string result = "";

            if (type == ResolveConstants.TypeOfDateTime)
            {
                //result = ("TO_TIMESTAMP(");
                //result += castObject;
                //result += (",'yyyy-mm-dd hh24:mi:ssxff')");

                result = ConvertDateTimeInOracle(castObject, false);
                return result;
            }
            else if (type == ResolveConstants.TypeOfString)
            {
                result = ("TO_CHAR(");
                result += castObject;
                result += (")");

                return result;
            }
            else if (type == ResolveConstants.TypeOfBoolean)
            {
                if (castObject != null)
                {
                    var str = castObject.ToString().ToUpper().Trim('\'');
                    if (str == "TRUE" || str == "1")
                    {
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                }

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
            //模糊搜索
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
                //返回的是一个长度为 16 的 byte[]
                //// "SYS_GUID()";
                result = "(substr(lower(RAWTOHEX(sys_guid())),1,8) || '-'||substr(lower(RAWTOHEX(sys_guid())),9,4) || '-' || substr(lower(RAWTOHEX(sys_guid())),13,4) || '-' || substr(lower(RAWTOHEX(sys_guid())),17,4) || '-' || substr(lower(RAWTOHEX(sys_guid())),21,12))";
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
                result = ("DBMS_RANDOM.VALUE");
            }
            else if (functionType == DbFunctionType.IfNull)
            {
                result = ("NVL(" + param0 + "," + parameters[1] + ")");
            }
            //Add DateTime 函数
            else if (functionType == DbFunctionType.AddYears)
            {
                /* add_months(systimestamp,12 * 2) */
                result = ("ADD_MONTHS(");
                result += param0;
                result += (",12 * ");
                result += (parameters[1]);
                result += (")");

            }
            else if (functionType == DbFunctionType.AddMonths)
            {
                /* add_months(systimestamp,2) */

                result = ("ADD_MONTHS(");
                result += param0;
                result += (",");
                result += (parameters[1]);
                result += (")");

            }
            else if (functionType == DbFunctionType.AddDays)
            {
                /* (systimestamp + 3) */

                result = ("(");
                result += param0;
                result += (" + ");
                result += (parameters[1]);
                result += (")");

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
                throw new NotSupportedException("ORACLE is not supports DbFunctionType.AddMilliseconds");
                //result = DbFunction_DATEADD("MILLISECOND", param0, parameters[1]);
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
            object v = v2;// ConvertSqlValue(v2, v2.GetType());

            if (member == ResolveConstants.PropertyInfo_DateTime_Year)
            {
                sql = DbFunction_DATEPART("yyyy", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Month)
            {
                sql = DbFunction_DATEPART("mm", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Day)
            {
                sql = DbFunction_DATEPART("dd", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Hour)
            {
                sql = DbFunction_DATEPART("hh24", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Minute)
            {
                sql = DbFunction_DATEPART("mi", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Second)
            {
                sql = DbFunction_DATEPART("ss", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_Millisecond)
            {
                /* exp.Expression must be TIMESTAMP,otherwise there will be an error occurred. */
                sql = DbFunction_DATEPART("ff3", v);
                return true;
            }

            if (member == ResolveConstants.PropertyInfo_DateTime_DayOfWeek)
            {
                // CAST(TO_CHAR(SYSDATE,'D') AS NUMBER) - 1

                string tmp = ("(");
                tmp += DbFunction_DATEPART("D", v);
                tmp += (" - 1)");
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
                    valueOfTime = "SYSDATE";// ("SYSTIMESTAMP");//SYSDATE
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_UtcNow)
                {
                    valueOfTime =("SYS_EXTRACT_UTC(SYSTIMESTAMP)");
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Today)
                {
                    valueOfTime = "TRUNC(SYSDATE,'DD')";
                }
                else if (member == ResolveConstants.PropertyInfo_DateTime_Date)//日期部分
                {
                    valueOfTime = "TRUNC(" + value + ",'DD')";
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
            str = "regexp_like("+ colName + ", '" + regex + "')"  ;
            return str;
        }
        #endregion
    }
}