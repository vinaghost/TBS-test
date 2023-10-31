using FluentResults;
using MainCore.Entities;

namespace MainCore.Common.Commands
{
    public interface IDelayClickCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}