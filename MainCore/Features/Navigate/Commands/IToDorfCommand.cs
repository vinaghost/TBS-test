using FluentResults;
using MainCore.Infrasturecture.Services;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToDorfCommand
    {
        Task<Result> Execute(int accountId, int dorf);
        Task<Result> Execute(IChromeBrowser chromeBrowser, int dorf);
    }
}