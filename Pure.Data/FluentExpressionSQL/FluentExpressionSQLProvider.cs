
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    internal class FluentExpressionSQLProvider 
	{
        private static readonly ConcurrentDictionary<Type, IFluentExpressionSQL> _FluentExpressionSQLCache = new ConcurrentDictionary<Type, IFluentExpressionSQL>();
        
        
		private static IFluentExpressionSQL GetFluentExpressionSQL(Expression expression)
		{
            if (expression == null)
            {
                throw new ArgumentNullException("expression", "不能为null");
            }

            Type type = expression.GetType();
            IFluentExpressionSQL sql;
            if (!_FluentExpressionSQLCache.TryGetValue(type, out sql))
            {
                if (expression is BinaryExpression)
                {
                    sql = BinaryFluentExpressionSQL.Instance;// new BinaryFluentExpressionSQL();
                }
                else if (expression is ConditionalExpression)
                {
                    sql = ConditionalFluentExpressionSQL.Instance;// new ConditionalFluentExpressionSQL();
                    //throw new NotImplementedException("未实现的ConditionalFluentExpressionSQL");
                }
                else if (expression is ConstantExpression)
                {
                    sql = ConstantFluentExpressionSQL.Instance;// new ConstantFluentExpressionSQL();
                }
                else if (expression is MemberExpression)
                {
                    sql = MemberFluentExpressionSQL.Instance;//new MemberFluentExpressionSQL();
                }
                else if (expression is MemberInitExpression)
                {
                    sql = MemberInitFluentExpressionSQL.Instance;//new MemberInitFluentExpressionSQL();
                }
                else if (expression is MethodCallExpression)
                {
                    sql = MethodCallFluentExpressionSQL.Instance;// new MethodCallFluentExpressionSQL();
                }
                else if (expression is NewArrayExpression)
                {
                    sql = NewArrayFluentExpressionSQL.Instance; //new NewArrayFluentExpressionSQL();
                }
                else if (expression is NewExpression)
                {
                    sql = NewFluentExpressionSQL.Instance; //new NewFluentExpressionSQL();
                }
                else if (expression is UnaryExpression)
                {
                    sql = UnaryFluentExpressionSQL.Instance;// new UnaryFluentExpressionSQL();
                }
                else if(expression is  ParameterExpression)
                {
                    sql = ParameterFluentExpressionSQL.Instance;// new ParameterFluentExpressionSQL();
                }
                else
                {
                    throw new NotImplementedException("未实现的"+type+"FluentExpressionSQL");
                }

                _FluentExpressionSQLCache[type] = sql;
                return sql;

                //else if (expression is BlockExpression)
                //{
                //    throw new NotImplementedException("未实现的BlockFluentExpressionSQL");
                //}
                //else if (expression is DebugInfoExpression)
                //{
                //    throw new NotImplementedException("未实现的DebugInfoFluentExpressionSQL");
                //}
                //else if (expression is DefaultExpression)
                //{
                //    throw new NotImplementedException("未实现的DefaultFluentExpressionSQL");
                //}
                //else if (expression is DynamicExpression)
                //{
                //    throw new NotImplementedException("未实现的DynamicFluentExpressionSQL");
                //}
                //else if (expression is GotoExpression)
                //{
                //    throw new NotImplementedException("未实现的GotoFluentExpressionSQL");
                //}
                //else if (expression is IndexExpression)
                //{
                //    throw new NotImplementedException("未实现的IndexFluentExpressionSQL");
                //}
                //else if (expression is InvocationExpression)
                //{
                //    throw new NotImplementedException("未实现的InvocationFluentExpressionSQL");
                //}
                //else if (expression is LabelExpression)
                //{
                //    throw new NotImplementedException("未实现的LabelFluentExpressionSQL");
                //}
                //else if (expression is LambdaExpression)
                //{
                //    throw new NotImplementedException("未实现的LambdaFluentExpressionSQL");
                //}
                //else if (expression is ListInitExpression)
                //{
                //    throw new NotImplementedException("未实现的ListInitFluentExpressionSQL");
                //}
                //else if (expression is LoopExpression)
                //{
                //    throw new NotImplementedException("未实现的LoopFluentExpressionSQL");
                //}

                //else if (expression is ParameterExpression)
                //{
                //    throw new NotImplementedException("未实现的ParameterFluentExpressionSQL");
                //}
                //else if (expression is RuntimeVariablesExpression)
                //{
                //    throw new NotImplementedException("未实现的RuntimeVariablesFluentExpressionSQL");
                //}
                //else if (expression is SwitchExpression)
                //{
                //    throw new NotImplementedException("未实现的SwitchFluentExpressionSQL");
                //}
                //else if (expression is TryExpression)
                //{
                //    throw new NotImplementedException("未实现的TryFluentExpressionSQL");
                //}
                //else if (expression is TypeBinaryExpression)
                //{
                //    throw new NotImplementedException("未实现的TypeBinaryFluentExpressionSQL");
                //}
                
            }

            return sql;

			
		}
         

		public static void Update(Expression expression, SqlPack sqlPack)
		{
             
            
			GetFluentExpressionSQL(expression).Update(expression, sqlPack);
		}
        public static void Insert(Expression expression, SqlPack sqlPack)
        {
            GetFluentExpressionSQL(expression).Insert(expression, sqlPack);
        }
		public static void Select(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Select(expression, sqlPack);
		}

		public static void Join(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Join(expression, sqlPack);
		}

		public static void Where(Expression expression, SqlPack sqlPack)
		{
            
			GetFluentExpressionSQL(expression).Where(expression, sqlPack);
		}

		public static void In(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).In(expression, sqlPack);
		}

		public static void GroupBy(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).GroupBy(expression, sqlPack);
		}
        public static void Having(Expression expression, SqlPack sqlPack)
        {
            GetFluentExpressionSQL(expression).Having(expression, sqlPack);
        }
		public static void OrderBy(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).OrderBy(expression, sqlPack);
		}

		public static void Max(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Max(expression, sqlPack);
		}

		public static void Min(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Min(expression, sqlPack);
		}

		public static void Avg(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Avg(expression, sqlPack);
		}

		public static void Count(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Count(expression, sqlPack);
		}

		public static void Sum(Expression expression, SqlPack sqlPack)
		{
			GetFluentExpressionSQL(expression).Sum(expression, sqlPack);
		}
	}
}
