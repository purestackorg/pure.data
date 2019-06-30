namespace Pure.Data.Pooling
{
    internal interface IObjectPoolHandle
    {
        void ReturnObjectToPool(PooledObject objectToReturnToPool, bool reRegisterForFinalization);
    }
}
