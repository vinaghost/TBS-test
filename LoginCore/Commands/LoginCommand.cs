using LoginCore.Parser;
using MainCore;
using MainCore.Commands;
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

        private readonly ILoginPageParser _loginPageParser;

        public LoginCommand(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IClickButtonCommand clickButtonCommand, IInputTextboxCommand inputTextboxCommand, ILoginPageParser loginPageParser)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _clickButtonCommand = clickButtonCommand;
            _inputTextboxCommand = inputTextboxCommand;
            _loginPageParser = loginPageParser;
        }

        public async Task Execute(int accountId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts.FindAsync(accountId);
            var access = await context.Accesses.FirstOrDefaultAsync(x => x.AccountId == accountId);
            var chromeBrowser = _chromeManager.Get(accountId);

            var html = chromeBrowser.Html;
            var usernameNode = _loginPageParser.GetUsernameNode(html);
            var passwordNode = _loginPageParser.GetPasswordNode(html);
            var buttonNode = _loginPageParser.GetLoginButton(html);

            await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(usernameNode.XPath), account.Username);
            await _inputTextboxCommand.Execute(chromeBrowser, By.XPath(passwordNode.XPath), access.Password);
            await _clickButtonCommand.Execute(chromeBrowser, By.XPath(buttonNode.XPath));
        }
    }
}