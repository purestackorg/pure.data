using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dapper
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
                            sql = sql.Replace(prefixPara + p.Key, Database.SqlDialectProvider.FormatValue(p.Value).ToString());
                            //sql = sql.Replace(prefixPara + p.Key, FormatValue(p.Value, dbType));
                        }
                        
                    }
                }
                return sql;
            }
            //private string FormatValue(object o, Pure.Data.DatabaseType dbType)
            //{
            //    if (o == null)
            //    {
            //        return "''";
            //    }
            //    Type t = o.GetType();
            //    if (t == typeof(string) || t == typeof(String))
            //    {
            //        return string.Format("'{0}'", o);
            //    }
            //    else if (t ==typeof(DateTime))
            //    {
            //        if (dbType == Pure.Data.DatabaseType.Oracle)
            //        {
            //            string result = ("TO_DATE('");
            //            result += o;
            //            result += ("','yy-mm-dd hh24:mi:ss')");
            //            return result;
            //        }
            //        else
            //        {
            //            return "'" + o + "'"; 
            //        }
                     
            //    }
            //    else if (t == typeof(Boolean) || t == typeof(bool))
            //    {
            //        bool b = (bool)o;
            //        if (b == true)
            //        {
            //            return "1";  
            //        }
            //        else
            //        {
            //            return "0";
            //        }
            //    }
            //    else if (t.IsPrimitive && t != typeof(string))
            //    {
            //        return o.ToString();
            //    }
            //    else if (t.IsEnum())
            //    {
            //        return ((int)o).ToString();
            //    }
            //    else
            //    {
            //        return string.Format("'{0}'", o);
            //    }
            //}

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
            AddClause("select", sql, parameters, " , ", prefix: "", postfix: "\n");
            return this;
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
    }
}
