
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentExpressionSQL
{
    class BinaryFluentExpressionSQL : BaseFluentExpressionSQL<BinaryExpression>
    {
        private static BinaryFluentExpressionSQL _Instance = null;

        public static BinaryFluentExpressionSQL Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (OLOCK)
                    {
                        _Instance = new BinaryFluentExpressionSQL();
                    }
                }
                return _Instance;
            }
        }
        private void OperatorParser(ExpressionType expressionNodeType, int operatorIndex, SqlPack sqlPack, bool useIs = false)
        {
            switch (expressionNodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlPack.Sql.Insert(operatorIndex, " AND");
                    break;
                case ExpressionType.Equal:
                    if (useIs)
                    {
                        sqlPack.Sql.Insert(operatorIndex, " IS");
                    }
                    else
                    {
                        sqlPack.Sql.Insert(operatorIndex, " =");
                    }
                    break;
                case ExpressionType.GreaterThan:
                    sqlPack.Sql.Insert(operatorIndex, " >");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sqlPack.Sql.Insert(operatorIndex, " >=");
                    break;
                case ExpressionType.NotEqual:
                    if (useIs)
                    {
                        sqlPack.Sql.Insert(operatorIndex, " IS NOT");
                    }
                    else
                    {
                        sqlPack.Sql.Insert(operatorIndex, " <>");
                    }
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sqlPack.Sql.Insert(operatorIndex, " OR");
                    break;
                case ExpressionType.LessThan:
                    sqlPack.Sql.Insert(operatorIndex, " <");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sqlPack.Sql.Insert(operatorIndex, " <=");
                    break;
                default:
                    throw new NotImplementedException("未实现的节点类型" + expressionNodeType);
            }
        }


        protected override SqlPack Join(BinaryExpression expression, SqlPack sqlPack)
        {
            FluentExpressionSQLProvider.Join(expression.Left, sqlPack);
            int operatorIndex = sqlPack.Sql.Length;

            FluentExpressionSQLProvider.Join(expression.Right, sqlPack);
            int sqlLength = sqlPack.Sql.Length;

            if (sqlLength - operatorIndex == 5 && sqlPack.ToString().EndsWith("null"))
            {
                OperatorParser(expression.NodeType, operatorIndex, sqlPack, true);
            }
            else
            {
                OperatorParser(expression.NodeType, operatorIndex, sqlPack);
            }

            return sqlPack;
        }
        public const string TrueLiteral = "(1=1)";
        public const string FalseLiteral = "(1=0)";
        protected override SqlPack Where(BinaryExpression expression, SqlPack sqlPack)
        {

            int signIndex = 0, sqlLength = 0;
            if (expression.NodeType == ExpressionType.AndAlso)
            {
                // true && a.ID == 1 或者 a.ID == 1 && true
                Expression left = expression.Left, right = expression.Right;
                ConstantExpression c = right as ConstantExpression;
                // DbExpression dbExp = null;
                //a.ID == 1 && true
                if (c != null)
                {
                    if ((bool)c.Value == true)
                    {

                        // (a.ID==1)==true
                        //dbExp = new DbEqualExpression(this.Visit(exp.Left), new DbConstantExpression(true));
                        //dbExp = this.Visit(Expression.Equal(exp.Left, UtilConstants.Constant_True));
                        FluentExpressionSQLProvider.Where(expression.Left, sqlPack);

                        signIndex = sqlPack.Length;

                        sqlPack += " " + TrueLiteral;
                        sqlLength = sqlPack.Length;

                        if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack, true);
                        }
                        else
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack);
                        }

                        return sqlPack;

                    }
                    else
                    {
                        sqlPack += " " + FalseLiteral;
                        return sqlPack;
                        //dbExp = DbEqualExpression.False;
                        // return dbExp;
                    }
                }

                c = left as ConstantExpression;
                // true && a.ID == 1  
                if (c != null)
                {
                    if ((bool)c.Value == true)
                    {
                        // (a.ID==1)==true

                        sqlPack += " " + TrueLiteral;
                        signIndex = sqlPack.Length;

                        FluentExpressionSQLProvider.Where(expression.Right, sqlPack);
                        sqlLength = sqlPack.Length;


                        if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack, true);
                        }
                        else
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack);
                        }

                        return sqlPack;

                        //dbExp = this.Visit(Expression.Equal(exp.Right, UtilConstants.Constant_True));
                        //return dbExp;
                    }
                    else
                    {
                        // 直接 (1=0)

                        sqlPack += " " + FalseLiteral;
                        return sqlPack;
                        //dbExp = DbEqualExpression.False;
                        // return dbExp;
                    }
                }

            }
            else if (expression.NodeType == ExpressionType.OrElse)
            {
                // true && a.ID == 1 或者 a.ID == 1 && true
                Expression left = expression.Left, right = expression.Right;
                ConstantExpression c = right as ConstantExpression;
                //DbExpression dbExp = null;
                //a.ID == 1 || true
                if (c != null)
                {
                    if ((bool)c.Value == false)
                    {
                        // (a.ID==1)==true
                        FluentExpressionSQLProvider.Where(expression.Left, sqlPack);
                        signIndex = sqlPack.Length;

                        sqlPack += " " + FalseLiteral;
                        sqlLength = sqlPack.Length;

                        if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack, true);
                        }
                        else
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack);
                        }

                        return sqlPack;
                        //dbExp = this.Visit(Expression.Equal(exp.Left, UtilConstants.Constant_True));
                        //return dbExp;
                    }
                    else
                    {
                        sqlPack += " " + TrueLiteral;
                        return sqlPack;

                       // dbExp = DbEqualExpression.True;
                       // return dbExp;
                    }
                }

                c = left as ConstantExpression;
                // true || a.ID == 1  
                if (c != null)
                {
                    if ((bool)c.Value == false)
                    {
                        // (a.ID==1)==true
                        sqlPack += " " + FalseLiteral;
                        signIndex = sqlPack.Length;

                        FluentExpressionSQLProvider.Where(expression.Right, sqlPack);
                        sqlLength = sqlPack.Length;
                        
                        if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack, true);
                        }
                        else
                        {
                            OperatorParser(expression.NodeType, signIndex, sqlPack);
                        }

                        return sqlPack;

                        //dbExp = this.Visit(Expression.Equal(exp.Right, UtilConstants.Constant_True));
                        //return dbExp;
                    }
                    else
                    {
                        // 直接 (1=1)
                        sqlPack += " " + TrueLiteral;
                        return sqlPack;
                        //dbExp = DbEqualExpression.True;
                        //return dbExp;
                    }
                }
            }



            if (HasBeginInOwnLogic(expression, sqlPack))
            {
                sqlPack += " (";
            }

            //sqlPack.WhereConditionIndex = sqlPack.WhereConditionIndex + 1;


            FluentExpressionSQLProvider.Where(expression.Left, sqlPack);
            signIndex = sqlPack.Length;

            FluentExpressionSQLProvider.Where(expression.Right, sqlPack);
            sqlLength = sqlPack.Length;

            if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack, true);
            }
            else
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack);
            }

            if (HasEndInOwnLogic(expression, sqlPack))
            {
                sqlPack += ")";
            }
            return sqlPack;
        }

        protected override SqlPack Having(BinaryExpression expression, SqlPack sqlPack)
        {
            if (HasBeginInOwnLogic(expression, sqlPack))
            {
                sqlPack += " (";
            }


            FluentExpressionSQLProvider.Where(expression.Left, sqlPack);
            int signIndex = sqlPack.Length;

            FluentExpressionSQLProvider.Where(expression.Right, sqlPack);
            int sqlLength = sqlPack.Length;

            if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null"))
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack, true);
            }
            else
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack);
            }

            if (HasEndInOwnLogic(expression, sqlPack))
            {
                sqlPack += ")";
            }
            return sqlPack;
        }

        //private string GenSQLInOwnLogic(Expression expression, SqlPack sqlPack)
        //{
        //    string result = "";
        //    if (expression != null && expression is BinaryExpression)
        //    {
        //        var expBinar = expression as BinaryExpression;
        //        if (IsLinkExpression(expBinar))
        //        {
        //            ///left
        //            if (IsLinkExpression(expBinar.Left))
        //            {
        //                if (HasBeginInOwnLogic(expBinar.Left, sqlPack) )
        //                {
        //                    sqlPack += " (";
        //                }
        //                result += GenSQLInOwnLogic(expBinar.Left, sqlPack);
        //            }
        //            else
        //            {
        //                AppendSql(expBinar.Left, sqlPack);
        //            }

        //            if (HasEndInOwnLogic(expBinar.Left, sqlPack))
        //            {
        //                sqlPack += ") ";
        //            }

        //            LinkOperatorParser(expBinar.NodeType, sqlPack);
        //            ///right

        //            if (IsLinkExpression(expBinar.Right))
        //            {
        //                if (HasBeginInOwnLogic(expBinar.Right, sqlPack))
        //                {
        //                    sqlPack += " (";
        //                }
        //                result += GenSQLInOwnLogic(expBinar.Right, sqlPack);
        //            }
        //            else
        //            {

        //                AppendSql(expBinar.Right, sqlPack);
        //            }

        //            if (HasEndInOwnLogic(expBinar.Right, sqlPack))
        //            {
        //                sqlPack += ") ";
        //            }

        //        }

        //    }
        //    return result;
        //}

        private void LinkOperatorParser(ExpressionType expressionNodeType, SqlPack sqlPack)
        {
            switch (expressionNodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    sqlPack += " AND";
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    sqlPack += " OR";
                    break;
                default:
                    throw new NotImplementedException("未实现的节点类型" + expressionNodeType);
            }
        }

        private void AppendSql(Expression exp, SqlPack sqlPack)
        {
            var expression = exp as BinaryExpression;
            FluentExpressionSQLProvider.Where(expression.Left, sqlPack);
            int signIndex = sqlPack.Length;

            FluentExpressionSQLProvider.Where(expression.Right, sqlPack);
            int sqlLength = sqlPack.Length;

            if (sqlLength - signIndex == 5 && sqlPack.ToString().EndsWith("null", StringComparison.InvariantCultureIgnoreCase))
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack, true);
            }
            else
            {
                OperatorParser(expression.NodeType, signIndex, sqlPack);
            }
        }


        private void GetChildrenTreeInOwnLogic(BinaryExpression expression, ref Dictionary<BinaryExpression, List<Expression>> dict)
        {
            // Dictionary<BinaryExpression, List<Expression>> dict = new Dictionary<BinaryExpression, List<Expression>>();
            List<Expression> list = new List<Expression>();
            if (expression != null)
            {
                if (expression.Left is BinaryExpression)
                {
                    var expLeft = expression.Left as BinaryExpression;
                    if (!HasChildrenInOwnLogic(expLeft))
                    {
                        list.Add(expLeft);
                    }
                    if (IsLinkExpression(expLeft))
                    {

                        var listChildren = GetChildrenInOwnLogic(expLeft);
                        dict.Add(expLeft, listChildren);
                        list.AddRange(listChildren);
                    }
                }


                if (expression.Right is BinaryExpression)
                {

                    var exp = expression.Right as BinaryExpression;
                    if (!HasChildrenInOwnLogic(exp))
                    {
                        list.Add(exp);

                    }
                    if (IsLinkExpression(exp))
                    {

                        var listChildren = GetChildrenInOwnLogic(exp);
                        dict.Add(exp, listChildren);
                        list.AddRange(listChildren);
                    }

                }
            }
            dict.Add(expression, list);

            //return list;
        }

        private List<Expression> GetChildrenInOwnLogic(BinaryExpression expression)
        {

            List<Expression> list = new List<Expression>();
            if (expression != null)
            {
                if (expression.Left is BinaryExpression)
                {
                    var expLeft = expression.Left as BinaryExpression;
                    if (!HasChildrenInOwnLogic(expLeft))
                    {
                        list.Add(expLeft);
                    }
                    if (IsLinkExpression(expLeft))
                    {

                        var listChildren = GetChildrenInOwnLogic(expLeft);
                        list.AddRange(listChildren);
                    }
                }


                if (expression.Right is BinaryExpression)
                {

                    var exp = expression.Right as BinaryExpression;
                    if (!HasChildrenInOwnLogic(exp))
                    {
                        list.Add(exp);

                    }
                    if (IsLinkExpression(exp))
                    {

                        var listChildren = GetChildrenInOwnLogic(exp);
                        list.AddRange(listChildren);
                    }

                }
            }
            return list;
        }


        private bool HasChildrenInOwnLogic(BinaryExpression expression)
        {
            bool result = false;
            if (expression != null)
            {
                var exp = expression as BinaryExpression;
                if (IsLinkExpression(exp))
                {
                    result = true;
                }
                //if (expression.Right is BinaryExpression)
                //{
                //    var exp = expression.Right as BinaryExpression;
                //    if (IsLinkExpression(exp))
                //    {
                //        result = true;
                //    }

                //}
            }
            return result;
        }


        //private bool IsLinkExpression(BinaryExpression expression)
        //{
        //    return (expression.NodeType == ExpressionType.Or || expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.And || expression.NodeType == ExpressionType.AndAlso);
        //}
        private bool IsLinkExpression(Expression expression)
        {
            return (expression.NodeType == ExpressionType.Or || expression.NodeType == ExpressionType.OrElse || expression.NodeType == ExpressionType.And || expression.NodeType == ExpressionType.AndAlso);
        }

        private bool HasBeginInOwnLogic(Expression expression, SqlPack sqlPack)
        {
            bool result = false;

            if (expression != null)
            {
                if (IsLinkExpression(expression) && sqlPack.CurrentWhereExpression != expression)
                {
                    sqlPack.HasBeginBrecket = true;
                    sqlPack.ExpressionBreckets.Add(expression, "1");
                    result = true;
                }


                //BinaryExpression expression1 = ((BinaryExpression)expression);
                //BinaryExpression expression1Left = ((BinaryExpression)expression1.Left);
                //BinaryExpression expression1Right = ((BinaryExpression)expression1.Right);

                //var levelLeft = GetChildrenInOwnLogic(expression1Left);
                //var levelRight = GetChildrenInOwnLogic(expression1Right);

                //int maxLevel = Math.Max(levelLeft.Count, levelRight.Count);
                //if (maxLevel >= 3) //同一层深度3层表达式连接: Where(x => x.Id != 999 && (x.Name && "速度" || x.Id == 30 || x.Id == 5 ))
                //{

                //    if (IsLinkExpression(expression) && !sqlPack.HasBeginBrecket &&
                //    (
                //    (!(IsLinkExpression(expression1.Left) || IsLinkExpression(expression1.Right)))
                //    ||
                //    !(IsLinkExpression(expression1Left.Left) || IsLinkExpression(expression1Left.Right) || IsLinkExpression(expression1Right.Left) || IsLinkExpression(expression1Right.Right))
                //    )
                //    ) // expression.Right is BinaryExpression
                //    {
                //        sqlPack.HasBeginBrecket = true;
                //        sqlPack.ExpressionBreckets.Add(expression, " (");
                //        result = true;

                //    }
                //}
                //else // 同一层深度2层表达式连接: Where(x => x.Id != 999 && (x.Name == "速度" || x.Id == 30 ))
                //{
                //    if (IsLinkExpression(expression) && !sqlPack.HasBeginBrecket &&
                //    (
                //    (!(IsLinkExpression(expression1.Left) || IsLinkExpression(expression1.Right)))
                //    )
                //    ) 
                //    {
                //        sqlPack.HasBeginBrecket = true;
                //        sqlPack.ExpressionBreckets.Add(expression, " (");
                //        result = true;

                //    }

                //}


            }
            return result;
        }

        private List<Expression> GetExpressionLevel(Expression expression, bool toLeft = true)
        {
            List<Expression> list = new List<Expression>();
            BinaryExpression expression1 = ((BinaryExpression)expression);
            if (IsLinkExpression(expression1))
            {
                if (toLeft)
                {
                    list.AddRange(GetExpressionLevel(expression1.Left, toLeft));
                }
                else
                {
                    list.AddRange(GetExpressionLevel(expression1.Right, toLeft));
                }
            }
            else
            {
                list.Add(expression1);
            }
            return list;
        }


        private bool HasEndInOwnLogic(Expression expression, SqlPack sqlPack)
        {
            bool result = false;

            if (expression != null)
            {
                if (sqlPack.ExpressionBreckets.ContainsKey(expression)) // expression.Right is BinaryExpression
                {
                    result = true;
                    sqlPack.HasBeginBrecket = false;
                    sqlPack.ExpressionBreckets[expression] = "0";
                    if (expression == sqlPack.CurrentWhereExpression)
                    {
                        sqlPack.CurrentWhereExpression = null;

                    }
                }
            }
            return result;
        }



    }
}