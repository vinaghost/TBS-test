using FluentResults;

namespace NavigateCore.Commands
{
    public interface IToHeroInventoryCommand
    {
        Task<Result> Execute(int accountId);
    }
}