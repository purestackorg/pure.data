using System;
using System.Collections.Generic;

namespace Pure.Data.Gen
{
    /// <summary>
    /// 数据结果输出上下文
    /// </summary>
    public class OutputContext
    {
        public string Key { get; set; } 
        /// <summary>
        /// 获取或者设置的输出文件名称
        /// </summary>
        public string OutputFileName { get; set; }
        public string ModelName { get; set; }
        /// <summary>
        /// 获取实际生成文件路径
        /// </summary>
        public string RealOutputFileName { get { return DbLoader.GetOutputFileName(ProjectConfig, GeneraterConfig, OutputFileName, ModelName); } }

        /// <summary>
        /// 生成配置（模板信息）
        /// </summary>
        public GeneraterConfig GeneraterConfig { get; set; }
        /// <summary>
        /// 项目工程配置
        /// </summary>
        public ProjectConfig ProjectConfig { get; set; }
        /// <summary>
        /// 解析配置
        /// </summary>
        public IParserConfig ParserConfig { get; set; }
         
        /// <summary>
        /// 当前项目所有表数据
        /// </summary>
      //  public List<IClassMapper> Tables { get; set; }
         public List<Table> Tables { get; set; }
        ///// <summary>
        ///// 所有实体表映射
        ///// </summary>
        //public System.Collections.Concurrent.ConcurrentDictionary<Type, IClassMapper> Mappers { get; set; }
        public OutputContext()
        {
            _Properties = new Dictionary<string, object>();
        }

        #region 拓展
       
        private IDictionary<string, object> _Properties = null;
        /// <summary>
        /// 附加属性
        /// </summary>
        public IDictionary<string, object> Properties { get { return _Properties; } }

        #endregion

    }
}
