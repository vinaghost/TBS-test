using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Farming.Commands
{
    public interface ISendAllFarmListCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}