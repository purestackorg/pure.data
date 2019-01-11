//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pure.Data.SqlMap
//{
//    public class SqlMapLoaderOption
//    {
//        public SqlMapLoaderOption()
//        { 
//            WatchSqlMapInterval = 5000;
//            SqlMapFilePaths = new List<string>();
//        }

//        public string ExecuteParameterPrefix { get; set; }
//        public string ParameterPrefix { get; set; }
//        public string ParameterSuffix { get; set; }

//        /// <summary>
//        /// 是否监听Sqlmap配置文件
//        /// </summary>
//        public bool IsWatchSqlMapFile { get; set; }
//        /// <summary>
//        /// 间隔监听Sqlmap配置文件时间（毫秒）
//        /// </summary>
//        public int WatchSqlMapInterval { get; set; }
//        /// <summary>
//        /// sql map配置路径
//        /// </summary>
//        public List<string> SqlMapFilePaths { get; set; }
//        /// <summary>
//        /// 是否重新刷新
//        /// </summary>
//        public bool Refresh { get; set; }

//        /// <summary>
//        /// sql map所在目录路径
//        /// </summary>
//        public List<string> SqlMapDirPaths { get; set; }

//        /// <summary>
//        /// Sql Map  Scope前缀命名空间
//        /// </summary>
//        public string NameSpacePrefix { get; set; }

//        /// <summary>
//        /// 输出Sql map加载日志代理
//        /// </summary>
//        public Action<string> OutputSqlMapLoaderLogs { get; set; } 

//        public void Log(string log) {
//            if (OutputSqlMapLoaderLogs != null)
//            {
//                OutputSqlMapLoaderLogs(log);
//            }
            
//            ;}
//    }
//}
