using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToDorfCommand
    {
        Task<Result> Execute(AccountId accountId, int dorf);
    }
}