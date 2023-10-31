using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchVillageCommand
    {
        Result Execute(AccountId accountId, VillageId villageId);
    }
}