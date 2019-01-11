using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data.Sql
{
    public class PostgreSqlDialect : SqlDialectBase
    {
        public override DatabaseType databaseType
        {
            get { return DatabaseType.PostgreSQL; }
        }
        public override string GetIdentitySql(IClassMapper table)
        {
            return "SELECT LASTVAL() AS Id";
            //get { return "select last_insert_id();"; }
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
			int startValue = (page-1) * resultsPerPage;
			return GetSetSql(sql, startValue, resultsPerPage, parameters);
		}
		
		public override string GetSetSql(string sql, int pageNumber, int maxResults, IDictionary<string, object> parameters)
		{
			string result = string.Format("{0} LIMIT @maxResults OFFSET @pageStartRowNbr", sql);
			parameters.Add("@maxResults", maxResults);
			parameters.Add("@pageStartRowNbr", pageNumber * maxResults);
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
    }

}
