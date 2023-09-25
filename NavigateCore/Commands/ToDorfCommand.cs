﻿using FluentResults;
using HtmlAgilityPack;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Services;
using NavigateCore.Parsers;
using OpenQA.Selenium;

namespace NavigateCore.Commands
{
    public class ToDorfCommand : IToDorfCommand
    {
        private readonly INavigationBarParser _navigationBarParser;
        private readonly IChromeManager _chromeManager;

        private readonly IClickCommand _clickCommand;
        private readonly IWaitCommand _waitCommand;

        public ToDorfCommand(INavigationBarParser navigationBarParser, IChromeManager chromeManager, IClickCommand clickCommand, IWaitCommand waitCommand)

        {
            _navigationBarParser = navigationBarParser;
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId, int dorf)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.Html;

            var button = GetButton(html, dorf);
            if (button is null) return Retry.ButtonNotFound($"dorf{dorf}");

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged($"dorf{dorf}"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private HtmlNode GetButton(HtmlDocument doc, int dorf)
        {
            return dorf switch
            {
                1 => _navigationBarParser.GetResourceButton(doc),
                2 => _navigationBarParser.GetBuildingButton(doc),
                _ => null,
            };
        }
    }
}