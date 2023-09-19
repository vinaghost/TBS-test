using FluentResults;

namespace NavigateCore.Commands
{
    public interface IToBuildingCommand
    {
        Task<Result> Execute(int accountId, int location);
    }
}