using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Login.Parsers;
using MainCore.Features.Navigate.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Services;
using MainCore.Repositories;
using OpenQA.Selenium;

namespace MainCore.Features.Login.Commands
{
    [RegisterAsTransient]
    public class LoginCommand : ILoginCommand
    {
        private readonly IChromeManager _chromeManager;

        private readonly ILoginPageParser _loginPageParser;
        private readonly IAccountRepository _accountRepository;
        private readonly INavigationBarParser _navigationBarParser;

        public LoginCommand(IChromeManager chromeManager, ILoginPageParser loginPageParser, IAccountRepository accountRepository, INavigationBarParser navigationBarParser)
        {
            _chromeManager = chromeManager;
            _loginPageParser = loginPageParser;
            _accountRepository = accountRepository;
            _navigationBarParser = navigationBarParser;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.Html;

            var resourceButton = _navigationBarParser.GetResourceButton(html);
            if (resourceButton is not null) return Result.Ok();

            var buttonNode = _loginPageParser.GetLoginButton(html);
            if (buttonNode is null) return Retry.ButtonNotFound("login");
            var usernameNode = _loginPageParser.GetUsernameNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("username");
            var passwordNode = _loginPageParser.GetPasswordNode(html);
            if (passwordNode is null) return Retry.TextboxNotFound("password");

            var username = await Task.Run(() => _accountRepository.GetUsernameById(accountId));
            var password = await Task.Run(() => _accountRepository.GetPasswordById(accountId));

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