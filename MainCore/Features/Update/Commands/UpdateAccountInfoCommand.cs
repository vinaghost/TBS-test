using FluentResults;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateAccountInfoCommand : IRequest<Result>
    {
        public int AccountId { get; }

        public UpdateAccountInfoCommand(int accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateAccountInfoCommandHandler : IRequestHandler<UpdateAccountInfoCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IAccountInfoParser _accountInfoParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UpdateAccountInfoCommandHandler(IChromeManager chromeManager, IAccountInfoParser accountInfoParser, IDbContextFactory<AppDbContext> contextFactory)
        {
            _chromeManager = chromeManager;
            _accountInfoParser = accountInfoParser;
            _contextFactory = contextFactory;
        }

        public async Task<Result> Handle(UpdateAccountInfoCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            await Task.Run(() => Update(accountId, dto), cancellationToken);
            return Result.Ok();
        }

        private void Update(int accountId, AccountInfoDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var dbAccountInfo = context.AccountsInfo
                .FirstOrDefault(x => x.AccountId == accountId);

            var mapper = new AccountInfoMapper();
            if (dbAccountInfo is null)
            {
                var accountInfo = mapper.Map(accountId, dto);
                context.Add(accountInfo);
            }
            else
            {
                mapper.MapToEntity(dto, dbAccountInfo);
                context.Update(dbAccountInfo);
            }
            context.SaveChanges();
        }
    }
}