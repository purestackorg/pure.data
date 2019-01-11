using System;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    /// <summary>
    /// 分组容器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupByExpression<T> : FluentExpressionSQLCore<T>
    {
        //public FluentExpressionSQLCore<T> ExpressionSqlCore { get; set; }
        public SqlPack SqlPack { get; set; }
        public GroupByExpression(FluentExpressionSQLCore<T> e)
            : base(e)
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

        public GroupByExpression<T> ThenGroupBy( Expression<Func<T, object>> expression)
        {
            SqlPack += " ,";
            FluentExpressionSQLProvider.GroupBy(expression.Body, SqlPack);
            return this;
        }
        public GroupByExpression<T> Having(Expression<Func<T, bool>> expression)
        {
            SqlPack += " HAVING ";
            SqlPack.CurrentWhereExpression = expression.Body;

            FluentExpressionSQLProvider.Having(expression.Body, SqlPack);

            return this;
        }

        public GroupByExpression<T> ThenGroupBy<T2>(Expression<Func<T2, object>> expression)
        {
            SqlPack += " ,";
            FluentExpressionSQLProvider.GroupBy(expression.Body, SqlPack);
            return this;
        }
        public GroupByExpression<T> Having<T2>(Expression<Func<T2, bool>> expression)
        {
            SqlPack += " HAVING ";
            SqlPack.CurrentWhereExpression = expression.Body;

            FluentExpressionSQLProvider.Having(expression.Body, SqlPack);

            return this;
        }

    }

    //public static class GroupByExpressionExt
    //{
    //    public static GroupByExpression<T> ThenGroupBy<T>(this GroupByExpression<T> orderExp, Expression<Func<T, object>> expression)
    //    {
    //        orderExp.SqlPack += " ,";
    //        FluentExpressionSQLProvider.GroupBy(expression.Body, orderExp.SqlPack);
    //        return orderExp;
    //    }
    //    public static GroupByExpression<T> Having<T>(this GroupByExpression<T> Exp, Expression<Func<T, bool>> expression)
    //    {
    //        Exp.SqlPack += " HAVING ";
    //        Exp.SqlPack.CurrentWhereExpression = expression.Body;

    //        FluentExpressionSQLProvider.Having(expression.Body, Exp.SqlPack);

    //        return Exp;
    //    }
         
    //    /// <summary>
    //    /// 结束分组过滤，返回过滤容器
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="orderExp"></param>
    //    /// <returns></returns>
    //    public static FluentExpressionSQLCore<T> EndGroupBy<T>(this GroupByExpression<T>  Exp)
    //    {
    //        return Exp.ExpressionSqlCore;
    //    }
    //}

}
