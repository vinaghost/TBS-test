using FluentResults;
using MainCore.Entities;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchTabCommand
    {
        Task<Result> Execute(AccountId accountId, int index);

        Task<Result> Execute(IChromeBrowser chromeBrowser, int index);
    }
}