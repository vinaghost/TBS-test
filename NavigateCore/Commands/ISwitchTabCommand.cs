using FluentResults;
using MainCore.Services;

namespace NavigateCore.Commands
{
    public interface ISwitchTabCommand
    {
        Task<Result> Execute(int accountId, int index);
        Task<Result> Execute(IChromeBrowser chromeBrowser, int index);
    }
}