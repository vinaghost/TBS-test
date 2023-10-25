using FluentResults;
using MainCore.Common.Errors;
using MainCore.Entities;
using MainCore.Features.Login.Parsers;
using MainCore.Infrasturecture.AutoRegisterDi;
using MainCore.Infrasturecture.Persistence;
using MainCore.Infrasturecture.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Features.Login.Commands
{
    [RegisterAsTransient]
    public class LoginCommand : ILoginCommand
    {
        private readonly AppDbContext _context;
        private readonly IChromeManager _chromeManager;

        private readonly IMediator _mediator;

        private readonly ILoginPageParser _loginPageParser;

        public LoginCommand(AppDbContext context, IChromeManager chromeManager, ILoginPageParser loginPageParser, IMediator mediator)
        {
            _context = context;
            _chromeManager = chromeManager;
            _loginPageParser = loginPageParser;
            _mediator = mediator;
        }

        public async Task<Result> Execute(AccountId accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            var access = await _context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.Html;
            var buttonNode = _loginPageParser.GetLoginButton(html);
            if (buttonNode is null) return Retry.ButtonNotFound("login");
            var usernameNode = _loginPageParser.GetUsernameNode(html);
            if (usernameNode is null) return Retry.TextboxNotFound("username");
            var passwordNode = _loginPageParser.GetPasswordNode(html);
            if (passwordNode is null) return Retry.TextboxNotFound("password");

            Result result;
            result = await chromeBrowser.InputTextbox(By.XPath(usernameNode.XPath), account.Username);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.InputTextbox(By.XPath(passwordNode.XPath), access.Password);
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.Click(By.XPath(buttonNode.XPath));
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            result = await chromeBrowser.WaitPageChanged("dorf");
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));
            result = await chromeBrowser.WaitPageLoaded();
            if (result.IsFailed) return result.WithError(new TraceMessage(TraceMessage.Line()));

            return Result.Ok();
        }
    }
}