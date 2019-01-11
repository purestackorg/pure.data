using FluentExpressionSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Pure.Data
{
    /// <summary>
    /// 延迟加载驱动器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlQueryProvider<T> : IQueryProvider
    {
        FluentExpressionSqlBuilder _builder = null;
        public SqlQueryProvider(FluentExpressionSqlBuilder builder)
        {
            _builder = builder;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            IQueryable<TElement> query = new SqlQuery<TElement>(expression, this);
            return query;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 解析Expression 并返回IEnumerable数据
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression expression)
        {
            MethodCallExpression methodCall = expression as MethodCallExpression;
            Expression<Func<T, bool>> result = null;
            while (methodCall != null)
            {
                Expression method = methodCall.Arguments[0];
                Expression lambda = methodCall.Arguments[1];
                LambdaExpression right = (lambda as UnaryExpression).Operand as LambdaExpression;
                if (result == null)
                {
                    result = Expression.Lambda<Func<T, bool>>(right.Body, right.Parameters);
                }
                else
                {
                    Expression left = (result as LambdaExpression).Body;
                    Expression temp = Expression.And(right.Body, left);//所有加入的查询条件都用And连接
                    result = Expression.Lambda<Func<T, bool>>(temp, result.Parameters);


              //return Expression.Lambda<Func<T, bool>>
              //(Expression.Or(expr1.Body, expr2.Body), expr1.Parameters);

                }
                methodCall = method as MethodCallExpression;
            }
            if (result != null)
            {
                var source = _builder.Select<T>().Where(result).ExecuteList();
                dynamic _temp = source;
                TResult t = (TResult)_temp;
                return t;
            }
            else
            {
                var source = _builder.Select<T>().ExecuteList();
                dynamic _temp = source;
                TResult t = (TResult)_temp;
                return t;
            }
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

       

    }
}
