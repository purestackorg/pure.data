namespace Pure.Data.Hilo // this should be available at the root namespace
{
    public interface IKeyGenerator<T>
    {
        /// <summary>
        /// 生成Hilo值
        /// </summary>
        /// <returns>An unique key.</returns>
        T GetKey();
    }
}
