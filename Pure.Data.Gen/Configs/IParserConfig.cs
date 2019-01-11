using System;

namespace Pure.Data.Gen
{
    public interface IParserConfig
    {

        /// <summary>
        /// 是否启用调试1
        /// </summary>
        bool EnableDebug
        {
            get;
        }
        /// <summary>
        /// 是否启用沙盒模式
        /// </summary>
        bool EnableSandbox
        {
            get;
        }
        /// <summary>
        /// 是否加载函数/存储过程
        /// </summary>
        bool EnableLoadDataFunction
        {
            get;
        }
        /// <summary>
        /// 是否加载数据库用户
        /// </summary>
        bool EnableLoadDataSchema
        {
            get;
        }
        /// <summary>
        /// 是否加载视图
        /// </summary>
        bool EnableLoadDataView
        {
            get;
        }
        /// <summary>
        /// 索引
        /// </summary>
        bool EnableLoadDataIndex
        {
            get;
        }
        /// <summary>
        /// 模板跟目录
        /// </summary>
        string TemplateRootDir
        {
            get;
        }
        /// <summary>
        /// 模板后缀
        /// </summary>
        string TemplateExt
        {
            get;
        }
    }
}
