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
    public class UpdateHeroItemsCommand : IRequest<Result>
    {
        public AccountId AccountId { get; }

        public UpdateHeroItemsCommand(AccountId accountId)
        {
            AccountId = accountId;
        }
    }

    public class UpdateHeroItemsCommandCommandHandler : IRequestHandler<UpdateHeroItemsCommand, Result>
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroParser _heroParser;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMediator _mediator;

        public UpdateHeroItemsCommandCommandHandler(IChromeManager chromeManager, IDbContextFactory<AppDbContext> contextFactory, IHeroParser heroParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _contextFactory = contextFactory;
            _heroParser = heroParser;
            _mediator = mediator;
        }

        public async Task<Result> Handle(UpdateHeroItemsCommand request, CancellationToken cancellationToken)
        {
            var accountId = request.AccountId;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dtos = _heroParser.GetItems(html);
            await Task.Run(() => Update(accountId, dtos.ToList()), cancellationToken);
            await _mediator.Publish(new HeroItemUpdated(accountId), cancellationToken);
            return Result.Ok();
        }

        private void Update(AccountId accountId, List<HeroItemDto> dtos)
        {
            using var context = _contextFactory.CreateDbContext();
            var dbHeroItems = context.HeroItems
                .Where(x => x.AccountId == accountId.Value)
                .ToList();

            foreach (var dto in dtos)
            {
                var dbHeroItem = dbHeroItems
                    .FirstOrDefault(x => x.Type == dto.Type);
                if (dbHeroItem is null)
                {
                    var heroItem = dto.ToEntity(accountId);
                    context.Add(heroItem);
                }
                else
                {
                    dto.To(dbHeroItem);
                    context.Update(dbHeroItem);
                    dbHeroItems.Remove(dbHeroItem);
                }
            }
            context.SaveChanges();

            var removedHeroItems = dbHeroItems.Select(x => x.Type).AsEnumerable();
            context.HeroItems
                .Where(x => x.AccountId == accountId.Value)
                .Where(x => removedHeroItems.Contains(x.Type))
                .ExecuteDelete();
        }
    }
}