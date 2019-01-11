using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pure.Data.Gen.CodeServer.RazorPaser
{
    public class RazorTemplatePaser : ITemplateParser, IDisposable
    {
        TemplateServiceConfiguration config;
        DelegateTemplateManager templateManager;
        public RazorTemplatePaser(IParserConfig parseConfig)
        {
            config = new TemplateServiceConfiguration();
            config.Debug = parseConfig.EnableDebug;
            //config.TemporaryDirectory 
            config.Language = Language.CSharp;
            config.DisableTempFileLocking = true;//as well. This will work in any AppDomain (including the default one)
            if (config.Debug)
            {
                config.DisableTempFileLocking = false;
            }
            //config.CachingProvider = new DefaultCachingProvider(t => { });
            config.CachingProvider = new DefaultCachingProvider();
            config.EncodedStringFactory = new RazorEngine.Text.RawStringFactory(); // Raw string encoding.
            config.ReferenceResolver = new LocalReferenceResolver();
            //DelegateTemplateManager: (default) Used as the default for historical reasons, easy solution when using dynamic template razor strings.
            //ResolvePathTemplateManager: Used to resolve templates from a given list of directory paths. Doesn't support adding templates dynamically via string. You can use a full path instead of a template name.
            //EmbeddedResourceTemplateManager: Used to resolve templates from assembly embedded resources. Uses Assembly.GetManifestResourceStream(Type, string) to load the template based on the type provided.
            //WatchingResolvePathTemplateManager: Same as ResolvePathTemplateManager but watches the filesystem and invalidates the cache. Note that this introduces a memory leak to your application, so only use this is you have an AppDomain recycle strategy in place or for debugging purposes.
            templateManager = new DelegateTemplateManager();
            config.TemplateManager = templateManager;
            //config.EncodedStringFactory = new RazorEngine.Text.HtmlEncodedStringFactory(); // Html encoding.
            if (parseConfig.EnableSandbox)
            {

                var service = IsolatedRazorEngineService.Create(new SandBoxConfigCreator(config), SandBox.SandboxCreator);
                Engine.Razor = service;

            }
            else
            {
                var service = RazorEngineService.Create(config);
                Engine.Razor = service;
            }


        }

        private void WriteOrAppendFile(string outputFileName, string content, string encoding = "utf-8", bool append = false)
        {
            if (append)
            {
                FileHelper.AppendText(outputFileName, content, System.Text.Encoding.GetEncoding(encoding));

            }
            else
            {
                FileHelper.WriteText(outputFileName, content, System.Text.Encoding.GetEncoding(encoding));

            }
        }


        public string Parse<T>(string template, string templateKey, T data)
        {
            string result = "";
            Type type = typeof(T);
            if (Engine.Razor.IsTemplateCached(templateKey, type))
            {
                result = Engine.Razor.Run(templateKey, type, data);

            }
            else
            {
                result = Engine.Razor.RunCompile(template, templateKey, type, data);
            }
            return result;
        }


        public string Parse(string template, string templateKey, object data)
        {
            string result = "";
            Type type = null;

            if (Engine.Razor.IsTemplateCached(templateKey, type))
            {
                result = Engine.Razor.Run(templateKey, type, data);

            }
            else
            {
                result = Engine.Razor.RunCompile(templateKey, type, data);
            }
            return result;
        }

        public string ParseFile(string filename, string templateKey, object data)
        {
            string template = FileHelper.ReadFileToString(filename);
            return Parse(template, templateKey, data);
        }

        public string ParseAndOutput(string outputFileName, string template, string templateKey, object data, string encoding = "utf-8", bool append = false)
        {
            string content = Parse(template, templateKey, data);
            OutputResult(outputFileName, content, encoding, append);
            return content;
        }

        public string ParseAndOutputFile(string outputFileName, string filename, string templateKey, object data, string encoding = "utf-8", bool append = false)
        {
            string content = ParseFile(filename, templateKey, data);
            OutputResult(outputFileName, content, encoding, append);
            return content;
        }

        public void OutputResult(string outputFileName, string content, string encoding = "utf-8", bool append = false)
        {
            WriteOrAppendFile(outputFileName, content, encoding, append);
        }


        public string ParseAndOutput<T>(string outputFileName, string template, string templateKey, T data, string encoding = "utf-8", bool append = false)
        {
            string content = Parse<T>(template, templateKey, data);
            OutputResult(outputFileName, content, encoding, append);
            return content;
        }

        public void Dispose()
        {
            if (Engine.Razor != null)
            {
                Engine.Razor.Dispose();
                Engine.Razor = null;
            }
        }


        public bool Reset(string templateKey)
        {
            bool result = false;
            var key = Engine.Razor.GetKey(templateKey);
            if (key != null)
            {
                templateManager.RemoveDynamic(key);
                result = DefaultCachingProvider.Remove(key);

            }
            return result;
        }
    }


}
