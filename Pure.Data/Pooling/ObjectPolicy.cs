//using System;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading;
//namespace Pure.Data.Pooling
//{
//    public interface IPooledObjectPolicy<T> where T : class 
//    {
//        T Create();

//        bool Return(T obj);

//        void OnCreateEvent(ObjectWrapper<T> obj);
//        void OnGetEvent(ObjectWrapper<T> obj);
//        void OnReturnEvent(ObjectWrapper<T> obj);
//        void OnDestroyEvent(ObjectWrapper<T> obj);
//    }
//    //public abstract class ObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, new()
//    //{
//    //    public abstract T Create();

//    //    public abstract bool Return(T obj);
//    //}

//    public class DefaultPooledObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, new()
//    {
//        public   T Create()
//        {
//            return new T();
//        }


//        // DefaultObjectPool<T> doesn't call 'Return' for the default policy.
//        // So take care adding any logic to this method, as it might require changes elsewhere.
//        public   bool Return(T obj)
//        {
//            return true;
//        }


//        public void OnCreateEvent(ObjectWrapper<T> obj)
//        {
            
//        }

//        public void OnDestroyEvent(ObjectWrapper<T> obj)
//        {
            
//        }

//        public void OnGetEvent(ObjectWrapper<T> obj)
//        {
             
//        }

//        public void OnReturnEvent(ObjectWrapper<T> obj)
//        {
 
//        }
//    }
//}
