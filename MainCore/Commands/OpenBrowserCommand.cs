using MainCore.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Commands
{
    public class OpenBrowserCommand : IOpenBrowserCommand
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;

        public OpenBrowserCommand(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
        }

        public async Task Execute(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var chromeBrowser = _chromeManager.Get(accountId);
            var account = await context.Accounts.FindAsync(accountId);
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            try
            {
                chromeBrowser.Setup(access);
                chromeBrowser.Navigate(account.Server);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}