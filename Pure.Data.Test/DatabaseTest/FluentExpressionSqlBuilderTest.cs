using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.Test
{
 
    public class FluentExpressionSqlBuilderTest
    {
        
        public static void Test()
        {


            string title = "FluentExpressionSqlBuilderTest";
            Console.Title = title;
            var db = DbMocker.InstanceDataBase();
            db.Config.EnableOrmLog = false;
            db.Config.EnableDebug = true;
            db.Config.Interceptors.Add(new ConnectionTestIntercept());
            
            db.LogHelper.Write("Add ConnectionTestIntercept");
            CodeTimer.Time(title, 1, () => {

                //CreateSql(db);
                //Update(db);
                //FetchWhereExpression(db);
                Query();
            });


            Console.Read();
            
            
        }
        static DateTime startTime = DateTime.Now;
        public static void CreateSql(IDatabase db)
        {
            db.EnsureAddClassToTableMap<UserInfo>();
            //string sql = db.FluentExpressionSqlBuilder.Update<UserInfo>(() => new UserInfo { Age = 88, Name = "李玮峰" })
            //    .Where(p => p.Id == 5).ToSqlString();

            string sql = db.FluentSqlBuilder.Select<UserInfo>(p => new
            {
                AddYearsVAR = startTime.AddYears(1),//DATEADD(YEAR,1,@P_0)
                AddYears = p.DTCreate.AddYears(1),//DATEADD(YEAR,1,@P_0)
                AddMonths = startTime.AddMonths(1),//DATEADD(MONTH,1,@P_0)
                AddDays = startTime.AddDays(1),//DATEADD(DAY,1,@P_0)
                AddHours = startTime.AddHours(1),//DATEADD(HOUR,1,@P_0)
                AddMinutes = startTime.AddMinutes(2),//DATEADD(MINUTE,2,@P_0)
                AddSeconds = startTime.AddSeconds(120),//DATEADD(SECOND,120,@P_0)
                //AddMilliseconds = startTime.AddMilliseconds(20000),//DATEADD(MILLISECOND,20000,@P_0)
            }).Where(p => p.DTCreate.AddYears(1) > DateTime.Now).ToSqlString();
            //db.LogHelper.Write(sql);
        }

        public static void Update(IDatabase db)
        {
            db.EnsureAddClassToTableMap<UserInfo>();
            string sql = db.FluentSqlBuilder.Update<UserInfo>(() => new UserInfo { Age = 88, Name = "李玮峰" })
                .Where(p => p.Id == 5).ToSqlString();
            //db.Update<UserInfo>(() => new UserInfo { Age = 88, Name = "李玮峰" }, p => p.Id == 5);
            db.Execute(sql);
        }

        public static void FetchWhereExpression(IDatabase db)
        {
           // var users = db.Query<UserInfo>(p => p.Age > 50 && p.Name.StartsWith("李"), (f) => f.Select(p => new { p.Name, p.Id}).OrderBy(p => p.Id).ThenBy(p => p.Name));
            var reader = db.FluentSqlBuilder.Select<UserInfo>(p => new { p.Id, p.Name }).Where(p => p.Age > 50).OrderBy(p => p.Age).TakePage(1, 5).ExecuteReader();
           //var l = reader.ToList<UserInfo>();
            //var dict = reader.ToDictionary<int,string>();
            
        }

        public static void Query()
        {
            bool hasDel = false;
            DateTime now = DateTime.Now;
            string testTrimStr = " 李佳佳 ";
            DateTime startTime = DateTime.Now;
            DateTime endTime = new DateTime(2016, 1, 1, 0, 0, 0);
            Guid guid = Guid.NewGuid();
            int intValue = -32;
            double decimalValue = 20.8251;

            using (IDatabase db = DbMocker.NewDataBase())
            {




                db.Config.EnableOrmLog = true;
                db.Config.EnableDebug = true;
                db.Config.Interceptors.Add(new ConnectionTestIntercept());
                var ExpressionSqlBuilder = db.FluentSqlBuilder;
                string keyword = "123";

                var lamb1 = ExpressionSqlBuilder.NewBuilder<UserInfo>()
                       .AndIf(!string.IsNullOrEmpty(keyword), p => p.Email.Contains(keyword) || p.Name.Contains(keyword));

                var total = Convert.ToInt32(ExpressionSqlBuilder.Count<UserInfo>().Append(lamb1).ExecuteScalar());

               var data = ExpressionSqlBuilder.Select<UserInfo>().Append(lamb1).OrderBy(p => p.Id).TakePage(1, 10).ExecuteList();


                //string sql = db.FluentExpressionSqlBuilder.Select<UserInfo>(p => new { p.Id, p.Name }).Where(p => p.Age > 50 && p.Id > 1).ToSqlString();

                ////"Enum枚举"
                //string sql = db.FluentExpressionSqlBuilder.Select<UserInfo>(p => new
                //       {
                //           p.Role
                //       }).Where(p => p.Role == RoleType.普通用户).OrderBy(p=>p.Role).EndOrder().ToSqlString();

                //"聚合函数"
                //string sql = db.FluentExpressionSqlBuilder.Select<UserInfo>(p => new
                //      {
                //          intValue,
                //          CountValue = p.Age.CountSQL(),
                //          SumValue = p.Age.SumSQL(),
                //          MaxValue = p.Age.MaxSQL(),
                //          MinValue = p.Age.MinSQL(),
                //          AvgValue = p.Age.AvgSQL(),
                //      }).ToSqlString();

                 //"Case When方法"
                //string sql = ExpressionSqlBuilder.Select<UserInfo>(p => new
                //      {
                //          //CaseWhen = p.Age == null ? false : (p.Age > 20 ? true : false),
                //          //CaseWhen1 = p.Age == null ? (p.Age > 20 ? true : false) :  false,
                //          年龄 = p.Age,
                //          年龄段 = p.Age < 15 ?
                //          "少年" :
                //          ((p.Age > 60) ?
                //          "老年" :
                //          (p.Age > 40) ? "中年" :
                //          "青年")
                //      }).ToSqlString();

                //string sql = ExpressionSqlBuilder.Select<UserInfo>(p => new
                //       {
                //           Abs = Math.Abs(p.Id),
                //           AbsVAR = Math.Abs(intValue),
                //           Round = Math.Round(decimalValue, 2),
                //           RoundNoPrecision = Math.Round(decimalValue),
                //           Ceiling = Math.Ceiling(decimalValue),
                //           Floor = Math.Floor(decimalValue),

                //           Sqrt = Math.Sqrt(decimalValue),
                //           Log = Math.Log(decimalValue, 23),
                //           Pow = Math.Pow(decimalValue, 2),
                //           Sign = Math.Sign(decimalValue),
                //           //Truncate = Math.Truncate(decimalValue),

                //           //ModV = ExDbFunction.Mod(decimalValue, 6),
                //           Rand = ExDbFunction.Rand(),
                //           IfNullV = ExDbFunction.IfNull(p.Name, "李梅"),


                //       }).ToSqlString();

                //"Add DateTime函数"
                //string sql = ExpressionSqlBuilder.Select<UserInfo>(p => new
                //       {
                //           AddYearsVAR = startTime.AddYears(1),//DATEADD(YEAR,1,@P_0)
                //           AddYears = p.DTCreate.AddYears(1),//DATEADD(YEAR,1,@P_0)
                //           AddMonths = startTime.AddMonths(1),//DATEADD(MONTH,1,@P_0)
                //           AddDays = startTime.AddDays(1),//DATEADD(DAY,1,@P_0)
                //           AddHours = startTime.AddHours(1),//DATEADD(HOUR,1,@P_0)
                //           AddMinutes = startTime.AddMinutes(2),//DATEADD(MINUTE,2,@P_0)
                //           AddSeconds = startTime.AddSeconds(120),//DATEADD(SECOND,120,@P_0)
                //           //AddMilliseconds = startTime.AddMilliseconds(20000),//DATEADD(MILLISECOND,20000,@P_0)
                //       }).Where(p => p.DTCreate.AddYears(1) > DateTime.Now).ToSqlString();

                //"Diff DateTime函数"
               // string sql = ExpressionSqlBuilder.Select<UserInfo>(p => new
               //{
               //    DiffYearsVAR = endTime.DiffYears(now),
               //    DiffYears = p.DTCreate.DiffYears(now),
               //    DiffMonths = endTime.DiffMonths(now),
               //    DiffDays = endTime.DiffDays(now),
               //    DiffHours = endTime.DiffHours(now),
               //    DiffMinutes = endTime.DiffMinutes(now),
               //    DiffSeconds = endTime.DiffSeconds(now),
               //    //DiffMilliseconds = endTime.DiffMilliseconds(now),//MAYBE FAIL : OUT BOUND OF INT RANGE
               //    //DiffMicroseconds = endTime.DiffMicroseconds(now),//MAYBE FAIL : OUT BOUND OF INT RANGE
               //}).Where(p => p.DTCreate.DiffDays(now) > 2).ToSqlString();//两天前过滤


                //  "长度Length函数"
                       string sql =ExpressionSqlBuilder.Select<UserInfo>(p => new
                       {
                           长度 = p.Name.Length,
                           大写 = p.Email.ToUpper(),
                           小写 = p.Email.ToLower(),
                           TrimStart = " 李佳佳 ".TrimStart(),
                           TrimEnd = testTrimStr.TrimEnd(),
                           Trim = testTrimStr.Trim(),
                           ToString = testTrimStr.ToString(),
                           Substring = testTrimStr.Substring(1, 2),
                           SubstringWithAutoLength = p.Email.Substring(1),
                           IsNullOrEmpty = string.IsNullOrEmpty(p.Email) ? "空" : "非空"
                       }).Where(p =>
                           //p.Email.Substring(1,2) == "A"
                           // p.Email.ToUpper() == "AAA"// &&
                           // p.Name.StartsWith("ss")
                           string.IsNullOrWhiteSpace(p.Email)


                       ).ToSqlString();
                     

                var list = db.QueryBySQL<UserInfo>(sql);

                db.LogHelper.Debug(list.Count().ToString());
            }
        }
    }
}
