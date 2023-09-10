using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateVillageListCommand
    {
        Task<Result> Execute(int accountId);
    }
}