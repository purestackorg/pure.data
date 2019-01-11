using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pure.Data.SqlMap;

namespace Pure.Data.Test
{
    public class SqlMapTest
    {

        public static void Test()
        {


            string title = "SqlMapTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {
                 
                SqlMapMethodTest().ConfigureAwait(false); 
            });


            Console.Read();
            
            
        }


        public static async Task SqlMapMethodTest()
        {
            var db = DbMocker.NewDataBase();

            try
            {

                db = DbMocker.NewDataBase();
                var user = new UserInfo() { Id = 9, Name = "'eo\"莱卡布\"" };
                var result88 = await db.UpdateOnlyAsync<UserInfo>(user,null, "Name", "Age");

                var result6 = db.QuerySqlMap("TB_USER", "GetListInclude", new { Name = "fsdsd", ID = 2, Nam = "23", Ids = new long[] { 1, 2, 3, 4 } });
                db.LogHelper.Write(result6.Sql);


                var result5 = db.QuerySqlMap("TB_USER", "GetListTest", new { Name="fsdsd", ID = 2, Nam="23", Ids = new long[] { 1, 2, 3, 4 } });
                db.LogHelper.Write(result5.Sql);

                


                db.LogHelper.Write("------------rawsql: " + result5.RawSql);
                var data5 = result5.ExecuteList<object>();

                List<UserInfo> dd = new List<UserInfo>();
                dd.Add(new UserInfo() { Name = "fdd", Id = 32 });
                //dd.Add(new UserInfo() { Name = "范德萨", Id = 22 });
               // dd = null;
                //var result2 = db.QuerySqlMap("TB_USER", "GetListByFor", new { Id = 9, Name = "eo", OrderBy = 3, Yes = true, No = true, LikeNames = dd });
                //db.LogHelper.Write(result2.Sql);
                //db.LogHelper.Write("------------rawsql: " + result2.RawSql);

                //var data2 = result2.ExecuteList<UserInfo>();


                //var TestVari = db.QuerySqlMap("TB_USER", "TestVari", new { tablename = "tablename2", ordertext = "name", orderdesc = "desc", Yes = true, Name = "" });
                //db.LogHelper.Write(TestVari.Sql);
                //db.LogHelper.Write("------------rawsql: " + TestVari.RawSql);

                int total = 0;
                var result44 = db.QuerySqlMap("TB_USER", "GetList", new { tablename = "刚发的", Id = 9, Name = "'eo\"莱卡布\"", Ids = new long[] { 1, 2, 3, 4 } });
                var data44 = result44.ExecutePageList<UserInfo>(1, 10, out total, "");
                ////获取列表In
                //var result = db.QuerySqlMap("TB_USER", "GetList", new { Ids = new long[] { 1, 2, 3, 4 } });
                //db.LogHelper.Write(result.Sql);
                //db.LogHelper.Write("------------rawsql: "+result.RawSql);

                //var data = result.ExecuteList<UserInfo>();


                ////组合查询
                //var result1 = db.QuerySqlMap("TB_USER", "GetListCombine", new { Id = 9, Name = "'eo\"莱卡布\"" });
                //db.LogHelper.Write(result1.Sql);
                //db.LogHelper.Write("------------rawsql: " + result1.RawSql);

                //var data1 = result1.ExecuteList<UserInfo>();
                //db = DbMocker.NewDataBase();

                //var result2 = db.QuerySqlMap("TB_USER", "GetListInclude", new { Id = 9, Name = "eo", OrderBy = 3, Yes = true, No = true, LikeNames = new string[] { "2", "55" } });
                //db.LogHelper.Write(result2.Sql);
                //db.LogHelper.Write("------------rawsql: " + result2.RawSql);

                //var data2 = result2.ExecuteList<UserInfo>();

               // System.Threading.Thread.Sleep(2 * 1000);
            }
            catch (Exception ex)
            {
                
                throw;
            }
            finally
            {
                db.Close();
            }
        

        }
    }
}
