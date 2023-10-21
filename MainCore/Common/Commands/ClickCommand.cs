using FluentResults;
using MainCore.Common.Errors;
using MainCore.Infrasturecture.Services;
using MediatR;
using OpenQA.Selenium;

namespace MainCore.Common.Commands
{
    public class ClickCommand : IRequest<Result>
    {
        public IChromeBrowser Browser { get; }
        public By By { get; }

        public ClickCommand(IChromeBrowser browser, By by)
        {
            Browser = browser;
            By = by;
        }
    }

    public class ClickCommandHandler : IRequestHandler<ClickCommand, Result>
    {
        public async Task<Result> Handle(ClickCommand request, CancellationToken cancellationToken)
        {
            var driver = request.Browser.Driver;
            var elements = driver.FindElements(request.By);
            if (elements.Count == 0) return Retry.ElementNotFound();
            var element = elements[0];
            if (!element.Displayed || !element.Enabled) return Retry.ElementNotClickable();

            await Task.Run(element.Click, cancellationToken);

            return Result.Ok();
        }
    }
}