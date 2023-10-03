using FluentResults;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateStorageCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}