using FluentResults;
using MainCore.Common.Commands;
using MainCore.Common.Errors;
using MainCore.Features.Login.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Features.Login.Commands
{
    [RegisterAsTransient]
    public class LoginCommand : ILoginCommand
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IChromeManager _chromeManager;

        private readonly IClickCommand _clickCommand;
        private readonly IInputTextboxCommand _inputTextboxCommand;
        private readonly IWaitCommand _waitCommand;

        private readonly ILoginPageParser _loginPageParser;

        public LoginCommand(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IClickCommand clickCommand, IInputTextboxCommand inputTextboxCommand, ILoginPageParser loginPageParser, IWaitCommand waitCommand)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _clickCommand = clickCommand;
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
            var buttonNode = _loginPageParser.GetLoginButton(html);
            if (buttonNode is null) return Retry.ButtonNotFound("login");
            var usernameNode = _loginPageParser.GetUsernameNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("username");
            var passwordNode = _loginPageParser.GetPasswordNode(html);
            if (passwordNode is null) return Retry.TextboxNotFound("password");

            Result result;
            result = await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _clickCommand.Execute(chromeBrowser, By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageChanged("dorf"));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = await _waitCommand.Execute(chromeBrowser, WaitCommand.PageLoaded);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}