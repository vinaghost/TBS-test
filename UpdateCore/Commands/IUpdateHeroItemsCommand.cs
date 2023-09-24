using FluentResults;

namespace UpdateCore.Commands
{
    public interface IUpdateHeroItemsCommand
    {
        Task<Result> Execute(int accountId);
    }
}