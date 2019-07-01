using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
namespace Pure.Data.Pooling
{

    public class PooledMemoryStream : PooledObject
    {
        #region Logging

        //private static readonly ILog Log = LogProvider.GetLogger(typeof(PooledMemoryStream));

        #endregion Logging

        /// <summary>
        ///   The tracked memory stream.
        /// </summary>
        private readonly TrackedMemoryStream _trackedMemoryStream;
        private OutputActionDelegate LogAction;
        /// <summary>
        ///   Builds a pooled memory stream.
        /// </summary>
        /// <param name="capacity">The capacity of the backing stream.</param>
        public PooledMemoryStream(int capacity, OutputActionDelegate logAction)
        {
            LogAction = logAction;
            _trackedMemoryStream = new TrackedMemoryStream(capacity)
            {
                Parent = this
            };

            OnValidateObject += (ctx) =>
            {
                if (ctx.Direction == PooledObjectDirection.Outbound)
                {
                    // We validate only inbound objects, because when they are in the pool they
                    // cannot change their state.
                    return true;
                }

                if (!_trackedMemoryStream.CanRead || !_trackedMemoryStream.CanWrite || !_trackedMemoryStream.CanSeek)
                {
                    LogAction("[ObjectPool] Memory stream has already been disposed", null, MessageType.Warning);
                    return false;
                }

                var memoryStreamPool = PooledObjectInfo.Handle as IMemoryStreamPool;
                if (_trackedMemoryStream.Capacity < memoryStreamPool.MinimumMemoryStreamCapacity)
                {
                    LogAction($"[ObjectPool] Memory stream capacity is {_trackedMemoryStream.Capacity}, while minimum required capacity is {memoryStreamPool.MinimumMemoryStreamCapacity}", null, MessageType.Warning);
                     
                    return false;
                }
                if (_trackedMemoryStream.Capacity > memoryStreamPool.MaximumMemoryStreamCapacity)
                {
                    LogAction($"[ObjectPool] Memory stream capacity is {_trackedMemoryStream.Capacity}, while maximum allowed capacity is {memoryStreamPool.MaximumMemoryStreamCapacity}", null, MessageType.Warning);
                    
                    return false;
                }

                return true; // Object is valid.
            };

            OnResetState += (o) =>
            {
                _trackedMemoryStream.Position = 0L;
                _trackedMemoryStream.SetLength(0L);
            };

            OnReleaseResources += (o) =>
            {
                _trackedMemoryStream.Parent = null;
                _trackedMemoryStream.Dispose();
            };
        }

        /// <summary>
        ///   The memory stream.
        /// </summary>
        public MemoryStream MemoryStream => _trackedMemoryStream;

        /// <summary>
        ///   Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => _trackedMemoryStream.ToString();

        private sealed class TrackedMemoryStream : MemoryStream
        {
            public TrackedMemoryStream(int capacity)
                : base(capacity)
            {
            }

            public PooledMemoryStream Parent { get; set; }

            protected override void Dispose(bool disposing)
            {
                if (disposing && Parent != null)
                {
                    Parent.Dispose();
                }
                else
                {
                    base.Dispose(disposing);
                }
            }
        }
    }

    public interface IMemoryStreamPool : IObjectPool<PooledMemoryStream>
    {
        /// <summary>
        ///   Maximum capacity a <see cref="MemoryStream"/> might have in order to be able to return
        ///   to pool. Defaults to <see cref="MemoryStreamPool.DefaultMaximumMemoryStreamCapacity"/>.
        /// </summary>
        int MaximumMemoryStreamCapacity { get; set; }

        /// <summary>
        ///   Minimum capacity a <see cref="MemoryStream"/> should have when created and this is the
        ///   minimum capacity of all streams stored in the pool. Defaults to <see cref="MemoryStreamPool.DefaultMinimumMemoryStreamCapacity"/>.
        /// </summary>
        int MinimumMemoryStreamCapacity { get; set; }
    }

    public sealed class MemoryStreamPool : ObjectPool<PooledMemoryStream>, IMemoryStreamPool
    {
        /// <summary>
        ///   Default maximum memory stream capacity. Shared by all <see cref="IMemoryStreamPool"/>
        ///   instances, defaults to 512KB.
        /// </summary>
        public const int DefaultMaximumMemoryStreamCapacity = 512 * 1024;

        /// <summary>
        ///   Default minimum memory stream capacity. Shared by all <see cref="IMemoryStreamPool"/>
        ///   instances, defaults to 4KB.
        /// </summary>
        public const int DefaultMinimumMemoryStreamCapacity = 4 * 1024;

        /// <summary>
        ///   Backing field for <see cref="MaximumMemoryStreamCapacity"/>.
        /// </summary>
        private int _maximumItemCapacity = DefaultMaximumMemoryStreamCapacity;

        /// <summary>
        ///   Backing field for <see cref="MinimumMemoryStreamCapacity"/>
        /// </summary>
        private int _minimumItemCapacity = DefaultMinimumMemoryStreamCapacity;

        /// <summary>
        ///   Builds the specialized pool.
        /// </summary>
        public MemoryStreamPool(OutputActionDelegate logAction = null)
            : base(ObjectPool.DefaultPoolMaximumSize, (Func<PooledMemoryStream>)null)
        {
            FactoryMethod = () => new PooledMemoryStream(MinimumMemoryStreamCapacity, logAction);
        }

        /// <summary>
        ///   Thread-safe pool instance.
        /// </summary>
        public static IMemoryStreamPool Instance { get; } = new MemoryStreamPool();

        /// <summary>
        ///   Maximum capacity a <see cref="MemoryStream"/> might have in order to be able to return
        ///   to pool. Defaults to <see cref="DefaultMaximumMemoryStreamCapacity"/>.
        /// </summary>
        public int MaximumMemoryStreamCapacity
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
        ///   Minimum capacity a <see cref="MemoryStream"/> should have when created and this is the
        ///   minimum capacity of all streams stored in the pool. Defaults to <see cref="DefaultMinimumMemoryStreamCapacity"/>.
        /// </summary>
        public int MinimumMemoryStreamCapacity
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
    }

}
