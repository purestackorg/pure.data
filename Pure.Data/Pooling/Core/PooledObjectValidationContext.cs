namespace Pure.Data.Pooling
{
    public enum PooledObjectDirection
    {
        /// <summary>
        ///   An object is returning to the pool.
        /// </summary>
        Inbound,

        /// <summary>
        ///   An object is getting out of the pool.
        /// </summary>
        Outbound
    }
    public  class PooledObjectValidationContext
    {
        /// <summary>
        ///   Whether an object is going out of the pool or into the pool.
        /// </summary>
        public PooledObjectDirection Direction { get; private set; }

        /// <summary>
        ///   The pooled object which has to be validated.
        /// </summary>
        public PooledObject PooledObject { get; private set; }

        /// <summary>
        ///   Info about the pooled object which has to be validated.
        /// </summary>
        public PooledObjectInfo PooledObjectInfo => PooledObject.PooledObjectInfo;

        /// <summary>
        ///   Used when an object is returning to the pool.
        /// </summary>
        /// <param name="pooledObject">The pooled object which has to be validated.</param>
        internal static PooledObjectValidationContext Inbound(PooledObject pooledObject) => new PooledObjectValidationContext
        {
            PooledObject = pooledObject,
            Direction = PooledObjectDirection.Inbound
        };

        /// <summary>
        ///   Used when an object is going out of the pool.
        /// </summary>
        /// <param name="pooledObject">The pooled object which has to be validated.</param>
        internal static PooledObjectValidationContext Outbound(PooledObject pooledObject) => new PooledObjectValidationContext
        {
            PooledObject = pooledObject,
            Direction = PooledObjectDirection.Outbound
        };
    }

    //public class PooledObjectValidationContext<T> where  T : class
    //{
         

    //    /// <summary>
    //    ///   The pooled object which has to be validated.
    //    /// </summary>
    //    public T PooledObject { get; private set; }
 
    //}
}
