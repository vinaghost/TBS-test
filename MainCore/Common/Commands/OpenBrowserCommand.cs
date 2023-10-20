using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Common.Commands
{
    [RegisterAsTransient]
    public class OpenBrowserCommand : IOpenBrowserCommand
    {
        private readonly AppDbContext _context;
        private readonly IChromeManager _chromeManager;

        public OpenBrowserCommand(AppDbContext context, IChromeManager chromeManager)
        {
            _context = context;
            _chromeManager = chromeManager;
        }

        public async Task Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var account = await _context.Accounts.FindAsync(accountId);
            var access = await _context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            await Task.Run(() =>
            {
                try
                {
                    chromeBrowser.Setup(access);
                    chromeBrowser.Navigate(account.Server);
                }
                catch
                {
                    return;
                }
            });
        }
    }
}