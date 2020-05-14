# pure.data
pure.data支持.Net core 和 .NetFramework4.5+的ORM框架，底层采用dapper，提高执行效率

包含如下功能：
1.支持多种配置方式xml，数据库连接字符串，连接对象

2.支持Linq写法转换并执行SQL

3.支持Mybatis写法，xml配置，可以灵活方便管理复杂SQL

4.支持数据库连接池

5.支持FluetValidation自动验证实体

6.主从读写分离

7.批量导入、导出，生成SQL 脚本

8.数据备份与还原、数据导入导出

9.附加代码生成工具

10.....很多功能，自行挖掘



下面是常用使用方法：
# 数据库上下文

所有数据库上下文都继承自DbContext

### 例子

```c#
public class TestDbContext : DbContext
    {
        public TestDbContext()
            : base("TestKey", config => //TestKey 对应web.config里面ConnectionString的key
            {

                config.ExecuteTimeout = GDCIC.Core.Toolset.AppConfigHelper.Get<int>("ExecuteTimeout", 30);//数据库执行超时时间
                config.EnableOrmLog = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableOrmLog", true);//是否启用日志追踪
                config.EnableDebug = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableDebug", true);//是否启用调试模式，只有true才能输出SQL
                config.EnableIntercept = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableIntercept", true); //是否启用侦听中断，用于嵌入自定义代码
                config.KeepConnectionAlive = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("KeepConnectionAlive", false);  //是否一直保持数据库连接
                config.AutoMigrate = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("AutoMigrate", true);//是否自动迁移数据库脚本，用于CodeFirst模式
                config.EnableAutoMigrateDebug = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableAutoMigrateDebug", true);//是否启用自动迁移数据库的调试模式输出SQL
                config.EnableAutoMigrateLog = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableAutoMigrateLog", true);//是否启用自动迁移数据库的调试模式输出本地LOG
                config.AutoMigrateOnContainTable = GDCIC.Core.Toolset.AppConfigHelper.Get<string>("AutoMigrateOnContainTable", "");//仅包含的指定表名才能自动迁移
                config.EnableSqlMap = GDCIC.Core.Toolset.AppConfigHelper.Get<bool>("EnableSqlMap", false);//是否启用Mybatis数据类似的SqlMap脚本模式
                //config.SqlMapPaths.Add(GDCIC.Core.Toolset.AppConfigHelper.Get<string>("SqlMapPaths", "sqlmap/sql_dev.xml"));//加入SqlMap的文件路径

                if (config.EnableDebug == true)//此代码用于Profiler监听SQL， 可以移除
                {
                    config.DbConnectionInit = (conn) =>
                    {

                        if ( ProfilingSession.Current == null)
                        {
                            return conn;
                        } 
                        var dbProfiler = new Pure.Profiler.Data.DbProfiler(Pure.Profiler.ProfilingSession.Current.Profiler);

                        conn = new Pure.Profiler.Data.ProfiledDbConnection(conn, dbProfiler);
                        return conn;
                    };
                }

            })
        {

        }
```

### 属性

```c#
    public IDatabase Database { get; }  //获取数据库对象
    public string ProviderName { get; } //获取当前数据库驱动器
    public IDbTransaction Transaction { get; } //当前事务
    public string ConnectionString { get; } //连接字符串
```
### 方法

> 获取数据库相关对象

```c#
IDbCommand GetCommand(IDbConnection con, string commmandText, CommandType commandType);
IDbConnection GetConnection();
DbProviderFactory GetFactory();
```

> 常规执行方法

```c#

//Execute
int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

//ExecuteReader
IDataReader ExecuteReader(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

//ExecuteScalar
object ExecuteScalar(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
```

> 拓展查询方法

```c#
//SqlQuery
IEnumerable<T> SqlQuery<T>(string sql, object param = null, bool buffer = true, int? commandTimeout = null, CommandType? commandType = null);

//分页
IEnumerable<TEntity> QueryPageBySQL<TEntity>(int pageIndex, int pagesize, string sqltext, out int totalCount) where TEntity : class;
IEnumerable<TEntity> QueryPageByWhere<TEntity>(int pageIndex, int pagesize, string wherestr, string orderstr, out int totalCount) where TEntity : class;

//sqlmap查询
SqlMapResult QuerySqlMap<TEntity>(string scope, string sqlID, object param = null);
```

> 事务相关

```c#
void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
void CommitTransaction();
void RollbackTransaction();
```

> 启用统一工作单元

```c#
IUnitOfWork BeginUnitOfWork(bool keepConnectionAlive = true);
```


# 实体关系映射

所有关系映射都继承自ClassMapper<T>， 只有配置了关系映射才能CodeFirst自动生成数据库表结构。

```c#
	public class TestMapper : ClassMapper<TestEntity>
    {
        public TestMapper()
        {
            Table("B_Test");//映射到对应表
            Map(m => m.Id).Key(KeyType.TriggerIdentity).Description("主键");//主键类型
            Map(m => m.Title).Size(500).Description("标题");
            Map(m => m.Content).Size(2000).Description("内容");//最大字符长度2000，相当于NVarchar(max)
            Map(m => m.PublishUser).Description("发布者");
            Map(m => m.PublishTime).Description("发布时间");
            Map(m => m.EndTime).Description("结束时间");
            Map(m => m.Remark).Size(2000).Description("备注");
            Map(m => m.StatusCode).Description("状态");
            Map(m => m.TypeCode).Description("类型");
            Map(m => m.CreateTime).Description("创建时间");

            Description("测试数据表");//表的注释信息
            AutoMap();
        }
    }
```

> ###### 表方法

- Table：表名
- Description：表注释信息
- Sequence：指定序列（Oracle有效）
- Schema：表所属模式

> ###### 列方法

- Column：列名，默认不填写与属性名称一致
- Key：主键类型(NotAKey/Identity/TriggerIdentity/Guid/Assigned)
- Description：列注释信息
- Size：长度（有长度属性的类型生效string/decimal等），注意：最大2000
- Nullable：是否可空
- DefaultValue：默认值
- ReadOnly：是否只读
- Ignore：忽略当前列的映射




# FluentSqlBuilder

通过Lambda表达式生成SQL，它是Database数据库对象的属性。

如下例子假设UserInfo对象映射到TB_USER表

> ### UserInfo

```c#
    public enum RoleType
    {
        管理员 = 1,
        普通用户 = 2,
        经理 = 3
    }
	
	//实体对象
	public class UserInfo:Entity
    {
        //public int Id { get; set; }

        public DateTime? DTUPDATE { get; set; }

        public int Age { get; set; }
        public int Sex { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DTCreate { get; set; }
        public bool HasDelete { get; set; }
        public RoleType Role { get; set; }

        public UserInfo() {
            DTCreate = DateTime.Now;
        }
    }
    
	//映射关系
	public class UserInfoMapper : ClassMapper<UserInfo>
    {
        public UserInfoMapper()
        {
            Table("TB_USER");
            Description("用户信息表");
            Map(m => m.Id).Key(KeyType.Identity).Description("主键");
            Map(m => m.Age).Nullable(false).Description("年龄");
            Map(m => m.DTCreate).Nullable(false).Description("创建日期");
            Map(m => m.DTUPDATE).Description("更新日期");
            Map(m => m.Name).Size(50).Description("名称");
            Map(m => m.Email).Size(1000).Description("邮箱");
            Map(m => m.Sex).Description("性别");
            Map(m => m.HasDelete).Description("是否删除");
            Map(m => m.Role).Description("角色");
            AutoMap();
        }
    }
```

> ### Insert

```c#
FluentSqlBuilder.Insert<UserInfo>(() => new UserInfo { DTCreate = DateTime.Now, Name = "567", Sex = 1, Email = "123456@qq.com" });
/*生成SQL：
INSERT INTO TB_USER (DTCreate,Name,Sex,Email) VALUES ( GETDATE(), '567', 1, '123456@qq.com')
*/
```

> ### Update

```c#
FluentSqlBuilder.Update<UserInfo>(() => new UserInfo { Name = "", Sex = 1, Email = "123456@qq.com" });
/*生成SQL：
UPDATE TB_USER SET Name = '',Sex = 1,Email = '123456@qq.com'
*/

FluentSqlBuilder.Update<UserInfo>(() => new { Name = "", Sex = 1, Email = "123456@qq.com" }).
                           Where(u => u.Id == 1);
/*生成SQL：
UPDATE TB_USER SET Name = '',Sex = 1,Email = '123456@qq.com' WHERE Id = 1
*/
```

> ### Delete

```c#
FluentSqlBuilder.Delete<UserInfo>();
/*生成SQL：
DELETE FROM TB_USER
*/

FluentSqlBuilder.Delete<UserInfo>().Where(u => u.Id == null);
/*生成SQL：
DELETE FROM TB_USER WHERE Id IS null
*/
```



> ### Select

```c#
FluentSqlBuilder.Select<UserInfo>();
/*生成SQL：
SELECT * FROM TB_USER a
*/

FluentSqlBuilder.Select<UserInfo>(u => new { u.Id, u.Name });
/*生成SQL：
SELECT a.Id, a.Name FROM TB_USER a
*/


//枚举类型
FluentSqlBuilder.Select<UserInfo>(p => new
                       {
                           p.Role
                       }).Where(p => p.Role == RoleType.普通用户).OrderBy(p => p.Role);
/*生成SQL：
SELECT a.Role FROM TB_USER a WHERE a.Role = 2 ORDER BY a.Role
*/
```

> ### 多表关联

```c#
//多表Join关联查询
FluentSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           Join<Account>((u, a) => u.Id == a.UserId);
/*生成SQL：
SELECT a.Id, b.Name FROM TB_USER a JOIN Account b ON a.Id = b.UserId
*/

//多表InnerJoin关联查询
FluentSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           InnerJoin<Account>((u, a) => u.Id == a.UserId);
/*生成SQL：
SELECT a.Id, b.Name FROM TB_USER a INNER JOIN Account b ON a.Id = b.UserId
*/

//多表LeftJoin关联查询
FluentSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           LeftJoin<Account>((u, a) => u.Id == a.UserId);
/*生成SQL：
SELECT a.Id, b.Name FROM TB_USER a LEFT JOIN Account b ON a.Id = b.UserId
*/

//多表RightJoin关联查询
FluentSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           RightJoin<Account>((u, a) => u.Id == a.UserId);
/*生成SQL：
SELECT a.Id, b.Name FROM TB_USER a RIGHT JOIN Account b ON a.Id = b.UserId
*/

//多表FullJoin关联查询
FluentSqlBuilder.Select<UserInfo, Account>((u, a) => new { u.Id, a.Name }).
                           FullJoin<Account>((u, a) => u.Id == a.UserId);
/*生成SQL：
SELECT a.Id, b.Name FROM TB_USER a FULL JOIN Account b ON a.Id = b.UserId
*/

//多表复合关联查询
FluentSqlBuilder.Select<UserInfo, Account, Student, Class, City, Country>((u, a, s, d, e, f) =>
                           new { u.Id, a.Name, StudentName = s.Name, ClassName = d.Name, e.CityName, CountryName = f.Name }).
                           Join<Account>((u, a) => u.Id == a.UserId).
                           LeftJoin<Account, Student>((a, s) => a.Id == s.AccountId).
                           RightJoin<Student, Class>((s, c) => s.Id == c.UserId).
                           InnerJoin<Class, City>((c, d) => c.CityId == d.Id).
                           FullJoin<City, Country>((c, d) => c.CountryId == d.Id).
                           Where(u => u.Id != null);
/*生成SQL：
太长了......省略打印
*/
```

> ### Where

```c#
FluentSqlBuilder.Select<UserInfo>().Where(u => (u.Name == "b" && u.Id > 2) || u.Id < 1000);
/*生成SQL：
SELECT * FROM TB_USER a WHERE (a.Name ='b' and a.Id > 2) or a.Id < 1000
*/

//SQL拼接
FluentSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).
                        Where(u =>
                            SqlFuncs.In(u.Id, 1, 2, 3)
                            && u.HasDelete == false 
                            ).And(" a.Name ={0}", "李四").Or(" a.Name ={0} and a.ID >{1} ", "张茂", 5);

/*生成SQL：
SELECT a.Name AS  CNAME, a.Name, a.Id AS  CID FROM TB_USER a WHERE a.Id IN  ( 1,2,3)  AND a.HasDelete = 0 AND ( a.Name ='李四') OR ( a.Name ='张茂' and a.ID >5 )
*/
```

> ### 聚合函数

```c#
FluentSqlBuilder.Max<UserInfo>(u => u.Id);
/*生成SQL：
SELECT MAX(Id) FROM TB_USER
*/

FluentSqlBuilder.Min<UserInfo>(u => u.Id).Where(p => p.Id == 5);
/*生成SQL：
SELECT MIN(Id) FROM TB_USER WHERE Id = 5
*/

FluentSqlBuilder.Avg<UserInfo>(u => u.Id);
/*生成SQL：
SELECT AVG(Id) FROM TB_USER
*/

FluentSqlBuilder.Count<UserInfo>();
/*生成SQL：
SELECT COUNT(1) FROM TB_USER
*/

FluentSqlBuilder.Count<UserInfo>(u => u.Id);
/*生成SQL：
SELECT COUNT(Id) FROM TB_USER
*/

FluentSqlBuilder.Sum<UserInfo>(u => u.Id);
/*生成SQL：
SELECT SUM(Id) FROM TB_USER
*/

```

> ### SqlFuncs

```c#
//聚合函数
FluentSqlBuilder.Select<UserInfo>(p => new
                      {
                          CountValue = SqlFuncs.Count(),
                          SumValue = SqlFuncs.Sum(p.Age),
                          MaxValue = SqlFuncs.Max(p.Age),
                          MinValue = SqlFuncs.Min(p.Age),
                          AvgValue = SqlFuncs.Avg(p.Age),
                      });
/*生成SQL：
SELECT COUNT(1) AS CountValue, SUM(a.Age) AS SumValue, MAX(a.Age) AS MaxValue, MIN(a.Age) AS MinValue, AVG(a.Age) AS AvgValue FROM TB_USER a
*/

FluentSqlBuilder.Select<UserInfo>(p => new
                      {
                           ModV = SqlFuncs.Mod(500, 6),//注意：可能某些数据库不含此函数
                           Rand = SqlFuncs.Rand(),
                           IfNullV = SqlFuncs.IfNull(p.Name, "李梅")
                      });
/*生成SQL：
SELECT MOD(500, 6) AS ModV, RAND() AS Rand, ISNULL(a.Name,'李梅') AS IfNullV FROM TB_USER a
*/

FluentSqlBuilder.Select<UserInfo>().Where(a =>
                            SqlFuncs.In(a.Id, 1, 2, 3) &&
                            && SqlFuncs.Like(u.Name, "a")
                             && SqlFuncs.LikeLeft(u.Name, "b")
                             && SqlFuncs.LikeRight(u.Name, "c")
                        );
/*生成SQL：
SELECT * FROM TB_USER a WHERE ( ( a.Id IN  ( 1,2,3)  AND a.Name LIKE  '%'+'a'+'%') AND a.Name LIKE  '%'+'b') AND a.Name LIKE  'c'+'%'
*/
```

> ### 常规转换方法

```c#
//常规方法
FluentSqlBuilder.Select<UserInfo>(p => new
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
                           string.IsNullOrWhiteSpace(p.Email)
                       );
/*生成SQL：
SELECT LEN(a.Name) AS  长度, UPPER(a.Email) AS 大写, LOWER(a.Email) AS 小写, LTRIM(' 李佳佳 ') AS TrimStart, RTRIM(' 李 佳佳 ') AS TrimEnd, RTRIM(LTRIM(' 李佳佳 ')) AS Trim, ' 李佳佳 ' AS ToString, SUBSTRING(' 李佳佳 ',1+1,2) AS Substring, SUBSTRING(a.Email,1+1,LEN(a.Email)) AS SubstringWithAutoLength,  (CASE WHEN (a.Email IS NULL OR a.Email= '') THEN '空' ELSE '非空' END) AS IsNullOrEmpty FROM TB_USER a WHERE (a.Email IS NULL OR a.Email= '')
*/


```

> ### 数学函数

```c#
int intValue = -32;
double decimalValue = 20.8251;
//数学函数
FluentSqlBuilder.Select<UserInfo>(p => new
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
                           //ModV = SqlFuncs.Mod(decimalValue, 6),
                           Rand = SqlFuncs.Rand()

                       });
/*生成SQL：
SELECT ABS(a.Id) AS Abs, ABS(-32) AS AbsVAR, ROUND(20.8251,2) AS Round, ROUND(20.8251,2) AS RoundNoPrecision, CEILING(20.8251) AS Ceiling, FLOOR(20.8251) AS Floor, SQRT(20.8251) AS Sqrt, LOG(20.8251) AS Log, POWER(20.8251,2) AS Pow, SIGN(20.8251) AS Sign, RAND() AS Rand FROM TB_USER a
*/

```

> ### 日期相关函数

```c#
DateTime startTime = new DateTime(2015, 3, 2, 6, 5, 12);
DateTime endTime = new DateTime(2016, 1, 1, 0, 0, 0);
DateTime now = DateTime.Now;

//获取当前时间
FluentSqlBuilder.Select<UserInfo>(u => new
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
              });
/*生成SQL：
SELECT CAST('2017/9/5 15:54:20' AS DATETIME) AS  现在时间, GETDATE() AS 属性现在时间, GETUTCDATE() AS UTC时间, CAST(GETDATE() AS DATE) AS 今天, CAST('2017/9/5 15:54:20' AS DATE) AS 日期部分, DATEPART(YEAR,CAST(a.DTCreate AS NVARCHAR(MAX))) AS 年2, DATEPART(YEAR,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 年, DATEPART(MONTH,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 月, DATEPART(DAY,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 日, DATEPART(HOUR,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 时, DATEPART(MINUTE,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 分, DATEPART(SECOND,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) AS 秒, (DATEPART(WEEKDAY,CAST('2017/9/5 15:54:20' AS NVARCHAR(MAX))) - 1) AS 本周第几天0是周日 FROM TB_USER a
*/



//Add DateTime函数
FluentSqlBuilder.Select<UserInfo>(p => new
                       {
                           AddYearsVAR = startTime.AddYears(1),//DATEADD(YEAR,1,@P_0)
                           AddYears = p.DTCreate.AddYears(1),//DATEADD(YEAR,1,@P_0)
                           AddMonths = startTime.AddMonths(1),//DATEADD(MONTH,1,@P_0)
                           AddDays = startTime.AddDays(1),//DATEADD(DAY,1,@P_0)
                           AddHours = startTime.AddHours(1),//DATEADD(HOUR,1,@P_0)
                           AddMinutes = startTime.AddMinutes(2),//DATEADD(MINUTE,2,@P_0)
                           AddSeconds = startTime.AddSeconds(120),//DATEADD(SECOND,120,@P_0)
                           //AddMilliseconds = startTime.AddMilliseconds(20000),//DATEADD(MILLISECOND,20000,@P_0)
                       }).Where(p => p.DTCreate.AddYears(1) > DateTime.Now);

/*生成SQL：
SELECT DATEADD(YEAR,1,'2015/3/2 6:05:12') AS AddYearsVAR, DATEADD(YEAR,1,a.DTCreate) AS AddYears, DATEADD(MONTH,1,'2015/3/2 6:05:12') AS AddMonths, DATEADD(DAY,1,'2015/3/2 6:05:12') AS AddDays, DATEADD(HOUR,1,'2015/3/2 6:05:12') AS AddHours, DATEADD(MINUTE,2,'2015/3/2 6:05:12') AS AddMinutes, DATEADD(SECOND,120,'2015/3/2 6:05:12') AS AddSeconds FROM TB_USER a WHERE DATEADD(YEAR,1,a.DTCreate) > GETDATE()
*/



//Diff DateTime函数
FluentSqlBuilder.Select<UserInfo>(p => new
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
               }).Where(p => p.DTCreate.DiffDays(now) > 2);//两天前过滤

/*生成SQL：
SELECT DATEDIFF(YEAR,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffYearsVAR, DATEDIFF(YEAR,a.DTCreate,'2017/9/5 15:43:23') AS DiffYears, DATEDIFF(MONTH,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffMonths, DATEDIFF(DAY,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffDays, DATEDIFF(HOUR,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffHours, DATEDIFF(MINUTE,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffMinutes, DATEDIFF(SECOND,'2016/1/1 0:00:00','2017/9/5 15:43:23') AS DiffSeconds FROM TB_USER a WHERE DATEDIFF(DAY,a.DTCreate,'2017/9/5 15:43:23') > 2
*/


```

> ### Like函数

```c#
FluentSqlBuilder.Select<UserInfo>().Where(a =>
                            a.Name.StartsWith("李") &&
                            a.Name.EndsWith("明") &&
                            a.Name.Contains("云") &&
                            SqlFuncs.LikeLeft(a.Name, "明") &&
                            SqlFuncs.LikeRight(a.Name, "云") &&
                            SqlFuncs.Like(a.Name, "云") 
                        );
/*生成SQL：
SELECT * FROM TB_USER a WHERE ( ( ( ( a.Name LIKE  '李'+'%' AND a.Name LIKE  '%'+'明') AND a.Name LIKE  '%'+'云'+'%') AND a.Name LIKE  '%'+'明') AND a.Name LIKE  '云'+'%') AND a.Name LIKE  '%'+'云'+'%'
*/
```

> ### 集合相关函数

```c#
List<int> ids = new List<int>() { 1, 2, 3 };
List<string> names = new List<string>() { "abc", "efg" };
List<string> nameNots = new List<string>() { "xyz" };

FluentSqlBuilder.Select<UserInfo>().Where(u => ids.Contains(u.Id) && SqlFuncs.In(u.Name, names) || SqlFuncs.InNot(u.Name, nameNots)),
;
/*生成SQL：
SELECT * FROM TB_USER a WHERE ( a.Id IN (1,2,3)  AND a.Name IN  ( 'abc','efg') ) OR a.Name NOT IN  ( 'xyz')
*/
```

> ### Parse转换函数

```c#
FluentSqlBuilder.Select<UserInfo>(p => new
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
                           Bool_Parse = bool.Parse("false"),//CAST(N'4' AS REAL)
                           Bool_INT_Parse = bool.Parse("1"),//CAST(N'4' AS REAL)
                           DateTime_Parse = DateTime.Parse("2014-05-21 22:10:05"),//CAST(N'4' AS REAL)
                           Guid_Parse = Guid.Parse("D544BC4C-739E-4CD3-A3D3-7BF803FCE179"),

                       }).Where(p => p.Age > int.Parse("33"));
/*生成SQL：
SELECT CAST('33' AS INT) AS Int_Parse, CAST('11' AS TINYINT) AS Byte_Parse, CAST('22' AS SMALLINT) AS Short_Parse, CAST('2123123213' AS BIGINT) AS Long_Parse, CAST('12' AS TINYINT) AS Sbyte_Parse, CAST('12211' AS SMALLINT) AS Int16_Parse, CAST('44' AS INT) AS Int32_Parse, CAST('12211' AS BIGINT) AS Int64_Parse, CAST('3213.2516' AS FLOAT) AS Double_Parse, CAST('3213.2516' AS FLOAT) AS DoubleUP_Parse, CAST('4123.213' AS REAL) AS Float_Parse, CAST('25222.234454' AS DECIMAL(19,4)) AS Decimal_Parse, CAST('5222.234454' AS DECIMAL(19,4)) AS DecimalUP_Parse, CAST('false' AS BIT) AS Bool_Parse, CAST('1' AS BIT) AS Bool_INT_Parse, CAST('2014-05-21 22:10:05' AS DATETIME) AS DateTime_Parse, CAST('D544BC4C-739E-4CD3-A3D3-7BF803FCE179' AS UNIQUEIDENTIFIER) AS Guid_Parse FROM TB_USER a WHERE a.Age > CAST('33' AS INT)
*/
```

> ### Case When

```c#
FluentSqlBuilder.Select<UserInfo>(p => new
                      {
                          年龄 = p.Age,
                          年龄段 = p.Age < 15 ?
                          "少年" :
                          ((p.Age > 60) ?
                          "老年" :
                          (p.Age > 40) ? "中年" :
                          "青年")
                      });
/*生成SQL：
SELECT a.Age AS  年龄,  (CASE WHEN (a.Age<15) THEN '少年' WHEN (a.Age>60) THEN '老年' WHEN (a.Age>40) THEN '中年' ELSE '青年' END) AS 年龄段 FROM TB_USER a
*/
```

> ### 分页

```c#
FluentSqlBuilder.Select<UserInfo>(u => new { u.Age, 现在时间 = now, u.Name, 局部变量 = "23" })
             .OrderBy(p => p.Age).
             ThenByDescending(p => p.Name).
             TakePage(3, 15);
/*生成SQL：
SELECT TOP(15) [_proj].[Age], [_proj].[现在时间], [_proj].[Name], [_proj].[局部变量] FROM (SELECT ROW_NUMBER() OVER(ORDER BY a.Age ,a.Name DESC) AS [_row_number], a.Age, CAST('2017/9/5 16:03:29' AS DATETIME) AS  现在时间, a.Name, '23' AS 局部变量 FROM TB_USER a) [_proj] WHERE [_proj].[_row_number] >= 31 ORDER BY [_proj].[_row_number]
*/


//take range
FluentSqlBuilder.Select<UserInfo>(u => new { u.Age, 现在时间 = now, u.Name, 局部变量 = "23" })
             .OrderBy(p => p.Age).
             ThenByDescending(p => p.Name).
             TakeRange(3, 15);
/*生成SQL：
SELECT TOP(15) [_proj].[Age], [_proj].[现在时间], [_proj].[Name], [_proj].[局部变量] FROM (SELECT ROW_NUMBER() OVER(ORDER BY a.Age ,a.Name DESC) AS [_row_number], a.Age, CAST('2017/9/5 16:03:29' AS DATETIME) AS  现在时间, a.Name, '23' AS 局部变量 FROM TB_USER a) [_proj] WHERE [_proj].[_row_number] >= 3 ORDER BY [_proj].[_row_number]
*/
```

> ### Order By 排序

```c#
FluentSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).OrderBy(p => p.Age).ThenByDescending(p => p.Name);
/*生成SQL：
SELECT a.Name AS  CNAME, a.Name, a.Id AS  CID FROM TB_USER a ORDER BY a.Age ,a.Name DESC
*/

FluentSqlBuilder.Select<UserInfo>(u => new { CNAME = u.Name, u.Name, CID = u.Id }).OrderByString("a.DTCreate desc ");
/*生成SQL：
SELECT a.Name AS  CNAME, a.Name, a.Id AS  CID FROM TB_USER a ORDER BY a.DTCreate desc
*/
```

> ### GroupBy 分组

```c#
FluentSqlBuilder.Select<UserInfo>(p => new { p.Age, CountV = SqlFuncs.Sum(p.Age) })
.GroupBy(u => new { u.Age })
.ThenGroupBy(p => p.Id)
.Having(u => u.Age > 0 && SqlFuncs.Count() > 10);
/*生成SQL：
SELECT a.Age, SUM(a.Age) AS CountV FROM TB_USER a GROUP BY a.Age ,a.Id HAVING  a.Age > 0 AND COUNT(1) > 10
*/
```

> ### 其他

```c#
//Top
FluentSqlBuilder.Select<UserInfo>(u => new { 现在时间 = now, 局部变量 = "23" }).Top(3);
/*生成SQL：
SELECT TOP 3 CAST('2017/9/5 16:03:29' AS DATETIME) AS  现在时间, '23' AS 局部变量 FROM TB_USER a
*/


//Distinct
FluentSqlBuilder.Select<UserInfo>(u => new { 现在时间 = now, 局部变量 = "23" }).Distinct();
/*生成SQL：
SELECT DISTINCT CAST('2017/9/5 16:03:29' AS DATETIME) AS  现在时间, '23' AS 局部变量 FROM TB_USER a
*/
```

### 








