using FluentResults;

namespace FarmingCore.Commands
{
    public interface ISendFarmListCommand
    {
        Task<Result> Execute(int accountId, int farmListId);
    }
}