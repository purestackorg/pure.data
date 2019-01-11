using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data
{
    /// <summary>
    /// 数据源类型
    /// </summary>
    public enum DataSourceType
    {
         /// <summary>
        /// 写数据库源
         /// </summary>
        Write,
        /// <summary>
        /// 读数据库源
        /// </summary>
        Read,
        /// <summary>
        /// 可读写数据源
        /// </summary>
        ReadAndWrite,
    }
}
