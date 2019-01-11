using System;
using System.IO;

namespace Pure.Data.Gen
{
    public class DefaultParserConfig : IParserConfig
    {


        public bool EnableDebug
        {
            get { return true; }
        }

        public bool EnableSandbox
        {
            get { return false; }
        }


        public bool EnableLoadDataFunction
        {
            get { return true; }
        }

        public bool EnableLoadDataSchema
        {
            get { return true; }
        }

        public bool EnableLoadDataView
        {
            get { return true; }
        }


        public bool EnableLoadDataIndex
        {
            get { return true; }
        }


        public string TemplateRootDir
        {
#if NET45
            get {
             
                 return AppDomain.CurrentDomain.BaseDirectory;
            }
#else
            get { return System.IO.Directory.GetCurrentDirectory(); }


#endif


        }


        public string TemplateExt
        {
            get { return ".cshtml"; }
        }
    }
}
