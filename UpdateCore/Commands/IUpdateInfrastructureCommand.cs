using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateInfrastructureCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}