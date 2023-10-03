using FluentResults;

namespace MainCore.Features.Farming.Commands
{
    public interface IToFarmListPageCommand
    {
        Task<Result> Execute(int accountId);
    }
}