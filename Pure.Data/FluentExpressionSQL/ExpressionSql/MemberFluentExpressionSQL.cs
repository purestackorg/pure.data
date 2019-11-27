
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FluentExpressionSQL
{
    class MemberFluentExpressionSQL : BaseFluentExpressionSQL<MemberExpression>
    {
        private static MemberFluentExpressionSQL _Instance = null;

        public static MemberFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new MemberFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        private string GetTableNameByExpression(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = expression.GetTableNameByExpression(sqlPack);// sqlPack.GetTableName(expression.Member.DeclaringType.UnderlyingSystemType);
            return tableName;
        }
        protected override SqlPack Select(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            sqlPack.SelectFields.Add(tableAlias + expression.Member.Name);
            return sqlPack;
        }

        protected override SqlPack Join(MemberExpression expression, SqlPack sqlPack)
        {
            if (expression.Expression !=null && expression.Expression is ConstantExpression)
            {
                 object value = expression.GetValueOfExpression(sqlPack, false);//expression.Value
                sqlPack.AddDbParameter(value);
                return sqlPack;
            }

            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            sqlPack += " " + tableAlias + expression.Member.Name;

            return sqlPack;
        }

        protected override SqlPack Where(MemberExpression expression, SqlPack sqlPack)
        {
            //datetime.now
            if (expression.Member.DeclaringType == ResolveConstants.TypeOfDateTime)//日期转换
            {
                var value = expression.GetValueOfExpression(sqlPack, false);
                object valueDatetime = sqlPack.SqlDialectProvider.ConvertDateTime(expression.Member, value);
                sqlPack.AddDbParameter(valueDatetime);
                return sqlPack;
            }
            else if (expression.Member.Name == "Length" && expression.Member.DeclaringType == ResolveConstants.TypeOfString)//长度转换
            {
                MemberExpression memberEx =  expression.Expression as MemberExpression;
                string tableName = GetTableNameByExpression(memberEx, sqlPack);
                sqlPack.SetTableAlias(tableName);
                string tableAlias = sqlPack.GetTableAlias(tableName);
                if (!string.IsNullOrWhiteSpace(tableAlias))
                {
                    tableAlias += ".";
                }
                string param = tableAlias + memberEx.Member.Name;

                sqlPack += " " + sqlPack.SqlDialectProvider.ConvertDbFunction(DbFunctionType.Length, param);
                //sqlPack += ("LEN(");
                //Where(((MemberExpression)expression.Expression), sqlPack);
                //sqlPack += (")");

                return sqlPack;
            }
            else if (expression.Member.MemberType == MemberTypes.Property || expression.Member.MemberType == MemberTypes.Field)
            {
                object val = null;
                if (expression.TryGetValueOfMemberExpressionWithFormat(sqlPack, out val, false))//如果能获取值就能直接引用变量的值,如 where(p=>p.Id > info.Id)
                {
                    val = val.SqlVerifyFragment2();

                    sqlPack.AddDbParameter(val);

                    return sqlPack;

                }
                else
                {
                    string tableName = GetTableNameByExpression(expression, sqlPack);
                    sqlPack.SetTableAlias(tableName);//expression.Member.DeclaringType.Name
                    string tableAlias = sqlPack.GetTableAlias(tableName);//expression.Member.DeclaringType.Name
                    if (!string.IsNullOrWhiteSpace(tableAlias))
                    {
                        tableAlias += ".";
                    }
                    sqlPack += " " + tableAlias + expression.Member.Name;

                    //sqlPack.HasParamName = true;
                    return sqlPack;
                }

            }
            
            sqlPack.AddDbParameter(expression.GetValueOfExpression(sqlPack, false));
            return sqlPack;

        }


        protected override SqlPack In(MemberExpression expression, SqlPack sqlPack)
        {
            var field = expression.Member as FieldInfo;
            if (field != null)
            {
                object val = field.GetValue(((ConstantExpression)expression.Expression).Value);

                if (val != null)
                {
                    //sqlPack += "(";
                    //sqlPack.AddDbParameter(val);
                    //sqlPack += ")";

                   

                    string itemJoinStr = "";
                    IEnumerable array = val as IEnumerable;

                    var sb = new StringBuilder();
                     
                    foreach (var item in array)
                    {

                        //sb.AppendFormat(",{0}", sqlPack.SqlDialectProvider.FormatValue(item, true));
                        object o = item.SqlVerifyFragment2();
                        sb.AppendFormat(",{0}", sqlPack.SqlDialectProvider.FormatValue(o, true));
                          
                        ////if (field.FieldType.Name == "String[]")
                        //if (item.GetType() == typeof(String) || item.GetType() == typeof(string))
                        //{
                        //    sqlPack.SqlDialectProvider.FormatValue(item, true);
                        //    itemJoinStr += string.Format(",'{0}'", item);
                        //}
                        //else
                        //{
                        //    itemJoinStr += string.Format(",{0}", item);
                        //}
                    }
                    itemJoinStr = sb.ToString();
                    if (itemJoinStr.Length > 0)
                    {
                        itemJoinStr = itemJoinStr.Remove(0, 1);
                        itemJoinStr = string.Format("({0})", itemJoinStr);
                        sqlPack += itemJoinStr;
                    }
                    else
                    {
                        itemJoinStr = string.Format("()", "");
                        sqlPack += itemJoinStr;
                    }
                }
            }

            return sqlPack;
        }

        protected override SqlPack GroupBy(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            sqlPack += sqlPack.GetTableAlias(tableName) + "." + expression.Member.Name;
            return sqlPack;
        }

        protected override SqlPack OrderBy(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            sqlPack += sqlPack.GetTableAlias(tableName) + "." + expression.Member.Name;
            return sqlPack;
        }

        protected override SqlPack Max(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);

            string columnName = expression.Member.Name;
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + expression.Member.Name;
            }


            sqlPack.Sql.AppendFormat("SELECT MAX({0}) FROM {1}", columnName, tableName);
            return sqlPack;
        }

        protected override SqlPack Min(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);

            string columnName = expression.Member.Name;
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + expression.Member.Name;
            }


            sqlPack.Sql.AppendFormat("SELECT MIN({0}) FROM {1}", columnName, tableName);
            return sqlPack;
        }

        protected override SqlPack Avg(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);

            string columnName = expression.Member.Name;
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + expression.Member.Name;
            }


            sqlPack.Sql.AppendFormat("SELECT AVG({0}) FROM {1}", columnName, tableName);
            return sqlPack;
        }

        protected override SqlPack Count(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);

             
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);

            string columnName = expression.Member.Name;
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + expression.Member.Name;
            }



            sqlPack.Sql.AppendFormat("SELECT COUNT({0}) FROM {1}", columnName, tableName);
            return sqlPack;
        }

        protected override SqlPack Sum(MemberExpression expression, SqlPack sqlPack)
        {
            string tableName = GetTableNameByExpression(expression, sqlPack);
            sqlPack.SetTableAlias(tableName);
            string tableAlias = sqlPack.GetTableAlias(tableName);

            string columnName = expression.Member.Name;
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + expression.Member.Name;
            }

            sqlPack.Sql.AppendFormat("SELECT SUM({0}) FROM {1}", columnName, tableName);
            return sqlPack;
        }
    }
}