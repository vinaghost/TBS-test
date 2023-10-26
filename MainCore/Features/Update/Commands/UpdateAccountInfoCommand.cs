using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Entities;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    public class UpdateAccountInfoCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IAccountInfoParser _accountInfoParser;
        private readonly IAccountInfoRepository _accountInfoRepository;

        public UpdateAccountInfoCommand(IChromeManager chromeManager, IAccountInfoParser accountInfoParser, IAccountInfoRepository accountInfoRepository)
        {
            _chromeManager = chromeManager;
            _accountInfoParser = accountInfoParser;
            _accountInfoRepository = accountInfoRepository;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            await _accountInfoRepository.Update(accountId, dto);
            return Result.Ok();
        }
    }
}