using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Common.Requests;
using MainCore.DTO;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateHeroItemsCommand : IUpdateHeroItemsCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroParser _heroParser;
        private readonly IHeroItemRepository _heroItemRepository;
        private readonly IMediator _mediator;

        public UpdateHeroItemsCommand(IChromeManager chromeManager, IHeroParser heroParser, IHeroItemRepository heroItemRepository, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _heroParser = heroParser;
            _heroItemRepository = heroItemRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var dtos = _heroParser.GetItems(html);

            var mapper = new HeroItemMapper();

            var items = new List<HeroItem>();
            foreach (var dto in dtos)
            {
                var item = mapper.Map(accountId, dto);
                items.Add(item);
            }
            _heroItemRepository.Update(accountId, items);
            await _mediator.Send(new HeroItemUpdate(accountId));
            return Result.Ok();
        }
    }
}