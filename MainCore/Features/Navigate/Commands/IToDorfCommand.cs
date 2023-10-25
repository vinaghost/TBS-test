using FluentResults;
using MainCore.Entities;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToDorfCommand
    {
        Task<Result> Execute(AccountId accountId, int dorf);

        Task<Result> Execute(IChromeBrowser chromeBrowser, int dorf);
    }
}