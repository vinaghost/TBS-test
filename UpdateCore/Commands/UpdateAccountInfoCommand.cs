﻿using FluentResults;
using MainCore.Enums;
using MainCore.Models;
using MainCore.Repositories;
using MainCore.Services;
using UpdateCore.Parsers;

namespace UpdateCore.Commands
{
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

            var accountInfo = new AccountInfo()
            {
                Tribe = TribeEnums.Any,
                HasPlusAccount = _accountInfoParser.HasPlusAccount(html),
                Gold = _accountInfoParser.GetGold(html),
                Silver = _accountInfoParser.GetSilver(html),
                AccountId = accountId,
            };
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