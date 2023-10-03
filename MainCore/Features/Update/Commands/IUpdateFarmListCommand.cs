using FluentResults;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateFarmListCommand
    {
        Task<Result> Execute(int accountId);
    }
}