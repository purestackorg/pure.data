using System;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    /// <summary>
    /// 排序容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrderExpression<T> : FluentExpressionSQLCore<T>
    {
        //public FluentExpressionSQLCore<T> ExpressionSqlCore { get; set; }
        public SqlPack SqlPack { get; set; }
        public OrderExpression(FluentExpressionSQLCore<T> e):base(e)
		{
            //ExpressionSqlCore = e;
            SqlPack = e._sqlPack;
            this.ExecuteDelegateAction = e.ExecuteDelegateAction;
            this.ExecuteScalarAction = e.ExecuteScalarAction;
            this.ExecuteReaderAction = e.ExecuteReaderAction;
            this.ExecuteDelegateAsyncAction = e.ExecuteDelegateAsyncAction;
            this.ExecuteScalarAsyncAction = e.ExecuteScalarAsyncAction;
            this.ExecuteReaderAsyncAction = e.ExecuteReaderAsyncAction;
            this.Database = e.Database;

		}

        public OrderExpression<T> ThenBy<T2>(Expression<Func<T2, object>> expression)
        {
            SqlPack += " ,";
            FluentExpressionSQLProvider.OrderBy(expression.Body, SqlPack);

            return this;
        }
        public OrderExpression<T> ThenBy( Expression<Func<T, object>> expression)
        {
             SqlPack += " ,";
            FluentExpressionSQLProvider.OrderBy(expression.Body,  SqlPack);

            return this;
        }
        public OrderExpression<T> ThenByDescending(Expression<Func<T, object>> expression)
        {
             SqlPack += " ,";
            FluentExpressionSQLProvider.OrderBy(expression.Body,  SqlPack);
             SqlPack += " DESC";
            return this;
        }
        public OrderExpression<T> ThenByDescending<T2>(Expression<Func<T2, object>> expression)
        {
            SqlPack += " ,";
            FluentExpressionSQLProvider.OrderBy(expression.Body, SqlPack);
            SqlPack += " DESC";
            return this;
        }

    }

    //public static class OrderExpressionExt
    //{
    //    public static OrderExpression<T> ThenBy<T>(this OrderExpression<T> orderExp, Expression<Func<T, object>> expression)
    //    {
    //        orderExp.SqlPack += " ,";
    //        FluentExpressionSQLProvider.OrderBy(expression.Body, orderExp.SqlPack);

    //        return orderExp;
    //    }
    //    public static OrderExpression<T> ThenByDescending<T>(this OrderExpression<T> orderExp, Expression<Func<T, object>> expression)
    //    {
    //        orderExp.SqlPack += " ,";
    //        FluentExpressionSQLProvider.OrderBy(expression.Body, orderExp.SqlPack);
    //        orderExp.SqlPack += " DESC";
    //        return orderExp;
    //    }
    //    /// <summary>
    //    /// 结束排序过滤，返回过滤容器
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="orderExp"></param>
    //    /// <returns></returns>
    //    public static FluentExpressionSQLCore<T> EndOrder<T>(this OrderExpression<T> orderExp)
    //    {
    //        return orderExp.ExpressionSqlCore;
    //    }
    //}

}
