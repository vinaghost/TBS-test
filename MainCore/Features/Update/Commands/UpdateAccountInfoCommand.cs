using FluentResults;
using MainCore.Common.Repositories;
using MainCore.Common.Requests;
using MainCore.DTO;
using MainCore.Features.Update.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Features.Update.Commands
{
    [RegisterAsTransient]
    public class UpdateAccountInfoCommand : IUpdateAccountInfoCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IAccountInfoParser _accountInfoParser;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMediator _mediator;

        public UpdateAccountInfoCommand(IChromeManager chromeManager, IAccountInfoParser accountInfoParser, IAccountInfoRepository accountInfoRepository, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _accountInfoParser = accountInfoParser;
            _accountInfoRepository = accountInfoRepository;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId, IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.Html;
            var dto = _accountInfoParser.Get(html);
            var mapper = new AccountInfoMapper();

            var accountInfo = mapper.Map(accountId, dto);

            _accountInfoRepository.Update(accountId, accountInfo);
            await _mediator.Send(new AccountInfoUpdate(accountId));
            return Result.Ok();
        }

        public async Task<Result> Execute(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            return await Execute(accountId, chromeBrowser);
        }
    }
}