using FluentResults;
using MainCore.Entities;

namespace MainCore.Commands.Special
{
    public interface IStartAllFarmListCommand
    {
        Result Execute(AccountId accountId);
    }
}