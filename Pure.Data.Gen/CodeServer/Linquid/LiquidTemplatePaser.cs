 
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks; 
//using System.IO;
//using DotLiquid;

//namespace Pure.Data.Gen.CodeServer.RazorPaser
//{
//    //https://blog.csdn.net/Bi_ZhiAn/article/details/72547958
//    //https://www.cnblogs.com/zkweb/p/5864794.html

//    public class LiquidHelper
//    {
//        private static Dictionary<string, Template> _map = new Dictionary<string, Template>();

//        public static Template CreateOrGet(string template, string templateKey) {
//            Template tmp = null;
//            if (_map.ContainsKey(templateKey))
//            {
//                tmp = _map[templateKey];
//            }
//            else
//            {
//                tmp = Template.Parse(template);
//                _map.Add(templateKey, tmp);
//            }
             
//            return tmp;
//        }

//        public static void Remove(string templateKey) {
//            if (_map.ContainsKey(templateKey))
//            {
//                _map.Remove(templateKey);
//            }
//        }

//    }
//    public class LiquidTemplatePaser : ITemplateParser, IDisposable
//    {
        
//        public LiquidTemplatePaser(IParserConfig parseConfig)
//        {
//            // 在原有的代码下添加
//            Template.RegisterFilter(typeof(DotliquidCustomFilter));
//            Template.RegisterSafeType(typeof(Column), new string[] { });
//            Template.RegisterSafeType(typeof(OutputContext), new string[] { }); 
//            Template.RegisterSafeType(typeof(Table), new string[] { });
//            //Template.RegisterSafeType(typeof(Column), Hash.FromAnonymousObject);
//            //Template.RegisterSafeType(typeof(OutputContext), Hash.FromAnonymousObject);
//        }

//        private void WriteOrAppendFile(string outputFileName, string content, string encoding = "utf-8", bool append = false)
//        {
//            if (append)
//            {
//                 FileHelper.AppendText(outputFileName, content, System.Text.Encoding.GetEncoding(encoding));

//            }
//            else
//            {
//                FileHelper.WriteText(outputFileName, content, System.Text.Encoding.GetEncoding(encoding));

//            }
//        }

//        public string Parse<T>(string template, string templateKey, T data)
//        {
//            string result = "";
//            Type type = typeof(T);


//            var otmp = LiquidHelper.CreateOrGet(template, templateKey);
//            result = otmp.Render(Hash.FromAnonymousObject(data));

//            return result;

          
//        }

 
//        public string Parse(string template, string templateKey, object data  )
//        {
//            string result = "";

//            var otmp = LiquidHelper.CreateOrGet(template,  templateKey);
//            result =otmp.Render(Hash.FromAnonymousObject(data));
            
//            return result;
//        }

//        public string ParseFile(string filename, string templateKey, object data)
//        {
//            string template =FileHelper.ReadFileToString(filename);
//            return Parse(template, templateKey, data);
//        }

//        public string ParseAndOutput(string outputFileName, string template, string templateKey, object data, string encoding = "utf-8", bool append = false)
//        {
//            string content = Parse(template, templateKey, data);
//            OutputResult(outputFileName, content, encoding, append);
//            return content;
//        }

//        public string ParseAndOutputFile(string outputFileName, string filename, string templateKey, object data, string encoding = "utf-8", bool append = false )
//        {
//            string content = ParseFile(filename, templateKey, data);
//            OutputResult(outputFileName, content, encoding, append);
//            return content;
//        }

//        public void OutputResult(string outputFileName, string content, string encoding = "utf-8", bool append = false)
//        {
//            WriteOrAppendFile(outputFileName, content, encoding, append);
//        }


//        public string ParseAndOutput<T>(string outputFileName, string template, string templateKey, T data, string encoding = "utf-8", bool append = false )
//        {
//            string content = Parse<T>(template, templateKey, data);
//            OutputResult(outputFileName, content, encoding, append);
//            return content;
//        }

//        public void Dispose()
//        {
//           //
//        }


//        public bool Reset(string templateKey)
//        {
//            //bool result = false;
//            //var key = Engine.Razor.GetKey(templateKey);
//            //if (key != null)
//            //{
//            //    templateManager.RemoveDynamic(key);
//            //    result = DefaultCachingProvider.Remove(key);

//            //}
//            //return result;
//            LiquidHelper.Remove(templateKey);
//            return true;
//        }
//    }

  
//}
