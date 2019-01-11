namespace Pure.Data.Hilo
{
    /// <summary>
    /// Contract for a objetc that holds all configuration needed by NHiLo to work.
    /// </summary>
    public interface IHiLoConfiguration
    {
        bool CreateHiLoStructureIfNotExists { get; set; }
        int DefaultMaxLo { get; set; }
        IEntityConfiguration GetEntityConfig(string entityName);
        string TableName{ get; set; }
        string NextHiColumnName{ get; set; }
        string EntityColumnName{ get; set; }
        HiLoStorageType StorageType { get; set; }
        string ObjectPrefix { get; set; }
    }
}
