﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data.Pooling
{
    public interface IParameterizedObjectPool<in TKey, out TValue>
    {
        /// <summary>
        ///   Gets or sets the Diagnostics class for the current Object Pool, whose goal is to record
        ///   data about how the pool operates. By default, however, an object pool records anything,
        ///   in order to be most efficient; in any case, you can enable it through the <see
        ///   cref="ObjectPoolDiagnostics.Enabled"/> property.
        /// </summary>
        ObjectPoolDiagnostics Diagnostics { get; set; }

        /// <summary>
        ///   Gets the Factory method that will be used for creating new objects.
        /// </summary>
        Func<TKey, TValue> FactoryMethod { get; }

        /// <summary>
        ///   Gets the count of the keys currently handled by the pool.
        /// </summary>
        int KeysInPoolCount { get; }

        /// <summary>
        ///   Gets or sets the maximum number of objects that could be available at the same time in
        ///   the pool.
        /// </summary>
        int MaximumPoolSize { get; set; }

        /// <summary>
        ///   Gets an object linked to given key.
        /// </summary>
        /// <param name="key">The key linked to the object.</param>
        /// <returns>The objects linked to given key.</returns>
        TValue GetObject(TKey key);
    }


    public class ParameterizedObjectPool<TKey, TValue> : IParameterizedObjectPool<TKey, TValue>
       where TValue : PooledObject
    {
        #region Public Properties

        /// <summary>
        ///   Backing field for <see cref="Diagnostics"/>.
        /// </summary>
        private ObjectPoolDiagnostics _diagnostics;

        /// <summary>
        ///   Backing field for <see cref="MaximumPoolSize"/>.
        /// </summary>
        private int _maximumPoolSize;

        /// <summary>
        ///   Gets or sets the Diagnostics class for the current Object Pool, whose goal is to record
        ///   data about how the pool operates. By default, however, an object pool records anything,
        ///   in order to be most efficient; in any case, you can enable it through the <see
        ///   cref="ObjectPoolDiagnostics.Enabled"/> property.
        /// </summary>
        public ObjectPoolDiagnostics Diagnostics
        {
            get { return _diagnostics; }
            set
            {
                // Safe copy of the current pools.
                var innerPools = _pools.Values.Cast<ObjectPool<TValue>>().ToArray();

                _diagnostics = value;
                foreach (var p in innerPools)
                {
                    p.Diagnostics = value;
                }
            }
        }

        /// <summary>
        ///   Gets the Factory method that will be used for creating new objects.
        /// </summary>
        public Func<TKey, TValue> FactoryMethod { get; private set; }

        /// <summary>
        ///   Gets the count of the keys currently handled by the pool.
        /// </summary>
        public int KeysInPoolCount => _pools.Count;

        /// <summary>
        ///   Gets or sets the maximum number of objects that could be available at the same time in
        ///   the pool.
        /// </summary>
        // ReSharper disable once ConvertToAutoProperty
        public int MaximumPoolSize
        {
            get { return _maximumPoolSize; }
            set
            {
                // Preconditions
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value), ErrorMessages.NegativeOrZeroMaximumPoolSize);

                _maximumPoolSize = value;
            }
        }

        #endregion Public Properties

        #region C'tor and Initialization code

        /// <summary>
        ///   Initializes a new pool with default settings.
        /// </summary>
        public ParameterizedObjectPool()
            : this(ObjectPool.DefaultPoolMaximumSize, null)
        {
        }

        /// <summary>
        ///   Initializes a new pool with specified maximum pool size.
        /// </summary>
        /// <param name="maximumPoolSize">The maximum pool size limit.</param>
        public ParameterizedObjectPool(int maximumPoolSize)
            : this(maximumPoolSize, null)
        {
        }

        /// <summary>
        ///   Initializes a new pool with specified factory method.
        /// </summary>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        public ParameterizedObjectPool(Func<TKey, TValue> factoryMethod)
            : this(ObjectPool.DefaultPoolMaximumSize, factoryMethod)
        {
        }

        /// <summary>
        ///   Initializes a new pool with specified factory method and maximum size.
        /// </summary>
        /// <param name="maximumPoolSize">The maximum pool size limit.</param>
        /// <param name="factoryMethod">The factory method that will be used to create new objects.</param>
        public ParameterizedObjectPool(int maximumPoolSize, Func<TKey, TValue> factoryMethod)
        {
            // Preconditions
            if (maximumPoolSize < 1) throw new ArgumentOutOfRangeException(nameof(maximumPoolSize), ErrorMessages.NegativeOrZeroMaximumPoolSize);

            // Assigning properties
            Diagnostics = new ObjectPoolDiagnostics();
            FactoryMethod = factoryMethod;
            _maximumPoolSize = maximumPoolSize;
        }

        #endregion C'tor and Initialization code

        #region Pool Operations

        /// <summary>
        ///   Clears the parameterized pool and each inner pool stored inside it.
        /// </summary>
        public void Clear()
        {
            ClearPools();
        }

        /// <summary>
        ///   Gets an object linked to given key.
        /// </summary>
        /// <param name="key">The key linked to the object.</param>
        /// <returns>The objects linked to given key.</returns>
        public TValue GetObject(TKey key)
        {
            ObjectPool<TValue> pool;
            if (!TryGetPool(key, out pool))
            {
                // Initialize and insert the new pool.
                pool = AddPool(key);
            }

            Debug.Assert(pool != null);
            return pool.GetObject();
        }

        #endregion Pool Operations

        #region Low-level Pooling

        private readonly System.Collections.Hashtable _pools = new System.Collections.Hashtable();

        private ObjectPool<TValue> AddPool(TKey key)
        {
            // We are going to write, so we need full locking.
            lock (_pools)
            {
                ObjectPool<TValue> objectPool;
                if (!_pools.ContainsKey(key))
                {
                    _pools.Add(key, objectPool = new ObjectPool<TValue>(MaximumPoolSize, PrepareFactoryMethod(key))
                    {
                        Diagnostics = _diagnostics
                    });
                }
                else
                {
                    // Someone added the same pool in the meanwhile.
                    objectPool = _pools[key] as ObjectPool<TValue>;
                }
                return objectPool;
            }
        }

        private void ClearPools()
        {
            // Safe copy of the current pools.
            var innerPools = _pools.Values.Cast<ObjectPool<TValue>>().ToArray();

            // Clear the main pool.
            lock (_pools)
            {
                _pools.Clear();
            }

            // Then clear each pool, taking it from the safe copy.
            foreach (var innerPool in innerPools)
            {
                innerPool.Clear();
            }
        }

        private bool TryGetPool(TKey key, out ObjectPool<TValue> objectPool)
        {
            // Hashtable requires no locking for readers.
            objectPool = _pools[key] as ObjectPool<TValue>;
            return objectPool != null;
        }

        #endregion Low-level Pooling

        #region Private Methods

        private Func<TValue> PrepareFactoryMethod(TKey key)
        {
            var factory = FactoryMethod;
            if (factory == null)
            {
                // Use the default parameterless constructor.
                return null;
            }
            return () => factory(key);
        }

        #endregion Private Methods
    }

}
