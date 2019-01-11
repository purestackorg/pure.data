
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentExpressionSQL
{
    class NewFluentExpressionSQL : BaseFluentExpressionSQL<NewExpression>
    {
        private static NewFluentExpressionSQL _Instance = null;

        public static NewFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new NewFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        protected override SqlPack Update(NewExpression expression, SqlPack sqlPack)
        {
            for (int i = 0; i < expression.Members.Count; i++)
            {
                MemberInfo m = expression.Members[i];
                //ConstantExpression c = expression.Arguments[i] as ConstantExpression;
                sqlPack += m.Name + " =";
                object value = expression.Arguments[i].GetValueOfExpression(sqlPack);
                sqlPack.AddDbParameter(value);
                sqlPack += ",";
            }
            if (sqlPack[sqlPack.Length - 1] == ',')
            {
                sqlPack.Sql.Remove(sqlPack.Length - 1, 1);
            }
            return sqlPack;
        }

        protected override SqlPack Insert(NewExpression expression, SqlPack sqlPack)
        {
            StringBuilder columns = new StringBuilder();
            for (int i = 0; i < expression.Members.Count; i++)
            {
                MemberInfo m = expression.Members[i];
                //ConstantExpression c = expression.Arguments[i] as ConstantExpression;
                //object value = c.Value;
                columns.Append(m.Name);
                columns.Append(",");

                object value = expression.Arguments[i].GetValueOfExpression(sqlPack);
                sqlPack.AddDbParameter(value);
                sqlPack += ",";
            }
            columns = columns.Remove(columns.Length - 1, 1);

            if (sqlPack[sqlPack.Length - 1] == ',')
            {
                sqlPack.Sql.Remove(sqlPack.Length - 1, 1);
            }

            sqlPack.FormatSql(columns.ToString());
            return sqlPack;
        }


        protected override SqlPack Select(NewExpression expression, SqlPack sqlPack)
        {
            //foreach (Expression item in expression.Arguments)
            //{
            //    FluentExpressionSQLProvider.Select(item, sqlPack);
            //}
            var memberAliasExpList = expression.Members;
            string tableAlias = "";
            string colStr = "";
            int i = 0;

            foreach (Expression item in expression.Arguments)
            {
                var memberAliasExp = memberAliasExpList[i];

                if (item is MemberExpression)
                {
                    MemberExpression memberValueExp = item as MemberExpression;
                    if (memberValueExp != null)
                    {

                        if (memberValueExp.Member.DeclaringType == ResolveConstants.TypeOfDateTime)//日期转换
                        {
                            var value = memberValueExp.Expression.GetValueOfExpression(sqlPack);

                            var valueDatetime = sqlPack.SqlDialectProvider.ConvertDateTime(memberValueExp.Member, value);
                            if (memberAliasExp.Name.Equals(memberValueExp.Member.Name, System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                colStr = valueDatetime.Text;
                            }
                            else
                            {
                                colStr = valueDatetime.Text + " " + sqlPack.ColumnAsAliasString  + memberAliasExp.Name;

                            }
                        }
                        else if (memberValueExp.Member.Name == "Length" && memberValueExp.Member.DeclaringType == ResolveConstants.TypeOfString)//长度转换
                        {
                            MemberExpression memberEx = memberValueExp.Expression as MemberExpression;
                            string tableName = memberEx.GetTableNameByExpression(sqlPack);
                            sqlPack.SetTableAlias(tableName);
                            tableAlias = sqlPack.GetTableAlias(tableName);
                            if (!string.IsNullOrWhiteSpace(tableAlias))
                            {
                                tableAlias += ".";
                            }
                            string param = tableAlias + memberEx.Member.Name;

                            colStr =  sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Length, param);

                            if (memberAliasExp.Name.Equals(memberEx.Member.Name, System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                //colStr = tableAlias + memberEx.Member.Name;
                            }
                            else
                            {
                                colStr += " " + sqlPack.ColumnAsAliasString + " " + memberAliasExp.Name;

                            }
                            
                        }
                        else if (memberValueExp.Member.MemberType == MemberTypes.Field) //局部变量
                        {
                            var value = memberValueExp.Expression.GetValueOfExpression(sqlPack);

                            var memberInfoType = memberValueExp.Member.GetPropertyOrFieldType();
                            var memberInfoValue = memberValueExp.Member.GetPropertyOrFieldValue(value);
                            memberInfoValue = sqlPack.SqlDialectProvider.FormatValue(memberInfoValue);//format
                            if (memberInfoType == ResolveConstants.TypeOfDateTime)
                            {
                                colStr = sqlPack.SqlDialectProvider.ConvertSqlValue(memberInfoValue, memberInfoType);
                            }
                            else
                            {
                                colStr = memberInfoValue.ToString();
                            }

                            if (!string.IsNullOrEmpty(memberAliasExp.Name) && !string.IsNullOrEmpty(colStr))
                            {
                                colStr += " " + sqlPack.ColumnAsAliasString + " " + memberAliasExp.Name;
                            }

                        }
                        else
                        {
                            string tableName = memberValueExp.GetTableNameByExpression(sqlPack);

                            sqlPack.SetTableAlias(tableName);//memberValueExp.Member.DeclaringType.Name
                            tableAlias = sqlPack.GetTableAlias(tableName);//memberValueExp.Member.DeclaringType.Name
                            if (!string.IsNullOrWhiteSpace(tableAlias))
                            {
                                tableAlias += ".";
                            }
                            if (memberAliasExp.Name.Equals(memberValueExp.Member.Name, System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                colStr = tableAlias + memberValueExp.Member.Name;
                            }
                            else
                            {
                                colStr = tableAlias + memberValueExp.Member.Name + " " + sqlPack.ColumnAsAliasString + " " + memberAliasExp.Name;

                            }
                        }

                        sqlPack.SelectFields.Add(colStr);


                    }
                }
                else
                {
                    sqlPack.CurrentColAlias = memberAliasExp.Name;
                    FluentExpressionSQLProvider.Select(item, sqlPack);
                    sqlPack.CurrentColAlias = null;
                }
                i++;
            }
            return sqlPack;
        }


        protected override SqlPack GroupBy(NewExpression expression, SqlPack sqlPack)
        {
            foreach (Expression item in expression.Arguments)
            {
                FluentExpressionSQLProvider.GroupBy(item, sqlPack);
            }
            return sqlPack;
        }

        protected override SqlPack OrderBy(NewExpression expression, SqlPack sqlPack)
        {
            foreach (Expression item in expression.Arguments)
            {
                FluentExpressionSQLProvider.OrderBy(item, sqlPack);
            }
            return sqlPack;
        }
    }
}
