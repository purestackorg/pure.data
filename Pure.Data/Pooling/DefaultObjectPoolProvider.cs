//using System;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading;
//namespace Pure.Data.Pooling
//{
//    public abstract class ObjectPoolProvider
//    {
//        public IObjectPool<T> Create<T>() where T : class, new()
//        {
//            return Create<T>(new DefaultPooledObjectPolicy<T>());
//        }

//        public abstract IObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy) where T : class;
//    }
//    public class DefaultObjectPoolProvider : ObjectPoolProvider
//    {
//        public int MaximumRetained { get; set; } = Environment.ProcessorCount * 2;

//        public override IObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
//        {
//            return new ObjectPool<T>(policy, MaximumRetained);
//        }
//    }
//}
