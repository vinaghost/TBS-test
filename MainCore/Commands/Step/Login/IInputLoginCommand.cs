using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Step.Login
{
    public interface IInputLoginCommand
    {
        Result Execute(AccountId accountId);
    }
}