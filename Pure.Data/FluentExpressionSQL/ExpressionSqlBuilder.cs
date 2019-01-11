
using FluentExpressionSQL.Mapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    /// <summary>
    /// Expression 构建SQL 容器
    /// </summary>
	public static class ExpressionSqlBuilder
	{
        public static ExpDbType DatabaseType { get; set; }
        public static ITableMapperContainer TableMapperContainer { get; set; }
        /// <summary>
        /// 临时表名
        /// </summary>
        public static readonly List<string> S_listEnglishWords = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

       
        public static void Init(ExpDbType dbType, ITableMapperContainer tableMapperContainer = null)
		{
			DatabaseType = dbType;
            TableMapperContainer = tableMapperContainer;
		}

        #region 子查询
        private static StatementSubQuery SubQueryStatement = new StatementSubQuery();
        public static void AddSubQuery(string sql, string join )
        {
            SubQueryStatement.Add(sql, join );
        }
        public static void AddUsedAlias( Type t, string tableAlias)
        {
            SubQueryStatement.AddUsedAlias(t, tableAlias);
        }
        public static void AddUsedParams(Dictionary<string, object> paramsDict)
        {
            SubQueryStatement.AddUsedParams(paramsDict);
        }
        public static Dictionary<string, object> GetExistDbParameters()
        {
            return SubQueryStatement.GetExistDbParameters();
        }
        public static List<string> GetExistTableAlias()
        {
            return SubQueryStatement.GetExistTableAlias();
        }
        public static string ParseSubQuery()
        {
            return SubQueryStatement.ToString();
        }
        public static bool ExistSubQuery()
        {
            return SubQueryStatement.Count() > 0;
        }
        public static void ClearSubQuery()
        {
            SubQueryStatement.Clear();
        }
        #endregion
       
        private static FluentExpressionSQLCore<T> NewExpressionContainer<T>()
        {
            return new FluentExpressionSQLCore<T>(DatabaseType, TableMapperContainer );
        }

		public static FluentExpressionSQLCore<T> Delete<T>()
		{
			return NewExpressionContainer<T>().Delete();
		}

		public static FluentExpressionSQLCore<T> Update<T>(Expression<Func<object>> expression = null)
		{
			return NewExpressionContainer<T>().Update(expression);
		}
        public static FluentExpressionSQLCore<T> Insert<T>(Expression<Func<object>> expression = null)
        {
            return NewExpressionContainer<T>().Insert(expression);
        }
		public static FluentExpressionSQLCore<T> Select<T>(Expression<Func<T, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2>(Expression<Func<T, T2, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3>(Expression<Func<T, T2, T3, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}
		public static FluentExpressionSQLCore<T> Select<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expression = null)
		{
			return NewExpressionContainer<T>().Select(expression);
		}

		public static FluentExpressionSQLCore<T> Max<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Max(expression);
		}

		public static FluentExpressionSQLCore<T> Min<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Min(expression);
		}

		public static FluentExpressionSQLCore<T> Avg<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Avg(expression);
		}

		public static FluentExpressionSQLCore<T> Count<T>(Expression<Func<T, object>> expression = null)
		{
			return NewExpressionContainer<T>().Count(expression);
		}

		public static FluentExpressionSQLCore<T> Sum<T>(Expression<Func<T, object>> expression)
		{
			return NewExpressionContainer<T>().Sum(expression);
		}

        /// <summary>
        /// 创建新的FluentExpressionSQL构造器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static FluentExpressionSQLCore<T> NewBuilder<T>( )
        {
            return NewExpressionContainer<T>().NewBuilder();
        }
      

	}
}
