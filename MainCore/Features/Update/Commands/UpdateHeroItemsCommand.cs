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
            var query = context.HeroItems.Where(x => x.AccountId == accountId);
            var types = query
                .Select(x => x.Type)
                .ToList();

            var dbHeroItemss = query.ToList();

            var mapper = new HeroItemMapper();
            foreach (var dto in dtos)
            {
                var dbHeroItems = dbHeroItemss.FirstOrDefault(x => x.Type == dto.Type);
                if (dbHeroItems is null)
                {
                    var HeroItems = mapper.Map(accountId, dto);
                    context.Add(HeroItems);
                }
                else
                {
                    mapper.MapToEntity(dto, dbHeroItems);
                    context.Update(dbHeroItems);
                }

                types.Remove(dto.Type);
            }
            context.SaveChanges();

            context.HeroItems
                .Where(x => x.AccountId == accountId)
                .Where(x => types.Contains(x.Type))
                .ExecuteDelete();
        }
    }
}