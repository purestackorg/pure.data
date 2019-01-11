 
using System;
using System.Collections;

namespace Pure.Data.SqlMap
{

    /// <summary>
    /// Summary description for MemoryCacheLevel.
    /// </summary>
    public class MemoryCacheLevel
    {
        #region Fields
        private static Hashtable _cacheLevelMap = new Hashtable();

        /// <summary>
        /// Constant for weak caching
        /// This cache model is probably the best choice in most cases. It will increase
        /// performance for popular results, but it will absolutely release the memory to
        /// be used in allocating other objects, assuming that the results are not currently
        /// in use.
        /// References an object while still allowing it to be garbage collected.
        /// </summary>
        public static MemoryCacheLevel Weak;

        /// <summary>
        /// Constant for strong caching.
        /// This cache model will guarantee that the results stay in memory until the cache 
        /// is explicitly flushed. This is ideal for results that are:
        /// <list>
        /// <item>very small</item>
        /// <item>absolutely static</item>
        /// <item>used very often</item>
        /// </list>
        /// The advantage is that performance will be very good for this particular query.
        /// The disadvantage is that if the memory used by these results is needed, then it
        /// will not be released to make room for other objects (possibly more important
        /// objects).
        /// </summary>
        public static MemoryCacheLevel Strong;

        private string _referenceType;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        /// 
        /// </summary>
        static MemoryCacheLevel()
        {
            Weak = new MemoryCacheLevel("WEAK");
            Strong = new MemoryCacheLevel("STRONG");

            _cacheLevelMap[Weak.ReferenceType] = Weak;
            _cacheLevelMap[Strong.ReferenceType] = Strong;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new instance of CacheLevel
        /// </summary>
        /// <param name="type">The type of the CacheLevel.</param>
        private MemoryCacheLevel(string type)
        {
            _referenceType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReferenceType
        {
            get
            {
                return _referenceType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceType"></param>
        /// <returns></returns>
        public static MemoryCacheLevel GetByRefenceType(string referenceType)
        {
            MemoryCacheLevel cacheLevel = (MemoryCacheLevel)_cacheLevelMap[referenceType];
            if (cacheLevel == null)
            {
                throw new ArgumentException("Error getting CacheLevel (reference type) for name: '" + referenceType + "'.");
            }
            return cacheLevel;
        }
        #endregion

    }
}
