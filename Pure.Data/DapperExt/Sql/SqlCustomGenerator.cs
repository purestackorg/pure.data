using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pure.Data.Sql
{
    public interface ISqlCustomGenerator
    {
        IDapperExtensionsConfiguration Configuration { get; }
        ISqlGenerator SqlGenerator { get; }

        string Count(string tableName, IDictionary<string, object> conditions, out IDictionary<string, object> realParameters);
        string Select(string tableName, string[] columnNames, IDictionary<string, object> conditions, IList<ISort> sort, out IDictionary<string, object> realParameters);
        string Insert(string tableName, IDictionary<string, object> parameters, out IDictionary<string, object> realParameters);
        string Update(string tableName, IDictionary<string, object> parameters, IDictionary<string, object> conditions, out IDictionary<string, object> realParameters);
        string Delete(string tableName, IDictionary<string, object> conditions, out IDictionary<string, object> realParameters);



        string Update(string tableName, IDictionary<string, object> parameters, IPredicate conditions, out IDictionary<string, object> realParameters);
        string Delete(string tableName, IPredicate conditions, out IDictionary<string, object> realParameters);
    }

    public class SqlCustomGenerator : ISqlCustomGenerator
    {
        ISqlGenerator _SqlGenerator;
        public SqlCustomGenerator(ISqlGenerator sqlBaseGen)
        {
            _SqlGenerator = sqlBaseGen;
        }
        public ISqlGenerator SqlGenerator
        {
            get { return _SqlGenerator; }
        }
        public IDapperExtensionsConfiguration Configuration
        {
            get { return SqlGenerator.Configuration; }
        }
        #region Private methods
        private string GetTableName(string name)
        {
            return SqlGenerator.GetTableName(name);
        }

        private string GetColumnName(string tableName, string name, string alias = null)
        {
            return Configuration.Dialect.GetColumnName(GetTableName(tableName), name, alias);
        }

        private string ParamNamePrefix = "P";

        private string BuildWhereSql(string tableName, IDictionary<string, object> conditions,  ref int index)
        {
            var tempIndex = index;
            //index += primaryKeyValuePair.Count;
            return string.Join(" AND ", conditions.Select((x, i) => x.Value == null || x.Value == DBNull.Value ? string.Format("{0} IS NULL", GetColumnName(tableName, x.Key)) : string.Format("{0} = {1}{2}", GetColumnName(tableName, x.Key),  Configuration.Dialect.ParameterPrefix, ParamNamePrefix + (tempIndex + i).ToString())).ToArray());
        }
        #endregion
       




        #region Base Sql Gen Ext
        public virtual string Insert(string tableName, IDictionary<string, object> parameters, out IDictionary<string, object> realParameters)
        {

            if (parameters == null || !parameters.Any())
            {
                throw new ArgumentException("No parameters .");
            }

            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();

            if (parameters != null && parameters.Count > 0)
            {

                foreach (var item in parameters)
                {
                    if (index > 0)
                        sb.Append(", ");

                    string paramName = ParamNamePrefix + index.ToString();
                    sb.AppendFormat("{0}{1}",  Configuration.Dialect.ParameterPrefix, paramName);

                    out_parameters.Add(paramName, item.Value);
                    index++;
                }

            }


            var columnNames = parameters.Select(p => GetColumnName(tableName, p.Key));
          
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                       GetTableName(tableName),
                                       columnNames.AppendStrings(),
                                       sb.ToString());

            realParameters = out_parameters;
             
            return sql;
        }

        public virtual string Update(string tableName, IDictionary<string, object> parameters,  IDictionary<string, object> conditions, out IDictionary<string, object> realParameters)
        {

            if (parameters == null || !parameters.Any())
            {
                throw new ArgumentException("No parameters .");
            }
             
            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();

            foreach (var item in parameters)
            {
                if (index > 0)
                    sb.Append(", ");

                string paramName = ParamNamePrefix + index.ToString();
                sb.AppendFormat("{0} = {1}{2}", GetColumnName(tableName, item.Key), Configuration.Dialect.ParameterPrefix, paramName);
                var value = item.Value;
                out_parameters.Add(paramName, value);
                index++;
            }

            var setSql = sb.ToString();

            string whereStr = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereStr = "WHERE " + BuildWhereSql(tableName, conditions, ref index);

                int temIndex = out_parameters.Count;
                foreach (var item in conditions)
                {
                    out_parameters.Add(ParamNamePrefix + temIndex, item.Value);
                    temIndex++;
                }

            }

           

            realParameters = out_parameters;
            
            return string.Format("UPDATE {0} SET {1} {2}",
                GetTableName(tableName),
                setSql ,
                whereStr);


            
        }

        public virtual string Delete(string tableName,   IDictionary<string, object> conditions, out IDictionary<string, object> realParameters)
        {
            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(tableName)));

            string whereStr = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereStr = " WHERE " + BuildWhereSql(tableName, conditions, ref index);

                int temIndex = out_parameters.Count;
                foreach (var item in conditions)
                {
                    out_parameters.Add(ParamNamePrefix + temIndex, item.Value);
                    temIndex++;
                }

            }

            sql.Append(whereStr);
            realParameters = out_parameters;

            return sql.ToString();
        }

        public virtual string Select(string tableName, string[] columnNames, IDictionary<string, object> conditions,  IList<ISort> sort, out IDictionary<string, object> realParameters)
        {
            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();

            string colString = "*";
            if (columnNames != null && columnNames.Length > 0)
            {
                colString = columnNames.AppendStrings();
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} FROM {1}", colString, GetTableName(tableName)));

            string whereStr = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereStr = " WHERE " + BuildWhereSql(tableName, conditions, ref index);

                int temIndex = out_parameters.Count;
                foreach (var item in conditions)
                {
                    out_parameters.Add(ParamNamePrefix + temIndex, item.Value);
                    temIndex++;
                }

            }
            sql.Append(whereStr);

            if (sort != null && sort.Any())
            {
                sql.Append(" ORDER BY ")
                    .Append(sort.Select(s => GetColumnName(tableName, s.PropertyName) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }

             
            realParameters = out_parameters;

            return sql.ToString();
        }


        public virtual string Count(string tableName, IDictionary<string, object> conditions,  out IDictionary<string, object> realParameters)
        {
            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();

            string colString = "COUNT(1)";
            
            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} FROM {1}", colString, GetTableName(tableName)));

            string whereStr = "";
            if (conditions != null && conditions.Count > 0)
            {
                whereStr = " WHERE " + BuildWhereSql(tableName, conditions, ref index);

                int temIndex = out_parameters.Count;
                foreach (var item in conditions)
                {
                    out_parameters.Add(ParamNamePrefix + temIndex, item.Value);
                    temIndex++;
                }

            }
            sql.Append(whereStr);
 
            realParameters = out_parameters;

            return sql.ToString();
        }






        public virtual string Update(string tableName, IDictionary<string, object> parameters, IPredicate conditions, out IDictionary<string, object> realParameters)
        {

            if (parameters == null || !parameters.Any())
            {
                throw new ArgumentException("No parameters .");
            }

            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();

            foreach (var item in parameters)
            {
                if (index > 0)
                    sb.Append(", ");

                string paramName = ParamNamePrefix + index.ToString();
                sb.AppendFormat("{0} = {1}{2}", GetColumnName(tableName, item.Key), Configuration.Dialect.ParameterPrefix, paramName);
                var value = item.Value;
                out_parameters.Add(paramName, value);
                index++;
            }

            var setSql = sb.ToString();

            string whereStr = "";
            if (conditions != null  )
            {
                 
                whereStr = "WHERE " + conditions.GetSql(SqlGenerator, out_parameters); 
            }



            realParameters = out_parameters;

            return string.Format("UPDATE {0} SET {1} {2}",
                GetTableName(tableName),
                setSql,
                whereStr);



        }

        public virtual string Delete(string tableName, IPredicate conditions, out IDictionary<string, object> realParameters)
        {
            int index = 0;
            IDictionary<string, object> out_parameters = new Dictionary<string, object>();

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(tableName)));

            string whereStr = "";

            if (conditions != null)
            {

                whereStr = "WHERE " + conditions.GetSql(SqlGenerator, out_parameters);
            }

           
            sql.Append(whereStr);
            realParameters = out_parameters;

            return sql.ToString();
        }







        #endregion



    }
}