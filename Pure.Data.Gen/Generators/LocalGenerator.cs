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
            parser = DbLoader.CreateTemplateEngine(parseConfig);// new RazorTemplatePaser(parseConfig);
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

        private void ParseAndOutputByTables(IDatabase database, ProjectConfig config, List<string> filterTables = null, List<string> withoutTables = null)
        {

            try
            { 
               
                config.Database = database;
                List<Table> tables = null;



                tables = DbLoader.GetTableInfos(database, config, filterTables, withoutTables);
 


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
         
        #endregion



        public void Run(IDatabase database, ProjectConfig _config, List<string> filterTables = null, List<string> withoutTables = null)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {

                OnLogReport(string.Format("Project - [{0}]", System.IO.Path.GetFileName(_config.Name)));
                OnLogReport("Generate Begin >>>");
           //     OnLogReport(_config.ConnectionString);


                ParseAndOutputByTables(  database, _config, filterTables, withoutTables);


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
