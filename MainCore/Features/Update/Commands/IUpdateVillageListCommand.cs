using FluentResults;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateVillageListCommand
    {
        Task<Result> Execute(int accountId);
    }
}