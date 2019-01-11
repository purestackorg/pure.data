using FluentExpressionSQL.Sql;
using Pure.Data.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;
namespace Pure.Data
{
    public interface ICodeGenEngine
    {
        string Generate(IDatabase database);
    }
    public class CodeGenHelper : Singleton<CodeGenHelper>
    {
        private object olock = new object();
        private bool HasGen = false;
        ICodeGenEngine engine = null;
        private static string  OutputGenDir = "";
        public string Gen(IDatabase db) {

             
            if (HasGen == false)
            {
                lock (olock)
                {
                    try
                    {
                        db.LogHelper.Debug("CodeGenEngine starting ...");
                        string fullName = "Pure.Data.Gen.CodeGenEngine";
                        string assemblyName = "Pure.Data.Gen";
                        if (engine == null)
                        {
                            try
                            {
                                engine = ReflectionHelper.CreateInstance<ICodeGenEngine>(fullName, assemblyName);

                            }
                            catch (Exception ex)
                            {
                                throw new PureDataException("Please referrence Pure.Data.Gen.CodeGenEngine.dll before generate code ! ", ex);
                            }

                        }
                        db.LogHelper.Debug("CodeGenEngine init finised !");

                        if (engine != null)
                        {
                            OutputGenDir = engine.Generate(db);

                            db.LogHelper.Debug("CodeGenEngine generate done !");
                             
                        }
                        else
                        {
                            throw new Exception("CodeGenEngine can not be null !" );

                        }
                    }
                    catch (Exception ex)
                    {

                        throw new Exception("CodeGenHelper occurs error:"+ex);
                    }

                    HasGen = true;
                }
            }
            return OutputGenDir;
        }
        ///// <summary>
        ///// 生成对象类
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="db"></param>
        ///// <param name="option"></param>
        ///// <returns></returns>
        //public bool Gen(IDatabase db, IDbMigratorService svr, CodeGenOption option)
        //{
        //    if (option.Enable == false)
        //    {
        //        return false;
        //    }
        //    var dbType = db.DatabaseType;
        //    string connectionstring = db.ConnectionString;
        //    if (option.Schema == "" || option.Schema == null)
        //    {
        //        option.Schema = db.DatabaseName;
        //    }

        //    var _provider = svr.CreateTransformationProviderByDatabaseType(db);
        //    _provider.Database = db;

        //    var getSchema = _provider.GetSchema();
        //    if (getSchema ==null || getSchema == "")
        //    {
        //        _provider.SetSchema(option.Schema);

        //    }

        //    var tables = _provider.GetTableInfos();

        //    string realPath = "";
        //    string ext = "";

        //    if (tables == null || tables.Count == 0)
        //    {
        //        throw new ArgumentException("There are no tables finded !");
        //    }

        //    List<TableInfo> clearTableInfos = tables;
        //    //指定表
        //    if (!string.IsNullOrEmpty(option.IncludeTableNames))
        //    {
        //        var filters = option.IncludeTableNames.ToUpper().Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //        clearTableInfos = tables.Where(p => filters.Contains(p.TableName.ToUpper())).ToList();

        //    }
        //    //过滤表
        //    if (!string.IsNullOrEmpty(option.IgnoreTableNames))
        //    {
        //        var filters = option.IgnoreTableNames.ToUpper().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //        clearTableInfos = tables.Where(p => !filters.Contains(p.TableName.ToUpper())).ToList();

        //    }

        //    string[] FilterTablePrefixsArray = null;
        //    if (!string.IsNullOrEmpty(option.FilterTablePrefixs))
        //    {
        //        FilterTablePrefixsArray = option.FilterTablePrefixs.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //    }

        //    bool HasFilterTablePrefix = FilterTablePrefixsArray != null && FilterTablePrefixsArray.Length > 0;
        //    //string strprefix = string.IsNullOrEmpty(option.ClassPrefix) ? "" : option.ClassPrefix, strsuffix = string.IsNullOrEmpty(config.ClassSuffix) ? "" : config.ClassSuffix;
        //    foreach (var table in clearTableInfos)
        //    {
        //        table.ClassName = CleanUpHelper.CleanUp(table.TableName);
        //        table.ClassName = Inflector.MakeSingular(table.ClassName);

        //        //过滤表名
        //        if (HasFilterTablePrefix == true)
        //        {
        //            foreach (string filter in FilterTablePrefixsArray)
        //            {
        //                if (!string.IsNullOrEmpty(filter))
        //                {
        //                    if (table.ClassName.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        table.ClassName = table.ClassName.Remove(0, filter.Length);

        //                        break;
        //                    }

        //                }
        //            }
        //        }



        //        //转换名称
        //        table.ClassName = ConvertName(option.ClassNameMode, table.ClassName);

        //        foreach (var col in table.Columns)
        //        {
        //            col.PropertyName = ConvertName(option.PropertyNameMode, col.PropertyName);

        //        }

        //        //生成
        //        BuildText(db, table, option);



        //    }






        //    return true;

        //}

        //private string ConvertName(ClassNameMode mode, string name)
        //{
        //    if (mode == ClassNameMode.Origin)
        //    {
        //        return name;
        //    }
        //    if (mode == ClassNameMode.CamelCase)
        //    {
        //        name = NameFixer.ToCamelCase(name);
        //    }
        //    else if (mode == ClassNameMode.PascalCase)
        //    {
        //        name = NameFixer.ToPascalCase(name);
        //    }
        //    else if (mode == ClassNameMode.UpperAll)
        //    {
        //        //格式化SYS_LOG > SysLog 
        //        var arr = name.Split('_');
        //        string tmp = "";
        //        foreach (var item in arr)
        //        {
        //            if (!string.IsNullOrEmpty(item))
        //            {
        //                tmp += item.Substring(0, 1).ToUpper() + item.Substring(1).ToLower();
        //            }
        //        }

        //        if (mode == ClassNameMode.UpperAll)
        //        {
        //            if (tmp != "")
        //            {
        //                name = tmp.ToUpper();
        //            }
        //        }

        //    }
        //    else if (mode  == ClassNameMode.UpperOrigin)
        //    {
        //        name = name.ToUpper();

        //    }

        //    return name;
        //}


        //public void BuildText(IDatabase db,  TableInfo  table, CodeGenOption option) 
        //{
        //    if (!System.IO.Directory.Exists(option.OutputDir))
        //    {
        //        System.IO.Directory.CreateDirectory(option.OutputDir);
        //    }
        //    //生成
        //    foreach (var template in option.Templates)
        //    {
        //        if (template.Enable == false)
        //        {
        //            break;
        //        }

        //        string ClassName = table.ClassName;
        //        string OutputFileName = "";//输出文件名
        //        OutputFileName = template.NamePrefix + ClassName + template.NameSuffix;

        //        if (template.Type == TemplateType.Mapper)
        //        {

        //        }
        //        else
        //        {
        //            ClassName = template.NamePrefix + ClassName + template.NameSuffix;

        //        }

        //        string content = ReplaceTemplate( template.TemplateContent, "TableName", table.TableName);
        //        content = ReplaceTemplate(content, "TableDescription", table.TableDescription);
        //        content = ReplaceTemplate(content, "Schema", table.Schema);
        //        content = ReplaceTemplate(content, "ClassName", ClassName);
        //        content = ReplaceTemplate(content, "Namespace", template.Namespace);

        //        StringBuilder sb = new StringBuilder();
        //        foreach (var column in table.Columns)
        //        {
        //            string innerTemplate = template.TemplateInnerContent;
        //            //生成Mapper
        //            if (template.Type == TemplateType.Mapper)
        //            {
        //                if (column.IsPrimaryKey)
        //                {
        //                    if (column.IsAutoIncrement )
        //                    {
        //                        innerTemplate = @"Map(m => m.#{PropertyName}).Key(KeyType.Identity).Description(""#{ColumnDescription}""); ";

        //                    }
        //                    else
        //                    {
        //                        innerTemplate = @"Map(m => m.#{PropertyName}).Key(KeyType.Assigned).Description(""#{ColumnDescription}""); ";

        //                    }
        //                }
        //                else
        //                {
        //                    if (column.DataType == typeof(int) || column.DataType == typeof(short) || column.DataType == typeof(long) || column.DataType == typeof(byte) || column.DataType == typeof(bool)
        //                         || column.DataType == typeof(DateTime) || column.DataType == typeof(Guid))
        //                    {
        //                        innerTemplate = @"            Map(m => m.#{PropertyName}).Description(""#{ColumnDescription}""); ";

        //                    }
        //                    else
        //                    {
        //                        innerTemplate = @"            Map(m => m.#{PropertyName}).Description(""#{ColumnDescription}"").Size(#{ColumnLength}); ";

        //                    }

        //                }

        //            }


        //            string contentInner = ReplaceTemplate(innerTemplate, "ColumnName", column.ColumnName);
        //            contentInner = ReplaceTemplate(contentInner, "ColumnDescription", column.ColumnDescription);
        //            contentInner = ReplaceTemplate(contentInner, "PropertyName", column.PropertyName);
        //            contentInner = ReplaceTemplate(contentInner, "PropertyType", column.PropertyType);
        //            contentInner = ReplaceTemplate(contentInner, "ColumnLength", (column.ColumnLength/2).ToString());




        //            sb.AppendLine(contentInner);
        //        }

        //        //替换内容
        //        content = ReplaceTemplate(content, "TemplateDetail", sb.ToString());




        //        string outputFileDir = System.IO.Path.Combine(option.OutputDir, template.Name);
        //        if (!System.IO.Directory.Exists(outputFileDir))
        //        {
        //            System.IO.Directory.CreateDirectory(outputFileDir);
        //        }


        //        string realPath = System.IO.Path.Combine(outputFileDir, OutputFileName + template.FileExtensions);
        //        System.IO.File.WriteAllText(realPath, content);

        //    }


        //}



        //private string GetKeyFormat(string key) {
        //    return "#{" + key + "}";
        //}

        //private string ReplaceTemplate(string content, string key, string value)
        //{
        //    return content.Replace(GetKeyFormat(key), value) ;
        //}

        //private string GetRemarkInfo(IDatabase db, IClassMapper mapper)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder(5000);
        //    //sb.AppendLine(@"/*");
        //    //sb.AppendLine(@"Pure Data Backup Exporter");
        //    //sb.AppendLine(@"");
        //    //sb.AppendLine(@"Database Type       : " + db.DatabaseType); 
        //    //sb.AppendLine(@"Database Name       : " +db.DatabaseName);
        //    //sb.AppendLine(@"Table Schema        : "+mapper.SchemaName); 
        //    //sb.AppendLine(@"Table Name          : "+mapper.TableName);
        //    //sb.AppendLine(@"Table Description   : " + mapper.TableDescription);
        //    //sb.AppendLine(@"");
        //    //sb.AppendLine(@"Date: "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    //sb.AppendLine(@"*/");
        //    //sb.AppendLine(@"");

        //    sb.AppendLine(@"-- Pure Data Backup Exporter");
        //    sb.AppendLine(@"-- ");
        //    sb.AppendLine(@"-- Database Type       : " + db.DatabaseType);
        //    sb.AppendLine(@"-- Database Name       : " + db.DatabaseName);
        //    sb.AppendLine(@"-- Table Schema        : " + mapper.SchemaName);
        //    sb.AppendLine(@"-- Table Name          : " + mapper.TableName);
        //    sb.AppendLine(@"-- Table Description   : " + mapper.TableDescription);
        //    sb.AppendLine(@"-- ");
        //    sb.AppendLine(@"-- Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        //    sb.AppendLine(@"-- ");

        //    return sb.ToString();

        //}



    }
}
