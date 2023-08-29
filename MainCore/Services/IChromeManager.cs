namespace MainCore.Services
{
    public interface IChromeManager
    {
        IChromeBrowser Get(int accountId);
        void LoadExtension();
        void Shutdown();
    }
}