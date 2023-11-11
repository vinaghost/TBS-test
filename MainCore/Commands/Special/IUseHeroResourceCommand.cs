using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IUseHeroResourceCommand
    {
        Task<Result> Execute(AccountId accountId, long[] requiredResource);
    }
}