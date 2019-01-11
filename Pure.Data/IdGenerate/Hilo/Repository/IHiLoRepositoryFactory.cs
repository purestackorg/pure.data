namespace Pure.Data.Hilo
{
    public interface IHiLoRepositoryFactory
    {
        IHiLoRepository GetRepository(string entityName, IHiLoConfiguration config);
    }
}
