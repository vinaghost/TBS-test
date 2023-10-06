using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Features.Update.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateAccountInfoCommand : IUpdateAccountInfoCommand
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

        public async Task<Result> Execute(int accountId, IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            var mapper = new AccountInfoMapper();

            var accountInfo = mapper.Map(accountId, dto);

            await _accountInfoRepository.Update(accountId, accountInfo);
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(accountId, chromeBrowser);
        }
    }
}