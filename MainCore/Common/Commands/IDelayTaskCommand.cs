using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Commands
{
    public interface IDelayTaskCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}