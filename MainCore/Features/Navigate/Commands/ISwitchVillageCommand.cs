using FluentResults;

namespace MainCore.Features.Navigate.Commands
{
    public interface ISwitchVillageCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}