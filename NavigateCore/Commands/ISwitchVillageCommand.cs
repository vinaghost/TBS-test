using FluentResults;

namespace NavigateCore.Commands
{
    public interface ISwitchVillageCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}