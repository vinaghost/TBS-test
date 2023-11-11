﻿using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using OpenQA.Selenium;

namespace MainCore.Commands.Navigate
{
    [RegisterAsTransient]
    public class SwitchVillageCommand : ISwitchVillageCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;

        public SwitchVillageCommand(IChromeManager chromeManager, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
        }

        public Result Execute(AccountId accountId, VillageId villageId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var node = _unitOfParser.VillagePanelParser.GetVillageNode(html, villageId);
            if (node is null) return Skip.VillageNotFound;

            if (_unitOfParser.VillagePanelParser.IsActive(node)) return Result.Ok();

            Result result;
            result = chromeBrowser.Click(By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = chromeBrowser.WaitPageChanged($"{villageId}");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }
    }
}