using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateStorageCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}