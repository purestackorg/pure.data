using Pure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureGen
{
    public class SqlmapHelpers
    {
        public static void Export(SqlMapOptions options)
        {
            LogHelpers.LogStatic($"Sqlmap Exporter query {options.QueryPath} output tot {options.OutPutPath}");

            var sqlmaps = Directory.GetFiles(options.QueryPath, "*.xml", SearchOption.AllDirectories);
            if (sqlmaps.Count() > 0)
            {
                LogHelpers.LogStatic($"query files: {sqlmaps.Length} "); 
            }

            if (!Directory.Exists(options.OutPutPath))
            {
                Directory.CreateDirectory(options.OutPutPath);
            }

            DirectoryInfo destDirectory = new DirectoryInfo(options.OutPutPath);

            foreach (var item in sqlmaps)
            {
                CopyFile(item, destDirectory);
            }

                LogHelpers.LogStatic($"copy files successfully. ");

        }
        private static void CopyFile(string srcFile, DirectoryInfo destDirectory)
        {
            string fileName = Path.GetFileName(srcFile);
            if (!File.Exists(srcFile))
            {
                return;
            }
              
            File.Copy(srcFile, destDirectory.FullName + @"\" + fileName, true);

        }
    }
}
