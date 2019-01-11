using System;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.Sql
{
    public class SqliteDialect : SqlDialectBase
    {
        public override DatabaseType databaseType
        {
            get { return DatabaseType.SQLite; }
        }
        public override string GetIdentitySql(IClassMapper table)
        {
            return "SELECT LAST_INSERT_ROWID() AS [Id]";
        }

        public override string GetCountSql(string sql)
        {
            return string.Format("SELECT COUNT(1) FROM ({0})", sql);
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
    }
}
