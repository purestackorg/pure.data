using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.SqlMap
{
    /// <summary>
    /// 配置文件加载器
    /// </summary>
    public interface ISqlMapLoader : IDisposable
    {
        /// <summary>
        /// 加载配置文件
        /// </summary>
       
        void Load(IDatabase db);


    }
}
