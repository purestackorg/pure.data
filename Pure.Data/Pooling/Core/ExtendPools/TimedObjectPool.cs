using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data.Pooling
{
    public interface ITimedObjectPool<T> : IObjectPool<T>
       where T : PooledObject
    {
        /// <summary>
        ///   When pooled objects have not been used for a time greater than <see cref="Timeout"/>,
        ///   then they will be destroyed by a cleaning task.
        /// </summary>
        TimeSpan Timeout { get; set; }
    }

    public class TimedObjectPool<T> : ObjectPool<T>, ITimedObjectPool<T>
       where T : PooledObject
    {
        #region Fields

        /// <summary>
        ///   Backing field for <see cref="Timeout"/>.
        /// </summary>
        private TimeSpan _timeout;

        #endregion Fields

        #region C'tor and Initialization code

        /// <summary>
        ///   Initializes a new timed pool with default settings and specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout of each pooled object.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public TimedObjectPool(TimeSpan timeout)
            : this(ObjectPool.DefaultPoolMaximumSize, null, timeout)
        {
        }

        /// <summary>
        ///   Initializes a new timed pool with specified maximum pool size and timeout.
        /// </summary>
        /// <param name="maximumPoolSize">The maximum pool size limit.</param>
        /// <param name="timeout">The timeout of each pooled object.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="maximumPoolSize"/> is less than or equal to zero. <paramref
        ///   name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public TimedObjectPool(int maximumPoolSize, TimeSpan timeout)
            : this(maximumPoolSize, null, timeout)
        {
        }

        /// <summary>
        ///   Initializes a new timed pool with specified factory method and timeout.
        /// </summary>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        /// <param name="timeout">The timeout of each pooled object.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public TimedObjectPool(Func<T> factoryMethod, TimeSpan timeout)
            : this(ObjectPool.DefaultPoolMaximumSize, factoryMethod, timeout)
        {
        }

        /// <summary>
        ///   Initializes a new timed pool with specified factory method, maximum size and timeout.
        /// </summary>
        /// <param name="maximumPoolSize">The maximum pool size limit.</param>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        /// <param name="timeout">The timeout of each pooled object.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="maximumPoolSize"/> is less than or equal to zero. <paramref
        ///   name="timeout"/> is less than or equal to <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public TimedObjectPool(int maximumPoolSize, Func<T> factoryMethod, TimeSpan timeout)
            : base(maximumPoolSize, factoryMethod, new EvictionSettings { Enabled = true, Delay = timeout, Period = timeout }, null)
        {
            // Preconditions
            if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout), ErrorMessages.NegativeOrZeroTimeout);

            // Assigning properties.
            _timeout = timeout;
        }

        #endregion C'tor and Initialization code

        #region Public Properties

        /// <summary>
        ///   When pooled objects have not been used for a time greater than <see cref="Timeout"/>,
        ///   then they will be destroyed by a cleaning task.
        /// </summary>
        public TimeSpan Timeout
        {
            get => _timeout;
            set
            {
                StartEvictor(new EvictionSettings { Enabled = true, Delay = value, Period = value });
                _timeout = value;
            }
        }

        #endregion Public Properties

        #region Core Methods

        /// <summary>
        ///   Creates a new pooled object, initializing its info.
        /// </summary>
        /// <returns>A new pooled object.</returns>
        protected override T CreatePooledObject()
        {
            var pooledObject = base.CreatePooledObject();

            // Register an handler which records the time at which the object returned to the pool.
            pooledObject.OnResetState += () =>
            {
                pooledObject.PooledObjectInfo.Payload = DateTime.UtcNow;
            };

            // Register an handler which validates pooled objects timeout.
            pooledObject.OnValidateObject += (ctx) =>
            {
                // An item which have been last used before following threshold will be destroyed.
                var threshold = DateTime.UtcNow - _timeout;
                return !(ctx.PooledObjectInfo.Payload is DateTime lastUsage && lastUsage < threshold);
            };

            return pooledObject;
        }

        /// <summary>
        ///   Creates a new pooled object, initializing its info.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="continueOnCapturedContext">
        ///   Whether async calls should continue on a captured synchronization context.
        /// </param>
        /// <returns>A new pooled object.</returns>
        protected override async Task<T> CreatePooledObjectAsync(
            CancellationToken cancellationToken = default(CancellationToken),
            bool continueOnCapturedContext = default(bool))
        {
            var pooledObject = await base.CreatePooledObjectAsync(cancellationToken, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);

            // Register an handler which records the time at which the object returned to the pool.
            pooledObject.OnResetState += () =>
            {
                pooledObject.PooledObjectInfo.Payload = DateTime.UtcNow;
            };

            // Register an handler which validates pooled objects timeout.
            pooledObject.OnValidateObject += (ctx) =>
            {
                // An item which have been last used before following threshold will be destroyed.
                var threshold = DateTime.UtcNow - _timeout;
                return !(ctx.PooledObjectInfo.Payload is DateTime lastUsage && lastUsage < threshold);
            };

            return pooledObject;
        }

        #endregion Core Methods
    }
}
