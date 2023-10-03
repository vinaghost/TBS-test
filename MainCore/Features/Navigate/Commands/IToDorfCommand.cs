using FluentResults;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToDorfCommand
    {
        Task<Result> Execute(int accountId, int dorf);
    }
}