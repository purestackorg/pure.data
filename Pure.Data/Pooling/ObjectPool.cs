//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Threading;
//namespace Pure.Data.Pooling
//{
//    public interface IObjectPool<T> where T : class
//    {
//        T Get();

//        void Return(T obj);
//        int GetAvtiveCount();
//        List<T> GetAll();
//        ObjectWrapper<T>[] GetAllPoolObject();
//        ObjectPoolDiagnostics Diagnostics { get; }

//    }
//    public class ObjectPool<T> : IObjectPool<T> where T : class
//    {
//        private readonly ObjectWrapper<T>[] _items;
//        private readonly IPooledObjectPolicy<T> _policy;
//        private readonly bool _isDefaultPolicy;
//        private ObjectWrapper<T> _firstItem;
//        public ObjectPoolDiagnostics Diagnostics { get; set; }
//        public bool EnableDiagnostics;
//        public bool EnableEvent;
//        // This class was introduced in 2.1 to avoid the interface call where possible
//        // private readonly ObjectPolicy<T> _fastPolicy;
//        private readonly ConditionalWeakTable<T, ObjectWrapper<T>> _wrapperMap = new ConditionalWeakTable<T, ObjectWrapper<T>>();

//        public ObjectPool(IPooledObjectPolicy<T> policy)
//            : this(policy, Environment.ProcessorCount * 2)
//        {
//        }
//        public ObjectPool(IPooledObjectPolicy<T> policy, int maximumRetained)
//        {


//            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
//            // _fastPolicy = policy as ObjectPolicy<T>;
//            _isDefaultPolicy = IsDefaultPolicy();

//            // -1 due to _firstItem
//            _items = new ObjectWrapper<T>[maximumRetained - 1];

//            bool IsDefaultPolicy()
//            {
//                var type = policy.GetType();

//                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DefaultPooledObjectPolicy<>);
//            }
//            EnableDiagnostics = true;
//            EnableEvent = true;
//            Diagnostics = new ObjectPoolDiagnostics(EnableDiagnostics);
//        }
//        public int GetAvtiveCount()
//        {
//            return _items.Count(p => p != null) ;
//        }
//        public List<T> GetAll()
//        {
//            return _items.Select(p => p.Element).ToList();
//        }
//        public ObjectWrapper<T>[] GetAllPoolObject()
//        {
//            return _items;
//        }
//        public T Get()
//        {
//            var item = _firstItem;
//            if (item == null || Interlocked.CompareExchange(ref _firstItem, null, item) != item)
//            {
//                item = GetViaScan();
//            }
//            _firstItem = item;
//            SetLastGetInfo(item);
//            _wrapperMap.GetValue(item.Element, _ => item);
            
//            if (EnableEvent)
//            {
//                _policy.OnGetEvent(item);

//            }

//            return item.Element;
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        private ObjectWrapper<T> GetViaScan()
//        {
//            var items = _items;
//            for (var i = 0; i < items.Length; i++)
//            {
//                //var item = items[i].Element;
//                //if (item != null && Interlocked.CompareExchange(ref items[i].Element, null, item) == item)
//                //{
//                //    return item;
//                //}

//                var item = items[i];
//                if (item != null && Interlocked.CompareExchange(ref items[i], null, item) == item)
//                {
//                    Diagnostics.IncrementPoolObjectHitCount();
                  
//                    return item;
//                }
//            }

//            Diagnostics.IncrementPoolObjectMissCount();
//            return Create();
//        }

//        private void SetLastGetInfo(ObjectWrapper<T> obj)
//        {
//            //obj.LastGetThreadId = Thread.CurrentThread.ManagedThreadId;
//            obj.LastGetTime = DateTime.Now;
//        }
//        private void SetLastReturnInfo(ObjectWrapper<T> obj)
//        {
//            //obj.LastReturnThreadId = Thread.CurrentThread.ManagedThreadId;
//            obj.LastReturnTime = DateTime.Now;
//        }
//        private int defaultId = 0;
//        // Non-inline to improve its code quality as uncommon path
//        [MethodImpl(MethodImplOptions.NoInlining)]
//        //private T Create() => _fastPolicy?.Create() ?? _policy.Create();
//        private ObjectWrapper<T> Create()
//        {
//            Diagnostics.IncrementObjectsCreatedCount();
//            var obj = new ObjectWrapper<T>()
//            {
//                //Element = _fastPolicy?.Create() ?? _policy.Create(),
//                Element = _policy.Create(),
//                Id = Interlocked.Increment(ref defaultId),
//                //InvokeTimes = 1,
//                Pool = this,
//                CreateTime = DateTime.Now,
//                LastGetTime = DateTime.Now,
//                //LastGetThreadId = Thread.CurrentThread.ManagedThreadId
//            };

//            //_firstItem = obj; //第一次设置firstItem
//            if (EnableEvent)
//            {
//                _policy.OnCreateEvent(obj);
//            }
        
//            return obj;
//        }

//        public void Return(T obj)
//        {
//            //if (_isDefaultPolicy || (_fastPolicy?.Return(obj) ?? _policy.Return(obj)))
//            if (_isDefaultPolicy || _policy.Return(obj))
//            {
//                if (_wrapperMap.TryGetValue(obj, out var pooledObject))
//                {
//                    SetLastReturnInfo(pooledObject);

//                    if (_firstItem != null || Interlocked.CompareExchange(ref _firstItem, pooledObject, null) != null)
//                    {
//                        Diagnostics.IncrementReturnedToPoolCount(); 
//                        ReturnViaScan(pooledObject);

//                    }
//                    else
//                    {
//                        //无法找到有null空位的位置，连接池满了
//                        Diagnostics.IncrementPoolOverflowCount();
//                        DestroyPooledObject(pooledObject);
//                    }
//                }


//                //if (_firstItem != null || Interlocked.CompareExchange(ref _firstItem, obj, null) != null)
//                //{
//                //    ReturnViaScan(obj);
//                //}
//            }
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        //private void ReturnViaScan(T obj)
//        //{
//        //    var items = _items;
//        //    for (var i = 0; i < items.Length && Interlocked.CompareExchange(ref items[i].Element, obj, null) != null; ++i)
//        //    {
//        //    }
//        //}
//        private void ReturnViaScan(ObjectWrapper<T> obj)
//        {
//            var items = _items;
//            if (EnableEvent)
//            {
//                _policy.OnReturnEvent(obj);

//            }
//            for (var i = 0; i < items.Length && Interlocked.CompareExchange(ref items[i], obj, null) != null; ++i)
//            {
               
//                return;//优先填充为null的一个位置
//            }
//        }
//        protected void DestroyPooledObject(ObjectWrapper<T> obj)
//        {

//            Diagnostics.IncrementObjectsDestroyedCount();
//            if (EnableEvent)
//            {
//                _policy.OnDestroyEvent(obj);

//            }

//            GC.SuppressFinalize(obj);
//        }

//    }

//    // PERF: the struct wrapper avoids array-covariance-checks from the runtime when assigning to elements of the array.
//    [DebuggerDisplay("{Element}")]
//    public class ObjectWrapper<T> : IDisposable where T : class
//    {

//        public T Element;
//        public int Id;
//        public DateTime CreateTime;
//        public DateTime LastGetTime;
//        public DateTime LastReturnTime;
//        //public int LastGetThreadId;
//        //public int LastReturnThreadId;
//        //public long InvokeTimes;
//        public IObjectPool<T> Pool;
//        public void Dispose()
//        {
//            Pool?.Return(Element);
//        }

//        public override bool Equals(object obj)
//        {
//            var poolobject = obj as ObjectWrapper<T>;
//            return poolobject != null &&
//                     Element == poolobject.Element;
//        }
//        public override string ToString()
//        {
//            return $"ActiveCount: {this.Pool.GetAvtiveCount()}, Id: {this.Id}, Element: {this.Element} , Time(R/G): {this.LastReturnTime.ToString("yyyy-MM-dd HH:mm:ss:ms")}/{this.LastGetTime.ToString("yyyy-MM-dd HH:mm:ss:ms")}"
//                + "\r\n"+Pool.Diagnostics.ToString();
//        }
//    }
//}
