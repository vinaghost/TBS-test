using FluentResults;

namespace NavigateCore.Commands
{
    public interface IToDorfCommand
    {
        Task<Result> Execute(int accountId, int dorf);
    }
}