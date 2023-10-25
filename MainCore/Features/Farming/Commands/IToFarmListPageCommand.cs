using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Farming.Commands
{
    public interface IToFarmListPageCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}