using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface ILoginCommand
    {
        Task<Result> Execute(AccountId accountId);
    }
}