 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pure.Data.SqlMap
{
   
    public class MemoryCacheProvider : ICacheProvider
    {
         #region Fields 
		private MemoryCacheLevel _cacheLevel = MemoryCacheLevel.Weak;
		private Hashtable _cache = null;
		#endregion

        public MemoryCacheProvider()
        {
            _cache = Hashtable.Synchronized(new Hashtable());
        }

        public bool Remove(CacheKey cacheKey)
        { 

            object value = null;
            object reference = this[cacheKey];
            _cache.Remove(cacheKey);
            //if (reference != null)
            //{
            //    if (reference is StrongReference)
            //    {
            //        value = ((StrongReference)reference).Target;
            //    }
            //    else if (reference is WeakReference)
            //    {
            //        value = ((WeakReference)reference).Target;
            //    }
            //}
            return true;
        }

        public void Flush()
        {
            lock (this)
            {
                _cache.Clear();
            }	
        }
        public object this[CacheKey cacheKey]
        {
            get
			{
				object value = null;
				object reference = _cache[cacheKey];
				if (reference != null) 
				{
					if (reference is StrongReference) 
					{
						value = ((StrongReference) reference).Target;
					} 
					else if (reference is WeakReference) 
					{
						value = ((WeakReference) reference).Target;
					}
				}				
				return value;
			}
			set
			{
				object reference = null;
				if (_cacheLevel.Equals(MemoryCacheLevel.Weak)) 
				{
					reference = new WeakReference(value);
				} 
				else if (_cacheLevel.Equals(MemoryCacheLevel.Strong)) 
				{
					reference = new StrongReference(value);
				}
				_cache[cacheKey] = reference;	
			
			}
             
        }
        public void Initialize(IDictionary properties)
        {
            string referenceType = (string)properties["Type"]; 
            if (referenceType != null)
            {
                _cacheLevel = MemoryCacheLevel.GetByRefenceType(referenceType.ToUpper());
            }
             
        }


      

	  
		/// <summary>
		/// Class to implement a strong (permanent) reference.
		/// </summary>
		private class StrongReference 
		{
			private object _target = null;

			public StrongReference(object obj) 
			{
				_target = obj;
			}

			/// <summary>
			/// Gets the object (the target) referenced by this instance.
			/// </summary>
			public object Target  
			{
				get
				{
					return _target ;
				}
			}
		}
    }
}
