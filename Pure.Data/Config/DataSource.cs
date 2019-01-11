using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    /// <summary>
    /// 数据源
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// 数据源链接字符串
        /// </summary>
        String ConnectionString { get; set; }
        String Provider { get; set; }

        String ParameterPrefix { get; set; }
        DataSourceType Type { get; set; }

        /// <summary>
        /// 被选中的权重
        /// </summary>
        int Weight { get; set; }

        bool IsMaster { get; set; }

    }

 
     
}
