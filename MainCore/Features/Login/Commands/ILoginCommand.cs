using FluentResults;

namespace MainCore.Features.Login.Commands
{
    public interface ILoginCommand
    {
        Task<Result> Execute(int accountId);
    }
}