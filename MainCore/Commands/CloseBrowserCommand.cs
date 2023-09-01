using MainCore.Services;

namespace MainCore.Commands
{
    public class CloseBrowserCommand : ICloseBrowserCommand
    {
        private readonly IChromeManager _chromeManager;

        public CloseBrowserCommand(IChromeManager chromeManager)
        {
            _chromeManager = chromeManager;
        }

        public async Task Execute(int accountId)
        {
            var chrome = _chromeManager.Get(accountId);
            await Task.Run(chrome.Close);
        }
    }
}