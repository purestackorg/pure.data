using Pure.Data;
using Pure.Data.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace PureGen
{
    /// <summary>
    /// 当前线程下全局上下文信息
    /// </summary>
    public class CurrentGenerateContext
    {
        public static string ProjectName { get; set; }
    }
    public class NewBoilerplateManage
    {
        public static NewBoilerplateResult Process(NewOptions options) {
            NewBoilerplateResult result = null;
            //NewBoilerplateContenxt ctx = new NewBoilerplateContenxt();
            //ctx.Boilerplate = options.Boilerplate;

            var provider = Providers.FirstOrDefault(p=>p.Name == options.Boilerplate);
            if (provider != null)
            {
                //ctx.BoilerplatPath = DbLoader.CombinePathWithAppDirectory(@"\Boilerplats\"+ ctx.Boilerplate);
                //ctx.ProjectName = options.Project;
                //ctx.ProjectOutputPath = DbLoader.CombinePathWithAppDirectory(@"\generate\" + ctx.ProjectName);

                //result = provider.Process(ctx);

                string config = DbLoader.CombinePathWithAppDirectory(@"Boilerplats\" + options.Boilerplate + @"\config.xml");
                NewBoilerplateManage.DoGen(config, options.Project, options.NameSpace, options.TablePrefixFilter, options.OnlyGenTable);


                string genDir = DbLoader.CombinePathWithAppDirectory(@"generate\" + options.Boilerplate ); 
                LogStatic("Starting copy resource to generate directory ......");
                LogStatic("Generate directory : " + genDir);

                string resourceDir = DbLoader.CombinePathWithAppDirectory(@"Boilerplats\" + options.Boilerplate + @"\resource");
                var dirs = Directory.GetDirectories(resourceDir, "*", SearchOption.TopDirectoryOnly);

                CurrentGenerateContext.ProjectName = options.Project;

                foreach (var dir in dirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    var dirname = dirInfo.Name;// Path.GetDirectoryName(dir);
                    string realDirName = FormatSimpleTemplate(dirname);
                    string toDir = Path.Combine(genDir, realDirName);
                    LogStatic("Copy directory resource " + dirname + " to " + toDir);

                    CopyDirectory(dir, toDir);
                    LogStatic("Finish copy directory.");

                }

                LogStatic("Finish copy resource to generate directory !");

            }
            else
            {
                LogStatic("Not find Boilerplate ["+ options.Boilerplate + "] !", null, MessageType.Error);
            }

            return result;
        }



        public static string FormatSimpleTemplate(string str) {
            string r = "";
            r = str.Replace("{%ProjectName%}", CurrentGenerateContext.ProjectName);
            return r;
        }

        public static List<string> NeedFormatFileExts { get; set; }

        static NewBoilerplateManage() {
            if (NeedFormatFileExts == null)
            {
                NeedFormatFileExts = new List<string>();
                NeedFormatFileExts.Add(".cs");
                NeedFormatFileExts.Add(".java");
                NeedFormatFileExts.Add(".py");
                NeedFormatFileExts.Add(".go");
                NeedFormatFileExts.Add(".php");
                NeedFormatFileExts.Add(".xml");
                NeedFormatFileExts.Add(".json");
                NeedFormatFileExts.Add(".txt");
                NeedFormatFileExts.Add(".config");
            }
           

        }

        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                            //LogStatic("Create directory " + destPath + "\\" + i.Name );

                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                        //LogStatic("Copy directory " + i.FullName + " to " + destPath + "\\" + i.Name);

                    }
                    else
                    {
                        string ext = Path.GetExtension(i.FullName).ToLower();
                        if (NeedFormatFileExts.Contains(ext))
                        {
                            string originalContent = File.ReadAllText(i.FullName);
                            string realContent = FormatSimpleTemplate(originalContent);
                            File.WriteAllText(destPath + "\\" + i.Name, realContent);

                        }
                        else
                        {
                            File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件

                        }

                       
                        //LogStatic("Copy file " + i.FullName + " to " + destPath + "\\" + i.Name);

                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static List<INewBoilerplate> Providers = new List<INewBoilerplate>();

        public static void Register(INewBoilerplate p)
        {
            Providers.Add(p);
        }


        public static void DoGen(string config, string projectName = "", string nameSpace = "", string tablePrefixFilter = "", string onlyTable = "") {

             
            config = ConfigHelpers.GetDefaultConfig(config);// "PureDataConfiguration.xml";
            //string config = "";
            //config = "PureDataConfiguration.xml";
            //if (args.Length > 0)
            //{
            //    config = args[0];

            //}


            IDatabase database = new Database(config, LogStatic, option=> {
                option.CodeGenProjectName = projectName;
                option.CodeGenNameSpace = nameSpace;
                option.CodeGenTableFilter = tablePrefixFilter;
                option.AutoMigrateOnContainTable = onlyTable;
            });

            try
            {
                string outputPath = database.GenerateCode();
                LogStatic("Generate output: " + outputPath);


                ConfigHelpers.OpenDir(outputPath);
                 
            }
            catch (Exception ex)
            {
                LogStatic("Generate error: " + ex.Message, ex, MessageType.Error);
            }


            //Console.ReadLine();
        }


        public static void LogStatic(string msg, Exception ex = null, Pure.Data.MessageType type = Pure.Data.MessageType.Debug)
        {
            LogHelpers.LogStatic(msg, ex, type);

        }
    }


}
