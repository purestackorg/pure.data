using Pure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class ConfigHelpers
    {
        public static string GetDefaultConfig(string config)
        {
            if (string.IsNullOrEmpty( config))
            {
                return "PureDataConfiguration.xml";
            }
            else
            {
                return config;
            }

        }

        private static object emptyobj = new object();
        public static dynamic config = emptyobj;
        public static dynamic GetDynamicConfig()
        {
            if (config == emptyobj)
            {
                config = TinyJsonConfig.LoadConfig("CmdConfig.json", null, false);

            }
            return config;
        }

        public static void OpenDir(string outputPath) {
            if (System.IO.Directory.Exists(outputPath))
            {
                string v_OpenFolderPath = outputPath;
                System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
            }
        }
    }
}
