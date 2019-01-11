
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
	class NewArrayFluentExpressionSQL : BaseFluentExpressionSQL<NewArrayExpression>
	{
        private static NewArrayFluentExpressionSQL _Instance = null;

        public static NewArrayFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new NewArrayFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        protected override SqlPack In(NewArrayExpression expression, SqlPack sqlPack)
		{
			sqlPack += " (";

            List<object> args = new List<object>();
			foreach (Expression expressionItem in expression.Expressions)
			{
                args.Add(expressionItem.GetValueOfExpression(sqlPack));
				//FluentExpressionSQLProvider.In(expressionItem, sqlPack);
			}
            sqlPack.AddDbParameter(args);
            //if (sqlPack.Sql[sqlPack.Sql.Length - 1] == ',')
            //{
            //    sqlPack.Sql.Remove(sqlPack.Sql.Length - 1, 1);
            //}

			sqlPack += ")";

			return sqlPack;
		}
	}
}
