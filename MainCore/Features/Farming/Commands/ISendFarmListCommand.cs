using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Farming.Commands
{
    public interface ISendFarmListCommand
    {
        Task<Result> Execute(AccountId accountId, FarmListId farmlistId);
    }
}