﻿using FluentResults;
using MainCore.Common.Errors;
using MainCore.Common.Models;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Parsers;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands.Step.UpgradeBuilding
{
    [RegisterAsTransient]
    public class ConstructCommand : IConstructCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;

        public ConstructCommand(IChromeManager chromeManager, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
        }

        public Result Execute(AccountId accountId, NormalBuildPlan plan)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var button = _unitOfParser.UpgradeBuildingParser.GetConstructButton(html, plan.Type);

            var result = chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = chromeBrowser.WaitPageChanged("dorf");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}