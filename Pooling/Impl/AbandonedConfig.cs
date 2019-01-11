

using System.IO;

namespace Pure.Data.Pooling.Impl
{
    /// <summary>
    ///     Configuration settings for abandoned object removal.
    /// </summary>
    public class AbandonedConfig
    {
        /// <summary>
        ///     Flag to remove abandoned objects if they exceed the
        ///     removeAbandonedTimeout when borrowObject is invoked.
        ///     The default value is false
        ///     If set to true, abandoned objects are removed by borrowObject if
        ///     there are fewer than 2 idle objects available in the pool and
        ///     <code>getNumActive() > getMaxTotal() - 3</code>
        /// </summary>
        public bool RemoveAbandonedOnBorrow { get; set; } = false;

        /// <summary>
        ///     Flag to remove abandoned objects if they exceed the
        ///     removeAbandonedTimeout when pool maintenance (the "evictor") runs
        ///     The default value is false.
        ///     If set to true, abandoned objects are removed by the pool
        ///     maintenance thread when it runs.  This setting has no effect
        ///     unless maintenance is enabled by setting
        ///     {@link GenericObjectPool#getTimeBetweenEvictionRunsMillis() timeBetweenEvictionRunsMillis}
        ///     to a positive number.
        /// </summary>
        public bool RemoveAbandonedOnMaintenance { get; set; } = false;

        /// <summary>
        ///  自动回收超时时间(以秒数为单位)，默认300 
        ///  超过时间限制，回收没有用(废弃)的连接（建议调整为180）
        /// </summary>
        public int RemoveAbandonedTimeout { get; set; } = 300;

        /// <summary>
        ///    设置在自动回收超时连接的时候打印连接的超时错误
        /// </summary>
        public bool LogAbandoned { get; set; } = false;

        /// <summary>
        ///   打印输出日志的代理
        /// </summary>
        public TextWriter LogWriter { get; set; }


        /// <summary>
        /// 每次在池化对象上调用方法并保留时，都会保存堆栈跟踪信息 
        /// </summary>
        public bool UseUsageTracking { get; set; } = false;
    }
}