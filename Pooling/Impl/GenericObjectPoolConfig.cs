

namespace Pure.Data.Pooling.Impl
{
    public class GenericObjectPoolConfig : BaseObjectPoolConfig
    {
        /// <summary>
        /// 同一时间可以从池分配的最多连接数量，默认8,设-1为没有限制（对象池中对象最大个数）
        /// 连接池中最大的空闲的连接数，超过的空闲连接将被释放，如果设置为负数表示不限制（默认为8个，maxIdle不能设置太小，因为假如在高负载的情况下，连接的打开时间比关闭的时间快，会引起连接池中idle的个数 上升超过maxIdle，而造成频繁的连接销毁和创建，类似于jvm参数中的Xmx设置)
        /// </summary>
        public int MaxIdle { get; set; } = 8;
        /// <summary>
        /// 池里不会被释放的最多空闲连接数量。,默认0
        /// 连接池中最小的空闲的连接数，低于这个数量会被创建新的连接（默认为0，调整为5，该参数越接近maxIdle，性能越好，因为连接的创建和销毁，都是需要消耗资源的；但是不能太大，因为在机器很空闲的时候，也会创建低于minidle个数的连接，类似于jvm参数中的Xmn设置）
        /// </summary>
        public int MinIdle { get; set; } = 0;
    }
}