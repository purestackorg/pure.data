using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentExpressionSQL.Sql
{
    
    public interface ISqlDialectProvider
    {
        char OpenQuote { get; }
        char CloseQuote { get; }
        string BatchSeperator { get; }
        bool SupportsMultipleStatements { get; }
        char ParameterPrefix { get; }
        string ColumnAsAliasString { get; }
        string EmptyExpression { get; }
        string GetIdentitySql(string tableName);

        string GetTableName(string schemaName, string tableName, string alias);
        string GetColumnName(string prefix, string columnName, string alias);
        string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters);
        string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters);

        bool IsQuoted(string value);
        string QuoteString(string value);

        string ConvertRegexStr(string colName, string regex);

        DateTimeDto ConvertDateTime(MemberInfo member, object value);
        string ConvertSqlValue(object castObject, Type type);
        object FormatValue(object v, Type type = null);
        string ConvertDbFunction(DbFunctionType functionType, params object[] parameters);
        string DoCaseWhen(List<CaseThenExpressionPair> listCase);
    }

    public abstract class SqlDialectBaseProvider : ISqlDialectProvider
    {
        #region Base
        public virtual char OpenQuote
        {
            get { return '"'; }
        }

        public virtual char CloseQuote
        {
            get { return '"'; }
        }

        public virtual string BatchSeperator
        {
            get { return ";" + Environment.NewLine; }
        }

        public virtual bool SupportsMultipleStatements
        {
            get { return true; }
        }

        public virtual char ParameterPrefix
        {
            get
            {
                return '@';
            }
        }
        public virtual string ColumnAsAliasString
        {
            get
            {
                return "AS ";
            }
        }
        public string EmptyExpression
        {
            get
            {
                return "1=1";
            }
        }

        public virtual string GetTableName(string schemaName, string tableName, string alias)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("TableName", "tableName cannot be null or empty.");
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                result.AppendFormat(QuoteString(schemaName) + ".");
            }

            result.AppendFormat(QuoteString(tableName));

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }
            return result.ToString();
        }

        public virtual string GetColumnName(string prefix, string columnName, string alias)
        {
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentNullException("ColumnName", "columnName cannot be null or empty.");
            }

            StringBuilder result = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                result.AppendFormat(QuoteString(prefix) + ".");
            }

            if (columnName != "*")
            {
                result.AppendFormat(QuoteString(columnName));
            }
            else
            {
                result.AppendFormat(columnName);
            }

            if (!string.IsNullOrWhiteSpace(alias))
            {
                result.AppendFormat(" AS {0}", QuoteString(alias));
            }

            return result.ToString();
        }

  

        public abstract string GetIdentitySql(string tableName);
        public abstract string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters);
        public abstract string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters);

        public virtual bool IsQuoted(string value)
        {
            if (value.Trim()[0] == OpenQuote)
            {
                return value.Trim().Last() == CloseQuote;
            }

            return false;
        }

        public virtual string QuoteString(string value)
        {
            return IsQuoted(value) ? value : string.Format("{0}{1}{2}", OpenQuote, value.Trim(), CloseQuote);
        }

        public virtual string UnQuoteString(string value)
        {
            return IsQuoted(value) ? value.Substring(1, value.Length - 2) : value;
        }


        #endregion
     
        #region Ext
        public abstract DateTimeDto ConvertDateTime(MemberInfo member, object value);
        public abstract string ConvertSqlValue(object castObject, Type type);
        public abstract object FormatValue(object v, Type type = null);
        public abstract string ConvertDbFunction(DbFunctionType functionType, params object[] parameters);
        public abstract string DoCaseWhen(List<CaseThenExpressionPair> listCase);

        public abstract string ConvertRegexStr(string colName, string regex);

        #endregion
    }
}