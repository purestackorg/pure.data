using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data.Pooling
{
    public static class PooledObjectWrapper
    {
        /// <summary>
        ///   Wraps a given resource so that it can be put in the pool.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="resource">The resource to be wrapped.</param>
        /// <exception cref="ArgumentNullException">Given resource is null.</exception>
        /// <returns>A wrapper for given resource.</returns>
        public static PooledObjectWrapper<T> Create<T>(T resource) where T : class => new PooledObjectWrapper<T>(resource);
    }

    /// <summary>
    ///   PooledObject wrapper, for classes which cannot inherit from that class.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    [Serializable]
    public sealed class PooledObjectWrapper<T> : PooledObject where T : class
    {
        /// <summary>
        ///   Wraps a given resource so that it can be put in the pool.
        /// </summary>
        /// <param name="resource">The resource to be wrapped.</param>
        /// <exception cref="ArgumentNullException">Given resource is null.</exception>
        public PooledObjectWrapper(T resource)
        {
            InternalResource = resource ?? throw new ArgumentNullException(nameof(resource), ErrorMessages.NullResource);

            base.OnReleaseResource += (o) => OnReleaseResources?.Invoke(InternalResource);
            base.OnResetState += (o) => OnResetState?.Invoke(InternalResource);

            base.OnEvictResource += (o) => OnEvictResource?.Invoke(InternalResource);
            base.OnGetResource += (o) => OnGetResource?.Invoke(InternalResource);
            base.OnCreateResource += (o) => OnCreateResource?.Invoke(InternalResource);
            base.OnReturnResource += (o) => OnReturnResource?.Invoke(InternalResource);
        }
        //private static object olock = new object();
        //private T innernalResource = null;
        /// <summary>
        ///   The resource wrapped inside this class.
        /// </summary>
        public T InternalResource { get; }


        //public T InternalResource { get {
        //        if (innernalResource == null)
        //        {
        //            lock (olock)
        //            {
        //                if (innernalResource == null)
        //                {
        //                    innernalResource = CreateResource();
        //                    if (innernalResource == null)
        //                    {
        //                        throw new ArgumentNullException(nameof(innernalResource), ErrorMessages.NullResource);
        //                    }

        //                    return innernalResource;
        //                }
        //            }
        //        }

        //        return innernalResource;
        //    } }
        //public new Func<T> CreateResource { get; set; }

        /// <summary>
        ///   Triggered by the pool manager when there is no need for this object anymore.
        /// </summary>
        public new Action<T> OnReleaseResources { get; set; }

        /// <summary>
        ///   Triggered by the pool manager just before the object is being returned to the pool.
        /// </summary>
        public new Action<T> OnResetState { get; set; }

        public new Action<T> OnEvictResource { get; set; }
        public new Action<T> OnGetResource { get; set; }
        public new Action<T> OnCreateResource { get; set; }
        public new Action<T> OnReturnResource { get; set; }

    }
}
