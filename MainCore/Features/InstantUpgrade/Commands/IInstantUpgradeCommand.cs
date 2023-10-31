using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.InstantUpgrade.Commands
{
    public interface IInstantUpgradeCommand
    {
        Result Execute(AccountId accountId);
    }
}