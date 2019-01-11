
using FluentExpressionSQL.Mapper;
using FluentExpressionSQL.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pure.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FluentExpressionSQL
{


    public class SqlDialectProviderLoader
    {
        public static ISqlDialectProvider GetSqlProvider(ExpDbType dbType)
        {
            ISqlDialectProvider provider = null;
            switch (dbType)
            {
                case ExpDbType.SQLServer:
                    provider = SqlServerDialectProvider.Instance;
                    break;
                case ExpDbType.MySQL:
                    provider = MySqlDialectProvider.Instance;
                    break;
                case ExpDbType.Oracle:
                    provider = OracleDialectProvider.Instance;
                    break;
                case ExpDbType.SQLite:
                    provider = SQLiteDialectProvider.Instance;
                    break;
                case ExpDbType.PostgreSQL:
                    provider = PostgreSqlDialectProvider.Instance;
                    break;
                case ExpDbType.SqlCe:
                    provider = SqlCeDialectProvider.Instance;
                    break;
                default://默认数据库驱动
                    provider = SqlServerDialectProvider.Instance;
                    break;
            }

            return provider;
        }

        public static ISqlDialectProvider GetSqlProviderByDatabaseType(DatabaseType DatabaseType)
        {
            ISqlDialectProvider _SqlDialect = null;
            switch (DatabaseType)
            {
                case DatabaseType.None:
                    break;
                case DatabaseType.SqlServer:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.SQLServer);
                    break;
                case DatabaseType.SqlCe:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.SqlCe);

                    break;
                case DatabaseType.PostgreSQL:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.PostgreSQL);

                    break;
                case DatabaseType.MySql:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.MySQL);

                    break;
                case DatabaseType.Oracle:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.Oracle);

                    break;
                case DatabaseType.SQLite:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.SQLite);

                    break;
                case DatabaseType.Access:
                    break;
                case DatabaseType.OleDb:
                    break;
                case DatabaseType.Firebird:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.Firebird);

                    break;
                case DatabaseType.DB2:
                    _SqlDialect = SqlDialectProviderLoader.GetSqlProvider(ExpDbType.DB2);

                    break;
                case DatabaseType.DB2iSeries:
                    break;
                case DatabaseType.SybaseASA:
                    break;
                case DatabaseType.SybaseASE:
                    break;
                case DatabaseType.SybaseUltraLite:
                    break;
                case DatabaseType.DM:
                    break;
                default:
                    break;
            }

            return _SqlDialect;
        }

    }

    public class FluentExpressionSQLCore<T>
    {
        public SqlPack _sqlPack = null;
        public string RawString { get {

            return  this._sqlPack.ToString();
        
        } }
        /// <summary>
        /// 动态Sql link执行链
        /// </summary>
        private List<FluentExpressionSQLCore<T>> FluentExpressionSqlActionList;
        public Dictionary<string, object> DbParams { get { return this._sqlPack.DbParams; } } 
        public FluentExpressionSQLCore(ExpDbType dbType, ITableMapperContainer tableMapperContainer = null )
        {
            _sqlPack = new SqlPack();
            this._sqlPack.DatabaseType = dbType;
            this._sqlPack.TableMapperContainer = tableMapperContainer;
            this._sqlPack.SqlDialectProvider = GetSqlProvider(dbType);
            FluentExpressionSqlActionList = new List<FluentExpressionSQLCore<T>>();

        }

        public FluentExpressionSQLCore(FluentExpressionSQLCore<T> e)
        {
            this._sqlPack = e._sqlPack;

            FluentExpressionSqlActionList = new List<FluentExpressionSQLCore<T>>();

        }

        private ISqlDialectProvider GetSqlProvider(ExpDbType dbType)
        {
            ISqlDialectProvider provider = null;
            provider = SqlDialectProviderLoader.GetSqlProvider(dbType);
            return provider;
        }

        #region 输出最终SQL
        /// <summary>
        /// 输出结果SQL
        /// </summary>
        /// <returns></returns>
        public string ToSqlString()
        {
            string result = RawString;
            foreach (KeyValuePair<string, object> item in DbParams)
            {
                result = result.Replace(item.Key.ToString(), FormatParamString(item.Value));
            }
            ///清空子查询列表临时数据
            if (ExpressionSqlBuilder.ExistSubQuery())
            {
                foreach (KeyValuePair<string, object> item in ExpressionSqlBuilder.GetExistDbParameters())
                {
                    result = result.Replace(item.Key.ToString(), FormatParamString(item.Value));
                }
                ExpressionSqlBuilder.ClearSubQuery();
            }
            return result;
        }

        private string FormatParamString(object value)
        {
            string result = "";
            if (value != null)
            {
                if (value is DateTimeDto)
                {
                    var _DateTimeDto = value as DateTimeDto;
                    if (_DateTimeDto != null)
                    {
                        result = string.Format("{0}", _DateTimeDto.Text);
                    }
                }
                else if (value is string || value is Guid)
                {
                    //result = string.Format("'{0}'", value);
                    result = string.Format("{0}", value);

                }
                else if (value is DateTime)
                {
                    //result = string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);
                    result = string.Format("{0}", value);
                }
                else if (value is System.Collections.IList || (value is System.Collections.IEnumerable) || value is Array)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (var item in value as System.Collections.IEnumerable)
                    {
                        sb.AppendFormat("{0},", FormatParamString(item));
                    }
                    result = sb.Remove(sb.Length - 1, 1).ToString();
                }
                else
                {
                    result = value.ToString();
                }
            }
            return result;
        }

        #endregion

        #region 拼接SQL过滤条件
        /// <summary>
        /// 拼接SQL过滤条件
        /// </summary>
        /// <param name="sqlFilter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        private string FormatFilter(SqlPack sqlpack, string sqlFilter, params object[] filterParams)
        {
            if (string.IsNullOrEmpty(sqlFilter))
                return null;
            if (filterParams == null)
            {
                return sqlFilter;
            }
            for (var i = 0; i < filterParams.Length; i++)
            {
                var pLiteral = "{" + i + "}";
                var filterParam = filterParams[i];
                //var sqlParams = filterParam as SqlInValues;
                //格式化
                filterParam = sqlpack.SqlDialectProvider.FormatValue(filterParam);
                string paraName = sqlpack.AddDbParameterWithoutPickSql(filterParam);
                if (filterParam != null && paraName != "")
                {
                    sqlFilter = sqlFilter.Replace(pLiteral, paraName);
                }
                else
                {
                    sqlFilter = sqlFilter.Replace(pLiteral, paraName);
                }
            }
            return sqlFilter;
        }


        #endregion


        public void Clear()
        {
            this._sqlPack.Clear();
        }

        /// <summary>
        /// 根据类型和表映射获取表名
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string GetTableName(Type t)
        {
            return this._sqlPack.GetTableName(t);
        }

        private string SelectParser(params Type[] ary)
        {
            this._sqlPack.Clear();
            this._sqlPack.IsSingleTable = false;
            //启用Select 语法
            this._sqlPack.SelectStatement = new StatementSelect(this._sqlPack);

            string tableName = "";
            foreach (var item in ary)
            {
                tableName = GetTableName(item);//item.Name;
                this._sqlPack.SetTableAlias(tableName);
            }

            tableName = GetTableName(typeof(T));
            //return "select {0} from " + tableName + " " + this._sqlPack.GetTableAlias(tableName);
            string fromTable = "FROM " + tableName + " " + this._sqlPack.GetTableAlias(tableName);
            this._sqlPack.Sql.AppendFormat(fromTable);
            return fromTable;

        }
        private void AddTableAlias(params Type[] ary)
        {
            this._sqlPack.Clear();
            this._sqlPack.IsSingleTable = false;
            string tableName = "";
            foreach (var item in ary)
            {
                tableName = GetTableName(item);//item.Name;
                this._sqlPack.SetTableAlias(tableName);
            }

            

        }
        public FluentExpressionSQLCore<T> Select(Expression<Func<T, object>> expression = null)
        {
            string sql = SelectParser(typeof(T));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2>(Expression<Func<T, T2, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3>(Expression<Func<T, T2, T3, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }
        public FluentExpressionSQLCore<T> Select<T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression = null)
        {
            string sql = SelectParser(typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10));

            if (expression == null)
            {
                //this._sqlPack.Sql.AppendFormat(sql, "*");
            }
            else
            {
                FluentExpressionSQLProvider.Select(expression.Body, this._sqlPack);
                //this._sqlPack.Sql.AppendFormat(sql, this._sqlPack.SelectFieldsStr);
            }

            return this;
        }

        private FluentExpressionSQLCore<T> JoinParser<T2>(Expression<Func<T, T2, bool>> expression, string leftOrRightJoin = "")
        {
            string joinTableName = GetTableName(typeof(T2)); //typeof(T2).Name;
            this._sqlPack.SetTableAlias(joinTableName);
            this._sqlPack.Sql.AppendFormat(" {0}JOIN {1} ON", leftOrRightJoin, joinTableName + " " + this._sqlPack.GetTableAlias(joinTableName));
            FluentExpressionSQLProvider.Join(expression.Body, this._sqlPack);
            return this;
        }
        private FluentExpressionSQLCore<T> JoinParser2<T2, T3>(Expression<Func<T2, T3, bool>> expression, string leftOrRightJoin = "")
        {
            string joinTableName = GetTableName(typeof(T3)); //typeof(T3).Name;
            this._sqlPack.SetTableAlias(joinTableName);
            this._sqlPack.Sql.AppendFormat(" {0}JOIN {1} ON", leftOrRightJoin, joinTableName + " " + this._sqlPack.GetTableAlias(joinTableName));
            FluentExpressionSQLProvider.Join(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return JoinParser(expression);
        }
        public FluentExpressionSQLCore<T> Join<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            return JoinParser2(expression);
        }

        public FluentExpressionSQLCore<T> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return JoinParser(expression, "INNER ");
        }
        public FluentExpressionSQLCore<T> InnerJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            return JoinParser2(expression, "INNER ");
        }

        public FluentExpressionSQLCore<T> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            return JoinParser(expression, "LEFT ");
        }
        public FluentExpressionSQLCore<T> LeftJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            return JoinParser2(expression, "LEFT ");
        }

        public FluentExpressionSQLCore<T> RightJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            if (this._sqlPack.DatabaseType == ExpDbType.SQLite)
            {
                throw new NotSupportedException(this._sqlPack.DatabaseType.ToString() + " is not supports Right Join Statement.");
            }
            return JoinParser(expression, "RIGHT ");
        }
        public FluentExpressionSQLCore<T> RightJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            if (this._sqlPack.DatabaseType == ExpDbType.SQLite)
            {
                throw new NotSupportedException(this._sqlPack.DatabaseType.ToString() + " is not supports Right Join Statement.");
            }
            return JoinParser2(expression, "RIGHT ");
        }

        public FluentExpressionSQLCore<T> FullJoin<T2>(Expression<Func<T, T2, bool>> expression)
        {
            if (this._sqlPack.DatabaseType == ExpDbType.MySQL || this._sqlPack.DatabaseType == ExpDbType.SQLite)
            {
                throw new NotSupportedException(this._sqlPack.DatabaseType.ToString() + " is not supports Full Join Statement.");
            }
            return JoinParser(expression, "FULL ");
        }
        public FluentExpressionSQLCore<T> FullJoin<T2, T3>(Expression<Func<T2, T3, bool>> expression)
        {
            if (this._sqlPack.DatabaseType == ExpDbType.MySQL || this._sqlPack.DatabaseType == ExpDbType.SQLite)
            {
                throw new NotSupportedException(this._sqlPack.DatabaseType.ToString() + " is not supports Full Join Statement.");
            }
            return JoinParser2(expression, "FULL ");
        }


        private void AppendWhereLink(string andOrStr = "AND")
        {
            this._sqlPack += this.RawString.Contains(" WHERE")
               ? (this._sqlPack.EnableNewSubQuery ? "  WHERE" : " " + andOrStr)
               : " WHERE";
        }
        /// <summary>
        /// 创建新的构造器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public FluentExpressionSQLCore<T> NewBuilder( )
        {
            AddTableAlias(typeof(T));
            //if (!string.IsNullOrEmpty(key))
            //{
            //    this._sqlPack.ParamNameKey += key + "Sub";

            //}
            //else
            //{
            //    this._sqlPack.ParamNameKey += "Sub";

            //}
            this._sqlPack.ParamNameKey += "Sub";

            return this;

        }
         
        public FluentExpressionSQLCore<T> Where(Expression<Func<T, bool>> expression)
        {
            AppendWhereLink("AND");

            //this._sqlPack += " where";
            this._sqlPack.CurrentWhereExpression = expression.Body;
            FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Where<T2>(Expression<Func<T, T2, bool>> expression)
        {
            AppendWhereLink("AND");
            
            this._sqlPack.CurrentWhereExpression = expression.Body;
            FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);

            return this;
        }

       
        public FluentExpressionSQLCore<T> WhereIf<T2>(bool ifTrue, Expression<Func<T, T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return Where<T2>(expression);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> Where<T2>(Expression<Func<T2, bool>> expression)
        {
            AppendWhereLink("AND");

            this._sqlPack.CurrentWhereExpression = expression.Body;
            FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);

            return this;
        }

        public FluentExpressionSQLCore<T> WhereIf<T2>(bool ifTrue, Expression<Func<T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return Where<T2>(expression);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> WhereIf(bool ifTrue,Expression<Func<T, bool>> expression)
        {
            if (ifTrue == true)
            {
                return Where(expression);
            } 
            return this;
        }
        /// <summary>
        /// Where(" a.Name ={0} and a.ID >0 ", "张茂")
        /// </summary>
        /// <param name="sqlFilter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public virtual FluentExpressionSQLCore<T> Where(string sqlFilter, params object[] filterParams)
        {
            if (ValidString(sqlFilter))
            {
                AppendWhereLink("AND");

                this._sqlPack += " " + FormatFilter(this._sqlPack, sqlFilter.SqlVerifyFragment(), filterParams);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> WhereIf(bool ifTrue, string sqlFilter, params object[] filterParams)
        {
            if (ifTrue == true)
            {
                return Where(sqlFilter, filterParams);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> And(Expression<Func<T, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("AND");

                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> AndIf(bool ifTrue, Expression<Func<T, bool>> expression)
        {
            if (ifTrue == true)
            {
                return And(expression);
            }
            
            return this;
        }

        public FluentExpressionSQLCore<T> And<T2>(Expression<Func<T, T2, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("AND");

                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> AndIf<T2>(bool ifTrue, Expression<Func<T, T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return And<T2>(expression);
            }
            return this;
        }

        public FluentExpressionSQLCore<T> And<T2>(Expression<Func<T2, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("AND");

                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> AndIf<T2>(bool ifTrue, Expression<Func<T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return And<T2>(expression);
            }
            return this;
        }


        
        /// <summary>
        /// And(" a.Name ={0}", "李四")
        /// </summary>
        /// <param name="sqlFilter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public virtual FluentExpressionSQLCore<T> And(string sqlFilter, params object[] filterParams)
        {
            if (ValidString(sqlFilter))
            {
                AppendWhereLink("AND");

                this._sqlPack += " (" + FormatFilter(this._sqlPack, sqlFilter.SqlVerifyFragment(), filterParams) + ")";
            }
            return this;
        }

        public FluentExpressionSQLCore<T> AndIf(bool ifTrue, string sqlFilter, params object[] filterParams)
        {
            if (ifTrue == true)
            {
                return And(sqlFilter, filterParams);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> Or(Expression<Func<T, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("OR");
                //this._sqlPack.CurrentWhereExpression = expression.Body;
                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }

        public FluentExpressionSQLCore<T> OrIf( bool ifValue , Expression<Func<T, bool>> expression)
        {
            if (ifValue == true)
            {
                return Or(expression);
            }
            
            return this;
        }

        public FluentExpressionSQLCore<T> Or<T2>(Expression<Func<T, T2, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("OR"); 
                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> OrIf<T2>(bool ifTrue, Expression<Func<T, T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return Or<T2>(expression);
            }
            return this;
        }


        public FluentExpressionSQLCore<T> Or<T2>(Expression<Func<T2, bool>> expression)
        {
            if (ValidExpression(expression))
            {
                AppendWhereLink("OR");
                FluentExpressionSQLProvider.Where(expression.Body, this._sqlPack);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> OrIf<T2>(bool ifTrue, Expression<Func<T2, bool>> expression)
        {
            if (ifTrue == true)
            {
                return Or<T2>(expression);
            }
            return this;
        }
        /// <summary>
        ///  Or(" a.Name ={0} and a.ID >{1} ", "张茂", 5)
        /// </summary>
        /// <param name="sqlFilter"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public virtual FluentExpressionSQLCore<T> Or(string sqlFilter, params object[] filterParams)
        {
            if (ValidString(sqlFilter))
            {
                AppendWhereLink("OR");

                this._sqlPack += " (" + FormatFilter(this._sqlPack, sqlFilter.SqlVerifyFragment(), filterParams) + ")";
            }
            return this;
        }
        public FluentExpressionSQLCore<T> OrIf(bool ifTrue, string sqlFilter, params object[] filterParams)
        {
            if (ifTrue == true)
            {
                return Or(sqlFilter, filterParams);
            }
            return this;
        }

        public GroupByExpression<T> GroupBy(Expression<Func<T, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " GROUP BY ";
                FluentExpressionSQLProvider.GroupBy(expression.Body, this._sqlPack);
            }
            
            return new GroupByExpression<T>(this);
        }

        public GroupByExpression<T> GroupBy<T2>(Expression<Func<T, T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " GROUP BY ";
                FluentExpressionSQLProvider.GroupBy(expression.Body, this._sqlPack);
            }
            return new GroupByExpression<T>(this);
        }
        public GroupByExpression<T> GroupByIf<T2>(bool ifTrue, Expression<Func<T, T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return GroupBy<T2>(expression);
            }
            return new GroupByExpression<T>(this);
        }

        public GroupByExpression<T> GroupBy<T2>(Expression<Func<T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " GROUP BY ";
                FluentExpressionSQLProvider.GroupBy(expression.Body, this._sqlPack);
            }
            return new GroupByExpression<T>(this);
        }
        public GroupByExpression<T> GroupByIf<T2>(bool ifTrue, Expression<Func<T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return GroupBy<T2>(expression);
            }
            return new GroupByExpression<T>(this);
        }
        /// <summary>
        /// GroupBy("a.Id ")
        /// </summary>
        /// <param name="strGroupBy"></param>
        /// <returns></returns>
        public FluentExpressionSQLCore<T> GroupByByString(string strGroupBy)
        {
            if (ValidString(strGroupBy))
            {
                this._sqlPack += " GROUP BY " + strGroupBy;
            }
            return (this);
        }
        public FluentExpressionSQLCore<T> GroupByByStringIf(bool ifTrue, string strGroupBy)
        {
            if (ifTrue == true)
            {
                return GroupByByString( strGroupBy);
            }
            
            return (this);
        }
        public FluentExpressionSQLCore<T> GroupByIf(bool ifTrue, Expression<Func<T, object>> expression)
        {
            if (ifTrue == true)
            {
                return GroupBy(expression);
            }

            return (this);

        }
        /// <summary>
        /// OrderByString("a.DTCreate desc ")
        /// </summary>
        /// <param name="strOrder"></param>
        /// <returns></returns>
        public FluentExpressionSQLCore<T> OrderByString(string strOrder)
        {
            if (ValidString( strOrder))
            {
                this._sqlPack += " ORDER BY " + strOrder; 
            }
            return (this);
        }

        public FluentExpressionSQLCore<T> OrderByStringIf(bool ifTrue, string strOrder)
        {
            if (ifTrue == true)
            {
                return OrderByString( strOrder);
            }
            
            return (this);
        }
        public OrderExpression<T> OrderBy(Expression<Func<T, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
            }
            
            return new OrderExpression<T>(this);
        }

        public OrderExpression<T> OrderBy<T2>(Expression<Func<T, T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
            }
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderByIf<T2>(bool ifTrue, Expression<Func<T, T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderBy<T2>(expression);
            }
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderBy<T2>(Expression<Func<T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
            }
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderByIf<T2>(bool ifTrue, Expression<Func<T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderBy<T2>(expression);
            }
            return new OrderExpression<T>(this);
        }


        public OrderExpression<T> OrderByIf(bool ifTrue, Expression<Func<T, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderBy(expression);
            }
             
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
                this._sqlPack += " DESC";
            }
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderByDescendingIf(bool ifTrue, Expression<Func<T, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderByDescending(expression);
            }

            return new OrderExpression<T>(this);
        }

        public OrderExpression<T> OrderByDescending<T2>(Expression<Func<T, T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
                this._sqlPack += " DESC";
            }
            return new OrderExpression<T>(this);
        }

        public OrderExpression<T> OrderByDescendingIf<T2>(bool ifTrue, Expression<Func<T, T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderByDescending<T2>(expression);
            }
            return new OrderExpression<T>(this);
        }
        public OrderExpression<T> OrderByDescending<T2>(Expression<Func<T2, object>> expression)
        {
            if (ValidExpression(expression))
            {
                this._sqlPack += " ORDER BY ";
                FluentExpressionSQLProvider.OrderBy(expression.Body, this._sqlPack);
                this._sqlPack += " DESC";
            }
            return new OrderExpression<T>(this);
        }

        public OrderExpression<T> OrderByDescendingIf<T2>(bool ifTrue, Expression<Func<T2, object>> expression)
        {
            if (ifTrue == true)
            {
                return OrderByDescending<T2>(expression);
            }
            return new OrderExpression<T>(this);
        }
        public FluentExpressionSQLCore<T> Max(Expression<Func<T, object>> expression)
        {
            this._sqlPack.Clear();
            //this._sqlPack.IsSingleTable = true;
            FluentExpressionSQLProvider.Max(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Min(Expression<Func<T, object>> expression)
        {
            this._sqlPack.Clear();
            //this._sqlPack.IsSingleTable = true;
            FluentExpressionSQLProvider.Min(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Avg(Expression<Func<T, object>> expression)
        {
            this._sqlPack.Clear();
            //this._sqlPack.IsSingleTable = true;
            FluentExpressionSQLProvider.Avg(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Count(Expression<Func<T, object>> expression = null)
        {
            this._sqlPack.Clear();
            //this._sqlPack.IsSingleTable = true;
            if (expression == null)
            {
                var tableName = GetTableName(typeof(T));
                this._sqlPack.SetTableAlias(tableName); 
                string tableAlias = this._sqlPack.GetTableAlias(tableName);
                 
                if (!string.IsNullOrWhiteSpace(tableAlias))
                {
                    tableName += " " + tableAlias; 
                }



                this._sqlPack.Sql.AppendFormat("SELECT COUNT(1) FROM {0}", tableName);
            }
            else
            {
                FluentExpressionSQLProvider.Count(expression.Body, this._sqlPack);
            }

            return this;
        }

        public FluentExpressionSQLCore<T> Sum(Expression<Func<T, object>> expression)
        {
            this._sqlPack.Clear();
            //this._sqlPack.IsSingleTable = true;
            FluentExpressionSQLProvider.Sum(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Delete()
        {
            this._sqlPack.Clear();
            this._sqlPack.IsSingleTable = true;
            string tableName = GetTableName(typeof(T));
            this._sqlPack.SetTableAlias(tableName);
            this._sqlPack += "DELETE FROM " + tableName;
            return this;
        }

        public FluentExpressionSQLCore<T> Update(Expression<Func<object>> expression = null)
        {
            this._sqlPack.Clear();
            this._sqlPack.IsSingleTable = true;
            string tableName = GetTableName(typeof(T));
            this._sqlPack += "UPDATE " + tableName + " SET ";
            FluentExpressionSQLProvider.Update(expression.Body, this._sqlPack);
            return this;
        }

        public FluentExpressionSQLCore<T> Insert(Expression<Func<object>> expression = null)
        {
            this._sqlPack.Clear();
            this._sqlPack.IsSingleTable = true;
            string tableName = GetTableName(typeof(T));

            this._sqlPack += "INSERT INTO " + tableName + " ({0}) VALUES (";
            FluentExpressionSQLProvider.Insert(expression.Body, this._sqlPack);
            this._sqlPack += ") ";

            return this;
        }

        public FluentExpressionSQLCore<T> Top(int count)
        {
            this._sqlPack.SelectStatement.Top = count;
            return this;
        }
        public FluentExpressionSQLCore<T> Distinct()
        {
            this._sqlPack.SelectStatement.Distinct = true;
            return this;
        }

        public FluentExpressionSQLCore<T> TakeSqlPage(int pageIndex, int pageSize, string sql)
        {
            this._sqlPack.SaveSqlWithNew(sql);
            this._sqlPack.PageStatement = new StatementPage(pageIndex, pageSize);
            return this;
        }
        public FluentExpressionSQLCore<T> TakePage(int pageIndex, int pageSize)
        {
            this._sqlPack.PageStatement = new StatementPage(pageIndex, pageSize);
            return this;
        }
        public FluentExpressionSQLCore<T> TakeRange(int startIndex, int resultCount)
        {
            this._sqlPack.RangeStatement = new StatementRange(startIndex, resultCount);
            return this;
        }



        #region 高级查询
        private FluentExpressionSQLCore<T> ExistsParser<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, bool>> expressionWhere = null, bool not = false)
        {
            AppendWhereLink("AND");
            string joinTableName = GetTableName(typeof(T2)); 
            this._sqlPack.SetTableAlias(joinTableName);
            if (not)
            {
                this._sqlPack += " NOT";
            }
            this._sqlPack.Sql.AppendFormat(" EXISTS (SELECT 1 FROM {0} ",  joinTableName + " " + this._sqlPack.GetTableAlias(joinTableName));
            this._sqlPack += "WHERE";

            FluentExpressionSQLProvider.Join(expression.Body, this._sqlPack);
            if (expressionWhere != null)
            {
                this._sqlPack += " AND";

                FluentExpressionSQLProvider.Where(expressionWhere.Body, this._sqlPack);

                
            }
             this._sqlPack +=")";
            return this;
        }
        public FluentExpressionSQLCore<T> ExistsIf<T2>(bool ifTrue, Expression<Func<T, T2, bool>> expression, Expression<Func<T2, bool>> expressionWhere = null)
        {
            if (ifTrue == true)
            {
                return Exists<T2>(expression, expressionWhere);
            }
            return this;
        }
        public FluentExpressionSQLCore<T> Exists<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, bool>> expressionWhere = null )
        {
            return ExistsParser(expression, expressionWhere, false);
        }
        public FluentExpressionSQLCore<T> ExistsNot<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, bool>> expressionWhere = null )
        {
            return ExistsParser(expression, expressionWhere, true);
        }

        public FluentExpressionSQLCore<T> ExistsNotIf<T2>(bool ifTrue, Expression<Func<T, T2, bool>> expression, Expression<Func<T2, bool>> expressionWhere = null)
        {
            if (ifTrue == true)
            {
                return ExistsNot<T2>(expression, expressionWhere);
            }
            return this;
        }
        /// <summary>
        /// 临时保存使用的临时表名和数据
        /// </summary>
        private void AddUsedData()
        {
            Type t = typeof(T);
            ExpressionSqlBuilder.AddUsedAlias(t, this._sqlPack.GetTableAlias(GetTableName(t)));
            ExpressionSqlBuilder.AddUsedParams(this.DbParams);
        }

        public FluentExpressionSQLCore<T> Union()
        {
            string newSql = this._sqlPack.ToString();
            string joiner = " UNION ";
            AddUsedData();
            ExpressionSqlBuilder.AddSubQuery(newSql, joiner);
            this._sqlPack.Clear();

            return this;
        }
        public FluentExpressionSQLCore<T2> Union<T2>()
        {
            string newSql = this._sqlPack.ToString();
            string joiner = " UNION ";
            AddUsedData();
            ExpressionSqlBuilder.AddSubQuery(newSql, joiner);
            this._sqlPack.Clear();
            return new FluentExpressionSQLCore<T2>(this._sqlPack.DatabaseType, this._sqlPack.TableMapperContainer);
        }
        
        public FluentExpressionSQLCore<T> UnionAll()
        {
            string newSql = this._sqlPack.ToString();
            string joiner = " UNION ALL ";
            AddUsedData();
            ExpressionSqlBuilder.AddSubQuery(newSql, joiner);
            this._sqlPack.Clear();

            return this;
        }
        public FluentExpressionSQLCore<T2> UnionAll<T2>()
        {
            string newSql = this._sqlPack.ToString();
            string joiner = " UNION ALL ";
            AddUsedData();
            ExpressionSqlBuilder.AddSubQuery(newSql, joiner);
            this._sqlPack.Clear();
            return new FluentExpressionSQLCore<T2>(this._sqlPack.DatabaseType, this._sqlPack.TableMapperContainer);
        }

        public FluentExpressionSQLCore<T> FromCache(string key)
        {
            string newSql = this._sqlPack.ToString();
            string joiner = " UNION ALL ";
            AddUsedData();
            ExpressionSqlBuilder.AddSubQuery(newSql, joiner);
            this._sqlPack.Clear();

            return this;
        }
        #endregion

        #region 拓展方法
        /// <summary>
        /// 设置新别名
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="newAlias"></param>
        /// <returns></returns>
        public FluentExpressionSQLCore<T> SetTableAlias<T2>(string newAlias)
        {
            string TableName = GetTableName(typeof(T2));
            this._sqlPack.ReplaceTableAlias(TableName, newAlias);
            return this;
        }
        /// <summary>
        /// 动态链接FluentExpressionSQLCore对象
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public FluentExpressionSQLCore<T> Append(FluentExpressionSQLCore<T> link)
        {
            if (link != null)
            {
                string key = link._sqlPack.DbParamPrefix + link._sqlPack.ParamNameKey;
                string newKey = key + (this._sqlPack.DbParams.Count).ToString()+"_";
                string tmpSql = link._sqlPack.Sql.ToString().Replace(key, newKey);
                this._sqlPack += tmpSql;
                string tmpKey = "";
                foreach (var item in link.DbParams)
                {
                    tmpKey = item.Key.Replace(key, "");
                    this._sqlPack.AddDbParameter(newKey + tmpKey, item.Value);

                } 
            }
            return this;
        }
        #endregion

        private bool ValidString(string str)
        {
            if (str != null && !string.IsNullOrEmpty(str.TrimEnd()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidExpression(object exp)
        {
            if (exp != null )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Execute
        public ExecuteScalarDelegate ExecuteScalarAction { get; set; }
        public ExecuteDelegate ExecuteDelegateAction { get; set; }
        public ExecuteReaderDelegate ExecuteReaderAction { get; set; }

        public ExecuteScalarAsyncDelegate ExecuteScalarAsyncAction { get; set; }
        public ExecuteAsyncDelegate ExecuteDelegateAsyncAction { get; set; }
        public ExecuteReaderAsyncDelegate ExecuteReaderAsyncAction { get; set; }
        public IDatabase Database { get; set; }

        #region Excute sync
        /// <summary>
        /// 执行并返回DataReader
        /// </summary>
        /// <returns></returns>
        public System.Data.IDataReader ExecuteReader()
        {
            if (ExecuteReaderAction != null)
            {
                string sql = this.ToSqlString();
                return ExecuteReaderAction(sql);
            }
            else
                throw new Exception("ExecuteReaderAction 不能为空！");

        }
        /// <summary>
        /// 执行并返回Scalar
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            if (ExecuteScalarAction != null)
            {
                string sql = this.ToSqlString();
                return ExecuteScalarAction(sql);
            }
            else
                throw new Exception("ExecuteScalar 不能为空！");

        }
        /// <summary>
        /// 执行并返回影响行数
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            if (ExecuteDelegateAction != null)
            {
                string sql = this.ToSqlString();
                return ExecuteDelegateAction(sql);
            }
            else
                throw new Exception("ExecuteDelegateAction 不能为空！");
        }

        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteList()
        {

            try
            {
                string sql = this.ToSqlString();
                var result = Database.SqlQuery<T>(sql).ToList();
                return result;

            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteListWithRowDelegate()
        {

            try
            {
                var result = ExecuteReader();
                if (result != null)
                {
                    List<T> data = result.ToList<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }


        /// <summary>
        /// 执行并返回List,适合大数据量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteListByEmit()
        {

            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    List<T> data = result.ToListByEmit<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteModel()
        {
            try
            {

                var result = ExecuteReader();
                if (result != null)
                {

                    T data = result.ToModel<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }


        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteModelByEmit()
        {
            try
            {

                var result = ExecuteReader();
                if (result != null)
                {

                    T data = result.ToModelByEmit<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTable()
        {
            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    DataTable data = result.ToDataTable(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTableWithRowDelegate()
        {
            try
            {

                string sql = this.ToSqlString();
                var result = Database.ExecuteDataTableWithRowDelegate(sql);
                return result;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataSet
        /// </summary>
        /// <returns></returns>
        public System.Data.DataSet ExecuteDataSet()
        {

            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    DataSet data = result.ToDataSet(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回Dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>()
        {

            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回dynamic
        /// </summary>
        public dynamic ExecuteExpandoObject()
        {

            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    dynamic data = result.ToExpandoObject(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回IEnumerable(dynamic)
        /// </summary>
        public IEnumerable<dynamic> ExecuteExpandoObjects()
        {

            try
            {

                var result = ExecuteReader();
                if (result != null)
                {
                    IEnumerable<dynamic> data = result.ToExpandoObjects(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        #endregion

        #region Excute Async
        /// <summary>
        /// 执行并返回DataReader
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.IDataReader> ExecuteReaderAsync()
        {
            if (ExecuteReaderAsyncAction != null)
            {
                string sql = this.ToSqlString();
                return await ExecuteReaderAsyncAction(sql);
            }
            else
                throw new Exception("ExecuteReaderAsyncAction 不能为空！");

        }
        /// <summary>
        /// 执行并返回Scalar
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalarAsync()
        {
            if (ExecuteScalarAsyncAction != null)
            {
                string sql = this.ToSqlString();
                return ExecuteScalarAsyncAction(sql);
            }
            else
                throw new Exception("ExecuteAsyncScalar 不能为空！");

        }
        /// <summary>
        /// 执行并返回影响行数
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteAsync()
        {
            if (ExecuteDelegateAsyncAction != null)
            {
                string sql = this.ToSqlString();
                return await ExecuteDelegateAsyncAction(sql);
            }
            else
                throw new Exception("ExecuteDelegateAction 不能为空！");
        }

        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> ExecuteListAsync()
        {

            try
            {
                string sql = this.ToSqlString();
                var result = Database.SqlQuery<T>(sql).ToList();
                return result;

            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> ExecuteListWithRowDelegateAsync()
        {

            try
            {
                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    List<T> data = result.ToList<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }


        /// <summary>
        /// 执行并返回List,适合大数据量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> ExecuteListByEmitAsync()
        {

            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    List<T> data = result.ToListByEmit<T>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteModelAsync()
        {
            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {

                    T data = result.ToModel<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }


        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteModelByEmitAsync()
        {
            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {

                    T data = result.ToModelByEmit<T>(true);

                    return data;
                }
                else
                    return default(T);
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.DataTable> ExecuteDataTableAsync()
        {
            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    DataTable data = result.ToDataTable(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.DataTable> ExecuteDataTableWithRowDelegateAsync()
        {
            try
            {

                string sql = this.ToSqlString();
                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    DataTable data = result.ToDataTableWithRowDelegate(true);

                    return data;
                }
                else
                    return null;
                
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataSet
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.DataSet> ExecuteDataSetAsync()
        {

            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    DataSet data = result.ToDataSet(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回Dictionary
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<TKey, TValue>> ExecuteDictionaryAsync<TKey, TValue>()
        {

            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回dynamic
        /// </summary>
        public async Task<dynamic> ExecuteExpandoObjectAsync()
        {

            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    dynamic data = result.ToExpandoObject(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回IEnumerable(dynamic)
        /// </summary>
        public async Task<IEnumerable<dynamic>> ExecuteExpandoObjectsAsync()
        {

            try
            {

                var result = await ExecuteReaderAsync();
                if (result != null)
                {
                    IEnumerable<dynamic> data = result.ToExpandoObjects(true);

                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw new PureDataException("FluentExpressionSQLCore", ex);
            }
            finally
            {
                if (Database != null)
                {
                    Database.Close();
                }
            }
        }
        #endregion

        #endregion
    }
}
