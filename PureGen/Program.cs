using CommandLine.Text;
using CommandLine;
using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{



    class Program
    {

        private static readonly HeadingInfo HeadingInfo = new HeadingInfo("PureGen", "v1.0.1.1");

        private static void WriteMsg(string str)
        {
            //HeadingInfo.WriteMessage(str);
            Console.WriteLine(str);
        }
        private static void WriteError(string str)
        {
            HeadingInfo.WriteError(str);
        }
        static void Main(string[] args)
        {
            //WriteMsg(HeadingInfo.ToString());
            Boostrapers.Init();


            string testCmd = "new -b csharp_mvc -t article;comments -p AGRYGL -n AGRYGL.Core";
            testCmd = @"--help";
            //testCmd = @"doc --help";
            //testCmd = @"doc -t docx -c PureDataConfiguration.xml -z";
            //testCmd = @"build -l csharp -p G:\Benson\Source\RoGenerator\Pure\Pure.Data\Pure.Data.stardard\Pure.Data\PureGen\bin\Debug\generate\csharp_mvc\AGRYGL.sln";
            //testCmd = @"run -l csharp -p G:\Benson\Source\RoGenerator\Pure\Pure.Data\Pure.Data.stardard\Pure.Data\PureGen\bin\Debug\generate\csharp_mvc\AGRYGL.sln";
            //testCmd = @"run -l csharp -p G:\Benson\Source\RoGenerator\Pure\Pure.Data\Pure.Data.stardard\Pure.Data\PureGen\bin\Debug\generate\csharp_mvc\AGRYGL.Web\bin\Debug\netcoreapp2.1\AGRYGL.Web.dll";
            //testCmd = @"run -l csharp -p C:\Users\benson\source\repos\PureGen.Web\PureGen.Web\bin\Debug\netcoreapp2.1\PureGen.Web.dll";
            //testCmd = "gen -c PureDataConfiguration2.xml -p TestDEmo -n TestDEmo.NameSpac";
            //testCmd = "new -b csharp_mvc -t article;comments -p AGRYGL -n AGRYGL.Core";

            Console.WriteLine(testCmd);
            Console.WriteLine("------------------------");

            args = testCmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ParseAndExecute(args);

            Console.ReadLine();

        }

        private static void ParseAndExecute(string[] args)
        {


            Parser.Default.ParseArguments<GenOptions, NewOptions, DocOptions, BuildOptions, RunOptions, RestoreOptions>(args)
   .WithParsed<GenOptions>(opts =>
   { //生成代码
       DoWhenNoArgs(opts);
   })
   .WithParsed<NewOptions>(opts =>
   { //创建新模板项目

       if (opts.Boilerplate == "" || opts.Boilerplate == null)
       {
           WriteMsg("` --b %` Boilerplate list: ");
           var listTemplateType = NewBoilerplateManage.Providers;
           foreach (var item in listTemplateType)
           {
               WriteMsg(item.Name + "\t" + "(" + item.Description + ")");

           }
       }
       else
       {

           NewBoilerplateManage.Process(opts);
       }

   })
     .WithParsed<DocOptions>(opts =>
     { //生成数据库字典文档
         DocGenerator.Process(opts);

     })
   .WithParsed<BuildOptions>(opts =>
   { //编译项目
       if (opts.LangType == LangType.none)
       {
           WriteMsg("` --l %` LangType list: ");
           var listTemplateType = EnumHelpers.GetAllItems(typeof(LangType));// NewBoilerplateManage.Providers;
           foreach (var item in listTemplateType)
           {
               WriteMsg(item.Text + "\t" + "(" + item.Description + ")");

           }
       }
       else
       {
            CmdHelper.Run(CmdHelper.GetBuildCmd(opts));
       }

   })
   .WithParsed<RunOptions>(opts =>
   { //运行项目

       if (opts.LangType == LangType.none)
       {
           WriteMsg("` --l %` LangType list: ");
           var listTemplateType = EnumHelpers.GetAllItems(typeof(LangType));// NewBoilerplateManage.Providers;
           foreach (var item in listTemplateType)
           {
               WriteMsg(item.Text + "\t" + "(" + item.Description + ")");

           }
       }
       else
       {
           CmdHelper.Run(CmdHelper.GetRunCmd(opts));
           //CmdHelper.RunCmd(CmdHelper.GetRunCmd(opts), true);
       }

   })
   .WithParsed<RestoreOptions>(opts =>
   {//还原项目包文件

   })
   .WithNotParsed(errs =>
   {
       foreach (var err in errs)
       {
           WriteError(err.ToString());
       }

   });

        }

        private static void DoWhenNoArgs(GenOptions options)
        {
             
            NewBoilerplateManage.DoGen(options.ConfigFile, options.Project, options.NameSpace, options.PrefixFilter, options.OnlyGenTable);


            Console.ReadLine();
            //IDatabase database = new Database(config, LogStatic, null);

            //try
            //{
            //    string outputPath = database.GenerateCode();
            //    LogStatic("Generate output: " + outputPath);

            //    if (System.IO.Directory.Exists(outputPath))
            //    {
            //        string v_OpenFolderPath = outputPath;
            //        System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    LogStatic("Generate error: " + ex.Message, ex, MessageType.Error);
            //}


            //Console.ReadLine();
        }




    }
}
