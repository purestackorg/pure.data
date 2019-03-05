using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pure.Data.SqlMap;
using Pure.Data.DynamicExpresso;
using System.Dynamic;
using System.Linq.Expressions;

namespace Pure.Data.Test
{
    public class DynamicExpressoTest
    {

        public static void Test()
        {


            string title = "DynamicExpressoTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                DynamicExpressoTestMethodTest().ConfigureAwait(false); 
            });


            Console.Read();
            
            
        }

        public static class Assert {
            public static bool AreEqual(object o , object o1) {
                if (o == null && o1 == null)
                {
                    Console.WriteLine("null等于null" );

                    return true;


                }
                if (o.ToString() != o1.ToString())
                {
                    throw new Exception(o+"不等于"+o1);
                }
                Console.WriteLine(o + "等于" + o1);

                return true;
            }
        }

        private static void PrintLine(string tile) {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine("----------------------------------"+ tile + "--------------------------------------");

        }

        public static async Task DynamicExpressoTestMethodTest()
        {
           

            try
            {
                string code = "\"22\"+Name";
                List<UserInfo> dd = new List<UserInfo>();
                dd.Add(new UserInfo() { Name = "fdd", Id = 32 }); 
                dd.Add(new UserInfo() { Name = "范德萨", Id = 122 });
                var obj = new { Name = "fsdsd", ID = 2, Nam = "23", LikeNames = dd, Ids = new long[] { 1, 2, 3, 4 } };
                var objDict = obj.ToDictionary();
               var result = ExpressoResolver.Instance.Resolve(code, objDict);
                Console.WriteLine(result);
                var target = ExpressoResolver.Instance.Interpreter;

                //类型比较
                PrintLine("类型比较");
                Assert.AreEqual(typeof(string), target.Parse("\"ciao\"").ReturnType);
                Assert.AreEqual(typeof(int), target.Parse("45").ReturnType);
                Assert.AreEqual(typeof(double), target.Parse("45.4").ReturnType);
                Assert.AreEqual(typeof(object), target.Parse("null").ReturnType);
                Assert.AreEqual(new DateTime(2015, 1, 24), target.Eval("new DateTime(2015, 1, 24)"));
                Assert.AreEqual(new string('a', 10), target.Eval("new string('a', 10)"));
                var lambda = target.Parse("null", typeof(int?));
                Assert.AreEqual(typeof(int?), lambda.ReturnType);
                lambda = target.Parse("4651", typeof(int?));
                Assert.AreEqual(typeof(int?), lambda.ReturnType);
                Assert.AreEqual(4651, lambda.Invoke());
                Assert.AreEqual(5.0.ToString(), target.Eval("5.0.ToString()"));
                Assert.AreEqual((5).ToString(), target.Eval("(5).ToString()"));
                Assert.AreEqual((5.0).ToString(), target.Eval("(5.0).ToString()"));
                Assert.AreEqual(5.ToString(), target.Eval("5.ToString()"));
                Assert.AreEqual((-.5).ToString(), target.Eval("-.5.ToString()"));
                Assert.AreEqual((.1).ToString(), target.Eval(".1.ToString()"));
                Assert.AreEqual((-1 - .1 - 0.1).ToString(), target.Eval("(-1-.1-0.1).ToString()"));
                var array = new[] { 5, 10, 6 };
                target.SetVariable("array", array);
                Assert.AreEqual(array.Contains(5), target.Eval("array.Contains(5)"));
                Assert.AreEqual(array.Contains(3), target.Eval("array.Contains(3)"));
                Assert.AreEqual(null, target.Eval("null ?? null"));
                Assert.AreEqual("hallo", target.Eval("\"hallo\" ?? null"));
                Assert.AreEqual("hallo", target.Eval("null ?? \"hallo\""));
                target.SetVariable("a", 1, typeof(int));
                target.SetVariable("b", 1.2, typeof(double?));
                Assert.AreEqual(2.2, target.Eval("a + b"));

                Assert.AreEqual(0, target.Eval("0"));
                Assert.AreEqual(0.0, target.Eval("0.0"));
                Assert.AreEqual(45, target.Eval("45"));
                Assert.AreEqual(-565, target.Eval("-565"));
                Assert.AreEqual(23423423423434, target.Eval("23423423423434"));
                Assert.AreEqual(45.5, target.Eval("45.5"));
                Assert.AreEqual(-0.5, target.Eval("-0.5"));
                Assert.AreEqual(.2, target.Eval(".2"));
                Assert.AreEqual(-.2, target.Eval("-.2"));
                Assert.AreEqual(+.2, target.Eval("+.2"));
                Assert.AreEqual(.02, target.Eval(".02"));
                Assert.AreEqual(-.02, target.Eval("-.02"));
                Assert.AreEqual(+.02, target.Eval("+.02"));
                Assert.AreEqual(.20, target.Eval(".20"));
                Assert.AreEqual(-.20, target.Eval("-.20"));
                Assert.AreEqual(+.20, target.Eval("+.20"));
                Assert.AreEqual(.201, target.Eval(".201"));
                Assert.AreEqual(-.201, target.Eval("-.201"));
                Assert.AreEqual(+.201, target.Eval("+.201"));

                // f suffix (single)
                Assert.AreEqual(4f, target.Eval("4f"));
                Assert.AreEqual(45F, target.Eval("45F"));
                Assert.AreEqual(45.8f, target.Eval("45.8f"));
                Assert.AreEqual(45.8F, target.Eval("45.8F"));
                Assert.AreEqual(45.8F, target.Eval(" 45.8F "));
                Assert.AreEqual(.2f, target.Eval(".2f"));
                Assert.AreEqual(.2F, target.Eval(".2F"));
                Assert.AreEqual(-.2f, target.Eval("-.2f"));
                Assert.AreEqual(-.2F, target.Eval("-.2F"));

                // m suffix (decimal)
                Assert.AreEqual(5M, target.Eval("5M"));
                Assert.AreEqual(254m, target.Eval("254m"));
                Assert.AreEqual(45.232M, target.Eval("45.232M"));
                Assert.AreEqual(45.232m, target.Eval("45.232m"));
                Assert.AreEqual(.022M, target.Eval(".022M"));
                Assert.AreEqual(.022m, target.Eval(".022m"));
                Assert.AreEqual(-.022m, target.Eval("-.022m"));
                Assert.AreEqual(-.022M, target.Eval("-.022M"));

                Assert.AreEqual("ciao".GetType(), target.Eval("\"ciao\".GetType()"));
                Assert.AreEqual('c'.GetType(), target.Eval("'c'.GetType()"));
                Assert.AreEqual(true.GetType(), target.Eval("true.GetType()"));
                Assert.AreEqual(false.GetType(), target.Eval("false.GetType()"));

                Assert.AreEqual(45.GetType(), target.Eval("45.GetType()"));
                Assert.AreEqual(23423423423434.GetType(), target.Eval("23423423423434.GetType()"));
                Assert.AreEqual(45.5.GetType(), target.Eval("45.5.GetType()"));
                Assert.AreEqual(45.8f.GetType(), target.Eval("45.8f.GetType()"));
                Assert.AreEqual(45.232M.GetType(), target.Eval("45.232M.GetType()"));

                // Note: in C# I cannot compile "-565.GetType()" , I need to add parentheses
                Assert.AreEqual((-565).GetType(), target.Eval("-565.GetType()"));
                Assert.AreEqual((-0.5).GetType(), target.Eval("-0.5.GetType()"));

                Assert.AreEqual((-.5).GetType(), target.Eval("-.5.GetType()"));
                Assert.AreEqual((-.5f).GetType(), target.Eval("-.5f.GetType()"));

                Assert.AreEqual((+.5).GetType(), target.Eval("+.5.GetType()"));
                Assert.AreEqual((+.5f).GetType(), target.Eval("+.5f.GetType()"));

                Assert.AreEqual("汉语/漢語", target.Eval("\"汉语/漢語\""));
                Assert.AreEqual('汉', target.Eval("'汉'"));
                Assert.AreEqual('\'', target.Eval("'\\''"));
                Assert.AreEqual('\"', target.Eval("'\\\"'"));
                Assert.AreEqual('\\', target.Eval("'\\\\'"));
                Assert.AreEqual('\0', target.Eval("'\\0'"));
                Assert.AreEqual('\a', target.Eval("'\\a'"));
                Assert.AreEqual('\b', target.Eval("'\\b'"));
                Assert.AreEqual('\f', target.Eval("'\\f'"));
                Assert.AreEqual('\n', target.Eval("'\\n'"));
                Assert.AreEqual('\r', target.Eval("'\\r'"));
                Assert.AreEqual('\t', target.Eval("'\\t'"));
                Assert.AreEqual('\v', target.Eval("'\\v'"));
                Assert.AreEqual("\'", target.Eval("\"\\'\""));
                Assert.AreEqual("\"", target.Eval("\"\\\"\""));
                Assert.AreEqual("\\", target.Eval("\"\\\\\""));
                Assert.AreEqual("\0", target.Eval("\"\\0\""));
                Assert.AreEqual("\a", target.Eval("\"\\a\""));
                Assert.AreEqual("\b", target.Eval("\"\\b\""));
                Assert.AreEqual("\f", target.Eval("\"\\f\""));
                Assert.AreEqual("\n", target.Eval("\"\\n\""));
                Assert.AreEqual("\r", target.Eval("\"\\r\""));
                Assert.AreEqual("\t", target.Eval("\"\\t\""));
                Assert.AreEqual("\v", target.Eval("\"\\v\""));

                Assert.AreEqual("L\'aquila\r\n\tè\tbella.", target.Eval("\"L\\'aquila\\r\\n\\tè\\tbella.\""));

                Assert.AreEqual(typeof(string), target.Parse("\"some string\"").ReturnType);
                Assert.AreEqual(typeof(string), target.Parse("\"\"").ReturnType);
                Assert.AreEqual(typeof(int), target.Parse("234").ReturnType);
                Assert.AreEqual(typeof(double), target.Parse("234.54").ReturnType);
                Assert.AreEqual(typeof(double), target.Parse(".9").ReturnType);
                Assert.AreEqual(typeof(double), target.Parse("-.9").ReturnType);
                Assert.AreEqual(typeof(float), target.Parse("4.5f").ReturnType);
                Assert.AreEqual(typeof(float), target.Parse("4.5F").ReturnType);
                Assert.AreEqual(typeof(float), target.Parse(".5f").ReturnType);
                Assert.AreEqual(typeof(float), target.Parse(".5F").ReturnType);
                Assert.AreEqual(typeof(decimal), target.Parse("234.48m").ReturnType);
                Assert.AreEqual(typeof(decimal), target.Parse("234.48M").ReturnType);
                Assert.AreEqual(typeof(decimal), target.Parse(".48m").ReturnType);
                Assert.AreEqual(typeof(decimal), target.Parse(".48M").ReturnType);
                Assert.AreEqual(typeof(object), target.Parse("null").ReturnType);

                Assert.AreEqual((45.5).GetType(), target.Eval("45.5").GetType());
                Assert.AreEqual((45.8f).GetType(), target.Eval("45.8f").GetType());
                Assert.AreEqual((45.232M).GetType(), target.Eval("45.232M").GetType());

                //方法和属性调用
                PrintLine("方法和属性调用");
                var MyTestService = new MyTestService();

                  target = new Interpreter().SetVariable("MyTestService", MyTestService); //变量模式
                Assert.AreEqual(MyTestService.HelloWorld(), target.Eval("MyTestService.HelloWorld()"));
                Assert.AreEqual(MyTestService.AProperty, target.Eval("MyTestService.AProperty"));
                Assert.AreEqual(MyTestService.AField, target.Eval("MyTestService.AField"));

                target = new Interpreter() ;
                var parametersMethods = new[] { //参数形式
                            new Parameter("MyTestService", MyTestService)
                            };

                Assert.AreEqual(MyTestService.HelloWorld(), target.Eval("MyTestService.HelloWorld()", parametersMethods));
                Assert.AreEqual(MyTestService.HELLOWORLD(), target.Eval("MyTestService.HELLOWORLD()", parametersMethods));
                Assert.AreEqual(MyTestService.AProperty, target.Eval("MyTestService.AProperty", parametersMethods));
                Assert.AreEqual(MyTestService.APROPERTY, target.Eval("MyTestService.APROPERTY", parametersMethods));
                Assert.AreEqual(MyTestService.AField, target.Eval("MyTestService.AField", parametersMethods));
                Assert.AreEqual(MyTestService.AFIELD, target.Eval("MyTestService.AFIELD", parametersMethods));

                var xData = new List<int> { 3, 4, 5, 6 };
                target.SetVariable("xData", xData);
                var yData = new Dictionary<string, int> { { "first", 1 }, { "second", 2 }, { "third", 3 } };
                target.SetVariable("yData", yData);

                Assert.AreEqual(xData[2], target.Eval("xData[2]"));
                Assert.AreEqual(yData["second"], target.Eval("yData[\"second\"]"));

                target.Eval("xData[2] = 1");
                Assert.AreEqual(xData, new List<int> { 3, 4, 1, 6 });
                target.Eval("yData[\"second\"] = 2000");
                Assert.AreEqual(yData, new Dictionary<string, int> { { "first", 1 }, { "second", 2000 }, { "third", 3 } });

                //数学运算
                PrintLine("数学运算");
                result = target.Eval<double>("Math.Pow(x, y) + 5",
                                                    new Parameter("x", typeof(double), 10),
                                                    new Parameter("y", typeof(double), 2));

                Assert.AreEqual(Math.Pow(10, 2) + 5, result);
                Assert.AreEqual(2 * 4, target.Eval("2 * 4"));
                Assert.AreEqual(8 / 2, target.Eval("8 / 2"));
                Assert.AreEqual(7 % 3, target.Eval("7 % 3"));
                Assert.AreEqual(45 + 5, target.Eval("45 + 5"));
                Assert.AreEqual(45 - 5, target.Eval("45 - 5"));
                Assert.AreEqual(1.0 - 0.5, target.Eval("1.0 - 0.5"));
                Assert.AreEqual(-45, target.Eval("-45"));
                Assert.AreEqual(5, target.Eval("+5"));
                Assert.AreEqual(46, target.Eval("     45\t\t  + 1 \r  \n"));
                Assert.AreEqual(null, target.Eval("  \t\t\r\n  \t   "));
                Assert.AreEqual(typeof(void), target.Parse("  \t\t\r\n  \t   ").ReturnType);
                Assert.AreEqual(int.MaxValue, target.Eval("Int32.MaxValue"));
                Assert.AreEqual(double.MaxValue, target.Eval("Double.MaxValue"));
                Assert.AreEqual(DateTime.MaxValue, target.Eval("DateTime.MaxValue"));
                Assert.AreEqual(DateTime.Today, target.Eval("DateTime.Today"));
                Assert.AreEqual(string.Empty, target.Eval("String.Empty"));
                Assert.AreEqual(bool.FalseString, target.Eval("Boolean.FalseString"));
                Assert.AreEqual(TimeSpan.TicksPerMillisecond, target.Eval("TimeSpan.TicksPerMillisecond"));
                Assert.AreEqual(Guid.Empty, target.Eval("Guid.Empty"));

                Assert.AreEqual(TimeSpan.FromMilliseconds(2000.49), target.Eval("TimeSpan.FromMilliseconds(2000.49)"));
                Assert.AreEqual(DateTime.DaysInMonth(2094, 11), target.Eval("DateTime.DaysInMonth(2094, 11)"));
                Assert.AreEqual(Math.Pow(3, 4), target.Eval("Math.Pow(3, 4)"));
                Assert.AreEqual(Math.Sin(30.234), target.Eval("Math.Sin(30.234)"));
                Assert.AreEqual(Convert.ToString(3), target.Eval("Convert.ToString(3)"));
                Assert.AreEqual(Convert.ToInt16("23"), target.Eval("Convert.ToInt16(\"23\")"));

                Assert.AreEqual(int.MaxValue, target.Eval("int.MaxValue"));
                Assert.AreEqual(double.MaxValue, target.Eval("double.MaxValue"));
                Assert.AreEqual(string.Empty, target.Eval("string.Empty"));
                Assert.AreEqual(bool.FalseString, target.Eval("bool.FalseString"));
                Assert.AreEqual(char.MinValue, target.Eval("char.MinValue"));
                Assert.AreEqual(byte.MinValue, target.Eval("byte.MinValue"));

                var xVal = 51.5;
                target.SetVariable("xVal", xVal);
                Assert.AreEqual(false, target.Eval("!true"));
                Assert.AreEqual((int)xVal, target.Eval("(int)xVal"));
                Assert.AreEqual(typeof(int), target.Parse("(int)xVal").ReturnType);
                Assert.AreEqual(typeof(object), target.Parse("(object)xVal").ReturnType);
                Assert.AreEqual((double)84 + 9 * 8, target.Eval("(double)84 + 9 *8"));
                Assert.AreEqual(8 / 2 + 2, target.Eval("8 / 2 + 2"));
                Assert.AreEqual(8 + 2 / 2, target.Eval("8 + 2 / 2"));

                Assert.AreEqual(typeof(string) != typeof(int), target.Eval("typeof(string) != typeof(int)"));
                // ReSharper disable once EqualExpressionComparison
                Assert.AreEqual(typeof(string) == typeof(string), target.Eval("typeof(string) == typeof(string)"));
                Assert.AreEqual("ciao " + 1981, target.Eval("\"ciao \" + 1981"));
                Assert.AreEqual(1981 + "ciao ", target.Eval("1981 + \"ciao \""));
                Assert.AreEqual(false,(bool)target.Eval("0 > 3"));
                Assert.AreEqual(false,(bool)target.Eval("0 >= 3"));
                Assert.AreEqual(true,(bool)target.Eval("3 < 5"));
                Assert.AreEqual(true,(bool)target.Eval("3 <= 5"));
                Assert.AreEqual(false,(bool)target.Eval("5 == 3"));
                Assert.AreEqual(true,(bool)target.Eval("5 == 5"));
                Assert.AreEqual(true,(bool)target.Eval("5 >= 5"));
                Assert.AreEqual(true,(bool)target.Eval("5 <= 5"));
                Assert.AreEqual(true,(bool)target.Eval("5m >= 5m"));
                Assert.AreEqual(true,(bool)target.Eval("5f <= 5f"));
                Assert.AreEqual(true,(bool)target.Eval("5 != 3"));
                Assert.AreEqual(true,(bool)target.Eval("\"dav\" == \"dav\""));
                Assert.AreEqual(false,(bool)target.Eval("\"dav\" == \"jack\""));

                Assert.AreEqual(3 < 5f, target.Eval("3 < 5f"));
                Assert.AreEqual(3f < 5, target.Eval("3f < 5"));
                Assert.AreEqual(43 > 5m, target.Eval("43 > 5m"));
                Assert.AreEqual(34 == 34m, target.Eval("34 == 34m"));
                Assert.AreEqual(true,(bool)target.Eval("0 > 3 || true"));
                Assert.AreEqual(false,(bool)target.Eval("0 > 3 && 4 < 6"));
                Assert.AreEqual(true,(bool)target.Eval("true ^ false"));
                Assert.AreEqual(true,(bool)target.Eval("false ^ true"));
                Assert.AreEqual(false,(bool)target.Eval("true ^ true"));
                Assert.AreEqual(false,(bool)target.Eval("false ^ false"));

                Assert.AreEqual(2, target.Eval("1 ^ 3"));
                Assert.AreEqual(3, target.Eval("1 ^ 2"));
                Assert.AreEqual(false,(bool)target.Eval("true && false"));
                Assert.AreEqual(false,(bool)target.Eval("false && true"));
                Assert.AreEqual(true,(bool)target.Eval("true && true"));
                Assert.AreEqual(false,(bool)target.Eval("false && false"));

                Assert.AreEqual(false,(bool)target.Eval("true & false"));
                Assert.AreEqual(false,(bool)target.Eval("false & true"));
                Assert.AreEqual(true,(bool)target.Eval("true & true"));
                Assert.AreEqual(false,(bool)target.Eval("false & false"));

                Assert.AreEqual(1, target.Eval("1 & 3"));
                Assert.AreEqual(0, target.Eval("1 & 2"));
                Assert.AreEqual(true,(bool)target.Eval("true || false"));
                Assert.AreEqual(true,(bool)target.Eval("false || true"));
                Assert.AreEqual(true,(bool)target.Eval("true || true"));
                Assert.AreEqual(false,(bool)target.Eval("false || false"));
                Assert.AreEqual(true,(bool)target.Eval("true | false"));
                Assert.AreEqual(true,(bool)target.Eval("false | true"));
                Assert.AreEqual(true,(bool)target.Eval("true | true"));
                Assert.AreEqual(false,(bool)target.Eval("false | false"));

                Assert.AreEqual(3, target.Eval("1 | 3"));
                Assert.AreEqual(3, target.Eval("1 | 2"));
                Assert.AreEqual(10 > 3 ? "yes" : "no", target.Eval("10 > 3 ? \"yes\" : \"no\""));
                // ReSharper disable once UnreachableCode
                Assert.AreEqual(10 < 3 ? "yes" : "no", target.Eval("10 < 3 ? \"yes\" : \"no\""));


                //变量操作
                PrintLine("变量操作");
                target = new Interpreter()
               .SetVariable("bool", 1) // this in c# is not permitted
               .SetVariable("Int32", 2)
               .SetVariable("Math", 3)
                .SetVariable("myk", 23);
              

                Assert.AreEqual(23, target.Eval("myk"));
                Assert.AreEqual(typeof(int), target.Parse("myk").ReturnType);
                Assert.AreEqual(1, target.Eval("bool"));
                Assert.AreEqual(2, target.Eval("Int32"));
                Assert.AreEqual(3, target.Eval("Math"));

                Expression<Func<double, double, double>> pow = (x1, y1) => Math.Pow(x1, y1);
                  target = new Interpreter()
                                        .SetExpression("pow", pow);

                Assert.AreEqual(9.0, target.Eval("pow(3, 2)"));

                var workingContext = new { FirstName = "homer" };

                var interpreter2 = new Interpreter();
                interpreter2.SetVariable("this", workingContext); 
                Assert.AreEqual(workingContext.FirstName, interpreter2.Eval("this.FirstName"));

                //静态扩展方法
                PrintLine("静态扩展方法");
                var MyClass = new MyClass();
                var listData = new int[] { 10, 30, 4 };
                target = new Interpreter()
                                    .Reference(typeof(TestExtensionsMethods))
                                 ///   .Reference(typeof(System.Linq.Enumerable))
                                    .SetVariable("MyClass", MyClass)
                                    .SetVariable("listData", listData);

                Assert.AreEqual(MyClass.HelloWorld(), target.Eval("MyClass.HelloWorld()"));
                Assert.AreEqual(MyClass.HelloWorldWithParam(DateTime.Now), target.Eval("MyClass.HelloWorldWithParam(DateTime.Now)"));
                
                Assert.AreEqual(listData.FirstOrDefault(), target.Eval("listData.FirstOrDefault()"));
                //  Assert.AreEqual(listData.Where(p=>p>= 10), target.Eval("listData.Where(\"pa1 > 10\", \"pa1\")"));
                var xLambdaData = new int[] { 10, 30, 4 };
                var xLambdaData2 = new LambdaUserClass[] { new LambdaUserClass { Id = 3, Name = "大草莓" }, new LambdaUserClass { Id =10, Name="张赛"}, new LambdaUserClass { Id = 19, Name = "李丹" } , new LambdaUserClass { Id = 60, Name = "黄博士" } , new LambdaUserClass { Id = 50, Name = "蒋中挺" } };

                target = ExpressoResolver.Instance.Interpreter; //new Interpreter().Reference(typeof(TestExtensionsMethods));
                                       // .SetVariable("xLambdaData", xLambdaData);
                var labParameter = new Parameter("xLambdaData", xLambdaData);
                var lamData = target.Eval("xLambdaData.FirstOrDefault(\"p>=10\")", labParameter);
                Assert.AreEqual(xLambdaData.FirstOrDefault(p=>p>=10), lamData);


                var lamDataWhere = target.Eval("xLambdaData.Where(\"p>=10\")", labParameter);
                Assert.AreEqual(xLambdaData.Where(p => p >= 10), lamDataWhere);

                var xLambdaDataPs2 = new Parameter("xLambdaData2", xLambdaData2);

                var lamDataWhere2 = target.Eval("xLambdaData2.Where(\"p.Id>=10\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Where(p => p.Id >= 10), lamDataWhere2);

                var lamDataWhereAny = target.Eval("xLambdaData2.Any(\"p.Id>=10\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Any(p => p.Id >= 10), lamDataWhereAny);
                // xLambdaData.Select(d => d);
                var lamDataWhereCount = target.Eval("xLambdaData2.Where(\"p.Id>=10\").Count()", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Where(p => p.Id >= 10).Count(), lamDataWhereCount);

                var lamDataWhereMax = target.Eval("xLambdaData2.Max(\"p.Id\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Max(p => p.Id), lamDataWhereMax);
                var lamDataWhereMin = target.Eval("xLambdaData2.Min(\"p.Id\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Min(p => p.Id), lamDataWhereMin);
                var lamDataWhereSum = target.Eval("xLambdaData2.Sum(\"p.Id\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Sum(p => p.Id), lamDataWhereSum);
                var lamDataWhereAverage = target.Eval("xLambdaData2.Average(\"p.Id\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Average(p => p.Id), lamDataWhereAverage);

                var lamDataWhereMax2 = target.Eval("xLambdaData2.Max(\"p.Id\") > 20", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Max(p => p.Id) > 20, lamDataWhereMax2);

                var lamDataWhereMaxpac1 = target.Eval("xLambdaData2.Max(\"pac1.Id\", \"pac1\")  ", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Max(p => p.Id)  , lamDataWhereMaxpac1);


                var lamDataWhereSelect = target.Eval("xLambdaData2.Select(\"p.Id\")", xLambdaDataPs2);
                Assert.AreEqual(xLambdaData2.Select(p => p.Id).ToList(), lamDataWhereSelect);

                //解析和代理
                PrintLine("解析和代理");
                var func = target.ParseAsDelegate<Func<double, double, double>>("Math.Pow(x, y) + 5", "x", "y"); 
                Assert.AreEqual(Math.Pow(10, 2) + 5, func(10, 2)); 
                func = target.ParseAsDelegate<Func<double, double, double>>("Math.Pow(x, y) + .5", "x", "y");
                Assert.AreEqual(Math.Pow(10, 2) + .5, func(10, 2));
                var function1 = target.ParseAsDelegate<Func<string, int>>("arg.Length"); 
                Assert.AreEqual(4, function1("ciao"));
                Assert.AreEqual(9, function1("123456879"));
                var argumentNames = new[] { "x", "y" };
                var function2 = target.ParseAsDelegate<Func<double, double, double>>("x * y", argumentNames); 
                Assert.AreEqual(6, function2(3, 2));
                Assert.AreEqual(50, function2(5, 10));
              


                //Lambda表达式解析
                PrintLine("Lambda表达式解析");
                Expression<Func<double, double>> lambdaExpression = target.ParseAsExpression<Func<double, double>>("arg + 5"); 
                Assert.AreEqual(15, lambdaExpression.Compile()(10)); 
                lambdaExpression = target.ParseAsExpression<Func<double, double>>("arg + .5");
                Assert.AreEqual(10.5, lambdaExpression.Compile()(10));

                var lambdaExpression1 = target.ParseAsExpression<Func<double, double, double>>("arg1 * arg2"); 
                Assert.AreEqual(6, lambdaExpression1.Compile()(3, 2));
                Assert.AreEqual(50, lambdaExpression1.Compile()(5, 10));

                var lambda3 = target.Parse("Math.Pow(x, y) + 5",
                new Parameter("x", typeof(double)),
                new Parameter("y", typeof(double))
            );
                Expression<Func<double, double, double>> lambdaExpression3 = lambda3.LambdaExpression<Func<double, double, double>>(); 
                Assert.AreEqual(Math.Pow(10, 2) + 5, lambdaExpression3.Compile()(10, 2));

                //集合运算
                PrintLine("集合运算");
                target.SetVariable("LinqFunc", new CollectionHelper<int>()); 
                var list = new List<int> { 1, 10, 19, 21 }; 
                var results = target.Eval("LinqFunc.Where(list, \"p > 19\")", new Parameter("list", list))
                                        as IEnumerable<int>; 
                Assert.AreEqual(1, results.Count());
                Assert.AreEqual(21, results.First());

                var x = new int[] { 10, 30, 4 };
                
                //target = new Interpreter()
                //                        .Reference(typeof(System.Linq.Enumerable))
                //                        .SetVariable("x", x);
                target = new Interpreter() ;
                target.SetVariable("LinqFunc", new CollectionHelper<int>());

                var px = new Parameter("x", x);

                Assert.AreEqual(x.Count(), target.Eval("x.Count()", px));
                Assert.AreEqual(x.Average(), target.Eval("x.Average()", px));
                Assert.AreEqual(x.First(), target.Eval("x.First()", px));
                Assert.AreEqual(x.Last(), target.Eval("x.Last()", px));
                Assert.AreEqual(x.Max(), target.Eval("x.Max()", px));
                var ooFirst = target.Eval("x.FirstOrDefault()", px);
                var oo = target.Eval("LinqFunc.Where(x,  \"pa1 > 10\", \"pa1\")", px);

                //CollectionAssert.AreEqual(x.Skip(2).ToArray(), target.Eval<IEnumerable<int>>("x.Skip(2)").ToArray());
                //CollectionAssert.AreEqual(x.Skip(2).ToArray(), target.Eval<IEnumerable<int>>("x.Skip(2)").ToArray());

                //动态变量
                PrintLine("动态变量");
                dynamic dyn = new ExpandoObject();
                dyn.Sub = new ExpandoObject();
                dyn.Sub.Foo = "bar";
                dyn.FooMethod = new Func<string>(() => "bar");

                var interpreter = new Interpreter()
                        .SetVariable("dyn", (object)dyn);

                Assert.AreEqual(dyn.Sub.Foo, interpreter.Eval("dyn.Sub.Foo"));
                Assert.AreEqual(dyn.Foo(), interpreter.Eval("dyn.FooMethod()"));
                 


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                throw;
            }
            finally
            {
                
            }
        

        }

        public class CollectionHelper<T>
        {
            private readonly Interpreter _interpreter;

            public CollectionHelper()
            {
                _interpreter = new Interpreter();
            }

            public IEnumerable<T> Where(IEnumerable<T> values, string expression, string item = "p")
            {
                var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

                return values.Where(predicate);
            }
            public bool Any(IEnumerable<T> values, string expression, string item = "p")
            {
                var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

                return values.Any(predicate);
            }
            public T FirstOrDefault(IEnumerable<T> values, string expression, string item = "p")
            {
                var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

                return values.FirstOrDefault(predicate);
            }
            public T FirstOrDefault(IEnumerable<T> values )
            { 
                return values.FirstOrDefault( );
            }




        }
      


    }


    public class MyClass
    {
    }
    public static class TestExtensionsMethods
    {
        private static readonly Interpreter _interpreter;

        static TestExtensionsMethods()
        {
            _interpreter = new Interpreter();
        }
        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, TResult>>(expression, item);

            return values.Select(predicate);
        }
        public static IEnumerable<T> Where<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Where(predicate);
        }
        public static T FirstOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.FirstOrDefault(predicate);
        }
        public static T First<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.First(predicate);
        }
        public static T LastOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.LastOrDefault(predicate);
        }
        public static T Last<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Last(predicate);
        }
        public static T SingleOrDefault<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.SingleOrDefault(predicate);
        }
        public static T Single<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Single(predicate);
        }
        public static bool Any<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Any(predicate);
        }
        public static bool All<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.All(predicate);
        }
        public static int Count<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.Count(predicate);
        }
        public static long LongCount<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, bool>>(expression, item);

            return values.LongCount(predicate);
        }

        public static double Average<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Average(predicate);
        }
        public static double AverageLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item); 
            return values.Average(predicate);
        }
        public static decimal AverageDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Average(predicate);
        }
        public static float AverageFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Average(predicate);
        }
        public static double AverageDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Average(predicate);
        }

        public static int Sum<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Sum(predicate);
        }
        public static long SumLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Sum(predicate);
        }
        public static decimal SumDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Sum(predicate);
        }
        public static float SumFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Sum(predicate);
        }
        public static double SumDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Sum(predicate);
        }

        public static int Max<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Max(predicate);
        }
        public static long MaxLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Max(predicate);
        }
        public static decimal MaxDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Max(predicate);
        }
        public static float MaxFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Max(predicate);
        }
        public static double MaxDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Max(predicate);
        }

        public static int Min<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, int>>(expression, item);

            return values.Min(predicate);
        }
        public static long MinLong<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, long>>(expression, item);
            return values.Min(predicate);
        }
        public static decimal MinDecimal<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, decimal>>(expression, item);
            return values.Min(predicate);
        }
        public static float MinFloat<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, float>>(expression, item);
            return values.Min(predicate);
        }
        public static double MinDouble<T>(this IEnumerable<T> values, string expression, string item = "p")
        {
            var predicate = _interpreter.ParseAsDelegate<Func<T, double>>(expression, item);
            return values.Min(predicate);
        }

        public static string HelloWorld(this MyClass test)
        {
            return "Hello Test Class";
        }

        public static string HelloWorldWithParam(this MyClass test, DateTime date)
        {
            return "Hello Test Class " + date.Year;
        }

        public static string GenericHello<T>(this T test)
            where T : MyClass
        {
            return "Hello with generic!";
        }

        public static string GenericParamHello<T>(this IEnumerable<T> test)
            where T : MyClass
        {
            return "Hello with generic param!";
        }

        public static string GenericWith2Params<T>(this IEnumerable<T> test, T another)
            where T : MyClass
        {
            return "Hello with 2 generic param!";
        }

        public static string GenericMixedParamHello<T>(this IDictionary<string, T> test)
            where T : MyClass
        {
            return "Hello with generic param!";
        }

        public static T2 GenericWith2Args<T1, T2>(this IDictionary<T1, T2> test)
                where T2 : MyClass
        {
            return test.First().Value;
        }
    }

    public class LambdaUserClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class MyTestService
    {
        public DateTime AField = DateTime.Now;
        public DateTime AFIELD = DateTime.UtcNow;
        public DateTime Today = DateTime.Today;

        public int AProperty
        {
            get { return 769; }
        }

        public int APROPERTY
        {
            get { return 887; }
        }

        public string HelloWorld()
        {
            return "Ciao mondo";
        }

        public string HELLOWORLD()
        {
            return "HELLO";
        }

        public int VoidMethodCalls { get; set; }
        public void VoidMethod()
        {
            System.Diagnostics.Debug.WriteLine("VoidMethod called");
            VoidMethodCalls++;
        }

        public string MethodWithNullableParam(string param1, int? param2)
        {
            return string.Format("{0} {1}", param1, param2);
        }

        public string MethodWithGenericParam<T>(T p)
        {
            return string.Format("{0}", p);
        }

        public string MethodWithGenericParam<T>(string a, T p)
        {
            return string.Format("{0} {1}", a, p);
        }

        public string MethodWithOptionalParam(string param1, string param2 = "2", string param3 = "3")
        {
            return string.Format("{0} {1} {2}", param1, param2, param3);
        }

        public string MethodWithOptionalNullParam(string param1, string param2 = null)
        {
            return string.Format("{0} {1}", param1, param2 ?? "(null)");
        }

        public DateTime this[int i]
        {
            get { return AField.AddDays(i); }
        }

        public DateTime this[long i]
        {
            set { AField = value.AddSeconds(i); }
        }

        public DateTime this[short i]
        {
            get => AField.AddYears(i);
            set => AField = value.AddYears(i);
        }

        public TimeSpan this[DateTime dateTime, int i = 3]
        {
            get => AField.AddDays(-i).Subtract(dateTime);
            set => AField = dateTime.AddDays(i).Add(value);
        }

        public int MethodWithParamsArrayCalls { get; set; }
        public int MethodWithParamsArray(DateTime fixedParam, params int[] paramsArray)
        {
            MethodWithParamsArrayCalls++;

            return paramsArray.Sum();
        }

        public int AmbiguousMethod_ParamsArrayCalls { get; set; }
        public void AmbiguousMethod(DateTime fixedParam, params int[] paramsArray)
        {
            AmbiguousMethod_ParamsArrayCalls++;
        }
        public int AmbiguousMethod_NormalCalls { get; set; }
        public void AmbiguousMethod(DateTime fixedParam, int p1, int p2)
        {
            AmbiguousMethod_NormalCalls++;
        }

        public int OverloadMethodWithParamsArray(params int[] paramsArray)
        {
            return paramsArray.Max();
        }
        public long OverloadMethodWithParamsArray(params long[] paramsArray)
        {
            return paramsArray.Max();
        }
    }

    public class MyTestServiceCaseInsensitive
    {
        public DateTime AField = DateTime.Now;

        public int AProperty
        {
            get { return 769; }
        }

        public string AMethod()
        {
            return "Ciao mondo";
        }
    }

}
