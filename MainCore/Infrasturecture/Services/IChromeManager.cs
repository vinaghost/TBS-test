using MainCore.Entities;

namespace MainCore.Infrasturecture.Services
{
    public interface IChromeManager
    {
        IChromeBrowser Get(AccountId accountId);

        void LoadExtension();

        void Shutdown();
    }
}