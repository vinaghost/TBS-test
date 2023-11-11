using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IUpgradeAdsCommand
    {
        Task<Result> Execute(AccountId accountId, NormalBuildPlan plan);
    }
}