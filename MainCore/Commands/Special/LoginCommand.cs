using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Parsers;
using MainCore.Repositories;
using OpenQA.Selenium;

namespace MainCore.Commands.Special
{
    [RegisterAsTransient]
    public class LoginCommand : ILoginCommand
    {
        private readonly IChromeManager _chromeManager;
        private readonly IUnitOfParser _unitOfParser;
        private readonly IUnitOfRepository _unitOfRepository;

        public LoginCommand(IChromeManager chromeManager, IUnitOfRepository unitOfRepository, IUnitOfParser unitOfParser)
        {
            _chromeManager = chromeManager;
            _unitOfRepository = unitOfRepository;
            _unitOfParser = unitOfParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.Html;

            var resourceButton = _unitOfParser.NavigationBarParser.GetResourceButton(html);
            if (resourceButton is not null) return Result.Ok();

            var buttonNode = _unitOfParser.LoginPageParser.GetLoginButton(html);
            if (buttonNode is null) return Retry.ButtonNotFound("login");
            var usernameNode = _unitOfParser.LoginPageParser.GetUsernameNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("username");
            var passwordNode = _unitOfParser.LoginPageParser.GetPasswordNode(html);
            if (passwordNode is null) return Retry.TextboxNotFound("password");

            var username = await Task.Run(() => _unitOfRepository.AccountRepository.GetUsername(accountId));
            var password = await Task.Run(() => _unitOfRepository.AccountRepository.GetPassword(accountId));

            Result result;
            result = chromeBrowser.InputTextbox(By.XPath(usernameNode.XPath), username);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = chromeBrowser.InputTextbox(By.XPath(passwordNode.XPath), password);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = chromeBrowser.Click(By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = chromeBrowser.WaitPageChanged("dorf");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}