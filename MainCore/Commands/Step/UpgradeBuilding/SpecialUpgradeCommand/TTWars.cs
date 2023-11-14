﻿using FluentResults;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using OpenQA.Selenium;

namespace MainCore.Commands.Step.UpgradeBuilding.SpecialUpgradeCommand
{
    [RegisterAsTransient(ServerEnums.TTWars)]
    public class TTWars : ISpecialUpgradeCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;

        public TTWars(IChromeManager chromeManager, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var button = _unitOfParser.UpgradeBuildingParser.GetSpecialUpgradeButton(html);
            var result = chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            await Task.CompletedTask;
            return Result.Ok();
        }
    }
}