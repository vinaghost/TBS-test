using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public class InputTextboxCommand : IRequest<Result>
    {
        public IChromeBrowser Browser { get; }
        public By By { get; }
        public string Content { get; }

        public InputTextboxCommand(IChromeBrowser browser, By by, string content)
        {
            Browser = browser;
            By = by;
            Content = content;
        }
    }

    public class InputTextboxCommandHandler : IRequestHandler<InputTextboxCommand, Result>
    {
        public async Task<Result> Handle(InputTextboxCommand request, CancellationToken cancellationToken)
        {
            var driver = request.Browser.Driver;
            var elements = driver.FindElements(request.By);
            if (elements.Count == 0) return Retry.ElementNotFound();

            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(() =>
            {
                element.SendKeys(Keys.Home);
                element.SendKeys(Keys.Shift + Keys.End);
                element.SendKeys(request.Content);
            }, cancellationToken);
            return Result.Ok();
        }
    }
}