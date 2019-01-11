using FluentExpressionSQL;
using FluentExpressionSQL.Mapper;
using Expression2SqlTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pure.Data.Migration;

namespace Pure.Data.Test
{
    public class BackupAndGenTest
    {

        public static void Test()
        {


            string title = "BackupAndGenTest";
            Console.Title = title;

            CodeTimer.Time(title, 1, () => {

                 //Backup();
                Gen();

            });


            Console.Read();
            
            
        }
        public static void Backup()
        {


           
            var db = DbMocker.NewDataBase();

            //List<UserInfo> list = new List<UserInfo>();
            //for (int i = 0; i < 10; i++)
            //{
            //    var user1 = new UserInfo
            //    {
            //        Name = "NameInsert" + i,
            //        Age = 20 + 16,
            //        DTCreate = new DateTime(1985, 1, 1),
            //        Role = RoleType.经理
            //    };
            //    list.Add(user1);
            //    //db.Insert(user1);
            //}

            ////db.InsertBulk(list);
            //db.InsertBatch(list, new BatchOptions() { BatchSize = 30 });


            BackupOption option = new BackupOption();
            option.BackupExportType = BackupExportType.InsertSQL;
            option.EnableSqlFilter = false;
            option.SQL = "";// "SELECT * FROM TB_USER WHERE ID >0 AND ID <=10 ORDER BY ID ";
            var result = db.Backup<UserInfo>(option);

            Console.WriteLine(result.ToString()); 

        }


        public static void Gen()
        {



            var db = DbMocker.NewDataBase();

            db.GenerateCode();

           // var option = new CodeGenOption();
           // option.Enable = true;
           // option.OutputDir = PathHelper.CombineWithBaseDirectory("puredata_gen");
           // option.FilterTablePrefixs = "TB_;B_"; 
           // option.Namespace = "Pure.Test.Domain";
           //// option.IncludeTableNames = "tb_user;";
           // option.ClassNameMode = ClassNameMode.UpperOrigin;
           // option.PropertyNameMode = ClassNameMode.UpperOrigin;

           // var result = CodeGenHelper.Instance.Gen(db, DbMigrateService.Instance, option);


           // Console.WriteLine(result.ToString());

        }


    }
}
