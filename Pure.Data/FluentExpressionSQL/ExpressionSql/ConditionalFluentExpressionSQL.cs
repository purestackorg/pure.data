
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{

    class ConditionalFluentExpressionSQL : BaseFluentExpressionSQL<ConditionalExpression>
	{
        private static ConditionalFluentExpressionSQL _Instance = null;

        public static ConditionalFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new ConditionalFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        private bool IsEndCase(ConditionalExpression expression)
        {
            bool isEnd = false;
            if (expression.IfFalse.IsEndToken() && expression.IfTrue.IsEndToken())
            {
                isEnd = true;
            }
            return isEnd;
        }

       

        protected override SqlPack Select(ConditionalExpression expression, SqlPack sqlPack)
        {
            
            var condition = expression.Test.GetValueOfExpression(sqlPack);
            if (expression.IfTrue.IsEndToken())
            {
                 object ifTrueValue = expression.IfTrue.GetValueWhenBoolToInt(sqlPack);
                    sqlPack.CaseWhenStatement.AddCaseWhen(condition, ifTrueValue);
            }
             

            if (IsEndCase(expression))//如果是结束标记
            {
                object ifFalseValue = expression.IfFalse.GetValueWhenBoolToInt(sqlPack);

                sqlPack.CaseWhenStatement.AddElse(ifFalseValue);

                string colStr = sqlPack.CaseWhenStatement.ToString();
                sqlPack.CaseWhenStatement.Clear();
                if (!string.IsNullOrEmpty(sqlPack.CurrentColAlias))
                {
                    colStr += " " + sqlPack.SqlDialectProvider.ColumnAsAliasString + sqlPack.CurrentColAlias;
                    sqlPack.CurrentColAlias = null;//重置
                }

                sqlPack.SelectFields.Add(colStr);
                return sqlPack;
            }
            else
            {
                if (!expression.IfTrue.IsEndToken())
                {
                    Select(expression.IfTrue, sqlPack);
                    
                }
                if (!expression.IfFalse.IsEndToken())
                {
                    Select(expression.IfFalse, sqlPack);
                    
                }
                return sqlPack;
            }

           
        }

	}


  

}