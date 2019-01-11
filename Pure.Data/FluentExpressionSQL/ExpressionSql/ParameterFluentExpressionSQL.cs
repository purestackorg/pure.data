
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    class ParameterFluentExpressionSQL : BaseFluentExpressionSQL<ParameterExpression>
	{
        private static ParameterFluentExpressionSQL _Instance = null;

        public static ParameterFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new ParameterFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        protected override SqlPack Select(ParameterExpression expression, SqlPack sqlPack)
        {

            string tableName = expression.GetTableNameByExpression(sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            var colStr = tableAlias+"*";
            sqlPack.SelectFields.Add(colStr);
            return sqlPack;

 
        }

	 
	}
}