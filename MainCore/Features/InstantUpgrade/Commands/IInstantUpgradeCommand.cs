using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.InstantUpgrade.Commands
{
    public interface IInstantUpgradeCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}