using Pure.Data.Gen.CodeServer.RazorPaser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public static string MapPath(string path)
        { 
            return  path.Replace("~/", GetAppDirectory().TrimEnd('/') +"/") ; 
        }
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

        #region Template Engine

        public static ITemplateParser CreateTemplateEngine(IParserConfig parseConfig = null)
        {
            if (parseConfig == null)
            {
                parseConfig = new DefaultParserConfig();
            }
            return new RazorTemplatePaser(parseConfig);
        }

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

        #endregion


        #region Convert DatabaseConfig to ProjectConfig
        public static ProjectConfig ConvertDatabaseConfigToProjectConfig(IDatabase database) {
            var config = database.Config;

            ProjectConfig projectConfig = new ProjectConfig();
            projectConfig.Database = database;
            projectConfig.Enable = config.EnableCodeGen;
            projectConfig.ClassNameMode = config.CodeGenClassNameMode;
            projectConfig.PropertyNameMode = config.CodeGenPropertyNameMode;
            projectConfig.Name = config.CodeGenProjectName;
            projectConfig.NameSpace = config.CodeGenNameSpace;
            projectConfig.TableFilter = config.CodeGenTableFilter;

            foreach (var temp in config.CodeGenTemplates)
            {
                GeneraterConfig genConfig = new GeneraterConfig();
                genConfig.Name = temp.Name;
                genConfig.OutputFileExtension = temp.OutputFileExtension;
                genConfig.FilePrefix = temp.FilePrefix;
                genConfig.FileSuffix = temp.FileSuffix;
                genConfig.FileNameFormat = temp.FileNameFormat;
                genConfig.Enabled = temp.Enabled;
                genConfig.TemplateFileName = temp.TemplateFileName;
                //genConfig.Template = temp.Template;
                genConfig.OutputDirectory = temp.OutputDirectory;
                genConfig.Append = temp.Append;
                genConfig.Encoding = temp.Encoding;
                genConfig.OutputType = temp.OutputType;

                projectConfig.GeneraterConfigs.Add(genConfig);


            }
            return projectConfig;
        }

        #endregion


        #region Get Tables and Columns
        public static List<Table> GetTableInfos(IDatabase database, ProjectConfig config, List<string> filterTables , List<string> withoutTables , out string msg) {

            List<Table> tables = null;
            msg = "";
            if (database.Config.CodeGenType == CodeGenType.CodeFirst)
            {
                var mapperDict = database.GetAllMap();

                if (mapperDict == null)
                {
                    throw new ArgumentException("找不到任何Mapper");
                }
                var classMaps = mapperDict.Select(p => p.Value).ToList();

                if (classMaps == null || classMaps.Count == 0)
                {
                    throw new ArgumentException("找不到任何Mapper中的表");
                }

                msg = "There are "+ classMaps.Count +" tables finded by "+ database.Config.CodeGenType +" mode .";


                //转换表
                tables = ConvertClassMapperToTables(config, classMaps, filterTables, withoutTables);
                //转换类名和属性名
                tables = MakeClassName(config, tables);

            }
            else if (database.Config.CodeGenType == CodeGenType.DbFirst)
            {

                var _provider = database.CreateTransformationProvider();
                _provider.Database = database;

                var getSchema = _provider.GetSchema();
                if (getSchema == null || getSchema == "")
                {
                    _provider.SetSchema(database.DatabaseName);

                }

                var tablesInDb = _provider.GetTableInfos();
                if (tablesInDb == null || tablesInDb.Count == 0)
                {
                    throw new ArgumentException("找不到任何数据库中的表");
                }
                msg = "There are " + tablesInDb.Count + " tables finded by " + database.Config.CodeGenType + " mode .";


                ///转换表
                tables = ConvertDatabaseTableToTables(config, tablesInDb, filterTables, withoutTables);
                //转换类名和属性名
                tables = MakeClassName(config, tables);

            }


            return tables;
        }

        public static List<Table> ConvertDatabaseTableToTables(ProjectConfig config, List<TableInfo> mappers, List<string> filterTables = null, List<string> withoutTables = null)
        {
            List<Table> tables = new List<Table>();
            foreach (var mapper in mappers)
            {
                //过滤表
                if (filterTables != null && filterTables.Count > 0)
                {
                    if (!filterTables.Contains(mapper.TableName.ToUpper()))
                    {
                        continue;
                    }
                }
                if (withoutTables != null && withoutTables.Count > 0)
                {
                    if (withoutTables.Contains(mapper.TableName.ToUpper()))
                    {
                        continue;
                    }
                }

                Table table = new Table();
                table.Name = mapper.TableName;
                table.ClassName = table.Name;

                table.SequenceName = "";// mapper.SequenceName;
                table.Schema = "";// mapper.SchemaName;
                table.Comment = mapper.TableDescription;
                table.Ignore = false;// mapper.IgnoredMigrate;

                foreach (var propertyInfo in mapper.Columns)
                {
                    Column col = new Column();
                    col.Name = propertyInfo.ColumnName;
                    col.PropertyName = MakePropertyName(config, propertyInfo.PropertyName);
                    col.PropertyType = (propertyInfo.PropertyType);
                    col.DataType = propertyInfo.DataType;
                    col.RawType = propertyInfo.RawType;



                    col.Comment = propertyInfo.ColumnDescription;
                    col.Length = propertyInfo.ColumnLength;
                    col.Precision = propertyInfo.ColumnPrecision;
                    col.Scale = propertyInfo.ColumnScale;

                    col.DefaultValue = propertyInfo.DefaultValue;
                    col.IsNullable = propertyInfo.IsNullable;
                    col.Ignore = false;// propertyInfo.Ignored;
                    col.IsPK = propertyInfo.IsPrimaryKey;
                    col.IsAutoIncrement = propertyInfo.IsAutoIncrement;// propertyInfo.KeyType == KeyType.Identity || propertyInfo.KeyType == KeyType.TriggerIdentity;
                    col.IsComputed = false;// propertyInfo.KeyType == KeyType.Assigned || propertyInfo.KeyType == KeyType.TriggerIdentity;

                    col.Table = table;

                    table.Columns.Add(col);
                }

                //    table.ClassMapper = mapper ;

                tables.Add(table);
            }


            return tables;
        }

        public static List<Table> ConvertClassMapperToTables(ProjectConfig config, List<IClassMapper> mappers, List<string> filterTables = null, List<string> withoutTables = null)
        {
            List<Table> tables = new List<Table>();
            foreach (var mapper in mappers)
            {
                //过滤表
                if (filterTables != null && filterTables.Count > 0)
                {
                    if (!filterTables.Contains(mapper.TableName.ToUpper()))
                    {
                        continue;
                    }
                }

                if (withoutTables != null && withoutTables.Count > 0)
                {
                    if (withoutTables.Contains(mapper.TableName.ToUpper()))
                    {
                        continue;
                    }
                }

                Table table = new Table();
                table.Name = mapper.TableName;
                table.ClassName = table.Name;

                table.SequenceName = mapper.SequenceName;
                table.Schema = mapper.SchemaName;
                table.Comment = mapper.TableDescription;
                table.Ignore = mapper.IgnoredMigrate;

                foreach (var propertyInfo in mapper.Properties)
                {
                    Column col = new Column();
                    col.Name = propertyInfo.ColumnName;
                    col.PropertyName = MakePropertyName(config, propertyInfo.PropertyInfo.Name);
                    col.PropertyType = GetProperyTypeString(propertyInfo.PropertyInfo.PropertyType);
                    col.DataType = propertyInfo.PropertyInfo.PropertyType;


                    col.Comment = propertyInfo.ColumnDescription;
                    col.Length = propertyInfo.ColumnSize;
                    col.DefaultValue = propertyInfo.ColumnDefaultValue;
                    col.IsNullable = propertyInfo.IsNullabled;
                    col.Ignore = propertyInfo.Ignored;
                    col.IsPK = propertyInfo.IsPrimaryKey;
                    col.IsAutoIncrement = propertyInfo.KeyType == KeyType.Identity || propertyInfo.KeyType == KeyType.TriggerIdentity;
                    col.IsComputed = propertyInfo.KeyType == KeyType.Assigned || propertyInfo.KeyType == KeyType.TriggerIdentity;

                    col.Table = table;

                    table.Columns.Add(col);
                }

                //    table.ClassMapper = mapper ;

                tables.Add(table);
            }


            return tables;
        }


        private static string GetProperyTypeString(Type type)
        {
            var realType = ReflectionHelper.GetNonNullableType(type);
            string result = "string";
            if (realType == typeof(int))
            {
                result = "int";
            }
            else if (realType == typeof(short))
            {
                result = "short";
            }
            else if (realType == typeof(long))
            {
                result = "long";
            }
            else if (realType == typeof(string) || realType == typeof(String))
            {
                result = "string";
            }
            else if (realType == typeof(DateTime))
            {
                result = "DateTime";
            }
            else if (realType == typeof(double))
            {
                result = "double";
            }
            else if (realType == typeof(float))
            {
                result = "float";
            }
            else if (realType == typeof(decimal))
            {
                result = "decimal";
            }
            else if (realType == typeof(bool))
            {
                result = "bool";
            }
            else if (realType == typeof(byte))
            {
                result = "byte";
            }
            else if (realType == typeof(Guid))
            {
                result = "Guid";
            }
            return result;

        }

        /// <summary>
        /// 生成类名称
        /// </summary>
        /// <returns></returns>
        public static List<Table> MakeClassName(ProjectConfig config, List<Table> tables)
        {
            //var rxClean = new Regex("^(Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
            //string strprefix = string.IsNullOrEmpty(config.ClassPrefix) ? "" : config.ClassPrefix, strsuffix = string.IsNullOrEmpty(config.ClassSuffix) ? "" : config.ClassSuffix;

            foreach (var t in tables)
            {
                //过滤表名
                if (!string.IsNullOrEmpty(config.TableFilter))
                {
                    foreach (string filter in config.TableFilter.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(filter))
                        {
                            if (t.ClassName.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                t.ClassName = t.ClassName.Remove(0, filter.Length);

                                break;
                            }
                            //t.ClassName = System.Text.RegularExpressions.Regex.Replace(t.ClassName, filter, "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        }
                    }
                }

                //     t.FilteredName = strprefix + t.ClassName + strsuffix;

                // t.ClassName = strprefix + t.ClassName + strsuffix;
                //foreach (var c in t.Columns)
                //{
                //    if (c != null && !string.IsNullOrEmpty(c.PropertyName))
                //    {
                //        c.PropertyName = rxClean.Replace(c.PropertyName, "_$1");

                //        // Make sure property name doesn't clash with class name
                //        if (c.PropertyName == t.ClassName)
                //            c.PropertyName = "_" + c.PropertyName;
                //    }

                //}

                t.ClassName = ConvertNameCase(config.ClassNameMode, t.ClassName);


                //if (config.ClassNameMode == CodeGenClassNameMode.UpperCamelCase || config.ClassNameMode == CodeGenClassNameMode.UpperAll)
                //{
                //    //格式化SYS_LOG > SysLog 
                //    var arr = t.ClassName.Split('_');
                //    string tmp = "";
                //    foreach (var item in arr)
                //    {
                //        if (!string.IsNullOrEmpty(item))
                //        {
                //            tmp += item.Substring(0, 1).ToUpper() + item.Substring(1).ToLower();
                //        }
                //    }

                //    if (config.ClassNameMode == CodeGenClassNameMode.UpperAll)
                //    {
                //        if (tmp != "")
                //        {
                //            t.ClassName = tmp.ToUpper();
                //        }
                //    }
                //    else
                //    {
                //        if (tmp != "")
                //        {
                //            t.ClassName = tmp;
                //        }
                //    }
                //}
                //else if (config.ClassNameMode == CodeGenClassNameMode.UpperOrigin)
                //{
                //    t.ClassName = t.ClassName.ToUpper();

                //}
            }

            return tables;
        }

        /// <summary>
        /// 生成属性名称
        /// </summary>
        /// <returns></returns>
        public static string MakePropertyName(ProjectConfig config, string name)
        {

            return ConvertNameCase(config.PropertyNameMode, name);
            //CodeGenClassNameMode mode = config.PropertyNameMode;

            //if (mode == CodeGenClassNameMode.UpperCamelCase || mode == CodeGenClassNameMode.UpperAll)
            //{
            //    //格式化SYS_LOG > SysLog 
            //    var arr = name.Split('_');
            //    string tmp = "";
            //    foreach (var item in arr)
            //    {
            //        if (!string.IsNullOrEmpty(item))
            //        {
            //            tmp += item.Substring(0, 1).ToUpper() + item.Substring(1).ToLower();
            //        }
            //    }

            //    if (mode == CodeGenClassNameMode.UpperAll)
            //    {
            //        if (tmp != "")
            //        {
            //            name = tmp.ToUpper();
            //        }
            //    }
            //    else
            //    {
            //        if (tmp != "")
            //        {
            //            name = tmp;
            //        }
            //    }
            //}
            //else if (mode == CodeGenClassNameMode.UpperOrigin)
            //{
            //    name = name.ToUpper();

            //}

            //return name;
        }


        public static string ConvertNameCase(CodeGenClassNameMode mode, string name)
        {
            if (mode == CodeGenClassNameMode.Origin)
            {
                return name;
            }
            else if (mode == CodeGenClassNameMode.CamelCase || mode == CodeGenClassNameMode.PascalCase || mode == CodeGenClassNameMode.UpperAll)
            {
                //格式化SYS_LOG > SysLog 
                // var arr = name.Split('_');
                string tmp = Inflector.ToCamelString(name);
                //foreach (var item in arr)
                //{
                //    if (!string.IsNullOrEmpty(item))
                //    {
                //        tmp += item.Substring(0, 1).ToUpper() + item.Substring(1).ToLower();
                //    }
                //}

                if (mode == CodeGenClassNameMode.UpperAll)
                {
                    name = tmp.ToUpper();
                }
                else if (mode == CodeGenClassNameMode.CamelCase)
                {
                    name = tmp;
                }
                else if (mode == CodeGenClassNameMode.PascalCase)
                {
                    tmp = Inflector.ToPascalString(name);
                    name = tmp;
                }

                return name;
            }
            else if (mode == CodeGenClassNameMode.HumanCase)
            {
                name = Inflector.ToHumanCase(name);

            }
            else if (mode == CodeGenClassNameMode.TitleCase)
            {
                name = Inflector.ToTitleCase(name);

            }
            else if (mode == CodeGenClassNameMode.UpperOrigin)
            {
                name = name.ToUpper();

            }
            else if (mode == CodeGenClassNameMode.LowerOrigin)
            {
                name = name.ToLower();

            }
            return name;
        }
        #endregion


    }
}
