
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    /// <summary>
    /// 常量表达式
    /// </summary>
	class ConstantFluentExpressionSQL : BaseFluentExpressionSQL<ConstantExpression>
	{
        private static ConstantFluentExpressionSQL _Instance = null;

        public static ConstantFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new ConstantFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        private object ConvertToData(ConstantExpression expression, SqlPack sqlPack)
        {
            object value = expression.GetValueOfExpression(sqlPack);
            //var dto = sqlPack.GetExtDto(expression);
            //if (dto != null)
            //{
            //    switch (dto.ActionType)
            //    {
            //        case SqlActionType.Like:
            //            value = "%" + value + "%";
            //            break;
            //        case SqlActionType.LikeLeft:
            //            value = "%" + value;
            //            break;
            //        case SqlActionType.LikeRight:
            //            value = value + "%";
            //            break;
            //        default:
            //            break;
            //    }
            //}
            return value;
        }

        protected override SqlPack Join(ConstantExpression expression, SqlPack sqlPack)
        {
            return Where(expression, sqlPack);
             

        }

        protected override SqlPack Select(ConstantExpression expression, SqlPack sqlPack)
        {

            object value = expression.GetValueOfExpression(sqlPack);
            string colStr = value.ToString();// sqlPack.SqlDialectProvider.FormatValue(value).ToString();
            if (!string.IsNullOrEmpty( sqlPack.CurrentColAlias))
            {
                colStr += " " +sqlPack.SqlDialectProvider.ColumnAsAliasString  + sqlPack.CurrentColAlias;
                sqlPack.CurrentColAlias = null;//重置
            }
            
            sqlPack.SelectFields.Add(colStr);
            return sqlPack;
        }
        public const string TrueLiteral = "(1=1)";
        public const string FalseLiteral = "(1=0)";
        protected override SqlPack Where(ConstantExpression expression, SqlPack sqlPack)
		{
            object value = ConvertToData(expression, sqlPack);//expression.Value

            //if (sqlPack.WhereConditionIndex ==1 && expression.Type == typeof(bool)) //只有第一次where才会拼接 1=1 或者1=0
            //{
            //    string str = (int)value == 1 ? TrueLiteral : FalseLiteral;
            //    sqlPack += " "+str;
            //    return sqlPack;

            //}
             
            sqlPack.AddDbParameter(value);
			return sqlPack;
		}

		protected override SqlPack In(ConstantExpression expression, SqlPack sqlPack)
		{
            object value = expression.GetValueOfExpression(sqlPack);
			if (expression.Type.Name == "String")
			{
                sqlPack.Sql.AppendFormat("'{0}',", value);
			}
			else
			{
                sqlPack.Sql.AppendFormat("{0},", value);
			}
			return sqlPack;
		}
	}
}