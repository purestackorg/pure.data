using System;
using System.IO;

namespace Pure.Data
{
    public static class PathHelper
    {
        public static string BaseDirectory { get; set; }
          static PathHelper()
        {
#if NET45
              var baseDirectory = AppDomain.CurrentDomain.BaseDirectory; //AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            //if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPath == null)
            //    baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            BaseDirectory =baseDirectory;
#else
            BaseDirectory = Directory.GetCurrentDirectory();// AppContext.BaseDirectory;// Directory.GetCurrentDirectory();//

#endif
        }
        public static string CombineWithBaseDirectory(string path)
        {
            return System.IO.Path.Combine( BaseDirectory, path);
        }
        /// <summary>
        /// 获取程序根目录
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory()
        {
            return BaseDirectory;
        }
        /// <summary>
        /// 获取dll存放路径
        /// </summary>
        /// <returns></returns>
        public static string GetAppExecuteDirectory()
        {
#if NET45
            var path =  AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (path == null)
            path = AppDomain.CurrentDomain.BaseDirectory;
            return path;

#else
            return AppContext.BaseDirectory; 
#endif

        }
        public static string GetRootedPath(string path)
        {
            string rootedPath = path ?? string.Empty;
            if (!Path.IsPathRooted(rootedPath))
            {
                if (string.IsNullOrEmpty(BaseDirectory))
                    throw new ArgumentNullException("请先设置BaseDirectory属性");
                rootedPath = Path.Combine(BaseDirectory, rootedPath);
            }
            string directory = Path.GetDirectoryName(rootedPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return rootedPath;
        }
    }
}
