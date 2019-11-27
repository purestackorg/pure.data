using Pure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pure.Data
{
    public class SqlBuilder
    {
        Dictionary<string, Clauses> data = new Dictionary<string, Clauses>();
        int seq;

        class Clause
        {
            public string Sql { get; set; }
            public object Parameters { get; set; }
            public bool IsInclusive { get; set; }
        }

        class Clauses : List<Clause>
        {
            string joiner;
            string prefix;
            string postfix;

            public Clauses(string joiner, string prefix = "", string postfix = "")
            {
                this.joiner = joiner;
                this.prefix = prefix;
                this.postfix = postfix;
            }

            public string ResolveClauses(DynamicParameters p)
            {
                foreach (var item in this)
                {
                    p.AddDynamicParams(item.Parameters);
                }
                return this.Any(a => a.IsInclusive)
                   ? prefix +
                     string.Join(joiner,
                         this.Where(a => !a.IsInclusive)
                             .Select(c => c.Sql)
                             .Union(new[]
                             {
                                  " ( " +
                                  string.Join(" OR ", this.Where(a => a.IsInclusive).Select(c => c.Sql).ToArray()) +
                                  " ) "
                             }).ToArray()) + postfix
                   : prefix + string.Join(joiner, this.Select(c => c.Sql).ToArray()) + postfix;

                //return prefix + string.Join(joiner, this.Select(c => c.Sql)) + postfix;
            }
        }

        public class Template
        {
            readonly string sql;
            readonly SqlBuilder builder;
            readonly object initParams;
            int dataSeq = -1; // Unresolved
            private IDatabase Database { get; set; }

            public Template(SqlBuilder builder, string sql, dynamic parameters, IDatabase db)
            {
                this.initParams = parameters;
                this.sql = sql;
                this.builder = builder;
                this.Database = db;
            }

            //static System.Text.RegularExpressions.Regex regex =
            //    new System.Text.RegularExpressions.Regex(@"\/\*\*.+\*\*\/", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);

            private static readonly Regex regex = new Regex(@"\/\*\*.+?\*\*\/", RegexOptions.Compiled | RegexOptions.Multiline);

            void ResolveSql()
            {
                if (dataSeq != builder.seq)
                {
                    DynamicParameters p = new DynamicParameters(initParams);

                    rawSql = sql;

                    foreach (var pair in builder.data)
                    {
                        rawSql = rawSql.Replace("/**" + pair.Key + "**/", pair.Value.ResolveClauses(p));
                    }
                    parameters = p;

                    // replace all that is left with empty
                    rawSql = regex.Replace(rawSql, "");

                    dataSeq = builder.seq;
                }
            }
            public string ToSqlString(string prefixPara = "@", Pure.Data.DatabaseType dbType = Pure.Data.DatabaseType.SqlServer)
            {
                string sql = RawSql;
                if (!string.IsNullOrEmpty(RawSql))
                {
                    if (Parameters != null)
                    {
                        foreach (var p in ParameterDict)
                        {
                            sql = sql.Replace(prefixPara + p.Key, Database.SqlDialectProvider.FormatValue(p.Value, true).ToString());
                            //sql = sql.Replace(prefixPara + p.Key, FormatValue(p.Value, dbType));
                        }
                        
                    }
                }
                return sql;
            }
           
            public IDictionary<string, object> ParameterDict
            {
                get
                {
                    IDictionary<string, object> dict = new Dictionary<string, object>();
                    if (Parameters != null)
                    {
                        if (Parameters is IDictionary<string, object>)
                        {
                            dict = Parameters as IDictionary<string, object>;
                        }
                        else if (Parameters is DynamicParameters)
                        {
                            DynamicParameters dps = Parameters as DynamicParameters;
                            foreach (var pname in dps.ParameterNames)
                            {
                                if (!dict.ContainsKey(pname))
                                {
                                    dict.Add(pname, dps.Get<object>(pname));

                                }
                            }
                        }

                    }
                    return dict;
                }
            }

            string rawSql;
            object parameters;

            public string RawSql { get { ResolveSql(); return rawSql; } }
            public object Parameters { get { ResolveSql(); return parameters; } }





        }
        int dataSeq = -1; // Unresolved
        public void ResolveSql()
        {
            if (dataSeq != this.seq)
            {
                DynamicParameters p = new DynamicParameters();
                
                foreach (var pair in this.data)
                {
                    rawSql = rawSql +" "+ pair.Value.ResolveClauses(p);
                }
                parameters = p;
                
                dataSeq = this.seq;
            }
        }
        public string ToSqlString(string prefixPara = "@", Pure.Data.DatabaseType dbType = Pure.Data.DatabaseType.SqlServer)
        {
            string sql = Sql;
            if (!string.IsNullOrEmpty(sql))
            {
                if (Parameters != null)
                {
                    foreach (var p in ParameterDict)
                    {
                        sql = sql.Replace(prefixPara + p.Key, Database.SqlDialectProvider.FormatValue(p.Value, true).ToString());
                        //sql = sql.Replace(prefixPara + p.Key, FormatValue(p.Value, dbType));
                    }

                }
            }
            return sql;
        }

        public IDictionary<string, object> ParameterDict
        {
            get
            {
                IDictionary<string, object> dict = new Dictionary<string, object>();
                if (Parameters != null)
                {
                    if (Parameters is IDictionary<string, object>)
                    {
                        dict = Parameters as IDictionary<string, object>;
                    }
                    else if (Parameters is DynamicParameters)
                    {
                        DynamicParameters dps = Parameters as DynamicParameters;
                        foreach (var pname in dps.ParameterNames)
                        {
                            if (!dict.ContainsKey(pname))
                            {
                                dict.Add(pname, dps.Get<object>(pname));

                            }
                        }
                    }

                }
                return dict;
            }
        }

        string rawSql;
        object parameters;

        public string Sql { get { ResolveSql(); return rawSql; } }
        public object Parameters { get { ResolveSql(); return parameters; } }



        public virtual string ParameterPrefix { get { return Database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString(); } }
        private IDatabase Database { get;  set; }
        public SqlBuilder(IDatabase db)
        {
            Database = db;
        }

        public Template AddTemplate(string sql, dynamic parameters = null)
        {
            return new Template(this, sql, parameters, Database);
        }

        void AddClause(string name, string sql, object parameters, string joiner, string prefix = "", string postfix = "", bool isInclusive = false)
        {
            Clauses clauses;
            if (!data.TryGetValue(name, out clauses))
            {
                clauses = new Clauses(joiner, prefix, postfix);
                data[name] = clauses;
            }
            clauses.Add(new Clause { Sql = sql, Parameters = parameters, IsInclusive = isInclusive });
            seq++;
        }

        public SqlBuilder Intersect(string sql, dynamic parameters = null)
        {
            AddClause("intersect", sql, parameters, joiner: "\nINTERSECT\n ", prefix: "\n ", postfix: "\n");
            return this;
        }
        public SqlBuilder InnerJoin(string sql, dynamic parameters = null)
        {
            AddClause("innerjoin", sql, parameters, joiner: "\nINNER JOIN ", prefix: "\nINNER JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder LeftJoin(string sql, dynamic parameters = null)
        {
            AddClause("leftjoin", sql, parameters, joiner: "\nLEFT JOIN ", prefix: "\nLEFT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder RightJoin(string sql, dynamic parameters = null)
        {
            AddClause("rightjoin", sql, parameters, joiner: "\nRIGHT JOIN ", prefix: "\nRIGHT JOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder Where(string sql, dynamic parameters = null)
        {
            AddClause("where", sql, parameters, " AND ", prefix: "WHERE ", postfix: "\n");
            return this;
        }
        public SqlBuilder OrWhere(string sql, dynamic parameters = null)
        {
            AddClause("where", sql, parameters, " OR ", prefix: "WHERE ", postfix: "\n", isInclusive:true);
            return this;
        }

        public SqlBuilder OrderBy(string sql, dynamic parameters = null)
        {
            AddClause("orderby", sql, parameters, " , ", prefix: "ORDER BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Select(string sql, dynamic parameters = null)
        {
            AddClause("select", sql, parameters, " , ", prefix: "SELECT ", postfix: "\n");
            return this;
        }
        public SqlBuilder From(string sql, dynamic parameters = null)
        {
            AddClause("from", sql, parameters, " , ", prefix: "FROM ", postfix: "\n");
            return this;
        }
        private string ParamNamePrefix = "P";
      
        public SqlBuilder Update(string tableName, IDictionary<string, object> parameters, ConditionalModelCollections conditions)
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
                sb.AppendFormat("{0} = {1}{2}", tableName.GetColumnName(  item.Key, Database), ParameterPrefix, paramName);
                var value = item.Value;
                out_parameters.Add(paramName, value);
                index++;
            }

            var setSql = sb.ToString();

            //string whereStr = "";
            //if (conditions != null)
            //{
            //    whereStr = "WHERE " + conditions.GetSql(SqlGenerator, out_parameters);
            //    conditions.ConditionalModelToSql();
            //}


             
            string sql =  string.Format("UPDATE {0} SET {1} ",
                tableName.GetTableName(Database),
                setSql);

            AddClause("update", sql, out_parameters, " ", prefix: "", postfix: "\n");

            return Where(  conditions, tableName);
            
        }
        public SqlBuilder Delete(string tableName,  ConditionalModelCollections conditions)
        {
            int index = 0;
           // IDictionary<string, object> out_parameters = new Dictionary<string, object>();

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", tableName.GetTableName(Database)));

          

            string sql2 = sql.ToString();

            AddClause("delete", sql2, null, " ", prefix: "", postfix: "\n");

            return Where(conditions , tableName);

        }

        public SqlBuilder AddParameters(dynamic parameters)
        {
            AddClause("--parameters", "", parameters, "");
            return this;
        }

        public SqlBuilder Join(string sql, dynamic parameters = null)
        {
            AddClause("join", sql, parameters, joiner: "\nJOIN ", prefix: "\nJOIN ", postfix: "\n");
            return this;
        }

        public SqlBuilder GroupBy(string sql, dynamic parameters = null)
        {
            AddClause("groupby", sql, parameters, joiner: " , ", prefix: "\nGROUP BY ", postfix: "\n");
            return this;
        }

        public SqlBuilder Having(string sql, dynamic parameters = null)
        {
            AddClause("having", sql, parameters, joiner: "\nAND ", prefix: "HAVING ", postfix: "\n");
            return this;
        }

        public SqlBuilder Where(  ConditionalModelCollections models, string table)
        {
            if (models == null  )
            {
                return this;
            }
            return Where(  models.conditions, table);
        }
        public SqlBuilder Where(  List<IConditionalModel> models, string table)
        {
            if (models ==null || models.Count == 0)
            {
                return this;
            }
            var conSql = ConditionalModelToSql(  Database,table,  models);
            if (conSql.Key == null || conSql.Key == "")
            {
                return this;

            }

            AddClause("where", conSql.Key, conSql.Value, " ", prefix: "WHERE ", postfix: "\n");
            return this;
        }

        public KeyValuePair<string, DynamicParameters> ConditionalModelToSql(IDatabase Database, string table, List<IConditionalModel> models, int beginIndex = 0)
        {
            DynamicParameters parameters = new DynamicParameters();
            if (models == null || models.Count == 0) return new KeyValuePair<string, DynamicParameters>();
            StringBuilder builder = new StringBuilder();
            
            foreach (var model in models)
            {
                if (model is ConditionalModel)
                {
                    var item = model as ConditionalModel;
                    var index = models.IndexOf(item) + beginIndex;
                    var type = index == 0 ? "" : "AND";
                    if (beginIndex > 0)
                    {
                        type = null;
                    }
                    string temp = " {0} {1} {2} {3} ";
                    string parameterName = string.Format("{0}P{2}_{1}", ParameterPrefix, item.FieldName, index);
                    if (parameterName.Contains("."))
                    {
                        parameterName = parameterName.Replace(".", "_");
                    }
                    switch (item.ConditionalType)
                    {
                        case ConditionalType.Equal:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "=", parameterName);
                            parameters.Add(parameterName, GetFieldValue(item));
                            break;
                        case ConditionalType.Like:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "LIKE", parameterName);
                            parameters.Add(parameterName, "%" + item.FieldValue + "%");
                            break;
                        case ConditionalType.GreaterThan:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), ">", parameterName);
                            parameters.Add(parameterName, GetFieldValue(item));
                            break;
                        case ConditionalType.GreaterThanOrEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), ">=", parameterName);
                            parameters.Add(parameterName, GetFieldValue(item));
                            break;
                        case ConditionalType.LessThan:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "<", parameterName);
                            parameters.Add(parameterName, GetFieldValue(item));
                            break;
                        case ConditionalType.LessThanOrEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "<=", parameterName);
                            parameters.Add(parameterName, GetFieldValue(item));
                            break;
                        case ConditionalType.In:
                            if (item.FieldValue == null) item.FieldValue = string.Empty;
                            var inValue1 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals(table, Database) + ")");
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "IN", inValue1);
                            parameters.Add(parameterName, item.FieldValue);
                            break;
                        case ConditionalType.NotIn:
                            if (item.FieldValue == null) item.FieldValue = string.Empty;
                            var inValue2 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals(table, Database) + ")");
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "NOT IN", inValue2);
                            parameters.Add(parameterName, item.FieldValue);
                            break;
                        case ConditionalType.LikeLeft:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "LIKE", parameterName);
                            parameters.Add(parameterName, item.FieldValue + "%");
                            break;
                        case ConditionalType.NoLike:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), " NOT LIKE", parameterName);
                            parameters.Add(parameterName, item.FieldValue + "%");
                            break;
                        case ConditionalType.LikeRight:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "LIKE", parameterName);
                            parameters.Add(parameterName, "%" + item.FieldValue);
                            break;
                        case ConditionalType.NoEqual:
                            builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "<>", parameterName);
                            parameters.Add(parameterName, item.FieldValue);
                            break;
                        case ConditionalType.IsNullOrEmpty:
                            builder.AppendFormat("{0} ({1}) OR ({2}) ", type, item.FieldName.ToSqlFilter(table, Database) + " IS NULL ", item.FieldName.ToSqlFilter(table, Database) + " = '' ");
                            parameters.Add(parameterName, item.FieldValue);
                            break;
                        case ConditionalType.IsNot:
                            if (item.FieldValue == null)
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), " IS NOT ", "NULL");
                            }
                            else
                            {
                                builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(table, Database), "<>", parameterName);
                                parameters.Add(parameterName, item.FieldValue);
                            }
                            break;
                        case ConditionalType.Regex:
                            string regexStr =Database.SqlDialectProvider.ConvertRegexStr(item.FieldName.ToSqlFilter(table, Database), item.FieldValue);
                            builder.AppendFormat(temp, type, regexStr, "", "");

                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var item = model as ConditionalCollections;
                    if (item != null && item.ConditionalList != null)
                    {
                        foreach (var con in item.ConditionalList)
                        {
                            var index = item.ConditionalList.IndexOf(con);
                            var isFirst = index == 0;
                            var isLast = index == (item.ConditionalList.Count - 1);
                            if (models.IndexOf(item) == 0 && index == 0 && beginIndex == 0)
                            {
                                builder.AppendFormat(" ( ");

                            }
                            else if (isFirst)
                            {
                                builder.AppendFormat(" {0} ( ", con.Key.ToString().ToUpper());
                            }
                            List<IConditionalModel> conModels = new List<IConditionalModel>();
                            conModels.Add(con.Value);
                            var childSqlInfo = ConditionalModelToSql( Database,table ,  conModels, 1000 * (1 + index) + models.IndexOf(item));
                            if (!isFirst)
                            {

                                builder.AppendFormat(" {0} ", con.Key.ToString().ToUpper());
                            }
                            builder.Append(childSqlInfo.Key);
                            parameters.AddDynamicParams(childSqlInfo.Value);

                            if (isLast)
                            {
                                builder.Append(" ) ");
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            return new KeyValuePair<string, DynamicParameters>(builder.ToString(), parameters);
        }

        //private KeyValuePair<string, SqlBuilderParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0)
        //{
           
        //    if (models == null || models.Count == 0) return new KeyValuePair<string, SqlBuilderParameter[]>();
        //    StringBuilder builder = new StringBuilder();
        //    List<SqlBuilderParameter> parameters = new List<SqlBuilderParameter>(); 
        //    foreach (var model in models)
        //    {
        //        if (model is ConditionalModel)
        //        {
        //            var item = model as ConditionalModel;
        //            var index = models.IndexOf(item) + beginIndex;
        //            var type = index == 0 ? "" : "AND";
        //            if (beginIndex > 0)
        //            {
        //                type = null;
        //            }
        //            string temp = " {0} {1} {2} {3} ";
        //            string parameterName = string.Format("{0}P{2}_{1}", ParameterPrefix, item.FieldName, index);
        //            if (parameterName.Contains("."))
        //            {
        //                parameterName = parameterName.Replace(".", "_");
        //            }
        //            switch (item.ConditionalType)
        //            {
        //                case ConditionalType.Equal:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, GetFieldValue(item)));
        //                    break;
        //                case ConditionalType.Like:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, "%" + item.FieldValue + "%"));
        //                    break;
        //                case ConditionalType.GreaterThan:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, GetFieldValue(item)));
        //                    break;
        //                case ConditionalType.GreaterThanOrEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">=", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, GetFieldValue(item)));
        //                    break;
        //                case ConditionalType.LessThan:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, GetFieldValue(item)));
        //                    break;
        //                case ConditionalType.LessThanOrEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<=", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, GetFieldValue(item)));
        //                    break;
        //                case ConditionalType.In:
        //                    if (item.FieldValue == null) item.FieldValue = string.Empty;
        //                    var inValue1 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "IN", inValue1);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue));
        //                    break;
        //                case ConditionalType.NotIn:
        //                    if (item.FieldValue == null) item.FieldValue = string.Empty;
        //                    var inValue2 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "NOT IN", inValue2);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue));
        //                    break;
        //                case ConditionalType.LikeLeft:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue + "%"));
        //                    break;
        //                case ConditionalType.NoLike:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " NOT LIKE", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue + "%"));
        //                    break;
        //                case ConditionalType.LikeRight:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, "%" + item.FieldValue));
        //                    break;
        //                case ConditionalType.NoEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue));
        //                    break;
        //                case ConditionalType.IsNullOrEmpty:
        //                    builder.AppendFormat("{0} ({1}) OR ({2}) ", type, item.FieldName.ToSqlFilter() + " IS NULL ", item.FieldName.ToSqlFilter() + " = '' ");
        //                    parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue));
        //                    break;
        //                case ConditionalType.IsNot:
        //                    if (item.FieldValue == null)
        //                    {
        //                        builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " IS NOT ", "NULL");
        //                    }
        //                    else
        //                    {
        //                        builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
        //                        parameters.Add(new SqlBuilderParameter(parameterName, item.FieldValue));
        //                    }
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            var item = model as ConditionalCollections;
        //            if (item != null && item.ConditionalList != null  )
        //            {
        //                foreach (var con in item.ConditionalList)
        //                {
        //                    var index = item.ConditionalList.IndexOf(con);
        //                    var isFirst = index == 0;
        //                    var isLast = index == (item.ConditionalList.Count - 1);
        //                    if (models.IndexOf(item) == 0 && index == 0 && beginIndex == 0)
        //                    {
        //                        builder.AppendFormat(" ( ");

        //                    }
        //                    else if (isFirst)
        //                    {
        //                        builder.AppendFormat(" {0} ( ", con.Key.ToString().ToUpper());
        //                    }
        //                    List<IConditionalModel> conModels = new List<IConditionalModel>();
        //                    conModels.Add(con.Value);
        //                    var childSqlInfo = ConditionalModelToSql(conModels, 1000 * (1 + index) + models.IndexOf(item));
        //                    if (!isFirst)
        //                    {

        //                        builder.AppendFormat(" {0} ", con.Key.ToString().ToUpper());
        //                    }
        //                    builder.Append(childSqlInfo.Key);
        //                    parameters.AddRange(childSqlInfo.Value);
        //                    if (isLast)
        //                    {
        //                        builder.Append(" ) ");
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return new KeyValuePair<string, SqlBuilderParameter[]>(builder.ToString(), parameters.ToArray());
        //}

        private static object GetFieldValue(ConditionalModel item)
        {
            if (item.FieldValueConvertFunc != null)
                return item.FieldValueConvertFunc(item.FieldValue);
            else
                return item.FieldValue;
        }

    }



    public enum ConditionalType
    {
        Equal = 0,
        Like = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5,
        In = 6,
        NotIn = 7,
        LikeLeft = 8,
        LikeRight = 9,
        NoEqual = 10,
        IsNullOrEmpty = 11,
        IsNot = 12,
        NoLike = 13,
        Regex = 14,
    }
    public enum WhereType
    {
        And = 0,
        Or = 1
    }
    public interface IConditionalModel
    {
        string FieldName { get; set; }
    }
    public class ConditionalCollections : IConditionalModel
    {
        public List<KeyValuePair<WhereType, ConditionalModel>> ConditionalList { get; set; }
        public string FieldName { get; set; }

    }

    public class ConditionalModelCollections
    {
        public List<IConditionalModel> conditions = new List<IConditionalModel>();

        public void Add(IConditionalModel c) {
            //if (c.FieldName == null ||  !conditions.Any(p=>p.FieldName == c.FieldName))
            //{
            //    conditions.Add(c);
            //}
            conditions.Add(c);

        }

        //private static object GetFieldValue(ConditionalModel item)
        //{
        //    if (item.FieldValueConvertFunc != null)
        //        return item.FieldValueConvertFunc(item.FieldValue);
        //    else
        //        return item.FieldValue;
        //}
        //public KeyValuePair<string, DynamicParameters> ConditionalModelToSql(IDatabase Database)
        //{
        //    var models = this.conditions;
        //    var ParameterPrefix = Database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();
        //    DynamicParameters parameters = new DynamicParameters();
        //    if (models == null || models.Count == 0) return new KeyValuePair<string, DynamicParameters>();
        //    StringBuilder builder = new StringBuilder();

        //    foreach (var model in models)
        //    {
        //        if (model is ConditionalModel)
        //        {
        //            var item = model as ConditionalModel;
        //            var index = models.IndexOf(item) + beginIndex;
        //            var type = index == 0 ? "" : "AND";
        //            if (beginIndex > 0)
        //            {
        //                type = null;
        //            }
        //            string temp = " {0} {1} {2} {3} ";
        //            string parameterName = string.Format("{0}P{2}_{1}", ParameterPrefix, item.FieldName, index);
        //            if (parameterName.Contains("."))
        //            {
        //                parameterName = parameterName.Replace(".", "_");
        //            }
        //            switch (item.ConditionalType)
        //            {
        //                case ConditionalType.Equal:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "=", parameterName);
        //                    parameters.Add(parameterName, GetFieldValue(item));
        //                    break;
        //                case ConditionalType.Like:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(parameterName, "%" + item.FieldValue + "%");
        //                    break;
        //                case ConditionalType.GreaterThan:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">", parameterName);
        //                    parameters.Add(parameterName, GetFieldValue(item));
        //                    break;
        //                case ConditionalType.GreaterThanOrEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), ">=", parameterName);
        //                    parameters.Add(parameterName, GetFieldValue(item));
        //                    break;
        //                case ConditionalType.LessThan:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<", parameterName);
        //                    parameters.Add(parameterName, GetFieldValue(item));
        //                    break;
        //                case ConditionalType.LessThanOrEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<=", parameterName);
        //                    parameters.Add(parameterName, GetFieldValue(item));
        //                    break;
        //                case ConditionalType.In:
        //                    if (item.FieldValue == null) item.FieldValue = string.Empty;
        //                    var inValue1 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "IN", inValue1);
        //                    parameters.Add(parameterName, item.FieldValue);
        //                    break;
        //                case ConditionalType.NotIn:
        //                    if (item.FieldValue == null) item.FieldValue = string.Empty;
        //                    var inValue2 = ("(" + item.FieldValue.Split(',').ToJoinSqlInVals() + ")");
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "NOT IN", inValue2);
        //                    parameters.Add(parameterName, item.FieldValue);
        //                    break;
        //                case ConditionalType.LikeLeft:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(parameterName, item.FieldValue + "%");
        //                    break;
        //                case ConditionalType.NoLike:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " NOT LIKE", parameterName);
        //                    parameters.Add(parameterName, item.FieldValue + "%");
        //                    break;
        //                case ConditionalType.LikeRight:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "LIKE", parameterName);
        //                    parameters.Add(parameterName, "%" + item.FieldValue);
        //                    break;
        //                case ConditionalType.NoEqual:
        //                    builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
        //                    parameters.Add(parameterName, item.FieldValue);
        //                    break;
        //                case ConditionalType.IsNullOrEmpty:
        //                    builder.AppendFormat("{0} ({1}) OR ({2}) ", type, item.FieldName.ToSqlFilter() + " IS NULL ", item.FieldName.ToSqlFilter() + " = '' ");
        //                    parameters.Add(parameterName, item.FieldValue);
        //                    break;
        //                case ConditionalType.IsNot:
        //                    if (item.FieldValue == null)
        //                    {
        //                        builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), " IS NOT ", "NULL");
        //                    }
        //                    else
        //                    {
        //                        builder.AppendFormat(temp, type, item.FieldName.ToSqlFilter(), "<>", parameterName);
        //                        parameters.Add(parameterName, item.FieldValue);
        //                    }
        //                    break;
        //                case ConditionalType.Regex:
        //                    string regexStr = Database.SqlDialectProvider.ConvertRegexStr(item.FieldName.ToSqlFilter(), item.FieldValue);
        //                    builder.AppendFormat(temp, type, regexStr, "", "");

        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            var item = model as ConditionalCollections;
        //            if (item != null && item.ConditionalList != null)
        //            {
        //                foreach (var con in item.ConditionalList)
        //                {
        //                    var index = item.ConditionalList.IndexOf(con);
        //                    var isFirst = index == 0;
        //                    var isLast = index == (item.ConditionalList.Count - 1);
        //                    if (models.IndexOf(item) == 0 && index == 0 && beginIndex == 0)
        //                    {
        //                        builder.AppendFormat(" ( ");

        //                    }
        //                    else if (isFirst)
        //                    {
        //                        builder.AppendFormat(" {0} ( ", con.Key.ToString().ToUpper());
        //                    }
        //                    List<IConditionalModel> conModels = new List<IConditionalModel>();
        //                    conModels.Add(con.Value);
        //                    var childSqlInfo = this.ConditionalModelToSql(Database, conModels, 1000 * (1 + index) + models.IndexOf(item));
        //                    if (!isFirst)
        //                    {

        //                        builder.AppendFormat(" {0} ", con.Key.ToString().ToUpper());
        //                    }
        //                    builder.Append(childSqlInfo.Key);
        //                    parameters.AddDynamicParams(childSqlInfo.Value);

        //                    if (isLast)
        //                    {
        //                        builder.Append(" ) ");
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return new KeyValuePair<string, DynamicParameters>(builder.ToString(), parameters);
        //}

    }
    public class ConditionalModel : IConditionalModel
    {
        public ConditionalModel()
        {
            this.ConditionalType = ConditionalType.Equal;
        }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public ConditionalType ConditionalType { get; set; }
        public Func<string, object> FieldValueConvertFunc { get; set; }
    }

    //public class SqlBuilderParameter : DbParameter
    //{
    //    public bool IsRefCursor { get; set; }
    //    public SqlBuilderParameter(string name, object value)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        if (value != null)
    //        {
    //            SettingDataType(value.GetType());
    //        }
    //    }
    //    public SqlBuilderParameter(string name, object value, Type type)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        SettingDataType(type);
    //    }
    //    public SqlBuilderParameter(string name, object value, Type type, ParameterDirection direction)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.Direction = direction;
    //        SettingDataType(type);
    //    }
    //    public SqlBuilderParameter(string name, object value, Type type, ParameterDirection direction, int size)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.Direction = direction;
    //        this.Size = size;
    //        SettingDataType(type);
    //    }


    //    public SqlBuilderParameter(string name, object value, System.Data.DbType type)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.DbType = type;
    //    }
    //    public SqlBuilderParameter(string name, DataTable value, string SqlServerTypeName)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.TypeName = SqlServerTypeName;
    //    }
    //    public SqlBuilderParameter(string name, object value, System.Data.DbType type, ParameterDirection direction)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.Direction = direction;
    //        this.DbType = type;
    //    }
    //    public SqlBuilderParameter(string name, object value, System.Data.DbType type, ParameterDirection direction, int size)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        this.Direction = direction;
    //        this.Size = size;
    //        this.DbType = type;
    //    }

    //    private void SettingDataType(Type type)
    //    {
    //        if (type == UtilConstants.ByteArrayType)
    //        {
    //            this.DbType = System.Data.DbType.Binary;
    //        }
    //        else if (type == UtilConstants.GuidType)
    //        {
    //            this.DbType = System.Data.DbType.Guid;
    //        }
    //        else if (type == UtilConstants.IntType)
    //        {
    //            this.DbType = System.Data.DbType.Int32;
    //        }
    //        else if (type == UtilConstants.ShortType)
    //        {
    //            this.DbType = System.Data.DbType.Int16;
    //        }
    //        else if (type == UtilConstants.LongType)
    //        {
    //            this.DbType = System.Data.DbType.Int64;
    //        }
    //        else if (type == UtilConstants.DateType)
    //        {
    //            this.DbType = System.Data.DbType.DateTime;
    //        }
    //        else if (type == UtilConstants.DobType)
    //        {
    //            this.DbType = System.Data.DbType.Double;
    //        }
    //        else if (type == UtilConstants.DecType)
    //        {
    //            this.DbType = System.Data.DbType.Decimal;
    //        }
    //        else if (type == UtilConstants.ByteType)
    //        {
    //            this.DbType = System.Data.DbType.Byte;
    //        }
    //        else if (type == UtilConstants.FloatType)
    //        {
    //            this.DbType = System.Data.DbType.Single;
    //        }
    //        else if (type == UtilConstants.BoolType)
    //        {
    //            this.DbType = System.Data.DbType.Boolean;
    //        }
    //        else if (type == UtilConstants.StringType)
    //        {
    //            this.DbType = System.Data.DbType.String;
    //        }
    //        else if (type == UtilConstants.DateTimeOffsetType)
    //        {
    //            this.DbType = System.Data.DbType.DateTimeOffset;
    //        }
    //        else if (type == UtilConstants.TimeSpanType)
    //        {
    //            if (this.Value != null)
    //                this.Value = this.Value.ToString();
    //        }
    //        else if (type.IsEnum())
    //        {
    //            this.DbType = System.Data.DbType.Int64;
    //        }

    //    }
    //    public SqlBuilderParameter(string name, object value, bool isOutput)
    //    {
    //        this.Value = value;
    //        this.ParameterName = name;
    //        if (isOutput)
    //        {
    //            this.Direction = ParameterDirection.Output;
    //        }
    //    }
    //    public override System.Data.DbType DbType
    //    {
    //        get; set;
    //    }

    //    public override ParameterDirection Direction
    //    {
    //        get; set;
    //    }

    //    public override bool IsNullable
    //    {
    //        get; set;
    //    }

    //    public override string ParameterName
    //    {
    //        get; set;
    //    }

    //    public int _Size;

    //    public override int Size
    //    {
    //        get
    //        {
    //            if (_Size == 0 && Value != null)
    //            {
    //                var isByteArray = Value.GetType() == UtilConstants.ByteArrayType;
    //                if (isByteArray)
    //                    _Size = -1;
    //                else
    //                {
    //                    var length = Value.ToString().Length;
    //                    _Size = length < 4000 ? 4000 : -1;

    //                }
    //            }
    //            if (_Size == 0)
    //                _Size = 4000;
    //            return _Size;
    //        }
    //        set
    //        {
    //            _Size = value;
    //        }
    //    }

    //    public override string SourceColumn
    //    {
    //        get; set;
    //    }

    //    public override bool SourceColumnNullMapping
    //    {
    //        get; set;
    //    }
    //    public string UdtTypeName
    //    {
    //        get;
    //        set;
    //    }

    //    public override object Value
    //    {
    //        get; set;
    //    }

    //    public Dictionary<string, object> TempDate
    //    {
    //        get; set;
    //    }

    //    /// <summary>
    //    /// 如果类库是.NET 4.5请删除该属性
    //    /// If the SqlSugar library is.NET 4.5, delete the property
    //    /// </summary>
    //    public override DataRowVersion SourceVersion
    //    {
    //        get; set;
    //    }

    //    public override void ResetDbType()
    //    {
    //        this.DbType = System.Data.DbType.String;
    //    }


    //    public string TypeName { get; set; }
    //}

    internal static class UtilConstants
    {
        
        internal static Type IntType = typeof(int);
        internal static Type LongType = typeof(long);
        internal static Type GuidType = typeof(Guid);
        internal static Type BoolType = typeof(bool);
        internal static Type BoolTypeNull = typeof(bool?);
        internal static Type ByteType = typeof(Byte);
        internal static Type ObjType = typeof(object);
        internal static Type DobType = typeof(double);
        internal static Type FloatType = typeof(float);
        internal static Type ShortType = typeof(short);
        internal static Type DecType = typeof(decimal);
        internal static Type StringType = typeof(string);
        internal static Type DateType = typeof(DateTime);
        internal static Type DateTimeOffsetType = typeof(DateTimeOffset);
        internal static Type TimeSpanType = typeof(TimeSpan);
        internal static Type ByteArrayType = typeof(byte[]);
        //internal static Type ModelType = typeof(ModelContext);
        internal static Type DynamicType = typeof(ExpandoObject);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicArraySS = typeof(Dictionary<string, string>);
        internal static Type DicArraySO = typeof(Dictionary<string, object>);
         
    }


    public static class DbExtensions
    {
        public static string ToJoinSqlInVals<T>(this T[] array, string table, IDatabase Database)
        {
            if (array == null || array.Length == 0)
            {
                return ToSqlValue(string.Empty, table , Database);
            }
            else
            {
                return string.Join(",", array.Where(c => c != null).Select(it => (it + "").ToSqlValue(table, Database)));
            }
        }

        public static string ToSqlValue(this string value, string table, IDatabase Database)
        {
            return string.Format("'{0}'", value.ToSqlFilter(table, Database));
        }

        /// <summary>
        ///Sql Filter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSqlFilter(this string value, string table, IDatabase Database )
        {
            if (!value.IsNullOrEmpty())
            {
                value = value.Replace("'", "''");
               
            }
            return value;
        }


        public static string GetTableName(this string name, IDatabase Database)
        {
            return Database.SqlGenerator.GetTableName(name);
        }

        public static string GetColumnName(this string tableName, string name,  IDatabase Database, string alias = null)
        {
            return Database.SqlGenerator.Configuration.Dialect.GetColumnName(GetTableName(tableName, Database), name, alias);
        }
    }

}
