using FluentResults;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Common.Commands
{
    public class OpenBrowserCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public OpenBrowserCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class OpenBrowserCommandHandler : IRequestHandler<OpenBrowserCommand, Result>
    {
        private readonly AppDbContext _context;
        private readonly IChromeManager _chromeManager;

        public OpenBrowserCommandHandler(AppDbContext context, IChromeManager chromeManager)
        {
            _context = context;
            _chromeManager = chromeManager;
        }

        public async Task<Result> Handle(OpenBrowserCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var account = _context.Accounts.Find(accountId);
            var access = _context.Accesses.FirstOrDefault(x => x.AccountId == accountId);

            var accountMapper = new AccountMapper();
            var accountDto = accountMapper.Map(account);
            var accessMapper = new AccessMapper();
            var accessDto = accessMapper.Map(access);
            await Task.Run(() =>
            {
                try
                {
                    chromeBrowser.Setup(accountDto, accessDto);
                    chromeBrowser.Navigate(accountDto.Server);
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