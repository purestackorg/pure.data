using Pure.Data.Gen.CodeServer.RazorPaser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq; 
using System.Text.RegularExpressions;

namespace Pure.Data.Gen
{

    public class LocalGenerator : IGenerator
    {
        private Encoding ReadFileEncoding = Encoding.UTF8;
        private CancellationToken _cancelToken;
        public static event Action<string> LogReport;
        public IDebuger debugger = null;// DefaultDebuger.Instance;
        public ITemplateParser parser;
        // private Dictionary<string, string> _templatePools = new Dictionary<string, string>();
        public IParserConfig _parseConfig = null;
        public LocalGenerator(IDatabase db, IParserConfig parseConfig)
        {
            _parseConfig = parseConfig;
            debugger = new DefaultDebuger(db);
            parser = new RazorTemplatePaser(parseConfig);
           // parser = new LiquidTemplatePaser(parseConfig);
            LogReport += (msg) => debugger.WriteLine(msg);
        }

        #region Private methods
        private void OnLogReport(string msg)
        {
            if (LogReport != null)
                LogReport(msg);
        }
        private void CheckCancellation()
        {
            if (_cancelToken != null && _cancelToken.IsCancellationRequested)
            {
                _cancelToken.ThrowIfCancellationRequested();
            }
        }
        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="templateConfig"></param>
        /// <returns></returns>
        private string GetTemplate(GeneraterConfig templateConfig)
        {

            if (!string.IsNullOrEmpty(templateConfig.Template))
            {
                return templateConfig.Template;
            }
            else
            {
                string template = "";
                string templateFileName = templateConfig.TemplateFileName;
                string key = templateConfig.Name;
                //if (!_templatePools.ContainsKey(key) || (_templatePools.ContainsKey(key) && string.IsNullOrEmpty(_templatePools[key])))
                //{
                template =  (DbLoader.GetTemplateContent(templateFileName, templateConfig.Encoding) );
                    
                //    _templatePools[key] = template;
                //}
                //else
                //    template = _templatePools[key];
                return template;
            }
        }


        public void ClearCache(ProjectConfig config)
        {
            string templateContent = "", templateKey = "", modelName = "", outputFileName = "", stepmsg = "", outputContent = "";

            var enabledTemplates = config.GeneraterConfigs ;
            foreach (var template in enabledTemplates)
            {
                templateKey = DbLoader.GetTemplateKey(template.Name, template.TemplateFileName);
                parser.Reset(templateKey);
            }
        }

        private void ParseAndOutputByTables(IDatabase database, ProjectConfig config, List<string> filterTables = null)
        {

            try
            { 
               
                config.Database = database;
                List<Table> tables = null;

                List<string> withoutTables = new List<string>();
                if (database.Config.AutoMigrateWithoutTable != null && database.Config.AutoMigrateWithoutTable != "")
                {
                    withoutTables = database.Config.AutoMigrateWithoutTable.ToUpper().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

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



                    ///转换表
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
                    ///转换表
                    tables = ConvertDatabaseTableToTables(config, tablesInDb, filterTables, withoutTables);
                    //转换类名和属性名
                    tables = MakeClassName(config, tables);
                    
                }



                OutputContext context = new OutputContext();

                config.LastOutputContext = context;

                context.Tables = tables;
               // context.Mappers = mapperDict;
                context.ProjectConfig = config;
                context.ParserConfig = _parseConfig;
                var enabledTemplates = config.GeneraterConfigs.Where(p=>p.Enabled == true);
                string templateContent = "", templateKey = "", modelName = "", outputFileName = "", stepmsg = "", outputContent = "";

                //清空上一次的生成数据
                string projectDir = DbLoader.GetProjectDirectory(config);// config.Database.Config.CodeGenBaseDirectory;// DbLoader.GetProjectDirectory(config);

                try
                {
                    var files = Directory.GetFiles(projectDir, "", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        if (item != "")
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                    var dirs = Directory.GetDirectories(projectDir);
                    foreach (var dir in dirs)
                    {
                        Directory.Delete(dir);

                    }
                }
                catch (Exception)
                {
                     
                }
                

                foreach (var template in enabledTemplates)
                {
                    //CheckCancellation();
                    try
                    {
                        templateContent = GetTemplate(template);
                        if (string.IsNullOrEmpty(templateContent))
                        {
                            database.LogHelper.Debug("TemplateContent is empty，skip at ："+ template.TemplateFileName);
                            continue;
                            //throw new ArgumentException("模板内容不能为空！");
                        }
                        context.GeneraterConfig = template;
                        // result.FileName = template.TemplateFileName;
                        templateKey = DbLoader.GetTemplateKey(template.Name, template.TemplateFileName);
                        string reallyOutputFileName = "";

                        var outType = template.OutputType;
                        if (outType == OutputType.Table)
                        {
                            foreach (Table table in context.Tables)
                            {
                                try
                                {
                                    modelName = table.ClassName;
                                    context.ModelName = modelName;
                                    context.OutputFileName = DbLoader.FormatFileName(config, template, modelName, modelName);

                                    table.CurrentOutputContext = context;
                                    var Model = table;
                                    outputContent = parser.Parse<Table>(templateContent, templateKey, Model);
                                    reallyOutputFileName = context.RealOutputFileName;
                                    parser.OutputResult(reallyOutputFileName, outputContent, template.Encoding, template.Append);


                                    //result.ParseResult.Add( parser.ParseAndOutput<Table>(outputFileName, templateContent, templateKey, table, template.Encoding, template.Append));
                                    stepmsg = "template = " + template.Name + " , table = " + table.Name + string.Format(" , Generate in Template {0}, Table Model {1} is OK : {2}", template.Name, table.Name, reallyOutputFileName);


                                    OnLogReport(stepmsg);
                                }
                                catch (Exception ex11)
                                {

                                    database.LogHelper.Error(ex11);

                                    stepmsg = "template = " + template.Name + " , table = " + table.Name + " , Error: " + ex11.Message;
                                    OnLogReport(stepmsg);
                                }


                            }
                        }
                        else if (outType == OutputType.OutputContext)
                        {
                            modelName = template.Name; //string.IsNullOrEmpty(context.Owner) ? context.ProjectConfig.Name : context.Owner;
                            context.OutputFileName = DbLoader.FormatFileName(config, template, modelName, modelName);
                            //outputFileName = context.RealOutputFileName;// DbLoader.GetOutputFileName(config, template, modelName);
                            outputContent = parser.Parse<OutputContext>(templateContent, templateKey, context);
                            parser.OutputResult(context.RealOutputFileName, outputContent, template.Encoding, template.Append);
                            //result.ParseResult.Add(outputContent);

                            //result.ParseResult.Add(parser.ParseAndOutput<OutputContext>(context.RealOutputFileName, templateContent, templateKey, context, template.Encoding, template.Append));

                            stepmsg = string.Format(" Template {0}, OutputContext Model {1} is OK", template.Name, modelName);
                            //result.StepResult.Add(stepmsg);
                            OnLogReport(stepmsg);
                        }

                       


                        stepmsg =string.Format("  {0} End", template.Name);
                        OnLogReport(stepmsg);
                    }
                    catch (Exception ex)
                    {
                        //result.ErrorMessages.Add(ex.ToFriendlyString());
                        //result.ErrorMessages.Add(ErrorDetail.CreateByContent(ex.Message));
                        database.LogHelper.Error(ex);

                        stepmsg = "Error: " + ex.Message; 
                        OnLogReport(stepmsg);
                    }

                }


            }
            catch (Exception ex)
            {
                throw new Exception("[ParseAndOutputByTables Error]:" + ex +ex.StackTrace + ex.Source, ex.InnerException);
            }


        }


        public List<Table> ConvertDatabaseTableToTables(ProjectConfig config, List<TableInfo> mappers, List<string> filterTables = null, List<string> withoutTables = null)
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
                    col.IsComputed = false ;// propertyInfo.KeyType == KeyType.Assigned || propertyInfo.KeyType == KeyType.TriggerIdentity;

                    col.Table = table;

                    table.Columns.Add(col);
                }

                //    table.ClassMapper = mapper ;

                tables.Add(table);
            }


            return tables;
        }



        public List<Table> ConvertClassMapperToTables(ProjectConfig config, List<IClassMapper> mappers, List<string> filterTables = null, List<string> withoutTables = null) {
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
                table.Schema  = mapper.SchemaName;
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


        private string GetProperyTypeString(Type type) {
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
        public List<Table> MakeClassName(ProjectConfig config, List<Table> tables ) {
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
        public string MakePropertyName(ProjectConfig config,string name)
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


        public string ConvertNameCase(CodeGenClassNameMode mode, string name)
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
                        name = tmp ; 
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



        public void Run(IDatabase database, ProjectConfig _config, List<string> filterTables = null)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {

                OnLogReport(string.Format("Project - [{0}]", System.IO.Path.GetFileName(_config.Name)));
                OnLogReport("Generate Begin >>>");
           //     OnLogReport(_config.ConnectionString);


                ParseAndOutputByTables(  database, _config, filterTables);


            }
            catch (Exception ex)
            {
                OnLogReport("Error: " + ex.Message);
            }
            finally
            {
                stopWatch.Stop();
                OnLogReport("Generate End <<<");
                OnLogReport(string.Format("[{0}s] was executed.", (double)stopWatch.ElapsedMilliseconds / 1000));
            }

        }


    }

}
