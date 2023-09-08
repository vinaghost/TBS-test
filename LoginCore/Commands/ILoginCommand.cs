using FluentResults;

namespace LoginCore.Commands
{
    public interface ILoginCommand
    {
        Task<Result> Execute(int accountId);
    }
}