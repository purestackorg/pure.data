using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pure.Data.MasterSlave
{
    /// <summary>
    /// 数据源管理器
    /// </summary>
    public interface IDataSourceManager
    { 
        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="sourceChoice"></param>
        /// <returns></returns>
        IDataSource GetDataSource(DataSourceType sourceChoice);
    }


    /// <summary>
    /// 数据源管理器
    /// </summary>
    public class DataSourceManager : IDataSourceManager
    {
        public IDatabaseConfig Config { get; private set; }
        /// <summary>
        /// 权重筛选器
        /// </summary>
        private WeightFilter<IDataSource> weightFilter = new WeightFilter<IDataSource>();
        public DataSourceManager(IDatabaseConfig config)
        {
            Config = config;
        }
        public IDataSource GetDataSource(DataSourceType sourceChoice)
        {
            IDataSource choiceDataSource = null;
            var DataSources = Config.DataSources.Where(p => p.Type == sourceChoice).ToList();
            if (sourceChoice != null
                && DataSources.Count > 0
                )
            {
                var seekList = DataSources.Select(readDataSource => new WeightFilter<IDataSource>.WeightSource
                {
                    Source = readDataSource,
                    Weight = readDataSource.Weight
                });
                choiceDataSource = weightFilter.Elect(seekList).Source;
            }
             
            return choiceDataSource;
        }
    }
}
