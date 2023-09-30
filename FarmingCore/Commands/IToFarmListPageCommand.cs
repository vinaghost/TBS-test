using FluentResults;

namespace FarmingCore.Commands
{
    public interface IToFarmListPageCommand
    {
        Task<Result> Execute(int accountId);
    }
}