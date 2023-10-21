using FluentResults;
using HtmlAgilityPack;
using MainCore.Common.Commands;
using MainCore.Common.Enums;
using MainCore.Common.Errors;
using MainCore.Common.Repositories;
using MainCore.Features.Navigate.Commands;
using MainCore.Features.Navigate.Parsers;
using MainCore.Features.Update.Commands;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Features.UpgradeBuilding.Commands
{
    [RegisterAsTransient]
    public class UseHeroResourceCommand : IUseHeroResourceCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IHeroItemRepository _heroItemRepository;

        private readonly IToHeroInventoryCommand _toHeroInventoryCommand;

        private readonly IHeroParser _heroParser;
        private readonly IMediator _mediator;

        public UseHeroResourceCommand(IChromeManager chromeManager, IHeroItemRepository heroItemRepository, IToHeroInventoryCommand toHeroInventoryCommand, IHeroParser heroParser, IMediator mediator)
        {
            _chromeManager = chromeManager;
            _heroItemRepository = heroItemRepository;
            _toHeroInventoryCommand = toHeroInventoryCommand;
            _heroParser = heroParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(int accountId, long[] requiredResource)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var currentUrl = chromeBrowser.CurrentUrl;
            Result result;
            result = await _toHeroInventoryCommand.Execute(accountId);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await _mediator.Send(new UpdateHeroItemsCommand(accountId));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            for (var i = 0; i < 4; i++)
            {
                requiredResource[i] = RoundUpTo100(requiredResource[i]);
            }

            result = _heroItemRepository.IsEnoughResource(accountId, requiredResource);
            if (result.IsFailed)
            {
                if (!result.HasError<Retry>())
                {
                    chromeBrowser.Navigate(currentUrl);
                }
                return result.WithError(new TraceMessage(TraceMessage.Line()));
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
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
                result = await _mediator.Send(new DelayCommand(accountId));
                if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            }

            chromeBrowser.Navigate(currentUrl);
            return Result.Ok();
        }

        private async Task<Result> UseResource(IChromeBrowser chromeBrowser, HeroItemEnums item, long amount)
        {
            if (amount == 0) return Result.Ok();
            Result result;
            result = await ClickItem(chromeBrowser, item);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await EnterAmount(chromeBrowser, amount);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await Confirm(chromeBrowser);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        private async Task<Result> ClickItem(IChromeBrowser chromeBrowser, HeroItemEnums item)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetItemSlot(html, item);
            if (node is null) return Result.Fail(Retry.NotFound($"{item}", "item"));

            Result result;
            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(node.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            var loadingCompleted = new Func<IWebDriver, bool>(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });

            result = await _mediator.Send(new WaitCommand(chromeBrowser, loadingCompleted));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        private async Task<Result> EnterAmount(IChromeBrowser chromeBrowser, long amount)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetAmountBox(html);
            if (node is null) return Result.Fail(Retry.TextboxNotFound("amount input"));
            Result result;
            result = await _mediator.Send(new InputTextboxCommand(chromeBrowser, By.XPath(node.XPath), amount.ToString()));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            return Result.Ok();
        }

        private async Task<Result> Confirm(IChromeBrowser chromeBrowser)
        {
            var html = chromeBrowser.Html;
            var node = _heroParser.GetConfirmButton(html);
            if (node is null) return Result.Fail(Retry.ButtonNotFound("Confirm"));

            Result result;
            result = await _mediator.Send(new ClickCommand(chromeBrowser, By.XPath(node.XPath)));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            var loadingCompleted = new Func<IWebDriver, bool>(driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });

            result = await _mediator.Send(new WaitCommand(chromeBrowser, loadingCompleted));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }

        private static long RoundUpTo100(long res)
        {
            var remainder = res % 100;
            return res + (100 - remainder);
        }
    }
}