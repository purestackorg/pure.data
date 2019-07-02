using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data.Pooling
{


    public class PooledStringBuilder : PooledObject
    {
        #region Logging

        //private static readonly ILog Log = LogProvider.GetLogger(typeof(PooledStringBuilder));

        #endregion Logging

        /// <summary>
        ///   Builds a pooled string builder.
        /// </summary>
        /// <param name="capacity">The capacity of the string builder.</param>
        public PooledStringBuilder(int capacity)
        {
            StringBuilder = new StringBuilder(capacity);

            OnValidateObject += (ctx) =>
            {
                if (ctx.Direction == PooledObjectDirection.Outbound)
                {
                    // We validate only inbound objects, because when they are in the pool they
                    // cannot change their state.
                    return true;
                }

                var stringBuilderPool = PooledObjectInfo.Handle as IStringBuilderPool;
                if (StringBuilder.Capacity > stringBuilderPool.MaximumStringBuilderCapacity)
                {
                    //if (Log.IsWarnEnabled()) Log.Warn($"[ObjectPool] String builder capacity is {StringBuilder.Capacity}, while maximum allowed capacity is {stringBuilderPool.MaximumStringBuilderCapacity}");
                    return false;
                }

                return true; // Object is valid.
            };

            OnResetState += ClearStringBuilder;

            OnReleaseResource += ClearStringBuilder;
        }

        /// <summary>
        ///   The string builder.
        /// </summary>
        public StringBuilder StringBuilder { get; }

        /// <summary>
        ///   Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => StringBuilder.ToString();

        /// <summary>
        ///   Clears the <see cref="StringBuilder"/> property, using specific methods depending on
        ///   the framework for which ObjectPool has been compiled.
        /// </summary>
        protected void ClearStringBuilder(PooledObject o)
        {
            StringBuilder.Clear();
        }
    }

    public interface IStringBuilderPool : IObjectPool<PooledStringBuilder>
    {
        /// <summary>
        ///   Maximum capacity a <see cref="StringBuilder"/> might have in order to be able to return
        ///   to pool. Defaults to <see cref="StringBuilderPool.DefaultMaximumStringBuilderCapacity"/>.
        /// </summary>
        int MaximumStringBuilderCapacity { get; set; }

        /// <summary>
        ///   Minimum capacity a <see cref="StringBuilder"/> should have when created and this is the
        ///   minimum capacity of all builders stored in the pool. Defaults to <see cref="StringBuilderPool.DefaultMinimumStringBuilderCapacity"/>.
        /// </summary>
        int MinimumStringBuilderCapacity { get; set; }

        /// <summary>
        ///   Returns a pooled string builder using given string as initial value.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.</param>
        /// <returns>A pooled string builder.</returns>
        PooledStringBuilder GetObject(string value);
    }


    public sealed class StringBuilderPool : ObjectPool<PooledStringBuilder>, IStringBuilderPool
    {
        /// <summary>
        ///   Default maximum string builder capacity. Shared by all <see cref="IStringBuilderPool"/>
        ///   instances, defaults to 524288 characters.
        /// </summary>
        public const int DefaultMaximumStringBuilderCapacity = 512 * 1024;

        /// <summary>
        ///   Default minimum string builder capacity. Shared by all <see cref="IStringBuilderPool"/>
        ///   instances, defaults to 4096 characters.
        /// </summary>
        public const int DefaultMinimumStringBuilderCapacity = 4 * 1024;

        /// <summary>
        ///   Backing field for <see cref="MaximumStringBuilderCapacity"/>.
        /// </summary>
        private int _maximumItemCapacity = DefaultMaximumStringBuilderCapacity;

        /// <summary>
        ///   Backing field for <see cref="MinimumStringBuilderCapacity"/>.
        /// </summary>
        private int _minimumItemCapacity = DefaultMinimumStringBuilderCapacity;

        /// <summary>
        ///   Builds the specialized pool.
        /// </summary>
        public StringBuilderPool()
            : base(ObjectPool.DefaultPoolMaximumSize, (Func<PooledStringBuilder>)null)
        {
            FactoryMethod = () => new PooledStringBuilder(MinimumStringBuilderCapacity);
        }

        /// <summary>
        ///   Thread-safe pool instance.
        /// </summary>
        public static IStringBuilderPool Instance { get; } = new StringBuilderPool();

        /// <summary>
        ///   Maximum capacity a <see cref="StringBuilder"/> might have in order to be able to return
        ///   to pool. Defaults to <see cref="DefaultMaximumStringBuilderCapacity"/>.
        /// </summary>
        public int MaximumStringBuilderCapacity
        {
            get { return _maximumItemCapacity; }
            set
            {
                var oldValue = _maximumItemCapacity;
                _maximumItemCapacity = value;
                if (oldValue > value)
                {
                    Clear();
                }
            }
        }

        /// <summary>
        ///   Minimum capacity a <see cref="StringBuilder"/> should have when created and this is the
        ///   minimum capacity of all builders stored in the pool. Defaults to <see cref="DefaultMinimumStringBuilderCapacity"/>.
        /// </summary>
        public int MinimumStringBuilderCapacity
        {
            get { return _minimumItemCapacity; }
            set
            {
                var oldValue = _minimumItemCapacity;
                _minimumItemCapacity = value;
                if (oldValue < value)
                {
                    Clear();
                }
            }
        }

        /// <summary>
        ///   Returns a pooled string builder using given string as initial value.
        /// </summary>
        /// <param name="value">The string used to initialize the value of the instance.</param>
        /// <returns>A pooled string builder.</returns>
        public PooledStringBuilder GetObject(string value)
        {
            var psb = GetObject();
            psb.StringBuilder.Append(value);
            return psb;
        }
    }
}
