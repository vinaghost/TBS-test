using FluentResults;

namespace MainCore.Features.Update.Commands
{
    public interface IUpdateHeroItemsCommand
    {
        Task<Result> Execute(int accountId);
    }
}