using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data.Sql
{
    public class FirebirdDialect : SqlDialectBase
    {
        public override DatabaseType databaseType
        {
            get { return DatabaseType.Firebird; }
        }
        public override string GetIdentitySql(IClassMapper table)
        {
            return "";
            //get { return "select last_insert_id();"; }
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
            StringBuilder sqlT = new StringBuilder("SELECT ");

            if (pageNumber > 0)
                sqlT.AppendFormat("FIRST {0} ", pageNumber);

            if (maxResults > 0)
                sqlT.AppendFormat("SKIP {0} ", maxResults);


            string tmp = sql.ToUpper();
            sql = sqlT.ToString() +" "+ tmp.Substring(tmp.IndexOf("SELECT") + "SELECT".Length + 1) ;


            return sql;
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
