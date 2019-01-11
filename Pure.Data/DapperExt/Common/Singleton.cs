using System;
using System.Collections.Generic;

namespace Pure.Data
{
	/// <summary>
/// A base class for the singleton design pattern.
/// </summary>
/// <typeparam name="T">Class type of the singleton</typeparam>
	public abstract class Singleton<T> where T : class
	{
	  #region Members
	
	  /// <summary>
	  /// Static instance. Needs to use lambda expression
	  /// to construct an instance (since constructor is private).
	  /// </summary>
	  private static readonly Lazy<T> sInstance = new Lazy<T>(() => CreateInstanceOfT());
	
	  #endregion
	
	  #region Properties
	
	  /// <summary>
	  /// Gets the instance of this singleton.
	  /// </summary>
	  public static T Instance { get { return sInstance.Value; } }
	
	  #endregion
	
	  #region Methods
	
	  /// <summary>
	  /// Creates an instance of T via reflection since T's constructor is expected to be private.
	  /// </summary>
	  /// <returns></returns>
	  private static T CreateInstanceOfT()
	  {
	    return Activator.CreateInstance(typeof(T), true) as T;
	  }
	
	  #endregion
	}


    ///// <summary>
    ///// Provides a singleton list for a certain type.
    ///// </summary>
    ///// <typeparam name="T">The type of list to store.</typeparam>
    //public class SingletonList<T> : Singleton<IList<T>>
    //{
    //    static SingletonList()
    //    {
    //        Singleton<IList<T>>.Instance = new List<T>();
    //    }

    //    /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this list for each type of T.</summary>
    //    public new static IList<T> Instance
    //    {
    //        get { return Singleton<IList<T>>.Instance; }
    //    }
    //}

    ///// <summary>
    ///// Provides a singleton dictionary for a certain key and vlaue type.
    ///// </summary>
    ///// <typeparam name="TKey">The type of key.</typeparam>
    ///// <typeparam name="TValue">The type of value.</typeparam>
    //public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    //{
    //    static SingletonDictionary()
    //    {
    //        Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
    //    }

    //    /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this dictionary for each type of T.</summary>
    //    public new static IDictionary<TKey, TValue> Instance
    //    {
    //        get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
    //    }
    //}

    ///// <summary>
    ///// Provides access to all "singletons" stored by <see cref="Singleton{T}"/>.
    ///// </summary>
    //public class Singleton
    //{
    //    static Singleton()
    //    {
    //        allSingletons = new Dictionary<Type, object>();
    //    }

    //    static readonly IDictionary<Type, object> allSingletons;

    //    /// <summary>Dictionary of type to singleton instances.</summary>
    //    public static IDictionary<Type, object> AllSingletons
    //    {
    //        get { return allSingletons; }
    //    }
    //}
    ///// <summary>
    ///// A statically compiled "singleton" used to store objects throughout the 
    ///// lifetime of the app domain. Not so much singleton in the pattern's 
    ///// sense of the word as a standardized way to store single instances.
    ///// </summary>
    ///// <typeparam name="T">The type of object to store.</typeparam>
    ///// <remarks>Access to the instance is not synchrnoized.</remarks>
    //public class Singleton<T> : Singleton
    //{
    //    static T instance;

    //    /// <summary>The singleton instance for the specified type T. Only one instance (at the time) of this object for each type of T.</summary>
    //    public static T Instance
    //    {
    //        get { return instance; }
    //        set
    //        {
    //            instance = value;
    //            AllSingletons[typeof(T)] = value;
    //        }
    //    }
    //}

}

