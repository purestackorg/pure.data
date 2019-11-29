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
    public class TestExpression2SQL
    {
        public static void Test()
        {


            string title = "Expression2SqlTest";
            Console.Title = title;


            CodeTimer.Time(title, 1, () =>
            {

                TestGen();
            });


            Console.Read();


        }

        private static int CalcAge(int i, int ad, int e)
        {
            return i + ad +e;
        }

        private static string CalcName(int i, int ad, int e)
        {
            return "姓名"+(i + ad + e);
        }
        public static void TestGen()
        {
            Console.Title = "Expression2SqlTest";
            //var query =
            //    new QueryBuilder<UserInfo>()
            //    .Join<Account>(x=>x.Id,x=>x.UserId)
            //    .Where(
            //        x =>
            //              x.Id != 999 ||                          // where Id is not null
            //      x.Name == "速度"//&& x.Id == 15
            //       && x.Id > 18 
            //            //x.DTCreate > from && x.DTCreate < to &&
            //            //(x.Name == "Test" || x.HasDelete)

            //            )
            //        //.GroupBy(x => x.Age, x => x.Name)
            //        //.OrderBy(x => x.Name)
            //        //.Sum(x => x.Age)
            //        .Select();

            //Console.WriteLine(query);


            //通过静态属性DatabaseType或者静态方法Init均可配置数据库类型
            //ExpressionSqlBuilder.DatabaseType = ExpDbType.SQLServer;
            ExpressionSqlBuilder.DatabaseType = ExpDbType.MySQL;
            //ExpressionSqlBuilder.DatabaseType = ExpDbType.Oracle;
            //ExpressionSqlBuilder.DatabaseType = ExpDbType.SQLite;
            //ExpressionSqlBuilder.DatabaseType = ExpDbType.PostgreSQL;
            //ExpressionSqlBuilder.Init(DatabaseType.SQLServer);

            #region Ext

            string dbName = "TestSqlServer";
            TableMapperContainer tbContainer = new TableMapperContainer(dbName);
            tbContainer.Add<UserInfo>(new TableMap<UserInfo>("TB_USER"));
            //tbContainer.Add<Account>(new TableMap<Account>("TB_COMPANY"));
            ExpressionSqlBuilder.TableMapperContainer = tbContainer;
            bool hasDel = false;
            DateTime now = DateTime.Now;
            string testTrimStr = " 李佳佳 ";
            DateTime startTime = new DateTime(2015, 3, 2, 6, 5, 12);
            DateTime endTime = new DateTime(2016, 1, 1, 0, 0, 0);
            Guid guid = Guid.NewGuid();
            int intValue = -32;
            double decimalValue = 20.8251;

            UserInfo info = new UserInfo();
            info.Id = 5;
            info.Name ="莱卡布";


            var testID = info.Id;

            FluentExpressionSqlBuilder b = new FluentExpressionSqlBuilder(ExpDbType.MySQL);
            //DbMocker.NewDataBase().FluentSqlBuilder.Select<UserInfo>((p) => new { p })
            //  .Where(p => p.Id > 30)
            //  .OrderBy(p => p.DTCreate)
            //  .TakePage(1, 10)
            //  .ExecuteList();


            //return;
            var idstr = "44";
            var testUpstr = "SFsdTgsH,";
            var expresion = PredicateExpressions.True<UserInfo>();
            expresion = expresion.And(p => p.HasDelete == true && true && p.Name.ToLower() == info.Name.ToLower() && true);
            // expresion = expresion.And(p =>  p.Id == info.Id && p.Name.ToLower() == info.Name.ToLower());

            Printf(
             ExpressionSqlBuilder.Select<UserInfo>(u => u.Name).
                       Where(u => SqlFuncs.In(u.Name, new string[] { "a", "b" })),
             "查询单表，带where in条件，写法三"
         );

            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(p => p.HasDelete == true),
            "bool测试"
       );

            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(p =>   p.Name  == "ggg )"),
            "SQL注入检查"
       );
            List<string> listNames = new List<string>() { "'li ')", "huang", "gao"};

            Printf(
         ExpressionSqlBuilder.Select<UserInfo>().Where(p =>   listNames.Contains(p.Name)),
         "数组in测试"
    );



            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(expresion),
            "true传入测试1=1"
       );

             expresion = PredicateExpressions.False<UserInfo>();
            expresion = expresion.Or(p => p.HasDelete == true ||  p.Name.ToLower() == info.Name.ToLower());
            // expresion = expresion.And(p =>  p.Id == info.Id && p.Name.ToLower() == info.Name.ToLower());
            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(expresion),
            "true传入测试1=0"
       );

             expresion = PredicateExpressions.True<UserInfo>();
            expresion = expresion.And(p => p.HasDelete == true || true && p.Name.ToLower() == info.Name.ToLower() && true);
            // expresion = expresion.And(p =>  p.Id == info.Id && p.Name.ToLower() == info.Name.ToLower());
            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(expresion),
            "混合true传入测试1=1"
       );

            

             
            Printf(
           ExpressionSqlBuilder.Select<UserInfo>().Where(p => p.Name.ToLower().ToUpper() == testUpstr.ToLower().ToUpper().TrimEnd(',') ),
          // ExpressionSqlBuilder.Select<UserInfo>().Where(p => p.Name.ToUpper().ToLower() == testUpstr.ToLower().ToUpper().TrimEnd(',') && p.Name.ToUpper() == info.Name.Trim().ToUpper()),
           "Trim().Lower().Trim()"
      );
            //return;

           
            
        

            Printf(
             ExpressionSqlBuilder.Select<UserInfo>().Where(p => p.Id == info.Id && p.Id == testID && p.Name.ToLower() == info.Name.ToLower()),
             "传入实体自动校验"
        );

            Printf(
             ExpressionSqlBuilder.Select<UserInfo>().Where(p => p.Id == info.Id && p.Name == testTrimStr && p.Name.ToLower() == info.Name.ToLower()),
             "传入实体自动校验"
        );

            

//            Printf(
//    ExpressionSqlBuilder.Select<UserInfo>(u => new { name = CalcAge(5, 9, 5), age = idstr.To<int>() ,  dssdf = CalcName(5, 9, 5)}).Where(u => u.Id == RoleType.经理.GetHashCode() && u.Age == CalcAge(5, 9, 5) && u.Name == CalcName(5, 9, 5))
//    .OrderBy(u => CalcAge(5, 9, 5)),

//    "select和order获取方法值"
//);

            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(u => u.Id == RoleType.经理.GetHashCode() && u.Age == CalcAge(5, 9, 5) && u.Name == CalcName(5, 9, 5)),

            "直接获取值"
        );
             

            
            var lambda1 = ExpressionSqlBuilder.Select<UserInfo, Account>((p, a) => new { p, a, sdf = p.Id, name4 = a.Name })
        .LeftJoin<Account>((u, a) => u.Id == a.Id && u.Id == 10 && a.StatusCode == 1);
           // var lambdaWhere = ExpressionSqlBuilder.Where<UserInfo>(p => p.Id > 20 && p.Name == "sdf").Exists<Account>((a, p) => a.Id == p.Id && p.Id > 0);
            var lambdaBuilder = ExpressionSqlBuilder.NewBuilder<UserInfo>().Where(p => p.Id > 20 && p.Name == "sdf").Exists<Account>((a, p) => a.Id == p.Id && p.Id > 0);
            var lambdaBuilderAnd = ExpressionSqlBuilder.NewBuilder<UserInfo>().AndIf(false, p =>  p.Name == "555").ExistsIf<Account>(false ,(a, p) => a.Id == p.Id && p.Id > 0);
            var lambdaBuilderOder = ExpressionSqlBuilder.NewBuilder<UserInfo>().OrderBy(p => p.DTCreate);
            var lambdaBuilderWhereMutil = ExpressionSqlBuilder.NewBuilder<UserInfo>().SetTableAlias<EmployeeEntity>("newT").Where<EmployeeEntity>((p, pa1) => p.Id > 0 && p.Id == pa1.Id && pa1.Remark == "aaaa").And<Account>((p, pa1) => pa1.Id > 10 && p.Id == pa1.Id);
            var lambdaBuilderWhereMutil2 = ExpressionSqlBuilder.NewBuilder<UserInfo>().SetTableAlias<EmployeeEntity>("newT").Where<EmployeeEntity>((pa1) =>  pa1.Remark == "aaaa").And<Account>(pa1 => pa1.Id > 10 && pa1.Name=="sdf");
             
            Printf(
             lambda1.Append(lambdaBuilderWhereMutil2).Append(lambdaBuilderAnd).Append(lambdaBuilderOder)
           ,
            "ExpressionSqlBuilder Append"
        );

           


            Printf(
              ExpressionSqlBuilder.Select<UserInfo>((p) => new { p }) 
            ,
             "Exists  A.* 查询"
         );
            Printf(
               ExpressionSqlBuilder.Select<UserInfo,Account>((p,a)=>new {p, a, sdf = p.Id, name4 = a.Name})
        .LeftJoin<Account>((u, a) => u.Id == a.Id && a.StatusCode == 1).Where(p => p.Id > 30)
             ,
              "Exists  查询"
          );




            Printf( 
                ExpressionSqlBuilder.Select<UserInfo>()
          .LeftJoin<Account>((u, a) => u.Id == a.Id && a.StatusCode == 1)
                .Where(p => p.Id > 30 )
              ,
               "InnerJoin  查询"
           );


            Printf(
                          ExpressionSqlBuilder.Select<UserInfo>().Where(a =>
                             // a.Name.StartsWith("李") 
                              //&&
                              //a.Name.EndsWith("明") &&
                              //a.Name.Contains("云") &&
                                  //a.Id.In(1, 2, 3) &&
                                  //a.Name.LikeLeft("明") &&
                                  //a.Name.LikeRight("云") &&
                                  //a.Name.Like("云") &&
                              SqlFuncs.LikeLeft(a.Name, "明") 
                              
                             //&&
                              //SqlFuncs.LikeRight(a.Name, "云") &&
                              //SqlFuncs.Like(a.Name, "云")


                          ),
                          "Like 方法"
                      );



            Printf(
                ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => u.Id)
          .Join<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
                .Where(p => p.Id > 30).
               Union<Account>().
               Select<Account>((u, a) => new { a.Id, a.Name }).Where(p => p.Id > 10),
               "Union 查询"
           );

            Printf(
          ExpressionSqlBuilder.Select<UserInfo, Account>((u,a) => u.Id)
          .Join<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
                .Where(p => p.Id > 30)
                // .OrderBy(p => p.Id)
                //  .TakePage(2, 10)
            ,
          "Select  Join 多条件查询"
      );



            Printf(
         ExpressionSqlBuilder.Count<UserInfo>(u => u.Id)
         .LeftJoin<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
         .Where(p => p.Id > 10)
           .Exists<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
         .OrderBy(p => p.DTCreate).ThenByDescending(p => p.Name)
         .GroupBy(p=>p.Id)
         .Having(u => u.Age > 0 && SqlFuncs.Count() > 10)
         .TakeRange(1,20)
           ,
         "Count Exists 多条件查询2"
     );

            Printf(
           ExpressionSqlBuilder.Count<UserInfo>(u=>u.Id)
           .LeftJoin<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
                //.Where(p => p.Id > 30)
                // .OrderBy(p => p.Id)
                //  .TakePage(2, 10)
             ,
           "Count  多条件查询"
       );

            Printf(
          ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, AID = a.Id, a.Name })
          .LeftJoin<Account>((u, a) => u.Id == a.Id && a.Age > 10 && u.Id > 5)
                //.Where(p => p.Id > 30)
                // .OrderBy(p => p.Id)
                //  .TakePage(2, 10)
            ,
          "Select InnerJoin 多条件查询"
      );

            Printf(
           ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name })
           .LeftJoin<Account>((u, a) => u.Id == a.Id && a.Id >0 || a.Name == dbName && a.Name.StartsWith("a"))
           .Where(p => p.Id > 30)
            .OrderBy(p => p.Id)
             .TakePage(2, 10)
             ,
           "InnerJoin 多条件查询"
       );



            Printf(
        ExpressionSqlBuilder.Insert<UserInfo>(() => new UserInfo { DTCreate = DateTime.Now, Name = "567", Sex = 1, Email = "123456@qq.com" }),
        "Insert带SQL 日期函数"
        );

            List<int> ids = new List<int>() { 1, 2, 3 };
            List<string> names = new List<string>() { "abc", "efg" };
            List<string> nameNots = new List<string>() { "xyz" };
            string ss = "";
            //q.Where(a => ids.Contains(a.Id)).ToList();
         //   Printf(
         //       //ExpressionSqlBuilder.Select<UserInfo>().Where(u => u.Id > 0 && names.Contains(u.Name) && ids.Contains(u.Id)),
         //    ExpressionSqlBuilder.Select<UserInfo>().Where(u => u.Name.Contains(ss)),

         //    "数组contains包含"
         //);
             

        //    Printf(
        //    ExpressionSqlBuilder.Select<UserInfo>(u => new { name = CalcAge(5, 9, 5) , age = idstr.To<int>() }).Where(u => u.Id == RoleType.经理.GetHashCode() && u.Age == CalcAge(5, 9, 5))
        //    .OrderBy(u => CalcAge(5, 9, 5)),

        //    "select和order获取方法值"
        //);
           
            Printf(
            ExpressionSqlBuilder.Select<UserInfo>().Where(u => u.Id == RoleType.经理.GetHashCode() &&  u.Age == CalcAge(5, 9 , 5)),

            "直接获取值"
        );

            Printf(
             ExpressionSqlBuilder.Select<UserInfo>().Where(u => ids.Contains(u.Id) && SqlFuncs.In(u.Name, names) || SqlFuncs.InNot(u.Name, nameNots)),

             "集合相关函数"
         );

            //from cache
            //string sql = b.FromCache( (a) => {
            //    return a.Select<UserInfo>().Where(p => p.Age > 5).ToSqlString();
            //});
            //Print(sql);

            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new { p.Id, p.Name }).Where(p => p.Age > 50 && p.Id > 1),
                       "测试聚合根Enity"
                   );

            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new
                       {
                           p.Role
                       }).Where(p => p.Role == RoleType.普通用户).OrderBy(p => p.Role),
                       "Enum枚举"
                   );

            //Printf(
            //           ExpressionSqlBuilder.Select<UserInfo>(p => new
            //           {
            //               intValue,
            //               CountValue = p.Age.Count(),
            //               SumValue = p.Age.Sum(),
            //               MaxValue = p.Age.Max(),
            //               MinValue = p.Age.Min(),
            //               AvgValue = p.Age.Avg(),
            //           }),
            //           "聚合函数"
            //       );


            Printf(
           ExpressionSqlBuilder.
           Select<UserInfo>().
           Where(u => SqlFuncs.In(u.Id, 1, 2, 3)
                             && SqlFuncs.Like(u.Name, "a")
                             && SqlFuncs.LikeLeft(u.Name, "b")
                             && SqlFuncs.LikeRight(u.Name, "c")
           ),
           "SqlFuncs的列子"
       );


            Printf(
                      ExpressionSqlBuilder.Select<UserInfo>(p => new
                      {
                          intValue,
                          CountValue = SqlFuncs.Count(),
                          SumValue = SqlFuncs.Sum(p.Age),
                          MaxValue = SqlFuncs.Max(p.Age),
                          MinValue = SqlFuncs.Min(p.Age),
                          AvgValue = SqlFuncs.Avg(p.Age),
                      }),
                      "聚合函数"
                  );

            //Printf(
            //           ExpressionSqlBuilder.Select<UserInfo>(p => new
            //           {
            //               NewGuid = Guid.NewGuid(),
            //           }).Where(p => Guid.NewGuid() == guid).OrderBy(p => Guid.NewGuid()).EndOrder(),
            //           "New Guid函数"
            //       );

            Printf(
                      ExpressionSqlBuilder.Select<UserInfo>(p => new
                      {
                          //CaseWhen = p.Age == null ? false : (p.Age > 20 ? true : false),
                          //CaseWhen1 = p.Age == null ? (p.Age > 20 ? true : false) :  false,
                          年龄 = p.Age,
                          年龄段 = p.Age < 15 ?
                          "少年" :
                          ((p.Age > 60) ?
                          "老年" :
                          (p.Age > 40) ? "中年" :
                          "青年")
                      }),
                      "Case When方法"
                  );


            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new
                       {
                           Abs = Math.Abs(p.Id),
                           AbsVAR = Math.Abs(intValue),
                           Round = Math.Round(decimalValue, 2),
                           RoundNoPrecision = Math.Round(decimalValue),
                           Ceiling = Math.Ceiling(decimalValue),
                           Floor = Math.Floor(decimalValue),

                           Sqrt = Math.Sqrt(decimalValue),
                           Log = Math.Log(decimalValue, 23),
                           Pow = Math.Pow(decimalValue, 2),
                           Sign = Math.Sign(decimalValue),
                           //Truncate = Math.Truncate(decimalValue),

                           //ModV = ExDbFunction.Mod(decimalValue, 6),
                           Rand = SqlFuncs.Rand(),
                           IfNullV = SqlFuncs.IfNull(p.Name, "李梅"),


                       }),
                       "数学函数"
                   );


            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new
                       {
                           AddYearsVAR = p.DTCreate.AddYears(1),//DATEADD(YEAR,1,@P_0)
                           AddYears = p.DTCreate.AddYears(1),//DATEADD(YEAR,1,@P_0)
                           AddMonths = p.DTCreate.AddMonths(1),//DATEADD(MONTH,1,@P_0)
                           AddDays = p.DTCreate.AddDays(1),//DATEADD(DAY,1,@P_0)
                           AddHours = p.DTCreate.AddHours(1),//DATEADD(HOUR,1,@P_0)
                           AddMinutes = p.DTCreate.AddMinutes(2),//DATEADD(MINUTE,2,@P_0)
                           AddSeconds = p.DTCreate.AddSeconds(120),//DATEADD(SECOND,120,@P_0)
                           //AddMilliseconds = startTime.AddMilliseconds(20000),//DATEADD(MILLISECOND,20000,@P_0)
                       }).Where(p => p.DTCreate.AddYears(1) > DateTime.Now),
                       "Add DateTime函数"
                   );

            Printf(
               ExpressionSqlBuilder.Select<UserInfo>(p => new
               {
                   DiffYearsVAR = endTime.DiffYears(now),
                   DiffYears = p.DTCreate.DiffYears(now),
                   DiffMonths = endTime.DiffMonths(now),
                   DiffDays = endTime.DiffDays(now),
                   DiffHours = endTime.DiffHours(now),
                   DiffMinutes = endTime.DiffMinutes(now),
                   DiffSeconds = endTime.DiffSeconds(now),
                   //DiffMilliseconds = endTime.DiffMilliseconds(now),//MAYBE FAIL : OUT BOUND OF INT RANGE
                   //DiffMicroseconds = endTime.DiffMicroseconds(now),//MAYBE FAIL : OUT BOUND OF INT RANGE
               }).Where(p => p.DTCreate.DiffDays(now) > 2),//两天前过滤
               "Diff DateTime函数"
           );

            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new
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


                       ),
                       "长度Length函数"
                   );

            Printf(
                       ExpressionSqlBuilder.Select<UserInfo>(p => new
                       {
                           Int_Parse = int.Parse("33"),
                           Byte_Parse = byte.Parse("11"),
                           Short_Parse = short.Parse("22"),
                           Long_Parse = long.Parse("2123123213"),//CAST(N'2' AS BIGINT)
                           Sbyte_Parse = sbyte.Parse("12"),//CAST(N'11' AS SMALLINT)
                           Int16_Parse = Int16.Parse("12211"),//CAST(N'11' AS SMALLINT)
                           Int32_Parse = Int32.Parse("44"),//CAST(N'1' AS INT)
                           Int64_Parse = Int64.Parse("12211"),//CAST(N'11' AS SMALLINT)
                           Double_Parse = double.Parse("3213.2516"),//CAST(N'3' AS FLOAT)
                           DoubleUP_Parse = Double.Parse("3213.2516"),//CAST(N'3' AS FLOAT)
                           Float_Parse = float.Parse("4123.213"),//CAST(N'4' AS REAL)
                           Decimal_Parse = decimal.Parse("25222.234454"),//CAST(N'5' AS DECIMAL) , ONLY SUPPORT 4 PERCION
                           DecimalUP_Parse = Decimal.Parse("5222.234454"),//CAST(N'5' AS DECIMAL)
                           Bool_Parse = bool.Parse("true"),//CAST(N'4' AS REAL)
                           Bool_INT_Parse = bool.Parse("1"),//CAST(N'4' AS REAL)
                           DateTime_Parse = DateTime.Parse("2014-05-21 22:10:05"),//CAST(N'4' AS REAL)
                           Guid_Parse = Guid.Parse("D544BC4C-739E-4CD3-A3D3-7BF803FCE179"),

                       }).Where(p => p.Age > int.Parse("33")),
                       "Parse转换函数"
                   );

            Printf(
                        ExpressionSqlBuilder.Select<UserInfo>().Where(a =>
                            a.Name.StartsWith("李") &&
                            a.Name.EndsWith("明") &&
                            a.Name.Contains("云") &&
                                //a.Id.In(1, 2, 3) &&
                                //a.Name.LikeLeft("明") &&
                                //a.Name.LikeRight("云") &&
                                //a.Name.Like("云") &&
                            SqlFuncs.LikeLeft(a.Name, "明") &&
                            SqlFuncs.LikeRight(a.Name, "云") &&
                            SqlFuncs.Like(a.Name, "云") 


                        ),
                        "Like 方法"
                    );

            Printf(
                        ExpressionSqlBuilder.Select<UserInfo>(a => new
           {
               AddV = 1 + 2,
               SubtractV = 2 - 1,
               MultiplyV = 2 * 11,
               DivideV = 4 / 2,
               AndV = true & false,
               IntAndV = 1 & 2,
               OrV = true | false,
               IntOrV = 3 | 1,
           }),
                        "运算符"
                    );

            Printf(
             ExpressionSqlBuilder.Select<UserInfo>(u => new { u.Age, 现在时间 = now, u.Name, 局部变量 = "23" })
             .OrderBy(p => p.Age).
             ThenByDescending(p => p.Name).
             TakePage(3, 15),
             "分页GetPage"
         );


            Printf(
             ExpressionSqlBuilder.Select<UserInfo>(u => new { u.Age, 现在时间 = now, u.Name, 局部变量 = "23" })
             .OrderBy(p => p.Age).
             ThenByDescending(p => p.Name).
             TakeRange(3, 15),
             "查询范围GetRange"
         );

            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { 现在时间 = now, 局部变量 = "23" }),
              "局部变量选择"
          );




            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { 现在时间 = now, 局部变量 = "23" }).Top(3),
              "Top"
          );
            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { 现在时间 = now, 局部变量 = "23" }).Distinct(),
              "Distinct"
          );

            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new
              {
                  现在时间 = now,
                  属性现在时间 = DateTime.Now,
                  UTC时间 = DateTime.UtcNow,
                  今天 = DateTime.Today,
                  日期部分 = now.Date,
                  年2 = u.DTCreate.Year,
                  年 = now.Year,
                  月 = now.Month,
                  日 = now.Day,
                  时 = now.Hour,
                  分 = now.Minute,
                  秒 = now.Second
                      //,
                      //毫秒 = now.Millisecond
                  ,
                  本周第几天0是周日 = now.DayOfWeek
              }),
              "日期函数操作"
          );


            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { BV = true, CNAME = u.Name, u.Name, CID = u.Id }
              ).
                        Where(u =>
                            //u.Id.In(1, 2, 3)
                            //&&
                            u.HasDelete == false// == hasDel
                            )//.Where(" a.Name ={0}", "李四").Where(" a.Name ={0} and a.ID >0 ", "张茂")
                            ,
              "拼接Where字符串"
          );
            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).
                        Where(u =>
                            SqlFuncs.In(u.Id, 1, 2, 3)
                            &&
                            u.HasDelete == false// == hasDel
                            ).And(" a.Name ={0}", "李四").Or(" a.Name ={0} and a.ID >{1} ", "张茂", 5),
              "拼接And/Or字符串"
          );

            Printf(
              ExpressionSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).OrderBy(p => p.Age).
             ThenByDescending(p => p.Name),
              "正常排序"
          );

            Printf(
               ExpressionSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id })
                             .OrderByString("a.DTCreate desc "),
               "自定义排序字符串"
           );


            string strName = "李四";

            var from = DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;
            Printf(
             ExpressionSqlBuilder.Insert<UserInfo>(() => new UserInfo { Name = strName, Sex = 1, Email = "123456@qq.com" }),
             "Insert UserInfo插入SQL"
         );

            Printf(

              ExpressionSqlBuilder.Insert<UserInfo>(() => new { CNAME = strName, Sex = 1, Email = "123456@qq.com" }),
              "Insert插入SQL"
          );

            Printf(

              ExpressionSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).
                         Where(x =>
                 x.Name == strName                    // or  Address is not null
                 || x.DTCreate > to
                  ),
              "变量传形参"
          );

            string updatename = "Marilyn";
            Printf(
            ExpressionSqlBuilder.Update<UserInfo>(() => new { Name = updatename, Sex = 1, Email = "123456@qq.com" }).Where(p => p.Id == 1),
            "A full table updates"
            );



            int[] aryId = { 1, 2, 3 };
            Printf(

               ExpressionSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).
                          Where(x =>
                               x.Id != 999 &&                          // where Id is not null
                              x.Name == "速度"//&& x.Id == 15
                               && x.Id > 18
                && aryId.Contains(x.Id) &&                  // and Id in (1,2,3)
                 x.Name.StartsWith("张") &&               // and Name like '张%'
                 x.Name.EndsWith("test") &&             // and Remark like '%test'
                 x.Email.Contains("@qq.com") ||           // and Email like '%@qq.com%'
                 x.Email != null                        // or  Address is not null
                   ),
               "查询单表，带where in条件，写法一"
           );


            Printf(
                ExpressionSqlBuilder.Max<UserInfo>(u => u.Name).
                          Where(u => SqlFuncs.In(u.Id, 1, 2, 3)),
                "查询单表，带where in条件，写法2"
            );


            #endregion

            #region 高级查询
         

            Printf(
                ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name }).Exists<Account>((u, a) => u.Id == a.Id, (p) => p.Id > 10).Where(p => p.Id > 30),
                "Exists 查询"
            );

            Printf(
                ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name }).Where(p => p.Id > 30).Exists<Account>((u, a) => u.Id == a.Id, (p) => p.Id > 10),
                "Exists 查询2"
            );
            Printf(
                ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name }).ExistsNot<Account>((u, a) => u.Id == a.Id, (p) => p.Id > 10).Where(p => p.Id > 30),
                "not Exists 查询"
            );
            Printf(
                ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name }).Where(p => p.Name == "云").
                Union<Account>().
                Select<Account>((u, a) => new { a.Id, a.Name }).Where(p => p.Id > 10),
                "Union 查询"
            );
            Printf(
               ExpressionSqlBuilder.Select<UserInfo>((u) => new { u.Id, u.Name }).
               UnionAll<Account>().
               Select<Account>((u, a) => new { a.Id, a.Name }).Where(p => p.Id > 10),
               "Union All 查询"
           );
            #endregion


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(),
                "查询单表所有字段"
            );

            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Id),
                "查询单表单个字段"
            );

            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => new { u.Id, u.Name }),
                "查询单表多个字段"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Id).
                          Where(u => SqlFuncs.Like(u.Name, "b")),
                "查询单表，带where Like条件"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Id).
                          Where(u => SqlFuncs.LikeLeft(u.Name, "b")),
                "查询单表，带where LikeLeft条件"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Id).
                          Where(u => SqlFuncs.LikeRight(u.Name, "b")),
                "查询单表，带where LikeRight条件"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Name).
                          Where(u => SqlFuncs.In(u.Id, 1, 2, 3)),
                "查询单表，带where in条件，写法一"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Name).
                          Where(u => SqlFuncs.In(u.Id, aryId)),
                "查询单表，带where in条件，写法二"
            );

            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Name).
                          Where(u => SqlFuncs.In(u.Name, new string[] { "a", "b" })),
                "查询单表，带where in条件，写法三"
            );


            Printf(
                ExpressionSqlBuilder.Select<UserInfo>(u => u.Id).
                          Where(
                                u => u.Name == "b"
                                  && u.Id > 2
                                  && u.Name != null
                                  && u.Id > int.MinValue
                                  && u.Id < int.MaxValue
                                  && SqlFuncs.In(u.Id, 1, 2, 3)
                                  && SqlFuncs.Like(u.Name, "a")
                                  && SqlFuncs.LikeLeft(u.Name, "b")
                                  && SqlFuncs.LikeRight(u.Name, "c")
                                  || u.Id == null
                                ),
                "查询单表，带多个where条件"
            );


            Printf(
                 ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           Join<Account>((u, a) => u.Id == a.UserId),
                 "多表Join关联查询"
            );


            Printf(
                 ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           InnerJoin<Account>((u, a) => u.Id == a.UserId),
                 "多表InnerJoin关联查询"
            );


            Printf(
                 ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           LeftJoin<Account>((u, a) => u.Id == a.UserId),
                 "多表LeftJoin关联查询"
            );




            Printf(
                 ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           RightJoin<Account>((u, a) => u.Id == a.UserId),
                 "多表RightJoin关联查询"
            );


            Printf(
                 ExpressionSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           FullJoin<Account>((u, a) => u.Id == a.UserId),
                 "多表FullJoin关联查询"
            );


            //Printf(
            //     ExpressionSqlBuilder.Select<UserInfo, Account, Student, Class, City, Country>((u, a, s, d, e, f) =>
            //               new { u.Id, a.Name, StudentName = s.Name, ClassName = d.Name, e.CityName, CountryName = f.Name }).
            //               Join<Account>((u, a) => u.Id == a.UserId).
            //               LeftJoin<Account, Student>((a, s) => a.Id == s.AccountId).
            //               RightJoin<Student, Class>((s, c) => s.Id == c.UserId).
            //               InnerJoin<Class, City>((c, d) => c.CityId == d.Id).
            //               FullJoin<City, Country>((c, d) => c.CountryId == d.Id).
            //               Where(u => u.Id != null),
            //     "多表复杂关联查询"
            //);


            Printf(
                 ExpressionSqlBuilder.Select<UserInfo>(p => new { p.Age, CountV = SqlFuncs.Sum(p.Age) }).
                           GroupBy(u => new { u.Age }).ThenGroupBy(p => p.Id).Having(u => u.Age > 0 && SqlFuncs.Count() > 10),
                 "GroupBy分组查询"
            );

            List<string> lll = new List<string>();
            Printf(
                 ExpressionSqlBuilder.Select<UserInfo>().
                           OrderBy(u => u.Id),
                 "OrderBy排序"
            );




            Printf(
                 ExpressionSqlBuilder.Max<UserInfo>(u => u.Id),
                 "返回一列中的最大值。NULL 值不包括在计算中。"
            );

            Printf(
                 ExpressionSqlBuilder.Min<UserInfo>(u => u.Id).Where(p => p.Id == 5),
                 "返回一列中的最小值。NULL 值不包括在计算中。"
            );

            Printf(
                 ExpressionSqlBuilder.Avg<UserInfo>(u => u.Id),
                 "返回数值列的平均值。NULL 值不包括在计算中。"
            );

            Printf(
                 ExpressionSqlBuilder.Count<UserInfo>(),
                 "返回表中的记录数"
            );

            Printf(
                 ExpressionSqlBuilder.Count<UserInfo>(u => u.Id),
                 "返回指定列的值的数目（NULL 不计入）"
            );

            Printf(
                 ExpressionSqlBuilder.Sum<UserInfo>(u => u.Id),
                 "返回数值列的总数（总额）。"
            );


            Printf(
                 ExpressionSqlBuilder.Delete<UserInfo>(),
                 "全表删除"
            );

            Printf(
                 ExpressionSqlBuilder.Delete<UserInfo>().
                           Where(u => u.Id == null),
                 "根据where条件删除指定表记录"
            );
            Printf(
                 ExpressionSqlBuilder.Delete<UserInfo>().
                           Where(u => u.Id != null),
                 "根据where条件删除指定表记录"
            );

            Printf(
                 ExpressionSqlBuilder.Update<UserInfo>(() => new UserInfo { Name = "", Sex = 1, Email = "123456@qq.com" }),
                 "UserInfo全表更新"
            );

            Printf(
                ExpressionSqlBuilder.Update<UserInfo>(() => new { Name = "", Sex = 1, Email = "123456@qq.com" }),
                "全表更新"
           );

            Printf(
                 ExpressionSqlBuilder.Update<UserInfo>(() => new { Name = "", Sex = 1, Email = "123456@qq.com" }).
                           Where(u => u.Id == 1),
                 "根据where条件更新指定表记录"
            );



            //to be continued...
        }


        private static void Print(string sql)
        {
            bool debug = true;
            if (debug)
            {
                Console.WriteLine(sql);
            }
        }


        public static void Printf<T>(FluentExpressionSQLCore<T> expression2Sql, string description = "")
        {
            // string sql = expression2Sql.ToSqlString();

            string comment = "--";
            if (!string.IsNullOrWhiteSpace(description))
            {
                Print(comment + description);
            }
            if (ExpressionSqlBuilder.ExistSubQuery())
            {
                foreach (KeyValuePair<string, object> item in ExpressionSqlBuilder.GetExistDbParameters())
                {
                    Print(comment + item.ToString() + " --- " + item.Value.GetType());

                }
            }
            //Console.WriteLine(expression2Sql.RawString); 
            Print("-------------------参数----------------------------");
            foreach (KeyValuePair<string, object> item in expression2Sql.DbParams)
            {
                Print(comment + item.ToString() + " --- " + item.Value.GetType());
            }

            Print("-------------------SQL----------------------------");
           
            Print(expression2Sql.RawString);

            Print("-------------------RawSQL----------------------------");
            Print(expression2Sql.ToSqlString());
            Console.WriteLine();
            Console.WriteLine();


        }
    }
}
