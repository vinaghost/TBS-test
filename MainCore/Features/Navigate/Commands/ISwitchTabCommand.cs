using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchTabCommand
    {
        Task<Result> Execute(AccountId accountId, int index);
    }
}