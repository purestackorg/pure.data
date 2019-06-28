
using FluentExpressionSQL.Mapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    /// <summary>
    /// Expression 构建SQL 容器
    /// </summary>
    public class FluentExpressionSqlBuilder
	{
        public ExpDbType DatabaseType { get; private set; }
        public ITableMapperContainer TableMapperContainer { get; set; }
        /// <summary>
        /// 临时表名
        /// </summary>
        private static List<string> S_listEnglishWords;
        static FluentExpressionSqlBuilder() {
            S_listEnglishWords = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        }
        #region 执行代理方法
        public ExecuteScalarDelegate ExecuteScalarAction { get; set; }
        public ExecuteDelegate ExecuteDelegateAction { get; set; }
        public ExecuteReaderDelegate ExecuteReaderAction { get; set; }

        public ExecuteScalarAsyncDelegate ExecuteScalarAsyncAction { get; set; }
        public ExecuteAsyncDelegate ExecuteDelegateAsyncAction { get; set; }
        public ExecuteReaderAsyncDelegate ExecuteReaderAsyncAction { get; set; }
        public Pure.Data.IDatabase Database { get; set; }
         
        #endregion
        
        public FluentExpressionSqlBuilder(ExpDbType dbType, ITableMapperContainer tableMapperContainer = null, List<string> _TableAliasContainer = null)
        {
            DatabaseType = dbType;
            TableMapperContainer = tableMapperContainer;
            if (_TableAliasContainer != null && _TableAliasContainer.Count > 0)
            {
                S_listEnglishWords = _TableAliasContainer;
            }
            SubQueryStatement = new StatementSubQuery();

            _ExpressionContainers = new Dictionary<Type, dynamic>();
        }

        #region 子查询
        private StatementSubQuery SubQueryStatement = null;
        public void AddSubQuery(string sql, string join )
        {
            SubQueryStatement.Add(sql, join );
        }
        public void AddUsedAlias( Type t, string tableAlias)
        {
            SubQueryStatement.AddUsedAlias(t, tableAlias);
        }
        public void AddUsedParams(Dictionary<string, object> paramsDict)
        {
            SubQueryStatement.AddUsedParams(paramsDict);
        }
        public Dictionary<string, object> GetExistDbParameters()
        {
            return SubQueryStatement.GetExistDbParameters();
        }
        public List<string> GetExistTableAlias()
        {
            return SubQueryStatement.GetExistTableAlias();
        }
        public string ParseSubQuery()
        {
            return SubQueryStatement.ToString();
        }
        public bool ExistSubQuery()
        {
            return SubQueryStatement.Count() > 0;
        }
        public void ClearSubQuery()
        {
            SubQueryStatement.Clear();
        }
        #endregion

        private Dictionary<Type, dynamic> _ExpressionContainers = null;

        private FluentExpressionSQLCore<T> NewExpressionContainer<T>()
        {
            dynamic cached = null;
            FluentExpressionSQLCore<T> result = null;
            Type type = typeof(T);
            if (_ExpressionContainers.TryGetValue(type, out cached))
            {
                result = cached as FluentExpressionSQLCore<T>;
            }

            if (result == null)
            {
                var e = new FluentExpressionSQLCore<T>(DatabaseType, TableMapperContainer);
                e.ExecuteScalarAction = ExecuteScalarAction;
                e.ExecuteDelegateAction = ExecuteDelegateAction;
                e.ExecuteReaderAction = ExecuteReaderAction;
                e.ExecuteScalarAsyncAction = ExecuteScalarAsyncAction;
                e.ExecuteDelegateAsyncAction = ExecuteDelegateAsyncAction;
                e.ExecuteReaderAsyncAction = ExecuteReaderAsyncAction;
                e.Database = Database;

                result = e;
                _ExpressionContainers[type] = e;
            }
            else
            {
                result.Clear();
            }
             
            return result;
        }

		public FluentExpressionSQLCore<T> Delete<T>()
		{
			return NewExpressionContainer<T>().Delete();
		}

		public FluentExpressionSQLCore<T> Update<T>(Expression<Func<object>> expression = null)
		{
			return NewExpressionContainer<T>().Update(expression);
		}
        public FluentExpressionSQLCore<T> Insert<T>(Expression<Func<object>> expression = null)
        {
            return NewExpressionContainer<T>().Insert(expression);
        }
		public FluentExpressionSQLCore<T> Select<T>(Expression<Func<T, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2>(Expression<Func<T, T2, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3>(Expression<Func<T, T2, T3, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}

		public FluentExpressionSQLCore<T> Max<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Max(expression);
		}

		public FluentExpressionSQLCore<T> Min<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Min(expression);
		}

		public FluentExpressionSQLCore<T> Avg<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Avg(expression);
		}

		public FluentExpressionSQLCore<T> Count<T>(Expression<Func<T, object>> expression = null)
		{
			return NewExpressionContainer<T>().Count(expression);
		}

		public FluentExpressionSQLCore<T> Sum<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Sum(expression);
		}

        /// <summary>
        /// 创建新的FluentExpressionSQL构造器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public  FluentExpressionSQLCore<T> NewBuilder<T>()
        {
            return NewExpressionContainer<T>().NewBuilder();
        }

        //public string FromCache(  Func<FluentExpressionSqlBuilder, string> fc)
        //{
        //    int key = fc.GetHashCode();
        //    var c = SqlCached.GetSQLCache(key);
        //    if (c == null)
        //    {
        //        if (fc != null)
        //        {
        //            string s = fc(this);
        //            SqlCacheItem item = new SqlCacheItem();
        //            item.Sql = s;
        //            SqlCached.SetSQLCache(key, item);
        //            return s;
        //        }
        //    }
        //    else
        //    {
        //        return c.Sql;
        //    }

        //    return "";
        //}
 

	}
}
