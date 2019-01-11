using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data.Sql
{
    public class SqlCeDialect : SqlDialectBase
    {
        public override DatabaseType databaseType
        {
            get { return DatabaseType.SqlCe; }
        }
        public override char OpenQuote
        {
            get { return '['; }
        }

        public override char CloseQuote
        {
            get { return ']'; }
        }

        public override bool SupportsMultipleStatements
        {
            get { return false; }
        }

        public override string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("TableName");
            }

            StringBuilder result = new StringBuilder();
            result.Append(OpenQuote);
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat("{0}_", schemaName);
            }

            result.AppendFormat("{0}{1}", tableName, CloseQuote);


            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}{1}{2}", OpenQuote, alias, CloseQuote);
            }

            return result.ToString();
        }

        public override string GetIdentitySql(IClassMapper table)
        {
            return "SELECT CAST(@@IDENTITY AS BIGINT) AS [Id]";
        }
        public override string GetCountSql(string sql)
        {
            return string.Format("SELECT COUNT(1) FROM ({0}) AS _purecount", sql);
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
            string result = string.Format("{0} OFFSET @firstResult ROWS FETCH NEXT @maxResults ROWS ONLY", sql);
            parameters.Add("@firstResult", firstResult);
            parameters.Add("@maxResults", maxResults);
            return result;            
        }
    }
}