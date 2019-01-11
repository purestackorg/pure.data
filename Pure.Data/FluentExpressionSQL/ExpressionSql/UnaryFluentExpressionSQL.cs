
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
	class UnaryFluentExpressionSQL : BaseFluentExpressionSQL<UnaryExpression>
	{
        private static UnaryFluentExpressionSQL _Instance = null;

        public static UnaryFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new UnaryFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        protected override SqlPack Select(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Select(expression.Operand, sqlPack);
			return sqlPack;
		}

        protected override SqlPack Join(UnaryExpression expression, SqlPack sqlPack)
        {
            FluentExpressionSQLProvider.Where(expression.Operand, sqlPack);
            return sqlPack;
        }

		protected override SqlPack Where(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Where(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack GroupBy(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.GroupBy(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack OrderBy(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.OrderBy(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack Max(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Max(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack Min(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Min(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack Avg(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Avg(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack Count(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Count(expression.Operand, sqlPack);
			return sqlPack;
		}

		protected override SqlPack Sum(UnaryExpression expression, SqlPack sqlPack)
		{
			FluentExpressionSQLProvider.Sum(expression.Operand, sqlPack);
			return sqlPack;
		}
	}
}
