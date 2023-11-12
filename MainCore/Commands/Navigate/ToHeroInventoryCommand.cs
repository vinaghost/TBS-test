﻿using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using OpenQA.Selenium;

namespace MainCore.Commands.Navigate
{
    [RegisterAsTransient]
    public class ToHeroInventoryCommand : IToHeroInventoryCommand
    {
        private readonly IUnitOfParser _unitOfParser;
        private readonly IChromeManager _chromeManager;

        public ToHeroInventoryCommand(IChromeManager chromeManager, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfParser = unitOfParser;
        }

        public Result Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;
            var avatar = _unitOfParser.HeroParser.GetHeroAvatar(html);
            if (avatar is null) return Result.Fail(Retry.ButtonNotFound("avatar hero"));

            Result result;
            result = chromeBrowser.Click(By.XPath(avatar.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = chromeBrowser.WaitPageChanged("hero");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            bool tabActived(IWebDriver driver)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                return _unitOfParser.HeroParser.InventoryTabActive(doc);
            };

            result = chromeBrowser.Wait(tabActived);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}