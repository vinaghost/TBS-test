namespace MainCore.Services
{
    public interface ITimerManager
    {
        void Shutdown();

        void Start(int accountId);
    }
}