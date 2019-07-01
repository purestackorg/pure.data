using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Pure.Data.Pooling
{
    public sealed class PooledObjectBuffer<T> : IEnumerable<T>
         where T : PooledObject
    {
        private const MethodImplOptions TryInline = MethodImplOptions.AggressiveInlining;

        /// <summary>
        ///   Used as default value for <see cref="_pooledObjects"/>.
        /// </summary>
        private static readonly T[] NoObjects = new T[0];

        /// <summary>
        ///   The concurrent buffer containing pooled objects.
        /// </summary>
        private T[] _pooledObjects = NoObjects;

        /// <summary>
        ///   The maximum capacity of this buffer.
        /// </summary>
        public int Capacity => _pooledObjects.Length;

        /// <summary>
        ///   The number of objects stored in this buffer.
        /// </summary>
        public int Count
        {
            get
            {
                var count = 0;
                for (var i = 0; i < _pooledObjects.Length; ++i)
                {
                    if (_pooledObjects[i] != null) count++;
                }
                return count;
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _pooledObjects)
            {
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///   Resizes the buffer so that it fits to given capacity. If new capacity is smaller than
        ///   current capacity, then exceeding items are returned.
        /// </summary>
        /// <param name="newCapacity">The new capacity of this buffer.</param>
        /// <returns>All exceeding items.</returns>
        public IList<T> Resize(int newCapacity)
        {
            if (_pooledObjects == NoObjects)
            {
                _pooledObjects = new T[newCapacity];
                return NoObjects;
            }

            var currentCapacity = _pooledObjects.Length;
            if (currentCapacity == newCapacity)
            {
                // Nothing to do.
                return NoObjects;
            }

            IList<T> exceedingItems = NoObjects;
            if (currentCapacity > newCapacity)
            {
                for (var i = newCapacity; i < currentCapacity; ++i)
                {
                    ref var item = ref _pooledObjects[i];
                    if (item != null)
                    {
                        if (exceedingItems == NoObjects)
                        {
                            exceedingItems = new List<T> { item };
                        }
                        else
                        {
                            exceedingItems.Add(item);
                        }
                        item = null;
                    }
                }
            }

            Array.Resize(ref _pooledObjects, newCapacity);
            return exceedingItems;
        }

        /// <summary>
        ///   Tries to dequeue an object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Output pooled object.</param>
        /// <returns>True if <paramref name="pooledObject"/> has a value, false otherwise.</returns>
        [MethodImpl(TryInline)]
        public bool TryDequeue(out T pooledObject)
        {
            for (var i = 0; i < _pooledObjects.Length; i++)
            {
                var item = _pooledObjects[i];
                if (item != null && Interlocked.CompareExchange(ref _pooledObjects[i], null, item) == item)
                {
                    pooledObject = item;
                    return true;
                }
            }
            pooledObject = null;
            return false;
        }

        /// <summary>
        ///   Tries to enqueue given object into the buffer.
        /// </summary>
        /// <param name="pooledObject">Input pooled object.</param>
        /// <returns>True if there was enough space to enqueue given object, false otherwise.</returns>
        [MethodImpl(TryInline)]
        public bool TryEnqueue(T pooledObject)
        {
            for (var i = 0; i < _pooledObjects.Length; i++)
            {
                ref var item = ref _pooledObjects[i];
                if (item == null && Interlocked.CompareExchange(ref item, pooledObject, null) == null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Tries to remove given object from the buffer.
        /// </summary>
        /// <param name="pooledObject">Pooled object to be removed.</param>
        /// <returns>True if <paramref name="pooledObject"/> has been removed, false otherwise.</returns>
        [MethodImpl(TryInline)]
        public bool TryRemove(T pooledObject)
        {
            for (var i = 0; i < _pooledObjects.Length; i++)
            {
                var item = _pooledObjects[i];
                if (item != null && item == pooledObject && Interlocked.CompareExchange(ref _pooledObjects[i], null, item) == item)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
