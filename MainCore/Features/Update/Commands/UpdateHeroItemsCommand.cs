using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateHeroItemsCommand : IUpdateHeroItemsCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroParser _heroParser;
        private readonly IHeroItemRepository _heroItemRepository;

        public UpdateHeroItemsCommand(IChromeManager chromeManager, IHeroParser heroParser, IHeroItemRepository heroItemRepository)
        {
            _chromeManager = chromeManager;
            _heroParser = heroParser;
            _heroItemRepository = heroItemRepository;
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
            await _heroItemRepository.Update(accountId, items);

            return Result.Ok();
        }
    }
}