using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateFieldCommand
    {
        Task<Result> Execute(int accountId, int villageId);
    }
}