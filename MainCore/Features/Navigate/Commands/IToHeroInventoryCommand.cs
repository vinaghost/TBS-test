using FluentResults;

namespace MainCore.Features.Navigate.Commands
{
    public interface IToHeroInventoryCommand
    {
        Task<Result> Execute(int accountId);
    }
}