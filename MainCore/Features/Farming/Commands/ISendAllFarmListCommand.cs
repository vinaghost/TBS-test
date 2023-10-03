using FluentResults;

namespace MainCore.Features.Farming.Commands
{
    public interface ISendAllFarmListCommand
    {
        Task<Result> Execute(int accountId);
    }
}