using FluentResults;

namespace MainCore.Features.InstantUpgrade.Commands
{
    public interface IInstantUpgradeCommand
    {
        Task<Result> Execute(int accountId);
    }
}