﻿using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Parsers;
using MainCore.Services;
using OpenQA.Selenium;

namespace MainCore.Commands.Step.NPC
{
    public class OpenNPCDialogCommand : IOpenNPCDialogCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;

        public OpenNPCDialogCommand(IChromeManager chromeManager, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            await Task.CompletedTask;
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var button = _unitOfParser.MarketParser.GetExchangeResourcesButton(html);
            Result result;
            result = chromeBrowser.Click(By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            bool dialogShown(IWebDriver driver)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);

                return _unitOfParser.MarketParser.NPCDialogShown(doc);
            }

            result = chromeBrowser.Wait(dialogShown);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}