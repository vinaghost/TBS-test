using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToHeroInventoryCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}