
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.Sql
{
    public class OracleDialect : SqlDialectBase
    {

        public override DatabaseType databaseType
        {
            get { return DatabaseType.Oracle; }
        }
        public override string GetCountSql(string sql)
        {
            return string.Format("SELECT COUNT(1) FROM ({0})", sql);
        }
        //from Simple.Data.Oracle implementation https://github.com/flq/Simple.Data.Oracle/blob/master/Simple.Data.Oracle/OraclePager.cs
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var toSkip = (page-1) * resultsPerPage;
            var topLimit = (page) * resultsPerPage;
         
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
            if (value != null && value[0]=='`')
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

        public override string GetIdentitySql(IClassMapper table)
        {
            //if (table != null && !string.IsNullOrEmpty(table.SequenceName))
            //    return string.Format("SELECT {0}.NEXTVAL FROM DUAL", table.SequenceName);
            return "";// "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
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
        
    }
}