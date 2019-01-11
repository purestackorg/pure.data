

namespace Pure.Data.Pooling.Proxy
{
    interface ProxyCreator<T>
    {
        T CreateProxy(T pooledObject, IUsageTracking<T> tracker);

        T ResolveProxy(T proxy);
    }
}