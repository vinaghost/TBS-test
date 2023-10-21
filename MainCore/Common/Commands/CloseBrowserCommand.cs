using FluentResults;
using MainCore.Infrasturecture.Services;
using MediatR;

namespace MainCore.Common.Commands
{
    public class CloseBrowserCommand : IRequest<Result>
    {
        public IChromeBrowser Browser { get; }

        public CloseBrowserCommand(IChromeBrowser browser)
        {
            Browser = browser;
        }
    }

    public class CloseBrowserCommandHandler : IRequestHandler<CloseBrowserCommand, Result>
    {
        public async Task<Result> Handle(CloseBrowserCommand request, CancellationToken cancellationToken)
        {
            var driver = request.Browser.Driver;
            await Task.Run(driver.Close, cancellationToken);
            return Result.Ok();
        }
    }
}