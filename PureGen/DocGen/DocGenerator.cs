using Pure.Data;
using Pure.Data.Gen;
using Pure.Data.Gen.CodeServer.RazorPaser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class DocGenerator
    {
        public static string Process(DocOptions options) {
            string result = "";

            try
            {
                var config = ConfigHelpers.GetDefaultConfig(options.Config);
                IDatabase database = new Database(config, LogHelpers.LogStatic, option => {
                    //option.CodeGenProjectName = projectName;
                    //option.CodeGenNameSpace = nameSpace;
                    //option.CodeGenTableFilter = tablePrefixFilter;
                    //option.AutoMigrateOnContainTable = onlyTable;
                });


                string type = options.Type;

                var projectConfig = DbLoader.ConvertDatabaseConfigToProjectConfig(database);
                LogHelpers.LogStatic(projectConfig.ToString());

                string msg = "";
                var tables = DbLoader.GetTableInfos(database, projectConfig,null, null, out msg);
                LogHelpers.LogStatic(msg);


                string docName = projectConfig.DatabaseName;
                OutputContext _OutputContext = new OutputContext();
                _OutputContext.Tables = tables;
                // context.Mappers = mapperDict;
                _OutputContext.ProjectConfig = projectConfig;
                //context.ParserConfig = _parseConfig;
                string FileName = docName + "数据库设计文档." + type;
                string RealOutputFileName = System.IO.Path.Combine(DbLoader.GetProjectDirectory(projectConfig), FileName);

                if (type == "html")
                {
                    string templatePath = DbLoader.MapPath("~/DocGen/dicthtml.cshtml");
                    if (System.IO.File.Exists(templatePath))
                    {
                        string tempalteContent = FileHelper.ReadFile(templatePath);
                        string key = "exportdicthtml";

                        var templateEngine = DbLoader.CreateTemplateEngine();
                        var generateResult = templateEngine.Parse<OutputContext>(tempalteContent, key, _OutputContext);

                        string content = generateResult;
                        templateEngine.OutputResult(RealOutputFileName, content);
                       //return File(RealOutputFileName, "application/zip-x-compressed", FileName);
                    }
                }
                else
                {
                    //Other types 
                    string tmppath = DbLoader.MapPath("~/DocGen/dict.dot");// System.IO.Path.Combine(path, "Template", "template.dot");
                    Aspose.Words.Document doc = new Aspose.Words.Document(tmppath); //载入模板
                    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);


                    string dbName = docName;
                    if (string.IsNullOrEmpty(dbName))
                    {
                        dbName = _OutputContext.ProjectConfig.Name;
                    }
                    doc.Range.Replace("{$.DBName}", dbName, false, false);

                    Aspose.Words.Tables.Table tabletemp = (Aspose.Words.Tables.Table)doc.GetChild(Aspose.Words.NodeType.Table, 0, true);
                    Aspose.Words.Tables.Table tableClone = (Aspose.Words.Tables.Table)tabletemp.Clone(true);

                    for (int k = 0; k < _OutputContext.Tables.Count; k++)
                    {
                        Aspose.Words.Tables.Table table = (Aspose.Words.Tables.Table)doc.GetChild(Aspose.Words.NodeType.Table, k, true);

                        var tb = _OutputContext.Tables[k];

                        //添加表头标题
                        var par = table.ParentNode.InsertBefore(new Aspose.Words.Paragraph(doc), table);
                        builder.MoveTo(par);
                        builder.ParagraphFormat.StyleIdentifier = Aspose.Words.StyleIdentifier.Heading1;
                        builder.Font.Size = 11;
                        builder.Write(tb.Comment + "(" + tb.Name + ")");


                        Aspose.Words.Tables.Cell cellh = table.FirstRow.Cells[1];
                        cellh.RemoveAllChildren();
                        builder.MoveToCell(k, 0, 1, 0);
                        builder.Write(tb.Name);
                        Aspose.Words.Tables.Cell cellh3 = table.FirstRow.Cells[3];
                        cellh3.RemoveAllChildren();
                        builder.MoveToCell(k, 0, 3, 0);
                        builder.Write(tb.Comment != null ? tb.Comment : "");


                        for (int i = 0; i < tb.Columns.Count; i++)
                        {
                            for (int j = 0; j < table.LastRow.Cells.Count; j++)
                            {
                                Aspose.Words.Tables.Cell cell = table.LastRow.Cells[j];
                                cell.RemoveAllChildren();
                                builder.MoveToCell(k, table.Rows.Count - 1, j, 0);
                                switch (j)
                                {
                                    case 0:
                                        builder.Write(tb.Columns[i].Comment != null ? tb.Columns[i].Comment : "");
                                        break;
                                    case 1:
                                        builder.Write(tb.Columns[i].Name);

                                        break;
                                    case 2:
                                        builder.Write(tb.Columns[i].RawType);

                                        break;
                                    case 3:
                                        builder.Write(tb.Columns[i].Length.ToString());

                                        break;
                                    case 4:
                                        builder.Write(tb.Columns[i].Scale.ToString());

                                        break;
                                    case 5:
                                        builder.Write(tb.Columns[i].IsPK ? "Y" : "");

                                        break;
                                    case 6:
                                        builder.Write(tb.Columns[i].IsNullable ? "Y" : "");
                                        break;
                                    case 7:
                                        builder.Write(tb.Columns[i].IsAutoIncrement ? "Y" : "");
                                        break;
                                    case 8:
                                        builder.Write(tb.Columns[i].DefaultValue == null ? "" : tb.Columns[i].DefaultValue.ToString());

                                        break;
                                    default:
                                        break;
                                }

                            }
                            if (i < tb.Columns.Count - 1)
                            {
                                Aspose.Words.Tables.Row clonedRow = (Aspose.Words.Tables.Row)table.LastRow.Clone(true);
                                table.AppendChild(clonedRow);
                            }
                        }
                        if (k < _OutputContext.Tables.Count - 1)
                        {
                            Aspose.Words.Tables.Table tbClone = (Aspose.Words.Tables.Table)tableClone.Clone(true);
                            table.ParentNode.InsertAfter(tbClone, table);
                        }
                    }


                    doc.UpdateFields();
                    doc.Range.Replace("{$.NowDateTime}", DateTime.Now.ToString(), false, false);
                    Aspose.Words.SaveFormat sformat = Aspose.Words.SaveFormat.Doc;
                    switch (type)
                    {
                        case "doc":
                            sformat = Aspose.Words.SaveFormat.Doc;
                            break;
                        case "docx":
                            sformat = Aspose.Words.SaveFormat.Docx;
                            break;
                        //case "html":
                        //    sformat = Aspose.Words.SaveFormat.Html;
                        //    break;
                        case "odt":
                            sformat = Aspose.Words.SaveFormat.Odt;
                            break;
                        case "pdf":
                            sformat = Aspose.Words.SaveFormat.Pdf;
                            break;
                        case "png":
                            sformat = Aspose.Words.SaveFormat.Png;
                            break;
                        case "tiff":
                            sformat = Aspose.Words.SaveFormat.Tiff;
                            break;
                        case "text":
                            sformat = Aspose.Words.SaveFormat.Text;
                            break; 
                        case "epub":
                            sformat = Aspose.Words.SaveFormat.Epub;
                            break;
                        default:
                            break;
                    }
                    doc.Save(RealOutputFileName, sformat);

                    //return File(RealOutputFileName, "application/zip-x-compressed", FileName);
                }


                if (options.Zip)
                {
                    var zipDir = RealOutputFileName;
                    string zipedName = docName + "-" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                    string zipOutFileName = System.IO.Path.Combine(DbLoader.GetDataDirectory(), zipedName);
                    //ZipHelper.ZipDir(zipDir, zipOutFileName, 9);
                    //ZipHelper.ZipManyFilesOrDictorys(zipDir, zipOutFileName, "");
                    ZipHelper.ZipFile(zipDir, zipOutFileName);

                    RealOutputFileName = zipOutFileName;
                }

                LogHelpers.LogStatic("生成数据库字典成功："+RealOutputFileName);
                ConfigHelpers.OpenDir(RealOutputFileName);

            }
            catch (Exception ex)
            {
                LogHelpers.LogStatic("DocGenerator 生成数据库字典出错！", ex, Pure.Data.MessageType.Error); 
            }
           

            return result;
        }


    }
}
