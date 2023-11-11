using FluentResults;
using MainCore.Common.Models;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IConstructCommand
    {
        Result Execute(AccountId accountId, NormalBuildPlan plan);
    }
}