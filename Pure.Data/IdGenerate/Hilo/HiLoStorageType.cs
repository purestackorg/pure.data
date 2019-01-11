namespace Pure.Data.Hilo
{
    /// <summary>
    /// 存储类型
    /// </summary>
    /// <remarks>No all DBMS can implement all these storage types.</remarks>
    public enum HiLoStorageType : int
    {
        /// <summary>
        /// 存储在数据表中
        /// </summary>
        Table = 0,
        /// <summary>
        /// 存储在序列中，如（oracle）
        /// </summary>
        Sequence = 1
    }
}
