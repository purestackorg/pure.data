//using FluentExpressionSQL;
//using FluentExpressionSQL.Mapper;
//using Expression2SqlTest;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Linq.Expressions;

//namespace Pure.Data.Test
//{
//    public class DIContainerAndSqlQueryTest
//    {

//        public static void Test()
//        {


//            string title = "DIContainerAndSqlQueryTest";
//            Console.Title = title;

//            CodeTimer.Time(title, 100, () => {

//                Resolee(); 
//            });


//            Console.Read();
            
            
//        }

//        public static void Resolee()
//        {
//            DIContainer.RegisterType(typeof(SqlServerTestDbContext), null, LifeStyle.Transient);

//            try
//            {
//                //var ex = PredicateBuilder.Where<UserInfo>(p => string.IsNullOrEmpty(p.Email));
//                var ctx =  DIContainer.Resolve<SqlServerTestDbContext>();
//                //var dd = ctx.Users.GetAllList();
//                //FluentExpressionSQLCore


//                //Expression Predicate 表达式预测条件
//                //var ex = PredicateBuilder.Where<UserInfo>();

//                //ex = ex.And(p => p.Email.Contains("b"));
//                //var dEx = ctx.Users.Query(ex).ToList();



//                //延迟加载
//                var data = ctx.Users.Entities;
//                var result0 = data.ToList();

//                var sss = data.Where(p => p.Email == "44");
//                sss = sss.Where(p => p.DTCreate <= DateTime.Now || !p.Name.IsNullOrEmpty());
//                sss = sss.Where(p => p.Id > 0);

//                var sss1 = sss.Where(p => SqlFuncs.In(p.Id,4, 6, 33));

//                var dd2 = ctx.Users.Query(p => p.DTCreate <= DateTime.Now || !p.Name.IsNullOrEmpty());
//                var result2 = dd2.ToArray();

//                var result1 = sss1.ToList();
//                //var d = sss.FirstOrDefault();

//                //order
//                //sss = sss.OrderBy(p => p.DTCreate);

//                var result = sss.ToList();


//            }
//            catch (Exception exx)
//            {
                
//                throw;
//            }
//        }
 

//    }
//}
//public static class PredicateBuilder
//{
   
//    public static Expression<Func<T, bool>> Where<T>(Expression<Func<T, bool>> expr2) { return expr2; }

//    /// <summary>
//    /// 机关函数应用True时：单个AND有效，多个AND有效；单个OR无效，多个OR无效；混应时写在AND后的OR有效  
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <returns></returns>
//    public static Expression<Func<T, bool>> True<T>() { return f => true; }

//    /// <summary>
//    /// 机关函数应用False时：单个AND无效，多个AND无效；单个OR有效，多个OR有效；混应时写在OR后面的AND有效  
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <returns></returns>
//    public static Expression<Func<T, bool>> False<T>() { return f => false; }
     

//    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
//                                                        Expression<Func<T, bool>> expr2)
//    {
//        //var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
//        return Expression.Lambda<Func<T, bool>>
//              (Expression.Or(expr1.Body, expr2.Body), expr1.Parameters);
//    }

//    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
//                                                         Expression<Func<T, bool>> expr2)
//    {
//       // var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
//        return Expression.Lambda<Func<T, bool>>
//              (Expression.And(expr1.Body, expr2.Body), expr1.Parameters);
//    }
//}