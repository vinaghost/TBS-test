using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToDorfCommand
    {
        Result Execute(AccountId accountId, int dorf);
    }
}