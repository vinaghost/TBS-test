using FluentResults;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchTabCommand
    {
        Task<Result> Execute(int accountId, int index);
        Task<Result> Execute(IChromeBrowser chromeBrowser, int index);
    }
}