
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pure.Data.Sql
{
    public class MySqlDialect : SqlDialectBase
    {
        public override DatabaseType databaseType
        {
            get { return DatabaseType.MySql; }
        }
        public override char OpenQuote
        {
            get { return '`'; }
        }

        public override char CloseQuote
        {
            get { return '`'; }
        }

        public override string GetIdentitySql(IClassMapper table)
        {
            return "SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS ID";
        }
        public override string GetCountSql(string sql)
        {
            return string.Format("SELECT COUNT(1) FROM ({0}) _purecount ", sql);
        }
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (page <= 0)
            {
                page = 1;
            }
            int startValue = (page - 1) * resultsPerPage;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            string result = string.Format("{0} LIMIT @firstResult, @maxResults", sql);
            parameters.Add("@firstResult", firstResult);
            parameters.Add("@maxResults", maxResults);
            return result;
        }
    }
}