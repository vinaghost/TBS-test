using FluentResults;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToBuildingCommand
    {
        Task<Result> Execute(int accountId, int location);
    }
}