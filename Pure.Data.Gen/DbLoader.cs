using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Pure.Data.Gen
{
    public class DbLoader
    {
        private const string RootBaseDirectoryName = "generate";
        private const string TemplateDirectoryName = "template";
        /// <summary>
        /// 解析器配置
        /// </summary>
        public static IParserConfig ParserConfig
        {
            get
            {
                if (_ParserConfig == null)
                {
                    _ParserConfig = new DefaultParserConfig();
                }
                return _ParserConfig;
            }
        }
        private static IParserConfig _ParserConfig = null;



        #region Output path
        public static string GetAppDirectory()
        {

            return ParserConfig.TemplateRootDir;// AppDomain.CurrentDomain.BaseDirectory;
        }
        public static string CombinePathWithAppDirectory(string path)
        {

            return PathHelper.CombineWithBaseDirectory( path);
        }
        public static string GetDataDirectory()
        {
            string dataFilePath = System.IO.Path.Combine(GetAppDirectory(), RootBaseDirectoryName);
            if (!Directory.Exists(dataFilePath))
            {
                Directory.CreateDirectory(dataFilePath);
            }
            return dataFilePath;
        }
        //public static string GetTemplateDirectory()
        //{
        //    string dataFilePath = System.IO.Path.Combine(GetAppDirectory(), TemplateDirectoryName);
        //    if (!Directory.Exists(dataFilePath))
        //    {
        //        Directory.CreateDirectory(dataFilePath);
        //    }
        //    return dataFilePath;
        //}
        public static string GetProjectDirectory(ProjectConfig config)
        {
            //string dataFilePath =     System.IO.Path.Combine(GetDataDirectory(), config.Name);

           string dataFilePath = config.Database.Config.CodeGenBaseDirectory.TrimEnd('\\'); 
             
            if (!Directory.Exists(dataFilePath))
            {
                Directory.CreateDirectory(dataFilePath);
            }
            return dataFilePath;
        }
        public static string GetModuleDirectory(ProjectConfig config, GeneraterConfig moduleconfig, string modelname)
        {
            if (string.IsNullOrEmpty(moduleconfig.OutputDirectory))
            {
                moduleconfig.OutputDirectory = "";
            }
            //格式化
            //string outDir = Format(moduleconfig.OutputDirectory, config, moduleconfig, moduleconfig.OutputDirectory, moduleconfig.OutputDirectory);
            string outDir =  Format(moduleconfig.OutputDirectory, config, moduleconfig, modelname, moduleconfig.OutputDirectory);
            string dataFilePath = (GetProjectDirectory(config) + outDir); //(config.Database.Config.CodeGenBaseDirectory.TrimEnd('\\') +outDir);
            //string dataFilePath = System.IO.Path.Combine(GetProjectDirectory(config), outDir);
            if (!Directory.Exists(dataFilePath))
            {
                Directory.CreateDirectory(dataFilePath);
            }
            return dataFilePath;
        }

        public static string GetTemplateContent(string templateFileName, string encoding = "utf-8")
        {
            string template = "";
            string filename = (GetAppDirectory()+ templateFileName);

            if (System.IO.File.Exists(filename))
            {
                template = FileHelper.ReadFileToString(filename, encoding);
            }
            return template;
        }
        public static string GetOutputFileName(ProjectConfig config, GeneraterConfig moduleconfig, string outputFileName, string modelName)
        {
            if (string.IsNullOrEmpty(outputFileName))
            {
                outputFileName = "";
            }
            string filename = moduleconfig.FilePrefix + outputFileName + moduleconfig.FileSuffix + moduleconfig.OutputFileExtension;
            string dataFilePath = System.IO.Path.Combine(GetModuleDirectory(config, moduleconfig, modelName), filename);
            string directory = Path.GetDirectoryName(dataFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            return dataFilePath;
        }

        public static string Format(string str, ProjectConfig config, GeneraterConfig template, string tableName, string defaultName)
        {
            string result = defaultName;
            if (!string.IsNullOrEmpty(str))
            {
                result = str;
                result = result.Replace("{%ProjectName%}", config.Name);
                result = result.Replace("{%TemplateName%}", template.Name);
                result = result.Replace("{%ObjectName%}", tableName);
                //result = result.Replace("{%BoilerplatPath%}", tableName);
                //result = result.Replace("{%ProjectOutputPath%}", tableName);
            }
            return result;
        }

        public static string FormatFileName(ProjectConfig config, GeneraterConfig template, string tableName, string defaultName)
        {

            return Format(template.FileNameFormat, config, template, tableName, defaultName);
        }


        //public static string GetModuleDirectory(GeneraterConfig config)
        //{
        //    string dataFilePath = System.IO.Path.Combine(GetDataDirectory(), config.OutputDirectory);
        //    if (!Directory.Exists(dataFilePath))
        //    {
        //        Directory.CreateDirectory(dataFilePath);
        //    }
        //    return dataFilePath;
        //}
        #endregion
        /// <summary>
        /// 获取模板Key
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateFileName"></param>
        /// <returns></returns>
        public static string GetTemplateKey(string templateName, string templateFileName)
        {
            string key = "";
            if (!string.IsNullOrEmpty(templateFileName))
            {
                key = templateFileName.Replace("\\", "_").Replace("/", "_").Replace(ParserConfig.TemplateExt, "");
            }
            else if (!string.IsNullOrEmpty(templateName))
            {
                key = templateName.Replace("\\", "_").Replace("/", "_").Replace(ParserConfig.TemplateExt, "");

            }

            return key;
        }

     

    }
}
