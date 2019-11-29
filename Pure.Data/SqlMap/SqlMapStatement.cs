
using System.Collections.Generic;
using FluentExpressionSQL;
using System.Data;
using System;
using FluentExpressionSQL.Sql;
using System.Collections;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Pure.Data.SqlMap
{
    /// <summary>
    /// Sql map执行命令
    /// </summary>
    public class SqlMapStatement
    {
        IDatabase database = null;
       
        private void Init(IDatabase db)
        {
            database = db;
        }

        public Statement Statement { get; private set; }
        public RequestContext Context { get; private set; }
        private string _Sql = null;
        public string Sql
        {
            get
            {
                if (_Sql == null)
                {
                    lock (this)
                    {

                        _Sql = SqlWithoutOrderBy;

                        if (!string.IsNullOrEmpty(Context.OrderByText))
                        {
                            _Sql += " " + Context.OrderByText;
                        }
                        if (database.Config.FormatSql == true)
                        {
                            _Sql = _Sql.MergeSpace();

                        }
                    }
                }
                return _Sql;
            }
        }


        private string _SqlWithoutOrderBy = null;



        public string SqlWithoutOrderBy
        {
            get
            {
                if (_SqlWithoutOrderBy == null)
                {
                    lock (this)
                    {
                        Log("Building SqlMap :" + Statement.FullSqlId);

                        _SqlWithoutOrderBy = Statement.BuildSql(Context).Trim();
                        if (database.Config.FormatSql == true)
                        {
                            _SqlWithoutOrderBy = _SqlWithoutOrderBy.MergeSpace();
                        }

                    }
                }

                return _SqlWithoutOrderBy;
            }
        }
        object _Parameters = null;
        public object Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    lock (this)
                    {
                        _Parameters = Context.RequestParameters;

                    }
                }
                return _Parameters;
            }
        }

        private void Log(string str)
        {
            if (database != null && database.Config.EnableDebug == true)
            {
                database.LogHelper.WriteLine(str);
            }
        }

        //private void LogError(Exception ex)
        //{
        //    if (database != null && database.Config.EnableDebug == true)
        //    {
        //        database.LogHelper.WriteException(ex);
        //    }
        //}
        public SqlMapStatement(IDatabase db, Statement statement = null, RequestContext context = null)
        {
            //Sql = sql;
            //Parameters = param;
            Statement = statement;
            Context = context;
            Init(db);
        }
        private static object oLock = new object();
        ISqlDialectProvider _SqlDialect = null;
        ISqlDialectProvider SqlDialect
        {
            get
            {
                if (_SqlDialect == null)
                {
                    lock (oLock)
                    {

                        _SqlDialect = SqlDialectProviderLoader.GetSqlProviderByDatabaseType(database.DatabaseType);

                    }
                }


                return _SqlDialect;
            }
        }

        public string ParameterSuffix
        {
            get
            {
                return this.database.Config.ParameterSuffix;

            }
        }


        public static string ParameterPrefixInject
        {
            get
            {
                return "${";

            }
        }
        public string ParameterPrefix
        {
            get
            {
                return this.database.Config.ParameterPrefix;

            }
        }
        public string ExecuteParameterPrefix
        {
            get
            {
                return this.database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();

            }
        }


        public static string ParseRawSQL(string Sql,object Parameters, ISqlDialectProvider SqlDialect ,bool forSqlMap, string ParameterPrefix, string parameterSuffix) {
            string str = Sql;
            var prefix = ParameterPrefix;
            var dict = Parameters != null ? Parameters.ToDictionary() : null;
            object tmp = null;
            Type type = null;
            if (dict != null)
            {
                dict = dict.OrderByDescending(o => o.Key).ToDictionary(o => o.Key, p => p.Value); //避免同名前缀的问题:ID 和:ID_2，优先处理:ID_2

                foreach (var item in dict)
                {
                    type = item.Value != null ? item.Value.GetType() : null;
                    if (type.IsListType())
                    {
                        var list = item.Value as IEnumerable;
                        var sb = new StringBuilder(" (");
                        string strListItem = "";
                        string itemText = "";
                        foreach (var itemEnu in list)
                        {
                            var itemVal = SqlDialect.FormatValue(itemEnu, true);
                            if (itemVal == null)
                            {
                                itemText = "null";
                            }
                            if (itemVal.GetType() == typeof(string))
                            {
                                if (itemVal.ToString() == "")
                                {
                                    itemText = "null";
                                }
                            }
                            else
                            {
                                itemText = itemVal.ToString();
                            }
                              
                            sb.AppendFormat("{0},", itemText);
                        }
                        if (sb.Length == 0)
                        {
                            strListItem = "";
                        }
                        else
                        {
                            strListItem = sb.Remove(sb.Length - 1, 1).ToString();

                        }
                        strListItem += ") ";

                        str = str.Replace(prefix + item.Key + parameterSuffix, strListItem);
                        str = str.Replace(ParameterPrefixInject + item.Key + parameterSuffix, strListItem);

                        //if (forSqlMap == false)
                        //{
                        //    str = str.Replace(prefix + item.Key, strListItem);
                        //}
                        //else
                        //{
                        //    str = str.Replace(prefix + item.Key + parameterSuffix, strListItem);
                        //    str = str.Replace(ParameterPrefixInject + item.Key + parameterSuffix, strListItem);
                        //}

                    }
                    else
                    {
                        if (str.Contains(prefix + item.Key + parameterSuffix) || str.Contains(ParameterPrefixInject + item.Key + parameterSuffix))
                        {
                            tmp = SqlDialect.FormatValue(item.Value, true); //item.Value;// SqlDialect.FormatValue(item.Value);
                            str = str.Replace(prefix + item.Key + parameterSuffix, tmp != null && tmp.ToString() != "" ? tmp.ToString() : "null");
                            str = str.Replace(ParameterPrefixInject + item.Key + parameterSuffix, tmp != null && tmp.ToString() != "" ? tmp.ToString() : "null");

                        }
                        //if (forSqlMap == false)
                        //{
                        //    if (str.Contains(prefix + item.Key))
                        //    {
                        //        tmp = SqlDialect.FormatValue(item.Value);
                        //        str = str.Replace(prefix + item.Key, tmp != null && tmp.ToString() != "" ? tmp.ToString() : "null");

                        //    }

                        //}
                        //else
                        //{
                        //    //sql map ${xx}
                        //    if (str.Contains(prefix + item.Key + parameterSuffix) || str.Contains(ParameterPrefixInject + item.Key + parameterSuffix))
                        //    {
                        //        tmp = SqlDialect.FormatValue(item.Value); //item.Value;// SqlDialect.FormatValue(item.Value);
                        //        str = str.Replace(prefix + item.Key + parameterSuffix, tmp != null && tmp.ToString() != "" ? tmp.ToString() : "null");
                        //        str = str.Replace(ParameterPrefixInject + item.Key + parameterSuffix, tmp != null && tmp.ToString() != "" ? tmp.ToString() : "null");

                        //    }

                        //}
                    }

                }
            }


            return str;
        }

        public string RawSql
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Sql))
                {

                    var prefix = ParameterPrefix;// this.database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();
                    string str = Sql;

                    str = ParseRawSQL(Sql, Parameters, SqlDialect, true, ParameterPrefix, ParameterSuffix);
                    //var dict = Parameters != null ? Parameters.ToDictionary() : null;
                    //object tmp = null;
                    //Type type = null;
                    //if (dict != null)
                    //{
                    //    foreach (var item in dict)
                    //    {
                    //        type = item.Value != null ? item.Value.GetType() : null;
                    //        if (type.IsListType())
                    //        {
                    //            var list = item.Value as IEnumerable;
                    //            var sb = new StringBuilder(" (");
                    //            string strListItem = "";
                    //            foreach (var itemEnu in list)
                    //            {
                    //                sb.AppendFormat("{0},", SqlDialect.FormatValue(itemEnu));
                    //            }
                    //            if (sb.Length == 0)
                    //            {
                    //                strListItem = "";
                    //            }
                    //            else
                    //            {
                    //                strListItem = sb.Remove(sb.Length - 1, 1).ToString();

                    //            }
                    //            strListItem += ") ";
                    //            str = str.Replace(prefix + item.Key + ParameterSuffix, strListItem);

                    //        }
                    //        else
                    //        {
                    //            tmp = SqlDialect.FormatValue(item.Value);
                    //            str = str.Replace(prefix + item.Key + ParameterSuffix, tmp != null ? tmp.ToString() : "");
                    //        }

                    //    }
                    //}


                    return str;
                }

                return this.Sql;

            }
        }

        /// <summary>
        /// 过滤用不上的参数，oracle会报错
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, object> FilterParameters(string sql, object parameters)
        {

            var prefix = ParameterPrefix;// this.database.SqlGenerator.Configuration.Dialect.ParameterPrefix.ToString();
            var dictFilterParas = parameters != null ? parameters.ToDictionary() : null;
            string sqltext = sql;

            if (dictFilterParas != null)
            {
                List<string> waitToRemoveKeys = new List<string>();

                foreach (var item in dictFilterParas)
                {
                    if (!sqltext.Contains(prefix + item.Key.TrimStart(prefix) + ParameterSuffix)//#{xx}
                        &&
                        !sqltext.Contains(ParameterPrefixInject + item.Key.TrimStart(ParameterPrefixInject) + ParameterSuffix)//${xx}
                        ) //NAME(EXCEPT),NAME2 -> like '%:NAME%'  , AND A.NAME =:NAME
                    {
                        waitToRemoveKeys.Add(item.Key);

                    }
                }
                foreach (var item in waitToRemoveKeys)
                {
                    if (dictFilterParas.ContainsKey(item))
                    {
                        dictFilterParas.Remove(item);
                    }
                }
            }




            return dictFilterParas;

        }

        private string FilterSql(string sql, IDictionary<string, object> dictFilterParas)
        {
            if (dictFilterParas != null)
            {
                foreach (var item in dictFilterParas)
                {
                    string p = ParameterPrefix + item.Key.TrimStart(ParameterPrefix) + ParameterSuffix;
                    if (sql.Contains(p)) //NAME(EXCEPT),NAME2 -> like '%:NAME%'  , AND A.NAME =:NAME
                    {
                        sql = sql.Replace(p, ExecuteParameterPrefix + item.Key.TrimStart(ParameterPrefix));//过滤掉后缀
                    }
                    p = ParameterPrefixInject + item.Key.TrimStart(ParameterPrefixInject) + ParameterSuffix;
                    if (sql.Contains(p)) //${xx}
                    {
                        sql = sql.Replace(p, item.Value != null ? item.Value.ToString() :"");//过滤掉后缀
                    }
                }
            }

            return sql;
        }

        private object TryGetOrSetCache(Func<object> func)
        {
            object o = null;

            if (Statement.HasCache == false)
            {

                if (func != null)
                {
                    o = func();
                }
                return o;
            }
            else
            {
                if (Statement != null && Context != null)
                {
                    o = CacheManager.Instance[Context];

                    if (o == null)
                    {
                        if (func != null)
                        {
                            o = func();
                        }
                        CacheManager.Instance[Context] = o;
                        return o;

                    }
                    else
                    {
                        return o;
                    }
                }

                return o;
            }
            
        }
        private async  Task<object> TryGetOrSetCacheAsync( Func<Task<object>> func)
        {
            object o = null;

            if (Statement.HasCache == false)
            {

                if (func != null)
                {
                    o = await func();
                }
                return o;
            }
            else
            {
                if (Statement != null && Context != null)
                {
                    o = CacheManager.Instance[Context];

                    if (o == null)
                    {
                        if (func != null)
                        {
                            o = await func();
                        }
                        CacheManager.Instance[Context] = o;
                        return o;

                    }
                    else
                    {
                        return o;
                    }
                }

                return o;
            }
        }

        /// <summary>
        /// 获取实际执行的SQL和参数集合
        /// </summary>
        /// <param name="filterParameters"></param>
        /// <returns></returns>
        public StatementExecuteContext GetStatementExecuteContext(bool formatSql = false)
        {
            string sql = "";
            sql = Statement.BuildSql(Context).Trim();
            if (formatSql == true)
            {
                sql = sql.MergeSpace();
            }

            if (!string.IsNullOrEmpty(Context.OrderByText))
            {
                sql += " " + Context.OrderByText;
            }
            if (formatSql == true)
            {
                sql = sql.MergeSpace();

            }

            IDictionary<string, object> filterParameters = FilterParameters(sql, this.Parameters);
            sql = FilterSql(sql, filterParameters);

            StatementExecuteContext context = new StatementExecuteContext(sql, filterParameters);


            return context;
        }

        #region Execute

        /// <summary>
        /// 执行并返回DataReader
        /// </summary>
        /// <returns></returns>
        public System.Data.IDataReader ExecuteReader(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return database.ExecuteReader(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("ExecuteReader中的database 不能为空！");


        }
        /// <summary>
        /// 执行并返回Scalar
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return database.ExecuteScalar(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("ExecuteScalar中的database 不能为空！");


        }
        /// <summary>
        /// 执行并返回影响行数
        /// </summary>
        /// <returns></returns>
        public int Execute(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return database.Execute(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("Execute中的database 不能为空！");


        }



        /// <summary>
        /// 转换为分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="orderText"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public List<T> ExecutePageList<T>(int pageIndex, int pageSize, out int total, string orderText = "", string countSql = "", IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {

                if (database != null)
                {
                    database.SetConnectionAlive(true);
                    string sqltext = this.SqlWithoutOrderBy;

                    pageSize = pageSize == 0 ? database.Config.DefaultPageSize : pageSize;
                    string strCount = "";
                    if (countSql != null && countSql != "")
                    {
                        strCount = countSql;
                    }
                    else
                    {
                        strCount = database.GetSqlOfCount(sqltext);
                    }

                    var filterParameters = FilterParameters(strCount, this.Parameters);
                    strCount = FilterSql(strCount, filterParameters);


                    total = Convert.ToInt32(database.ExecuteScalar(strCount, filterParameters));
                    if (!string.IsNullOrEmpty(orderText))
                    {
                        sqltext = sqltext + " ORDER BY " + orderText;
                    }
                    else
                    {
                        sqltext = sqltext + " " + Context.OrderByText;
                    }



                    var dictParameters = filterParameters;
                    string sqlPage = database.GetSqlOfPage(pageIndex, pageSize, sqltext, dictParameters);
                    //var data = database.ExecuteReader(sqlPage, dictParameters, transaction, commandTimeout, commandType);
                    //return data.ToList<T>(true);
                    sqlPage = FilterSql(sqlPage, dictParameters);
                    var result = database.SqlQuery<T>(sqlPage, dictParameters, transaction, true, commandTimeout, commandType).ToList();

                    return result;



                }
                else
                {
                    total = 0;
                    return null;
                }


            }
            catch (Exception ex)
            {
                //LogError(ex);
                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.SetConnectionAlive(false);
                    database.Close();
                }
            }

        }


        public IDataReader ExecutePageReader(int pageIndex, int pageSize, out int total, string orderText = "", string countSql = "", IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {

                if (database != null)
                {
                    database.SetConnectionAlive(true);
                    string sqltext = this.SqlWithoutOrderBy;

                    pageSize = pageSize == 0 ? database.Config.DefaultPageSize : pageSize;
                    string strCount = "";
                    if (countSql != null && countSql != "")
                    {
                        strCount = countSql;
                    }
                    else
                    {
                        strCount = database.GetSqlOfCount(sqltext);
                    }

                    var filterParameters = FilterParameters(strCount, this.Parameters);
                    strCount = FilterSql(strCount, filterParameters);


                    total = Convert.ToInt32(database.ExecuteScalar(strCount, filterParameters));
                    if (!string.IsNullOrEmpty(orderText))
                    {
                        sqltext = sqltext + " ORDER BY " + orderText;
                    }
                    else
                    {
                        sqltext = sqltext + " " + Context.OrderByText;
                    }



                    var dictParameters = filterParameters;
                    string sqlPage = database.GetSqlOfPage(pageIndex, pageSize, sqltext, dictParameters);
                    //var data = database.ExecuteReader(sqlPage, dictParameters, transaction, commandTimeout, commandType);
                    //return data.ToList<T>(true);
                    sqlPage = FilterSql(sqlPage, dictParameters);
                    var result = database.ExecuteReader(sqlPage, dictParameters, transaction,  commandTimeout, commandType) ;

                    return result;



                }
                else
                {
                    total = 0;
                    return null;
                }


            }
            catch (Exception ex)
            {
                //LogError(ex);
                throw new PureDataException("SqlMapStatement", ex);
            }
            //finally
            //{
            //    if (database != null)
            //    {
            //        database.SetConnectionAlive(false);
            //        database.Close();
            //    }
            //}

        }

        public IEnumerable<dynamic> ExecutePageExpandoObjects(int pageIndex, int pageSize, out int total, string orderText = "", string countSql = "", IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {

                if (database != null)
                {
                    database.SetConnectionAlive(true);
                    string sqltext = this.SqlWithoutOrderBy;

                    pageSize = pageSize == 0 ? database.Config.DefaultPageSize : pageSize;
                    string strCount = "";
                    if (countSql != null && countSql != "")
                    {
                        strCount = countSql;
                    }
                    else
                    {
                        strCount = database.GetSqlOfCount(sqltext);
                    }

                    var filterParameters = FilterParameters(strCount, this.Parameters);
                    strCount = FilterSql(strCount, filterParameters);


                    total = Convert.ToInt32(database.ExecuteScalar(strCount, filterParameters));
                    if (!string.IsNullOrEmpty(orderText))
                    {
                        sqltext = sqltext + " ORDER BY " + orderText;
                    }
                    else
                    {
                        sqltext = sqltext + " " + Context.OrderByText;
                    }



                    var dictParameters = filterParameters;
                    string sqlPage = database.GetSqlOfPage(pageIndex, pageSize, sqltext, dictParameters);
                    //var data = database.ExecuteReader(sqlPage, dictParameters, transaction, commandTimeout, commandType);
                    //return data.ToList<T>(true);
                    sqlPage = FilterSql(sqlPage, dictParameters);
                    var result = database.ExecuteExpandoObjects(sqlPage, dictParameters, transaction, commandTimeout, commandType);

                    return result;



                }
                else
                {
                    total = 0;
                    return null;
                }


            }
            catch (Exception ex)
            {
                //LogError(ex);
                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.SetConnectionAlive(false);
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteList<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text, bool buffer = true)
        {
            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    string sql = this.Sql;
                    var filterParameters = FilterParameters(sql, this.Parameters);
                    sql = FilterSql(sql, filterParameters);

                    var result = database.SqlQuery<T>(sql, filterParameters, transaction, buffer, commandTimeout, commandType).ToList();

                    return result;
                    //if (result != null)
                    //{
                    //    List<T> data = result.ToList<T>(true);

                    //    return data;
                    //}
                    //else
                    //    return null;
                });

                return o as List<T>;
            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }


        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteListWithRowDelegate<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var sql = this.Sql;
                    var filterParameters = FilterParameters(sql, this.Parameters);
                    sql = FilterSql(sql, filterParameters);

                    var result = database.ExecuteListWithRowDelegate<T>(sql, filterParameters, transaction, commandTimeout, commandType).ToList();

                    return result;

                });

                return o as List<T>;
            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回List,适合大数据量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteListByEmit<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            try
            {
                var o = TryGetOrSetCache(() =>
                {

                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        List<T> data = result.ToListByEmit<T>(true);

                        return data;
                    }
                    else
                        return null;
                });

                return o as List<T>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteModel<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = TryGetOrSetCache(() =>
                {

                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        T data = result.ToModel<T>(true, database);
                        return data;
                    }
                    else
                        return default(T);
                });

                return (T)o;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }

        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteModelByEmit<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        T data = result.ToModelByEmit<T>(true);
                        return data;
                    }
                    else
                        return default(T);
                });

                return (T)o;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTable(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = TryGetOrSetCache(() =>
                {

                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        DataTable data = result.ToDataTable(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as System.Data.DataTable;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable ExecuteDataTableWithRowDelegate(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var sql = this.Sql;
                    var filterParameters = FilterParameters(sql, this.Parameters);
                    sql = FilterSql(sql, filterParameters);

                    var result = database.ExecuteDataTableWithRowDelegate(sql, filterParameters, transaction, commandTimeout, commandType);

                    return result;
                });

                return o as System.Data.DataTable;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataSet
        /// </summary>
        public System.Data.DataSet ExecuteDataSet(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        var data = result.ToDataSet(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as System.Data.DataSet;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回Dictionary
        /// </summary>
        public Dictionary<TKey, TValue> ExecuteDictionary<TKey, TValue>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as Dictionary<TKey, TValue>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回dynamic
        /// </summary>
        public dynamic ExecuteExpandoObject(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        dynamic data = result.ToExpandoObject(true, database);
                        return data;
                    }
                    else
                        return null;
                });

                return o as dynamic;


            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回IEnumerable(dynamic)
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> ExecuteExpandoObjects(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = TryGetOrSetCache(() =>
                {
                    var result = ExecuteReader(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        IEnumerable<dynamic> data = result.ToExpandoObjects(true, database);
                        return data;
                    }
                    else
                        return null;
                });

                return o as IEnumerable<dynamic>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        #endregion
#if ASYNC
        #region Execute Async

        /// <summary>
        /// 执行并返回DataReader
        /// </summary>
        /// <returns></returns>
        public async Task<IDataReader> ExecuteReaderAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return await database.ExecuteReaderAsync(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("ExecuteReader中的database 不能为空！");


        }
        /// <summary>
        /// 执行并返回Scalar
        /// </summary>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return await database.ExecuteScalarAsync(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("ExecuteScalar中的database 不能为空！");


        }
        /// <summary>
        /// 执行并返回影响行数
        /// </summary>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            if (database != null)
            {
                string sql = this.Sql;
                var filterParameters = FilterParameters(sql, this.Parameters);
                sql = FilterSql(sql, filterParameters);
                return await database.ExecuteAsync(sql, filterParameters, transaction, commandTimeout, commandType);
            }
            else
                throw new Exception("Execute中的database 不能为空！");


        }



        /// <summary>
        /// 转换为分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="orderText"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<PageDataResult<List<T>>> ExecutePageListAsync<T>(int pageIndex, int pageSize,  string orderText = "", string countSql = "", IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            try
            {
                if (database != null)
                {
                    database.SetConnectionAlive(true);
                    string sqltext = this.SqlWithoutOrderBy;

                    pageSize = pageSize == 0 ? database.Config.DefaultPageSize : pageSize;
                    string strCount = "";
                    if (countSql != null && countSql != "")
                    {
                        strCount = countSql;
                    }
                    else
                    {
                        strCount = database.GetSqlOfCount(sqltext);
                    }

                    var filterParameters = FilterParameters(strCount, this.Parameters);
                    strCount = FilterSql(strCount, filterParameters);

                    var countObj = await database.ExecuteScalarAsync(strCount, filterParameters);
                    var total = Convert.ToInt32(countObj);
                    if (!string.IsNullOrEmpty(orderText))
                    {
                        sqltext = sqltext + " ORDER BY " + orderText;
                    }
                    else
                    {
                        sqltext = sqltext + " " + Context.OrderByText;
                    }



                    var dictParameters = filterParameters;
                    string sqlPage = database.GetSqlOfPage(pageIndex, pageSize, sqltext, dictParameters);
                    //var data = database.ExecuteReader(sqlPage, dictParameters, transaction, commandTimeout, commandType);
                    //return data.ToList<T>(true);
                    sqlPage = FilterSql(sqlPage, dictParameters);
                    var result = await database.ExecuteListAsync<T>(sqlPage, dictParameters, transaction, true, commandTimeout, commandType);

                    return new PageDataResult<List<T>>(pageIndex, pageSize, total, result);



                }
                else
                {
                     
                    return PageDataResult<List<T>>.Empty(pageIndex, pageSize);
                }


            }
            catch (Exception ex)
            {
                //LogError(ex);
                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.SetConnectionAlive(false);
                    database.Close();
                }
            }

        }


        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> ExecuteListAsync<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text, bool buffer = true)
        {
            try
            {
                var o =  await TryGetOrSetCacheAsync(async () =>
                {
                    string sql = this.Sql;
                    var filterParameters = FilterParameters(sql, this.Parameters);
                    sql = FilterSql(sql, filterParameters);
                    var result = await database.ExecuteListAsync<T>(sql, filterParameters, transaction, buffer, commandTimeout, commandType) ;
                    return result;
                });
                return o as List<T>;
            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }


        /// <summary>
        /// 执行并返回List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<List<T>> ExecuteListWithRowDelegateAsync<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var sql = this.Sql;
                    var filterParameters = FilterParameters(sql, this.Parameters);
                    sql = FilterSql(sql, filterParameters);

                    var result = await database.ExecuteListWithRowDelegateAsync<T>(sql, filterParameters, transaction, commandTimeout, commandType) ;

                    return result;

                });

                return o as List<T>;
            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回List,适合大数据量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async  Task<List<T>> ExecuteListByEmitAsync<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {
            try
            {
                var o =await TryGetOrSetCacheAsync(async () =>
                {

                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        List<T> data = result.ToListByEmit<T>(true);

                        return data;
                    }
                    else
                        return null;
                });

                return o as List<T>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteModelAsync<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                
                var o =await TryGetOrSetCacheAsync(async () =>
                {

                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        T data = result.ToModel<T>(true, database);
                        return data;
                    }
                    else
                        return default(T);
                });

                return (T)o;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }

        /// <summary>
        /// 执行并返回单个实体数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> ExecuteModelByEmitAsync<T>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        T data = result.ToModelByEmit<T>(true);
                        return data;
                    }
                    else
                        return default(T);
                });

                return (T)o;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }

        }
        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.DataTable> ExecuteDataTableAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {

                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        DataTable data = result.ToDataTable(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as System.Data.DataTable;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回DataTable
        /// </summary>
        /// <returns></returns>
        public async Task<System.Data.DataTable> ExecuteDataTableWithRowDelegateAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {

                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        DataTable data = result.ToDataTableWithRowDelegate(true, database);
                         
                        return data;
                    }
                    else
                        return null;
                });


                
                return o as System.Data.DataTable;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回DataSet
        /// </summary>
        public async Task<System.Data.DataSet> ExecuteDataSetAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        var data = result.ToDataSet(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as System.Data.DataSet;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        /// <summary>
        /// 执行并返回Dictionary
        /// </summary>
        public async Task<Dictionary<TKey, TValue>> ExecuteDictionaryAsync<TKey, TValue>(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        Dictionary<TKey, TValue> data = result.ToDictionary<TKey, TValue>(true);
                        return data;
                    }
                    else
                        return null;
                });

                return o as Dictionary<TKey, TValue>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回dynamic
        /// </summary>
        public async Task<dynamic> ExecuteExpandoObjectAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {

            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        dynamic data = result.ToExpandoObject(true, database);
                        return data;
                    }
                    else
                        return null;
                });

                return o as dynamic;


            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }

        /// <summary>
        /// 执行并返回IEnumerable(dynamic)
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> ExecuteExpandoObjectsAsync(IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = CommandType.Text)
        {


            try
            {
                var o = await TryGetOrSetCacheAsync(async () =>
                {
                    var result = await ExecuteReaderAsync(transaction, commandTimeout, commandType);
                    if (result != null)
                    {
                        IEnumerable<dynamic> data = result.ToExpandoObjects(true, database);
                        return data;
                    }
                    else
                        return null;
                });

                return o as IEnumerable<dynamic>;

            }
            catch (Exception ex)
            {
                //LogError(ex);

                throw new PureDataException("SqlMapStatement", ex);
            }
            finally
            {
                if (database != null)
                {
                    database.Close();
                }
            }
        }
        #endregion
#endif


    }

    /// <summary>
    /// 实际执行的上下文参数
    /// </summary>
    public class StatementExecuteContext
    {
        public StatementExecuteContext(string sql, IDictionary<string, object> parameters)
        {
            Sql = sql;
            Parameters = parameters;
        }
        public string Sql { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }

}
