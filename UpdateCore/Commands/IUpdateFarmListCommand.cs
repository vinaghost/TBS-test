using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateFarmListCommand
    {
        Task<Result> Execute(int accountId);
    }
}