using MainCore.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;

        public LoginCommand(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
        }

        public async Task Execute(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts.FindAsync(accountId);
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            var chromeBrowser = _chromeManager.Get(accountId);
            try
            {
                chromeBrowser.Setup(access);
                //chromeBrowser.Navigate(account.Server);
                chromeBrowser.Navigate("https://nowsecure.nl");
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}