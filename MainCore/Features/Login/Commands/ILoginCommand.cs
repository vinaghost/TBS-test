using FluentResults;
using MainCore.Entities;

namespace MainCore.Features.Login.Commands
{
    public interface ILoginCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}