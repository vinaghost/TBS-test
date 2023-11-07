using FluentResults;
using MainCore.Common.Notification;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainCore.Features.Update.Commands
{
    public class UpdateAccountInfoCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IAccountInfoParser _accountInfoParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public UpdateAccountInfoCommand(IChromeManager chromeManager, IAccountInfoParser accountInfoParser, IDbContextFactory<AppDbContext> contextFactory, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _accountInfoParser = accountInfoParser;
            _contextFactory = contextFactory;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            await Task.Run(() => Update(accountId, dto));
            await _mediator.Publish(new AccountInfoUpdated(accountId));
            return Result.Ok();
        }

        public void Update(AccountId accountId, AccountInfoDto dto)
        {
            using var context = _contextFactory.CreateDbContext();

            var dbAccountInfo = context.AccountsInfo
                .Where(x => x.AccountId == accountId.Value)
                .FirstOrDefault();

            if (dbAccountInfo is null)
            {
                var accountInfo = dto.ToEntity(accountId);
                context.Add(accountInfo);
            }
            else
            {
                dto.To(dbAccountInfo);
                context.Update(dbAccountInfo);
            }
            context.SaveChanges();
        }
    }
}