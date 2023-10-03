using FluentResults;
using MainCore.Common.Repositories;
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

            var items = _heroParser.GetItems(html).ToList();
            items.ForEach(item => item.AccountId = accountId);
            await _heroItemRepository.Update(accountId, items);

            return Result.Ok();
        }
    }
}