using FluentResults;
using HtmlAgilityPack;
using MainCore.Commands;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Repositories;
using MainCore.Services;
using NavigateCore.Commands;
using NavigateCore.Parsers;
using OpenQA.Selenium;
using UpdateCore.Commands;

namespace UpgradeBuildingCore.Commands
{
    public class UseHeroResourceCommand : IUseHeroResourceCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroItemRepository _heroItemRepository;

        private readonly IToHeroInventoryCommand _toHeroInventoryCommand;
        private readonly IUpdateHeroItemsCommand _updateHeroItemsCommand;

        private readonly IClickCommand _clickCommand;
        private readonly IInputTextboxCommand _inputTextboxCommand;
        private readonly IWaitCommand _waitCommand;
        private readonly IDelayCommand _delayCommand;

        private readonly IHeroParser _heroParser;

        public UseHeroResourceCommand(IChromeManager chromeManager, IHeroItemRepository heroItemRepository, IToHeroInventoryCommand toHeroInventoryCommand, IUpdateHeroItemsCommand updateHeroItemsCommand, IClickCommand clickCommand, IWaitCommand waitCommand, IHeroParser heroParser, IInputTextboxCommand inputTextboxCommand, IDelayCommand delayCommand)
        {
            _chromeManager = chromeManager;
            _heroItemRepository = heroItemRepository;
            _toHeroInventoryCommand = toHeroInventoryCommand;
            _updateHeroItemsCommand = updateHeroItemsCommand;
            _clickCommand = clickCommand;
            _waitCommand = waitCommand;
            _heroParser = heroParser;
            _inputTextboxCommand = inputTextboxCommand;
            _delayCommand = delayCommand;
        }

        public async Task<Result> Execute(int accountId, long[] requiredResource)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var currentUrl = chromeBrowser.CurrentUrl;
            Result result;
            result = await _toHeroInventoryCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _updateHeroItemsCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            for (var i = 0; i < 4; i++)
            {
                requiredResource[i] = RoundUpTo100(requiredResource[i]);
            }

            result = await _heroItemRepository.IsEnoughResource(accountId, requiredResource);
            if (result.IsFailed)
            {
                if (!result.HasError<Retry>())
                {
                    chromeBrowser.Navigate(currentUrl);
                }
                return result.WithError(new Trace(Trace.TraceMessage()));
            }

            var items = new List<(HeroItemEnums, long)>()
            {
                (HeroItemEnums.Wood, requiredResource[0]),
                (HeroItemEnums.Clay, requiredResource[1]),
                (HeroItemEnums.Iron, requiredResource[2]),
                (HeroItemEnums.Crop, requiredResource[3]),
            };

            foreach (var item in items)
            {
                result = await UseResource(chromeBrowser, item.Item1, item.Item2);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                result = await _delayCommand.Execute(accountId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            }

            chromeBrowser.Navigate(currentUrl);
            return Result.Ok();
        }

        private async Task<Result> UseResource(IChromeBrowser chromeBrowser, HeroItemEnums item, long amount)
        {
            if (amount == 0) return Result.Ok();
            Result result;
            result = await ClickItem(chromeBrowser, item);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await EnterAmount(chromeBrowser, amount);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await Confirm(chromeBrowser);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private async Task<Result> ClickItem(IChromeBrowser chromeBrowser, HeroItemEnums item)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetItemSlot(html, item);
            if (node is null) return Result.Fail(Retry.NotFound($"{item}", "item"));

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private async Task<Result> EnterAmount(IChromeBrowser chromeBrowser, long amount)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetAmountBox(html);
            if (node is null) return Result.Fail(Retry.TextboxNotFound("amount input"));
            Result result;
            result = await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(node.XPath), amount.ToString());
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private async Task<Result> Confirm(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetConfirmButton(html);
            if (node is null) return Result.Fail(Retry.ButtonNotFound("Confirm"));

            Result result;
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(node.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private static long RoundUpTo100(long res)
        {
            var remainder = res % 100;
            return res + (100 - remainder);
        }
    }
}