using System;
using System.IO;

namespace Pure.Data.SqlMap
{
    public class FileLoader
    {

        public static string GetPath(String filePath)
        {
            bool isAbsolute = filePath.IndexOf(":") > 0;
            if (!isAbsolute)
            {
                filePath = Path.Combine(PathHelper.GetBaseDirectory(), filePath);
            }
            return filePath;
        }
        public static FileInfo GetInfo(String filePath)
        {
            filePath = GetPath(filePath);
            return new FileInfo(filePath);
        }

       

        public static Stream Load(String filePath)
        {
            var fileInfo = GetInfo(filePath);
            return Load(fileInfo);
        }
        private static int maxLoadCount = 0;
        public static Stream Load(FileInfo fileInfo)
        {
            if (!File.Exists(fileInfo.FullName))
            {
                throw new Exception("Loading [" + fileInfo.FullName + "] not exist ! ");

            }
            while (true)
            {
                try
                {
                    
                    var st = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    return st;
                }
                catch (Exception ex)
                {
                    maxLoadCount++;
                    if (maxLoadCount == 30)
                    {
                        maxLoadCount = 0;
                        throw new Exception("Loading ["+ fileInfo.FullName + "] error :"+ex);
                    }
                    //db.Debug(string.Format("Output file {0} not yet ready ({1})", filePath, ex.Message));
                    //Console.WriteLine(string.Format("Output file {0} not yet ready ({1})", filePath, ex.Message));
                    System.Threading.Thread.Sleep(300);
                }
            }
        }

        public static string LoadText(String filePath, IDatabase db)
        {

            while (true)
            {
                try
                {
                    var fileInfo = GetInfo(filePath);

                    string text = System.IO.File.ReadAllText(fileInfo.FullName);

                    return text;
                     
                }
                catch (Exception ex)
                {
                    db.Debug(string.Format("Output file {0} not yet ready ({1})", filePath, ex.Message));

                    //Console.WriteLine(string.Format("Output file {0} not yet ready ({1})", filePath, ex.Message));
                    System.Threading.Thread.Sleep(300);

                }
            }
            
        }
    }
}
