using FluentExpressionSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pure.Data
{
    /// <summary>
    /// 延迟加载实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlQuery<T> : IOrderedQueryable<T>
    {
        private Expression _expression;
        private IQueryProvider _provider;

        public SqlQuery(FluentExpressionSqlBuilder builder)
        {
            _provider = new SqlQueryProvider<T>(builder);
            _expression = Expression.Constant(this);
        }

        public SqlQuery(Expression expression, IQueryProvider provider)
        {
            _expression = expression;
            _provider = provider;
        }
        /// <summary>
        /// 调用ToList方法会自动执行解析
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var result = _provider.Execute<List<T>>(_expression);
            if (result == null)
                yield break;
            foreach (var item in result)
            {
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType
        {
            get { return typeof(SqlQuery<T>); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }
    }
}
