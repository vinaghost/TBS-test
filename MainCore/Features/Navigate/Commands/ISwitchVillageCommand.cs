using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchVillageCommand
    {
        Task<Result> Execute(AccountId accountId, VillageId villageId);
    }
}