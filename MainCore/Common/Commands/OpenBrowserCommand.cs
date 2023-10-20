using FluentResults;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Common.Commands
{
    public class OpenBrowserCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public OpenBrowserCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class OpenBrowserCommandHandler : IRequestHandler<OpenBrowserCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly AppDbContext _context;

        public OpenBrowserCommandHandler(IChromeManager chromeManager, AppDbContext context)
        {
            _chromeManager = chromeManager;
            _context = context;
        }

        public async Task<Result> Handle(OpenBrowserCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var account = _context.Accounts.Find(accountId);
            var access = _context.Accesses.FirstOrDefault(x => x.AccountId == accountId);
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
            }, cancellationToken);
            return Result.Ok();
        }
    }
}