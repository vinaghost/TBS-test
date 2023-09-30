using FluentResults;

namespace FarmingCore.Commands
{
    public interface ISendAllFarmListCommand
    {
        Task<Result> Execute(int accountId);
    }
}