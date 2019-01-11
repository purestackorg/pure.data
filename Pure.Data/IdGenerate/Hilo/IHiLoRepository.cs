namespace Pure.Data.Hilo
{
    public interface IHiLoRepository
    {
        void PrepareRepository();
        long GetNextHi();
    }
}
