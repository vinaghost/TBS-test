using FluentResults;

namespace MainCore.Features.Farming.Commands
{
    public interface ISendFarmListCommand
    {
        Task<Result> Execute(int accountId, int farmListId);
    }
}