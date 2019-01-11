

namespace Pure.Data.Pooling.Impl
{
    public class BaseObjectPoolConfig
    {
        /// <summary>
        /// 如果对象池没有数据是否阻断
        /// </summary>
        public const bool DefaultBlockWhenExhausted = true;

        /// <summary>
        ///  默认最大等待时间，默认-1不生效
        /// </summary>
        public const long DefaultMaxWaitMillis = -1L;

        public const BorrowStrategy DefaultBorrowStrategy = BorrowStrategy.LIFO;
        public static long DefaultSoftMinEvictableIdleTimeMillis = -1;
        public static long DefaultMinEvictableIdleTimeMillis = 1000L * 60L * 30L;
        public static int DefaultNumTestsPerEvictionRun = 3;
        public static long DefaultTimeBetweenEvictionRunsMillis = -1L;
        public static bool DefaultTestWhileIdle = false;
        public static bool DefaultTestOnReturn = false;
        public static bool DefaultTestOnBorrow = false;
        public static bool DefaultTestOnCreate = false;


        /// <summary>
        ///     The default value for the {@code maxTotal} configuration attribute.
        /// </summary>
        public const int DEFAULT_MAX_TOTAL = -1;

        /// <summary>
        ///  连接池中可同时连接的最大的连接数，默认-1全部激活不限制
        /// </summary>
        public int MaxTotal { get; set; } = DEFAULT_MAX_TOTAL;

        /// <summary>
        ///  获取对象策略
        /// </summary>
        public BorrowStrategy BorrowStrategy { get; set; } = DefaultBorrowStrategy;

        ///// <summary>
        ///// Get the value for the {@code fairness} configuration attribute for pools created with this configuration instance.
        ///// The current setting of {@code fairness} for this configuration instance
        ///// </summary>
        //public bool Fairness { get; set; } = false;

        /// <summary>
        ///  配置获取连接等待超时的时间,以毫秒为单位，在抛出异常之前，池等待连接被回收的最长时间（当没有可用连接时）。设置为-1表示无限等待。
        ///  最大等待时间，当没有可用连接时，连接池等待连接释放的最大时间，超过该时间限制会抛出异常，如果设置-1表示无限等待（默认为无限，建议调整为60000ms，避免因线程池不够用，而导致请求被无限制挂起）
        /// </summary>
        public long MaxWaitMillis { get; set; } = DefaultMaxWaitMillis;

        /// <summary>
        ///  配置一个连接在池中最小生存的时间，单位是毫秒，默认30分钟
        ///  连接池中连接，在时间段内一直空闲， 被逐出连接池的时间
        /// </summary>
        public long MinEvictableIdleTimeMillis { get; set; } = DefaultMinEvictableIdleTimeMillis;

        /// <summary>
        ///     Get the value for the {@code softMinEvictableIdleTimeMillis}
        ///     configuration attribute for pools created with this configuration
        ///     instance.
        /// </summary>
        public long SoftMinEvictableIdleTimeMillis { get; set; } = DefaultSoftMinEvictableIdleTimeMillis;

        /// <summary>
        ///  代表每次检查链接的数量，建议设置和maxActive一样大，这样每次可以有效检查所有的链接.
        /// </summary>
        public int NumTestsPerEvictionRun { get; set; } = DefaultNumTestsPerEvictionRun;

        /// <summary>
        ///   创建对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnCreate { get; set; } = DefaultTestOnCreate;

        /// <summary>
        ///  取得对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnBorrow { get; set; } = DefaultTestOnBorrow;

        /// <summary>
        /// 返回对象时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestOnReturn { get; set; } = DefaultTestOnReturn;

        /// <summary>
        ///  空闲时是否进行验证，检查对象是否有效，默认为false
        /// </summary>
        public bool TestWhileIdle { get; set; } = DefaultTestWhileIdle;

        /// <summary>
        /// 失效检查线程运行时间间隔，如果小于等于-1，不会启动检查线程，默认-1
        /// </summary>
        public long TimeBetweenEvictionRunsMillis { get; set; } = DefaultTimeBetweenEvictionRunsMillis;

        /// <summary>
        ///     Get the value for the {@code evictionPolicyClassName} configuration
        ///     attribute for pools created with this configuration instance.
        /// </summary>
        public string EvictionPolicyClassName { get; set; }

        /// <summary>
        ///  当池中没有数据时候是否阻断
        /// </summary>
        public bool BlockWhenExhausted { get; set; } = DefaultBlockWhenExhausted;

    }
}