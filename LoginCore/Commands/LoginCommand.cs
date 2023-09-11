using FluentResults;
using LoginCore.Parsers;
using MainCore;
using MainCore.Commands;
using MainCore.Errors;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace LoginCore.Commands
{
    public class LoginCommand : ILoginCommand
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;

        private readonly IClickButtonCommand _clickButtonCommand;
        private readonly IInputTextboxCommand _inputTextboxCommand;
        private readonly IWaitCommand _waitCommand;

        private readonly ILoginPageParser _loginPageParser;

        public LoginCommand(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IClickButtonCommand clickButtonCommand, IInputTextboxCommand inputTextboxCommand, ILoginPageParser loginPageParser, IWaitCommand waitCommand)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _clickButtonCommand = clickButtonCommand;
            _inputTextboxCommand = inputTextboxCommand;
            _loginPageParser = loginPageParser;
            _waitCommand = waitCommand;
        }

        public async Task<Result> Execute(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts.FindAsync(accountId);
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.Html;
            var usernameNode = _loginPageParser.GetUsernameNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("username");
            var passwordNode = _loginPageParser.GetPasswordNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("password");
            var buttonNode = _loginPageParser.GetLoginButton(html);
            if (buttonNode is null) return Retry.ButtonNotFound("login");

            Result result;
            result = await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _clickButtonCommand.Execute(chromeBrowser, By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(accountId, WaitCommand.PageChanged("dorf"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(accountId, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}